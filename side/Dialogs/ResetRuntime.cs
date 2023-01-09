using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class ResetRuntime : Form
	{
		public ResetRuntime()
		{
			this.InitializeComponent();
		}

		private void ResetRuntime_Shown(object sender, EventArgs e)
		{
			try
			{
				Application.DoEvents();
				base.Enabled = false;
				this.ChipMethod.SelectedIndex = 0;
				this.StateLabel.Text = "ロード中...";
				this.StateLabel.Refresh();
				if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
				{
					MessageBox.Show("ランタイムフォルダが見つかりません。" + Environment.NewLine + "Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					base.DialogResult = DialogResult.Cancel;
					base.Close();
				}
				else
				{
					string[] files = Directory.GetFiles(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir), "*.xml", SearchOption.TopDirectoryOnly);
					foreach (string text in files)
					{
						try
						{
							string text2 = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
							if (Directory.Exists(text2))
							{
								Runtime runtime = Runtime.ParseXML(text);
								string[] array2 = Runtime.CheckFiles(text2, runtime);
								if (array2.Length == 0)
								{
									this.runtimes.Add(text);
									this.runtimedatas.Add(runtime);
									this.RuntimeView.Items.Add(new ListViewItem(new string[]
									{
										runtime.Definitions.Name,
										runtime.Definitions.Author,
										(runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×"
									}));
								}
								else
								{
									MessageBox.Show(string.Concat(new string[]
									{
										"必須ファイルが欠落しています。",
										Environment.NewLine,
										Path.GetFileName(text),
										" : ",
										string.Join(",", array2)
									}), "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								}
							}
							else
							{
								MessageBox.Show("ランタイムフォルダが見つかりません:" + Path.GetFileName(text), "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show("読み込めませんでした:" + Path.GetFileName(text) + Environment.NewLine + ex.Message, "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
					}
					if (this.RuntimeView.Items.Count == 0)
					{
						MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						base.DialogResult = DialogResult.Cancel;
						base.Close();
					}
				}
			}
			finally
			{
				this.StateLabel.Text = "";
				this.StateLabel.Refresh();
				base.Enabled = true;
			}
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			base.Enabled = false;
			try
			{
				if (this.RuntimeView.SelectedIndices.Count == 0)
				{
					MessageBox.Show("ランタイムが選択されていません。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					Runtime runtime = this.runtimedatas[this.RuntimeView.SelectedIndices[0]];
					string nRPath = this.runtimes[this.RuntimeView.SelectedIndices[0]];
					if (!Global.cpd.UseLayer && runtime.Definitions.LayerSize.bytesize != 0)
					{
						MessageBox.Show("レイヤー未使用プロジェクトからレイヤー使用プロジェクトへ移行します。" + Environment.NewLine + "レイヤー画像を指定してください。", "レイヤープロジェクトへの移行", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						using (OpenFileDialog openFileDialog = new OpenFileDialog())
						{
							openFileDialog.DefaultExt = "*.gif";
							openFileDialog.Filter = "gif画像 (*.gif)|*.gif|png画像 (*.png)|*.png|全てのファイル (*.*)|*.*";
							openFileDialog.InitialDirectory = Global.cpd.where;
							if (openFileDialog.ShowDialog() != DialogResult.OK)
							{
								return;
							}
							Global.cpd.project.Config.LayerImage = openFileDialog.FileName;
						}
					}
					if (MessageBox.Show("移行を開始します。この操作は取り消せません。" + Environment.NewLine + "よろしいですか？", "操作の確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.Cancel)
					{
						this.StateLabel.Text = "プロジェクトを移行しています...";
						this.StateLabel.Refresh();
						this.MoveProject(nRPath, runtime);
					}
				}
			}
			finally
			{
				this.StateLabel.Text = "";
				base.Enabled = true;
			}
		}

		private void ChipDefinitionValidation_CheckedChanged(object sender, EventArgs e)
		{
			this.ChipMethod.Enabled = this.ChipDefinitionValidation.Checked;
		}

		private void NoTouch_CheckedChanged(object sender, EventArgs e)
		{
			if (this.NoTouch.Checked && MessageBox.Show(string.Concat(new string[]
			{
				"データを移行しないと、編集できなくなったり、破損したりする恐れがあります。",
				Environment.NewLine,
				"データを完全を失ってしまう事もあるかもしれません。",
				Environment.NewLine,
				"本当にいいんですか？"
			}), "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				this.NoTouch.Checked = false;
			}
			this.ChipDefinitionValidation.Enabled = !this.NoTouch.Checked;
			this.ChipMethod.Enabled = !this.NoTouch.Checked;
		}

		private void MoveProject(string nRPath, Runtime nR)
		{
			Project project = new Project();
			ChipDataClass chipDataClass = null;
			try
			{
				project.Config = ConfigurationOwner.LoadXML(Path.Combine(Path.Combine(Path.GetDirectoryName(nRPath), Path.GetFileNameWithoutExtension(nRPath)), nR.Definitions.Configurations));
				project.Runtime = nR;
				chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.Combine(Path.GetDirectoryName(nRPath), Path.GetFileNameWithoutExtension(nRPath)), nR.Definitions.ChipDefinition));
			}
			catch
			{
				MessageBox.Show("移行に失敗しました。" + Environment.NewLine + "移行先の設定が読み込めませんでした。", "移行失敗", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
				return;
			}
			this.StateLabel.Text = "設定を移行しています...";
			this.StateLabel.Refresh();
			int num = 0;
			int num2 = 0;
			foreach (ConfigParam configParam in project.Config.Configurations)
			{
				this.StateLabel.Text = string.Concat(new object[]
				{
					"設定を移行しています...(",
					num2.ToString(),
					"/",
					project.Config.Configurations.Length,
					")"
				});
				this.StateLabel.Refresh();
				bool flag = false;
				foreach (ConfigParam configParam2 in Global.cpd.project.Config.Configurations)
				{
					if (configParam.Name == configParam2.Name && configParam.Typestr == configParam2.Typestr)
					{
						project.Config.Configurations[num].Value = configParam2.Value;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num2++;
				}
				num++;
			}
			if (this.NoTouch.Checked) // 手を加えない（全部そのままコピー）
			{
				this.StateLabel.Text = "ステージをコピーしています...";
				this.StateLabel.Refresh();
				project.StageData = (string[])Global.cpd.project.StageData.Clone();
				project.StageData2 = (string[])Global.cpd.project.StageData2.Clone();
				project.StageData3 = (string[])Global.cpd.project.StageData3.Clone();
				project.StageData4 = (string[])Global.cpd.project.StageData4.Clone();
				project.LayerData = (string[])Global.cpd.project.LayerData.Clone();
				project.LayerData2 = (string[])Global.cpd.project.LayerData2.Clone();
				project.LayerData3 = (string[])Global.cpd.project.LayerData3.Clone();
				project.LayerData4 = (string[])Global.cpd.project.LayerData4.Clone();
				project.MapData = (string[])Global.cpd.project.MapData.Clone();
			}
			else
			{
				string character = chipDataClass.Mapchip[0].character;
				project.StageData = StageDataCopy(project, Global.cpd.project.StageData, chipDataClass.Mapchip, character);
				project.StageData2 = StageDataCopy(project, Global.cpd.project.StageData2, chipDataClass.Mapchip, character);
				project.StageData3 = StageDataCopy(project, Global.cpd.project.StageData3, chipDataClass.Mapchip, character);
				project.StageData4 = StageDataCopy(project, Global.cpd.project.StageData4, chipDataClass.Mapchip, character);
				if (project.Runtime.Definitions.LayerSize.bytesize != 0) // レイヤーあり
				{
					character = chipDataClass.Layerchip[0].character;
					project.LayerData = LayerDataCopy(project, Global.cpd.project.LayerData, chipDataClass.Layerchip, character);
					project.LayerData2 = LayerDataCopy(project, Global.cpd.project.LayerData2, chipDataClass.Layerchip, character);
					project.LayerData3 = LayerDataCopy(project, Global.cpd.project.LayerData3, chipDataClass.Layerchip, character);
					project.LayerData4 = LayerDataCopy(project, Global.cpd.project.LayerData4, chipDataClass.Layerchip, character);
				}
				character = chipDataClass.WorldChip[0].character;
				project.MapData = MapDataCopy(project, Global.cpd.project.MapData, chipDataClass.WorldChip, character);
			}
			this.StateLabel.Text = "その他のデータを移行しています...";
			this.StateLabel.Refresh();
			project.Config.ConfigReady();
			project.Config.PatternImage = Global.cpd.project.Config.PatternImage;
			project.Config.EndingImage = Global.cpd.project.Config.EndingImage;
			project.Config.GameoverImage = Global.cpd.project.Config.GameoverImage;
			if (project.Runtime.Definitions.LayerSize.bytesize != 0)
			{
				project.Config.LayerImage = Global.cpd.project.Config.LayerImage;
			}
			project.Name = Global.cpd.project.Name;
			project.Config.TitleImage = Global.cpd.project.Config.TitleImage;
			this.StateLabel.Text = "新ランタイムのファイルをコピーしています...";
			this.StateLabel.Refresh();
			foreach (string text2 in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(nRPath), Path.GetFileNameWithoutExtension(nRPath)), "*", SearchOption.TopDirectoryOnly))
			{
				if (!(Path.GetFileName(text2) == nR.Definitions.Configurations))
				{
					string text3 = Path.Combine(Global.cpd.where, Path.GetFileName(text2));
					if (File.Exists(text3))
					{
						if (MessageBox.Show(string.Concat(new string[]
						{
							text3,
							Environment.NewLine,
							"はすでに存在しています。",
							Environment.NewLine,
							"上書きしますか？"
						}), "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
						{
							goto IL_B9B;
						}
					}
					try
					{
						File.Copy(text2, text3, true);
					}
					catch (Exception ex)
					{
						MessageBox.Show("ファイルのコピーに失敗しました。" + Environment.NewLine + ex.Message, "コピー失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
				IL_B9B:;
			}
			this.StateLabel.Text = "データを反映しています...";
			this.StateLabel.Refresh();
			Global.cpd.project = project;
			Global.state.Background = Global.cpd.project.Config.Background;
			Global.cpd.runtime = Global.cpd.project.Runtime;
			if (Global.cpd.UseLayer)
			{
				Global.cpd.Layerchip = chipDataClass.Layerchip;
			}
			Global.cpd.Mapchip = chipDataClass.Mapchip;
			this.StateLabel.Text = "編集システムを再スタートしています...";
			this.StateLabel.Refresh();
			Global.MainWnd.MainDesigner.PrepareImages();
			Global.MainWnd.MainDesigner.CreateDrawItemReference();
			Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
			if (Global.cpd.UseLayer)
			{
				Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
			}
			Global.MainWnd.ChipItemReady();
			Global.MainWnd.EditPatternChip_Click(this, new EventArgs());
			Global.state.EditingForeground = true;
			Global.state.EditFlag = true;
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private string[] StageDataCopy(Project project, string[] StageData, ChipsData[] Mapchip, string nullcharacter)
		{
			string[] result = new string[project.Runtime.Definitions.StageSize.y];

			for (int k = 0; k < project.Runtime.Definitions.StageSize.y; k++)
			{
				this.StateLabel.Text = string.Concat(new string[]
				{
						"ステージを移行しています...(",
						k.ToString(),
						"/",
						project.Runtime.Definitions.StageSize.y.ToString(),
						")"
				});
				this.StateLabel.Refresh();
				StringBuilder stringBuilder = new StringBuilder();
				if (k < StageData.Length)
				{
					int l;
					for (l = 0; l < Global.cpd.runtime.Definitions.StageSize.x; l++)
					{
						if (l >= project.Runtime.Definitions.StageSize.x)
						{
							break;
						}
						string text = StageData[k].Substring(l * Global.cpd.runtime.Definitions.StageSize.bytesize, Global.cpd.runtime.Definitions.StageSize.bytesize);
						switch (this.ChipMethod.SelectedIndex)
						{
							case 0: // 文字で合わせる
								foreach (ChipsData chipsData in Mapchip)
								{
									if (chipsData.character == text)
									{
										stringBuilder.Append(chipsData.character);
										break;
									}
								}
								break;
							case 1: // マップチップの位置で合わせる
								{
									ChipsData chipsData2 = Global.MainWnd.MainDesigner.DrawItemRef[text];
									foreach (ChipsData chipsData3 in Mapchip)
									{
										if (chipsData2.Chips[0].pattern == chipsData3.Chips[0].pattern)
										{
											stringBuilder.Append(chipsData3.character);
											break;
										}
									}
									break;
								}
							case 2: // 定義リストの順番で合わせる
								{
									int num3 = 0;
									foreach (ChipsData chipsData4 in Global.cpd.Mapchip)
									{
										if (chipsData4.character == text)
										{
											break;
										}
										num3++;
									}
									if (num3 < Mapchip.Length)
									{
										stringBuilder.Append(Mapchip[num3].character);
									}
									else
									{
										stringBuilder.Append(nullcharacter);
									}
									break;
								}
						}
					}
					while (l < project.Runtime.Definitions.StageSize.x)
					{
						stringBuilder.Append(nullcharacter);
						l++;
					}
				}
				else
				{
					for (int num5 = 0; num5 < project.Runtime.Definitions.StageSize.x; num5++)
					{
						stringBuilder.Append(nullcharacter);
					}
				}
				result[k] = stringBuilder.ToString();
			}
			return result;
		}
		private string[] LayerDataCopy(Project project, string[] LayerData, ChipsData[] Layerchip, string nullcharacter)
		{
			string[] result = new string[project.Runtime.Definitions.LayerSize.y];

			for (int num6 = 0; num6 < project.Runtime.Definitions.LayerSize.y; num6++)
			{
				this.StateLabel.Text = string.Concat(new string[]
				{
							"レイヤーを移行しています...(",
							num6.ToString(),
							"/",
							project.Runtime.Definitions.LayerSize.y.ToString(),
							")"
				});
				this.StateLabel.Refresh();
				StringBuilder stringBuilder = new StringBuilder();
				if (Global.cpd.UseLayer)
				{
					if (num6 < LayerData.Length)
					{
						int num7;
						for (num7 = 0; num7 < Global.cpd.runtime.Definitions.LayerSize.x; num7++)
						{
							if (num7 >= project.Runtime.Definitions.LayerSize.x)
							{
								break;
							}
							string text = LayerData[num6].Substring(num7 * Global.cpd.runtime.Definitions.LayerSize.bytesize, Global.cpd.runtime.Definitions.LayerSize.bytesize);
							switch (this.ChipMethod.SelectedIndex)
							{
								case 0: // 文字で合わせる
									foreach (ChipsData chipsData5 in Layerchip)
									{
										if (chipsData5.character == text)
										{
											stringBuilder.Append(chipsData5.character);
											break;
										}
									}
									break;
								case 1: // マップチップの位置で合わせる
									{
										ChipsData chipsData6 = Global.MainWnd.MainDesigner.DrawLayerRef[text];
										foreach (ChipsData chipsData7 in Layerchip)
										{
											if (chipsData6.Chips[0].pattern == chipsData7.Chips[0].pattern)
											{
												stringBuilder.Append(chipsData7.character);
												break;
											}
										}
										break;
									}
								case 2: // 定義リストの順番で合わせる
									{
										int num10 = 0;
										foreach (ChipsData chipsData8 in Global.cpd.Layerchip)
										{
											if (chipsData8.character == text)
											{
												break;
											}
											num10++;
										}
										if (num10 < Layerchip.Length)
										{
											stringBuilder.Append(Layerchip[num10]);
										}
										else
										{
											stringBuilder.Append(nullcharacter);
										}
										break;
									}
							}
						}
						while (num7 < project.Runtime.Definitions.LayerSize.x)
						{
							stringBuilder.Append(nullcharacter);
							num7++;
						}
					}
					else
					{
						for (int num12 = 0; num12 < project.Runtime.Definitions.LayerSize.x; num12++)
						{
							stringBuilder.Append(nullcharacter);
						}
					}
				}
				else
				{
					for (int num13 = 0; num13 < project.LayerData.Length; num13++)
					{
						stringBuilder = new StringBuilder();
						for (int num14 = 0; num14 < project.Runtime.Definitions.LayerSize.x; num14++)
						{
							stringBuilder.Append(nullcharacter);
						}
					}
				}
				result[num6] = stringBuilder.ToString();
			}
			return result;
		}
		private string[] MapDataCopy(Project project, string[] MapData, ChipsData[] Worldchip, string nullcharacter)
		{// 上記StageDataCopyとほぼ同じ
			string[] result = new string[project.Runtime.Definitions.MapSize.y];

			for (int k = 0; k < project.Runtime.Definitions.MapSize.y; k++)
			{
				this.StateLabel.Text = string.Concat(new string[]
				{
						"地図画面を移行しています...(",
						k.ToString(),
						"/",
						project.Runtime.Definitions.MapSize.y.ToString(),
						")"
				});
				this.StateLabel.Refresh();
				StringBuilder stringBuilder = new StringBuilder();
				if (k < MapData.Length)
				{
					int l;
					for (l = 0; l < Global.cpd.runtime.Definitions.MapSize.x; l++)
					{
						if (l >= project.Runtime.Definitions.MapSize.x)
						{
							break;
						}
						string text = MapData[k].Substring(l * Global.cpd.runtime.Definitions.MapSize.bytesize, Global.cpd.runtime.Definitions.MapSize.bytesize);
						switch (this.ChipMethod.SelectedIndex)
						{
							case 0: // 文字で合わせる
								foreach (ChipsData chipsData in Worldchip)
								{
									if (chipsData.character == text)
									{
										stringBuilder.Append(chipsData.character);
										break;
									}
								}
								break;
							case 1: // マップチップの位置で合わせる
								{
									ChipsData chipsData2 = Global.MainWnd.MainDesigner.DrawItemRef[text];
									foreach (ChipsData chipsData3 in Worldchip)
									{
										if (chipsData2.Chips[0].pattern == chipsData3.Chips[0].pattern)
										{
											stringBuilder.Append(chipsData3.character);
											break;
										}
									}
									break;
								}
							case 2: // 定義リストの順番で合わせる
								{
									int num3 = 0;
									foreach (ChipsData chipsData4 in Global.cpd.Mapchip)
									{
										if (chipsData4.character == text)
										{
											break;
										}
										num3++;
									}
									if (num3 < Worldchip.Length)
									{
										stringBuilder.Append(Worldchip[num3].character);
									}
									else
									{
										stringBuilder.Append(nullcharacter);
									}
									break;
								}
						}
					}
					while (l < project.Runtime.Definitions.MapSize.x)
					{
						stringBuilder.Append(nullcharacter);
						l++;
					}
				}
				else
				{
					for (int num5 = 0; num5 < project.Runtime.Definitions.MapSize.x; num5++)
					{
						stringBuilder.Append(nullcharacter);
					}
				}
				result[k] = stringBuilder.ToString();
			}
			return result;
		}


		public List<string> runtimes = new List<string>();

		public List<Runtime> runtimedatas = new List<Runtime>();
	}
}

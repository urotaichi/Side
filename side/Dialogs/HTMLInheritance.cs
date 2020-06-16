using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	// Token: 0x02000018 RID: 24
	public partial class HTMLInheritance : Form
	{
		// Token: 0x060000CF RID: 207 RVA: 0x000132B8 File Offset: 0x000114B8
		public HTMLInheritance(string pf)
		{
			this.InitializeComponent();
			this.ParseFile = pf;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00013310 File Offset: 0x00011510
		private void Inheritance_Load(object sender, EventArgs e)
		{
			this.TargetFile.Text = this.ParseFile;
			this.ProjectName.Text = Path.GetFileNameWithoutExtension(this.ParseFile);
			this.RootDir.Text = Global.config.lastData.ProjDirF;
			this.OK.Text = "お待ちください...";
			this.OK.Enabled = false;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0001337C File Offset: 0x0001157C
		private void Inheritance_Shown(object sender, EventArgs e)
		{
			Application.DoEvents();
			if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
			{
				MessageBox.Show("ランタイムフォルダが見つかりません。" + Environment.NewLine + "Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Cancel;
				base.Close();
				return;
			}
			string[] files = Directory.GetFiles(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir), "*.xml", SearchOption.TopDirectoryOnly);
			foreach (string text in files)
			{
				try
				{
					string text2 = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
					if (Directory.Exists(text2))
					{
						Runtime runtime = Runtime.ParseXML(text);
						if (runtime != null)
						{
							string[] array2 = Runtime.CheckFiles(text2, runtime);
							if (array2.Length == 0)
							{
								this.runtimes.Add(text);
								this.runtimedatas.Add(runtime);
								this.runtimeuselayer.Add(runtime.Definitions.LayerSize.bytesize != 0);
								this.RuntimeSet.Items.Add(string.Concat(new string[]
								{
									runtime.Definitions.Name,
									" [Author:",
									runtime.Definitions.Author,
									" Layer:",
									(runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×",
									"] : ",
									Path.GetFileName(text)
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
			if (this.RuntimeSet.Items.Count == 0)
			{
				MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Cancel;
				base.Close();
			}
			else
			{
				this.RuntimeSet.SelectedIndex = 0;
			}
			this.OK.Enabled = true;
			this.OK.Text = "OK";
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00013634 File Offset: 0x00011834
		private void RootDirBrowse_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "プロジェクトのルートディレクトリを選択してください。";
				folderBrowserDialog.SelectedPath = this.RootDir.Text;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					this.RootDir.Text = folderBrowserDialog.SelectedPath;
				}
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00002898 File Offset: 0x00000A98
		private void ProjectName_TextChanged(object sender, EventArgs e)
		{
			this.ValidationText();
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00002898 File Offset: 0x00000A98
		private void RootDir_TextChanged(object sender, EventArgs e)
		{
			this.ValidationText();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0001369C File Offset: 0x0001189C
		private void ValidationText()
		{
			if (this.ProjectName.Text == "")
			{
				this.OK.Enabled = false;
				return;
			}
			if (this.RootDir.Text == "" || !Directory.Exists(this.RootDir.Text))
			{
				this.OK.Enabled = false;
				return;
			}
			this.OK.Enabled = true;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00013710 File Offset: 0x00011910
		private void OK_Click(object sender, EventArgs e)
		{
			try
			{
				base.Enabled = false;
				this.OK.Text = "生成中...";
				this.OK.Refresh();
				this.StatusText.Text = "プロジェクト生成準備中...";
				this.StatusText.Refresh();
				Global.config.lastData.ProjDir = this.RootDir.Text;
				string text = Path.Combine(this.RootDir.Text, this.ProjectName.Text);
				if (Directory.Exists(text) && MessageBox.Show(string.Concat(new string[]
				{
					"ディレクトリ",
					Environment.NewLine,
					"\"",
					text,
					"\"",
					Environment.NewLine,
					"はすでに存在します。",
					Environment.NewLine,
					"中に含まれるファイルは上書きされてしまう事があります。",
					Environment.NewLine,
					"続行しますか？"
				}), "プロジェクト生成警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
				{
					return;
				}
				this.ProjectFile = Path.Combine(text, this.ProjectName.Text + Global.definition.ProjExt);
				string text2 = Path.Combine(Path.GetDirectoryName(this.runtimes[this.RuntimeSet.SelectedIndex]), Path.GetFileNameWithoutExtension(this.runtimes[this.RuntimeSet.SelectedIndex]));
				Project project = new Project();
				project.Name = this.ProjectName.Text;
				project.Runtime = this.runtimedatas[this.RuntimeSet.SelectedIndex];
				project.Config = ConfigurationOwner.LoadXML(Path.Combine(text2, this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.Configurations));
				if (project.Runtime.Definitions.LayerSize.bytesize != 0)
				{
					project.LayerData = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
					project.LayerData2 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
					project.LayerData3 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
					project.LayerData4 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
				}
				project.StageData = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.StageSize.y];
				project.StageData2 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.StageSize.y];
				project.StageData3 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.StageSize.y];
				project.StageData4 = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.StageSize.y];
				project.MapData = new string[this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.MapSize.y];
				ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(text2, this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.ChipDefinition));
				string character = chipDataClass.Mapchip[0].character;
				for (int i = 0; i < project.StageData.Length; i++)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < project.Runtime.Definitions.StageSize.x; j++)
					{
						stringBuilder.Append(character);
					}
					project.StageData[i] = stringBuilder.ToString();
				}
				project.StageData2 = (string[])project.StageData.Clone();
				project.StageData3 = (string[])project.StageData.Clone();
				project.StageData4 = (string[])project.StageData.Clone();
				character = chipDataClass.WorldChip[0].character;
				for (int k = 0; k < project.MapData.Length; k++)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					for (int l = 0; l < project.Runtime.Definitions.MapSize.x; l++)
					{
						stringBuilder2.Append(character);
					}
					project.MapData[k] = stringBuilder2.ToString();
				}
				if (project.Runtime.Definitions.LayerSize.bytesize != 0)
				{
					character = chipDataClass.Layerchip[0].character;
					for (int m = 0; m < project.LayerData.Length; m++)
					{
						StringBuilder stringBuilder3 = new StringBuilder();
						for (int n = 0; n < project.Runtime.Definitions.LayerSize.x; n++)
						{
							stringBuilder3.Append(character);
						}
						project.LayerData[m] = stringBuilder3.ToString();
					}
					project.LayerData2 = (string[])project.LayerData.Clone();
					project.LayerData3 = (string[])project.LayerData.Clone();
					project.LayerData4 = (string[])project.LayerData.Clone();
				}
				this.StatusText.Text = "HTMLデータ取得準備中...";
				this.StatusText.Refresh();
				string input = "";
				try
				{
					input = Subsystem.LoadUnknownTextFile(this.ParseFile);
				}
				catch
				{
					MessageBox.Show("ファイルをロードできませんでした。", "コンバート失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					base.DialogResult = DialogResult.Cancel;
					base.Close();
				}
				List<string> list = new List<string>();
				if (this.SeekHeaderFooter.Checked)
				{
					Regex regex = new Regex("^.*?<[ ]*?APPLET .*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					Match match = regex.Match(input);
					if (match.Success)
					{
						project.Runtime.DefaultConfigurations.HeaderHTML = match.Value;
					}
					regex = new Regex("<[ ]*?/[ ]*?APPLET[ ]*?>.*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					match = regex.Match(input);
					if (match.Success)
					{
						project.Runtime.DefaultConfigurations.FooterHTML = match.Value;
					}
				}
				this.StatusText.Text = "HTMLデータ取得中...";
				this.StatusText.Refresh();
				Regex regex2 = new Regex("<[ ]*PARAM[ ]+NAME=\"(?<name>.*?)\"[ ]+VALUE=\"(?<value>.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				Match match2 = regex2.Match(input);
				while (match2.Success)
				{
					dictionary.Add(match2.Groups["name"].Value, match2.Groups["value"].Value);
					match2 = match2.NextMatch();
				}
				this.StatusText.Text = "マップソース生成中...";
				this.StatusText.Refresh();
				this.GetMapSource(ref project.MapData, project.Runtime.Definitions.MapName, project.Runtime.Definitions.MapSize.y, ref dictionary);
				this.StatusText.Text = "ステージソース生成中[1/4]...";
				this.StatusText.Refresh();
				this.GetMapSource(ref project.StageData, project.Runtime.Definitions.ParamName, project.Runtime.Definitions.StageSize.y, ref dictionary);
				this.StatusText.Text = "ステージソース生成中[2/4]...";
				this.StatusText.Refresh();
				this.GetMapSource(ref project.StageData2, project.Runtime.Definitions.ParamName2, project.Runtime.Definitions.StageSize.y, ref dictionary);
				this.StatusText.Text = "ステージソース生成中[3/4]...";
				this.StatusText.Refresh();
				this.GetMapSource(ref project.StageData3, project.Runtime.Definitions.ParamName3, project.Runtime.Definitions.StageSize.y, ref dictionary);
				this.StatusText.Text = "ステージソース生成中[4/4]...";
				this.StatusText.Refresh();
				this.GetMapSource(ref project.StageData4, project.Runtime.Definitions.ParamName4, project.Runtime.Definitions.StageSize.y, ref dictionary);
				if (project.Runtime.Definitions.LayerSize.bytesize != 0)
				{
					this.StatusText.Text = "レイヤーソース生成中[1/4]...";
					this.StatusText.Refresh();
					this.GetMapSource(ref project.LayerData, project.Runtime.Definitions.LayerName, project.Runtime.Definitions.LayerSize.y, ref dictionary);
					this.StatusText.Text = "レイヤーソース生成中[2/4]...";
					this.StatusText.Refresh();
					this.GetMapSource(ref project.LayerData2, project.Runtime.Definitions.LayerName2, project.Runtime.Definitions.LayerSize.y, ref dictionary);
					this.StatusText.Text = "レイヤーソース生成中[3/4]...";
					this.StatusText.Refresh();
					this.GetMapSource(ref project.LayerData3, project.Runtime.Definitions.LayerName3, project.Runtime.Definitions.LayerSize.y, ref dictionary);
					this.StatusText.Text = "レイヤーソース生成中[4/4]...";
					this.StatusText.Refresh();
					this.GetMapSource(ref project.LayerData4, project.Runtime.Definitions.LayerName4, project.Runtime.Definitions.LayerSize.y, ref dictionary);
				}
				this.StatusText.Text = "パラメータ反映中...";
				this.StatusText.Refresh();
				int num = 0;
				while (num < project.Config.Configurations.Length)
				{
					switch (project.Config.Configurations[num].Type)
					{
					case ConfigParam.Types.b:
						if (dictionary.ContainsKey(project.Config.Configurations[num].Name))
						{
							if (dictionary[project.Config.Configurations[num].Name] == "2")
							{
								project.Config.Configurations[num].Value = "false";
							}
							else
							{
								project.Config.Configurations[num].Value = "true";
							}
						}
						break;
					case ConfigParam.Types.s:
					case ConfigParam.Types.i:
					case ConfigParam.Types.l:
						goto IL_D9E;
					case ConfigParam.Types.t:
					{
						string name = project.Config.Configurations[num].Name;
						List<string> list2 = new List<string>();
						int num2 = 0;
						while (dictionary.ContainsKey(name + "-" + num2.ToString()))
						{
							list2.Add(dictionary[name + "-" + num2.ToString()]);
						}
						if (list2.Count > 0)
						{
							if (project.Config.Configurations[num].Rows > list2.Count)
							{
								while (project.Config.Configurations[num].Rows > list2.Count)
								{
									list2.Add("0");
								}
							}
							else if (project.Config.Configurations[num].Rows < list2.Count)
							{
								list2.RemoveRange(project.Config.Configurations[num].Rows, list2.Count - project.Config.Configurations[num].Rows);
							}
							project.Config.Configurations[num].Value = string.Join(Environment.NewLine, list2.ToArray());
						}
						break;
					}
					case ConfigParam.Types.f:
						if (dictionary.ContainsKey(project.Config.Configurations[num].Name))
						{
							list.Add(dictionary[project.Config.Configurations[num].Name]);
							project.Config.Configurations[num].Value = Path.GetFileName(dictionary[project.Config.Configurations[num].Name]);
						}
						break;
					case ConfigParam.Types.c:
					{
						string[] array = new string[]
						{
							"red",
							"green",
							"blue"
						};
						int[] array2 = new int[3];
						string name = project.Config.Configurations[num].Name;
						bool flag = true;
						for (int num3 = 0; num3 < 3; num3++)
						{
							if (!dictionary.ContainsKey(name + array[num3]))
							{
								flag = false;
								break;
							}
							if (!int.TryParse(dictionary[name + array[num3]], out array2[num3]))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							Colors colors = default(Colors);
							colors.r = array2[0];
							colors.g = array2[1];
							colors.b = array2[2];
							project.Config.Configurations[num].Value = colors.ToString();
						}
						break;
					}
					default:
						goto IL_D9E;
					}
					IL_DF3:
					num++;
					continue;
					IL_D9E:
					if (dictionary.ContainsKey(project.Config.Configurations[num].Name))
					{
						project.Config.Configurations[num].Value = dictionary[project.Config.Configurations[num].Name];
						goto IL_DF3;
					}
					goto IL_DF3;
				}
				Directory.CreateDirectory(text);
				foreach (string text3 in Directory.GetFiles(text2, "*", SearchOption.TopDirectoryOnly))
				{
					if (!(Path.GetFileName(text3) == this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.Configurations))
					{
						string text4 = Path.Combine(text, Path.GetFileName(text3));
						if (!File.Exists(text4) || MessageBox.Show(string.Concat(new string[]
						{
							text4,
							Environment.NewLine,
							"はすでに存在しています。",
							Environment.NewLine,
							"上書きしてもよろしいですか？"
						}), "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
						{
							File.Copy(text3, text4, true);
						}
					}
				}
				foreach (string text5 in list)
				{
					if (File.Exists(Path.Combine(Path.GetDirectoryName(this.ParseFile), text5)))
					{
						File.Copy(Path.Combine(Path.GetDirectoryName(this.ParseFile), text5), Path.Combine(text, Path.GetFileName(text5)), true);
					}
				}
				project.SaveXML(this.ProjectFile);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"プロジェクト生成に失敗しました。",
					Environment.NewLine,
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}), "プロジェクト生成エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			finally
			{
				this.OK.Text = "OK";
				base.Enabled = true;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00014750 File Offset: 0x00012950
		private bool GetMapSource(ref string[] overwrite, string f, int dysize, ref Dictionary<string, string> Params)
		{
			List<string> list = new List<string>();
			int num = 0;
			for (;;)
			{
				int num2 = 0;
				while (Params.ContainsKey(string.Format(f, num2, num)))
				{
					if (list.Count <= num)
					{
						list.Add(Params[string.Format(f, num2, num)]);
					}
					else
					{
						list[num] += Params[string.Format(f, num2, num)];
					}
					num2++;
				}
				if (num2 == 0)
				{
					break;
				}
				num++;
			}
			if (num == dysize)
			{
				overwrite = list.ToArray();
				return true;
			}
			return false;
		}

		// Token: 0x040000D0 RID: 208
		public string ProjectFile = "";

		// Token: 0x040000D1 RID: 209
		public List<string> runtimes = new List<string>();

		// Token: 0x040000D2 RID: 210
		public List<Runtime> runtimedatas = new List<Runtime>();

		// Token: 0x040000D3 RID: 211
		public List<bool> runtimeuselayer = new List<bool>();

		// Token: 0x040000D4 RID: 212
		private string ParseFile = "";
	}
}

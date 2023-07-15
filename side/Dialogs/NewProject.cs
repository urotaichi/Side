using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class NewProject : Form
	{
		public NewProject()
		{
			this.InitializeComponent();
		}

		private void NewProject_Load(object sender, EventArgs e)
		{
			this.RuntimeSet.DropDownWidth = base.Width - this.RuntimeSet.Left;
			this.RootDir.Text = Global.config.lastData.ProjDirF;
			this.OK.Text = "お待ちください...";
			this.OK.Enabled = false;
		}

		private void NewProject_Shown(object sender, EventArgs e)
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
								this.RuntimeSet.Items.Add($"{runtime.Definitions.Name} [Author:{runtime.Definitions.Author} Layer:"
									+ ((runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×")
									+ $"] : {Path.GetFileName(text)}");
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
						MessageBox.Show($"ランタイムフォルダが見つかりません:{Path.GetFileName(text)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
			this.OK.Text = "OK";
			this.CheckInput();
		}

		private void CheckInput()
		{
			if (this.ProjectName.Text != "" && this.RootDir.Text != "" && Directory.Exists(this.RootDir.Text) && (!this.LayerPattern.Enabled || this.LayerPattern.Text == "" || File.Exists(this.LayerPattern.Text)))
			{
				string[] array = new string[]
				{
					this.MapChip.Text,
					this.TitleImage.Text,
					this.EndingImage.Text,
					this.GameoverImage.Text
				};
				foreach (string text in array)
				{
					if (text != "" && !File.Exists(text))
					{
						this.OK.Enabled = false;
						return;
					}
				}
				this.OK.Enabled = true;
				return;
			}
			this.OK.Enabled = false;
		}

		private void TextCheckNullable(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (File.Exists(textBox.Text))
			{
				textBox.ForeColor = Color.Black;
				textBox.BackColor = Color.White;
			}
			else
			{
				textBox.ForeColor = Color.White;
				textBox.BackColor = Color.Red;
			}
			this.CheckInput();
		}

		private void ValidText(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text != "")
			{
				textBox.ForeColor = Color.Black;
				textBox.BackColor = Color.White;
			}
			else
			{
				textBox.ForeColor = Color.White;
				textBox.BackColor = Color.Red;
			}
			this.CheckInput();
		}

		private void ValidPath(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text != "" && File.Exists(textBox.Text))
			{
				textBox.ForeColor = Color.Black;
				textBox.BackColor = Color.White;
			}
			else
			{
				textBox.ForeColor = Color.White;
				textBox.BackColor = Color.Red;
			}
			this.CheckInput();
		}

		private void ValidPathEmptiable(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text == "" || File.Exists(textBox.Text))
			{
				textBox.ForeColor = Color.Black;
				textBox.BackColor = Color.White;
			}
			else
			{
				textBox.ForeColor = Color.White;
				textBox.BackColor = Color.Red;
			}
			this.CheckInput();
		}

		private void ValidDir(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text != "" && Directory.Exists(textBox.Text))
			{
				textBox.ForeColor = Color.Black;
				textBox.BackColor = Color.White;
			}
			else
			{
				textBox.ForeColor = Color.White;
				textBox.BackColor = Color.Red;
			}
			this.CheckInput();
		}

		private void RootDirBrowse_Click(object sender, EventArgs e)
		{
            using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "プロジェクトのルートディレクトリを選択してください。";
            folderBrowserDialog.SelectedPath = this.RootDir.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.RootDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

		private void MapChipBrowse_Click(object sender, EventArgs e)
		{
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.bmp)|*.gif;*.png;*.bmp|全てのファイル (*.*)|*.*";
            if (this.MapChip.Text != "")
            {
                openFileDialog.FileName = this.MapChip.Text;
            }
            else
            {
                openFileDialog.FileName = this.RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.MapChip.Text = openFileDialog.FileName;
            }
        }

		private void LayerPatternBrowse_Click(object sender, EventArgs e)
		{
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.bmp)|*.gif;*.png;*.bmp|全てのファイル (*.*)|*.*";
            if (this.MapChip.Text != "")
            {
                openFileDialog.FileName = this.LayerPattern.Text;
            }
            else
            {
                openFileDialog.FileName = this.RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.LayerPattern.Text = openFileDialog.FileName;
            }
        }

		private void TitleImageBrowse_Click(object sender, EventArgs e)
		{
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (this.TitleImage.Text != "")
            {
                openFileDialog.FileName = this.TitleImage.Text;
            }
            else
            {
                openFileDialog.FileName = this.RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.TitleImage.Text = openFileDialog.FileName;
            }
        }

		private void EndingImageBrowse_Click(object sender, EventArgs e)
		{
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (this.EndingImage.Text != "")
            {
                openFileDialog.FileName = this.EndingImage.Text;
            }
            else
            {
                openFileDialog.FileName = this.RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.EndingImage.Text = openFileDialog.FileName;
            }
        }

		private void GameoverBrowse_Click(object sender, EventArgs e)
		{
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (this.GameoverImage.Text != "")
            {
                openFileDialog.FileName = this.GameoverImage.Text;
            }
            else
            {
                openFileDialog.FileName = this.RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.GameoverImage.Text = openFileDialog.FileName;
            }
        }

		private void OK_Click(object sender, EventArgs ev)
		{
			List<string> list = new List<string>();
			try
			{
				base.Enabled = false;
				this.OK.Text = "生成中...";
				this.OK.Refresh();
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
				string text2 = Path.Combine(text, this.ProjectName.Text + Global.definition.ProjExt);
				string text3 = Path.Combine(Path.GetDirectoryName(this.runtimes[this.RuntimeSet.SelectedIndex]), Path.GetFileNameWithoutExtension(this.runtimes[this.RuntimeSet.SelectedIndex]));
				Project project = new Project();
				project.Name = this.ProjectName.Text;
				project.Runtime = this.runtimedatas[this.RuntimeSet.SelectedIndex];
				project.Config = ConfigurationOwner.LoadXML(Path.Combine(text3, this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.Configurations));
				if (this.TitleImage.Text != "")
				{
					list.Add(project.Config.TitleImage);
					project.Config.TitleImage = Path.GetFileName(this.TitleImage.Text);
				}
				if (this.MapChip.Text != "")
				{
					list.Add(project.Config.PatternImage);
					project.Config.PatternImage = Path.GetFileName(this.MapChip.Text);
				}
				if (this.EndingImage.Text != "")
				{
					list.Add(project.Config.EndingImage);
					project.Config.EndingImage = Path.GetFileName(this.EndingImage.Text);
				}
				if (this.GameoverImage.Text != "")
				{
					list.Add(project.Config.GameoverImage);
					project.Config.GameoverImage = Path.GetFileName(this.GameoverImage.Text);
				}
				if (project.Runtime.Definitions.LayerSize.bytesize != 0)
				{
					if (this.LayerPattern.Text != "")
					{
						project.Config.LayerImage = Path.GetFileName(this.LayerPattern.Text);
					}
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
				project.Config.StageNum = (int)this.StageNum.Value;
				ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(text3, project.Runtime.Definitions.ChipDefinition));
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
				Directory.CreateDirectory(text);
				foreach (string text4 in Directory.GetFiles(text3, "*", SearchOption.TopDirectoryOnly))
				{
					if (!(Path.GetFileName(text4) == this.runtimedatas[this.RuntimeSet.SelectedIndex].Definitions.Configurations) && !list.Contains(Path.GetFileName(text4)))
					{
						string text5 = Path.Combine(text, Path.GetFileName(text4));
						if (!File.Exists(text5) || MessageBox.Show(string.Concat(new string[]
						{
							text5,
							Environment.NewLine,
							"はすでに存在しています。",
							Environment.NewLine,
							"上書きしてもよろしいですか？"
						}), "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
						{
							File.Copy(text4, text5, true);
						}
					}
				}
				if (this.TitleImage.Text != "")
				{
					File.Copy(this.TitleImage.Text, Path.Combine(text, project.Config.TitleImage), true);
				}
				if (this.MapChip.Text != "")
				{
					File.Copy(this.MapChip.Text, Path.Combine(text, project.Config.PatternImage), true);
				}
				if (this.EndingImage.Text != "")
				{
					File.Copy(this.EndingImage.Text, Path.Combine(text, project.Config.EndingImage), true);
				}
				if (this.GameoverImage.Text != "")
				{
					File.Copy(this.GameoverImage.Text, Path.Combine(text, project.Config.GameoverImage), true);
				}
				if (project.Runtime.Definitions.LayerSize.bytesize != 0 && this.LayerPattern.Text != "")
				{
					File.Copy(this.LayerPattern.Text, Path.Combine(text, project.Config.LayerImage), true);
				}
				project.SaveXML(text2);
				this.CreatedProject = text2;
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

		private void RuntimeSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.RuntimeSet.SelectedIndex != -1)
			{
				this.LayerPattern.Enabled = this.runtimeuselayer[this.RuntimeSet.SelectedIndex];
				this.LayerPatternBrowse.Enabled = this.runtimeuselayer[this.RuntimeSet.SelectedIndex];
				this.LPLabel.Enabled = this.runtimeuselayer[this.RuntimeSet.SelectedIndex];
				if (this.runtimeuselayer[this.RuntimeSet.SelectedIndex])
				{
					this.LayerUnsupNotice.Visible = false;
					this.ValidPathEmptiable(this.LayerPattern, new EventArgs());
				}
				else
				{
					this.LayerPattern.BackColor = Color.LightGray;
					this.LayerPattern.ForeColor = Color.Gray;
					this.LayerUnsupNotice.Visible = true;
				}
			}
			this.CheckInput();
		}

		private void UseDefaultPict_Click(object sender, EventArgs e)
		{
			this.MapChip.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultChip);
			if (this.LayerPattern.Enabled)
			{
				this.LayerPattern.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultLayerChip);
			}
			this.TitleImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultTitleImage);
			this.EndingImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultEndingImage);
			this.GameoverImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultGameoverImage);
		}

		public string CreatedProject = "";

		public List<string> runtimes = new List<string>();

		public List<Runtime> runtimedatas = new List<Runtime>();

		public List<bool> runtimeuselayer = new List<bool>();
	}
}

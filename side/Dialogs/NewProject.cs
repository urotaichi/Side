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
            InitializeComponent();
        }

        private void NewProject_Load(object sender, EventArgs e)
        {
            RuntimeSet.DropDownWidth = Width - RuntimeSet.Left;
            RootDir.Text = Global.config.lastData.ProjDirF;
            OK.Text = "お待ちください...";
            OK.Enabled = false;
        }

        private void NewProject_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
            {
                MessageBox.Show("ランタイムフォルダが見つかりません。" + Environment.NewLine + "Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Cancel;
                Close();
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
                                runtimes.Add(text);
                                runtimedatas.Add(runtime);
                                runtimeuselayer.Add(runtime.Definitions.LayerSize.bytesize != 0);
                                RuntimeSet.Items.Add($"{runtime.Definitions.Name} [Author:{runtime.Definitions.Author} Layer:"
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
            if (RuntimeSet.Items.Count == 0)
            {
                MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                RuntimeSet.SelectedIndex = 0;
            }
            OK.Text = "OK";
            CheckInput();
        }

        private void CheckInput()
        {
            if (ProjectName.Text != "" && RootDir.Text != "" && Directory.Exists(RootDir.Text) && (!LayerPattern.Enabled || LayerPattern.Text == "" || File.Exists(LayerPattern.Text)))
            {
                string[] array = new string[]
                {
                    MapChip.Text,
                    TitleImage.Text,
                    EndingImage.Text,
                    GameoverImage.Text
                };
                foreach (string text in array)
                {
                    if (text != "" && !File.Exists(text))
                    {
                        OK.Enabled = false;
                        return;
                    }
                }
                OK.Enabled = true;
                return;
            }
            OK.Enabled = false;
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
            CheckInput();
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
            CheckInput();
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
            CheckInput();
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
            CheckInput();
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
            CheckInput();
        }

        private void RootDirBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "プロジェクトのルートディレクトリを選択してください。";
            folderBrowserDialog.SelectedPath = RootDir.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                RootDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void MapChipBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.bmp)|*.gif;*.png;*.bmp|全てのファイル (*.*)|*.*";
            if (MapChip.Text != "")
            {
                openFileDialog.FileName = MapChip.Text;
            }
            else
            {
                openFileDialog.FileName = RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MapChip.Text = openFileDialog.FileName;
            }
        }

        private void LayerPatternBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.bmp)|*.gif;*.png;*.bmp|全てのファイル (*.*)|*.*";
            if (MapChip.Text != "")
            {
                openFileDialog.FileName = LayerPattern.Text;
            }
            else
            {
                openFileDialog.FileName = RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LayerPattern.Text = openFileDialog.FileName;
            }
        }

        private void TitleImageBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (TitleImage.Text != "")
            {
                openFileDialog.FileName = TitleImage.Text;
            }
            else
            {
                openFileDialog.FileName = RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TitleImage.Text = openFileDialog.FileName;
            }
        }

        private void EndingImageBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (EndingImage.Text != "")
            {
                openFileDialog.FileName = EndingImage.Text;
            }
            else
            {
                openFileDialog.FileName = RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                EndingImage.Text = openFileDialog.FileName;
            }
        }

        private void GameoverBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "*.gif";
            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
            if (GameoverImage.Text != "")
            {
                openFileDialog.FileName = GameoverImage.Text;
            }
            else
            {
                openFileDialog.FileName = RootDir.Text;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                GameoverImage.Text = openFileDialog.FileName;
            }
        }

        private void OK_Click(object sender, EventArgs ev)
        {
            List<string> list = new List<string>();
            try
            {
                Enabled = false;
                OK.Text = "生成中...";
                OK.Refresh();
                Global.config.lastData.ProjDir = RootDir.Text;
                string text = Path.Combine(RootDir.Text, ProjectName.Text);
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
                string text2 = Path.Combine(text, ProjectName.Text + Global.definition.ProjExt);
                string text3 = Path.Combine(Path.GetDirectoryName(runtimes[RuntimeSet.SelectedIndex]), Path.GetFileNameWithoutExtension(runtimes[RuntimeSet.SelectedIndex]));
                Project project = new Project();
                project.Name = ProjectName.Text;
                project.Runtime = runtimedatas[RuntimeSet.SelectedIndex];
                project.Config = ConfigurationOwner.LoadXML(Path.Combine(text3, runtimedatas[RuntimeSet.SelectedIndex].Definitions.Configurations));
                if (TitleImage.Text != "")
                {
                    list.Add(project.Config.TitleImage);
                    project.Config.TitleImage = Path.GetFileName(TitleImage.Text);
                }
                if (MapChip.Text != "")
                {
                    list.Add(project.Config.PatternImage);
                    project.Config.PatternImage = Path.GetFileName(MapChip.Text);
                }
                if (EndingImage.Text != "")
                {
                    list.Add(project.Config.EndingImage);
                    project.Config.EndingImage = Path.GetFileName(EndingImage.Text);
                }
                if (GameoverImage.Text != "")
                {
                    list.Add(project.Config.GameoverImage);
                    project.Config.GameoverImage = Path.GetFileName(GameoverImage.Text);
                }
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    if (LayerPattern.Text != "")
                    {
                        project.Config.LayerImage = Path.GetFileName(LayerPattern.Text);
                    }
                    project.LayerData = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
                    project.LayerData2 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
                    project.LayerData3 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
                    project.LayerData4 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.LayerSize.y];
                }
                project.StageData = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.StageSize.y];
                project.StageData2 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.StageSize.y];
                project.StageData3 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.StageSize.y];
                project.StageData4 = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.StageSize.y];
                project.MapData = new string[runtimedatas[RuntimeSet.SelectedIndex].Definitions.MapSize.y];
                project.Config.StageNum = (int)StageNum.Value;
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
                    if (!(Path.GetFileName(text4) == runtimedatas[RuntimeSet.SelectedIndex].Definitions.Configurations) && !list.Contains(Path.GetFileName(text4)))
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
                if (TitleImage.Text != "")
                {
                    File.Copy(TitleImage.Text, Path.Combine(text, project.Config.TitleImage), true);
                }
                if (MapChip.Text != "")
                {
                    File.Copy(MapChip.Text, Path.Combine(text, project.Config.PatternImage), true);
                }
                if (EndingImage.Text != "")
                {
                    File.Copy(EndingImage.Text, Path.Combine(text, project.Config.EndingImage), true);
                }
                if (GameoverImage.Text != "")
                {
                    File.Copy(GameoverImage.Text, Path.Combine(text, project.Config.GameoverImage), true);
                }
                if (project.Runtime.Definitions.LayerSize.bytesize != 0 && LayerPattern.Text != "")
                {
                    File.Copy(LayerPattern.Text, Path.Combine(text, project.Config.LayerImage), true);
                }
                project.SaveXML(text2);
                CreatedProject = text2;
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
                OK.Text = "OK";
                Enabled = true;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RuntimeSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RuntimeSet.SelectedIndex != -1)
            {
                LayerPattern.Enabled = runtimeuselayer[RuntimeSet.SelectedIndex];
                LayerPatternBrowse.Enabled = runtimeuselayer[RuntimeSet.SelectedIndex];
                LPLabel.Enabled = runtimeuselayer[RuntimeSet.SelectedIndex];
                if (runtimeuselayer[RuntimeSet.SelectedIndex])
                {
                    LayerUnsupNotice.Visible = false;
                    ValidPathEmptiable(LayerPattern, new EventArgs());
                }
                else
                {
                    LayerPattern.BackColor = Color.LightGray;
                    LayerPattern.ForeColor = Color.Gray;
                    LayerUnsupNotice.Visible = true;
                }
            }
            CheckInput();
        }

        private void UseDefaultPict_Click(object sender, EventArgs e)
        {
            MapChip.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultChip);
            if (LayerPattern.Enabled)
            {
                LayerPattern.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultLayerChip);
            }
            TitleImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultTitleImage);
            EndingImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultEndingImage);
            GameoverImage.Text = Path.Combine(Global.config.lastData.PictDirF, Global.config.lastData.DefaultGameoverImage);
        }

        public string CreatedProject = "";

        public List<string> runtimes = new List<string>();

        public List<Runtime> runtimedatas = new List<Runtime>();

        public List<bool> runtimeuselayer = new List<bool>();
    }
}

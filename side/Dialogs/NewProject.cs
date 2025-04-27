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
                MessageBox.Show($"ランタイムフォルダが見つかりません。{Environment.NewLine}Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
                                MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{Path.GetFileName(text)} : {string.Join(",", array2)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
                    MessageBox.Show($"読み込めませんでした:{Path.GetFileName(text)}{Environment.NewLine}{ex.Message}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
                string[] array =
                [
                    MapChip.Text,
                    TitleImage.Text,
                    EndingImage.Text,
                    GameoverImage.Text
                ];
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
            using FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "プロジェクトのルートディレクトリを選択してください。";
            folderBrowserDialog.SelectedPath = RootDir.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                RootDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void MapChipBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
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
            using OpenFileDialog openFileDialog = new();
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
            using OpenFileDialog openFileDialog = new();
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
            using OpenFileDialog openFileDialog = new();
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
            using OpenFileDialog openFileDialog = new();
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
            List<string> list = [];
            try
            {
                Enabled = false;
                OK.Text = "生成中...";
                OK.Refresh();
                Global.config.lastData.ProjDir = RootDir.Text;
                string text = Path.Combine(RootDir.Text, ProjectName.Text);
                if (Directory.Exists(text) && MessageBox.Show($"ディレクトリ{Environment.NewLine}\"{text}\"{Environment.NewLine}はすでに存在します。{Environment.NewLine}中に含まれるファイルは上書きされてしまう事があります。{Environment.NewLine}続行しますか？", "プロジェクト生成警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
                string text2 = Path.Combine(text, ProjectName.Text + Global.definition.ProjExt);
                string text3 = Path.Combine(Path.GetDirectoryName(runtimes[RuntimeSet.SelectedIndex]), Path.GetFileNameWithoutExtension(runtimes[RuntimeSet.SelectedIndex]));
                Project project = new()
                {
                    Name = ProjectName.Text,
                    Runtime = runtimedatas[RuntimeSet.SelectedIndex],
                    Config = ConfigurationOwner.LoadXML(Path.Combine(text3, runtimedatas[RuntimeSet.SelectedIndex].Definitions.Configurations))
                };
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
                if (project.Config.ScreenSize == "1")
                {
                    Global.state.MinimumStageSize = new Size(20, 15);
                }
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    if (LayerPattern.Text != "")
                    {
                        project.Config.LayerImage = Path.GetFileName(LayerPattern.Text);
                    }
                    void setLayerSize(ref Runtime.DefinedData.StageSizeData size)
                    {
                        if (size.x < Global.state.MinimumStageSize.Width) size.x = project.Runtime.Definitions.LayerSize.x;
                        if (size.y < Global.state.MinimumStageSize.Height) size.y = project.Runtime.Definitions.LayerSize.y;
                        size.bytesize = project.Runtime.Definitions.LayerSize.bytesize;
                    }
                    setLayerSize(ref project.Runtime.Definitions.LayerSize2);
                    setLayerSize(ref project.Runtime.Definitions.LayerSize3);
                    setLayerSize(ref project.Runtime.Definitions.LayerSize4);
                    project.LayerData = new string[project.Runtime.Definitions.LayerSize.y];
                    project.LayerData2 = new string[project.Runtime.Definitions.LayerSize2.y];
                    project.LayerData3 = new string[project.Runtime.Definitions.LayerSize3.y];
                    project.LayerData4 = new string[project.Runtime.Definitions.LayerSize4.y];
                }
                void setStageSize(ref Runtime.DefinedData.StageSizeData size)
                {
                    if (size.x < Global.state.MinimumStageSize.Width) size.x = project.Runtime.Definitions.StageSize.x;
                    if (size.y < Global.state.MinimumStageSize.Height) size.y = project.Runtime.Definitions.StageSize.y;
                    size.bytesize = project.Runtime.Definitions.StageSize.bytesize;
                }
                setStageSize(ref project.Runtime.Definitions.StageSize2);
                setStageSize(ref project.Runtime.Definitions.StageSize3);
                setStageSize(ref project.Runtime.Definitions.StageSize4);
                project.StageData = new string[project.Runtime.Definitions.StageSize.y];
                project.StageData2 = new string[project.Runtime.Definitions.StageSize2.y];
                project.StageData3 = new string[project.Runtime.Definitions.StageSize3.y];
                project.StageData4 = new string[project.Runtime.Definitions.StageSize4.y];
                project.MapData = new string[project.Runtime.Definitions.MapSize.y];
                project.Config.StageNum = (int)StageNum.Value;
                ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(text3, project.Runtime.Definitions.ChipDefinition));
                string character = chipDataClass.Mapchip[0].character;
                Project.setStageData(project.StageData, project.Runtime.Definitions.StageSize.x, character);
                Project.setStageData(project.StageData2, project.Runtime.Definitions.StageSize2.x, character);
                Project.setStageData(project.StageData3, project.Runtime.Definitions.StageSize3.x, character);
                Project.setStageData(project.StageData4, project.Runtime.Definitions.StageSize4.x, character);
                character = chipDataClass.WorldChip[0].character;
                Project.setStageData(project.MapData, project.Runtime.Definitions.MapSize.x, character);
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    character = chipDataClass.Layerchip[0].character;
                    Project.setStageData(project.LayerData, project.Runtime.Definitions.LayerSize.x, character);
                    Project.setStageData(project.LayerData2, project.Runtime.Definitions.LayerSize2.x, character);
                    Project.setStageData(project.LayerData3, project.Runtime.Definitions.LayerSize3.x, character);
                    Project.setStageData(project.LayerData4, project.Runtime.Definitions.LayerSize4.x, character);
                }
                Directory.CreateDirectory(text);
                foreach (string text4 in Directory.GetFiles(text3, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!(Path.GetFileName(text4) == project.Runtime.Definitions.Configurations) && !list.Contains(Path.GetFileName(text4)))
                    {
                        string text5 = Path.Combine(text, Path.GetFileName(text4));
                        if (!File.Exists(text5) || MessageBox.Show($"{text5}{Environment.NewLine}はすでに存在しています。{Environment.NewLine}上書きしてもよろしいですか？", "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
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
                MessageBox.Show($"プロジェクト生成に失敗しました。{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", "プロジェクト生成エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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

        public List<string> runtimes = [];

        public List<Runtime> runtimedatas = [];

        public List<bool> runtimeuselayer = [];
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
    public partial class ProjInheritance : Form
    {
        public ProjInheritance(string PrevProj)
        {
            InitializeComponent();
            PrevProjPath = PrevProj;
            try
            {
                PrevProject = Project.ParseXML(PrevProj);
                if (PrevProject == null)
                {
                    throw new Exception("Project Analysis Failured.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}ErrorReason:{ex.Message}", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            NewProjName.Text = $"{PrevProject.Name}_Inherited";
        }

        private void NewProjName_TextChanged(object sender, EventArgs e)
        {
            if (NewProjName.Text == "")
            {
                OKBtn.Enabled = false;
                return;
            }
            OKBtn.Enabled = true;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            string text = Path.Combine(Path.GetDirectoryName(PrevProjPath), NewProjName.Text + Global.definition.ProjExt);
            if (File.Exists(text) && MessageBox.Show($"{NewProjName.Text}{Global.definition.ProjExt}は同じディレクトリに既に存在しています。{Environment.NewLine}上書きするとそのプロジェクトのデータを失ってしまいます。{Environment.NewLine}本当に上書きしてもよろしいですか？", "上書きの警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                return;
            }
            Enabled = false;
            OKBtn.Text = "生成中...";
            OKBtn.Refresh();
            Project project = new()
            {
                Name = NewProjName.Text,
                Runtime = PrevProject.Runtime,
                Config = PrevProject.Config,
                CustomPartsDefinition = PrevProject.CustomPartsDefinition
            };
            if (PrevProject.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                void setLayerSize(ref Runtime.DefinedData.StageSizeData size)
                {
                    size.x = 180;
                    size.y = 30;
                    size.bytesize = PrevProject.Runtime.Definitions.LayerSize.bytesize;
                }
                setLayerSize(ref PrevProject.Runtime.Definitions.LayerSize);
                setLayerSize(ref PrevProject.Runtime.Definitions.LayerSize2);
                setLayerSize(ref PrevProject.Runtime.Definitions.LayerSize3);
                setLayerSize(ref PrevProject.Runtime.Definitions.LayerSize4);
                project.LayerData = new string[PrevProject.Runtime.Definitions.LayerSize.y];
                project.LayerData2 = new string[PrevProject.Runtime.Definitions.LayerSize2.y];
                project.LayerData3 = new string[PrevProject.Runtime.Definitions.LayerSize3.y];
                project.LayerData4 = new string[PrevProject.Runtime.Definitions.LayerSize4.y];
            }
            void setStageSize(ref Runtime.DefinedData.StageSizeData size)
            {
                size.x = 180;
                size.y = 30;
                size.bytesize = PrevProject.Runtime.Definitions.StageSize.bytesize;
            }
            setStageSize(ref PrevProject.Runtime.Definitions.StageSize);
            setStageSize(ref PrevProject.Runtime.Definitions.StageSize2);
            setStageSize(ref PrevProject.Runtime.Definitions.StageSize3);
            setStageSize(ref PrevProject.Runtime.Definitions.StageSize4);
            project.StageData = new string[PrevProject.Runtime.Definitions.StageSize.y];
            project.StageData2 = new string[PrevProject.Runtime.Definitions.StageSize2.y];
            project.StageData3 = new string[PrevProject.Runtime.Definitions.StageSize3.y];
            project.StageData4 = new string[PrevProject.Runtime.Definitions.StageSize4.y];
            project.MapData = new string[PrevProject.Runtime.Definitions.MapSize.y];
            ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(PrevProjPath), PrevProject.Runtime.Definitions.ChipDefinition));
            void setStageData(string[] data, int x, string character)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    StringBuilder stringBuilder = new();
                    for (int j = 0; j < x; j++)
                    {
                        stringBuilder.Append(character);
                    }
                    data[i] = stringBuilder.ToString();
                }
            }
            string character = chipDataClass.Mapchip[0].character;
            setStageData(project.StageData, PrevProject.Runtime.Definitions.StageSize.x, character);
            setStageData(project.StageData2, PrevProject.Runtime.Definitions.StageSize2.x, character);
            setStageData(project.StageData3, PrevProject.Runtime.Definitions.StageSize3.x, character);
            setStageData(project.StageData4, PrevProject.Runtime.Definitions.StageSize4.x, character);
            character = chipDataClass.WorldChip[0].character;
            setStageData(project.MapData, PrevProject.Runtime.Definitions.MapSize.x, character);
            if (PrevProject.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                character = chipDataClass.Layerchip[0].character;
                setStageData(project.LayerData, PrevProject.Runtime.Definitions.LayerSize.x, character);
                setStageData(project.LayerData2, PrevProject.Runtime.Definitions.LayerSize2.x, character);
                setStageData(project.LayerData3, PrevProject.Runtime.Definitions.LayerSize3.x, character);
                setStageData(project.LayerData4, PrevProject.Runtime.Definitions.LayerSize4.x, character);
            }
            project.SaveXML(text);
            NewProjectName = text;
            DialogResult = DialogResult.OK;
            Close();
        }

        public string NewProjectName = "";

        public string PrevProjPath;

        public Project PrevProject;
    }
}

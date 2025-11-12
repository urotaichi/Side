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
            Project.SetAllStageData(project, Path.GetDirectoryName(PrevProjPath), PrevProject);
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

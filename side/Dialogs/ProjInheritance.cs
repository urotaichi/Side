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
				MessageBox.Show("プロジェクトをロードできませんでした。" + Environment.NewLine + "ErrorReason:" + ex.Message, "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			NewProjName.Text = PrevProject.Name + "_Inherited";
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
			if (File.Exists(text) && MessageBox.Show(string.Concat(new string[]
			{
				NewProjName.Text,
				Global.definition.ProjExt,
				"は同じディレクトリに既に存在しています。",
				Environment.NewLine,
				"上書きするとそのプロジェクトのデータを失ってしまいます。",
				Environment.NewLine,
				"本当に上書きしてもよろしいですか？"
			}), "上書きの警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return;
			}
			base.Enabled = false;
			OKBtn.Text = "生成中...";
			OKBtn.Refresh();
			Project project = new Project();
			project.Name = NewProjName.Text;
			project.Runtime = PrevProject.Runtime;
			project.Config = PrevProject.Config;
			if (PrevProject.Runtime.Definitions.LayerSize.bytesize != 0)
			{
				project.LayerData = new string[PrevProject.Runtime.Definitions.LayerSize.y];
				project.LayerData2 = new string[PrevProject.Runtime.Definitions.LayerSize.y];
				project.LayerData3 = new string[PrevProject.Runtime.Definitions.LayerSize.y];
				project.LayerData4 = new string[PrevProject.Runtime.Definitions.LayerSize.y];
			}
			project.StageData = new string[PrevProject.Runtime.Definitions.StageSize.y];
			project.StageData2 = new string[PrevProject.Runtime.Definitions.StageSize.y];
			project.StageData3 = new string[PrevProject.Runtime.Definitions.StageSize.y];
			project.StageData4 = new string[PrevProject.Runtime.Definitions.StageSize.y];
			project.MapData = new string[PrevProject.Runtime.Definitions.MapSize.y];
			ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(PrevProjPath), PrevProject.Runtime.Definitions.ChipDefinition));
			string character = chipDataClass.Mapchip[0].character;
			for (int i = 0; i < PrevProject.StageData.Length; i++)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < PrevProject.Runtime.Definitions.StageSize.x; j++)
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
			if (PrevProject.Runtime.Definitions.LayerSize.bytesize != 0)
			{
				character = chipDataClass.Layerchip[0].character;
				for (int m = 0; m < PrevProject.LayerData.Length; m++)
				{
					StringBuilder stringBuilder3 = new StringBuilder();
					for (int n = 0; n < PrevProject.Runtime.Definitions.LayerSize.x; n++)
					{
						stringBuilder3.Append(character);
					}
					project.LayerData[m] = stringBuilder3.ToString();
				}
				project.LayerData2 = (string[])project.LayerData.Clone();
				project.LayerData3 = (string[])project.LayerData.Clone();
				project.LayerData4 = (string[])project.LayerData.Clone();
			}
			project.SaveXML(text);
			NewProjectName = text;
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		public string NewProjectName = "";

		public string PrevProjPath;

		public Project PrevProject;
	}
}

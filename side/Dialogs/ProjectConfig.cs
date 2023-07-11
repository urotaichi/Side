using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
	public partial class ProjectConfig : Form
	{
		public ProjectConfig()
		{
			this.InitializeComponent();
			this.OutHeader.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
			this.OutMiddle.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
			this.OutFooter.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
		}

		private void ProjectConfig_Load(object sender, EventArgs e)
		{
			this.ProjectName.Text = Global.cpd.project.Name;
			this.OutDir.Text = Global.cpd.runtime.DefaultConfigurations.OutputDir;
			this.OutExt.Text = Global.cpd.runtime.DefaultConfigurations.FileExt;
			this.StageF.Text = Global.cpd.runtime.DefaultConfigurations.StageParam;
			this.LayerF.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam;
			this.StageF2.Text = Global.cpd.runtime.DefaultConfigurations.StageParam2;
			this.LayerF2.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam2;
			this.StageF3.Text = Global.cpd.runtime.DefaultConfigurations.StageParam3;
			this.LayerF3.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam3;
			this.StageF4.Text = Global.cpd.runtime.DefaultConfigurations.StageParam4;
			this.LayerF4.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam4;
			this.MapF.Text = Global.cpd.runtime.DefaultConfigurations.MapParam;
			this.ProjNum.Value = Global.cpd.project.Config.StageNum;
			this.UseWorldmap.Checked = Global.cpd.project.Config.UseWorldmap;
            this.Use3rdMapData.Checked = Global.cpd.project.Use3rdMapData;

            this.OutHeader.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.HeaderHTML);
			this.OutMiddle.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.MiddleHTML);
			this.OutFooter.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.FooterHTML);

			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					this.OutputReplaceView.Rows.Add(new string[]
					{
						htmlreplaceData.Name,
						htmlreplaceData.Value
					});
				}
			}
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			Global.cpd.project.Name = this.ProjectName.Text;
			Global.cpd.runtime.DefaultConfigurations.OutputDir = this.OutDir.Text;
			Global.cpd.runtime.DefaultConfigurations.FileExt = this.OutExt.Text;
			Global.cpd.runtime.DefaultConfigurations.StageParam = this.StageF.Text;
			Global.cpd.runtime.DefaultConfigurations.LayerParam = this.LayerF.Text;
			Global.cpd.runtime.DefaultConfigurations.StageParam2 = this.StageF2.Text;
			Global.cpd.runtime.DefaultConfigurations.LayerParam2 = this.LayerF2.Text;
			Global.cpd.runtime.DefaultConfigurations.StageParam3 = this.StageF3.Text;
			Global.cpd.runtime.DefaultConfigurations.LayerParam3 = this.LayerF3.Text;
			Global.cpd.runtime.DefaultConfigurations.StageParam4 = this.StageF4.Text;
			Global.cpd.runtime.DefaultConfigurations.LayerParam4 = this.LayerF4.Text;
			Global.cpd.runtime.DefaultConfigurations.MapParam = this.MapF.Text;
			Global.cpd.project.Config.StageNum = (int)this.ProjNum.Value;

			Global.cpd.runtime.DefaultConfigurations.HeaderHTML = Subsystem.EncodeBase64(this.OutHeader.Text);
			Global.cpd.runtime.DefaultConfigurations.MiddleHTML = Subsystem.EncodeBase64(this.OutMiddle.Text);
			Global.cpd.runtime.DefaultConfigurations.FooterHTML = Subsystem.EncodeBase64(this.OutFooter.Text);

			Global.cpd.project.Config.UseWorldmap = this.UseWorldmap.Checked;
            Global.cpd.project.Use3rdMapData = this.Use3rdMapData.Checked;
            Global.state.EditFlag = true;
			List<HTMLReplaceData> list = new List<HTMLReplaceData>();
			foreach (object obj in ((IEnumerable)this.OutputReplaceView.Rows))
			{
				DataGridViewRow dataGridViewRow = (DataGridViewRow)obj;
				if (dataGridViewRow.Cells[0].Value != null)
				{
					list.Add(new HTMLReplaceData(dataGridViewRow.Cells[0].Value.ToString(), (dataGridViewRow.Cells[1].Value == null) ? "" : dataGridViewRow.Cells[1].Value.ToString()));
				}
			}
			Global.cpd.runtime.DefaultConfigurations.OutputReplace = list.ToArray();
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void TextUndo_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Undo();
		}

		private void TextCut_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Cut();
		}

		private void TextCopy_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Copy();
		}

		private void TextPaste_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Paste();
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
	// Token: 0x02000007 RID: 7
	public partial class ProjectConfig : Form
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002352 File Offset: 0x00000552
		public ProjectConfig()
		{
			this.InitializeComponent();
			this.OutHeader.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
			this.OutMiddle.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
			this.OutFooter.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000097D8 File Offset: 0x000079D8
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

		// Token: 0x0600003D RID: 61 RVA: 0x00009A58 File Offset: 0x00007C58
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

		// Token: 0x0600003E RID: 62 RVA: 0x0000237A File Offset: 0x0000057A
		private void TextUndo_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Undo();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002391 File Offset: 0x00000591
		private void TextCut_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Cut();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000023A8 File Offset: 0x000005A8
		private void TextCopy_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Copy();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000023BF File Offset: 0x000005BF
		private void TextPaste_Click(object sender, EventArgs e)
		{
			((RichTextBox)this.EditContext.SourceControl).Paste();
		}
	}
}

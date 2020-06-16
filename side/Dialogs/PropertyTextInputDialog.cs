using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	// Token: 0x02000017 RID: 23
	public partial class PropertyTextInputDialog : Form
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x0000281B File Offset: 0x00000A1B
		public PropertyTextInputDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002834 File Offset: 0x00000A34
		private void PropertyTextInputDialog_Shown(object sender, EventArgs e)
		{
			this.InputText.Text = this.InputStr;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00002847 File Offset: 0x00000A47
		private void PropertyTextInputDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.InputStr = this.InputText.Text;
		}

		// Token: 0x040000BB RID: 187
		public string InputStr = "";
	}
}

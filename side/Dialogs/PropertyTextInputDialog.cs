using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class PropertyTextInputDialog : Form
	{
		public PropertyTextInputDialog()
		{
			this.InitializeComponent();
		}

		private void PropertyTextInputDialog_Shown(object sender, EventArgs e)
		{
			this.InputText.Text = this.InputStr;
		}

		private void PropertyTextInputDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.InputStr = this.InputText.Text;
		}

		public string InputStr = "";
	}
}

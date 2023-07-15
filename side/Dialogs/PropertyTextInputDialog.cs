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
			InitializeComponent();
		}

		private void PropertyTextInputDialog_Shown(object sender, EventArgs e)
		{
			InputText.Text = InputStr;
		}

		private void PropertyTextInputDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			InputStr = InputText.Text;
		}

		public string InputStr = "";
	}
}

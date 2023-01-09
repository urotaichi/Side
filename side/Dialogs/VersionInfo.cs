using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
	public partial class VersionInfo : Form
	{
		public VersionInfo()
		{
			this.InitializeComponent();
		}

		private void VersionInfo_Load(object sender, EventArgs e)
		{
			this.VersionLabel.Text = Global.definition.Version;
		}

		private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel)sender).Text);
		}

		private void ViewDat_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Process.Start(e.LinkText);
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}

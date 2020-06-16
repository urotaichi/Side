using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
	// Token: 0x02000038 RID: 56
	public partial class VersionInfo : Form
	{
		// Token: 0x06000226 RID: 550 RVA: 0x00003722 File Offset: 0x00001922
		public VersionInfo()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00003730 File Offset: 0x00001930
		private void VersionInfo_Load(object sender, EventArgs e)
		{
			this.VersionLabel.Text = Global.definition.Version;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00003747 File Offset: 0x00001947
		private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel)sender).Text);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000375A File Offset: 0x0000195A
		private void ViewDat_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Process.Start(e.LinkText);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x000030DD File Offset: 0x000012DD
		private void OKButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}

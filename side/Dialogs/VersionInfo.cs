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
            InitializeComponent();
        }

        private void VersionInfo_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = Global.definition.Version;
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = ((LinkLabel)sender).Text, UseShellExecute = true });
        }

        private void ViewDat_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.LinkText, UseShellExecute = true });
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

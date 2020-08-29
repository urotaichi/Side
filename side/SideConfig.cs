using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MasaoPlus
{
	// Token: 0x0200002A RID: 42
	public partial class SideConfig : Form
	{
		// Token: 0x0600012D RID: 301 RVA: 0x00002C0E File Offset: 0x00000E0E
		public SideConfig()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00018870 File Offset: 0x00016A70
		private void SideConfig_Load(object sender, EventArgs e)
		{
			this.OutputFileEncode.Text = Global.config.localSystem.FileEncoding.WebName;
			this.UseZoomInterp.Checked = Global.config.draw.StageInterpolation;
			this.UseClassicCLInterp.Checked = Global.config.draw.ClassicChipListInterpolation;
			this.CursorPageScroll.Checked = Global.config.draw.PageScroll;
			this.TempFileName.Text = Global.config.testRun.TempFile;
			this.UseIntegBrow.Checked = Global.config.testRun.UseIntegratedBrowser;
			this.UnuseIntegBrow.Enabled = !this.UseIntegBrow.Checked;
			this.KillExternalBrow.Checked = Global.config.testRun.KillTestrunOnFocus;
			this.BrowPath.Text = Global.config.localSystem.UsingWebBrowser;
			this.UseQTRun.Checked = Global.config.testRun.QuickTestrun;
			OperatingSystem osversion = Environment.OSVersion;
			this.ReversePosition.Checked = Global.config.localSystem.ReverseTabView;
			this.EnableAutoUpdate.Checked = Global.config.localSystem.CheckAutoUpdate;
			this.RCContextMenu.Checked = Global.config.draw.RightClickMenu;
			this.ExDraw.Checked = Global.config.draw.ExtendDraw;
			this.EnableAlpha.Checked = Global.config.draw.AlphaBlending;
			this.ClassicSelectionMode.SelectedIndex = (int)Global.config.draw.SelDrawMode;
			this.BufferingDraw.Checked = Global.config.draw.UseBufferingDraw;
			this.ChipSkip.Checked = Global.config.draw.SkipFirstChip;
			this.ReuseDraw.Checked = Global.config.draw.SkipBufferedDraw;
			this.IntegrateEditorId.Checked = Global.config.localSystem.IntegrateEditorId;
			this.UsePropTextDialog.Checked = Global.config.localSystem.UsePropExTextEditor;
			this.OutPutInititalSourceCode.Checked = Global.config.localSystem.OutPutInititalSourceCode;
			this.WrapPropText.Checked = Global.config.localSystem.WrapPropText;
			this.StartWithCL.Checked = Global.config.draw.StartUpWithClassicCL;
			this.WheelHorz.Checked = Global.config.draw.HScrollDefault;
			if (!Native.CheckRegistryAddmittion())
			{
				if (osversion.Version.Major >= 6)
				{
					RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false);
					if (registryKey == null || (int)registryKey.GetValue("EnableLUA") != 0)
					{
						Native.USER32.SendMessage(this.RegistProjFile.Handle, 5644U, 0, 1);
						Native.USER32.SendMessage(this.UnregistProjFile.Handle, 5644U, 0, 1);
						this.UseUAC = true;
					}
					else
					{
						this.RegistProjFile.Enabled = false;
						this.UnregistProjFile.Enabled = false;
					}
				}
				else
				{
					this.RegistProjFile.Enabled = false;
					this.UnregistProjFile.Enabled = false;
				}
			}
			this.FormShown = true;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00018B90 File Offset: 0x00016D90
		private void Accept_Click(object sender, EventArgs e)
		{
			Global.config.localSystem.FileEncoding = Encoding.GetEncoding(this.OutputFileEncode.Items[this.OutputFileEncode.SelectedIndex].ToString());
			Global.config.draw.StageInterpolation = this.UseZoomInterp.Checked;
			Global.config.draw.ClassicChipListInterpolation = this.UseClassicCLInterp.Checked;
			Global.config.draw.PageScroll = this.CursorPageScroll.Checked;
			Global.config.testRun.TempFile = this.TempFileName.Text;
			Global.config.testRun.UseIntegratedBrowser = this.UseIntegBrow.Checked;
			Global.config.testRun.KillTestrunOnFocus = this.KillExternalBrow.Checked;
			Global.config.localSystem.UsingWebBrowser = this.BrowPath.Text;
			Global.config.testRun.QuickTestrun = this.UseQTRun.Checked;
			Global.config.localSystem.ReverseTabView = this.ReversePosition.Checked;
			Global.config.localSystem.CheckAutoUpdate = this.EnableAutoUpdate.Checked;
			Global.config.draw.RightClickMenu = this.RCContextMenu.Checked;
			Global.config.draw.ExtendDraw = this.ExDraw.Checked;
			Global.config.draw.AlphaBlending = this.EnableAlpha.Checked;
			Global.config.draw.SelDrawMode = (Config.Draw.SelectionDrawMode)this.ClassicSelectionMode.SelectedIndex;
			Global.config.draw.UseBufferingDraw = this.BufferingDraw.Checked;
			Global.config.draw.SkipFirstChip = this.ChipSkip.Checked;
			Global.config.draw.SkipBufferedDraw = this.ReuseDraw.Checked;
			Global.config.localSystem.IntegrateEditorId = this.IntegrateEditorId.Checked;
			Global.config.localSystem.UsePropExTextEditor = this.UsePropTextDialog.Checked;
			Global.config.localSystem.OutPutInititalSourceCode = this.OutPutInititalSourceCode.Checked;
			Global.config.localSystem.WrapPropText = this.WrapPropText.Checked;
			Global.config.draw.StartUpWithClassicCL = this.StartWithCL.Checked;
			Global.config.draw.HScrollDefault = this.WheelHorz.Checked;
			base.Close();
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00002C1C File Offset: 0x00000E1C
		private void UseIntegBrow_CheckedChanged(object sender, EventArgs e)
		{
			this.UnuseIntegBrow.Enabled = !this.UseIntegBrow.Checked;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00018DFC File Offset: 0x00016FFC
		private void RegistProjFile_Click(object sender, EventArgs e)
		{
			try
			{
				this.UnregistProjFile.Enabled = false;
				this.RegistProjFile.Enabled = false;
				if (this.UseUAC)
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.UseShellExecute = true;
					processStartInfo.WorkingDirectory = Application.StartupPath;
					processStartInfo.FileName = Path.GetFileName(Application.ExecutablePath);
					processStartInfo.Arguments = "/reg";
					processStartInfo.Verb = "runas";
					try
					{
						Process process = Process.Start(processStartInfo);
						process.WaitForExit();
						MessageBox.Show("関連付けしました。", "関連付け処理成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						goto IL_AE;
					}
					catch (Exception ex)
					{
						MessageBox.Show("関連付けを実行できませんでした。" + Environment.NewLine + ex.Message, "関連付け処理失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						goto IL_AE;
					}
				}
				Native.SetSideFileRelate();
				IL_AE:;
			}
			finally
			{
				this.UnregistProjFile.Enabled = true;
				this.RegistProjFile.Enabled = true;
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00018EF0 File Offset: 0x000170F0
		private void UnregistProjFile_Click(object sender, EventArgs e)
		{
			try
			{
				this.UnregistProjFile.Enabled = false;
				this.RegistProjFile.Enabled = false;
				if (this.UseUAC)
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.UseShellExecute = true;
					processStartInfo.WorkingDirectory = Application.StartupPath;
					processStartInfo.FileName = Path.GetFileName(Application.ExecutablePath);
					processStartInfo.Arguments = "/unreg";
					processStartInfo.Verb = "runas";
					try
					{
						Process process = Process.Start(processStartInfo);
						process.WaitForExit();
						MessageBox.Show("関連付けを解除しました。", "関連付け処理成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						goto IL_AE;
					}
					catch (Exception ex)
					{
						MessageBox.Show("関連付けを実行できませんでした。" + Environment.NewLine + ex.Message, "関連付け処理失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						goto IL_AE;
					}
				}
				Native.DeleteSideFileRelate();
				IL_AE:;
			}
			finally
			{
				this.UnregistProjFile.Enabled = true;
				this.RegistProjFile.Enabled = true;
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00002C37 File Offset: 0x00000E37
		private void ChipSkip_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ChipSkip.Checked)
			{
				this.ReuseDraw.Checked = false;
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00018FE4 File Offset: 0x000171E4
		private void ReuseDraw_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ReuseDraw.Checked)
			{
				if (this.FormShown && MessageBox.Show("この機能は未だ多くの問題を抱えています。" + Environment.NewLine + "本当にオンにしますか？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
				{
					this.ReuseDraw.Checked = false;
					return;
				}
				this.ChipSkip.Checked = false;
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00019044 File Offset: 0x00017244
		private void BPSelect_Click(object sender, EventArgs e)
		{
			using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
			{
				openFileDialog.CheckFileExists = true;
				openFileDialog.Filter = "アプリケーション(*.exe)|*.exe";
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					this.BrowPath.Text = openFileDialog.FileName;
				}
			}
		}

		// Token: 0x0400016D RID: 365
		private bool UseUAC;

		// Token: 0x0400016E RID: 366
		private bool FormShown;
	}
}

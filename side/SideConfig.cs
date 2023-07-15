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
	public partial class SideConfig : Form
	{
		public SideConfig()
		{
			InitializeComponent();
		}

		private void SideConfig_Load(object sender, EventArgs e)
		{
			OutputFileEncode.Text = Global.config.localSystem.FileEncoding.WebName;
			UseZoomInterp.Checked = Global.config.draw.StageInterpolation;
			UseClassicCLInterp.Checked = Global.config.draw.ClassicChipListInterpolation;
			CursorPageScroll.Checked = Global.config.draw.PageScroll;
			TempFileName.Text = Global.config.testRun.TempFile;
			UseIntegBrow.Checked = Global.config.testRun.UseIntegratedBrowser;
			UnuseIntegBrow.Enabled = !UseIntegBrow.Checked;
			KillExternalBrow.Checked = Global.config.testRun.KillTestrunOnFocus;
			BrowPath.Text = Global.config.localSystem.UsingWebBrowser;
			UseQTRun.Checked = Global.config.testRun.QuickTestrun;
			OperatingSystem osversion = Environment.OSVersion;
			ReversePosition.Checked = Global.config.localSystem.ReverseTabView;
			EnableAutoUpdate.Checked = Global.config.localSystem.CheckAutoUpdate;
			RCContextMenu.Checked = Global.config.draw.RightClickMenu;
			ExDraw.Checked = Global.config.draw.ExtendDraw;
			EnableAlpha.Checked = Global.config.draw.AlphaBlending;
			ClassicSelectionMode.SelectedIndex = (int)Global.config.draw.SelDrawMode;
			BufferingDraw.Checked = Global.config.draw.UseBufferingDraw;
			ChipSkip.Checked = Global.config.draw.SkipFirstChip;
			ReuseDraw.Checked = Global.config.draw.SkipBufferedDraw;
			IntegrateEditorId.Checked = Global.config.localSystem.IntegrateEditorId;
			UsePropTextDialog.Checked = Global.config.localSystem.UsePropExTextEditor;
			OutPutInititalSourceCode.Checked = Global.config.localSystem.OutPutInititalSourceCode;
			WrapPropText.Checked = Global.config.localSystem.WrapPropText;
			StartWithCL.Checked = Global.config.draw.StartUpWithClassicCL;
			WheelHorz.Checked = Global.config.draw.HScrollDefault;
			if (!Native.CheckRegistryAddmittion())
			{
				if (osversion.Version.Major >= 6)
				{
					RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false);
					if (registryKey == null || (int)registryKey.GetValue("EnableLUA") != 0)
					{
						Native.USER32.SendMessage(RegistProjFile.Handle, 5644U, 0, 1);
						Native.USER32.SendMessage(UnregistProjFile.Handle, 5644U, 0, 1);
						UseUAC = true;
					}
					else
					{
						RegistProjFile.Enabled = false;
						UnregistProjFile.Enabled = false;
					}
				}
				else
				{
					RegistProjFile.Enabled = false;
					UnregistProjFile.Enabled = false;
				}
			}
			FormShown = true;
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			Global.config.localSystem.FileEncoding = Encoding.GetEncoding(OutputFileEncode.Items[OutputFileEncode.SelectedIndex].ToString());
			Global.config.draw.StageInterpolation = UseZoomInterp.Checked;
			Global.config.draw.ClassicChipListInterpolation = UseClassicCLInterp.Checked;
			Global.config.draw.PageScroll = CursorPageScroll.Checked;
			Global.config.testRun.TempFile = TempFileName.Text;
			Global.config.testRun.UseIntegratedBrowser = UseIntegBrow.Checked;
			Global.config.testRun.KillTestrunOnFocus = KillExternalBrow.Checked;
			Global.config.localSystem.UsingWebBrowser = BrowPath.Text;
			Global.config.testRun.QuickTestrun = UseQTRun.Checked;
			Global.config.localSystem.ReverseTabView = ReversePosition.Checked;
			Global.config.localSystem.CheckAutoUpdate = EnableAutoUpdate.Checked;
			Global.config.draw.RightClickMenu = RCContextMenu.Checked;
			Global.config.draw.ExtendDraw = ExDraw.Checked;
			Global.config.draw.AlphaBlending = EnableAlpha.Checked;
			Global.config.draw.SelDrawMode = (Config.Draw.SelectionDrawMode)ClassicSelectionMode.SelectedIndex;
			Global.config.draw.UseBufferingDraw = BufferingDraw.Checked;
			Global.config.draw.SkipFirstChip = ChipSkip.Checked;
			Global.config.draw.SkipBufferedDraw = ReuseDraw.Checked;
			Global.config.localSystem.IntegrateEditorId = IntegrateEditorId.Checked;
			Global.config.localSystem.UsePropExTextEditor = UsePropTextDialog.Checked;
			Global.config.localSystem.OutPutInititalSourceCode = OutPutInititalSourceCode.Checked;
			Global.config.localSystem.WrapPropText = WrapPropText.Checked;
			Global.config.draw.StartUpWithClassicCL = StartWithCL.Checked;
			Global.config.draw.HScrollDefault = WheelHorz.Checked;
            Close();
		}

		private void UseIntegBrow_CheckedChanged(object sender, EventArgs e)
		{
			UnuseIntegBrow.Enabled = !UseIntegBrow.Checked;
		}

		private void RegistProjFile_Click(object sender, EventArgs e)
		{
			try
			{
				UnregistProjFile.Enabled = false;
				RegistProjFile.Enabled = false;
				if (UseUAC)
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
				UnregistProjFile.Enabled = true;
				RegistProjFile.Enabled = true;
			}
		}

		private void UnregistProjFile_Click(object sender, EventArgs e)
		{
			try
			{
				UnregistProjFile.Enabled = false;
				RegistProjFile.Enabled = false;
				if (UseUAC)
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
				UnregistProjFile.Enabled = true;
				RegistProjFile.Enabled = true;
			}
		}

		private void ChipSkip_CheckedChanged(object sender, EventArgs e)
		{
			if (ChipSkip.Checked)
			{
				ReuseDraw.Checked = false;
			}
		}

		private void ReuseDraw_CheckedChanged(object sender, EventArgs e)
		{
			if (ReuseDraw.Checked)
			{
				if (FormShown && MessageBox.Show("この機能は未だ多くの問題を抱えています。" + Environment.NewLine + "本当にオンにしますか？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
				{
					ReuseDraw.Checked = false;
					return;
				}
				ChipSkip.Checked = false;
			}
		}

		private void BPSelect_Click(object sender, EventArgs e)
		{
            using System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "アプリケーション(*.exe)|*.exe";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BrowPath.Text = openFileDialog.FileName;
            }
        }

		private bool UseUAC;

		private bool FormShown;
	}
}

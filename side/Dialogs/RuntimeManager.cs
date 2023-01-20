using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class RuntimeManager : Form
	{
		public RuntimeManager()
		{
			this.InitializeComponent();
		}

		private void InstalledRuntime_Shown(object sender, EventArgs e)
		{
			Application.DoEvents();
			this.Text = "ランタイムをロードしています...";
			base.Enabled = false;
			if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
			{
				MessageBox.Show("ランタイムフォルダが見つかりません。" + Environment.NewLine + "Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			string[] files = Directory.GetFiles(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir), "*.xml", SearchOption.TopDirectoryOnly);
			try
			{
				foreach (string text in files)
				{
					try
					{
						string text2 = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
						if (Directory.Exists(text2))
						{
							Runtime runtime = Runtime.ParseXML(text);
							if (runtime != null)
							{
								string[] array2 = Runtime.CheckFiles(text2, runtime);
								if (array2.Length == 0)
								{
									this.runtimedatas.Add(runtime);
									this.runtimefiles.Add(text);
									this.RuntimeViewer.Items.Add(new ListViewItem(new string[]
									{
										runtime.Definitions.Name,
										runtime.Definitions.Author,
										runtime.Definitions.Package,
										(runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×",
										runtime.Definitions.DefVersion.ToString("F2")
									}));
								}
								else
								{
									MessageBox.Show(string.Concat(new string[]
									{
										"必須ファイルが欠落しています。",
										Environment.NewLine,
										Path.GetFileName(text),
										" : ",
										string.Join(",", array2)
									}), "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								}
							}
						}
						else
						{
							MessageBox.Show("ランタイムフォルダが見つかりません:" + Path.GetFileName(text), "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("読み込めませんでした:" + Path.GetFileName(text) + Environment.NewLine + ex.Message, "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
			}
			finally
			{
				this.Text = "ランタイム マネージャ";
				base.Enabled = true;
			}
			if (this.RuntimeViewer.Items.Count == 0)
			{
				MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void NewRuntimeInstall_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					"ランタイムパッケージ(*",
					Global.definition.RuntimeArchiveExt,
					")|*",
					Global.definition.RuntimeArchiveExt
				});
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						base.Enabled = false;
						this.Text = "ランタイムをインストールしています...";
						if (Subsystem.InstallRuntime(openFileDialog.FileName))
						{
							MessageBox.Show("ランタイムインストールが完了しました。", "インストール成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						}
					}
					finally
					{
						base.DialogResult = DialogResult.Retry;
						base.Close();
					}
				}
			}
		}

		private void RuntimeViewer_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.RuntimeViewer.SelectedIndices.Count == 0)
			{
				this.NetworkUpdate.Enabled = false;
				this.Uninstall.Enabled = false;
				return;
			}
			if (this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]].Definitions.Update != null && this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]].Definitions.Update != "")
			{
				this.NetworkUpdate.Enabled = Global.definition.IsAutoUpdateEnabled;
			}
			else
			{
				this.NetworkUpdate.Enabled = false;
			}
			this.Uninstall.Enabled = true;
		}

		private void NetworkUpdate_Click(object sender, EventArgs e)
		{
			if (this.RuntimeViewer.SelectedIndices.Count == 0)
			{
				MessageBox.Show("対象が選択されていません。", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			Runtime runtime = this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]];
			if (runtime.Definitions.Update == null || runtime.Definitions.Update == "")
			{
				MessageBox.Show("更新先が記述されていません。", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			this.ur = this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]];
			if (MessageBox.Show(this.ur.Definitions.Name + "の更新を確認しますか？", "アップデート確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			string update = this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]].Definitions.Update;
			this.Text = "更新を受信しています...";
			this.DownProgress.Visible = true;
			base.Enabled = false;
			this.tempfile = Path.GetTempFileName();
			this.dlClient = new WebClient();
			this.dlClient.DownloadProgressChanged += this.dlClient_DownloadProgressChanged;
			this.dlClient.DownloadFileCompleted += this.dlClient_DownloadFileCompleted;
			Uri address = new Uri(update);
			try
			{
				this.dlClient.DownloadFileAsync(address, this.tempfile);
			}
			catch (Exception ex)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + ex.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Retry;
				base.Close();
			}
		}

		private void dlClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
            {
                MessageBox.Show("更新できませんでした。" + Environment.NewLine + e.Error.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.DialogResult = DialogResult.Retry;
				base.Close();
				return;
			}
			UpdateData updateData = UpdateData.ParseXML(this.tempfile);
			if (this.ur.Definitions.DefVersion >= updateData.DefVersion)
			{
				MessageBox.Show("最新のランタイムです。更新の必要はありません。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else if (Global.definition.CheckVersion < updateData.RequireLower)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"最新のランタイムはこのバージョンの",
					Global.definition.AppName,
					"には対応していません。",
					Environment.NewLine,
					"最新版へ更新してください。"
				}), "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else if (MessageBox.Show(string.Concat(new string[]
			{
				"新しいランタイムがリリースされています。",
				Environment.NewLine,
				this.ur.Definitions.DefVersion.ToString(),
				" -> ",
				updateData.DefVersion.ToString(),
				Environment.NewLine,
				"更新しますか？"
			}), "更新の確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				this.dlClient.Dispose();
				this.tempfile = Path.GetTempFileName();
				this.dlClient = new WebClient();
				this.dlClient.DownloadProgressChanged += this.dlClient_DownloadProgressChanged;
                this.dlClient.DownloadProgressChanged += this.dlClient_DownloadTaskbarManagerProgressChanged;
                this.dlClient.DownloadFileCompleted += this.dlClient_DownloadFileCompleted_Package;
				Uri address = new Uri(updateData.Update);
				try
				{
					this.Text = "ランタイムをダウンロードしています...";
					this.dlClient.DownloadFileAsync(address, this.tempfile);
					return;
				}
				catch (Exception ex)
                {
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    MessageBox.Show("更新できませんでした。" + Environment.NewLine + ex.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                    base.DialogResult = DialogResult.Retry;
					base.Close();
				}
            }
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            base.DialogResult = DialogResult.Retry;
			base.Close();
		}

		private void dlClient_DownloadFileCompleted_Package(object sender, AsyncCompletedEventArgs e)
        {
            this.Text = "ランタイムをインストールしています...";
			Subsystem.InstallRuntime(this.tempfile);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            base.DialogResult = DialogResult.Retry;
			base.Close();
		}

		private void dlClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.DownProgress.Value = e.ProgressPercentage;
        }

        private void dlClient_DownloadTaskbarManagerProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressValue(e.ProgressPercentage, 100);
        }

        private void Uninstall_Click(object sender, EventArgs e)
		{
			if (this.RuntimeViewer.SelectedIndices.Count == 0)
			{
				MessageBox.Show("対象が選択されていません。", "アンインストールエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			Runtime runtime = this.runtimedatas[this.RuntimeViewer.SelectedIndices[0]];
			if (MessageBox.Show(runtime.Definitions.Name + " をアンインストールします。" + Environment.NewLine + "本当によろしいですか？", "アンインストールの警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return;
			}
			string path = this.runtimefiles[this.RuntimeViewer.SelectedIndices[0]];
			File.Delete(path);
			string path2 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
			Directory.Delete(path2, true);
			MessageBox.Show("アンインストールしました。", "アンインストール完了", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			base.DialogResult = DialogResult.Retry;
			base.Close();
		}

		private WebClient dlClient;

		private string tempfile;

		private Runtime ur;

		public List<Runtime> runtimedatas = new List<Runtime>();

		private List<string> runtimefiles = new List<string>();
	}
}

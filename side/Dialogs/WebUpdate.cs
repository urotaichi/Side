using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace MasaoPlus.Dialogs
{
    public partial class WebUpdate : Form
    {
        public WebUpdate()
        {
            InitializeComponent();
        }

        private void WebUpdate_Shown(object sender, EventArgs e)
        {
            SUpdate("更新を確認しています...");
            if (Global.config.localSystem.UpdateServer != Global.definition.BaseUpdateServer && MessageBox.Show($"アップデート接続先サーバーが変更されています。{Environment.NewLine}既定のサーバーを利用しますか？", "更新先の変更の検知", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.config.localSystem.UpdateServer = Global.definition.BaseUpdateServer;
            }
            dlClient = new WebClient();
            dlClient.Headers.Add("User-Agent", $"{Global.definition.AppName} - {Global.definition.AppNameFull}/{Global.definition.Version}(Windows NT 10.0; Win64; x64)");
            dlClient.DownloadProgressChanged += dlClient_DownloadProgressChanged;
            dlClient.DownloadFileCompleted += dlClient_DownloadFileCompleted;
            tempfile = Path.GetTempFileName();
            Uri address = new Uri(Global.config.localSystem.UpdateServer);
            try
            {
                dlClient.DownloadFileAsync(address, tempfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void dlClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            dlClient.Dispose();
            SUpdate("更新をチェックしています...");
            if (e.Error != null)
            {
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{e.Error.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            ud = UpdateData.ParseXML(tempfile);
            File.Delete(tempfile);
            if (ud.DefVersion <= Global.definition.CheckVersion)
            {
                MessageBox.Show("最新版を利用しています。更新の必要はありません。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            if (ud.RequireLower > Global.definition.CheckVersion)
            {
                MessageBox.Show($"このバージョンからの更新は、自動アップデートでは対応していません。{Environment.NewLine}公式サイトから手動で更新してください。", "自動更新非対応", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            if (MessageBox.Show($"バージョン{ud.Name}がリリースされています。{Environment.NewLine}ダウンロードしてインストールしてもよろしいですか？", $"{Global.definition.AppName}の更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            progressBar1.Value = 0;
            SUpdate("パッケージをダウンロードしています...[1/2]");
            dlClient = new WebClient();
            dlClient.Headers.Add("User-Agent", $"{Global.definition.AppName} - {Global.definition.AppNameFull}/{Global.definition.Version}(Windows NT 10.0; Win64; x64)");
            dlClient.DownloadProgressChanged += dlClient_DownloadProgressChanged;
            dlClient.DownloadProgressChanged += dlClient_DownloadTaskbarManagerProgressChanged;
            dlClient.DownloadFileCompleted += dlClient_DownloadFileCompleted2;
            while (TemporaryFolder == null || Directory.Exists(TemporaryFolder))
            {
                TemporaryFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            Directory.CreateDirectory(TemporaryFolder);
            tempfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Update.Replace('/', '\\')));
            Uri address = new Uri(ud.Update);
            try
            {
                dlClient.DownloadFileAsync(address, tempfile);
            }
            catch (Exception ex)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void dlClient_DownloadFileCompleted2(object sender, AsyncCompletedEventArgs e)
        {
            dlClient.Dispose();
            if (e.Error != null)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{e.Error.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            progressBar1.Value = 0;
            SUpdate("パッケージをダウンロードしています...[2/2]");
            dlClient = new WebClient();
            dlClient.Headers.Add("User-Agent", $"{Global.definition.AppName} - {Global.definition.AppNameFull}/{Global.definition.Version}(Windows NT 10.0; Win64; x64)");
            dlClient.DownloadProgressChanged += dlClient_DownloadProgressChanged;
            dlClient.DownloadProgressChanged += dlClient_DownloadTaskbarManagerProgressChanged;
            dlClient.DownloadFileCompleted += dlClient_DownloadFileCompleted3;
            runfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Installer.Replace('/', '\\')));
            Uri address = new Uri(ud.Installer);
            try
            {
                dlClient.DownloadFileAsync(address, runfile);
            }
            catch (Exception ex)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void dlClient_DownloadFileCompleted3(object sender, AsyncCompletedEventArgs e)
        {
            dlClient.Dispose();
            if (e.Error != null)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{e.Error.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            SUpdate("更新の準備をしています...");
            new UpdateData
            {
                Installer = Subsystem.ExtractZipArchive(tempfile),
                Update = Application.StartupPath,
                Name = Path.GetFileName(Application.ExecutablePath)
            }.SaveXML(Path.Combine(TemporaryFolder, Global.definition.UpdateTempData));
            SUpdate("更新を開始します...");
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void dlClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Refresh();
        }

        private void dlClient_DownloadTaskbarManagerProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressValue(e.ProgressPercentage, 100);
        }

        private void SUpdate(string state)
        {
            StateLabel.Text = state;
            StateLabel.Refresh();
        }

        private WebClient dlClient;

        private string tempfile;

        public string runfile;

        private string TemporaryFolder;

        private UpdateData ud;
    }
}

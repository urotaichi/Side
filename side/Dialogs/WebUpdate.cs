using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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

        private async void WebUpdate_Shown(object sender, EventArgs e)
        {
            SUpdate("更新を確認しています...");
            if (Global.config.localSystem.UpdateServer != Global.definition.BaseUpdateServer && MessageBox.Show($"アップデート接続先サーバーが変更されています。{Environment.NewLine}既定のサーバーを利用しますか？", "更新先の変更の検知", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Global.config.localSystem.UpdateServer = Global.definition.BaseUpdateServer;
            }
            HttpClientManager.InitializeDownloadProgress(progressBar1);
            tempfile = Path.GetTempFileName();
            try
            {
                await HttpClientManager.DownloadFileAsync(
                    Global.config.localSystem.UpdateServer, 
                    tempfile);
                await dlClient_DownloadFileCompleted();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                DialogResult = DialogResult.Abort;
                Close();
            }
            finally
            {
                if (File.Exists(tempfile))
                {
                    try { File.Delete(tempfile); } catch { }
                }
            }
        }

        private async Task dlClient_DownloadFileCompleted()
        {
            SUpdate("更新をチェックしています...");
            ud = UpdateData.ParseXML(tempfile);
            File.Delete(tempfile);
            if (ud.DefVersion <= Global.definition.CheckVersion)
            {
                MessageBox.Show("最新版を利用しています。更新の必要はありません。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                DialogResult = DialogResult.Abort;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                Close();
                return;
            }
            if (ud.RequireLower > Global.definition.CheckVersion)
            {
                MessageBox.Show($"このバージョンからの更新は、自動アップデートでは対応していません。{Environment.NewLine}公式サイトから手動で更新してください。", "自動更新非対応", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Abort;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                Close();
                return;
            }
            if (MessageBox.Show($"バージョン{ud.Name}がリリースされています。{Environment.NewLine}ダウンロードしてインストールしてもよろしいですか？", $"{Global.definition.AppName}の更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                DialogResult = DialogResult.Abort;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                Close();
                return;
            }
            progressBar1.Value = 0;
            SUpdate("パッケージをダウンロードしています...[1/2]");
            while (TemporaryFolder == null || Directory.Exists(TemporaryFolder))
            {
                TemporaryFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            Directory.CreateDirectory(TemporaryFolder);
            tempfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Update.Replace('/', '\\')));
            try
            {
                await HttpClientManager.DownloadFileAsync(
                    ud.Update,
                    tempfile);
                await dlClient_DownloadFileCompleted2();
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

        private async Task dlClient_DownloadFileCompleted2()
        {
            progressBar1.Value = 0;
            SUpdate("パッケージをダウンロードしています...[2/2]");
            runfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Installer.Replace('/', '\\')));
            try
            {
                await HttpClientManager.DownloadFileAsync(
                    ud.Installer,
                    runfile);
                await dlClient_DownloadFileCompleted3();
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

        private Task dlClient_DownloadFileCompleted3()
        {
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
            return Task.CompletedTask;
        }

        private void SUpdate(string state)
        {
            StateLabel.Text = state;
            StateLabel.Refresh();
        }

        private string tempfile;

        public string runfile;

        private string TemporaryFolder;

        private UpdateData ud;
    }
}

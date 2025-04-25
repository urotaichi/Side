using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
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
            dlClient = new HttpClient();
            dlClient.DefaultRequestHeaders.Add("User-Agent", $"{Global.definition.AppName}/{Global.definition.Version} ({Global.definition.AppNameFull}; Windows NT 10.0; Win64; x64)");
            tempfile = Path.GetTempFileName();
            Uri address = new(Global.config.localSystem.UpdateServer);
            try
            {
                using var response = await dlClient.GetAsync(address);
                using var stream = await response.Content.ReadAsStreamAsync();
                using (var fs = File.Create(tempfile))
                {
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        dlClient_DownloadProgressChanged(fs.Length, response.Content.Headers.ContentLength.Value);
                    }
                }
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
            dlClient.Dispose();
            SUpdate("更新をチェックしています...");
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
            dlClient = new HttpClient();
            dlClient.DefaultRequestHeaders.Add("User-Agent", $"{Global.definition.AppName}/{Global.definition.Version} ({Global.definition.AppNameFull}; Windows NT 10.0; Win64; x64)");
            while (TemporaryFolder == null || Directory.Exists(TemporaryFolder))
            {
                TemporaryFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            Directory.CreateDirectory(TemporaryFolder);
            tempfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Update.Replace('/', '\\')));
            Uri address = new(ud.Update);
            try
            {
                using var response = await dlClient.GetAsync(address);
                using var stream = await response.Content.ReadAsStreamAsync();
                using (var fs = File.Create(tempfile))
                {
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        dlClient_DownloadProgressChanged(fs.Length, response.Content.Headers.ContentLength.Value);
                    }
                }
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
            dlClient.Dispose();
            progressBar1.Value = 0;
            SUpdate("パッケージをダウンロードしています...[2/2]");
            dlClient = new HttpClient();
            dlClient.DefaultRequestHeaders.Add("User-Agent", $"{Global.definition.AppName}/{Global.definition.Version} ({Global.definition.AppNameFull}; Windows NT 10.0; Win64; x64)");
            runfile = Path.Combine(TemporaryFolder, Path.GetFileName(ud.Installer.Replace('/', '\\')));
            Uri address = new(ud.Installer);
            try
            {
                using var response = await dlClient.GetAsync(address);
                using var stream = await response.Content.ReadAsStreamAsync();
                using (var fs = File.Create(runfile))
                {
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
                    if (response.Content.Headers.ContentLength.HasValue)
                    {
                        dlClient_DownloadProgressChanged(fs.Length, response.Content.Headers.ContentLength.Value);
                    }
                }
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
            dlClient.Dispose();
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

        private void dlClient_DownloadProgressChanged(long bytesReceived, long totalBytes)
        {
            if (totalBytes > 0)
            {
                int progress = (int)(100 * bytesReceived / totalBytes);
                progressBar1.Value = progress;
                progressBar1.Refresh();
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                TaskbarManager.Instance.SetProgressValue(progress, 100);
            }
        }

        private void SUpdate(string state)
        {
            StateLabel.Text = state;
            StateLabel.Refresh();
        }

        private HttpClient dlClient;

        private string tempfile;

        public string runfile;

        private string TemporaryFolder;

        private UpdateData ud;
    }
}

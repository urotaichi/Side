using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
    public partial class RuntimeManager : Form
    {
        public RuntimeManager()
        {
            InitializeComponent();
        }

        private void InitializeHttpClient()
        {
            var progressHandler = HttpClientManager.ProgressHandler;
            progressHandler.HttpReceiveProgress += (_, args) =>
            {
                if (args.TotalBytes.HasValue)
                {
                    dlClient_DownloadProgressChanged(args.BytesTransferred, args.TotalBytes.Value);
                }
            };
        }

        private void InstalledRuntime_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Text = "ランタイムをロードしています...";
            Enabled = false;
            if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
            {
                MessageBox.Show($"ランタイムフォルダが見つかりません。{Environment.NewLine}Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
                                    runtimedatas.Add(runtime);
                                    runtimefiles.Add(text);
                                    RuntimeViewer.Items.Add(new ListViewItem(
                                    [
                                        runtime.Definitions.Name,
                                        runtime.Definitions.Author,
                                        runtime.Definitions.Package,
                                        (runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×",
                                        runtime.Definitions.DefVersion.ToString("F2")
                                    ]));
                                }
                                else
                                {
                                    MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{Path.GetFileName(text)} : {string.Join(",", array2)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"ランタイムフォルダが見つかりません:{Path.GetFileName(text)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"読み込めませんでした:{Path.GetFileName(text)}{Environment.NewLine}{ex.Message}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            finally
            {
                Text = "ランタイム マネージャ";
                Enabled = true;
            }
            if (RuntimeViewer.Items.Count == 0)
            {
                MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void NewRuntimeInstall_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = $"{Global.definition.AppName}ランタイムパッケージ(*{Global.definition.RuntimeArchiveExt})|*{Global.definition.RuntimeArchiveExt}";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Enabled = false;
                    Text = "ランタイムをインストールしています...";
                    if (Subsystem.InstallRuntime(openFileDialog.FileName))
                    {
                        MessageBox.Show("ランタイムインストールが完了しました。", "インストール成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                finally
                {
                    DialogResult = DialogResult.Retry;
                    Close();
                }
            }
        }

        private void RuntimeViewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RuntimeViewer.SelectedIndices.Count == 0)
            {
                NetworkUpdate.Enabled = false;
                Uninstall.Enabled = false;
                return;
            }
            if (runtimedatas[RuntimeViewer.SelectedIndices[0]].Definitions.Update != null && runtimedatas[RuntimeViewer.SelectedIndices[0]].Definitions.Update != "")
            {
                NetworkUpdate.Enabled = Global.definition.IsAutoUpdateEnabled;
            }
            else
            {
                NetworkUpdate.Enabled = false;
            }
            Uninstall.Enabled = true;
        }

        private async void NetworkUpdate_Click(object sender, EventArgs e)
        {
            if (RuntimeViewer.SelectedIndices.Count == 0)
            {
                MessageBox.Show("対象が選択されていません。", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            Runtime runtime = runtimedatas[RuntimeViewer.SelectedIndices[0]];
            if (runtime.Definitions.Update == null || runtime.Definitions.Update == "")
            {
                MessageBox.Show("更新先が記述されていません。", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            ur = runtimedatas[RuntimeViewer.SelectedIndices[0]];
            if (MessageBox.Show($"{ur.Definitions.Name}の更新を確認しますか？", "アップデート確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            string update = runtimedatas[RuntimeViewer.SelectedIndices[0]].Definitions.Update;
            Text = "更新を受信しています...";
            DownProgress.Visible = true;
            Enabled = false;
            tempfile = Path.GetTempFileName();
            InitializeHttpClient();
            Uri address = new(update);
            try
            {
                using var response = await HttpClientManager.ProgressClient.GetAsync(address);
                using var stream = await response.Content.ReadAsStreamAsync();
                using (var fs = File.Create(tempfile))
                {
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
                }
                await dlClient_DownloadFileCompleted();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Retry;
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
            UpdateData updateData = UpdateData.ParseXML(tempfile);
            if (ur.Definitions.DefVersion >= updateData.DefVersion)
            {
                MessageBox.Show("最新のランタイムです。更新の必要はありません。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else if (Global.definition.CheckVersion < updateData.RequireLower)
            {
                MessageBox.Show($"最新のランタイムはこのバージョンの{Global.definition.AppName}には対応していません。{Environment.NewLine}最新版へ更新してください。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else if (MessageBox.Show($"新しいランタイムがリリースされています。{Environment.NewLine}{ur.Definitions.DefVersion} -> {updateData.DefVersion}{Environment.NewLine}更新しますか？", "更新の確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                tempfile = Path.GetTempFileName();
                Uri address = new(updateData.Update);
                try
                {
                    Text = "ランタイムをダウンロードしています...";
                    using var response = await HttpClientManager.ProgressClient.GetAsync(address);
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using (var fs = File.Create(tempfile))
                    {
                        await stream.CopyToAsync(fs);
                        await fs.FlushAsync();
                    }
                    dlClient_DownloadFileCompleted_Package();
                    return;
                }
                catch (Exception ex)
                {
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    MessageBox.Show($"更新できませんでした。{Environment.NewLine}{ex.Message}", "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                    DialogResult = DialogResult.Retry;
                    Close();
                }
            }
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void dlClient_DownloadFileCompleted_Package()
        {
            Text = "ランタイムをインストールしています...";
            Subsystem.InstallRuntime(tempfile);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void dlClient_DownloadProgressChanged(long bytesReceived, long totalBytes)
        {
            if (totalBytes > 0)
            {
                int progress = (int)(100 * bytesReceived / totalBytes);
                
                if (DownProgress.InvokeRequired)
                {
                    DownProgress.Invoke(() =>
                    {
                        DownProgress.Value = progress;
                        DownProgress.Refresh();
                    });
                }
                else
                {
                    DownProgress.Value = progress;
                    DownProgress.Refresh();
                }

                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                TaskbarManager.Instance.SetProgressValue(progress, 100);
            }
        }

        private void Uninstall_Click(object sender, EventArgs e)
        {
            if (RuntimeViewer.SelectedIndices.Count == 0)
            {
                MessageBox.Show("対象が選択されていません。", "アンインストールエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            Runtime runtime = runtimedatas[RuntimeViewer.SelectedIndices[0]];
            if (MessageBox.Show($"{runtime.Definitions.Name} をアンインストールします。{Environment.NewLine}本当によろしいですか？", "アンインストールの警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                return;
            }
            string path = runtimefiles[RuntimeViewer.SelectedIndices[0]];
            File.Delete(path);
            string path2 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            Directory.Delete(path2, true);
            MessageBox.Show("アンインストールしました。", "アンインストール完了", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            DialogResult = DialogResult.Retry;
            Close();
        }

        private string tempfile;

        private Runtime ur;

        public List<Runtime> runtimedatas = [];

        private readonly List<string> runtimefiles = [];
    }
}

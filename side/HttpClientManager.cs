using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Handlers;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace MasaoPlus
{
    public class HttpClientManager
    {
        private static HttpClient CreateHttpClient(HttpMessageHandler? handler = null)
        {
            var client = handler != null ? new HttpClient(handler) : new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            return client;
        }

        private static readonly string _userAgent = 
            $"{Global.definition.AppName}/{Global.definition.Version} ({Global.definition.AppNameFull}; Windows NT 10.0; Win64; x64)";

        private static readonly Lazy<HttpClient> _defaultClient = new(() => CreateHttpClient());

        private static readonly Lazy<ProgressMessageHandler> _progressHandler = new(() =>
        {
            var handler = new HttpClientHandler();
            return new ProgressMessageHandler(handler);
        });

        private static readonly Lazy<HttpClient> _progressClient = new(() => 
            CreateHttpClient(_progressHandler.Value));

        private static HttpClient DefaultClient => _defaultClient.Value;
        private static ProgressMessageHandler ProgressHandler => _progressHandler.Value;
        private static HttpClient ProgressClient => _progressClient.Value;

        public static void InitializeDownloadProgress(ProgressBar progressBar)
        {
            var progressHandler = ProgressHandler;
            var taskbarManager = TaskbarManager.Instance;

            progressHandler.HttpReceiveProgress += (_, args) =>
            {
                if (!args.TotalBytes.HasValue || args.TotalBytes.Value <= 0) return;
                UpdateDownloadProgress(progressBar, taskbarManager, args);
            };
        }

        private static void UpdateDownloadProgress(ProgressBar progressBar, TaskbarManager taskbarManager, HttpProgressEventArgs args)
        {
            int progress = (int)(100 * args.BytesTransferred / args.TotalBytes!.Value);
            UpdateProgressUI(progressBar, progress);
            UpdateTaskbarProgress(taskbarManager, progress);
        }

        private static void UpdateProgressUI(ProgressBar progressBar, int progress)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(() => SetProgressBarValue(progressBar, progress));
            }
            else
            {
                SetProgressBarValue(progressBar, progress);
            }
        }

        private static void SetProgressBarValue(ProgressBar progressBar, int progress)
        {
            progressBar.Value = progress;
            progressBar.Refresh();
        }

        private static void UpdateTaskbarProgress(TaskbarManager taskbarManager, int progress)
        {
            taskbarManager.SetProgressState(TaskbarProgressBarState.Normal);
            taskbarManager.SetProgressValue(progress, 100);
        }

        private static async Task DownloadFileInternalAsync(HttpClient client, string url, string destinationPath)
        {
            Uri address = new(url);

            using var response = await client.GetAsync(address);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fs = File.Create(destinationPath);
            await stream.CopyToAsync(fs);
            await fs.FlushAsync();
        }

        public static Task DownloadFileAsync(string url, string destinationPath)
        {
            return DownloadFileInternalAsync(ProgressClient, url, destinationPath);
        }

        public static Task DownloadFileSimpleAsync(string url, string destinationPath)
        {
            return DownloadFileInternalAsync(DefaultClient, url, destinationPath);
        }
    }
}

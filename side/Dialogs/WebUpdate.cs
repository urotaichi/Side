using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class WebUpdate : Form
	{
		public WebUpdate()
		{
			this.InitializeComponent();
		}

		private void WebUpdate_Shown(object sender, EventArgs e)
		{
			this.SUpdate("更新を確認しています...");
			if (Global.config.localSystem.UpdateServer != Global.definition.BaseUpdateServer && MessageBox.Show("アップデート接続先サーバーが変更されています。" + Environment.NewLine + "既定のサーバーを利用しますか？", "更新先の変更の検知", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Global.config.localSystem.UpdateServer = Global.definition.BaseUpdateServer;
			}
			this.dlClient = new WebClient();
			this.dlClient.Headers.Add("User-Agent", string.Concat(new string[]
			{
				Global.definition.AppName,
				" - ",
				Global.definition.AppNameFull,
				"/",
				Global.definition.Version,
				"(compatible; MSIE 6.0/7.0; Windows XP/Vista)"
			}));
			this.dlClient.DownloadProgressChanged += this.dlClient_DownloadProgressChanged;
			this.dlClient.DownloadFileCompleted += this.dlClient_DownloadFileCompleted;
			this.tempfile = Path.GetTempFileName();
			Uri address = new Uri(Global.config.localSystem.UpdateServer);
			try
			{
				this.dlClient.DownloadFileAsync(address, this.tempfile);
			}
			catch (Exception ex)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + ex.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
			}
		}

		private void dlClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			this.dlClient.Dispose();
			this.SUpdate("更新をチェックしています...");
			if (e.Error != null)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + e.Error.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			this.ud = UpdateData.ParseXML(this.tempfile);
			File.Delete(this.tempfile);
			if (this.ud.DefVersion <= Global.definition.CheckVersion)
			{
				MessageBox.Show("最新版を利用しています。更新の必要はありません。", "更新結果", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			if (this.ud.RequireLower > Global.definition.CheckVersion)
			{
				MessageBox.Show("このバージョンからの更新は、自動アップデートでは対応していません。" + Environment.NewLine + "公式サイトから手動で更新してください。", "自動更新非対応", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			if (MessageBox.Show(string.Concat(new string[]
			{
				"バージョン",
				this.ud.Name.ToString(),
				"がリリースされています。",
				Environment.NewLine,
				"ダウンロードしてインストールしてもよろしいですか？"
			}), Global.definition.AppName + "の更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			this.progressBar1.Value = 0;
			this.SUpdate("パッケージをダウンロードしています...[1/2]");
			this.dlClient = new WebClient();
			this.dlClient.Headers.Add("User-Agent", string.Concat(new string[]
			{
				Global.definition.AppName,
				" - ",
				Global.definition.AppNameFull,
				"/",
				Global.definition.Version,
				"(compatible; MSIE 6.0/7.0; Windows XP/Vista)"
			}));
			this.dlClient.DownloadProgressChanged += this.dlClient_DownloadProgressChanged;
			this.dlClient.DownloadFileCompleted += this.dlClient_DownloadFileCompleted2;
			while (this.TemporaryFolder == null || Directory.Exists(this.TemporaryFolder))
			{
				this.TemporaryFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			}
			Directory.CreateDirectory(this.TemporaryFolder);
			this.tempfile = Path.Combine(this.TemporaryFolder, Path.GetFileName(this.ud.Update.Replace('/', '\\')));
			Uri address = new Uri(this.ud.Update);
			try
			{
				this.dlClient.DownloadFileAsync(address, this.tempfile);
			}
			catch (Exception ex)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + ex.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
			}
		}

		private void dlClient_DownloadFileCompleted2(object sender, AsyncCompletedEventArgs e)
		{
			this.dlClient.Dispose();
			if (e.Error != null)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + e.Error.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			this.progressBar1.Value = 0;
			this.SUpdate("パッケージをダウンロードしています...[2/2]");
			this.dlClient = new WebClient();
			this.dlClient.Headers.Add("User-Agent", string.Concat(new string[]
			{
				Global.definition.AppName,
				" - ",
				Global.definition.AppNameFull,
				"/",
				Global.definition.Version,
				"(compatible; MSIE 6.0/7.0; Windows XP/Vista)"
			}));
			this.dlClient.DownloadProgressChanged += this.dlClient_DownloadProgressChanged;
			this.dlClient.DownloadFileCompleted += this.dlClient_DownloadFileCompleted3;
			this.runfile = Path.Combine(this.TemporaryFolder, Path.GetFileName(this.ud.Installer.Replace('/', '\\')));
			Uri address = new Uri(this.ud.Installer);
			try
			{
				this.dlClient.DownloadFileAsync(address, this.runfile);
			}
			catch (Exception ex)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + ex.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
			}
		}

		private void dlClient_DownloadFileCompleted3(object sender, AsyncCompletedEventArgs e)
		{
			this.dlClient.Dispose();
			if (e.Error != null)
			{
				MessageBox.Show("更新できませんでした。" + Environment.NewLine + e.Error.Message, "アップデートエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = DialogResult.Abort;
				base.Close();
				return;
			}
			this.SUpdate("更新の準備をしています...");
			new UpdateData
			{
				Installer = Subsystem.ExtractZipArchive(this.tempfile),
				Update = Application.StartupPath,
				Name = Path.GetFileName(Application.ExecutablePath)
			}.SaveXML(Path.Combine(this.TemporaryFolder, Global.definition.UpdateTempData));
			this.SUpdate("更新を開始します...");
			base.DialogResult = DialogResult.Retry;
			base.Close();
		}

		private void dlClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.progressBar1.Value = e.ProgressPercentage;
			this.progressBar1.Refresh();
		}

		private void SUpdate(string state)
		{
			this.StateLabel.Text = state;
			this.StateLabel.Refresh();
		}

		private WebClient dlClient;

		private string tempfile;

		public string runfile;

		private string TemporaryFolder;

		private UpdateData ud;
	}
}

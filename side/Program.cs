using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace MasaoPlus
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				try
				{
					if (File.Exists(Path.Combine(Application.StartupPath, Global.definition.ConfigFile)))
					{
						Global.config = Config.ParseXML(Path.Combine(Application.StartupPath, Global.definition.ConfigFile));
					}
				}
				catch
				{
					MessageBox.Show("設定をロードできませんでした。", "設定読み込み失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				bool flag = true;
				if (Environment.GetCommandLineArgs().Length == 2)
				{
					flag = false;
					string a;
					if ((a = Environment.GetCommandLineArgs()[1]) != null)
					{
						if (a == "/reg")
						{
							Native.SetSideFileRelate();
							goto IL_104;
						}
						if (a == "/unreg")
						{
							Native.DeleteSideFileRelate();
							goto IL_104;
						}
					}
					string text = Environment.GetCommandLineArgs()[1];
					if (File.Exists(text) && Path.GetExtension(text) == Global.definition.RuntimeArchiveExt)
					{
						if (Subsystem.InstallRuntime(text))
						{
							MessageBox.Show("指定されたランタイム " + Path.GetFileName(text) + " をインストールしました。", "新しいランタイムのインストール", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						}
						Global.state.ParseCommandline = true;
					}
					flag = true;
				}
				IL_104:
				if (flag)
				{
					//WebView2 ランタイムがインストールされているかを調べて、されていない場合インストールする
					Microsoft.Win32.RegistryKey key;
					if (Environment.Is64BitOperatingSystem)
						key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");
					else
						key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");

					if (key == null || key.GetValue("pv") == null)
					{
						DialogResult result = MessageBox.Show("Sideを起動するにはWebView2 ランタイムのインストールが必要です。" + Environment.NewLine +
							"WebView2 ランタイムをインストールします。" + Environment.NewLine +
							"(インストール後、PCの再起動が必要な場合があります)",
							"WebView2 ランタイムのインストール",
							MessageBoxButtons.YesNo);
						if (result == DialogResult.Yes)
						{
							Process.Start(new ProcessStartInfo()
							{
								FileName = @"runtimes\MicrosoftEdgeWebview2Setup.exe",
								UseShellExecute = true,
							});
						}
						else
						{
							MessageBox.Show("Sideを終了します。", "Sideの終了");
						}
					}
					else
					{
						Application.Run(new MainWindow());
					}
				}
			}

			catch (Exception ex)
			{
				try
				{
                    using StreamWriter streamWriter = new StreamWriter(Global.definition.Dump, true);
                    streamWriter.WriteLine("------------------------------------------------------------");
                    streamWriter.WriteLine("| Reported:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    streamWriter.WriteLine("------------------------------------------------------------");
                    streamWriter.WriteLine(ex.Message);
                    streamWriter.WriteLine(ex.Source);
                    streamWriter.WriteLine("*StackTrace");
                    streamWriter.WriteLine(ex.StackTrace);
                    streamWriter.Close();
                }
				catch
				{
					MessageBox.Show("Error Dump Output Failed.", "Error Handling Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					Application.Exit();
				}
				MessageBox.Show(string.Concat(new string[]
				{
					Global.definition.AppName,
					"に致命的な障害が発生しました。",
					Environment.NewLine,
					Environment.NewLine,
					ex.Message,
					Environment.NewLine,
					"強制終了します。ご迷惑をお掛けし、申し訳ございません。",
					Environment.NewLine,
					Environment.NewLine,
					Global.definition.Dump,
					"にエラーダンプを出力しました。"
				}), Global.definition.AppNameFull + " Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				try
				{
					Process.Start("notepad", Global.definition.Dump);
				}
				catch
				{
				}
			}
			finally
			{
				Application.Exit();
				Environment.Exit(-1);
			}
		}
	}
}

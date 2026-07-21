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
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    string configFilePath = Global.definition.GetUserDataPath(Global.definition.ConfigFile);
                    if (File.Exists(configFilePath))
                    {
                        Global.config = Config.ParseXML(configFilePath);
                    }
                }
                catch
                {
                    MessageBox.Show("設定をロードできませんでした。", "設定読み込み失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                if (Global.config.localSystem.DarkMode == SystemColorMode.System)
                {
                    if (Application.SystemColorMode == SystemColorMode.Dark)
                        Global.state.DarkMode = SystemColorMode.Dark;
                    else
                        Global.state.DarkMode = SystemColorMode.Classic;
                }
                else
                {
                    Global.state.DarkMode = Global.config.localSystem.DarkMode;
                }
                Application.SetColorMode(Global.state.DarkMode);
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
                            MessageBox.Show($"指定されたランタイム {Path.GetFileName(text)} をインストールしました。", "新しいランタイムのインストール", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                        DialogResult result = MessageBox.Show($"Sideを起動するにはWebView2 ランタイムのインストールが必要です。{Environment.NewLine}WebView2 ランタイムをインストールします。{Environment.NewLine}(インストール後、PCの再起動が必要な場合があります)",
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
                        ApplicationConfiguration.Initialize();
                        
#if MICROSOFT_STORE
                        // Microsoft Store版用のフォルダ初期化
                        InitializeMicrosoftStoreDirectories();
#endif
                        
                        Application.Run(new MainWindow());
                    }
                }
            }

            catch (Exception ex)
            {
                try
                {
                    using StreamWriter streamWriter = new(Global.definition.Dump, true);
                    streamWriter.WriteLine("------------------------------------------------------------");
                    streamWriter.WriteLine($"| Reported:{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
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
                MessageBox.Show($"{Global.definition.AppName}に致命的な障害が発生しました。{Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}強制終了します。ご迷惑をお掛けし、申し訳ございません。{Environment.NewLine}{Environment.NewLine}{Global.definition.Dump}にエラーダンプを出力しました。", $"{Global.definition.AppNameFull} Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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

#if MICROSOFT_STORE
        /// <summary>
        /// Microsoft Store版用のディレクトリを初期化します
        /// </summary>
        private static void InitializeMicrosoftStoreDirectories()
        {
            try
            {
                // Documents/SideData フォルダを作成
                string sideDataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SideData");
                if (!Directory.Exists(sideDataRoot))
                {
                    Directory.CreateDirectory(sideDataRoot);
                }

                // 画像データは既存の pictures 配下をそのまま使用する
                string appPicturesDir = Global.definition.GetUserDataPath(Path.Combine("pictures", "default"));
                if (!Directory.Exists(appPicturesDir))
                {
                    Directory.CreateDirectory(appPicturesDir);
                }
            }
            catch (Exception ex)
            {
                // フォルダ作成に失敗してもアプリケーションは継続
                MessageBox.Show($"プロジェクトフォルダの初期化に失敗しました: {ex.Message}", 
                    "初期化警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
#endif
    }
}

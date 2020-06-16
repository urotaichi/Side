using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MasaoPlus
{
	// Token: 0x0200003A RID: 58
	internal static class Program
	{
		// Token: 0x06000251 RID: 593 RVA: 0x00029E68 File Offset: 0x00028068
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
					Application.Run(new MainWindow());
				}
			}
			catch (Exception ex)
			{
				try
				{
					using (StreamWriter streamWriter = new StreamWriter(Global.definition.Dump, true))
					{
						streamWriter.WriteLine("------------------------------------------------------------");
						streamWriter.WriteLine("| Reported:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
						streamWriter.WriteLine("------------------------------------------------------------");
						streamWriter.WriteLine(ex.Message);
						streamWriter.WriteLine(ex.Source);
						streamWriter.WriteLine("*StackTrace");
						streamWriter.WriteLine(ex.StackTrace);
						streamWriter.Close();
					}
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

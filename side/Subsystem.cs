using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using MasaoPlus.Dialogs;
using System.Text.RegularExpressions;

namespace MasaoPlus
{
	// Token: 0x02000036 RID: 54
	public static class Subsystem
	{
		// Token: 0x060001FA RID: 506 RVA: 0x000263A0 File Offset: 0x000245A0
		public static void MakeTestrun(int startup)
		{
			using (StreamWriter streamWriter = new StreamWriter(Subsystem.GetTempFileWhere(), false, Global.config.localSystem.FileEncoding))
			{
				string value = Subsystem.MakeHTMLCode(startup);
				streamWriter.Write(value);
				streamWriter.Close();
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000263F8 File Offset: 0x000245F8
		public static void MakeTestrun(int startup, int replace, string[] sts)
		{
			using (StreamWriter streamWriter = new StreamWriter(Subsystem.GetTempFileWhere(), false, Global.config.localSystem.FileEncoding))
			{
				string value = Subsystem.MakeHTMLCode(startup, replace, sts);
				streamWriter.Write(value);
				streamWriter.Close();
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000358D File Offset: 0x0000178D
		public static string GetTempFileWhere()
		{
			return Path.Combine(Global.cpd.where, Global.config.testRun.TempFile + "." + Global.cpd.runtime.DefaultConfigurations.FileExt);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000035CB File Offset: 0x000017CB
		public static string MakeHTMLCode(int StartStage)
		{
			return Subsystem.MakeHTMLCode(StartStage, -1, null);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00026454 File Offset: 0x00024654
		public static string MakeHTMLCode(int StartStage, int ReplaceStage, string[] sts)
		{
			Global.cpd.project.Config.StageStart = StartStage + 1;
			StringBuilder stringBuilder = new StringBuilder();

			// ヘッダーを出力
			string text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.HeaderHTML);
			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					text = text.Replace("<?" + htmlreplaceData.Name + ">", htmlreplaceData.Value);
				}
			}
			foreach (string value in text.Split(new string[]
			{
				Environment.NewLine,
				"\r",
				"\n"
			}, StringSplitOptions.None))
			{
				stringBuilder.AppendLine(value);
			}

			//エデイタ識別コードを出力
			if (Global.config.localSystem.IntegrateEditorId)
			{
				stringBuilder.AppendLine(Global.definition.EditorIdStr);
			}

			//パラメータを出力
			stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 0) ? sts : Global.cpd.project.StageData));
			if (Global.cpd.project.Config.StageNum >= 2)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam2, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 1) ? sts : Global.cpd.project.StageData2));
			}
			if (Global.cpd.project.Config.StageNum >= 3)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam3, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 2) ? sts : Global.cpd.project.StageData3));
			}
			if (Global.cpd.project.Config.StageNum >= 4)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam4, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 3) ? sts : Global.cpd.project.StageData4));
			}

			if (Global.cpd.runtime.Definitions.LayerSize.bytesize != 0)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData));
				if (Global.cpd.project.Config.StageNum >= 2)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam2, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData2));
				}
				if (Global.cpd.project.Config.StageNum >= 3)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam3, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData3));
				}
				if (Global.cpd.project.Config.StageNum >= 4)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam4, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData4));
				}
			}

			if (Global.cpd.project.Config.UseWorldmap)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.MapParam, 0, Global.cpd.project.MapData));
			}

			string parameter = Global.cpd.runtime.DefaultConfigurations.Parameter;
			ConfigParam[] configurations = Global.cpd.project.Config.Configurations;
			int k = 0;
			while (k < configurations.Length)
			{
				ConfigParam configParam = configurations[k];
				if (configParam.RequireStages > 1 && configParam.RequireStages < 5)
				{
					if (configParam.RequireStages <= Global.cpd.project.Config.StageNum)
					{
						goto IL_4DB;
					}
				}
				else if (configParam.RequireStages != 5 || Global.cpd.project.Config.UseWorldmap)
				{
					goto IL_4DB;
				}
				IL_718:
				k++;
				continue;
				IL_4DB:
				string typestr;
				switch (typestr = configParam.Typestr)
				{
				case "bool":
				case "bool21":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, (configParam.Value == "true") ? "1" : "2"));
					goto IL_718;
				case "bool10":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, (configParam.Value == "true") ? "1" : "0"));
					goto IL_718;
				case "int":
				case "list":
				case "list_athletic":
				case "string":
				case "file":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, configParam.Value));
					goto IL_718;
				case "text":
				{
					string[] array2 = configParam.Value.Split(new string[]
					{
						Environment.NewLine
					}, StringSplitOptions.None);
					int num2 = 1;

					Regex text_name_regx = new Regex(@"-(\d+)$");
					Match text_name_match = text_name_regx.Match(configParam.Name);
					if(text_name_match.Success){
						num2 = int.Parse(text_name_match.Groups[1].Value);
						configParam.Name = text_name_regx.Replace(configParam.Name, string.Empty);
					}

					foreach (string arg in array2)
					{
						stringBuilder.AppendLine(string.Format(parameter, configParam.Name + "-" + num2.ToString(), arg));
						num2++;
					}
					goto IL_718;
				}
				case "color":
				{
					Colors colors = new Colors(configParam.Value);
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "red"), colors.r.ToString()));
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "green"), colors.g.ToString()));
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "blue"), colors.b.ToString()));
					goto IL_718;
				}
				}
				throw new Exception("不明な型が含まれています:" + configParam.Typestr);
			}

			//フッターを出力
			text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.FooterHTML);
			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData2 in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					text = text.Replace("<?" + htmlreplaceData2.Name + ">", htmlreplaceData2.Value);
				}
			}
			foreach (string value2 in text.Split(new string[]
			{
				Environment.NewLine,
				"\r",
				"\n"
			}, StringSplitOptions.None))
			{
				stringBuilder.AppendLine(value2);
			}

			//末尾の,を除去（文法的にはセーフだが）
			string result = new Regex(@",(\s*?)}").Replace(stringBuilder.ToString(), "$1}");

			return result;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x000035D5 File Offset: 0x000017D5
		public static string DecodeBase64(string s)
		{
			return s;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000035D5 File Offset: 0x000017D5
		public static string EncodeBase64(string str)
		{
			return str;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00026C74 File Offset: 0x00024E74
		public static string MakeStageParameter(string Parameter, int StageSplit, string[] StageText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder[] array = new StringBuilder[StageSplit + 1];
			int num = 0;
			foreach (string text in StageText)
			{
				if (StageSplit != 0 && text.Length % (StageSplit + 1) != 0)
				{
					throw new Exception("分割数の設定が異常です。");
				}
				int num2 = 0;
				for (int j = 0; j <= StageSplit; j++)
				{
					if (array[j] == null)
					{
						array[j] = new StringBuilder();
					}
					if (StageSplit == 0)
					{
						array[j].AppendLine(string.Format(Parameter, new object[]
						{
							num,
							text.Substring(num2, text.Length / (StageSplit + 1))
						}));
					}
					else
					{
						array[j].AppendLine(string.Format(Parameter, new object[]
						{
							j,
							num,
							text.Substring(num2, text.Length / (StageSplit + 1))
						}));
					}
					num2 += text.Length / (StageSplit + 1);
				}
				num++;
			}
			foreach (StringBuilder stringBuilder2 in array)
			{
				stringBuilder.AppendLine(stringBuilder2.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00026DC8 File Offset: 0x00024FC8
		public static string LoadUnknownTextFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("ファイルをロードできませんでした。");
			}
			byte[] array = new byte[0];
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
			}
			return Subsystem.GetCode(array).GetString(array);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00026E38 File Offset: 0x00025038
		public static Encoding GetCode(byte[] byts)
		{
			int num = byts.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < num; i++)
			{
				if (byts[i] <= 6 || byts[i] == 127 || byts[i] == 255)
				{
					num2++;
					if (num - 1 > i && byts[i] == 0 && i > 0 && byts[i - 1] <= 127)
					{
						num3++;
					}
				}
			}
			if (num2 > 0)
			{
				if (num3 > 0)
				{
					return Encoding.Unicode;
				}
				return null;
			}
			else
			{
				for (int j = 0; j < num - 1; j++)
				{
					byte b = byts[j];
					byte b2 = byts[j + 1];
					if (b == 27)
					{
						if (b2 >= 128)
						{
							return Encoding.ASCII;
						}
						if (num - 2 > j && b2 == 36 && byts[j + 2] == 64)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 36 && byts[j + 2] == 66)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 5 > j && b2 == 38 && byts[j + 2] == 64 && byts[j + 3] == 27 && byts[j + 4] == 36 && byts[j + 5] == 66)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 3 > j && b2 == 36 && byts[j + 2] == 40 && byts[j + 3] == 68)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 40 && (byts[j + 2] == 66 || byts[j + 2] == 74))
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 40 && byts[j + 2] == 73)
						{
							return Encoding.GetEncoding(50220);
						}
					}
				}
				for (int k = 0; k < num - 1; k++)
				{
					byte b = byts[k];
					byte b2 = byts[k + 1];
					if (((b >= 129 && b <= 159) || (b >= 224 && b <= 252)) && ((b2 >= 64 && b2 <= 126) || (b2 >= 128 && b2 <= 252)))
					{
						num4 += 2;
						k++;
					}
				}
				for (int l = 0; l < num - 1; l++)
				{
					byte b = byts[l];
					byte b2 = byts[l + 1];
					if ((b >= 161 && b <= 254 && b2 >= 161 && b2 <= 254) || (b == 142 && b2 >= 161 && b2 <= 223))
					{
						num5 += 2;
						l++;
					}
					else if (num - 2 > l && b == 143 && b2 >= 161 && b2 <= 254 && byts[l + 2] >= 161 && byts[l + 2] <= 254)
					{
						num5 += 3;
						l += 2;
					}
				}
				for (int m = 0; m < num - 1; m++)
				{
					byte b = byts[m];
					byte b2 = byts[m + 1];
					if (b >= 192 && b <= 223 && b2 >= 128 && b2 <= 191)
					{
						num6 += 2;
						m++;
					}
					else if (num - 2 > m && b >= 224 && b <= 239 && b2 >= 128 && b2 <= 191 && byts[m + 2] >= 128 && byts[m + 2] <= 191)
					{
						num6 += 3;
						m += 2;
					}
				}
				if (num5 > num4 && num5 > num6)
				{
					return Encoding.GetEncoding(51932);
				}
				if (num4 > num5 && num4 > num6)
				{
					return Encoding.GetEncoding(932);
				}
				if (num6 > num5 && num6 > num4)
				{
					return Encoding.UTF8;
				}
				return Encoding.Default;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00027200 File Offset: 0x00025400
		public static string ExtractZipArchive(string InputArchive)
		{
			string text = "";
			while (text == "" || Directory.Exists(text))
			{
				text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			}
			try
			{
				Directory.CreateDirectory(text);
			}
			catch
			{
				return null;
			}
			using (FileStream fileStream = new FileStream(InputArchive, FileMode.Open))
			{
				using (ZipInputStream zipInputStream = new ZipInputStream(fileStream))
				{
					ZipEntry nextEntry;
					while ((nextEntry = zipInputStream.GetNextEntry()) != null)
					{
						if (!nextEntry.IsDirectory)
						{
							string fileName = Path.GetFileName(nextEntry.Name);
							string text2 = Path.Combine(text, Path.GetDirectoryName(nextEntry.Name));
							Directory.CreateDirectory(text2);
							string path = Path.Combine(text2, fileName);
							using (FileStream fileStream2 = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
							{
								byte[] array = new byte[Global.definition.ZipExtractBufferLength];
								int count;
								while ((count = zipInputStream.Read(array, 0, array.Length)) > 0)
								{
									fileStream2.Write(array, 0, count);
								}
							}
						}
					}
				}
			}
			return text;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00027344 File Offset: 0x00025544
		public static bool InstallRuntime(string Source)
		{
			string text = Subsystem.ExtractZipArchive(Source);
			string[] files = Directory.GetFiles(text, "*.xml", SearchOption.TopDirectoryOnly);
			if (files.Length != 1)
			{
				MessageBox.Show("ランタイムを特定できません。" + Environment.NewLine + "インストールに失敗しました。", "ランタイム インストール エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string fileName = Path.GetFileName(files[0]);
			if (!File.Exists(Path.Combine(text, fileName)))
			{
				MessageBox.Show("定義ファイルの展開に失敗しました。" + Environment.NewLine + "インストールに失敗しました。", "ランタイム インストール エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Runtime runtime = Runtime.ParseXML(Path.Combine(text, fileName));
			if (Global.definition.CheckVersion < runtime.Definitions.RequireLower)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"定義ファイルはこのバージョンの",
					Global.definition.AppName,
					"には対応していません。",
					Environment.NewLine,
					Global.definition.AppName,
					"を最新のバージョンへ更新してください。"
				}), "インストール拒否", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string text2 = Path.Combine(Application.StartupPath, Global.definition.RuntimeDir);
			string text3 = Path.Combine(text2, fileName);
			if (File.Exists(text3))
			{
				if (MessageBox.Show(fileName + "はすでに存在します。" + Environment.NewLine + "上書きインストールして更新しますか?", "ランタイムパッケージの更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					using (SaveFileDialog saveFileDialog = new SaveFileDialog())
					{
						saveFileDialog.InitialDirectory = text2;
						saveFileDialog.FileName = fileName;
						saveFileDialog.DefaultExt = ".xml";
						if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
						{
							return false;
						}
						text3 = saveFileDialog.FileName;
						goto IL_20A;
					}
				}
				Runtime runtime2 = Runtime.ParseXML(text3);
				if (runtime2.Definitions.DefVersion >= runtime.Definitions.DefVersion && MessageBox.Show(string.Concat(new string[]
				{
					"上書きインストールしようとしているランタイムは、",
					Environment.NewLine,
					"現在のランタイムと同等か、それより過去のバージョンです。",
					Environment.NewLine,
					"上書きしてよろしいですか？"
				}), "ダウングレード警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
				{
					return false;
				}
			}
			IL_20A:
			File.Copy(Path.Combine(text, fileName), text3, true);
			text3 = Path.Combine(Path.GetDirectoryName(text3), Path.GetFileNameWithoutExtension(text3));
			Directory.CreateDirectory(text3);
			foreach (string text4 in Directory.GetFiles(Path.Combine(text, Path.GetFileNameWithoutExtension(fileName)), "*", SearchOption.TopDirectoryOnly))
			{
				string text5 = Path.Combine(text3, Path.GetFileName(text4));
				if (File.Exists(text5))
				{
					File.Delete(text5);
				}
				File.Move(text4, text5);
			}
			Directory.Delete(text, true);
			return true;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00027600 File Offset: 0x00025800
		public static void UpdateAutoCheck()
		{
			if (!Global.definition.IsAutoUpdateEnabled)
			{
				return;
			}
			if (Global.config.localSystem.UpdateServer != Global.definition.BaseUpdateServer && MessageBox.Show("アップデート接続先サーバーが変更されています。" + Environment.NewLine + "既定のサーバーを利用しますか？", "更新先の変更の検知", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Global.config.localSystem.UpdateServer = Global.definition.BaseUpdateServer;
			}
			Subsystem.dlClient = new WebClient();
			Subsystem.dlClient.Headers.Add("User-Agent", string.Concat(new string[]
			{
				Global.definition.AppName,
				" - ",
				Global.definition.AppNameFull,
				"/",
				Global.definition.Version,
				"(compatible; MSIE 6.0/7.0; Windows XP/Vista)"
			}));
			Subsystem.dlClient.DownloadFileCompleted += Subsystem.dlClient_DownloadFileCompleted;
			Subsystem.tempfile = Path.GetTempFileName();
			Uri address = new Uri(Global.config.localSystem.UpdateServer);
			try
			{
				Subsystem.dlClient.DownloadFileAsync(address, Subsystem.tempfile);
			}
			catch
			{
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0002773C File Offset: 0x0002593C
		private static void dlClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Subsystem.dlClient.Dispose();
			if (e.Error != null)
			{
				return;
			}
			UpdateData updateData = UpdateData.ParseXML(Subsystem.tempfile);
			File.Delete(Subsystem.tempfile);
			if (updateData.DefVersion <= Global.definition.CheckVersion)
			{
				return;
			}
			if (MessageBox.Show(string.Concat(new string[]
			{
				"Sideの新しいバージョンが公開されています。",
				Environment.NewLine,
				"(バージョン ",
				updateData.Name.ToString(),
				")",
				Environment.NewLine,
				"更新しますか？"
			}), "Sideの更新", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
			{
				MessageBox.Show("自動更新チェックはオフになります。" + Environment.NewLine + "再度有効にする場合はエディタオプションより設定してください。", "更新の中止", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				Global.config.localSystem.CheckAutoUpdate = false;
				return;
			}
			using (WebUpdate webUpdate = new WebUpdate())
			{
				if (webUpdate.ShowDialog() == DialogResult.Retry)
				{
					Global.state.RunFile = (string)webUpdate.runfile.Clone();
					Global.MainWnd.Close();
				}
			}
		}

		// Token: 0x04000293 RID: 659
		private static WebClient dlClient;

		// Token: 0x04000294 RID: 660
		private static string tempfile;
	}
}

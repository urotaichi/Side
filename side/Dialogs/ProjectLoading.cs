﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class ProjectLoading : Form
	{
		public ProjectLoading(string LoadProject)
		{
			this.load = LoadProject;
			this.InitializeComponent();
		}

		private void ProjectLoading_Shown(object sender, EventArgs e)
		{
			Application.DoEvents();
            LoadProjectDlg loadProjectDlg = new LoadProjectDlg(this.LoadProject);
			loadProjectDlg.BeginInvoke(new AsyncCallback(this.EndInv), null);
		}

		public void EndInv(IAsyncResult iar)
		{
            LoadProjectDlg method = new LoadProjectDlg(this.EndInvInv);
			try
			{
				base.Invoke(method);
			}
			catch
			{
			}
		}

		public void EndInvInv()
		{
			Global.MainWnd.UpdateTitle();
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		public void LoadProject()
		{
			try
			{
				try
				{
					Global.state.ChipRegister = new Dictionary<string, string>();
					Global.cpd.project = Project.ParseXML(this.load);
					if (Global.cpd.project.ProjVer != 0.0 && Global.cpd.project.ProjVer < Global.definition.CProjVer)
					{
						if (MessageBox.Show($"古いバージョンのプロジェクトファイルが指定されました。{Environment.NewLine}プロジェクトファイルのアップグレードを試みます。{Environment.NewLine}よろしいですか？", "レガシー プロジェクト ファイルの読み込み", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
						{
							this.SetState("プロジェクトをコンバートしています...");
							double projVer = Global.cpd.project.ProjVer;
							MessageBox.Show($"このプロジェクトファイルはサポートされていません。{Environment.NewLine}通常の読み込みを試みます。", "コンバート エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
						else
						{
							MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							base.DialogResult = DialogResult.Abort;
							base.Close();
						}
					}
					if (Global.cpd.project == null)
					{
						MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						base.DialogResult = DialogResult.Abort;
						base.Close();
					}
					this.SetState("プロジェクト設定を読み込んでいます...");
					Global.cpd.where = Path.GetDirectoryName(this.load);
					Global.cpd.filename = this.load;
					Global.state.Background = Global.cpd.project.Config.Background;
					Global.cpd.runtime = Global.cpd.project.Runtime;
					string[] array = Runtime.CheckFiles(Global.cpd.where, Global.cpd.runtime, false);
					if (array.Length != 0)
					{
						MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{string.Join(",", array)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						base.DialogResult = DialogResult.Abort;
						base.Close();
					}
					this.SetState("チップデータを読み込んでいます...");
					ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(this.load), Global.cpd.runtime.Definitions.ChipDefinition));
					if (Global.cpd.UseLayer)
                    {
                        for (int i = 0; i < chipDataClass.Layerchip.Length; i++)
                        {
							if (chipDataClass.Layerchip[i].character == "..") chipDataClass.Layerchip[i].code = 0;
                            else chipDataClass.Layerchip[i].code = int.Parse(chipDataClass.Layerchip[i].character, System.Globalization.NumberStyles.HexNumber);
                        }
                        Global.cpd.Layerchip = chipDataClass.Layerchip;
                    }
                    for (int i = 0; i < chipDataClass.Mapchip.Length; i++)
                    {
                        if (chipDataClass.Mapchip[i].character == ".") chipDataClass.Mapchip[i].code = 0;
                        else if (chipDataClass.Mapchip[i].code == default) chipDataClass.Mapchip[i].code = chipDataClass.Mapchip[i].character.ToCharArray(0, 1)[0];
                    }
                    Global.cpd.Mapchip = chipDataClass.Mapchip;
					Global.cpd.Worldchip = chipDataClass.WorldChip;
                    Global.cpd.VarietyChip = chipDataClass?.VarietyChip;
                    Global.cpd.EditingMap = Global.cpd.project.StageData;
					if (Global.cpd.UseLayer)
					{
						Global.cpd.EditingLayer = Global.cpd.project.LayerData;
                    }
                    Global.MainWnd.MainDesigner.CreateDrawItemReference();
					this.SetState("画像を準備しています...");
					Global.MainWnd.MainDesigner.PrepareImages();
					this.SetState("グラフィカルデザイナを初期化しています...");
					Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
					if (Global.cpd.UseLayer)
					{
						Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
					}
					if (Global.state.TransparentUnactiveLayer)
					{
						Global.MainWnd.MainDesigner.InitTransparent();
					}
					this.SetState("チップリストを作成しています...");
					Global.MainWnd.ChipItemReady();
					this.SetState("編集を開始します...");
				}
				catch (InvalidOperationException)
				{
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}{ex.Message}{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Application.Restart();
				Environment.Exit(-1);
			}
		}

		private void SetState(string text)
		{
            SetStateDelg method = new SetStateDelg(this.SetStateInv);
			base.Invoke(method, new object[]
			{
				text
			});
		}

		private void SetStateInv(string text)
		{
			this.WaiterText.Text = text;
			this.WaiterText.Refresh();
		}

		private string load = "";

		private delegate void LoadProjectDlg();

		private delegate void SetStateDelg(string text);
	}
}

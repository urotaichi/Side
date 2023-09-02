using System;
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
            load = LoadProject;
            InitializeComponent();
        }

        private void ProjectLoading_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            LoadProjectDlg loadProjectDlg = new LoadProjectDlg(LoadProject);
            loadProjectDlg.BeginInvoke(new AsyncCallback(EndInv), null);
        }

        public void EndInv(IAsyncResult iar)
        {
            LoadProjectDlg method = new LoadProjectDlg(EndInvInv);
            try
            {
                Invoke(method);
            }
            catch
            {
            }
        }

        public void EndInvInv()
        {
            Global.MainWnd.UpdateTitle();
            DialogResult = DialogResult.OK;
            Close();
        }

        public void LoadProject()
        {
            try
            {
                try
                {
                    Global.state.ChipRegister = new Dictionary<string, string>();
                    Global.cpd.project = Project.ParseXML(load);
                    if (Global.cpd.project.ProjVer != 0.0 && Global.cpd.project.ProjVer < Global.definition.CProjVer)
                    {
                        if (MessageBox.Show($"古いバージョンのプロジェクトファイルが指定されました。{Environment.NewLine}プロジェクトファイルのアップグレードを試みます。{Environment.NewLine}よろしいですか？", "レガシー プロジェクト ファイルの読み込み", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            SetState("プロジェクトをコンバートしています...");
                            // コンバート動作が未定義
                            double projVer = Global.cpd.project.ProjVer;
                            MessageBox.Show($"このプロジェクトファイルはサポートされていません。{Environment.NewLine}通常の読み込みを試みます。", "コンバート エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            DialogResult = DialogResult.Abort;
                            Close();
                        }
                    }
                    if (Global.cpd.project == null)
                    {
                        MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        DialogResult = DialogResult.Abort;
                        Close();
                    }
                    SetState("プロジェクト設定を読み込んでいます...");
                    Global.cpd.where = Path.GetDirectoryName(load);
                    Global.cpd.filename = load;
                    Global.state.Background = Global.cpd.project.Config.Background;
                    Global.cpd.runtime = Global.cpd.project.Runtime;
                    string[] array = Runtime.CheckFiles(Global.cpd.where, Global.cpd.runtime, false);
                    if (array.Length != 0)
                    {
                        MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{string.Join(",", array)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        DialogResult = DialogResult.Abort;
                        Close();
                    }
                    SetState("チップデータを読み込んでいます...");
                    ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(load), Global.cpd.runtime.Definitions.ChipDefinition));
                    if (Global.cpd.UseLayer)
                    {
                        for (int i = 0; i < chipDataClass.Layerchip.Length; i++)
                        {
                            chipDataClass.Layerchip[i].code = ChipDataClass.CharToCode(chipDataClass.Layerchip[i].character);
                        }
                        Global.cpd.Layerchip = chipDataClass.Layerchip;
                    }
                    for (int i = 0; i < chipDataClass.Mapchip.Length; i++)
                    {
                        chipDataClass.Mapchip[i].code = ChipDataClass.CharToCode(chipDataClass.Mapchip[i].character);
                    }
                    Global.cpd.Mapchip = chipDataClass.Mapchip;
                    Global.cpd.Worldchip = chipDataClass.WorldChip;
                    Global.cpd.VarietyChip = chipDataClass?.VarietyChip;
                    Global.cpd.CustomPartsChip = Global.cpd.project?.CustomPartsDefinition;
                    if(Global.cpd.CustomPartsChip != null) Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[0];
                    Global.cpd.EditingMap = Global.cpd.project.StageData;
                    if (Global.cpd.UseLayer)
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData;
                    }
                    Global.MainWnd.MainDesigner.CreateDrawItemReference();
                    SetState("画像を準備しています...");
                    Global.MainWnd.MainDesigner.PrepareImages();
                    SetState("グラフィカルデザイナを初期化しています...");
                    Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                    if (Global.cpd.UseLayer)
                    {
                        Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
                    }
                    if (Global.state.TransparentUnactiveLayer)
                    {
                        Global.MainWnd.MainDesigner.InitTransparent();
                    }
                    SetState("チップリストを作成しています...");
                    Global.MainWnd.ChipItemReady();
                    SetState("編集を開始します...");
                    Global.MainWnd.MasaoConfigList.Prepare();
                    Global.MainWnd.CustomPartsConfigList.Prepare();
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
            SetStateDelg method = new SetStateDelg(SetStateInv);
            Invoke(method, new object[]
            {
                text
            });
        }

        private void SetStateInv(string text)
        {
            WaiterText.Text = text;
            WaiterText.Refresh();
        }

        private readonly string load = "";

        private delegate void LoadProjectDlg();

        private delegate void SetStateDelg(string text);
    }
}

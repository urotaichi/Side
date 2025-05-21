using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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

        private async void ProjectLoading_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            await Task.Run(LoadProject);
            EndInvInv();
        }

        public void EndInvInv()
        {
            Global.MainWnd.UpdateLayerVisibility();
            Global.MainWnd.UpdateTitle();
            Global.MainWnd.ClearLayerCountsExceptFirst();
            Global.MainWnd.AddLayerMenuItem(Global.cpd.LayerCount - 1);
            DialogResult = DialogResult.OK;
            Close();
        }

        public void LoadProject()
        {
            try
            {
                try
                {
                    Global.state.ChipRegister = [];
                    Global.cpd.project = Project.ParseXML(load);
                    bool flag = false;
                    if (Global.cpd.project.ProjVer != 0.0 && Global.cpd.project.ProjVer < Global.definition.CProjVer)
                    {
                        MessageBox.Show($"古いバージョンのプロジェクトファイルが指定されました。{Environment.NewLine}プロジェクトファイルのアップグレードを試みます。", "レガシー プロジェクト ファイルの読み込み", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        SetState("プロジェクトをコンバートしています...");
                        flag = true;
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
                    if (Global.cpd.project.Config.ScreenSize == "1")
                    {
                        Global.state.MinimumStageSize = new Size(20, 15);
                    }
                    Global.cpd.runtime = Global.cpd.project.Runtime;
                    string[] array = Runtime.CheckFiles(Global.cpd.where, Global.cpd.runtime, false);
                    if (array.Length != 0)
                    {
                        MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{string.Join(",", array)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        MessageBox.Show($"プロジェクトをロードできませんでした。{Environment.NewLine}アプリケーションを再起動します。", "プロジェクトロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        DialogResult = DialogResult.Abort;
                        Close();
                    }
                    void setStageSize(ref Runtime.DefinedData.StageSizeData size)
                    {
                        if (size.x < Global.state.MinimumStageSize.Width) size.x = Global.cpd.project.Runtime.Definitions.StageSize.x;
                        if (size.y < Global.state.MinimumStageSize.Height) size.y = Global.cpd.project.Runtime.Definitions.StageSize.y;
                        size.bytesize = Global.cpd.project.Runtime.Definitions.StageSize.bytesize;
                    }
                    setStageSize(ref Global.cpd.project.Runtime.Definitions.StageSize2);
                    setStageSize(ref Global.cpd.project.Runtime.Definitions.StageSize3);
                    setStageSize(ref Global.cpd.project.Runtime.Definitions.StageSize4);
                    if (Global.cpd.project.Runtime.Definitions.LayerSize.bytesize != 0)
                    {
                        void setLayerSize(ref Runtime.DefinedData.StageSizeData size)
                        {
                            if (size.x < Global.state.MinimumStageSize.Width) size.x = Global.cpd.project.Runtime.Definitions.LayerSize.x;
                            if (size.y < Global.state.MinimumStageSize.Height) size.y = Global.cpd.project.Runtime.Definitions.LayerSize.y;
                            size.bytesize = Global.cpd.project.Runtime.Definitions.LayerSize.bytesize;
                        }
                        setLayerSize(ref Global.cpd.project.Runtime.Definitions.LayerSize2);
                        setLayerSize(ref Global.cpd.project.Runtime.Definitions.LayerSize3);
                        setLayerSize(ref Global.cpd.project.Runtime.Definitions.LayerSize4);
                    }
                    SetState("チップデータを読み込んでいます...");
                    ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(load), Global.cpd.runtime.Definitions.ChipDefinition));
                    if (CurrentProjectData.UseLayer)
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
                    if (Global.cpd.CustomPartsChip != null && Global.cpd.CustomPartsChip.Length > 0) Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[0];
                    Global.cpd.EditingMap = Global.cpd.project.StageData;
                    if (CurrentProjectData.UseLayer)
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData[0];
                    }
                    Global.MainWnd.MainDesigner.CreateDrawItemReference();
                    SetState("画像を準備しています...");
                    Global.MainWnd.MainDesigner.PrepareImages();
                    SetState("グラフィカルデザイナを初期化しています...");
                    Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                    if (CurrentProjectData.UseLayer)
                    {
                        Global.MainWnd.MainDesigner.BackLayerBmp = new List<Bitmap>(Global.cpd.LayerCount);
                        for (int i = 0; i < Global.cpd.LayerCount; i++)
                        {
                            Global.MainWnd.MainDesigner.BackLayerBmp.Add(null);
                            Global.MainWnd.MainDesigner.UpdateBackgroundBuffer(i);
                        }
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
                    if (flag) Global.state.EditFlag = true;
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
            SetStateDelg method = new(SetStateInv);
            Invoke(method,
            [
                text
            ]);
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

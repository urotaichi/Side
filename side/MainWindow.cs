using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MasaoPlus.Controls;
using MasaoPlus.Dialogs;
using MasaoPlus.Properties;

namespace MasaoPlus
{
    public partial class MainWindow : Form
    {
        public event MainDesignerScrollInvoke MainDesignerScroll;

        public MainWindow()
        {
            Global.MainWnd = this;
            Global.state.UpdateCurrentChipInvoke += state_UpdateCurrentChipInvoke;
            Global.state.UpdateCurrentCustomPartsChipInvoke += state_UpdateCurrentCustomPartsChipInvoke;
            InitializeComponent();
            MainDesigner.MouseMove += MainDesigner_MouseMove;
            MainDesigner.ChangeBufferInvoke += MainDesigner_ChangeBufferInvoke;
            MainEditor.PatternChipLayer.Click += EditPatternChip_Click;
            MainEditor.BackgroundLayer.Click += EditBackground_Click;
            MainDesigner.MouseWheel += MainDesigner_MouseWheel;
        }

        private void MainDesigner_MouseWheel(object sender, MouseEventArgs e)
        {
            int num = e.Delta / 120 * Global.cpd.runtime.Definitions.ChipSize.Height;
            bool flag = (ModifierKeys & Keys.Shift) == Keys.Shift;
            if ((!flag && Global.config.draw.HScrollDefault) || (flag && !Global.config.draw.HScrollDefault))
            {
                State state = Global.state;
                state.MapPoint.X -= num;
            }
            else
            {
                State state2 = Global.state;
                state2.MapPoint.Y -= num;
            }
            Global.state.AdjustMapPoint();
            CommitScrollbar();
            MainDesigner.Refresh();
        }

        public void RefreshAll()
        {
            UpdateStatus("パラメータを反映しています...");
            Global.state.ForceNoBuffering = true;
            GuiChipList.Refresh();
            ChipList.Refresh();
            if (Global.cpd.project.Use3rdMapData) GuiCustomPartsChipList.Refresh();
            ChipItemReadyInvoke();
            MainDesigner.UpdateBackgroundBuffer();
            MainDesigner.UpdateForegroundBuffer();
            MainDesigner.InitTransparent();
            MainDesigner.Refresh();
            MainDesigner_ChangeBufferInvoke();
            Global.state.ForceNoBuffering = false;
            UpdateStatus("完了");
        }

        public void Testrun()
        {
            EditTab.SelectedIndex = 2;
        }

        private void MainDesigner_ChangeBufferInvoke()
        {
            ItemUndo.Enabled = MainDesigner.BufferCurrent != 0;
            GMUndo.Enabled = ItemUndo.Enabled;
            ItemRedo.Enabled = MainDesigner.BufferCurrent != MainDesigner.StageBuffer.Count - 1;
            GMRedo.Enabled = ItemRedo.Enabled;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            MUpdateApp.Enabled = Global.definition.IsAutoUpdateEnabled;
            SetDesktopLocation(LogicalToDeviceUnits(Global.config.lastData.WndPoint.X), LogicalToDeviceUnits(Global.config.lastData.WndPoint.Y));
            Size = LogicalToDeviceUnits(Global.config.lastData.WndSize);
            WindowState = Global.config.lastData.WndState;
            MainSplit.SplitterDistance = (int)(Width * Global.config.lastData.SpliterDist);
            if (Global.config.localSystem.ReverseTabView)
            {
                EditTab.Parent = MainSplit.Panel1;
                SideTab.Parent = MainSplit.Panel2;
            }
            else
            {
                SideTab.Parent = MainSplit.Panel1;
                EditTab.Parent = MainSplit.Panel2;
            }
            if (Global.config.draw.StartUpWithClassicCL)
            {
                DrawType.SelectedIndex = 3;
            }
            else
            {
                DrawType.SelectedIndex = 0;
            }
            UpdateTitle();
            ShowGrid.Checked = Global.config.draw.DrawGrid;
            GMShowGrid.Checked = Global.config.draw.DrawGrid;
            int num = (int)(Global.config.draw.ZoomIndex * 100.0);
            if (num <= 50)
            {
                if (num == 25)
                {
                    StageZoom25.Checked = true;
                    GMZoom25.Checked = true;
                    return;
                }
                if (num == 50)
                {
                    StageZoom50.Checked = true;
                    GMZoom50.Checked = true;
                    return;
                }
            }
            else
            {
                if (num == 100)
                {
                    StageZoom100.Checked = true;
                    GMZoom100.Checked = true;
                    return;
                }
                if (num == 200)
                {
                    StageZoom200.Checked = true;
                    GMZoom200.Checked = true;
                    return;
                }
            }
            StageZoom100.Checked = true;
            GMZoom100.Checked = true;
            Global.config.draw.ZoomIndex = 1.0;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            EditorSystemPanel_Resize(this, new EventArgs());
            string text = "";
            if (Environment.GetCommandLineArgs().Length > 1 && !Global.state.ParseCommandline)
            {
                if (Environment.GetCommandLineArgs().Length > 2)
                {
                    MessageBox.Show("起動時に指定できるファイルは一つだけです。", "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    string text2 = Environment.GetCommandLineArgs()[1];
                    if (!File.Exists(text2))
                    {
                        MessageBox.Show($"指定されたファイルが存在しないか、不正なパスです。{Environment.NewLine}{text2}", "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else if (Path.GetExtension(text2) != Global.definition.ProjExt)
                    {
                        MessageBox.Show("指定されたファイルはプロジェクトファイルではありません。", "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        text = text2;
                    }
                }
            }
            if (text == "")
            {
                using StartUp startUp = new();
                if (startUp.ShowDialog() != DialogResult.OK)
                {
                    Close();
                    Application.Exit();
                    return;
                }
                text = startUp.ProjectPath;
            }
            using (ProjectLoading projectLoading = new(text))
            {
                if (projectLoading.ShowDialog() == DialogResult.Abort)
                {
                    Close();
                    Application.Restart();
                    return;
                }
            }
            MainDesigner.AddBuffer();
            EditorSystemPanel_Resize(this, new EventArgs());
            UpdateScrollbar();
            LayerState(CurrentProjectData.UseLayer);
            UpdateStageSelector();
        }

        public void UpdateTitle()
        {
            if (Global.cpd.project == null)
            {
                Text = $"{Global.definition.AppNameFull} - {Global.definition.AppName}";
                return;
            }
            Text = Global.cpd.project.Name;
            if (Global.state.EditFlag)
            {
                Text += "*";
            }
            switch (EditTab.SelectedIndex)
            {
                case 0:
                    Text += " [グラフィカルデザイナ] - ";
                    GMTool.Visible = true;
                    GMEdit.Visible = true;
                    GMStage.Visible = true;
                    TMEdit.Visible = false;
                    BMView.Visible = false;
                    break;
                case 1:
                    Text += " [テキストエディタ] - ";
                    GMTool.Visible = false;
                    GMEdit.Visible = false;
                    GMStage.Visible = true;
                    TMEdit.Visible = true;
                    BMView.Visible = false;
                    break;
                case 2:
                    Text += " [テスト実行] - ";
                    BMView.Visible = true;
                    GMTool.Visible = false;
                    GMEdit.Visible = false;
                    GMStage.Visible = false;
                    TMEdit.Visible = false;
                    break;
            }
            Text += Global.definition.AppName;
        }

        public void UpdateLayerVisibility()
        {
            bool useMultiLayers = CurrentProjectData.UseLayer && Global.cpd.LayerCount > 1;
            
            // レイヤー関連のUIコンポーネントの表示状態を設定
            LayerSelector.Visible = useMultiLayers;
            MainEditor.LayerSelector.Visible = useMultiLayers;
            LayerMenuSelector.Visible = useMultiLayers;
            BackgroundLayer.Visible = !useMultiLayers;
            MainEditor.BackgroundLayer.Visible = !useMultiLayers;
            EditBackground.Visible = !useMultiLayers;
            
            if (!useMultiLayers)
                return;
            
            // レイヤーメニューの再構築
            RebuildLayerMenus();
        }
        
        /// <summary>
        /// レイヤー関連のメニューを再構築します
        /// </summary>
        private void RebuildLayerMenus()
        {
            // 最初のアイテムを保持
            var firstLayerCountItem = LayerCount[0];
            var firstLayerSelectorItem = LayerSelector.DropDownItems[0];
            var firstLayerEditorSelectorItem = MainEditor.LayerSelector.DropDownItems[0];
            var firstLayerMenuCountItem = LayerMenuCount[0];
            var firstLayerMenuSelectorItem = LayerMenuSelector.DropDownItems[0];
            
            // メニュー項目をクリア
            LayerCount.Clear();
            LayerSelector.DropDownItems.Clear();
            MainEditor.LayerSelector.DropDownItems.Clear();
            LayerMenuCount.Clear();
            LayerMenuSelector.DropDownItems.Clear();
            
            // 最初のアイテムを復元
            LayerCount.Add(firstLayerCountItem);
            LayerSelector.DropDownItems.Add(firstLayerSelectorItem);
            MainEditor.LayerSelector.DropDownItems.Add(firstLayerEditorSelectorItem);
            LayerMenuCount.Add(firstLayerMenuCountItem);
            LayerMenuSelector.DropDownItems.Add(firstLayerMenuSelectorItem);
            
            // レイヤーメニュー項目を追加
            for(int i = 1; i < Global.cpd.LayerCount; i++)
            {
                AddLayerMenuItem(i);
            }
        }
        
        /// <summary>
        /// 指定されたインデックスのレイヤーメニュー項目を追加します
        /// </summary>
        /// <param name="index">レイヤーのインデックス</param>
        private void AddLayerMenuItem(int index)
        {
            // LayerSelector用のメニュー項目を作成
            var layerSelectorItem = CreateLayerMenuItem("LayerSelector", index);
            LayerCount.Add(layerSelectorItem);
            LayerSelector.DropDownItems.Add(layerSelectorItem);
            
            var layerEditorSelectorItem = CreateLayerMenuItem("LayerEditorSelector", index);
            MainEditor.LayerCount.Add(layerEditorSelectorItem);
            MainEditor.LayerSelector.DropDownItems.Add(layerEditorSelectorItem);
            
            // LayerMenuSelector用のメニュー項目を作成
            var layerMenuCountItem = CreateLayerMenuItem("LayerMenuCount", index);
            LayerMenuCount.Add(layerMenuCountItem);
            LayerMenuSelector.DropDownItems.Add(layerMenuCountItem);
        }
        
        /// <summary>
        /// レイヤーメニュー項目を作成します
        /// </summary>
        /// <param name="name">メニュー項目の名前の接頭辞</param>
        /// <param name="index">レイヤーのインデックス</param>
        /// <returns>作成されたToolStripMenuItem</returns>
        private ToolStripMenuItem CreateLayerMenuItem(string name, int index)
        {
            var menuItem = new ToolStripMenuItem
            {
                Checked = false,
                CheckState = CheckState.Unchecked,
                Name = $"{name}{index + 1}",
                Size = LogicalToDeviceUnits(new Size(180, 22)),
                Text = $"レイヤー {index + 1}"
            };
            
            menuItem.Click += (sender, e) => LayerCount_Click(index);
            
            return menuItem;
        }

        public void LayerCount_Click(int layerIndex)
        {
            if (Global.state.EdittingLayerIndex == layerIndex)
            {
                return;
            }
            if (!ChangePreCheck())
            {
                return;
            }
            switch (Global.state.EdittingStage)
            {
                case 0:
                    Global.cpd.EditingLayer = Global.cpd.project.LayerData[layerIndex];
                    break;
                case 1:
                    Global.cpd.EditingLayer = Global.cpd.project.LayerData2[layerIndex];
                    break;
                case 2:
                    Global.cpd.EditingLayer = Global.cpd.project.LayerData3[layerIndex];
                    break;
                case 3:
                    Global.cpd.EditingLayer = Global.cpd.project.LayerData4[layerIndex];
                    break;
            }
            Global.state.EditingForeground = false;
            EditPatternChip.Checked = false;
            PatternChipLayer.Checked = false;
            MainEditor.PatternChipLayer.Checked = false;
            for(int i = 0; i < LayerCount.Count; i++)
            {
                if (i == layerIndex)
                {
                    LayerCount[i].Checked = true;
                    MainEditor.LayerCount[i].Checked = true;
                    LayerMenuCount[i].Checked = true;
                }
                else
                {
                    LayerCount[i].Checked = false;
                    MainEditor.LayerCount[i].Checked = false;
                    LayerMenuCount[i].Checked = false;
                }
            }
            MainEditor.BackgroundLayer.Checked = true;
            Global.state.EdittingLayerIndex = layerIndex;
            UpdateLayer(layerIndex);
        }

        // ステータスバーっぽいところに表示される小さいアイコンや文字
        private void MainDesigner_MouseMove(object sender, MouseEventArgs e)
        {
            bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
            Point p = default;
            p.X = (e.X + Global.state.MapPoint.X) / MainDesigner.CurrentChipSize.Width;
            p.Y = (e.Y + Global.state.MapPoint.Y) / MainDesigner.CurrentChipSize.Height;
            GuiPositionInfo.Text = $"{p.X},{p.Y}";
            if (!GUIDesigner.StageText.IsOverflow(p))
            {
                int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : (Global.state.EditingForeground ? Global.cpd.runtime.Definitions.StageSize.bytesize : Global.cpd.runtime.Definitions.LayerSize.bytesize);
                string text;
                if (Global.state.EditingForeground)
                {
                    if (Global.state.Use3rdMapDataCurrently) text = Global.cpd.EditingMap[p.Y].Split(',')[p.X];
                    else text = Global.cpd.EditingMap[p.Y].Substring(p.X * num, num);
                }
                else
                {
                    if (Global.cpd.project.Use3rdMapData) text = Global.cpd.EditingLayer[p.Y].Split(',')[p.X];
                    else text = Global.cpd.EditingLayer[p.Y].Substring(p.X * num, num);
                }
                ChipsData chipsData;
                if (Global.state.MapEditMode)
                {
                    if (text == Global.cpd.Worldchip[0].character || !MainDesigner.DrawWorldRef.TryGetValue(text, out ChipsData value))
                    {
                        ChipNavigator.Visible = false;
                        return;
                    }
                    chipsData = value;
                }
                else if (Global.state.EditingForeground)
                {
                    if (Global.cpd.project.Use3rdMapData && (text == Global.cpd.Mapchip[0].code || !MainDesigner.DrawItemCodeRef.ContainsKey(text))
                        || !Global.cpd.project.Use3rdMapData && (text == Global.cpd.Mapchip[0].character || !MainDesigner.DrawItemRef.ContainsKey(text)))
                    {
                        ChipNavigator.Visible = false;
                        return;
                    }
                    if (Global.cpd.project.Use3rdMapData) chipsData = MainDesigner.DrawItemCodeRef[text];
                    else chipsData = MainDesigner.DrawItemRef[text];
                }
                else
                {
                    if (Global.cpd.project.Use3rdMapData && (text == Global.cpd.Layerchip[0].code || !MainDesigner.DrawLayerCodeRef.ContainsKey(text))
                        || !Global.cpd.project.Use3rdMapData && (text == Global.cpd.Layerchip[0].character || !MainDesigner.DrawLayerRef.ContainsKey(text)))
                    {
                        ChipNavigator.Visible = false;
                        return;
                    }
                    if (Global.cpd.project.Use3rdMapData) chipsData = MainDesigner.DrawLayerCodeRef[text];
                    else chipsData = MainDesigner.DrawLayerRef[text];
                }
                ChipData cschip = chipsData.GetCSChip();
                Size size = (cschip.size == default) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size;
                if (oriboss_view && chipsData.character == "Z" && MainDesigner.DrawOribossOrig != null) size = MainDesigner.DrawOribossOrig.Size;
                ChipNavigator.Image?.Dispose();
                Bitmap bitmap;
                if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
                {
                    bitmap = new Bitmap(size.Height, size.Width);
                }
                else bitmap = new Bitmap(size.Width, size.Height);
                using Graphics graphics = Graphics.FromImage(bitmap);

                using Brush brush = new SolidBrush(Global.state.Background);
                graphics.FillRectangle(brush, new Rectangle(new Point(0, 0), size));

                if (Global.state.EditingForeground)
                {
                    graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    if (oriboss_view && chipsData.character == "Z")
                    {
                        if (MainDesigner.DrawOribossOrig != null) graphics.DrawImage(MainDesigner.DrawOribossOrig, 0, 0, MainDesigner.DrawOribossOrig.Size.Width, MainDesigner.DrawOribossOrig.Size.Height);
                    }
                    else if (ChipRenderer.IsAthleticChip(cschip.name))
                    {
                        AthleticView.list[cschip.name].Min(cschip, graphics, size);
                    }
                    else
                    {
                        int rotate_o = default;
                        if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
                        {
                            rotate_o = (cschip.size.Width - cschip.size.Height) / (cschip.size.Width / cschip.size.Height) * Math.Sign(cschip.rotate);
                        }
                        graphics.TranslateTransform(size.Width / 2, size.Height / 2);
                        graphics.RotateTransform(cschip.rotate);

                        // 水の半透明処理
                        if (ChipRenderer.ShouldApplyWaterTransparency(out float waterLevel) && chipsData.character == "4")
                        {
                            ChipRenderer.ApplyWaterTransparency(graphics, MainDesigner.DrawChipOrig,
                                new Rectangle(new Point(-size.Width / 2 + rotate_o, -size.Height / 2 + rotate_o), size),
                                cschip.pattern, size, waterLevel);
                        }
                        else graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(new Point(-size.Width / 2 + rotate_o, -size.Height / 2 + rotate_o), size), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    graphics.DrawImage(MainDesigner.DrawLayerOrig[Global.state.EdittingLayerIndex], new Rectangle(new Point(0, 0), size), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
                }
                ChipNavigator.Image = (Image)bitmap.Clone();
                bitmap.Dispose();
                string name, description;

                if (Global.state.EditingForeground && oriboss_view && chipsData.character == "Z")
                {
                    name = "オリジナルボス";
                    if (Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki))
                    {
                        description = ChipRenderer.GetOribossMovementDescription(int.Parse(oriboss_ugoki));
                    }
                    else description = "";

                }
                else
                {
                    name = cschip.name;
                    description = cschip.description;
                }

                if (Global.state.Use3rdMapDataCurrently) ChipNavigator.Text = $"[{chipsData.code}]{name}/{description}";
                else ChipNavigator.Text = $"[{chipsData.character}]{name}/{description}";
                ChipNavigator.Visible = true;
            }
        }

        public void CommitScrollbar()
        {
            GHorzScroll.Value = Global.state.MapPoint.X;
            GVirtScroll.Value = Global.state.MapPoint.Y;
            MainDesignerScroll?.Invoke();
        }

        public void UpdateScrollbar()
        {
            UpdateScrollbar(Global.config.draw.ZoomIndex);
        }

        public void UpdateScrollbar(double oldZoomIndex)
        {
            double num = Global.config.draw.ZoomIndex / oldZoomIndex;
            Point point = new((int)(GHorzScroll.Value * num), (int)(GVirtScroll.Value * num));
            Size displaySize = MainDesigner.DisplaySize;
            Size mapMoveMax = new(displaySize.Width - MainDesigner.Width, displaySize.Height - MainDesigner.Height);
            Global.state.MapMoveMax = mapMoveMax;
            if (displaySize.Width <= MainDesigner.Width)
            {
                GHorzScroll.Enabled = false;
            }
            else
            {
                Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : GUIDesigner.CurrentStageSize;
                GHorzScroll.Enabled = true;
                GHorzScroll.LargeChange = (int)(displaySize.Width / (stageSizeData.x / 4.0) * Global.config.draw.ZoomIndex);
                GHorzScroll.Maximum = mapMoveMax.Width + GHorzScroll.LargeChange - 1;
            }
            if (displaySize.Height <= MainDesigner.Height)
            {
                GVirtScroll.Enabled = false;
            }
            else
            {
                Runtime.DefinedData.StageSizeData stageSizeData2 = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : GUIDesigner.CurrentStageSize;
                GVirtScroll.Enabled = true;
                GVirtScroll.LargeChange = (int)(displaySize.Height / (stageSizeData2.y / 4.0) * Global.config.draw.ZoomIndex);
                GVirtScroll.Maximum = mapMoveMax.Height + GVirtScroll.LargeChange - 1;
                GVirtScroll.Maximum = mapMoveMax.Height + GVirtScroll.LargeChange - 1;
            }
            if (GHorzScroll.Enabled)
            {
                if (point.X > mapMoveMax.Width)
                {
                    GHorzScroll.Value = mapMoveMax.Width;
                }
                else
                {
                    GHorzScroll.Value = point.X;
                }
            }
            else
            {
                GHorzScroll.Value = 0;
            }
            Global.state.MapPoint.X = GHorzScroll.Value;
            if (GVirtScroll.Enabled)
            {
                if (point.Y > mapMoveMax.Height)
                {
                    GVirtScroll.Value = mapMoveMax.Height;
                }
                else
                {
                    GVirtScroll.Value = point.Y;
                }
            }
            else
            {
                GVirtScroll.Value = 0;
            }
            Global.state.MapPoint.Y = GVirtScroll.Value;
            MainDesignerScroll?.Invoke();
            MainDesigner.Refresh();
        }

        private void EditorSystemPanel_Resize(object sender, EventArgs e)
        {
            MainDesigner.Width = EditorSystemPanel.Width - LogicalToDeviceUnits(20);
            MainDesigner.Height = EditorSystemPanel.Height - LogicalToDeviceUnits(20);
            GVirtScroll.Left = MainDesigner.Width;
            GVirtScroll.Height = MainDesigner.Height;
            GVirtScroll.Refresh();
            GHorzScroll.Width = MainDesigner.Width;
            GHorzScroll.Top = MainDesigner.Height;
            GHorzScroll.Refresh();
            UpdateScrollbar();
        }

        private void EditTab_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1 && !MainEditor.CanConvertTextSource() && MessageBox.Show($"ステージのテキストが規定の形式を満たしていないため、デザイナでの編集やテスト実行はできません。{Environment.NewLine}編集を続行すると、テキストでの編集結果は失われます。{Environment.NewLine}続行してもよろしいですか？", "コンバート失敗", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }

            if (e.TabPageIndex == 2)
            {
                IntegrateBrowser.TrySuspendAsync();
            }
        }

        private void EditTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2)
            {
                if (Global.state.QuickTestrunSource != null)
                {
                    Subsystem.MakeTestrun(Global.state.EdittingStage, Global.state.EdittingStage, Global.state.QuickTestrunSource);
                    Global.state.QuickTestrunSource = null;
                }
                else if (Global.state.TestrunAll)
                {
                    Subsystem.MakeTestrun(-1);
                    Global.state.TestrunAll = false;
                }
                else
                {
                    Subsystem.MakeTestrun(Global.state.EdittingStage);
                }
                if (!IntegrateBrowser.Navigate(Subsystem.GetTempFileWhere()))
                {
                    e.Cancel = true;
                }
            }
        }

        private static string JoinLayerToCloneableString(LayerObject layer)
        {
            return (string)string.Join(Environment.NewLine, layer).Clone();
        }

        private void EditTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Restartupping)
            {
                return;
            }
            if (EditTab.SelectedIndex == 1)
            {
                if (StageConvert)
                {
                    if (Global.state.EditingForeground)
                    {
                        MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingMap);
                    }
                    else
                    {
                        MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingLayer);
                    }
                }
                else
                {
                    StageConvert = true;
                }
                MainEditor.BufferClear();
                MainEditor.AddBuffer();
                ChipList.Enabled = false;
                GuiChipList.Enabled = false;
                if (Global.cpd.project.Use3rdMapData) GuiCustomPartsChipList.Enabled = false;
                MainEditor.InitEditor();
                MSave.Enabled = false;
                MSaveAs.Enabled = false;
                MTSave.Enabled = false;
            }
            else
            {
                if (MainEditor.CanConvertTextSource())
                {
                    LayerObject clonedText = [.. (string[])MainEditor.StageTextEditor.Lines.Clone()];
                    if (Global.state.EditingForeground)
                    {
                        switch (Global.state.EdittingStage)
                        {
                            case 0:
                                Global.cpd.project.StageData = clonedText;
                                Global.cpd.EditingMap = Global.cpd.project.StageData;
                                break;
                            case 1:
                                Global.cpd.project.StageData2 = clonedText;
                                Global.cpd.EditingMap = Global.cpd.project.StageData2;
                                break;
                            case 2:
                                Global.cpd.project.StageData3 = clonedText;
                                Global.cpd.EditingMap = Global.cpd.project.StageData3;
                                break;
                            case 3:
                                Global.cpd.project.StageData4 = clonedText;
                                Global.cpd.EditingMap = Global.cpd.project.StageData4;
                                break;
                            case 4:
                                Global.cpd.project.MapData = clonedText;
                                Global.cpd.EditingMap = Global.cpd.project.MapData;
                                break;
                        }
                        MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingMap);
                        MainEditor.BufferClear();
                        MainEditor.AddBuffer();
                    }
                    else
                    {
                        switch (Global.state.EdittingStage)
                        {
                            case 0:
                                Global.cpd.project.LayerData[Global.state.EdittingLayerIndex] = clonedText;
                                Global.cpd.EditingLayer = Global.cpd.project.LayerData[Global.state.EdittingLayerIndex];
                                break;
                            case 1:
                                Global.cpd.project.LayerData2[Global.state.EdittingLayerIndex] = clonedText;
                                Global.cpd.EditingLayer = Global.cpd.project.LayerData2[Global.state.EdittingLayerIndex];
                                break;
                            case 2:
                                Global.cpd.project.LayerData3[Global.state.EdittingLayerIndex] = clonedText;
                                Global.cpd.EditingLayer = Global.cpd.project.LayerData3[Global.state.EdittingLayerIndex];
                                break;
                            case 3:
                                Global.cpd.project.LayerData4[Global.state.EdittingLayerIndex] = clonedText;
                                Global.cpd.EditingLayer = Global.cpd.project.LayerData4[Global.state.EdittingLayerIndex];
                                break;
                        }
                        MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingLayer);
                        MainEditor.BufferClear();
                        MainEditor.AddBuffer();
                    }
                    MainDesigner.ClearBuffer();
                    MainDesigner.StageSourceToDrawBuffer();
                    MainDesigner.AddBuffer();
                }
                ChipList.Enabled = true;
                GuiChipList.Enabled = true;
                if (Global.cpd.project.Use3rdMapData) GuiCustomPartsChipList.Enabled = true;
                MSave.Enabled = true;
                MSaveAs.Enabled = true;
                MTSave.Enabled = true;
                UpdateStatus("完了");
            }
            UpdateTitle();
        }

        public void ChipItemReady()
        {
            CRID method = new(ChipItemReadyInvoke);
            Invoke(method);
        }

        // クラシックチップリスト以外
        public void ChipItemReadyInvoke()
        {
            ChipList.BeginUpdate();
            ChipList.Items.Clear();
            ChipsData[] array;
            if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
            {
                if (Global.state.MapEditMode)
                {
                    array = Global.cpd.Worldchip;
                }
                else
                {
                    array = Global.cpd.Mapchip;
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        array = [.. array, .. Global.cpd.VarietyChip];
                        if (Global.cpd.CustomPartsChip != null)
                        {
                            array = [.. array, .. Global.cpd.CustomPartsChip];
                        }
                    }
                }
            }
            else
            {
                array = Global.cpd.Layerchip;
            }

            foreach (ChipsData chipsData in array)
            {
                ChipData chipData = chipsData.GetCSChip();
                if (chipData.description == "")
                {
                    ChipList.Items.Add(chipData.name);
                }
                else if (Global.state.EditingForeground && chipsData.character == "Z" && Global.state.ChipRegister.TryGetValue("oriboss_v", out string value) && int.Parse(value) == 3)
                {
                    string description;
                    if (Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki))
                    {
                        description = ChipRenderer.GetOribossMovementDescription(int.Parse(oriboss_ugoki));
                    }
                    else description = "";

                    ChipList.Items.Add($"オリジナルボス/{description}");

                }
                else
                {
                    ChipList.Items.Add($"{chipData.name}/{chipData.description}");
                }
            }
            if (ChipList.Items.Count != 0)
            {
                ChipList.SelectedIndex = 0;
            }
            ChipList.EndUpdate();
        }

        private void GVirtScroll_Scroll(object sender, ScrollEventArgs e)
        {
            Global.state.MapPoint.Y = GVirtScroll.Value;
            MainDesigner.Refresh();
            MainDesignerScroll?.Invoke();
        }

        private void GHorzScroll_Scroll(object sender, ScrollEventArgs e)
        {
            Global.state.MapPoint.X = GHorzScroll.Value;
            MainDesigner.Refresh();
            MainDesignerScroll?.Invoke();
        }

        private void ShowGrid_Click(object sender, EventArgs e)
        {
            GMShowGrid.Checked = ShowGrid.Checked;
            Global.config.draw.DrawGrid = ShowGrid.Checked;
            MainDesigner.Refresh();
        }

        private void GMShowGrid_Click(object sender, EventArgs e)
        {
            ShowGrid.Checked = GMShowGrid.Checked;
            Global.config.draw.DrawGrid = GMShowGrid.Checked;
            MainDesigner.Refresh();
        }

        private void ZoomInit()
        {
            StageZoom25.Checked = false;
            StageZoom50.Checked = false;
            StageZoom100.Checked = false;
            StageZoom200.Checked = false;
            GMZoom25.Checked = false;
            GMZoom50.Checked = false;
            GMZoom100.Checked = false;
            GMZoom200.Checked = false;
        }

        private void StageZoom25_Click(object sender, EventArgs e)
        {
            ZoomInit();
            StageZoom25.Checked = true;
            GMZoom25.Checked = true;
            double zoomIndex = Global.config.draw.ZoomIndex;
            Global.config.draw.ZoomIndex = 0.25;
            UpdateScrollbar(zoomIndex);
        }

        private void StageZoom50_Click(object sender, EventArgs e)
        {
            ZoomInit();
            StageZoom50.Checked = true;
            GMZoom50.Checked = true;
            double zoomIndex = Global.config.draw.ZoomIndex;
            Global.config.draw.ZoomIndex = 0.5;
            UpdateScrollbar(zoomIndex);
        }

        private void StageZoom100_Click(object sender, EventArgs e)
        {
            ZoomInit();
            StageZoom100.Checked = true;
            GMZoom100.Checked = true;
            double zoomIndex = Global.config.draw.ZoomIndex;
            Global.config.draw.ZoomIndex = 1.0;
            UpdateScrollbar(zoomIndex);
        }

        private void StageZoom200_Click(object sender, EventArgs e)
        {
            ZoomInit();
            StageZoom200.Checked = true;
            GMZoom200.Checked = true;
            double zoomIndex = Global.config.draw.ZoomIndex;
            Global.config.draw.ZoomIndex = 2.0;
            UpdateScrollbar(zoomIndex);
        }

        private void ChipList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = ChipList.SelectedIndex;

            if (i >= 0)
            {
                if (Global.state.MapEditMode)
                {
                    Global.state.CurrentChip = Global.cpd.Worldchip[i];
                    return;
                }
                if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                {
                    ChipsData[] array = Global.cpd.Mapchip;
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        array = [.. array, .. Global.cpd.VarietyChip];
                        if (Global.cpd.CustomPartsChip != null)
                        {
                            array = [.. array, .. Global.cpd.CustomPartsChip];
                        }
                    }
                    Global.state.CurrentChip = array[i];
                    return;
                }
                Global.state.CurrentChip = Global.cpd.Layerchip[i];
            }
        }

        // 選択中のチップが変わったらチップリストの左上の文字を変える
        private void state_UpdateCurrentChipInvoke()
        {
            bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
            ChipImage.Refresh();
            string chara;
            ChipsData cc = Global.state.CurrentChip;
            if (Global.state.Use3rdMapDataCurrently) chara = cc.code;
            else chara = cc.character;

            if (cc.character == "Z" && oriboss_view)
                ChipDescription.Text = "オリジナルボス";
            else ChipDescription.Text = cc.GetCSChip().name;
            if (cc.GetCSChip().description != "")
            {
                if (Global.state.EditingForeground && cc.character == "Z" && oriboss_view)
                {
                    string description;
                    if (Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki))
                    {
                        description = ChipRenderer.GetOribossMovementDescription(int.Parse(oriboss_ugoki));
                    }
                    else description = "";

                    ChipChar.Text = $"[{chara}]{description}";

                }
                else ChipChar.Text = $"[{chara}]{cc.GetCSChip().description}";
            }
            else
            {
                ChipChar.Text = $"[{chara}]";
            }
            if (DrawType.SelectedIndex == 3)
            {
                GuiChipList.Refresh();
                return;
            }
            int num = 0;
            if (Global.state.MapEditMode)
            {
                foreach (ChipsData chipsData in Global.cpd.Worldchip)
                {
                    if (cc.character == chipsData.character)
                    {
                        ChipList.SelectedIndex = num;
                        return;
                    }
                    num++;
                }
                return;
            }
            if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
            {
                if (Global.cpd.project.Use3rdMapData)
                {
                    foreach (ChipsData chipsData2 in Global.cpd.Mapchip)
                    {
                        if (cc.code == chipsData2.code)
                        {
                            ChipList.SelectedIndex = num;
                            return;
                        }
                        num++;
                    }
                }
                else
                {
                    foreach (ChipsData chipsData2 in Global.cpd.Mapchip)
                    {
                        if (cc.character == chipsData2.character)
                        {
                            ChipList.SelectedIndex = num;
                            return;
                        }
                        num++;
                    }
                }
                return;
            }
            foreach (ChipsData chipsData3 in Global.cpd.Layerchip)
            {
                if (Global.cpd.project.Use3rdMapData)
                {
                    if (cc.code == chipsData3.code)
                    {
                        ChipList.SelectedIndex = num;
                        return;
                    }
                    num++;
                }
                else
                {
                    if (cc.character == chipsData3.character)
                    {
                        ChipList.SelectedIndex = num;
                        return;
                    }
                    num++;
                }
            }
        }

        // チップリストの左上の画像が変わった時
        // 左上のチップのサンプル画像　拡張描画画像がなぜか描画されていないので後で追加？
        private void ChipImage_Paint(object sender, PaintEventArgs e)
        {
            bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
            if (MainDesigner.DrawChipOrig != null)
            {
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                ChipData cschip = Global.state.CurrentChip.GetCSChip();
                if (DeviceDpi / 96 >= 2 && (cschip.size == default || cschip.size.Width / cschip.size.Height == 1))
                {
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }
                else e.Graphics.InterpolationMode = InterpolationMode.High;
                if (!CurrentProjectData.UseLayer || Global.state.EditingForeground) // パターン画像
                {
                    if (Global.state.EditingForeground && oriboss_view && Global.state.CurrentChip.character == "Z")
                    {
                        if (MainDesigner.DrawOribossOrig != null) e.Graphics.DrawImage(MainDesigner.DrawOribossOrig, 0, 0, ChipImage.Width, ChipImage.Height);
                    }
                    else if (ChipRenderer.IsAthleticChip(cschip.name))
                    {
                        AthleticView.list[cschip.name].Main(this, cschip, e.Graphics, new Size(32, 32));
                    }
                    else
                    {
                        e.Graphics.TranslateTransform(ChipImage.Width / 2, ChipImage.Height / 2);
                        if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                        // 水の半透明処理
                        if (ChipRenderer.ShouldApplyWaterTransparency(out float waterLevel) && Global.state.CurrentChip.character == "4")
                        {
                            ChipRenderer.ApplyWaterTransparency(e.Graphics, MainDesigner.DrawChipOrig,
                                new Rectangle(-ChipImage.Width / 2, -ChipImage.Height / 2, ChipImage.Width, ChipImage.Height),
                                cschip.pattern, Global.cpd.runtime.Definitions.ChipSize, waterLevel);
                        }
                        else e.Graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(-ChipImage.Width / 2, -ChipImage.Height / 2, ChipImage.Width, ChipImage.Height), new Rectangle(cschip.pattern, (cschip.size == default) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size), GraphicsUnit.Pixel);
                    }
                }
                else // レイヤー画像
                {
                    e.Graphics.DrawImage(MainDesigner.DrawLayerOrig[Global.state.EdittingLayerIndex], new Rectangle(new Point(0, 0), ChipImage.Size), new Rectangle(cschip.pattern, (cschip.size == default) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size), GraphicsUnit.Pixel);
                }
            }
        }

        // カスタムパーツリストの左上の文字
        private void state_UpdateCurrentCustomPartsChipInvoke()
        {
            CustomPartsChipImage.Refresh();
            ChipsData cc = Global.state.CurrentCustomPartsChip;

            CustomPartsChipDescription.Text = cc.GetCSChip().name;
            if (cc.GetCSChip().description != "")
            {
                CustomPartsChipChar.Text = $"[{cc.code}]{cc.GetCSChip().description}";
            }
            else
            {
                CustomPartsChipChar.Text = $"[{cc.code}]";
            }
            GuiCustomPartsChipList.Refresh();
            return;
        }

        // カスタムパーツリストの左上
        private void CustomPartsChipImage_Paint(object sender, PaintEventArgs e)
        {
            if (MainDesigner.DrawChipOrig != null)
            {
                ChipData cschip = Global.state.CurrentCustomPartsChip.GetCSChip();
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                if (DeviceDpi / 96 >= 2 && (cschip.size == default || cschip.size.Width / cschip.size.Height == 1))
                {
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }
                else e.Graphics.InterpolationMode = InterpolationMode.High;

                if (ChipRenderer.IsAthleticChip(cschip.name))
                {
                    AthleticView.list[cschip.name].Main(this, cschip, e.Graphics, new Size(ChipImage.Width, ChipImage.Height));
                }
                else
                {
                    e.Graphics.TranslateTransform(ChipImage.Width / 2, ChipImage.Height / 2);
                    if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                    e.Graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(-ChipImage.Width / 2, -ChipImage.Height / 2, ChipImage.Width, ChipImage.Height), new Rectangle(cschip.pattern, (cschip.size == default) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size), GraphicsUnit.Pixel);
                }
            }
        }

        private void ItemReload_Click(object sender, EventArgs e)
        {
            UpdateStatus("描画中...");
            Global.state.ForceNoBuffering = true;
            int tickCount = Environment.TickCount;
            MainDesigner.StageSourceToDrawBuffer();
            MainDesigner.Refresh();
            Global.state.ForceNoBuffering = false;
            UpdateStatus($"完了({Environment.TickCount - tickCount}ms)");
        }

        public void UpdateStatus(string t)
        {
            CurrentStatus.Text = t;
            GUIEditorStatus.Refresh();
        }

        private void ItemUncheck()
        {
            ItemCursor.Checked = false;
            ItemPen.Checked = false;
            ItemLine.Checked = false;
            ItemRect.Checked = false;
            ItemFill.Checked = false;
            GMCursor.Checked = false;
            GMPen.Checked = false;
            GMLine.Checked = false;
            GMRect.Checked = false;
            GMFill.Checked = false;
        }

        private void ItemCursor_Click(object sender, EventArgs e)
        {
            ItemUncheck();
            ItemCursor.Checked = true;
            GMCursor.Checked = true;
            MainDesigner.CurrentTool = GUIDesigner.EditTool.Cursor;
        }

        private void ItemPen_Click(object sender, EventArgs e)
        {
            ItemUncheck();
            ItemPen.Checked = true;
            GMPen.Checked = true;
            MainDesigner.CurrentTool = GUIDesigner.EditTool.Pen;
        }

        private void ItemLine_Click(object sender, EventArgs e)
        {
            ItemUncheck();
            ItemLine.Checked = true;
            GMLine.Checked = true;
            MainDesigner.CurrentTool = GUIDesigner.EditTool.Line;
        }

        private void ItemRect_Click(object sender, EventArgs e)
        {
            ItemUncheck();
            ItemRect.Checked = true;
            GMRect.Checked = true;
            MainDesigner.CurrentTool = GUIDesigner.EditTool.Rect;
        }

        private void ItemFill_Click(object sender, EventArgs e)
        {
            ItemUncheck();
            ItemFill.Checked = true;
            GMFill.Checked = true;
            MainDesigner.CurrentTool = GUIDesigner.EditTool.Fill;
        }

        private void ItemUndo_Click(object sender, EventArgs e)
        {
            MainDesigner.Undo();
        }

        private void GMUndo_Click(object sender, EventArgs e)
        {
            ItemUndo_Click(this, new EventArgs());
        }

        private void ItemRedo_Click(object sender, EventArgs e)
        {
            MainDesigner.Redo();
        }

        private void GMRedo_Click(object sender, EventArgs e)
        {
            ItemRedo_Click(this, new EventArgs());
        }

        private void GMCut_Click(object sender, EventArgs e)
        {
            GuiCut.Checked = !GuiCut.Checked;
            GuiCut_Click(sender, e);
        }

        private void GMCopy_Click(object sender, EventArgs e)
        {
            GuiCopy.Checked = !GuiCopy.Checked;
            GuiCopy_Click(sender, e);
        }

        private void GMPaste_Click(object sender, EventArgs e)
        {
            GuiPaste.Checked = !GuiPaste.Checked;
            GuiPaste_Click(sender, e);
        }

        private void MainDesigner_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (keyCode - Keys.Left <= 3)
            {
                e.IsInputKey = true;
                return;
            }
        }

        private void MainDesigner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                e.Handled = true;
                Keys keyCode = e.KeyCode;
                if (keyCode <= Keys.F)
                {
                    switch (keyCode)
                    {
                        case Keys.Space:
                            using (OverViewWindow overViewWindow = new())
                            {
                                overViewWindow.ShowDialog();
                                return;
                            }
                        case Keys.Prior:
                        case Keys.Next:
                        case Keys.End:
                        case Keys.Home:
                            goto IL_2CF;
                        case Keys.Left:
                            break;
                        case Keys.Up:
                            if (Global.config.draw.PageScroll)
                            {
                                MoveMainDesigner(new Size(0, MainDesigner.Height * -1));
                                return;
                            }
                            MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height * -1));
                            return;
                        case Keys.Right:
                            if (Global.config.draw.PageScroll)
                            {
                                MoveMainDesigner(new Size(MainDesigner.Width, 0));
                                return;
                            }
                            MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width, 0));
                            return;
                        case Keys.Down:
                            if (Global.config.draw.PageScroll)
                            {
                                MoveMainDesigner(new Size(0, MainDesigner.Height));
                                return;
                            }
                            MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height));
                            return;
                        default:
                            switch (keyCode)
                            {
                                case Keys.C:
                                    ItemCursor_Click(this, new EventArgs());
                                    return;
                                case Keys.D:
                                    ItemPen_Click(this, new EventArgs());
                                    return;
                                case Keys.E:
                                    goto IL_2CF;
                                case Keys.F:
                                    ItemFill_Click(this, new EventArgs());
                                    return;
                                default:
                                    goto IL_2CF;
                            }
                    }
                    if (Global.config.draw.PageScroll)
                    {
                        MoveMainDesigner(new Size(MainDesigner.Width * -1, 0));
                        return;
                    }
                    MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width * -1, 0));
                    return;
                }
                else
                {
                    if (keyCode == Keys.R)
                    {
                        ItemRect_Click(this, new EventArgs());
                        return;
                    }
                    if (keyCode == Keys.S)
                    {
                        ItemLine_Click(this, new EventArgs());
                        return;
                    }
                    switch (keyCode)
                    {
                        case Keys.F1:
                            StageSelectionChange(0);
                            return;
                        case Keys.F2:
                            StageSelectionChange(1);
                            return;
                        case Keys.F3:
                            StageSelectionChange(2);
                            return;
                        case Keys.F4:
                            StageSelectionChange(3);
                            return;
                        case Keys.F6:
                            StageSelectionChange(4);
                            return;
                        case Keys.F7:
                            EditPatternChip_Click(this, new EventArgs());
                            return;
                        case Keys.F8:
                            EditBackground_Click(this, new EventArgs());
                            return;
                        case Keys.F9:
                            StageZoom25_Click(this, new EventArgs());
                            return;
                        case Keys.F10:
                            StageZoom50_Click(this, new EventArgs());
                            return;
                        case Keys.F11:
                            StageZoom100_Click(this, new EventArgs());
                            return;
                        case Keys.F12:
                            StageZoom200_Click(this, new EventArgs());
                            return;
                    }
                }
            IL_2CF:
                e.Handled = false;
                return;
            }
            if (e.Shift)
            {
                if (e.KeyCode != Keys.ShiftKey)
                {
                    return;
                }
                if (MainDesigner.CurrentTool != GUIDesigner.EditTool.Cursor)
                {
                    OldET = MainDesigner.CurrentTool;
                    MainDesigner.CurrentTool = GUIDesigner.EditTool.Cursor;
                    return;
                }
            }
            else if (e.Control && e.Control)
            {
                e.Handled = true;
                Keys keyCode2 = e.KeyCode;
                if (keyCode2 <= Keys.G)
                {
                    if (keyCode2 == Keys.C)
                    {
                        GMCopy_Click(this, new EventArgs());
                        goto IL_3E8;
                    }
                    if (keyCode2 == Keys.G)
                    {
                        ShowGrid.Checked = !ShowGrid.Checked;
                        ShowGrid_Click(this, new EventArgs());
                        goto IL_3E8;
                    }
                }
                else
                {
                    switch (keyCode2)
                    {
                        case Keys.V:
                            GMPaste_Click(this, new EventArgs());
                            goto IL_3E8;
                        case Keys.W:
                            break;
                        case Keys.X:
                            GMCut_Click(this, new EventArgs());
                            goto IL_3E8;
                        default:
                            if (keyCode2 == Keys.F10)
                            {
                                ProjectConfig_Click(this, new EventArgs());
                                goto IL_3E8;
                            }
                            if (keyCode2 == Keys.F11)
                            {
                                MSysConfig_Click(this, new EventArgs());
                                goto IL_3E8;
                            }
                            break;
                    }
                }
                e.Handled = false;
            IL_3E8:
                MainDesigner.Focus();
            }
        }

        public void MoveMainDesigner(Size Scroll)
        {
            State state = Global.state;
            state.MapPoint.X += Scroll.Width;
            State state2 = Global.state;
            state2.MapPoint.Y += Scroll.Height;
            Global.state.AdjustMapPoint();
            Global.MainWnd.CommitScrollbar();
            MainDesigner.Refresh();
        }

        private void MainDesigner_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                if (e.KeyCode != Keys.ShiftKey)
                {
                    return;
                }
                if (OldET != GUIDesigner.EditTool.Cursor && MainDesigner.CurrentTool == GUIDesigner.EditTool.Cursor)
                {
                    MainDesigner.CurrentTool = OldET;
                    OldET = GUIDesigner.EditTool.Cursor;
                }
            }
        }

        public void GuiCut_Click(object sender, EventArgs e)
        {
            if (GuiCut.Checked)
            {
                GuiCopy.Checked = false;
                GuiPaste.Checked = false;
                MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Cut;
                return;
            }
            CopyPasteInit();
        }

        public void GuiCopy_Click(object sender, EventArgs e)
        {
            if (GuiCopy.Checked)
            {
                GuiCut.Checked = false;
                GuiPaste.Checked = false;
                MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Copy;
                return;
            }
            CopyPasteInit();
        }

        public void GuiPaste_Click(object sender, EventArgs e)
        {
            if (!GuiPaste.Checked)
            {
                CopyPasteInit();
                return;
            }
            MainDesigner.ClipedString = Clipboard.GetText();
            if (MainDesigner.CheckBuffer())
            {
                GuiCopy.Checked = false;
                GuiCut.Checked = false;
                MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Paste;
                return;
            }
            CopyPasteInit();
            UpdateStatus("クリップテキストが不正です");
        }

        public void CopyPasteInit()
        {
            GuiCopy.Checked = false;
            GuiCut.Checked = false;
            GuiPaste.Checked = false;
            MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.None;
            MainDesigner.Refresh();
        }

        private void GUIProjConfig_Click(object sender, EventArgs e)
        {
            ProjectConfig_Click(this, new EventArgs());
        }

        private void DrawType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DrawType.SelectedIndex == 3)
            {
                ChipList.Visible = false;
                GuiChipList.Visible = true;
                GuiChipList.ResizeInvoke();
                GuiChipList.Refresh();
                return;
            }
            ChipList.Visible = true;
            GuiChipList.Visible = false;
            ChipList.DrawMode = DrawMode.OwnerDrawFixed;
            ChipList.DrawMode = DrawMode.OwnerDrawVariable;
            ChipList.Refresh();
        }

        private void ChipList_DrawItem(object sender, DrawItemEventArgs e)
        {
            int i = e.Index;
            if (i == -1)
            {
                return;
            }
            e.DrawBackground();
            try
            {
                bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
                ChipData cschip;
                ChipsData[] array = Global.cpd.Mapchip;
                using Brush brush = new SolidBrush(e.ForeColor);
                int width = 0;
                GraphicsState transState;
                Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
                if (Global.state.MapEditMode)
                {
                    cschip = Global.cpd.Worldchip[i].GetCSChip();
                }
                else
                {
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        array = [.. array, .. Global.cpd.VarietyChip];
                        if (Global.cpd.CustomPartsChip != null)
                        {
                            array = [.. array, .. Global.cpd.CustomPartsChip];
                        }
                    }
                    cschip = array[i].GetCSChip();
                }

                switch (DrawType.SelectedIndex)
                {
                    case 0: // サムネイル
                        if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                        {
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                            transState = e.Graphics.Save();
                            e.Graphics.TranslateTransform(e.Bounds.X, e.Bounds.Y);
                            if (Global.state.EditingForeground && oriboss_view && array[i].character == "Z")
                            {
                                if (MainDesigner.DrawOribossOrig != null) e.Graphics.DrawImage(MainDesigner.DrawOribossOrig, 0, 0, e.Bounds.Height, e.Bounds.Height);
                            }
                            else if (ChipRenderer.IsAthleticChip(cschip.name))
                            {
                                AthleticView.list[cschip.name].Small(this, cschip, e.Graphics, chipsize, e.Bounds.Height);
                            }
                            else
                            {
                                e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);
                                if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);
                                // 水の半透明処理
                                if (ChipRenderer.ShouldApplyWaterTransparency(out float waterLevel) && array[i].character == "4")
                                {
                                    ChipRenderer.ApplyWaterTransparency(e.Graphics, MainDesigner.DrawChipOrig,
                                        new Rectangle(-e.Bounds.Height / 2, -e.Bounds.Height / 2, e.Bounds.Height, e.Bounds.Height),
                                        cschip.pattern, chipsize, waterLevel);
                                }
                                else e.Graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(-e.Bounds.Height / 2, -e.Bounds.Height / 2, e.Bounds.Height, e.Bounds.Height), new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                            }
                            e.Graphics.Restore(transState);
                        }
                        else
                        {
                            cschip = Global.cpd.Layerchip[i].GetCSChip();
                            e.Graphics.DrawImage(MainDesigner.DrawLayerOrig[Global.state.EdittingLayerIndex], new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                        }
                        width = e.Bounds.Height;
                        break;
                    case 2: // チップ
                        if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                        {
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                            transState = e.Graphics.Save();

                            e.Graphics.TranslateTransform(e.Bounds.X, e.Bounds.Y);
                            if (cschip.size == default)
                            {
                                if (ChipRenderer.IsAthleticChip(cschip.name))
                                {
                                    AthleticView.list[cschip.name].Large(cschip, e.Graphics, chipsize);
                                }
                                else
                                {
                                    e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
                                    e.Graphics.RotateTransform(cschip.rotate);
                                    // 水の半透明処理
                                    if (ChipRenderer.ShouldApplyWaterTransparency(out float waterLevel) && array[i].character == "4")
                                    {
                                        ChipRenderer.ApplyWaterTransparency(e.Graphics, MainDesigner.DrawChipOrig,
                                            new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize),
                                            cschip.pattern, chipsize, waterLevel);
                                    }
                                    else e.Graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                                }
                                width = chipsize.Width;
                            }
                            else
                            {
                                if (Global.state.EditingForeground && oriboss_view && array[i].character == "Z")
                                {
                                    if (MainDesigner.DrawOribossOrig != null)
                                    {
                                        e.Graphics.DrawImage(MainDesigner.DrawOribossOrig, 0, 0);
                                        width = MainDesigner.DrawOribossOrig.Width;
                                    }
                                    else
                                    {
                                        width = cschip.size.Width;
                                    }
                                }
                                else
                                {
                                    int rotate_o = default;
                                    if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
                                    {
                                        rotate_o = (cschip.size.Width - cschip.size.Height) / (cschip.size.Width / cschip.size.Height) * Math.Sign(cschip.rotate);
                                        width = cschip.size.Height;
                                    }
                                    else
                                    {
                                        width = cschip.size.Width;
                                    }
                                    e.Graphics.TranslateTransform(cschip.size.Width / 2, cschip.size.Height / 2);
                                    if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);
                                    e.Graphics.DrawImage(MainDesigner.DrawChipOrig,
                                        new Rectangle(new Point(-cschip.size.Width / 2 + rotate_o, -cschip.size.Height / 2 + rotate_o), cschip.size),
                                        new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
                                }
                            }

                            e.Graphics.Restore(transState);
                        }
                        else
                        {
                            cschip = Global.cpd.Layerchip[i].GetCSChip();
                            if (cschip.size == default)
                            {
                                e.Graphics.DrawImage(MainDesigner.DrawLayerOrig[Global.state.EdittingLayerIndex], new Rectangle(e.Bounds.Location, chipsize), new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                                width = chipsize.Width;
                            }
                            else
                            {
                                e.Graphics.DrawImage(MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, cschip.size), new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
                                width = cschip.size.Width;
                            }
                        }
                        break;
                }
                e.Graphics.PixelOffsetMode = default;
                e.Graphics.DrawString(((ListBox)sender).Items[i].ToString(), e.Font, brush, new Rectangle(e.Bounds.X + width, e.Bounds.Y, e.Bounds.Width - width, e.Bounds.Height));

                if (!ChipList.Enabled) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(160, Color.White)), e.Bounds); // 無効時に白くする
            }
            catch
            {
            }
            e.DrawFocusRectangle();
        }

        private void ChipList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            int i = e.Index;
            int chip_hight = Global.cpd.runtime.Definitions.ChipSize.Height;

            switch (DrawType.SelectedIndex)
            {
                case 0: // サムネイル
                case 1: // テキスト
                    e.ItemHeight = ChipList.ItemHeight;
                    return;
                case 2: // チップ
                    int height;
                    if (Global.state.MapEditMode)
                    {
                        if (Global.cpd.Worldchip[i].GetCSChip().size.Height == 0)
                        {
                            height = chip_hight;
                        }
                        else
                        {
                            height = Global.cpd.Worldchip[i].GetCSChip().size.Height;
                        }
                        e.ItemHeight = height;
                        return;
                    }
                    if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                    {
                        ChipData cschip;
                        ChipsData[] array = Global.cpd.Mapchip;
                        if (Global.cpd.project.Use3rdMapData)
                        {
                            array = [.. array, .. Global.cpd.VarietyChip];
                            if (Global.cpd.CustomPartsChip != null)
                            {
                                array = [.. array, .. Global.cpd.CustomPartsChip];
                            }
                        }
                        cschip = array[i].GetCSChip();
                        if (cschip.size.Height == 0)
                        {
                            height = chip_hight;
                        }
                        else
                        {
                            if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
                            {
                                height = cschip.size.Width;
                            }
                            else
                            {
                                if (Global.state.ChipRegister.TryGetValue("oriboss_v", out string value) && int.Parse(value) == 3 && array[i].character == "Z" && MainDesigner.DrawOribossOrig != null)
                                {
                                    height = MainDesigner.DrawOribossOrig.Height;
                                }
                                else height = cschip.size.Height;
                            }
                        }
                        e.ItemHeight = height;
                        return;
                    }
                    if (Global.cpd.Layerchip[i].GetCSChip().size.Height == 0)
                    {
                        height = chip_hight;
                    }
                    else
                    {
                        height = Global.cpd.Layerchip[i].GetCSChip().size.Height;
                    }
                    e.ItemHeight = height;
                    return;
            }
        }

        private void TMUndo_Click(object sender, EventArgs e)
        {
            MainEditor.Undo();
        }

        private void TMRedo_Click(object sender, EventArgs e)
        {
            MainEditor.Redo();
        }

        private void TextCut_Click(object sender, EventArgs e)
        {
            MainEditor.StageTextEditor.Cut();
        }

        private void TextCopy_Click(object sender, EventArgs e)
        {
            MainEditor.StageTextEditor.Copy();
        }

        private void TextPaste_Click(object sender, EventArgs e)
        {
            MainEditor.StageTextEditor.Paste();
        }

        private void ProjTestRun(object sender, EventArgs e)
        {
            EditTab.SelectedIndex = 2;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (EditTab.SelectedIndex == 0)
            {
                MainDesigner.Refresh();
            }
            if (Global.state.Testrun != null && Global.config.testRun.KillTestrunOnFocus)
            {
                if (!Global.state.Testrun.HasExited && !Global.state.Testrun.CloseMainWindow())
                {
                    try
                    {
                        Global.state.Testrun.Kill();
                    }
                    catch
                    {
                        MessageBox.Show($"ブラウザプロセスを終了できませんでした。{Environment.NewLine}ブラウザを終了する設定をオフにします。", "終了失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Global.config.testRun.KillTestrunOnFocus = false;
                    }
                }
                Global.state.Testrun.Close();
                Global.state.Testrun.Dispose();
            }
            Global.state.Testrun = null;
        }

        private void MainDesigner_Enter(object sender, EventArgs e)
        {
            if (OldET != GUIDesigner.EditTool.Cursor && MainDesigner.CurrentTool == GUIDesigner.EditTool.Cursor)
            {
                MainDesigner.CurrentTool = OldET;
                OldET = GUIDesigner.EditTool.Cursor;
            }
        }

        private void RestartUp(string su)
        {
            Restartupping = true;
            MEditStage1_Click(this, new EventArgs());
            EditPatternChip_Click(this, new EventArgs());
            Global.state.EditingForeground = true;
            EditTab.SelectedIndex = 0;
            SideTab.SelectedIndex = 0;
            ChipList.Enabled = true;
            GuiChipList.Enabled = true;
            if (Global.cpd.project.Use3rdMapData) GuiCustomPartsChipList.Enabled = true;
            MSave.Enabled = true;
            MSaveAs.Enabled = true;
            MTSave.Enabled = true;
            Global.state.EditFlag = false;
            using (ProjectLoading projectLoading = new(su))
            {
                if (projectLoading.ShowDialog() == DialogResult.Abort)
                {
                    Close();
                    Application.Restart();
                    return;
                }
            }
            MainDesigner.ClearBuffer();
            MainDesigner.AddBuffer();
            EditorSystemPanel_Resize(this, new EventArgs());
            Global.state.StageSizeChanged = true;
            MainDesigner.ForceBufferResize();
            UpdateLayer();
            UpdateScrollbar();
            LayerState(CurrentProjectData.UseLayer);
            UpdateStageSelector();
            Restartupping = false;
        }

        private void LayerState(bool enabled)
        {
            ViewUnactiveLayer.Enabled = enabled;
            LayerMenu.Visible = enabled;
            StageLayer.Visible = enabled;
            MainEditor.StageLayer.Visible = enabled;
        }

        private void MNew_Click(object sender, EventArgs e)
        {
            if (Global.state.EditFlag && MessageBox.Show($"データが保存されていません。{Environment.NewLine}保存していないデータは失われます。{Environment.NewLine}続行してよろしいですか？", "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }
            using NewProject newProject = new();
            if (newProject.ShowDialog() == DialogResult.OK)
            {
                RestartUp(newProject.CreatedProject);
            }
        }

        private void MOpen_Click(object sender, EventArgs e)
        {
            if (Global.state.EditFlag && MessageBox.Show($"データが保存されていません。{Environment.NewLine}保存していないデータは失われます。{Environment.NewLine}続行してよろしいですか？", "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = $"{Global.definition.AppName} プロジェクト (*{Global.definition.ProjExt})|*{Global.definition.ProjExt}|全てのファイル|*.*";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                RestartUp(openFileDialog.FileName);
            }
        }

        private void MSave_Click(object sender, EventArgs e)
        {
            Global.cpd.project.SaveXML(Global.cpd.filename);
            Global.state.EditFlag = false;
            UpdateStatus("保存しました");
        }

        private void MSaveAs_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = $"{Global.definition.AppName} プロジェクト (*{Global.definition.ProjExt})|*{Global.definition.ProjExt}|全てのファイル|*.*";
            saveFileDialog.InitialDirectory = Global.cpd.where;
            saveFileDialog.FileName = Path.GetFileName(Global.cpd.filename);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DialogResult dialogResult = MessageBox.Show($"保存先のディレクトリに元のディレクトリのファイルをコピーしますか？{Environment.NewLine}移動しないとテスト実行に失敗したり、編集に失敗したりする恐れがあります。", "ファイルのコピー", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (string text in Directory.GetFiles(Global.cpd.where, "*", SearchOption.TopDirectoryOnly))
                    {
                        string text2 = Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(text));
                        if (!File.Exists(text2) || MessageBox.Show($"{text2}{Environment.NewLine}はすでに存在しています。{Environment.NewLine}上書きしてもよろしいですか？", "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
                        {
                            File.Copy(text, text2, true);
                        }
                    }
                    File.Delete(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(Global.cpd.filename)));
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }
                Global.cpd.where = Path.GetDirectoryName(saveFileDialog.FileName);
                Global.cpd.filename = saveFileDialog.FileName;
            }
            MSave_Click(this, new EventArgs());
        }

        private void MWriteHTML_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.DefaultExt = Global.cpd.runtime.DefaultConfigurations.FileExt;
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = $"出力ファイル(*.{Global.cpd.runtime.DefaultConfigurations.FileExt})|*{Global.cpd.runtime.DefaultConfigurations.FileExt}";
            if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                if (Path.GetDirectoryName(saveFileDialog.FileName) != Global.cpd.where)
                {
                    using OutputControl outputControl = new(Path.GetDirectoryName(saveFileDialog.FileName));
                    if (outputControl.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                using (StreamWriter streamWriter = new(saveFileDialog.FileName, false, Global.config.localSystem.FileEncoding))
                {
                    string value = Subsystem.MakeHTMLCode(0);
                    streamWriter.Write(value);
                    streamWriter.Close();
                }
                MessageBox.Show("出力しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void MWriteJSON_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "JSONファイル(*.json)|*.json|全てのファイル(*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                if (Path.GetDirectoryName(saveFileDialog.FileName) != Global.cpd.where)
                {
                    using OutputControl outputControl = new(Path.GetDirectoryName(saveFileDialog.FileName));
                    if (outputControl.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                using (StreamWriter streamWriter = new(saveFileDialog.FileName, false, new UTF8Encoding(false)))
                {
                    streamWriter.Write(Subsystem.MakeHTMLCode(0, true));
                    streamWriter.Close();
                }
                MessageBox.Show("出力しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void MExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && Global.state.EditFlag && MessageBox.Show($"編集データが保存されていません。{Environment.NewLine}終了してもよろしいですか？", $"{Global.definition.AppName}の終了", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void ViewUnactiveLayer_Click(object sender, EventArgs e)
        {
            Global.state.DrawUnactiveLayer = !Global.state.DrawUnactiveLayer;
            ViewUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
            MDUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
            if (!ViewUnactiveLayer.Checked)
            {
                if (Global.state.TransparentUnactiveLayer)
                {
                    TransparentUnactiveLayer.Enabled = false;
                    if (Global.state.EditingForeground)
                    {
                        MainDesigner.UpdateBackgroundBuffer();
                    }
                    else
                    {
                        MainDesigner.UpdateForegroundBuffer();
                    }
                }
                MTUnactiveLayer.Enabled = false;
            }
            else
            {
                MainDesigner.InitTransparent();
                TransparentUnactiveLayer.Enabled = true;
                MTUnactiveLayer.Enabled = true;
            }
            MainDesigner.Refresh();
        }

        private void TransparentUnactiveLayer_Click(object sender, EventArgs e)
        {
            UpdateStatus("描画モードを切り替えています...");
            Global.state.TransparentUnactiveLayer = !Global.state.TransparentUnactiveLayer;
            TransparentUnactiveLayer.Checked = Global.state.TransparentUnactiveLayer;
            MTUnactiveLayer.Checked = Global.state.TransparentUnactiveLayer;
            if (TransparentUnactiveLayer.Checked)
            {
                MainDesigner.InitTransparent();
            }
            else if (Global.state.EditingForeground)
            {
                MainDesigner.UpdateBackgroundBuffer();
            }
            else
            {
                MainDesigner.UpdateForegroundBuffer();
            }
            MainDesigner.Refresh();
            UpdateStatus("完了");
        }

        public void EditPatternChip_Click(object sender, EventArgs e)
        {
            if (Global.state.EditingForeground)
            {
                return;
            }
            if (!ChangePreCheck())
            {
                return;
            }
            Global.state.EditingForeground = true;
            EditPatternChip.Checked = true;
            PatternChipLayer.Checked = true;
            MainEditor.PatternChipLayer.Checked = true;
            EditBackground.Checked = false;
            BackgroundLayer.Checked = false;
            for (int i = 0; i < LayerMenuCount.Count; i++)
            {
                LayerMenuCount[i].Checked = false;
                LayerCount[i].Checked = false;
                MainEditor.LayerCount[i].Checked = false;
            }
            MainEditor.BackgroundLayer.Checked = false;
            Global.state.EdittingLayerIndex = -1;
            UpdateLayer();
        }

        public void EditBackground_Click(object sender, EventArgs e)
        {
            if (!Global.state.EditingForeground)
            {
                return;
            }
            if (!ChangePreCheck())
            {
                return;
            }
            Global.state.EditingForeground = false;
            EditPatternChip.Checked = false;
            PatternChipLayer.Checked = false;
            MainEditor.PatternChipLayer.Checked = false;
            EditBackground.Checked = true;
            BackgroundLayer.Checked = true;
            MainEditor.BackgroundLayer.Checked = true;
            Global.state.EdittingLayerIndex = 0;
            UpdateLayer();
        }

        private bool ChangePreCheck()
        {
            return EditTab.SelectedIndex != 1 || MainEditor.CanConvertTextSource() || MessageBox.Show($"ステージのテキストが規定の形式を満たしていないため、テキストを反映できません。{Environment.NewLine}レイヤーを切り替えると、編集結果は失われます。{Environment.NewLine}続行してもよろしいですか？", "コンバート失敗", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.Cancel;
        }

        private void StageSelectionChange(int newValue)
        {
            if (Global.state.EdittingStage == newValue)
            {
                return;
            }
            UpdateStatus("ステージを切り替えています...");
            MEditStage1.Checked = false;
            MEditStage2.Checked = false;
            MEditStage3.Checked = false;
            MEditStage4.Checked = false;
            MEditMap.Checked = false;
            Global.state.MapEditMode = false;
            switch (newValue)
            {
                case 0:
                    SetStageData(Global.cpd.project.StageData, 
                        Global.cpd.project.LayerData?.FirstOrDefault(), 
                        Global.cpd.project.Config.Background, MEditStage1, CurrentProjectData.UseLayer);
                    break;
                case 1:
                    SetStageData(Global.cpd.project.StageData2, 
                        Global.cpd.project.LayerData2?.FirstOrDefault(), 
                        Global.cpd.project.Config.Background2, MEditStage2, CurrentProjectData.UseLayer);
                    break;
                case 2:
                    SetStageData(Global.cpd.project.StageData3, 
                        Global.cpd.project.LayerData3?.FirstOrDefault(), 
                        Global.cpd.project.Config.Background3, MEditStage3, CurrentProjectData.UseLayer);
                    break;
                case 3:
                    SetStageData(Global.cpd.project.StageData4, 
                        Global.cpd.project.LayerData4?.FirstOrDefault(), 
                        Global.cpd.project.Config.Background4, MEditStage4, CurrentProjectData.UseLayer);
                    break;
                case 4:
                    SetStageData(Global.cpd.project.MapData, null, Global.cpd.project.Config.BackgroundM, MEditMap, false);
                    Global.state.MapEditMode = true;
                    Global.state.EditingForeground = true;
                    break;
            }
            Global.state.EdittingStage = newValue;
            UpdateLayerVisibility();
            MainDesigner.ForceBufferResize();
            MainDesigner.PrepareImages();
            if (!Global.state.EditingForeground)
            {
                EditBackground.Checked = true;
                BackgroundLayer.Checked = true;
                MainEditor.BackgroundLayer.Checked = true;
                LayerCount[0].Checked = true;
                MainEditor.LayerCount[0].Checked = true;
                LayerMenuCount[0].Checked = true;
                Global.state.EdittingLayerIndex = 0;
                
            }
            UpdateLayer();
            UpdateScrollbar();
        }

        /// <summary>
        /// ステージデータ、レイヤーデータ、背景色を設定し、対応するメニュー項目をチェックしてレイヤー状態を更新します
        /// </summary>
        /// <param name="stageData">ステージデータ</param>
        /// <param name="layerData">レイヤーデータ</param>
        /// <param name="backgroundColor">背景色</param>
        /// <param name="menuItem">チェックするメニュー項目</param>
        /// <param name="enableLayer">レイヤー機能を有効にするかどうか</param>
        private void SetStageData(LayerObject stageData, LayerObject layerData, Color backgroundColor, ToolStripMenuItem menuItem, bool enableLayer)
        {
            Global.cpd.EditingMap = stageData;
            if (layerData != null) Global.cpd.EditingLayer = layerData;
            menuItem.Checked = true;
            Global.state.Background = backgroundColor;
            LayerState(enableLayer);
        }

        private void UpdateStageSelector()
        {
            MEditStage2.Enabled = (Global.cpd.project.Config.StageNum >= 2) || Global.cpd.project.Config.UseWorldmap;
            MEditStage3.Enabled = (Global.cpd.project.Config.StageNum >= 3) || Global.cpd.project.Config.UseWorldmap;
            MEditStage4.Enabled = (Global.cpd.project.Config.StageNum >= 4) || Global.cpd.project.Config.UseWorldmap;
            MEditMap.Enabled = Global.cpd.project.Config.UseWorldmap;
        }

        public void UpdateLayer(int layerIndex = -1)
        {
            if (EditTab.SelectedIndex == 0)
            {
                UpdateStatus("描画を更新しています...");
                if (Global.state.EditingForeground)
                {
                    Global.state.CurrentChip = Global.cpd.Mapchip[0];
                }
                else
                {
                    Global.state.CurrentChip = Global.cpd.Layerchip[0];
                }
                ChipItemReadyInvoke();
                MainDesigner.ClearBuffer();
                MainDesigner.UpdateForegroundBuffer();
                MainDesigner.UpdateBackgroundBuffer(layerIndex);
                if (Global.state.TransparentUnactiveLayer)
                {
                    MainDesigner.InitTransparent();
                }
                MainDesigner.AddBuffer();
                MainDesigner.Refresh();
                UpdateStatus("完了");
                return;
            }
            if (EditTab.SelectedIndex == 1)
            {
                LayerObject clonedText = [.. (string[])MainEditor.StageTextEditor.Lines.Clone()];
                if (Global.state.EditingForeground)
                {
                    Global.cpd.EditingLayer = clonedText;
                    MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingMap);
                    MainEditor.BufferClear();
                    MainEditor.AddBuffer();
                    return;
                }
                Global.cpd.EditingMap = clonedText;
                MainEditor.StageTextEditor.Text = JoinLayerToCloneableString(Global.cpd.EditingLayer);
                MainEditor.BufferClear();
                MainEditor.AddBuffer();
            }
        }

        private void DrawType_Resize(object sender, EventArgs e)
        {
            DrawType.Refresh();
        }

        public void MSysConfig_Click(object sender, EventArgs e)
        {
            using SideConfig sideConfig = new();
            if (sideConfig.ShowDialog() == DialogResult.OK && EditTab.SelectedIndex == 0)
            {
                UpdateStatus("描画を更新しています...");
                GuiChipList.Refresh();
                if (Global.cpd.project.Use3rdMapData) GuiCustomPartsChipList.Refresh();
                MainDesigner.UpdateBackgroundBuffer();
                MainDesigner.UpdateForegroundBuffer();
                MainDesigner.InitTransparent();
                MainDesigner.Refresh();
                MasaoConfigList.Reload();
                UpdateStatus("完了");
            }
        }

        private void MVersion_Click(object sender, EventArgs e)
        {
            using VersionInfo versionInfo = new();
            versionInfo.ShowDialog();
        }

        private void SideTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SideTab.SelectedIndex == 1)
            {
                //MasaoConfigList.Prepare();
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                Global.config.lastData.SpliterDist = MainSplit.SplitterDistance / (double)Width;
                Rectangle normalWindowLocation = Native.GetNormalWindowLocation(this);
                Global.config.lastData.WndSize = new Size(normalWindowLocation.Size.Width * 96 / DeviceDpi, normalWindowLocation.Size.Height * 96 / DeviceDpi);
                Global.config.lastData.WndPoint = new Point(normalWindowLocation.Location.X * 96 / DeviceDpi, normalWindowLocation.Location.Y * 96 / DeviceDpi);
                Global.config.lastData.WndState = WindowState;
                Global.config.SaveXML(Path.Combine(Application.StartupPath, Global.definition.ConfigFile));
                if (Global.state.RunFile != null)
                {
                    try
                    {
                        Process.Start(Global.state.RunFile);
                    }
                    catch
                    {
                        MessageBox.Show($"アップデートの実行ができませんでした。{Environment.NewLine}手動更新してください。", "更新失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                Keys keyCode = e.KeyCode;
                if (keyCode != Keys.ShiftKey)
                {
                    if (keyCode != Keys.Space)
                    {
                        return;
                    }
                    if (e.Shift)
                    {
                        AllTestrun(this, new EventArgs());
                    }
                    else
                    {
                        ProjTestRun(this, new EventArgs());
                    }
                    e.Handled = true;
                    return;
                }
                else
                {
                    if (!Global.config.localSystem.ShiftFocusShift)
                    {
                        return;
                    }
                    if (EditTab.SelectedIndex == 0)
                    {
                        if (SideTab.SelectedIndex != 0)
                        {
                            return;
                        }
                        if (MainDesigner.Focused)
                        {
                            if (DrawType.SelectedIndex < 3)
                            {
                                ChipList.Focus();
                            }
                            else
                            {
                                GuiChipList.Focus();
                            }
                        }
                        else
                        {
                            MainDesigner.Focus();
                        }
                        e.Handled = true;
                    }
                    else if (EditTab.SelectedIndex == 2 && Global.cpd.project.Use3rdMapData)
                    {
                        if (SideTab.SelectedIndex != 0)
                        {
                            return;
                        }
                        if (MainDesigner.Focused)
                        {
                            GuiCustomPartsChipList.Focus();
                        }
                        else
                        {
                            MainDesigner.Focus();
                        }
                        e.Handled = true;
                    }
                }
            }
        }

        public void ProjectConfig_Click(object sender, EventArgs e)
        {
            using (ProjectConfig projectConfig = new())
            {
                projectConfig.ShowDialog();
            }
            UpdateStageSelector();
        }

        private void MWriteStagePicture_Click(object sender, EventArgs e)
        {
            string text = "";
            using (SaveFileDialog saveFileDialog = new())
            {
                saveFileDialog.Filter = "PNG画像(*.png)|*.png|GIF画像(*.gif)|*.gif|JPEG画像(*.jpg)|*.jpg|ビットマップ(*.bmp)|*.bmp";
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.FileName = "無題";
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                text = saveFileDialog.FileName;
            }
            Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : GUIDesigner.CurrentStageSize;
            using (Bitmap bitmap = new(stageSizeData.x * Global.cpd.runtime.Definitions.ChipSize.Width, stageSizeData.y * Global.cpd.runtime.Definitions.ChipSize.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Brush brush = new SolidBrush(Global.state.Background))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                    }
                    Global.MainWnd.MainDesigner.PaintStage(graphics, MessageBox.Show("拡張描画画像も一緒に書き込みますか？", "拡張描画の選択", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                }
                string a;
                if ((a = Path.GetExtension(text).ToLower()) != null)
                {
                    if (a == ".jpg")
                    {
                        bitmap.Save(text, ImageFormat.Jpeg);
                        goto IL_1DF;
                    }
                    if (a == ".bmp")
                    {
                        bitmap.Save(text, ImageFormat.Bmp);
                        goto IL_1DF;
                    }
                    if (a == ".png")
                    {
                        bitmap.Save(text, ImageFormat.Png);
                        goto IL_1DF;
                    }
                    if (a == ".gif")
                    {
                        bitmap.Save(text, ImageFormat.Gif);
                        goto IL_1DF;
                    }
                }
                bitmap.Save(text, ImageFormat.Bmp);
            }
        IL_1DF:
            MessageBox.Show("保存しました。", "ステージ画像の保存", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void MResetProjRuntime_Click(object sender, EventArgs e)
        {
            using (ResetRuntime resetRuntime = new())
            {
                if (resetRuntime.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }
            MainDesigner.ClearBuffer();
            MainDesigner.AddBuffer();
            EditorSystemPanel_Resize(this, new EventArgs());
            LayerState(CurrentProjectData.UseLayer);
        }

        private void InstalledRuntime_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Retry;
            while (dialogResult == DialogResult.Retry)
            {
                using RuntimeManager runtimeManager = new();
                dialogResult = runtimeManager.ShowDialog();
            }
        }

        private void MProjectInheritNew_Click(object sender, EventArgs e)
        {
            if (Global.state.EditFlag && MessageBox.Show($"データが保存されていません。{Environment.NewLine}保存していないデータは失われます。{Environment.NewLine}続行してよろしいですか？", "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = $"{Global.definition.AppName} プロジェクト (*{Global.definition.ProjExt})|*{Global.definition.ProjExt}";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using ProjInheritance projInheritance = new(openFileDialog.FileName);
                if (projInheritance.ShowDialog() == DialogResult.OK)
                {
                    RestartUp(projInheritance.NewProjectName);
                }
            }
        }

        private void MConvertHTML_Click(object sender, EventArgs e)
        {
            if (Global.state.EditFlag && MessageBox.Show($"データが保存されていません。{Environment.NewLine}保存していないデータは失われます。{Environment.NewLine}続行してよろしいですか？", "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "HTML/XML/JS/JSON ドキュメント(*.htm*;*.xml;*.js;*.json)|*.htm*;*.xml;*.js;*.json|全てのファイル|*.*";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using HTMLInheritance htmlinheritance = new(openFileDialog.FileName);
                if (htmlinheritance.ShowDialog() == DialogResult.OK)
                {
                    RestartUp(htmlinheritance.ProjectFile);
                }
            }
        }

        private void MUpdateApp_Click(object sender, EventArgs e)
        {
            using WebUpdate webUpdate = new();
            if (webUpdate.ShowDialog() == DialogResult.Retry)
            {
                Global.state.RunFile = (string)webUpdate.runfile.Clone();
                Close();
            }
        }

        private void MStageRev_Click(object sender, EventArgs e)
        {
            UpdateStatus("ステージ反転処理中...");
            List<string> list;
            Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : GUIDesigner.CurrentStageSize;
            for (int i = 0; i < stageSizeData.y; i++)
            {
                list = [];
                string text = Global.cpd.EditingMap[i];

                if (Global.cpd.project.Use3rdMapData)
                {
                    var stagearray = text.Split(',');
                    for (int j = 0; j < stageSizeData.x; j++)
                    {
                        list.Add(stagearray[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < stageSizeData.x; j++)
                    {
                        list.Add(text.Substring(j * stageSizeData.bytesize, stageSizeData.bytesize));
                    }
                }
                string[] array = [.. list];
                Array.Reverse(array);
                if (Global.cpd.project.Use3rdMapData)
                {
                    Global.cpd.EditingMap[i] = string.Join(",", array);
                }
                else
                {
                    Global.cpd.EditingMap[i] = string.Join("", array);
                }
            }
            if (CurrentProjectData.UseLayer)
            {
                stageSizeData = GUIDesigner.CurrentLayerSize;
                for (int k = 0; k < stageSizeData.y; k++)
                {
                    list = [];
                    string text2 = Global.cpd.EditingLayer[k];
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        var stagearray = text2.Split(',');
                        for (int l = 0; l < stageSizeData.x; l++)
                        {
                            list.Add(stagearray[l]);
                        }
                    }
                    else
                    {
                        for (int l = 0; l < stageSizeData.x; l++)
                        {
                            list.Add(text2.Substring(l * stageSizeData.bytesize, stageSizeData.bytesize));
                        }
                    }
                    string[] array2 = [.. list];
                    Array.Reverse(array2);
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        Global.cpd.EditingLayer[k] = string.Join(",", array2);
                    }
                    else
                    {
                        Global.cpd.EditingLayer[k] = string.Join("", array2);
                    }
                }
            }
            MainDesigner.ClearBuffer();
            MainDesigner.AddBuffer();
            MainDesigner.UpdateBackgroundBuffer();
            MainDesigner.UpdateForegroundBuffer();
            if (Global.state.TransparentUnactiveLayer)
            {
                MainDesigner.InitTransparent();
            }
            MainDesigner.Refresh();
            Global.state.EditFlag = true;
            UpdateStatus("完了");
        }

        public void MEditStage1_Click(object sender, EventArgs e)
        {
            StageSelectionChange(0);
        }

        private void MEditStage2_Click(object sender, EventArgs e)
        {
            StageSelectionChange(1);
        }

        private void MEditStage3_Click(object sender, EventArgs e)
        {
            StageSelectionChange(2);
        }

        private void MEditStage4_Click(object sender, EventArgs e)
        {
            StageSelectionChange(3);
        }

        private void MEditMap_Click(object sender, EventArgs e)
        {
            StageSelectionChange(4);
        }

        private void AllTestrun(object sender, EventArgs e)
        {
            Global.state.TestrunAll = true;
            EditTab.SelectedIndex = 2;
        }

        private void MReloadImage_Click(object sender, EventArgs e)
        {
            UpdateStatus("更新しています...");
            Global.MainWnd.MainDesigner.PrepareImages();
            Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
            Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
            MainDesigner.Refresh();
            UpdateStatus("完了");
        }

        private void ShowOverView_Click(object sender, EventArgs e)
        {
            if (ovw == null)
            {
                ovw = new OverViewWindow();
                ovw.FormClosed += ovw_FormClosed;
                ovw.Show(this);
                ShowOverView.Checked = true;
                return;
            }
            ovw.Close();
            ovw = null;
            ShowOverView.Checked = false;
        }

        private void BMTestEnd_Click(object sender, EventArgs e)
        {
            EditTab.SelectedIndex = 0;
        }

        private void ovw_FormClosed(object sender, FormClosedEventArgs e)
        {
            ovw.Dispose();
            ovw = null;
            ShowOverView.Checked = false;
        }

        public OverViewWindow ovw;

        private bool StageConvert = true;

        private GUIDesigner.EditTool OldET;

        private bool Restartupping;

        public delegate void MainDesignerScrollInvoke();

        private delegate void CRID();
    }
}

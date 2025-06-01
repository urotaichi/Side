using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
    public class LayerObjectConfigList : ConfigList
    {
        private int dragRowIndex = -1;
        private Rectangle dragBoxFromMouseDown;
        private int dropTargetRowIndex = -1;
        private bool showDropLine = false;
        private int dropLineY = -1;

        public override void Prepare()
        {
            ConfigSelector.Items.Clear();
            for (int i = 0; i < Global.cpd.project.Config.StageNum; i++)
            {
                ConfigSelector.Items.Add($"ステージ{i + 1}のレイヤー設定");
            }
            ConfigSelector.SelectedIndex = Global.state.EdittingStage;
        }

        protected override void ConfigSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigSelector.Items.Count < 1)
            {
                return;
            }
            ConfView.Rows.Clear();

            PopulateLayerObjectRows(ConfigSelector.SelectedIndex);

            ApplyWrapModeIfEnabled();
        }

        public void PopulateLayerObjectRows(int selectedIndex)
        {
            var (stageData, layerData) = GetStageAndLayerData(selectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(selectedIndex);
            
            if (stageData == null || layerData == null || layerSize == null)
            {
                return;
            }

            // mainOrderを考慮してレイヤー順序を決定
            List<(string name, string source, string rowTag, bool useDefault)> orderedLayers = [];
            
            // 背景レイヤーを順番に追加
            for (int i = 0; i < layerData.Count; i++)
            {
                string name = $"背景レイヤーチップ{i + 1}のファイル名";
                orderedLayers.Add((name, layerData[i].Source, $"layer:{i}", layerData[i].Source == Global.cpd.project.Config.LayerImage));
            }

            // メインレイヤーをmainOrderの位置に挿入
            int mainOrder = Math.Max(0, Math.Min(layerSize.mainOrder, orderedLayers.Count));
            orderedLayers.Insert(mainOrder, ("キャラクターパターンのファイル名", stageData.Source, "stage", stageData.Source == Global.cpd.project.Config.PatternImage));

            // 各レイヤーオブジェクトの設定行を追加
            for (int i = 0; i < orderedLayers.Count; i++)
            {
                var (name, source, rowTag, useDefaultImage) = orderedLayers[i];
                ConfView.Rows.Add([name, source + "..."]);

                DataGridViewButtonCell dataGridViewButtonCell = new()
                {
                    Value = source + "...",
                    FlatStyle = FlatStyle.Popup
                };
                ConfView[1, ConfView.Rows.Count - 1] = dataGridViewButtonCell;
                
                ConfView.Rows[^1].Tag = rowTag;

                DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new()
                {
                    TrueValue = "true",
                    FalseValue = "false",
                    Value = useDefaultImage.ToString()
                };
                ConfView[2, ConfView.Rows.Count - 1] = dataGridViewCheckBoxCell;
            }
        }

        private static (LayerObject stageData, List<LayerObject> layerData) GetStageAndLayerData(int selectedIndex)
        {
            return selectedIndex switch
            {
                0 => (Global.cpd.project.StageData, Global.cpd.project.LayerData),
                1 => (Global.cpd.project.StageData2, Global.cpd.project.LayerData2),
                2 => (Global.cpd.project.StageData3, Global.cpd.project.LayerData3),
                3 => (Global.cpd.project.StageData4, Global.cpd.project.LayerData4),
                _ => (null, null)
            };
        }

        protected override void ConfView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // チェックボックス列のクリック処理
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                var checkBoxCell = (DataGridViewCheckBoxCell)ConfView[2, e.RowIndex];
                bool currentValue = bool.Parse(checkBoxCell.Value?.ToString() ?? "false");
                
                // trueの場合はデフォルト画像の値をランタイム定義に書き込んでfalseに変更
                if (currentValue)
                {
                    HandleSetCustomImage(e.RowIndex);
                    return;
                }

                HandleCheckBoxClick(e.RowIndex);
                return;
            }

            // ファイル選択列のクリック処理
            if (e.ColumnIndex != 1 || e.RowIndex < 0)
            {
                return;
            }

            HandleFileSelection(e);
        }

        private void HandleSetCustomImage(int rowIndex)
        {
            string rowTag = ConfView.Rows[rowIndex].Tag?.ToString() ?? "";
            string defaultFileName = GetDefaultFileName(rowTag);
            
            if (string.IsNullOrEmpty(defaultFileName))
            {
                return;
            }

            // ランタイム定義にデフォルト画像の値を設定
            SetRuntimeDefinitionValue(rowTag, defaultFileName);

            // チェックボックスをfalseに変更
            ConfView[2, rowIndex].Value = "false";

            Global.state.EditFlag = true;
        }

        private void SetRuntimeDefinitionValue(string rowTag, string fileName)
        {
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);

            if (rowTag == "stage")
            {
                if (stageSize?.mainPattern != null)
                {
                    stageSize.mainPattern.Value = fileName;
                }
            }
            else if (rowTag.StartsWith("layer:"))
            {
                if (int.TryParse(rowTag.AsSpan(6), out int layerIndex) && 
                    layerSize?.mapchips != null && layerIndex < layerSize.mapchips.Count)
                {
                    layerSize.mapchips[layerIndex].Value = fileName;
                }
            }
        }

        private void HandleCheckBoxClick(int rowIndex)
        {
            string rowTag = ConfView.Rows[rowIndex].Tag?.ToString() ?? "";
            string defaultFileName = GetDefaultFileName(rowTag);
            
            if (string.IsNullOrEmpty(defaultFileName))
            {
                return;
            }

            SetDefaultImage(rowIndex, rowTag, defaultFileName);
        }

        private static string GetDefaultFileName(string rowTag)
        {
            return rowTag == "stage" 
                ? Global.cpd.project.Config.PatternImage 
                : rowTag.StartsWith("layer:") 
                    ? Global.cpd.project.Config.LayerImage 
                    : "";
        }

        private void SetDefaultImage(int rowIndex, string rowTag, string defaultFileName)
        {
            // 表示を更新
            ConfView[1, rowIndex].Value = defaultFileName + "...";
            ConfView[2, rowIndex].Value = "true";

            // データソースを更新
            UpdateDataSourceAndRefresh(rowTag, defaultFileName);

            // ランタイム定義の値をnullに設定してデフォルト値を使用
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);
            
            if (rowTag == "stage")
            {
                if (stageSize?.mainPattern != null)
                {
                    stageSize.mainPattern.Value = null;
                }
            }
            else if (rowTag.StartsWith("layer:"))
            {
                if (int.TryParse(rowTag.AsSpan(6), out int layerIndex) && 
                    layerSize?.mapchips != null && layerIndex < layerSize.mapchips.Count)
                {
                    layerSize.mapchips[layerIndex].Value = null;
                }
            }

            Global.state.EditFlag = true;
        }

        private void HandleFileSelection(DataGridViewCellEventArgs e)
        {
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            string currentSource = "";
            
            // クリックされた行のタグを確認
            string rowTag = ConfView.Rows[e.RowIndex].Tag?.ToString() ?? "";
            
            if (rowTag == "stage")
            {
                currentSource = stageData?.Source ?? "";
            }
            else if (rowTag.StartsWith("layer:"))
            {
                if (int.TryParse(rowTag.AsSpan(6), out int layerIndex) && 
                    layerData != null && layerIndex < layerData.Count)
                {
                    currentSource = layerData[layerIndex].Source;
                }
            }

            using OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Global.cpd.where;
            openFileDialog.FileName = currentSource; // 現在の値を初期ファイル名に
            openFileDialog.Filter = "画像(*.gif;*.png)|*.gif;*.png|全てのファイル (*.*)|*.*";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = openFileDialog.FileName;
            string dest = Path.Combine(Global.cpd.where, Path.GetFileName(fileName));
            string destFileName = Path.GetFileName(dest);

            if (!TryCopyFileToProject(fileName, ref dest))
            {
                return;
            }

            ConfView[e.ColumnIndex, e.RowIndex].Value = destFileName + "...";


            // データソースを更新
            UpdateDataSourceAndRefresh(rowTag, destFileName);

            Global.state.EditFlag = true;
        }

        private void UpdateDataSourceAndRefresh(string rowTag, string fileName)
        {
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);

            if (rowTag == "stage")
            {
                stageData.Source = fileName;
                stageSize.mainPattern.Value = fileName;
                RefreshDesignerForRelation("PATTERN");
            }
            else if (rowTag.StartsWith("layer:"))
            {
                if (int.TryParse(rowTag.AsSpan(6), out int layerIndex) && 
                    layerData != null && layerIndex < layerData.Count)
                {
                    layerData[layerIndex].Source = fileName;
                    layerSize.mapchips[layerIndex].Value = fileName;
                    RefreshDesignerForRelation("LAYERCHIP");
                }
            }
        }

        private static (Runtime.DefinedData.StageSizeData stageSize, Runtime.DefinedData.LayerSizeData layerSize) GetRuntimeDefinitions(int selectedIndex)
        {
            return selectedIndex switch
            {
                0 => (Global.cpd.runtime.Definitions.StageSize, Global.cpd.runtime.Definitions.LayerSize),
                1 => (Global.cpd.runtime.Definitions.StageSize2, Global.cpd.runtime.Definitions.LayerSize2),
                2 => (Global.cpd.runtime.Definitions.StageSize3, Global.cpd.runtime.Definitions.LayerSize3),
                3 => (Global.cpd.runtime.Definitions.StageSize4, Global.cpd.runtime.Definitions.LayerSize4),
                _ => (null, null)
            };
        }

        private void MoveLayerData(int sourceDisplayIndex, int targetDisplayIndex)
        {
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);

            if (layerData == null || layerSize == null)
            {
                return;
            }

            // 現在の表示順序からソースとターゲットのレイヤー情報を取得
            string sourceRowTag = ConfView.Rows[sourceDisplayIndex].Tag?.ToString() ?? "";

            if (sourceRowTag == "stage" || sourceRowTag.StartsWith("layer:"))
            {
                // ソースがメインレイヤーの場合
                if (sourceRowTag == "stage")
                {
                    // 新しいmainOrderを設定
                    layerSize.mainOrder = targetDisplayIndex;
                    RefreshDesignerForRelation("PATTERN");
                }
                else if (sourceRowTag.StartsWith("layer:"))
                {
                    // 背景レイヤーの移動
                    if (int.TryParse(sourceRowTag.AsSpan(6), out int sourceLayerIndex) &&
                        sourceLayerIndex < layerData.Count)
                    {
                        // 移動するレイヤーを取得
                        var sourceLayerItem = layerData[sourceLayerIndex];
                        var sourceMapchipItem = sourceLayerIndex < layerSize.mapchips.Count ? layerSize.mapchips[sourceLayerIndex] : null;

                        // ソースレイヤーを削除
                        layerData.RemoveAt(sourceLayerIndex);
                        if (sourceLayerIndex < layerSize.mapchips.Count)
                        {
                            layerSize.mapchips.RemoveAt(sourceLayerIndex);
                        }
                        // ターゲット位置を計算（メインレイヤーの位置を考慮)
                        int newLayerIndex = CalculateNewLayerIndex(targetDisplayIndex, layerSize.mainOrder);

                        // 新しい位置に挿入
                        if (newLayerIndex >= layerData.Count)
                        {
                            layerData.Add(sourceLayerItem);
                        }
                        else
                        {
                            layerData.Insert(newLayerIndex, sourceLayerItem);
                        }
                        
                        if (sourceMapchipItem != null)
                        {
                            if (newLayerIndex >= layerSize.mapchips.Count)
                            {
                                layerSize.mapchips.Add(sourceMapchipItem);
                            }
                            else
                            {
                                layerSize.mapchips.Insert(newLayerIndex, sourceMapchipItem);
                            }
                        }

                        // mainOrderの調整
                        AdjustMainOrderAfterLayerMove(sourceDisplayIndex, targetDisplayIndex, layerSize);
                        
                        RefreshDesignerForRelation("LAYERCHIP");
                    }
                }

                Global.state.EditFlag = true;
            }
        }

        private static int CalculateNewLayerIndex(int targetDisplayIndex, int mainOrder)
        {
            // 表示インデックスからレイヤーインデックスに変換
            if (targetDisplayIndex < mainOrder)
            {
                return targetDisplayIndex;
            }
            else
            {
                return targetDisplayIndex - 1;
            }
        }

        private static void AdjustMainOrderAfterLayerMove(int sourceDisplayIndex, int targetDisplayIndex, Runtime.DefinedData.LayerSizeData layerSize)
        {
            // 表示位置ベースでmainOrderを調整
            if (sourceDisplayIndex < layerSize.mainOrder && targetDisplayIndex >= layerSize.mainOrder)
            {
                // メインレイヤーより前から後への移動
                layerSize.mainOrder--;
            }
            else if (sourceDisplayIndex > layerSize.mainOrder && targetDisplayIndex <= layerSize.mainOrder)
            {
                // メインレイヤーより後から前への移動
                layerSize.mainOrder++;
            }
        }

        private void ConfView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hitTest = ConfView.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0) // 全ての行が移動可能
                {
                    dragRowIndex = hitTest.RowIndex;
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxFromMouseDown = new Rectangle(
                        new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)),
                        dragSize);
                }
                else
                {
                    dragRowIndex = -1;
                    dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }

        private void ConfView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dragRowIndex >= 0)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && 
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    ConfView.DoDragDrop(dragRowIndex, DragDropEffects.Move);
                }
            }
        }

        private void ConfView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            
            Point clientPoint = ConfView.PointToClient(new Point(e.X, e.Y));
            var hitTest = ConfView.HitTest(clientPoint.X, clientPoint.Y);
            
            UpdateDropVisualFeedback(clientPoint, hitTest);
        }

        private void UpdateDropVisualFeedback(Point clientPoint, DataGridView.HitTestInfo hitTest)
        {
            int previousDropTarget = dropTargetRowIndex;
            int previousDropLineY = dropLineY;
            
            dropTargetRowIndex = -1;
            showDropLine = false;
            dropLineY = -1;

            if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0) // 全ての行が対象
            {
                Rectangle cellRect = ConfView.GetCellDisplayRectangle(0, hitTest.RowIndex, false);
                int cellMiddleY = cellRect.Y + cellRect.Height / 2;

                if (clientPoint.Y < cellMiddleY)
                {
                    // セルの上半分 - その行の上にドロップライン
                    showDropLine = true;
                    dropLineY = cellRect.Y;
                    dropTargetRowIndex = hitTest.RowIndex;
                }
                else
                {
                    // セルの下半分 - その行の下にドロップライン
                    showDropLine = true;
                    dropLineY = cellRect.Y + cellRect.Height;
                    dropTargetRowIndex = hitTest.RowIndex + 1;
                }
            }
            else if (hitTest.Type == DataGridViewHitTestType.None && ConfView.Rows.Count > 0)
            {
                // グリッド外 - 最後の行の下にドロップライン
                Rectangle lastCellRect = ConfView.GetCellDisplayRectangle(0, ConfView.Rows.Count - 1, false);
                showDropLine = true;
                dropLineY = lastCellRect.Y + lastCellRect.Height;
                dropTargetRowIndex = ConfView.Rows.Count;
            }

            // 描画更新が必要な場合のみ再描画
            if (previousDropTarget != dropTargetRowIndex || previousDropLineY != dropLineY)
            {
                ConfView.Invalidate();
            }
        }

        private void ConfView_DragLeave(object sender, EventArgs e)
        {
            ClearDropVisualFeedback();
        }

        private void ConfView_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = ConfView.PointToClient(new Point(e.X, e.Y));
            var hitTest = ConfView.HitTest(clientPoint.X, clientPoint.Y);

            int targetDisplayIndex = GetDropTargetDisplayIndex(clientPoint, hitTest, dragRowIndex);
            
            if (targetDisplayIndex >= 0 && dragRowIndex >= 0)
            {
                // 移動前後のインデックスが同じ場合は何もしない
                if (dragRowIndex == targetDisplayIndex)
                {
                    ClearDropVisualFeedback();
                    dragRowIndex = -1;
                    dragBoxFromMouseDown = Rectangle.Empty;
                    return;
                }

                // データソースでの移動
                MoveLayerData(dragRowIndex, targetDisplayIndex);

                // 表示の更新（タグ情報も含めて再構築）
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);

                Global.MainWnd.UpdateLayerVisibility();

                // 編集中のレイヤーが移動した場合の処理
                int newEditingIndex = GetNewEditingLayerIndex(dragRowIndex, targetDisplayIndex);
                if(Global.state.EdittingLayerIndex == dragRowIndex && newEditingIndex >= 0) 
                {
                    Global.MainWnd.LayerCount_Click(newEditingIndex);
                }
            }

            ClearDropVisualFeedback();
            dragRowIndex = -1;
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void ClearDropVisualFeedback()
        {
            if (dropTargetRowIndex >= 0 || showDropLine)
            {
                dropTargetRowIndex = -1;
                showDropLine = false;
                dropLineY = -1;
                ConfView.Invalidate();
            }
        }

        private void ConfView_Paint(object sender, PaintEventArgs e)
        {
            // ドロップライン描画
            if (showDropLine && dropLineY >= 0)
            {
                using Pen dropLinePen = new(Color.Blue, 2);
                // dropLineYはGetCellDisplayRectangleで取得した座標なのでそのまま使用
                e.Graphics.DrawLine(dropLinePen, 0, dropLineY, ConfView.Width, dropLineY);
            }
        }

        private int GetDropTargetDisplayIndex(Point clientPoint, DataGridView.HitTestInfo hitTest, int sourceRowIndex)
        {
            // 行の境界でのドロップを検出
            if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
            {
                Rectangle cellRect = ConfView.GetCellDisplayRectangle(0, hitTest.RowIndex, false);
                int cellMiddleY = cellRect.Y + cellRect.Height / 2;

                int targetIndex;
                if (clientPoint.Y < cellMiddleY)
                {
                    targetIndex = hitTest.RowIndex;
                }
                else
                {
                    targetIndex = hitTest.RowIndex + 1;
                }

                // 移動前の行より後に挿入する場合は、移動前の行が削除されることを考慮
                if (targetIndex > sourceRowIndex)
                {
                    targetIndex--;
                }

                return targetIndex;
            }
            else if (hitTest.Type == DataGridViewHitTestType.None || hitTest.RowIndex < 0)
            {
                // グリッド外での判定
                if (ConfView.Rows.Count > 0)
                {
                    Rectangle firstCellRect = ConfView.GetCellDisplayRectangle(0, 0, false);
                    Rectangle lastCellRect = ConfView.GetCellDisplayRectangle(0, ConfView.Rows.Count - 1, false);
                    
                    if (clientPoint.Y < firstCellRect.Y)
                    {
                        // グリッドの上部 - 先頭に挿入
                        return 0;
                    }
                    else if (clientPoint.Y > lastCellRect.Y + lastCellRect.Height)
                    {
                        // グリッドの下部 - 最後に挿入
                        int targetIndex = ConfView.Rows.Count;
                        // 移動前の行より後に挿入する場合は、移動前の行が削除されることを考慮
                        if (targetIndex > sourceRowIndex)
                        {
                            targetIndex--;
                        }
                        return targetIndex;
                    }
                }
                
                // その他の場合は最後に挿入
                int defaultTargetIndex = ConfView.Rows.Count;
                if (defaultTargetIndex > sourceRowIndex)
                {
                    defaultTargetIndex--;
                }
                return defaultTargetIndex;
            }

            return -1;
        }

        private static int GetNewEditingLayerIndex(int sourceIndex, int targetIndex)
        {
            if (targetIndex > sourceIndex)
            {
                return targetIndex - 1;
            }
            else
            {
                return targetIndex;
            }
        }

        protected override void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ConfigSelector = new ComboBox();
            ConfView = new DataGridView();
            CNames = new DataGridViewTextBoxColumn();
            CValues = new DataGridViewTextBoxColumn();
            UseDefault = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)ConfView).BeginInit();
            SuspendLayout();

            ConfigSelector.Dock = DockStyle.Top;
            ConfigSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigSelector.FlatStyle = FlatStyle.System;
            ConfigSelector.FormattingEnabled = true;
            ConfigSelector.Location = new Point(0, 0);
            ConfigSelector.Name = "ConfigSelector";
            ConfigSelector.Size = LogicalToDeviceUnits(new Size(298, 20));
            ConfigSelector.TabIndex = 3;
            ConfigSelector.Resize += ConfigSelector_Resize;
            ConfigSelector.SelectedIndexChanged += ConfigSelector_SelectedIndexChanged;

            //先頭行
            ConfView.AllowUserToAddRows = false;
            ConfView.AllowUserToDeleteRows = false;
            ConfView.AllowUserToResizeRows = false;
            ConfView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConfView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            ConfView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ConfView.Columns.AddRange(
            [
                CNames,
                CValues,
                UseDefault
            ]);
            ConfView.Dock = DockStyle.Fill;
            ConfView.EditMode = DataGridViewEditMode.EditOnEnter;
            ConfView.Location = new Point(0, LogicalToDeviceUnits(20));
            ConfView.MultiSelect = false;
            ConfView.Name = "LayerConfView";
            ConfView.RowHeadersVisible = false;
            ConfView.RowTemplate.Height = 21;
            ConfView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ConfView.Size = LogicalToDeviceUnits(new Size(298, 338));
            ConfView.TabIndex = 4;
            ConfView.AllowDrop = true;
            ConfView.CellValueChanged += ConfView_CellValueChanged;
            ConfView.CellClick += ConfView_CellClick;
            ConfView.EditingControlShowing += ConfView_EditingControlShowing;
            ConfView.CurrentCellDirtyStateChanged += ConfView_CurrentCellDirtyStateChanged;
            ConfView.CellContentClick += ConfView_CellContentClick;
            ConfView.MouseDown += ConfView_MouseDown;
            ConfView.MouseMove += ConfView_MouseMove;
            ConfView.DragOver += ConfView_DragOver;
            ConfView.DragDrop += ConfView_DragDrop;
            ConfView.DragLeave += ConfView_DragLeave;
            ConfView.Paint += ConfView_Paint;

            CNames.HeaderText = "レイヤー";
            CNames.Name = "CNames";
            CNames.ReadOnly = true;
            CNames.SortMode = DataGridViewColumnSortMode.NotSortable;
            CNames.FillWeight = 100f;
            CValues.HeaderText = "画像ファイル";
            CValues.Name = "CValues";
            CValues.SortMode = DataGridViewColumnSortMode.NotSortable;
            CValues.FillWeight = 70f;
            UseDefault.HeaderText = "デフォルトの画像を使う";
            UseDefault.Name = "UseDefault";
            UseDefault.SortMode = DataGridViewColumnSortMode.NotSortable;
            UseDefault.ReadOnly = true;
            // AutoScaleDimensions = new SizeF(6f, 12f);
            // AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ConfView);
            Controls.Add(ConfigSelector);
            Name = "LayerConfigList";
            Size = LogicalToDeviceUnits(new Size(298, 358));
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);
        }

        private DataGridViewTextBoxColumn UseDefault;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Controls
{
    public class LayerObjectConfigList : ConfigList
    {
        private int dragRowIndex = -1;
        private Rectangle dragBoxFromMouseDown;
        private int dropTargetRowIndex = -1;
        private bool showDropLine = false;
        private int dropLineY = -1;
        private Panel buttonPanel;
        private Button btnAddLayer;
        private Button btnDeleteLayer;
        private Button btnDuplicateLayer;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem menuAddLayer;
        private ToolStripMenuItem menuDeleteLayer;
        private ToolStripMenuItem menuDuplicateLayer;
        private ToolStripMenuItem menuMoveUp;
        private ToolStripMenuItem menuMoveDown;
        private int rightClickedRowIndex = -1;

        public override void Prepare()
        {
            // 初回のみ項目追加
            if (ConfigSelector.Items.Count == 0)
            {
                for (int i = 0; i < 4; i++)  // 最大4ステージ分
                {
                    ConfigSelector.Items.Add($"ステージ{i + 1}のレイヤー設定");
                }
            }

            ConfigSelector.SelectedIndex = Global.state.EdittingStage;

            UpdateControlStates();
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
            UpdateButtonStates();
        }

        public void UpdateControlStates()
        {
            bool enable3rdMapData = Global.cpd.project.Use3rdMapData;
            buttonPanel.Enabled = enable3rdMapData;
            contextMenuStrip.Enabled = enable3rdMapData;
            ConfView.AllowDrop = enable3rdMapData;

            // セルの有効/無効状態を設定
            if (!enable3rdMapData)
            {
                foreach (DataGridViewRow row in ConfView.Rows)
                {
                    // ボタンセルを無効化
                    if (row.Cells[1] is DataGridViewButtonCell buttonCell)
                    {
                        buttonCell.FlatStyle = FlatStyle.Standard;
                        row.Cells[1].ReadOnly = true;
                        row.Cells[1].Style.BackColor = SystemColors.Control;
                        row.Cells[1].Style.ForeColor = SystemColors.GrayText;
                    }
                    // チェックボックスセルを無効化
                    if (row.Cells[2] is DataGridViewCheckBoxCell checkBoxCell)
                    {
                        row.Cells[2].ReadOnly = true;
                        row.Cells[2].Style.BackColor = SystemColors.Control;
                        row.Cells[2].Style.ForeColor = SystemColors.GrayText;
                    }
                }
            }
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
            if (!Global.cpd.project.Use3rdMapData)
            {
                return;
            }

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
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                HandleFileSelection(e);
            }
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
                return Math.Max(0, targetDisplayIndex);
            }
            else
            {
                return Math.Max(0, targetDisplayIndex - 1);
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
            if (!Global.cpd.project.Use3rdMapData)
            {
                return;
            }

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
            else if (e.Button == MouseButtons.Right)
            {
                var hitTest = ConfView.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
                {
                    rightClickedRowIndex = hitTest.RowIndex;
                    UpdateContextMenuStates();
                    contextMenuStrip.Show(ConfView, e.Location);
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
            else if (hitTest.Type == DataGridViewHitTestType.None || hitTest.RowIndex < 0)
            {
                // グリッド外での判定
                if (ConfView.Rows.Count > 0)
                {
                    Rectangle firstCellRect = ConfView.GetCellDisplayRectangle(0, 0, false);
                    Rectangle lastCellRect = ConfView.GetCellDisplayRectangle(0, ConfView.Rows.Count - 1, false);
                    
                    if (clientPoint.Y < firstCellRect.Y)
                    {
                        // グリッド上方向 - 最初の行の上にドロップライン
                        showDropLine = true;
                        dropLineY = firstCellRect.Y;
                        dropTargetRowIndex = 0;
                    }
                    else if (clientPoint.Y > lastCellRect.Y + lastCellRect.Height)
                    {
                        // グリッド下方向 - 最後の行の下にドロップライン
                        showDropLine = true;
                        dropLineY = lastCellRect.Y + lastCellRect.Height;
                        dropTargetRowIndex = ConfView.Rows.Count;
                    }
                    else
                    {
                        // その他の場合は最後に挿入
                        showDropLine = true;
                        dropLineY = lastCellRect.Y + lastCellRect.Height;
                        dropTargetRowIndex = ConfView.Rows.Count;
                    }
                }
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

                ProcessLayerMove(dragRowIndex, targetDisplayIndex);
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

        private void ButtonPanel_CreateLayer_Click(object sender, EventArgs e)
        {
            CreateNewLayer(-1); // 末尾に追加
        }

        private void CreateNewLayer(int insertAfterRowIndex)
        {
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);
            
            if (layerData == null || layerSize == null)
            {
                return;
            }

            // 新しいレイヤーオブジェクトを作成
            var newLayer = new LayerObject
            {
                Source = Global.cpd.project.Config.LayerImage,
                Strings = new string[layerSize.y]
            };

            // Stringsを初期化
            string fillValue = Global.cpd.Layerchip[0].code.ToString();
            for (int i = 0; i < newLayer.Length; i++)
            {
                string[] array = new string[layerSize.x];
                for (int j = 0; j < layerSize.x; j++)
                {
                    array[j] = fillValue;
                }
                newLayer[i] = string.Join(",", array);
            }
            
            var newMapchip = new Runtime.DefinedData.StageSizeData.LayerObject
            {
                Value = null
            };

            int insertIndex;
            int newRowIndex;
            bool adjustMainOrder = false;

            if (insertAfterRowIndex == -1)
            {
                // 末尾に追加（ボタンから呼ばれた場合）
                insertIndex = layerData.Count;
                newRowIndex = ConfView.Rows.Count - 1;
            }
            else
            {
                // 指定された行の直下に追加（右クリックメニューから呼ばれた場合）
                string rowTag = ConfView.Rows[insertAfterRowIndex].Tag?.ToString() ?? "";
                
                if (rowTag == "stage")
                {
                    // メインレイヤーの直下に挿入
                    insertIndex = CalculateNewLayerIndex(insertAfterRowIndex + 1, layerSize.mainOrder);
                    adjustMainOrder = insertAfterRowIndex < layerSize.mainOrder;
                }
                else if (rowTag.StartsWith("layer:") && int.TryParse(rowTag.AsSpan(6), out int layerIndex))
                {
                    // 右クリックされた背景レイヤーの直後に挿入
                    insertIndex = layerIndex + 1;
                    adjustMainOrder = insertAfterRowIndex < layerSize.mainOrder;
                }
                else
                {
                    // 末尾に追加
                    insertIndex = layerData.Count;
                }
                
                newRowIndex = insertAfterRowIndex + 1;
            }

            // レイヤーを挿入
            if (insertIndex >= layerData.Count)
            {
                layerData.Add(newLayer);
                layerSize.mapchips.Add(newMapchip);
            }
            else
            {
                layerData.Insert(insertIndex, newLayer);
                layerSize.mapchips.Insert(insertIndex, newMapchip);
            }

            // mainOrderの調整
            if (adjustMainOrder)
            {
                layerSize.mainOrder++;
            }

            // 表示を更新
            ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
            
            // 新しく追加された行を選択
            if (insertAfterRowIndex == -1)
            {
                // 末尾に追加された場合
                if (ConfView.Rows.Count > 0)
                {
                    newRowIndex = ConfView.Rows.Count - 1;
                    ConfView.ClearSelection();
                    ConfView.Rows[newRowIndex].Selected = true;
                    ConfView.CurrentCell = ConfView[0, newRowIndex];
                }
            }
            else
            {
                // 指定位置に追加された場合
                if (newRowIndex < ConfView.Rows.Count)
                {
                    ConfView.ClearSelection();
                    ConfView.Rows[newRowIndex].Selected = true;
                    ConfView.CurrentCell = ConfView[0, newRowIndex];
                }
            }

            Global.MainWnd.MainDesigner.BackLayerBmp.Add(null);
            RefreshDesignerForRelation("LAYERCHIP");
            Global.MainWnd.RebuildLayerMenus();
            Global.state.EditFlag = true;
        }

        private void ButtonPanel_DeleteLayer_Click(object sender, EventArgs e)
        {
            if (ConfView.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedRowIndex = ConfView.SelectedRows[0].Index;
            DeleteLayer(selectedRowIndex);
        }

        private void DeleteLayer(int rowIndex)
        {
            string rowTag = ConfView.Rows[rowIndex].Tag?.ToString() ?? "";
            
            if (!rowTag.StartsWith("layer:"))
            {
                return;
            }

            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);
            
            if (layerData == null || layerSize == null)
            {
                return;
            }

            if (int.TryParse(rowTag.AsSpan(6), out int layerIndex) && layerIndex < layerData.Count)
            {
                // 編集中のレイヤーが削除される場合の処理
                bool isDeletingCurrentEditingLayer = layerIndex == Global.state.EdittingLayerIndex;
                
                // レイヤーを削除
                layerData.RemoveAt(layerIndex);
                if (layerIndex < layerSize.mapchips.Count)
                {
                    layerSize.mapchips.RemoveAt(layerIndex);
                }
                Global.MainWnd.MainDesigner.BackLayerBmp.RemoveAt(layerIndex);

                // mainOrderの調整
                if (rowIndex < layerSize.mainOrder)
                {
                    layerSize.mainOrder--;
                }

                // 表示を更新
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
                
                // 編集中のレイヤーが削除された場合の処理
                if (isDeletingCurrentEditingLayer)
                {
                    int newEditingDisplayIndex = Math.Max(0, rowIndex - 1);
                    
                    if (newEditingDisplayIndex < layerSize.mainOrder)
                    {
                        Global.MainWnd.LayerCount_Click(newEditingDisplayIndex);
                    }
                    else if (newEditingDisplayIndex == layerSize.mainOrder)
                    {
                        Global.MainWnd.EditPatternChip_Click(this, new EventArgs());
                    }
                    else
                    {
                        int backgroundLayerIndex = newEditingDisplayIndex - 1;
                        Global.MainWnd.LayerCount_Click(backgroundLayerIndex);
                    }
                }
                
                // 選択状態を調整
                if (ConfView.Rows.Count > 0)
                {
                    int newSelectionIndex = Math.Min(rowIndex, ConfView.Rows.Count - 1);
                    ConfView.ClearSelection();
                    ConfView.Rows[newSelectionIndex].Selected = true;
                    ConfView.CurrentCell = ConfView[0, newSelectionIndex];
                }

                RefreshDesignerForRelation("LAYERCHIP");
                Global.MainWnd.RebuildLayerMenus();
                Global.state.EditFlag = true;
            }
        }

        private void ButtonPanel_DuplicateLayer_Click(object sender, EventArgs e)
        {
            if (ConfView.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedRowIndex = ConfView.SelectedRows[0].Index;
            DuplicateLayer(selectedRowIndex);
        }

        private void DuplicateLayer(int rowIndex)
        {
            string rowTag = ConfView.Rows[rowIndex].Tag?.ToString() ?? "";
            
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);
            
            if (layerData == null || layerSize == null)
            {
                return;
            }

            if (rowTag.StartsWith("layer:") && int.TryParse(rowTag.AsSpan(6), out int layerIndex) && layerIndex < layerData.Count)
            {
                // 背景レイヤーの複製
                var sourceLayer = layerData[layerIndex];
                var sourceMapchip = layerIndex < layerSize.mapchips.Count ? layerSize.mapchips[layerIndex] : null;

                var newLayer = new LayerObject
                {
                    Source = sourceLayer.Source,
                    Strings = new string[sourceLayer.Length]
                };

                // Stringsを完全にコピー
                for (int i = 0; i < sourceLayer.Length; i++)
                {
                    newLayer[i] = sourceLayer[i];
                }
                
                var newMapchip = new Runtime.DefinedData.StageSizeData.LayerObject
                {
                    Value = sourceMapchip?.Value
                };

                // 元のレイヤーの直後に挿入
                if (layerIndex + 1 >= layerData.Count)
                {
                    layerData.Add(newLayer);
                    layerSize.mapchips.Add(newMapchip);
                }
                else
                {
                    layerData.Insert(layerIndex + 1, newLayer);
                    layerSize.mapchips.Insert(layerIndex + 1, newMapchip);
                }

                // mainOrderの調整（複製されたレイヤーがメインレイヤーより上にある場合）
                if (rowIndex < layerSize.mainOrder)
                {
                    layerSize.mainOrder++;
                }

                // 表示を更新
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
                
                // 複製された行を選択
                if (ConfView.Rows.Count > rowIndex + 1)
                {
                    ConfView.ClearSelection();
                    ConfView.Rows[rowIndex + 1].Selected = true;
                    ConfView.CurrentCell = ConfView[0, rowIndex + 1];
                }

                Global.MainWnd.MainDesigner.BackLayerBmp.Add(null);
                RefreshDesignerForRelation("LAYERCHIP");
                Global.MainWnd.RebuildLayerMenus();
                Global.state.EditFlag = true;
            }
        }

        private void ButtonPanel_MoveUp_Click(object sender, EventArgs e)
        {
            if (ConfView.SelectedRows.Count == 0 || ConfView.SelectedRows[0].Index == 0)
            {
                return;
            }

            MoveSelectedLayer(-1);
        }

        private void ButtonPanel_MoveDown_Click(object sender, EventArgs e)
        {
            if (ConfView.SelectedRows.Count == 0 || ConfView.SelectedRows[0].Index >= ConfView.Rows.Count - 1)
            {
                return;
            }

            MoveSelectedLayer(1);
        }

        private void MoveSelectedLayer(int direction)
        {
            int selectedRowIndex = ConfView.SelectedRows[0].Index;
            int targetIndex = selectedRowIndex + direction;
            
            ProcessLayerMove(selectedRowIndex, targetIndex);
        }

        private void ProcessLayerMove(int sourceDisplayIndex, int targetDisplayIndex)
        {
            // 移動元の背景レイヤーインデックスを取得
            string sourceRowTag = ConfView.Rows[sourceDisplayIndex].Tag?.ToString() ?? "";
            if (sourceRowTag.StartsWith("layer:") && int.TryParse(sourceRowTag.AsSpan(6), out int sourceLayerIndex))
            {
                // 編集中のレイヤーが移動される場合の処理
                bool isMovingCurrentEditingLayer = sourceLayerIndex == Global.state.EdittingLayerIndex;
                
                MoveLayerData(sourceDisplayIndex, targetDisplayIndex);
                
                // 表示の更新
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
                
                // 編集中のレイヤーが移動した場合、新しいレイヤーインデックスを設定
                if (isMovingCurrentEditingLayer)
                {
                    // 移動後の新しいレイヤーインデックスを計算
                    var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);
                    int newLayerIndex = CalculateNewLayerIndex(targetDisplayIndex, layerSize.mainOrder);
                    Global.MainWnd.LayerCount_Click(newLayerIndex);
                }
                
                // 移動後の行を選択
                ConfView.ClearSelection();
                ConfView.Rows[targetDisplayIndex].Selected = true;
                ConfView.CurrentCell = ConfView[0, targetDisplayIndex];
            }
            else
            {
                // メインレイヤーの移動の場合は従来通り
                MoveLayerData(sourceDisplayIndex, targetDisplayIndex);
                
                // 表示の更新
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
                
                // 移動後の行を選択
                ConfView.ClearSelection();
                ConfView.Rows[targetDisplayIndex].Selected = true;
                ConfView.CurrentCell = ConfView[0, targetDisplayIndex];
            }

            Global.MainWnd.UpdateLayerVisibility();
            Global.state.EditFlag = true;
        }

        private void UpdateButtonStates()
        {
            if (ConfView.SelectedRows.Count == 0)
            {
                btnDeleteLayer.Enabled = false;
                btnDuplicateLayer.Enabled = false;
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
                return;
            }

            int selectedRowIndex = ConfView.SelectedRows[0].Index;
            string rowTag = ConfView.Rows[selectedRowIndex].Tag?.ToString() ?? "";
            
            // 上下移動ボタンの状態制御
            btnMoveUp.Enabled = selectedRowIndex > 0;
            btnMoveDown.Enabled = selectedRowIndex < ConfView.Rows.Count - 1;
            
            // メインレイヤーが選択されている場合
            if (rowTag == "stage")
            {
                btnDeleteLayer.Enabled = false;
                btnDuplicateLayer.Enabled = false;
            }
            else if (rowTag.StartsWith("layer:"))
            {
                var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
                
                // 背景レイヤーが1個しかない場合は削除不可
                btnDeleteLayer.Enabled = layerData != null && layerData.Count > 1;
                btnDuplicateLayer.Enabled = true;
            }
            else
            {
                btnDeleteLayer.Enabled = false;
                btnDuplicateLayer.Enabled = false;
            }
        }

        private void ConfView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateContextMenuStates()
        {
            if (!Global.cpd.project.Use3rdMapData || rightClickedRowIndex < 0 || rightClickedRowIndex >= ConfView.Rows.Count)
            {
                menuAddLayer.Enabled = false;
                menuDeleteLayer.Enabled = false;
                menuDuplicateLayer.Enabled = false;
                menuMoveUp.Enabled = false;
                menuMoveDown.Enabled = false;
                return;
            }

            string rowTag = ConfView.Rows[rightClickedRowIndex].Tag?.ToString() ?? "";
            
            // 上下移動メニューの状態制御
            menuMoveUp.Enabled = rightClickedRowIndex > 0;
            menuMoveDown.Enabled = rightClickedRowIndex < ConfView.Rows.Count - 1;
            
            // メインレイヤーが右クリックされた場合
            if (rowTag == "stage")
            {
                menuAddLayer.Enabled = true; // メインレイヤーの直下に背景レイヤーを追加可能
                menuDeleteLayer.Enabled = false;
                menuDuplicateLayer.Enabled = false;
            }
            else if (rowTag.StartsWith("layer:"))
            {
                var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
                
                menuAddLayer.Enabled = true;
                // 背景レイヤーが1個しかない場合は削除不可
                menuDeleteLayer.Enabled = layerData != null && layerData.Count > 1;
                menuDuplicateLayer.Enabled = true;
            }
            else
            {
                menuAddLayer.Enabled = false;
                menuDeleteLayer.Enabled = false;
                menuDuplicateLayer.Enabled = false;
            }
        }

        private void MenuAddLayer_Click(object sender, EventArgs e)
        {
            if (rightClickedRowIndex < 0 || rightClickedRowIndex >= ConfView.Rows.Count)
            {
                return;
            }

            CreateNewLayer(rightClickedRowIndex); // 右クリックされた行の直下に追加
        }

        private void MenuDeleteLayer_Click(object sender, EventArgs e)
        {
            if (rightClickedRowIndex < 0 || rightClickedRowIndex >= ConfView.Rows.Count)
            {
                return;
            }

            DeleteLayer(rightClickedRowIndex);
        }

        private void MenuDuplicateLayer_Click(object sender, EventArgs e)
        {
            if (rightClickedRowIndex < 0 || rightClickedRowIndex >= ConfView.Rows.Count)
            {
                return;
            }

            DuplicateLayer(rightClickedRowIndex);
        }

        private void MenuMoveUp_Click(object sender, EventArgs e)
        {
            if (rightClickedRowIndex <= 0 || rightClickedRowIndex >= ConfView.Rows.Count)
            {
                return;
            }

            ProcessLayerMove(rightClickedRowIndex, rightClickedRowIndex - 1);
        }

        private void MenuMoveDown_Click(object sender, EventArgs e)
        {
            if (rightClickedRowIndex < 0 || rightClickedRowIndex >= ConfView.Rows.Count - 1)
            {
                return;
            }

            ProcessLayerMove(rightClickedRowIndex, rightClickedRowIndex + 1);
        }

        protected override void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ConfigSelector = new ComboBox();
            buttonPanel = new Panel();
            btnAddLayer = new Button();
            btnDeleteLayer = new Button();
            btnDuplicateLayer = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            ConfView = new DataGridView();
            CNames = new DataGridViewTextBoxColumn();
            CValues = new DataGridViewTextBoxColumn();
            UseDefault = new DataGridViewTextBoxColumn();
            contextMenuStrip = new ContextMenuStrip();
            menuAddLayer = new ToolStripMenuItem();
            menuDeleteLayer = new ToolStripMenuItem();
            menuDuplicateLayer = new ToolStripMenuItem();
            menuMoveUp = new ToolStripMenuItem();
            menuMoveDown = new ToolStripMenuItem();
            ((ISupportInitialize)ConfView).BeginInit();
            contextMenuStrip.SuspendLayout();
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

            // ボタンパネルの設定
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = LogicalToDeviceUnits(30);
            buttonPanel.Padding = new Padding(2);

            // ボタンの設定
            int buttonWidth = LogicalToDeviceUnits(70);
            int buttonHeight = LogicalToDeviceUnits(24);
            int buttonSpacing = LogicalToDeviceUnits(2);

            btnAddLayer.Text = "追加";
            btnAddLayer.Size = new Size(buttonWidth, buttonHeight);
            btnAddLayer.Location = new Point(buttonSpacing, LogicalToDeviceUnits(3));
            btnAddLayer.Image = new IconImageView(DeviceDpi, Resources.shape_square_add).View();
            btnAddLayer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAddLayer.Click += ButtonPanel_CreateLayer_Click;

            btnDeleteLayer.Text = "削除";
            btnDeleteLayer.Size = new Size(buttonWidth, buttonHeight);
            btnDeleteLayer.Location = new Point(buttonWidth + buttonSpacing * 2, LogicalToDeviceUnits(3));
            btnDeleteLayer.Image = new IconImageView(DeviceDpi, Resources.cross).View();
            btnDeleteLayer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDeleteLayer.Click += ButtonPanel_DeleteLayer_Click;

            btnDuplicateLayer.Text = "複製";
            btnDuplicateLayer.Size = new Size(buttonWidth, buttonHeight);
            btnDuplicateLayer.Location = new Point((buttonWidth + buttonSpacing) * 2 + buttonSpacing, LogicalToDeviceUnits(3));
            btnDuplicateLayer.Image = new IconImageView(DeviceDpi, Resources.copy).View();
            btnDuplicateLayer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDuplicateLayer.Click += ButtonPanel_DuplicateLayer_Click;

            btnMoveUp.Text = "";
            btnMoveUp.Size = new Size(LogicalToDeviceUnits(25), buttonHeight);
            btnMoveUp.Location = new Point((buttonWidth + buttonSpacing) * 3 + buttonSpacing, LogicalToDeviceUnits(3));
            btnMoveUp.Image = new IconImageView(DeviceDpi, Resources.arrow_up).View();
            btnMoveUp.Click += ButtonPanel_MoveUp_Click;

            btnMoveDown.Text = "";
            btnMoveDown.Size = new Size(LogicalToDeviceUnits(25), buttonHeight);
            btnMoveDown.Location = new Point((buttonWidth + buttonSpacing) * 3 + LogicalToDeviceUnits(25) + buttonSpacing * 2, LogicalToDeviceUnits(3));
            btnMoveDown.Image = new IconImageView(DeviceDpi, Resources.arrow_down).View();
            btnMoveDown.Click += ButtonPanel_MoveDown_Click;

            buttonPanel.Controls.AddRange([btnAddLayer, btnDeleteLayer, btnDuplicateLayer, btnMoveUp, btnMoveDown]);

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
            ConfView.Location = new Point(0, LogicalToDeviceUnits(50));
            ConfView.MultiSelect = false;
            ConfView.Name = "LayerConfView";
            ConfView.RowHeadersVisible = false;
            ConfView.RowTemplate.Height = 21;
            ConfView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ConfView.Size = LogicalToDeviceUnits(new Size(298, 308));
            ConfView.TabIndex = 4;
            ConfView.AllowDrop = true;
            ConfView.CellValueChanged += ConfView_CellValueChanged;
            ConfView.CellClick += ConfView_CellClick;
            ConfView.EditingControlShowing += ConfView_EditingControlShowing;
            ConfView.CurrentCellDirtyStateChanged += ConfView_CurrentCellDirtyStateChanged;
            ConfView.CellContentClick += ConfView_CellContentClick;
            ConfView.SelectionChanged += ConfView_SelectionChanged;
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
            Controls.Add(buttonPanel);
            Controls.Add(ConfigSelector);
            Name = "LayerConfigList";
            Size = LogicalToDeviceUnits(new Size(298, 358));
            // コンテキストメニューの設定
            contextMenuStrip.Items.AddRange([
                menuAddLayer,
                menuDeleteLayer,
                menuDuplicateLayer,
                new ToolStripSeparator(),
                menuMoveUp,
                menuMoveDown
            ]);
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = LogicalToDeviceUnits(new Size(180, 120));

            menuAddLayer.Name = "menuAddLayer";
            menuAddLayer.Size = LogicalToDeviceUnits(new Size(179, 22));
            menuAddLayer.Text = "レイヤーを追加";
            menuAddLayer.Image = new IconImageView(DeviceDpi, Resources.shape_square_add).View();
            menuAddLayer.Click += MenuAddLayer_Click;

            menuDeleteLayer.Name = "menuDeleteLayer";
            menuDeleteLayer.Size = LogicalToDeviceUnits(new Size(179, 22));
            menuDeleteLayer.Text = "レイヤーを削除";
            menuDeleteLayer.Image = new IconImageView(DeviceDpi, Resources.cross).View();
            menuDeleteLayer.Click += MenuDeleteLayer_Click;

            menuDuplicateLayer.Name = "menuDuplicateLayer";
            menuDuplicateLayer.Size = LogicalToDeviceUnits(new Size(179, 22));
            menuDuplicateLayer.Text = "レイヤーを複製";
            menuDuplicateLayer.Image = new IconImageView(DeviceDpi, Resources.copy).View();
            menuDuplicateLayer.Click += MenuDuplicateLayer_Click;

            menuMoveUp.Name = "menuMoveUp";
            menuMoveUp.Size = LogicalToDeviceUnits(new Size(179, 22));
            menuMoveUp.Text = "上に移動";
            menuMoveUp.Image = new IconImageView(DeviceDpi, Resources.arrow_up).View();
            menuMoveUp.Click += MenuMoveUp_Click;

            menuMoveDown.Name = "menuMoveDown";
            menuMoveDown.Size = LogicalToDeviceUnits(new Size(179, 22));
            menuMoveDown.Text = "下に移動";
            menuMoveDown.Image = new IconImageView(DeviceDpi, Resources.arrow_down).View();
            menuMoveDown.Click += MenuMoveDown_Click;

            // DataGridViewにコンテキストメニューを設定
            ConfView.ContextMenuStrip = contextMenuStrip;

            contextMenuStrip.ResumeLayout(false);
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);
        }

        private DataGridViewTextBoxColumn UseDefault;
    }
}

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
        private Panel buttonPanel;
        private Button btnAddLayer;
        private Button btnDeleteLayer;
        private Button btnDuplicateLayer;
        private Button btnMoveUp;
        private Button btnMoveDown;

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
            UpdateButtonStates();
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

                // 移動後の行を選択状態にする
                int finalSelectionIndex = newEditingIndex;
                
                // 下方向の移動の場合は次の行を選択
                if (targetDisplayIndex > dragRowIndex)
                {
                    finalSelectionIndex = newEditingIndex + 1;
                }
                
                if (finalSelectionIndex >= 0 && finalSelectionIndex < ConfView.Rows.Count)
                {
                    ConfView.ClearSelection();
                    ConfView.Rows[finalSelectionIndex].Selected = true;
                    ConfView.CurrentCell = ConfView[0, finalSelectionIndex];
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

        private void ButtonPanel_CreateLayer_Click(object sender, EventArgs e)
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

            // Stringsを初期化（3rdMapData形式で空のレイヤーデータで埋める）
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
            
            // 新しいマップチップアイテムを作成
            var newMapchip = new Runtime.DefinedData.StageSizeData.LayerObject
            {
                Value = null // デフォルト値を使用
            };

            // 末尾に追加
            layerData.Add(newLayer);
            layerSize.mapchips.Add(newMapchip);

            // 表示を更新
            ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
            
            // 新しく追加された行を選択
            if (ConfView.Rows.Count > 0)
            {
                int newRowIndex = ConfView.Rows.Count - 1;
                ConfView.ClearSelection();
                ConfView.Rows[newRowIndex].Selected = true;
                ConfView.CurrentCell = ConfView[0, newRowIndex];
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
            string rowTag = ConfView.Rows[selectedRowIndex].Tag?.ToString() ?? "";
            
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

                // mainOrderの調整（削除されたレイヤーがメインレイヤーより上にある場合）
                if (selectedRowIndex < layerSize.mainOrder)
                {
                    layerSize.mainOrder--;
                }

                // 表示を更新
                ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
                
                // 編集中のレイヤーが削除された場合、一つ上のレイヤーを編集中にする
                if (isDeletingCurrentEditingLayer)
                {
                    // 表示行を基準に一つ上のレイヤーを特定
                    int newEditingDisplayIndex = Math.Max(0, selectedRowIndex - 1);
                    
                    // 新しい編集レイヤーのインデックスを計算
                    if (newEditingDisplayIndex < layerSize.mainOrder)
                    {
                        // メインレイヤーより上の場合は背景レイヤー
                        Global.MainWnd.LayerCount_Click(newEditingDisplayIndex);
                    }
                    else if (newEditingDisplayIndex == layerSize.mainOrder)
                    {
                        // メインレイヤーの場合はEditPatternChip_Clickを使用
                        Global.MainWnd.EditPatternChip_Click(this, new EventArgs());
                    }
                    else
                    {
                        // メインレイヤーより下の場合は背景レイヤー（mainOrderを考慮）
                        int backgroundLayerIndex = newEditingDisplayIndex - 1;
                        Global.MainWnd.LayerCount_Click(backgroundLayerIndex);
                    }
                }
                
                // 選択状態を調整
                if (ConfView.Rows.Count > 0)
                {
                    int newSelectionIndex = Math.Min(selectedRowIndex, ConfView.Rows.Count - 1);
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
            string rowTag = ConfView.Rows[selectedRowIndex].Tag?.ToString() ?? "";
            
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
                if (selectedRowIndex < layerSize.mainOrder)
                {
                    layerSize.mainOrder++;
                }
            }

            // 表示を更新
            ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
            
            // 複製された行を選択
            if (ConfView.Rows.Count > selectedRowIndex + 1)
            {
                ConfView.ClearSelection();
                ConfView.Rows[selectedRowIndex + 1].Selected = true;
                ConfView.CurrentCell = ConfView[0, selectedRowIndex + 1];
            }

            Global.MainWnd.MainDesigner.BackLayerBmp.Add(null);
            RefreshDesignerForRelation("LAYERCHIP");
            Global.MainWnd.RebuildLayerMenus();
            Global.state.EditFlag = true;
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
            
            MoveLayerData(selectedRowIndex, targetIndex);
            
            // 表示の更新
            ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);
            
            // 移動後の行を選択
            ConfView.ClearSelection();
            ConfView.Rows[targetIndex].Selected = true;
            ConfView.CurrentCell = ConfView[0, targetIndex];

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

            // ボタンパネルの設定
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = LogicalToDeviceUnits(30);
            buttonPanel.Padding = new Padding(2);

            // ボタンの設定
            int buttonWidth = LogicalToDeviceUnits(50);
            int buttonHeight = LogicalToDeviceUnits(24);
            int buttonSpacing = LogicalToDeviceUnits(2);

            btnAddLayer.Text = "追加";
            btnAddLayer.Size = new Size(buttonWidth, buttonHeight);
            btnAddLayer.Location = new Point(buttonSpacing, LogicalToDeviceUnits(3));
            btnAddLayer.Click += ButtonPanel_CreateLayer_Click;

            btnDeleteLayer.Text = "削除";
            btnDeleteLayer.Size = new Size(buttonWidth, buttonHeight);
            btnDeleteLayer.Location = new Point(buttonWidth + buttonSpacing * 2, LogicalToDeviceUnits(3));
            btnDeleteLayer.Click += ButtonPanel_DeleteLayer_Click;

            btnDuplicateLayer.Text = "複製";
            btnDuplicateLayer.Size = new Size(buttonWidth, buttonHeight);
            btnDuplicateLayer.Location = new Point((buttonWidth + buttonSpacing) * 2 + buttonSpacing, LogicalToDeviceUnits(3));
            btnDuplicateLayer.Click += ButtonPanel_DuplicateLayer_Click;

            btnMoveUp.Text = "↑";
            btnMoveUp.Size = new Size(LogicalToDeviceUnits(25), buttonHeight);
            btnMoveUp.Location = new Point((buttonWidth + buttonSpacing) * 3 + buttonSpacing, LogicalToDeviceUnits(3));
            btnMoveUp.Click += ButtonPanel_MoveUp_Click;

            btnMoveDown.Text = "↓";
            btnMoveDown.Size = new Size(LogicalToDeviceUnits(25), buttonHeight);
            btnMoveDown.Location = new Point((buttonWidth + buttonSpacing) * 3 + LogicalToDeviceUnits(25) + buttonSpacing * 2, LogicalToDeviceUnits(3));
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
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);
        }

        private DataGridViewTextBoxColumn UseDefault;
    }
}

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
            List<(string, string)> layerObjects = GetLayerObjectsForStage(selectedIndex);

            // 各レイヤーオブジェクトの設定行を追加
            for (int i = 0; i < layerObjects.Count; i++)
            {
                var (name, defaultValue) = layerObjects[i];
                ConfView.Rows.Add(
                [
                    name,
                    defaultValue + "..."
                ]);

                DataGridViewButtonCell dataGridViewButtonCell = new()
                {
                    Value = defaultValue + "...",
                    FlatStyle = FlatStyle.Popup
                };
                ConfView[1, ConfView.Rows.Count - 1] = dataGridViewButtonCell;

                // 行にタグ情報を設定（stageDataかlayerDataのインデックスか）
                string rowTag;
                bool useDefaultImage;
                
                if (i == 0)
                {
                    rowTag = "stage";
                    useDefaultImage = defaultValue == Global.cpd.project.Config.PatternImage;
                }
                else
                {
                    rowTag = $"layer:{i - 1}";
                    useDefaultImage = defaultValue == Global.cpd.project.Config.LayerImage;
                }
                
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

        private static List<(string, string)> GetLayerObjectsForStage(int selectedIndex)
        {
            List<(string, string)> layerObjects = [];

            var (stageData, layerData) = GetStageAndLayerData(selectedIndex);
            
            if (stageData != null && layerData != null && layerData.Count > 0)
            {
                layerObjects.Add(("キャラクターパターンのファイル名", stageData.Source));
                layerObjects.Add(("背景レイヤーチップ1のファイル名", layerData[0].Source));
                
                for (int i = 1; i < layerData.Count; i++)
                {
                    layerObjects.Add(("背景レイヤーチップ" + (i + 1) + "のファイル名", layerData[i].Source));
                }
            }

            return layerObjects;
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

        private void MoveLayerData(int sourceIndex, int targetIndex)
        {
            var (stageData, layerData) = GetStageAndLayerData(ConfigSelector.SelectedIndex);
            var (stageSize, layerSize) = GetRuntimeDefinitions(ConfigSelector.SelectedIndex);

            if (layerData != null && layerSize != null &&
                sourceIndex < layerData.Count && targetIndex <= layerData.Count)
            {
                // ソースアイテムを取得
                var sourceLayerItem = layerData[sourceIndex];
                var sourceMapchipItem = sourceIndex < layerSize.mapchips.Count ? layerSize.mapchips[sourceIndex] : null;

                // ソースアイテムを削除
                layerData.RemoveAt(sourceIndex);
                if (sourceIndex < layerSize.mapchips.Count)
                {
                    layerSize.mapchips.RemoveAt(sourceIndex);
                }

                // 削除により位置が変わる場合の調整
                int adjustedTargetIndex = targetIndex > sourceIndex ? targetIndex - 1 : targetIndex;

                // ターゲット位置に挿入
                layerData.Insert(adjustedTargetIndex, sourceLayerItem);
                if (sourceMapchipItem != null)
                {
                    layerSize.mapchips.Insert(adjustedTargetIndex, sourceMapchipItem);
                }

                Global.state.EditFlag = true;
                RefreshDesignerForRelation("LAYERCHIP");
            }
        }

        private void ConfView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hitTest = ConfView.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex > 0) // ステージ行（0行目）は移動不可
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

            if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex > 0) // ステージ行以外
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
            else if (hitTest.Type == DataGridViewHitTestType.None && ConfView.Rows.Count > 1)
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

            int targetLayerIndex = GetDropTargetLayerIndex(clientPoint, hitTest);
            
            if (targetLayerIndex >= 0 && dragRowIndex > 0)
            {
                int sourceLayerIndex = dragRowIndex - 1; // ステージ行を除いたレイヤーインデックス

                if (sourceLayerIndex != targetLayerIndex)
                {
                    // データソースでの移動
                    MoveLayerData(sourceLayerIndex, targetLayerIndex);

                    // 表示の更新（タグ情報も含めて再構築）
                    ConfigSelector_SelectedIndexChanged(null, EventArgs.Empty);

                    Global.MainWnd.UpdateLayerVisibility();

                    // 編集中のレイヤーが移動した場合の処理
                    int newEditingIndex = GetNewEditingLayerIndex(sourceLayerIndex, targetLayerIndex);
                    if(Global.state.EdittingLayerIndex == sourceLayerIndex && newEditingIndex >= 0) 
                    {
                        Global.MainWnd.LayerCount_Click(newEditingIndex);
                    }
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

        private int GetDropTargetLayerIndex(Point clientPoint, DataGridView.HitTestInfo hitTest)
        {
            // 行の境界でのドロップを検出
            if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
            {
                Rectangle cellRect = ConfView.GetCellDisplayRectangle(0, hitTest.RowIndex, false);
                int cellMiddleY = cellRect.Y + cellRect.Height / 2;

                if (clientPoint.Y < cellMiddleY)
                {
                    // セルの上半分 - その行の前に挿入
                    return Math.Max(0, hitTest.RowIndex - 1);
                }
                else
                {
                    // セルの下半分 - その行の後に挿入
                    return hitTest.RowIndex;
                }
            }
            else if (hitTest.Type == DataGridViewHitTestType.None || hitTest.RowIndex < 0)
            {
                // グリッド外 - 最後に挿入
                return ConfView.Rows.Count - 1;
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

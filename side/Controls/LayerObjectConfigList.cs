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
            ConfView.CellValueChanged += ConfView_CellValueChanged;
            ConfView.CellClick += ConfView_CellClick;
            ConfView.EditingControlShowing += ConfView_EditingControlShowing;
            ConfView.CurrentCellDirtyStateChanged += ConfView_CurrentCellDirtyStateChanged;
            ConfView.CellContentClick += ConfView_CellContentClick;

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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MasaoPlus.Dialogs;
using MasaoPlus.Properties;
using WMPLib;

namespace MasaoPlus.Controls
{
    public partial class ConfigList : UserControl
    {
        private class ComboBoxColumnHandler
        {
            private readonly DataGridView _dataGridView;

            public ComboBoxColumnHandler(DataGridView dataGridView)
            {
                _dataGridView = dataGridView;
                _dataGridView.EditingControlShowing += DataGridView_EditingControlShowing;
            }

            private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
            {
                if (e.Control is ComboBox comboBox)
                {
                    // 現在の列がComboBox列かチェック
                    if (_dataGridView.CurrentCell is DataGridViewComboBoxCell)
                    {
                        // 最大幅を計算
                        int maxWidth = CalculateMaxDropDownWidth(comboBox);
                        
                        // 画面の右端を超えないように調整
                        int maxAllowedWidth = Screen.FromControl(_dataGridView).WorkingArea.Width - 
                                            _dataGridView.GetCellDisplayRectangle(_dataGridView.CurrentCell.ColumnIndex, 
                                            _dataGridView.CurrentCell.RowIndex, false).Left;
                        comboBox.DropDownWidth = Math.Min(maxWidth, maxAllowedWidth);
                    }
                }
            }

            private int CalculateMaxDropDownWidth(ComboBox comboBox)
            {
                int maxWidth = 0;
                using (Graphics g = comboBox.CreateGraphics())
                {
                    foreach (object item in comboBox.Items)
                    {
                        int width = (int)g.MeasureString(item.ToString(), comboBox.Font).Width;
                        maxWidth = Math.Max(maxWidth, width);
                    }
                }
                return maxWidth + SystemInformation.VerticalScrollBarWidth + 10;
            }
        }

        private readonly ComboBoxColumnHandler _comboBoxHandler;

        public ConfigList()
        {
            InitializeComponent();
            _comboBoxHandler = new ComboBoxColumnHandler(ConfView);
        }

        public virtual void Prepare()
        {
            ConfigSelector.Items.Clear();
            ConfigSelector.Items.Add("全部");
            foreach (string text in Global.cpd.project.Config.Categories)
            {
                if (text != null) ConfigSelector.Items.Add(text);
            }
            ConfigSelector.SelectedIndex = 0;
        }

        public virtual void Reload()
        {
            ConfigSelector_SelectedIndexChanged(this, new EventArgs());
        }


        // 表示を変える
        protected virtual void ConfigSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigSelector.Items.Count < 1)
            {
                return;
            }
            
            ClearAndPrepareView();
            PopulateConfigurationRows();
            ApplyWrapModeIfEnabled();
        }

        private void ClearAndPrepareView()
        {
            OrigIdx.Clear();
            ConfView.Rows.Clear();
        }

        private void PopulateConfigurationRows()
        {
            for (int i = 0; i < Global.cpd.project.Config.Configurations.Length; i++)
            {
                ConfigParam configParam = Global.cpd.project.Config.Configurations[i];
                if (ShouldIncludeConfiguration(configParam))
                {
                    TrackSpecialIndices(configParam, i);
                    AddConfigurationRow(configParam, i);
                }
            }
        }

        private bool ShouldIncludeConfiguration(ConfigParam configParam)
        {
            bool isInSelectedCategory = ConfigSelector.SelectedIndex == 0 || 
                                      configParam.Category == Global.cpd.project.Config.Categories[ConfigSelector.SelectedIndex - 1];
            
            bool isNotSpecialRelation = configParam.Relation != "STAGENUM" && 
                                      configParam.Relation != "STAGESTART" && 
                                      configParam.Relation != "STAGESELECT";
            
            return isInSelectedCategory && isNotSpecialRelation;
        }

        private void TrackSpecialIndices(ConfigParam configParam, int index)
        {
            if (configParam.Name == "width") 
                width_index = index;
            else if (configParam.Name == "height") 
                height_index = index;
        }

        private void AddConfigurationRow(ConfigParam configParam, int originalIndex)
        {
            OrigIdx.Add(originalIndex);
            ConfView.Rows.Add([configParam.Description, configParam.Value]);
            
            int rowIndex = OrigIdx.Count - 1;
            SetupCellForConfigType(configParam, rowIndex);
            SetRowBackgroundColor(configParam, rowIndex);
        }

        private void SetupCellForConfigType(ConfigParam configParam, int rowIndex)
        {
            switch (configParam.Type)
            {
                case ConfigParam.Types.b:
                case ConfigParam.Types.b0:
                    SetupBooleanCell(configParam, rowIndex, "true", "false");
                    break;
                case ConfigParam.Types.b2:
                    SetupBooleanCell(configParam, rowIndex, "false", "true");
                    break;
                case ConfigParam.Types.i:
                    SetupNumericCell(configParam, rowIndex);
                    break;
                case ConfigParam.Types.t:
                    SetupTextCell(configParam, rowIndex);
                    break;
                case ConfigParam.Types.f:
                case ConfigParam.Types.f_i:
                case ConfigParam.Types.f_a:
                    SetupFileCell(configParam, rowIndex);
                    break;
                case ConfigParam.Types.l:
                    SetupListCell(configParam, rowIndex);
                    break;
                case ConfigParam.Types.l_a:
                    SetupAthleticListCell(configParam, rowIndex);
                    break;
                case ConfigParam.Types.c:
                    SetupColorCell(configParam, rowIndex);
                    break;
            }
        }

        private void SetupBooleanCell(ConfigParam configParam, int rowIndex, string trueValue, string falseValue)
        {
            DataGridViewCheckBoxCell cell = new()
            {
                TrueValue = trueValue,
                FalseValue = falseValue
            };
            
            if (bool.TryParse(configParam.Value, out bool flag))
            {
                cell.Value = (configParam.Type == ConfigParam.Types.b2 ? !flag : flag).ToString();
            }
            else
            {
                cell.Value = false;
            }
            
            ConfView[1, rowIndex] = cell;
        }

        private void SetupNumericCell(ConfigParam configParam, int rowIndex)
        {
            DataGridViewNumericUpdownCell cell = new()
            {
                Value = configParam.Value
            };
            ConfView[1, rowIndex] = cell;
        }

        private void SetupTextCell(ConfigParam configParam, int rowIndex)
        {
            if (Global.config.localSystem.UsePropExTextEditor)
            {
                DataGridViewButtonCell cell = new()
                {
                    Value = configParam.Value,
                    FlatStyle = FlatStyle.Popup
                };
                ConfView[1, rowIndex] = cell;
            }
            else
            {
                ConfView[1, rowIndex].Value = configParam.Value;
                ConfView[1, rowIndex].Style.WrapMode = DataGridViewTriState.True;
                ConfView[1, rowIndex].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                ConfView[1, rowIndex].ToolTipText = "Shift+Enterで改行できます。";
            }
        }

        private void SetupFileCell(ConfigParam configParam, int rowIndex)
        {
            DataGridViewButtonCell cell = new()
            {
                Value = configParam.Value + "...",
                FlatStyle = FlatStyle.Popup
            };
            ConfView[1, rowIndex] = cell;
        }

        private void SetupListCell(ConfigParam configParam, int rowIndex)
        {
            DataGridViewComboBoxCell cell = new()
            {
                FlatStyle = FlatStyle.Popup
            };
            cell.Items.AddRange(configParam.ListItems);

            if (int.TryParse(configParam.Value, out int num) && num <= cell.Items.Count && num > 0)
            {
                cell.Value = configParam.ListItems[num - 1];
            }
            else
            {
                cell.Value = configParam.ListItems[0];
            }

            ConfView[1, rowIndex] = cell;
        }

        private void SetupAthleticListCell(ConfigParam configParam, int rowIndex)
        {
            DataGridViewComboBoxCell cell = new()
            {
                FlatStyle = FlatStyle.Popup
            };
            cell.Items.AddRange(configParam.ListItems);

            int MaxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;
            if (int.TryParse(configParam.Value, out int num) && 
                (num > 0 && num <= MaxAthleticNumber || num >= 1001 && num <= 1249))
            {
                if (num <= MaxAthleticNumber)
                {
                    cell.Value = configParam.ListItems[num - 1];
                }
                else
                {
                    cell.Value = configParam.ListItems[num - (1001 - MaxAthleticNumber)];
                }
            }
            else
            {
                cell.Value = configParam.ListItems[0];
            }

            ConfView[1, rowIndex] = cell;
        }

        private void SetupColorCell(ConfigParam configParam, int rowIndex)
        {
            DataGridViewButtonCell cell = new()
            {
                Value = configParam.Value
            };
            
            Colors colors = new(configParam.Value);
            cell.Style.BackColor = colors.c;
            cell.Style.SelectionBackColor = colors.c;
            cell.FlatStyle = FlatStyle.Popup;
            
            ConfView[1, rowIndex] = cell;
        }

        private void SetRowBackgroundColor(ConfigParam configParam, int rowIndex)
        {
            ConfView.Rows[rowIndex].DefaultCellStyle.BackColor = configParam.Category switch
            {
                "システム" => Color.LightCyan,
                "表示" => Color.AliceBlue,
                "BGM" => Color.GhostWhite,
                "効果音" => Color.SeaShell,
                "仕掛け" => Color.MistyRose,
                "画像" => Color.Honeydew,
                "装備" => Color.MintCream,
                "敵" => Color.Lavender,
                "お店" => Color.LavenderBlush,
                "地図" => Color.Azure,
                "オリジナルボス" => Color.LightYellow,
                "リンク土管" => Color.MistyRose,
                "メッセージ" => Color.OldLace,
                _ => default,
            };
        }

        public void ApplyWrapModeIfEnabled()
        {
            if (Global.config.localSystem.WrapPropText) 
                ConfView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        protected virtual void ConfigSelector_Resize(object sender, EventArgs e)
        {
            ConfigSelector.Refresh();
        }

        protected virtual void ConfView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsValidCellEventArgs(e))
            {
                return;
            }

            int configIndex = OrigIdx[e.RowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[configIndex];

            if (HandleConfigParamClick(configParam, configIndex, e))
            {
                Global.state.EditFlag = true;
            }
        }

        private bool IsValidCellEventArgs(DataGridViewCellEventArgs e)
        {
            return e.ColumnIndex == 1 && 
                   e.RowIndex >= 0 && 
                   e.RowIndex < OrigIdx.Count;
        }

        private bool HandleConfigParamClick(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            return configParam.Type switch
            {
                ConfigParam.Types.t => HandleTextConfig(configParam, configIndex, e),
                ConfigParam.Types.f or ConfigParam.Types.f_i or ConfigParam.Types.f_a => HandleFileConfig(configParam, configIndex, e),
                ConfigParam.Types.c => HandleColorConfig(configParam, configIndex, e),
                ConfigParam.Types.l or ConfigParam.Types.l_a => false,
                _ => false
            };
        }

        private bool HandleTextConfig(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            if (!Global.config.localSystem.UsePropExTextEditor)
            {
                return false;
            }

            using PropertyTextInputDialog dialog = new();
            dialog.InputStr = configParam.Value;
            
            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                ConfView[e.ColumnIndex, e.RowIndex].Value = dialog.InputStr;
                Global.cpd.project.Config.Configurations[configIndex].Value = dialog.InputStr;
                return true;
            }
            
            return false;
        }

        private bool HandleFileConfig(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            SetupFileDialog(openFileDialog, configParam);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            string selectedFile = openFileDialog.FileName;
            string targetPath = Path.Combine(Global.cpd.where, Path.GetFileName(selectedFile));

            if (!TryCopyFileToProject(selectedFile, ref targetPath))
            {
                return false;
            }

            UpdateFileConfigValue(configParam, configIndex, targetPath, e);
            HandleFileConfigRelations(configParam, configIndex);
            
            return true;
        }

        private static void SetupFileDialog(OpenFileDialog dialog, ConfigParam configParam)
        {
            dialog.InitialDirectory = Global.cpd.where;
            dialog.FileName = configParam.Value;
            
            dialog.Filter = configParam.Type switch
            {
                ConfigParam.Types.f_i => "画像(*.gif;*.png;*.jpg;*.bmp)|*.gif;*.png;*.jpg;*.bmp|全てのファイル (*.*)|*.*",
                ConfigParam.Types.f_a => "音声(*.wav;*.mp3;*.ogg)|*.wav;*.mp3;*.ogg|全てのファイル (*.*)|*.*",
                _ => "全てのファイル (*.*)|*.*"
            };
        }

        protected static bool TryCopyFileToProject(string sourceFile, ref string targetPath)
        {
            if (Path.GetDirectoryName(sourceFile) == Global.cpd.where)
            {
                return true;
            }

            if (!ConfirmFileCopy())
            {
                return false;
            }

            try
            {
                File.Copy(sourceFile, targetPath, true);
                return true;
            }
            catch (IOException)
            {
                return HandleFileCopyError(sourceFile, ref targetPath);
            }
        }

        private static bool ConfirmFileCopy()
        {
            string message = $"ファイルはプロジェクトディレクトリに含まれていません。{Environment.NewLine}" +
                           $"プロジェクトディレクトリへコピーします。{Environment.NewLine}" +
                           $"また、同じ名前のファイルがある場合は上書きされます。{Environment.NewLine}" +
                           "よろしいですか？";
            
            return MessageBox.Show(message, "ファイルの追加", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private static bool HandleFileCopyError(string sourceFile, ref string targetPath)
        {
            MessageBox.Show($"ファイルをコピーできませんでした。{Environment.NewLine}ファイルのコピー先を再指定してください。", 
                          "コピー失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);

            return GetAlternativeFilePath(sourceFile, ref targetPath);
        }

        private static bool GetAlternativeFilePath(string sourceFile, ref string targetPath)
        {
            while (true)
            {
                using SaveFileDialog saveDialog = new();
                SetupSaveDialog(saveDialog, sourceFile, targetPath);

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                if (ValidateAlternativePath(saveDialog.FileName, ref targetPath))
                {
                    return true;
                }
            }
        }

        private static void SetupSaveDialog(SaveFileDialog dialog, string sourceFile, string targetPath)
        {
            dialog.InitialDirectory = Path.GetDirectoryName(targetPath);
            dialog.FileName = Path.GetFileName(targetPath);
            dialog.Filter = "保存ファイル|*" + Path.GetExtension(sourceFile);
            dialog.AddExtension = true;
            dialog.DefaultExt = Path.GetExtension(sourceFile);
        }

        private static bool ValidateAlternativePath(string selectedPath, ref string targetPath)
        {
            if (File.Exists(selectedPath))
            {
                MessageBox.Show($"上書きはできません。{Environment.NewLine}元のファイルを消すか、別のファイルを指定してください。", 
                              "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            if (Path.GetDirectoryName(selectedPath) != Global.cpd.where)
            {
                MessageBox.Show("プロジェクトディレクトリ内に保存してください。", 
                              "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            targetPath = selectedPath;
            return true;
        }

        private void UpdateFileConfigValue(ConfigParam configParam, int configIndex, string filePath, DataGridViewCellEventArgs e)
        {
            string fileName = Path.GetFileName(filePath);
            ConfView[e.ColumnIndex, e.RowIndex].Value = $"{fileName}...";
            Global.cpd.project.Config.Configurations[configIndex].Value = fileName;
            if (Global.cpd.project.Config.Configurations[configIndex].Relation == "PATTERN" || Global.cpd.project.Config.Configurations[configIndex].Relation == "LAYERCHIP")
            {
                if (Global.cpd.project.Config.Configurations[configIndex].Relation == "PATTERN")
                {
                    ProjectLoading.SetStageDataSources();
                }
                else if (Global.cpd.project.Config.Configurations[configIndex].Relation == "LAYERCHIP")
                {
                    ProjectLoading.SetLayerDataSources();
                }
                Global.MainWnd.LayerObjectConfigList.ConfView.Rows.Clear();
                Global.MainWnd.LayerObjectConfigList.PopulateLayerObjectRows(Global.MainWnd.LayerObjectConfigList.ConfigSelector.SelectedIndex);
            }
        }

        private static void HandleFileConfigRelations(ConfigParam configParam, int configIndex)
        {
            string relation = configParam.Relation;
            if (string.IsNullOrEmpty(relation))
            {
                HandleSpecialFileConfigs(configParam);
                return;
            }

            RefreshDesignerForRelation(relation);
            HandleSpecialFileConfigs(configParam);
        }

        protected static void RefreshDesignerForRelation(string relation)
        {
            switch (relation)
            {
                case "PATTERN":
                    Global.MainWnd.MainDesigner.PrepareImages();
                    Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                    if(!Global.state.EditingForeground)
                    {
                        Global.MainWnd.MainDesigner.InitTransparent();
                    }
                    break;
                case "LAYERCHIP":
                    Global.MainWnd.MainDesigner.PrepareImages();
                    Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
                    Global.MainWnd.MainDesigner.InitTransparentForBackground();
                    break;
            }
            Global.MainWnd.MainDesigner.Refresh();
        }

        private static void HandleSpecialFileConfigs(ConfigParam configParam)
        {
            if (configParam.Name == "filename_oriboss_left1")
            {
                Global.MainWnd.MainDesigner.PrepareImages();
                Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                Global.MainWnd.MainDesigner.Refresh();
            }
            else if (reg_file_prepare().IsMatch(configParam.Name))
            {
                Global.MainWnd.MainDesigner.PrepareImages();
                Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
                Global.MainWnd.MainDesigner.Refresh();
            }
        }

        private bool HandleColorConfig(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            using ColorDialog colorDialog = new();
            Colors colors = new(configParam.Value);
            colorDialog.Color = colors.c;

            if (colorDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            UpdateColorConfigValue(colors, colorDialog.Color, configIndex, e);
            if (!string.IsNullOrEmpty(configParam.Relation) && configParam.Relation.StartsWith("BACKGROUND"))
            {
                HandleBackgroundColorUpdate(configParam, colorDialog.Color);
            }

            return true;
        }

        private void UpdateColorConfigValue(Colors colors, Color newColor, int configIndex, DataGridViewCellEventArgs e)
        {
            colors.c = newColor;
            ConfView[e.ColumnIndex, e.RowIndex].Value = colors.ToString();
            ConfView[e.ColumnIndex, e.RowIndex].Style.BackColor = colors.c;
            ConfView[e.ColumnIndex, e.RowIndex].Style.SelectionBackColor = colors.c;
            Global.cpd.project.Config.Configurations[configIndex].Value = colors.ToString();
        }

        private static void HandleBackgroundColorUpdate(ConfigParam configParam, Color color)
        {
            var backgroundMappings = new Dictionary<string, int>
            {
                { "BACKGROUND", 0 },
                { "BACKGROUND2", 1 },
                { "BACKGROUND3", 2 },
                { "BACKGROUND4", 3 },
                { "BACKGROUNDM", 4 }
            };

            if (backgroundMappings.TryGetValue(configParam.Relation, out int stageIndex) &&
                Global.state.EdittingStage == stageIndex)
            {
                Global.state.Background = color;
                Global.MainWnd.UpdateLayer();
            }
        }

        private void ConfView_PreviewAudio(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            if (!IsAudioPreviewRow(e.RowIndex))
                return;

            int num = OrigIdx[e.RowIndex];
            if (now_playing_item != num)
            {
                PlayAudio(e.RowIndex);
            }
            else
            {
                StopAudio();
            }
        }

        private bool IsAudioPreviewRow(int rowIndex)
        {
            if (rowIndex >= OrigIdx.Count || rowIndex < 0)
                return false;

            int num = OrigIdx[rowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
            return configParam.Type == ConfigParam.Types.f_a;
        }

        private bool isPlaying = false;

        private void MediaPlayer_PlayStateChange(int NewState)
        {
            if (NewState == (int)WMPPlayState.wmppsStopped || 
                NewState == (int)WMPPlayState.wmppsMediaEnded)
            {
                isPlaying = false;
                now_playing_item = -1;
                
                // コントロールスレッドで実行する必要がある
                if (InvokeRequired)
                {
                    Invoke(new Action(() => menuStopAudio.Enabled = false));
                }
                else
                {
                    menuStopAudio.Enabled = false;
                }
            }
            else if (NewState == (int)WMPPlayState.wmppsPlaying)
            {
                isPlaying = true;
                if (InvokeRequired)
                {
                    Invoke(new Action(() => menuStopAudio.Enabled = true));
                }
                else
                {
                    menuStopAudio.Enabled = true;
                }
            }
        }

        private void PlayAudio(int rowIndex)
        {
            if (!IsAudioPreviewRow(rowIndex))
                return;

            int num = OrigIdx[rowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[num];

            if (now_playing_item != num)
            {
                now_playing_item = num;
                mediaPlayer.URL = Path.Combine(Global.cpd.where, configParam.Value);
            }
            else
            {
                mediaPlayer.controls.stop();
            }
            mediaPlayer.controls.play();
        }

        private void StopAudio()
        {
            if (now_playing_item != -1)
            {
                now_playing_item = -1;
                mediaPlayer.controls.stop();
            }
        }

        private void MenuPlayAudio_Click(object sender, EventArgs e)
        {
            PlayAudio(rightClickedRowIndex);
        }

        private void MenuStopAudio_Click(object sender, EventArgs e)
        {
            StopAudio();
        }

        private static void PreviewAudio_error(object pMediaObject)
        {
            MessageBox.Show("ファイルの読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        protected virtual void ConfView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = ConfView.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && 
                    hitTest.RowIndex >= 0 && 
                    IsAudioPreviewRow(hitTest.RowIndex))
                {
                    rightClickedRowIndex = hitTest.RowIndex;
                    
                    // メニューアイテムの有効/無効を設定
                    int configIndex = OrigIdx[hitTest.RowIndex];
                    ConfigParam configParam = Global.cpd.project.Config.Configurations[configIndex];
                    string audioPath = Path.Combine(Global.cpd.where, configParam.Value);
                    
                    menuPlayAudio.Enabled = File.Exists(audioPath);
                    menuStopAudio.Enabled = isPlaying;
                    
                    audioContextMenu.Show(ConfView, e.Location);
                }
            }
        }

        protected virtual void ConfView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl dataGridViewTextBoxEditingControl)
            {
                dataGridViewTextBoxEditingControl.PreviewKeyDown += ct_PreviewKeyDown;
                dataGridViewTextBoxEditingControl.ScrollBars = ScrollBars.Both;
            }
        }

        private void ct_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (keyCode != Keys.Return)
            {
                return;
            }
            e.IsInputKey = true;
        }

        protected virtual void ConfView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (ConfView.CurrentCellAddress.X == 1 && ConfView.IsCurrentCellDirty)
            {
                ConfView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        protected virtual void ConfView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsValidCellEventArgs(e))
            {
                return;
            }

            int configIndex = OrigIdx[e.RowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[configIndex];

            if (ProcessConfigValueChange(configParam, configIndex, e))
            {
                HandlePostValueChangeActions(configParam, configIndex);
                Global.state.EditFlag = true;
            }
        }

        private bool ProcessConfigValueChange(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            return configParam.Type switch
            {
                ConfigParam.Types.b or ConfigParam.Types.b2 or ConfigParam.Types.b0 => ProcessBooleanValue(configParam, configIndex, e),
                ConfigParam.Types.s => ProcessStringValue(configParam, configIndex, e),
                ConfigParam.Types.i => ProcessIntegerValue(configParam, configIndex, e),
                ConfigParam.Types.t => ProcessTextValue(configParam, configIndex, e),
                ConfigParam.Types.f or ConfigParam.Types.f_i or ConfigParam.Types.f_a => false,
                ConfigParam.Types.l => ProcessListValue(configParam, configIndex, e),
                ConfigParam.Types.l_a => ProcessAthleticListValue(configParam, configIndex, e),
                _ => false
            };
        }

        private bool ProcessBooleanValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            string newValue = ConfView[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
            if (configParam.Value == newValue)
            {
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = newValue;
            return true;
        }

        private bool ProcessStringValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
            {
                ConfView[e.ColumnIndex, e.RowIndex].Value = "";
            }

            string newValue = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (configParam.Value == newValue)
            {
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = newValue;
            return true;
        }

        private bool ProcessIntegerValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
            {
                ConfView[e.ColumnIndex, e.RowIndex].Value = "0";
            }

            string valueString = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (!int.TryParse(valueString, out int parsedValue))
            {
                MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                if (configParam.Value == valueString)
                {
                    return false;
                }
                ConfView[e.ColumnIndex, e.RowIndex].Value = configParam.Value;
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = parsedValue.ToString();
            return true;
        }

        private bool ProcessTextValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
            {
                ConfView[e.ColumnIndex, e.RowIndex].Value = "";
            }

            string text = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
            string[] lines = text.Split([Environment.NewLine], StringSplitOptions.None);

            if (configParam.Rows > 0)
            {
                lines = ProcessTextRows(lines, configParam.Rows);
            }

            string processedText = string.Join(Environment.NewLine, lines);
            if (configParam.Value == processedText)
            {
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = processedText;
            ConfView[e.ColumnIndex, e.RowIndex].Value = processedText;
            return true;
        }

        private static string[] ProcessTextRows(string[] lines, int maxRows)
        {
            List<string> lineList = [.. lines];

            if (lineList.Count > maxRows)
            {
                MessageBox.Show($"行数が最大値を超えています。{Environment.NewLine}超えた行は削除されます。", 
                              "行の超過", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                lineList.RemoveRange(maxRows, lineList.Count - maxRows);
            }
            else if (lineList.Count < maxRows)
            {
                while (lineList.Count < maxRows)
                {
                    lineList.Add("0");
                }
            }

            return [.. lineList];
        }

        private bool ProcessListValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            var comboCell = (DataGridViewComboBoxCell)ConfView[e.ColumnIndex, e.RowIndex];
            int selectedIndex = comboCell.Items.IndexOf(ConfView[e.ColumnIndex, e.RowIndex].Value) + 1;
            string newValue = selectedIndex.ToString();

            if (configParam.Value == newValue)
            {
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = newValue;

            if (configParam.Name == "mcs_screen_size")
            {
                HandleScreenSizeChange(newValue);
            }

            return true;
        }

        private void HandleScreenSizeChange(string newValue)
        {
            if (newValue == "1")
            {
                ApplyScreenSize640x480();
            }
            else if (newValue == "2")
            {
                ApplyScreenSize512x320();
            }

            UpdateEditingReferences();
            NotifyScreenSizeChange();
        }

        private void ApplyScreenSize640x480()
        {
            Global.cpd.project.Config.Configurations[width_index].Value = "640";
            Global.cpd.project.Config.Configurations[height_index].Value = "480";
            
            int tmp_y = Global.cpd.runtime.Definitions.MapSize.y;
            Global.cpd.runtime.Definitions.MapSize.x = 19;
            Global.cpd.runtime.Definitions.MapSize.y = 14;

            ResizeMapData(tmp_y);
            Global.state.MinimumStageSize = new Size(20, 15);

            ResizeAllStageData();
        }

        private static void ResizeMapData(int originalHeight)
        {
            Array.Resize(ref Global.cpd.project.MapData.Strings, Global.cpd.runtime.Definitions.MapSize.y);
            for (int i = 0; i < Global.cpd.runtime.Definitions.MapSize.y; i++)
            {
                int paddingNeeded = i < originalHeight 
                    ? Global.cpd.runtime.Definitions.MapSize.x - Global.cpd.project.MapData[i].Length
                    : Global.cpd.runtime.Definitions.MapSize.x;
                
                for (int j = 0; j < paddingNeeded; j++)
                {
                    Global.cpd.project.MapData[i] += ".";
                }
            }
        }

        private static void ResizeAllStageData()
        {
            string nullcode = Global.cpd.Mapchip[0].code;

            ResizeStageData(ref Global.cpd.project.StageData.Strings, ref Global.cpd.project.Runtime.Definitions.StageSize, nullcode);
            ResizeStageData(ref Global.cpd.project.StageData2.Strings, ref Global.cpd.project.Runtime.Definitions.StageSize2, nullcode);
            ResizeStageData(ref Global.cpd.project.StageData3.Strings, ref Global.cpd.project.Runtime.Definitions.StageSize3, nullcode);
            ResizeStageData(ref Global.cpd.project.StageData4.Strings, ref Global.cpd.project.Runtime.Definitions.StageSize4, nullcode);

            if (Global.cpd.project.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                ProcessAllLayerData();
            }
        }

        private static void ResizeStageData(ref string[] data, ref Runtime.DefinedData.StageSizeData sizeData, string nullcode)
        {
            ResizeDataHeight(sizeData, Global.state.MinimumStageSize.Height, ref data, nullcode);
            ResizeDataWidth(sizeData.x, Global.state.MinimumStageSize.Width, ref data, nullcode);
            UpdateStageSizeData(ref sizeData);
        }

        private static void ResizeDataHeight(Runtime.DefinedData.StageSizeData beforeSize, int height, ref string[] data, string nullcode)
        {
            if (beforeSize.y < height)
            {
                Array.Resize(ref data, height);
                var array = new string[beforeSize.x];
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = nullcode;
                }
                var s = string.Join(",", array);
                for (int i = beforeSize.y; i < data.Length; i++)
                {
                    data[i] = s;
                }
            }
        }

        private static void ResizeDataWidth(int before, int width, ref string[] data, string nullcode)
        {
            if (before < width)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var array = data[i].Split(',');
                    Array.Resize(ref array, width);
                    for (int j = before; j < array.Length; j++)
                    {
                        array[j] = nullcode;
                    }
                    data[i] = string.Join(",", array);
                }
            }
        }

        private static void UpdateStageSizeData(ref Runtime.DefinedData.StageSizeData size)
        {
            if (size.x < Global.state.MinimumStageSize.Width) size.x = Global.state.MinimumStageSize.Width;
            if (size.y < Global.state.MinimumStageSize.Height) size.y = Global.state.MinimumStageSize.Height;
            size.bytesize = Global.cpd.project.Runtime.Definitions.StageSize.bytesize;
        }

        private static void ProcessAllLayerData()
        {
            ProcessLayerDataCollection(Global.cpd.project.LayerData, ref Global.cpd.project.Runtime.Definitions.LayerSize);
            ProcessLayerDataCollection(Global.cpd.project.LayerData2, ref Global.cpd.project.Runtime.Definitions.LayerSize2);
            ProcessLayerDataCollection(Global.cpd.project.LayerData3, ref Global.cpd.project.Runtime.Definitions.LayerSize3);
            ProcessLayerDataCollection(Global.cpd.project.LayerData4, ref Global.cpd.project.Runtime.Definitions.LayerSize4);
        }

        private static void ProcessLayerDataCollection(List<LayerObject> layerData, ref Runtime.DefinedData.LayerSizeData sizeData)
        {
            string nullcode = Global.cpd.Layerchip[0].code;
            for (int i = 0; i < layerData.Count; i++)
            {
                ResizeDataHeight(sizeData, Global.state.MinimumStageSize.Height, ref layerData[i].Strings, nullcode);
                ResizeDataWidth(sizeData.x, Global.state.MinimumStageSize.Width, ref layerData[i].Strings, nullcode);
            }

            if (sizeData.x < Global.state.MinimumStageSize.Width) sizeData.x = Global.state.MinimumStageSize.Width;
            if (sizeData.y < Global.state.MinimumStageSize.Height) sizeData.y = Global.state.MinimumStageSize.Height;
            sizeData.bytesize = Global.cpd.project.Runtime.Definitions.LayerSize.bytesize;
        }

        private void ApplyScreenSize512x320()
        {
            Global.cpd.project.Config.Configurations[width_index].Value = "512";
            Global.cpd.project.Config.Configurations[height_index].Value = "320";
            Global.cpd.runtime.Definitions.MapSize.x = 15;
            Global.cpd.runtime.Definitions.MapSize.y = 9;

            Array.Resize(ref Global.cpd.project.MapData.Strings, Global.cpd.runtime.Definitions.MapSize.y);
            for (int i = 0; i < Global.cpd.runtime.Definitions.MapSize.y; i++)
            {
                Global.cpd.project.MapData[i] = Global.cpd.project.MapData[i][..Global.cpd.runtime.Definitions.MapSize.x];
            }
            Global.state.MinimumStageSize = new Size(16, 10);
        }

        private static void UpdateEditingReferences()
        {
            switch (Global.state.EdittingStage)
            {
                case 0:
                    if (Global.state.EditingForeground)
                    {
                        Global.cpd.EditingMap = Global.cpd.project.StageData;
                    }
                    else
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData[Global.state.EdittingLayerIndex];
                    }
                    break;
                case 1:
                    if (Global.state.EditingForeground)
                    {
                        Global.cpd.EditingMap = Global.cpd.project.StageData2;
                    }
                    else
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData2[Global.state.EdittingLayerIndex];
                    }
                    break;
                case 2:
                    if (Global.state.EditingForeground)
                    {
                        Global.cpd.EditingMap = Global.cpd.project.StageData3;
                    }
                    else
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData3[Global.state.EdittingLayerIndex];
                    }
                    break;
                case 3:
                    if (Global.state.EditingForeground)
                    {
                        Global.cpd.EditingMap = Global.cpd.project.StageData4;
                    }
                    else
                    {
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData4[Global.state.EdittingLayerIndex];
                    }
                    break;
                case 4:
                    Global.cpd.EditingMap = Global.cpd.project.MapData;
                    break;
            }
        }

        private static void NotifyScreenSizeChange()
        {
            Global.state.StageSizeChanged = true;
            Global.MainWnd.MainDesigner.ForceBufferResize();
            Global.MainWnd.UpdateLayer();
            Global.MainWnd.UpdateScrollbar();
        }

        private bool ProcessAthleticListValue(ConfigParam configParam, int configIndex, DataGridViewCellEventArgs e)
        {
            var comboCell = (DataGridViewComboBoxCell)ConfView[e.ColumnIndex, e.RowIndex];
            int configParamNum = comboCell.Items.IndexOf(ConfView[e.ColumnIndex, e.RowIndex].Value) + 1;
            int maxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;

            string newValue = configParamNum <= maxAthleticNumber 
                ? configParamNum.ToString()
                : (configParamNum - 1 - maxAthleticNumber + 1001).ToString();

            bool isCurrentValue = configParamNum <= maxAthleticNumber 
                ? configParam.Value == configParamNum.ToString()
                : configParam.Value == (configParamNum - 1 - maxAthleticNumber + 1001).ToString();

            if (isCurrentValue)
            {
                return false;
            }

            Global.cpd.project.Config.Configurations[configIndex].Value = newValue;
            return true;
        }

        private static void HandlePostValueChangeActions(ConfigParam configParam, int configIndex)
        {
            if (!string.IsNullOrEmpty(configParam.ChipRelation))
            {
                Global.state.ChipRegister[configParam.ChipRelation] = Global.cpd.project.Config.Configurations[configIndex].Value;
                Global.MainWnd.RefreshAll();
            }
        }

        protected virtual void ConfView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                ConfView.BeginEdit(true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ConfigSelector = new ComboBox();
            ConfView = new DataGridView();
            CNames = new DataGridViewTextBoxColumn();
            CValues = new DataGridViewTextBoxColumn();
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
                CValues
            ]);
            ConfView.Dock = DockStyle.Fill;
            ConfView.EditMode = DataGridViewEditMode.EditOnEnter;
            ConfView.Location = new Point(0, LogicalToDeviceUnits(20));
            ConfView.MultiSelect = false;
            ConfView.Name = "ConfView";
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
            ConfView.CellDoubleClick += ConfView_PreviewAudio;
            ConfView.MouseDown += ConfView_MouseDown;

            CNames.HeaderText = "項目名";
            CNames.Name = "CNames";
            CNames.ReadOnly = true;
            CNames.SortMode = DataGridViewColumnSortMode.NotSortable;
            CValues.FillWeight = 50f;
            CValues.HeaderText = "値";
            CValues.Name = "CValues";
            CValues.SortMode = DataGridViewColumnSortMode.NotSortable;
            // AutoScaleDimensions = new SizeF(6f, 12f);
            // AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ConfView);
            Controls.Add(ConfigSelector);
            Name = "ConfigList";
            Size = LogicalToDeviceUnits(new Size(298, 358));
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);

            // メディアプレーヤークラスのインスタンスを作成する
            mediaPlayer = new WindowsMediaPlayer();
            mediaPlayer.MediaError += PreviewAudio_error;
            mediaPlayer.PlayStateChange += MediaPlayer_PlayStateChange;

            audioContextMenu = new ContextMenuStrip();
            menuPlayAudio = new ToolStripMenuItem
            {
                Name = "menuPlayAudio",
                Text = "音声を再生",
                Size = LogicalToDeviceUnits(new Size(179, 22)),
                Image = new IconImageView(DeviceDpi, Resources.control_play_blue).View()
            };
            menuStopAudio = new ToolStripMenuItem
            {
                Name = "menuStopAudio",
                Text = "音声を停止",
                Size = LogicalToDeviceUnits(new Size(179, 22)),
                Image = new IconImageView(DeviceDpi, Resources.control_stop_blue).View()
            };

            audioContextMenu.Items.AddRange([menuPlayAudio, menuStopAudio]);
            audioContextMenu.Size = LogicalToDeviceUnits(new Size(180, 48));

            menuPlayAudio.Click += MenuPlayAudio_Click;
            menuStopAudio.Click += MenuStopAudio_Click;
        }

        protected readonly List<int> OrigIdx = [];

        private readonly IContainer components;

        public ComboBox ConfigSelector;

        public DataGridView ConfView;

        protected DataGridViewTextBoxColumn CNames;

        protected DataGridViewTextBoxColumn CValues;

        private int width_index, height_index;

        private WindowsMediaPlayer mediaPlayer;

        private int now_playing_item = -1;

        [GeneratedRegex("^(filename_haikei|filename_second_haikei|filename_chizu)")]
        private static partial Regex reg_file_prepare();

        private ContextMenuStrip audioContextMenu;
        private ToolStripMenuItem menuPlayAudio;
        private ToolStripMenuItem menuStopAudio;
        private int rightClickedRowIndex = -1;
    }
}

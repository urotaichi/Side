using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MasaoPlus.Dialogs;
using WMPLib;

namespace MasaoPlus.Controls
{
    public class ConfigList : UserControl
    {
        public ConfigList()
        {
            InitializeComponent();
        }

        public void Prepare()
        {
            ConfigSelector.Items.Clear();
            ConfigSelector.Items.Add("全部");
            foreach (string text in Global.cpd.project.Config.Categories)
            {
                if (text != null) ConfigSelector.Items.Add(text);
            }
            ConfigSelector.SelectedIndex = 0;
        }

        public void Reload()
        {
            ConfigSelector_SelectedIndexChanged(this, new EventArgs());
        }


        // 表示を変える
        private void ConfigSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigSelector.Items.Count < 1)
            {
                return;
            }
            OrigIdx.Clear();
            ConfView.Rows.Clear();
            for (int i = 0; i < Global.cpd.project.Config.Configurations.Length; i++)
            {
                ConfigParam configParam = Global.cpd.project.Config.Configurations[i];
                if ((ConfigSelector.SelectedIndex == 0 || configParam.Category == Global.cpd.project.Config.Categories[ConfigSelector.SelectedIndex - 1]) && !(configParam.Relation == "STAGENUM") && !(configParam.Relation == "STAGESTART") && !(configParam.Relation == "STAGESELECT"))
                {
                    if (configParam.Name == "width") width_index = i;
                    else if (configParam.Name == "height") height_index = i;

                    OrigIdx.Add(i);
                    ConfView.Rows.Add(new string[]
                    {
                        configParam.Description,
                        configParam.Value
                    });
                    switch (configParam.Type)
                    {
                        case ConfigParam.Types.b:
                        case ConfigParam.Types.b0:
                            {
                                DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new DataGridViewCheckBoxCell
                                {
                                    TrueValue = "true",
                                    FalseValue = "false"
                                };
                                bool flag;
                                if (bool.TryParse(configParam.Value, out flag))
                                {
                                    dataGridViewCheckBoxCell.Value = flag.ToString();
                                }
                                else
                                {
                                    dataGridViewCheckBoxCell.Value = false;
                                }
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewCheckBoxCell;
                                break;
                            }
                        case ConfigParam.Types.b2:
                            {
                                DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new DataGridViewCheckBoxCell
                                {
                                    TrueValue = "false",
                                    FalseValue = "true"
                                };
                                bool flag;
                                if (bool.TryParse(configParam.Value, out flag))
                                {
                                    dataGridViewCheckBoxCell.Value = (!flag).ToString();
                                }
                                else
                                {
                                    dataGridViewCheckBoxCell.Value = false;
                                }
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewCheckBoxCell;
                                break;
                            }
                        case ConfigParam.Types.i:
                            {
                                DataGridViewNumericUpdownCell dataGridViewNumericUpdownCell = new DataGridViewNumericUpdownCell
                                {
                                    Value = configParam.Value
                                };
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewNumericUpdownCell;
                                break;
                            }
                        case ConfigParam.Types.t:
                            if (Global.config.localSystem.UsePropExTextEditor)
                            {
                                DataGridViewButtonCell dataGridViewButtonCell = new DataGridViewButtonCell
                                {
                                    Value = configParam.Value,
                                    FlatStyle = FlatStyle.Popup
                                };
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewButtonCell;
                            }
                            else
                            {
                                ConfView[1, OrigIdx.Count - 1].Value = configParam.Value;
                                ConfView[1, OrigIdx.Count - 1].Style.WrapMode = DataGridViewTriState.True;
                                ConfView[1, OrigIdx.Count - 1].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                                ConfView[1, OrigIdx.Count - 1].ToolTipText = "Shift+Enterで改行できます。";
                            }
                            break;
                        case ConfigParam.Types.f:
                        case ConfigParam.Types.f_i:
                        case ConfigParam.Types.f_a:
                            {
                                DataGridViewButtonCell dataGridViewButtonCell2 = new DataGridViewButtonCell
                                {
                                    Value = configParam.Value + "...",
                                    FlatStyle = FlatStyle.Popup
                                };
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewButtonCell2;
                                break;
                            }
                        case ConfigParam.Types.l:
                            {
                                DataGridViewComboBoxCell dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
                                dataGridViewComboBoxCell.Items.AddRange(configParam.ListItems);
                                dataGridViewComboBoxCell.FlatStyle = FlatStyle.Popup;
                                int num;
                                if (int.TryParse(configParam.Value, out num) && num <= dataGridViewComboBoxCell.Items.Count && num > 0)
                                {
                                    dataGridViewComboBoxCell.Value = configParam.ListItems[num - 1];
                                }
                                else
                                {
                                    dataGridViewComboBoxCell.Value = configParam.ListItems[0];
                                }
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewComboBoxCell;
                                break;
                            }
                        case ConfigParam.Types.l_a:
                            {
                                DataGridViewComboBoxCell dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
                                dataGridViewComboBoxCell.Items.AddRange(configParam.ListItems);
                                dataGridViewComboBoxCell.FlatStyle = FlatStyle.Popup;
                                int num, MaxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;
                                if (int.TryParse(configParam.Value, out num) && (num > 0 && num <= MaxAthleticNumber || num >= 1001 && num <= 1249))
                                {
                                    if (num <= MaxAthleticNumber)
                                    {
                                        dataGridViewComboBoxCell.Value = configParam.ListItems[num - 1];
                                    }
                                    else
                                    {
                                        dataGridViewComboBoxCell.Value = configParam.ListItems[num - (1001 - MaxAthleticNumber)];
                                    }
                                }
                                else
                                {
                                    dataGridViewComboBoxCell.Value = configParam.ListItems[0];
                                }
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewComboBoxCell;
                                break;
                            }
                        case ConfigParam.Types.c:
                            {
                                DataGridViewButtonCell dataGridViewButtonCell3 = new DataGridViewButtonCell
                                {
                                    Value = configParam.Value
                                };
                                Colors colors = new Colors(configParam.Value);
                                dataGridViewButtonCell3.Style.BackColor = colors.c;
                                dataGridViewButtonCell3.Style.SelectionBackColor = colors.c;
                                dataGridViewButtonCell3.FlatStyle = FlatStyle.Popup;
                                ConfView[1, OrigIdx.Count - 1] = dataGridViewButtonCell3;
                                break;
                            }
                    }
                    ConfView.Rows[OrigIdx.Count - 1].DefaultCellStyle.BackColor = configParam.Category switch
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
            }
            if (Global.config.localSystem.WrapPropText) ConfView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void ConfigSelector_Resize(object sender, EventArgs e)
        {
            ConfigSelector.Refresh();
        }

        private void ConfView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                return;
            }
            if (e.RowIndex >= OrigIdx.Count || e.RowIndex < 0)
            {
                return;
            }
            int num = OrigIdx[e.RowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
            switch (configParam.Type)
            {
                case ConfigParam.Types.t:
                    if (Global.config.localSystem.UsePropExTextEditor)
                    {
                        using PropertyTextInputDialog propertyTextInputDialog = new PropertyTextInputDialog();
                        propertyTextInputDialog.InputStr = configParam.Value;
                        if (propertyTextInputDialog.ShowDialog() != DialogResult.Cancel)
                        {
                            ConfView[e.ColumnIndex, e.RowIndex].Value = propertyTextInputDialog.InputStr;
                            Global.cpd.project.Config.Configurations[num].Value = propertyTextInputDialog.InputStr;
                        }
                        break;
                    }
                    return;
                case ConfigParam.Types.f:
                case ConfigParam.Types.f_i:
                case ConfigParam.Types.f_a:
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = Global.cpd.where;
                        openFileDialog.FileName = configParam.Value;
                        if (configParam.Type == ConfigParam.Types.f_i)
                            openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.bmp)|*.gif;*.png;*.jpg;*.bmp|全てのファイル (*.*)|*.*";
                        else if (configParam.Type == ConfigParam.Types.f_a)
                            openFileDialog.Filter = "音声(*.wav;*.mp3;*.ogg)|*.wav;*.mp3;*.ogg|全てのファイル (*.*)|*.*";
                        if (openFileDialog.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                        string fileName = openFileDialog.FileName;
                        string text = Path.Combine(Global.cpd.where, Path.GetFileName(openFileDialog.FileName));
                        if (Path.GetDirectoryName(openFileDialog.FileName) != Global.cpd.where)
                        {
                            if (MessageBox.Show($"ファイルはプロジェクトディレクトリに含まれていません。{Environment.NewLine}プロジェクトディレクトリへコピーします。{Environment.NewLine}また、同じ名前のファイルがある場合は上書きされます。{Environment.NewLine}よろしいですか？", "ファイルの追加", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return;
                            }
                            try
                            {
                                File.Copy(fileName, text, true);
                            }
                            catch (IOException)
                            {
                                MessageBox.Show($"ファイルをコピーできませんでした。{Environment.NewLine}ファイルのコピー先を再指定してください。", "コピー失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                bool flag = false;
                                while (!flag)
                                {
                                    using SaveFileDialog saveFileDialog = new SaveFileDialog();
                                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(text);
                                    saveFileDialog.FileName = Path.GetFileName(text);
                                    saveFileDialog.Filter = "保存ファイル|*" + Path.GetExtension(fileName);
                                    saveFileDialog.AddExtension = true;
                                    saveFileDialog.DefaultExt = Path.GetExtension(fileName);
                                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                                    {
                                        return;
                                    }
                                    if (File.Exists(saveFileDialog.FileName))
                                    {
                                        MessageBox.Show($"上書きはできません。{Environment.NewLine}元のファイルを消すか、別のファイルを指定してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    }
                                    else if (Path.GetDirectoryName(saveFileDialog.FileName) != Global.cpd.where)
                                    {
                                        MessageBox.Show("プロジェクトディレクトリ内に保存してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    }
                                    else
                                    {
                                        text = saveFileDialog.FileName;
                                        flag = true;
                                    }
                                }
                            }
                        }
                        ConfView[e.ColumnIndex, e.RowIndex].Value = Path.GetFileName(text) + "...";
                        Global.cpd.project.Config.Configurations[num].Value = Path.GetFileName(text);
                        string relation;
                        if (Global.cpd.project.Config.Configurations[num].Relation != null && Global.cpd.project.Config.Configurations[num].Relation != "" && (relation = Global.cpd.project.Config.Configurations[num].Relation) != null)
                        {// バグ　編集していない方のレイヤーを半透明表示にしている際にそのレイヤーの画像を変更すると半透明表示が解除される
                            if (!(relation == "PATTERN"))
                            {
                                if (relation == "LAYERCHIP")
                                {
                                    Global.MainWnd.MainDesigner.PrepareImages();
                                    Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
                                    Global.MainWnd.MainDesigner.Refresh();
                                }
                            }
                            else
                            {
                                Global.MainWnd.MainDesigner.PrepareImages();
                                Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                        }
                        if (Global.cpd.project.Config.Configurations[num].Name == "filename_oriboss_left1")
                        {
                            Global.MainWnd.MainDesigner.PrepareImages();
                            Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                            Global.MainWnd.MainDesigner.Refresh();
                        }
                        if (Regex.IsMatch(Global.cpd.project.Config.Configurations[num].Name, "^(filename_haikei|filename_second_haikei|filename_chizu)"))
                        {
                            Global.MainWnd.MainDesigner.PrepareImages();
                            Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
                            Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
                            Global.MainWnd.MainDesigner.Refresh();
                        }
                        break;
                    }
                case ConfigParam.Types.l:
                case ConfigParam.Types.l_a:
                    return;
                case ConfigParam.Types.c:
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        Colors colors = new Colors(configParam.Value);
                        colorDialog.Color = colors.c;
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            colors.c = colorDialog.Color;
                            ConfView[e.ColumnIndex, e.RowIndex].Value = colors.ToString();
                            ConfView[e.ColumnIndex, e.RowIndex].Style.BackColor = colors.c;
                            Global.cpd.project.Config.Configurations[num].Value = colors.ToString();
                            if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND" && Global.state.EdittingStage == 0)
                            {
                                Global.state.Background = colors.c;
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                            else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND2" && Global.state.EdittingStage == 1)
                            {
                                Global.state.Background = colors.c;
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                            else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND3" && Global.state.EdittingStage == 2)
                            {
                                Global.state.Background = colors.c;
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                            else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND4" && Global.state.EdittingStage == 3)
                            {
                                Global.state.Background = colors.c;
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                            else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUNDM" && Global.state.EdittingStage == 4)
                            {
                                Global.state.Background = colors.c;
                                Global.MainWnd.MainDesigner.Refresh();
                            }
                        }
                        break;
                    }
                default:
                    return;
            }
            Global.state.EditFlag = true;
        }
        private void ConfView_PreviewAudio(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                return;
            }
            if (e.RowIndex >= OrigIdx.Count || e.RowIndex < 0)
            {
                return;
            }
            int num = OrigIdx[e.RowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
            if (configParam.Type == ConfigParam.Types.f_a)
            {
                if (now_playing_item != num)
                {
                    now_playing_item = num;
                    mediaPlayer.URL = Path.Combine(Global.cpd.where, configParam.Value);
                    mediaPlayer.controls.play();
                }
                else
                {
                    now_playing_item = -1;
                    mediaPlayer.controls.stop();
                }
            }
        }
        private void PreviewAudio_error(object pMediaObject)
        {
            MessageBox.Show("ファイルの読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private void ConfView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridViewTextBoxEditingControl dataGridViewTextBoxEditingControl = (DataGridViewTextBoxEditingControl)e.Control;
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

        private void ConfView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (ConfView.CurrentCellAddress.X == 1 && ConfView.IsCurrentCellDirty)
            {
                ConfView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ConfView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                return;
            }
            if (e.RowIndex >= OrigIdx.Count || e.RowIndex < 0)
            {
                return;
            }
            int num = OrigIdx[e.RowIndex];
            ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
            switch (configParam.Type)
            {
                case ConfigParam.Types.b:
                case ConfigParam.Types.b2:
                case ConfigParam.Types.b0:
                    if (configParam.Value == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                    {
                        return;
                    }
                    Global.cpd.project.Config.Configurations[num].Value = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
                    break;
                case ConfigParam.Types.s:
                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        ConfView[e.ColumnIndex, e.RowIndex].Value = "";
                    }
                    if (configParam.Value == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                    {
                        return;
                    }
                    Global.cpd.project.Config.Configurations[num].Value = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
                    break;
                case ConfigParam.Types.i:
                    {
                        int num2;
                        if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                        {
                            ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                        }
                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                        {
                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            if (configParam.Value.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                            {
                                return;
                            }
                            ConfView[e.ColumnIndex, e.RowIndex].Value = configParam.Value.ToString();
                            return;
                        }
                        else
                        {
                            Global.cpd.project.Config.Configurations[num].Value = num2.ToString();
                        }
                        break;
                    }
                case ConfigParam.Types.t:
                    {
                        if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                        {
                            ConfView[e.ColumnIndex, e.RowIndex].Value = "";
                        }
                        string text = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
                        string[] array = text.Split(new string[]
                        {
                    Environment.NewLine
                        }, StringSplitOptions.None);
                        if (configParam.Rows > 0)
                        {
                            List<string> list = new List<string>(array);
                            if (list.Count > configParam.Rows)
                            {
                                MessageBox.Show($"行数が最大値を超えています。{Environment.NewLine}超えた行は削除されます。", "行の超過", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                list.RemoveRange(configParam.Rows, list.Count - configParam.Rows);
                            }
                            else if (list.Count < configParam.Rows)
                            {
                                while (list.Count < configParam.Rows)
                                {
                                    list.Add("0");
                                }
                            }
                            array = list.ToArray();
                        }
                        text = string.Join(Environment.NewLine, array);
                        if (configParam.Value == text)
                        {
                            return;
                        }
                        Global.cpd.project.Config.Configurations[num].Value = text;
                        ConfView[e.ColumnIndex, e.RowIndex].Value = text;
                        break;
                    }
                case ConfigParam.Types.f:
                case ConfigParam.Types.f_i:
                case ConfigParam.Types.f_a:
                    return;
                case ConfigParam.Types.l:
                    if (configParam.Value == (((DataGridViewComboBoxCell)ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(ConfView[e.ColumnIndex, e.RowIndex].Value) + 1).ToString())
                    {
                        return;
                    }
                    Global.cpd.project.Config.Configurations[num].Value = (((DataGridViewComboBoxCell)ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(ConfView[e.ColumnIndex, e.RowIndex].Value) + 1).ToString();
                    if (Global.cpd.project.Config.Configurations[num].Name == "mcs_screen_size")
                    {
                        if (Global.cpd.project.Config.Configurations[num].Value == "1")
                        {
                            Global.cpd.project.Config.Configurations[width_index].Value = "640";
                            Global.cpd.project.Config.Configurations[height_index].Value = "480";
                            int tmp_y = Global.cpd.runtime.Definitions.MapSize.y;
                            Global.cpd.runtime.Definitions.MapSize.x = 19;
                            Global.cpd.runtime.Definitions.MapSize.y = 14;

                            // 予め地図画面の配列数を多めに取っておけば↓は不要

                            Array.Resize(ref Global.cpd.project.MapData, Global.cpd.runtime.Definitions.MapSize.y);
                            for (int i = 0; i < Global.cpd.runtime.Definitions.MapSize.y; i++)
                            {
                                int k;
                                if (i < tmp_y) k = Global.cpd.runtime.Definitions.MapSize.x - Global.cpd.project.MapData[i].Length;
                                else k = Global.cpd.runtime.Definitions.MapSize.x;
                                for (int j = 0; j < k; j++)
                                    Global.cpd.project.MapData[i] += ".";
                            }
                        }
                        else if (Global.cpd.project.Config.Configurations[num].Value == "2")
                        {
                            Global.cpd.project.Config.Configurations[width_index].Value = "512";
                            Global.cpd.project.Config.Configurations[height_index].Value = "320";
                            Global.cpd.runtime.Definitions.MapSize.x = 15;
                            Global.cpd.runtime.Definitions.MapSize.y = 9;
                        }
                    }
                    break;
                case ConfigParam.Types.l_a:
                    {
                        int configParam_num = ((DataGridViewComboBoxCell)ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(ConfView[e.ColumnIndex, e.RowIndex].Value) + 1;
                        int MaxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;
                        if (configParam_num <= MaxAthleticNumber && configParam.Value == configParam_num.ToString() || configParam_num > MaxAthleticNumber && configParam.Value == (configParam_num - 1 - MaxAthleticNumber + 1001).ToString())
                        {
                            return;
                        }
                        Global.cpd.project.Config.Configurations[num].Value = ((configParam_num <= MaxAthleticNumber) ? configParam_num : configParam_num - 1 - MaxAthleticNumber + 1001).ToString();
                        break;
                    }
                default:
                    return;
            }
            if (configParam.ChipRelation != "" && configParam.ChipRelation != null)
            {
                Global.state.ChipRegister[configParam.ChipRelation] = Global.cpd.project.Config.Configurations[num].Value;
                Global.MainWnd.RefreshAll();
            }
            Global.state.EditFlag = true;
        }

        private void ConfView_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void InitializeComponent()
        {
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
            ConfigSelector.Size = new Size(298, 20);
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
            ConfView.Columns.AddRange(new DataGridViewColumn[]
            {
                CNames,
                CValues
            });
            ConfView.Dock = DockStyle.Fill;
            ConfView.EditMode = DataGridViewEditMode.EditOnEnter;
            ConfView.Location = new Point(0, 20);
            ConfView.MultiSelect = false;
            ConfView.Name = "ConfView";
            ConfView.RowHeadersVisible = false;
            ConfView.RowTemplate.Height = 21;
            ConfView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ConfView.Size = new Size(298, 338);
            ConfView.TabIndex = 4;
            ConfView.CellValueChanged += ConfView_CellValueChanged;
            ConfView.CellClick += ConfView_CellClick;
            ConfView.EditingControlShowing += ConfView_EditingControlShowing;
            ConfView.CurrentCellDirtyStateChanged += ConfView_CurrentCellDirtyStateChanged;
            ConfView.CellContentClick += ConfView_CellContentClick;
            ConfView.CellDoubleClick += ConfView_PreviewAudio;

            CNames.HeaderText = "項目名";
            CNames.Name = "CNames";
            CNames.ReadOnly = true;
            CNames.SortMode = DataGridViewColumnSortMode.NotSortable;
            CValues.FillWeight = 50f;
            CValues.HeaderText = "値";
            CValues.Name = "CValues";
            CValues.SortMode = DataGridViewColumnSortMode.NotSortable;
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ConfView);
            Controls.Add(ConfigSelector);
            Name = "ConfigList";
            Size = new Size(298, 358);
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);

            // メディアプレーヤークラスのインスタンスを作成する
            mediaPlayer = new WindowsMediaPlayer();
            mediaPlayer.MediaError += PreviewAudio_error;
        }

        private readonly List<int> OrigIdx = new List<int>();

        private readonly IContainer components;

        private ComboBox ConfigSelector;

        private DataGridView ConfView;

        private DataGridViewTextBoxColumn CNames;

        private DataGridViewTextBoxColumn CValues;

        private int width_index, height_index;

        private WindowsMediaPlayer mediaPlayer;

        private int now_playing_item = -1;
    }
}

using MasaoPlus.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;
using System.Linq;

namespace MasaoPlus.Controls
{
    public class CustomPartsConfigList : ConfigList
    {
        private Panel buttonPanel;
        private Button btnCopyParts;
        private Button btnDeleteParts;
        private Button btnAddParts;  // 追加

        public override void Prepare()
        {
            BasePartsTypes = new DataGridViewComboBoxCell();
            BasePartsTypes.Items.Clear();
            CustomizeParts.Clear();

            for (int i = 0; i < Global.cpd.VarietyChip.Length; i++)
            {
                if (int.Parse(Global.cpd.VarietyChip[i].code) > 5000)
                {
                    BasePartsTypes.Items.Add($"{Global.cpd.VarietyChip[i].Chips[0].name} {Global.cpd.VarietyChip[i].Chips[0].description}");
                    CustomizeParts.Add(Global.cpd.VarietyChip[i]);
                }
            }
            PrepareCurrentCustomPartsParam();
            BasePartsTypes.FlatStyle = FlatStyle.Popup;
            ConfView[1, 1] = BasePartsTypes;
            
            UpdateControlStates();
        }

        private void UpdateControlStates()
        {
            bool enable = Global.cpd.project.Use3rdMapData && CustomizeParts.Count > 0;
            bool hasCustomParts = enable && Global.cpd.CustomPartsChip != null && Global.cpd.CustomPartsChip.Length > 0;
            
            btnAddParts.Enabled = enable;
            btnCopyParts.Enabled = hasCustomParts;
            btnDeleteParts.Enabled = hasCustomParts;
            
            buttonPanel.Enabled = enable;
            
            foreach (DataGridViewRow row in ConfView.Rows)
            {
                row.ReadOnly = !enable;
                if (row.Cells[1] is DataGridViewButtonCell buttonCell)
                {
                    buttonCell.FlatStyle = enable ? FlatStyle.Popup : FlatStyle.Standard;
                }
            }
        }

        public void PrepareCurrentCustomPartsParam()
        {
            for (int i = 0; i < CustomizeParts.Count; i++)
            {
                if (CustomizeParts[i].code == Global.state.CurrentCustomPartsChip.basecode)
                {
                    ConfView[1, 0].Value = Global.state.CurrentCustomPartsChip.Chips[0].name;
                    BasePartsTypes.Value = BasePartsTypes.Items[i].ToString();
                    break;
                }
            }
            ConfigSelector_SelectedIndexChanged(Global.state.CurrentCustomPartsChip);
        }

        // 表示を変える
        protected void ConfigSelector_SelectedIndexChanged(ChipsData c)
        {
            if (BasePartsTypes.Items.Count < 1)
            {
                return;
            }
            for (int i = ConfView.RowCount - 1; i > 1; i--)
            {
                ConfView.Rows.RemoveAt(i);
            }
            if (BasePartsTypes.Value != null)
            {
                PrepareCustomPartsParam(c, c.basecode);
            }
        }
        public void ConfigSelector_SelectedIndexChanged()
        {
            if (BasePartsTypes.Items.Count < 1)
            {
                return;
            }
            if (Global.cpd.CustomPartsChip == null || Global.cpd.CustomPartsChip.Length < 1)
            {
                ConfView[1, 0].Value = "";
                ConfView[1, 1].Value = "";
                ConfView[1, 0].ReadOnly = true;
                ConfView[1, 1].ReadOnly = true;
            }
            else
            {
                ConfView[1, 0].ReadOnly = false;
                ConfView[1, 1].ReadOnly = false;
            }
            for (int i = ConfView.RowCount - 1; i > 1; i--)
            {
                ConfView.Rows.RemoveAt(i);
            }
            for (int i = 0; i < CustomizeParts.Count; i++)
            {
                if (BasePartsTypes.Value != null && BasePartsTypes.Items.IndexOf(BasePartsTypes.Value) == i)
                {
                    ChipsData c;
                    if (CustomizeParts[i].code == Global.state.CurrentCustomPartsChip.basecode)
                    {
                        c = Global.state.CurrentCustomPartsChip;
                        PrepareCustomPartsParam(c, c.basecode);
                    }
                    else
                    {
                        c = CustomizeParts[i];
                        PrepareCustomPartsParam(c, c.code);
                    }
                    break;
                }
            }

            UpdateControlStates();
        }

        protected void PrepareCustomPartsParam(ChipsData c, string chipcode)
        {
            if (int.TryParse(chipcode, out int code))
            {
                if (code != 5701) // ヤチャモ（何もしない）を除く
                {
                    switch ((code - 5000) / 10)
                    {
                        case 10: // 亀（向きを変える）
                        case 40: // ヒノララシ
                        case 80: // ミズタロウ
                        case 120: // 追跡亀
                            ConfView.Rows.Add(
                            [
                                        "歩く速度",
                                        c.Properties.walk_speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.walk_speed.ToString()
                            };
                            break;
                        case 11: // 亀（落ちる）
                            ConfView.Rows.Add(
                            [
                                        "歩く速度",
                                        c.Properties.walk_speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.walk_speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "落ちる速度",
                                        c.Properties.fall_speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.fall_speed.ToString()
                            };
                            break;
                        case 20: // ピカチー
                            ConfView.Rows.Add(
                            [
                                        "ジャンプの初速",
                                        c.Properties.jump_vy.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.jump_vy.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "索敵範囲",
                                        c.Properties.search_range.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.search_range.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "行動間隔",
                                        c.Properties.interval.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.interval.ToString()
                            };
                            break;
                        case 30: // チコリン（はっぱカッター）
                            ConfView.Rows.Add(
                            [
                                "行動周期",
                                c.Properties.period.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.period.ToString()
                            };
                            for (int j = 0; j < c.Properties.attack_timing.Count; j++)
                            {
                                ConfView.Rows.Add(
                                [
                                    "葉っぱを投げるタイミング",
                                    c.Properties.attack_timing[j].AttackFrame.ToString()
                                ]);
                                ConfView[1, j + 3] = new DataGridViewNumericUpdownCell
                                {
                                    Value = c.Properties.attack_timing[j].AttackFrame.ToString()
                                };
                                DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new()
                                {
                                    TrueValue = "true",
                                    FalseValue = "false"
                                };
                                if (bool.TryParse(c.Properties.attack_timing[j].IsPlaySoundFrame.ToString(), out bool flag))
                                {
                                    dataGridViewCheckBoxCell.Value = flag.ToString();
                                }
                                else
                                {
                                    dataGridViewCheckBoxCell.Value = false;
                                }
                                ConfView[2, j + 3] = dataGridViewCheckBoxCell;
                                ConfView[2, j + 3].ReadOnly = false;
                            }
                            ConfView.Rows.Add(
                            [
                                "葉っぱを投げるタイミング",
                                "＋"
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            ConfView[1, ConfView.Rows.Count - 1].ReadOnly = true;
                            break;
                        case 31: // チコリン（敵を投げる）
                        case 33: // チコリン（ソーラービーム）
                        case 52: // ポッピー（火の粉）
                        case 54: // ポッピー（バブル光線３発）
                        case 55: // ポッピー（ハリケンブラスト）
                        case 70: // ヤチャモ（一定タイミングで攻撃）
                        case 71: // ヤチャモ（火の粉 速射 / 3連射）
                        case 72: // ヤチャモ（破壊光線）
                        case 92: // エアームズ（その場で投下）
                        case 110: // クラゲッソ（バブル光線 水中専用）
                            ConfView.Rows.Add(
                            [
                                        "行動間隔",
                                        c.Properties.interval.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.interval.ToString()
                            };
                            break;
                        case 32: // チコリン（はっぱカッター 乱れ射ち）
                        case 53: // ポッピー（火の粉 ３連射）
                            break;
                        case 50: // ポッピー（上下移動）
                            ConfView.Rows.Add(
                            [
                                        "飛行速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "折り返し時の加速度",
                                        c.Properties.accel.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.accel.ToString()
                            };
                            break;
                        case 51: // ポッピー（直進）
                        case 93: // エアームズ（左右に動いて爆弾投下）
                            ConfView.Rows.Add(
                            [
                                        "飛行速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            break;
                        case 100: // タイキング（左右移動 水中専用）
                        case 140: // 重力無視の追跡ピカチー等
                            ConfView.Rows.Add(
                            [
                                        "移動速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            break;
                        case 60: // マリリ（ジャンプ）
                            ConfView.Rows.Add(
                            [
                                        "横方向の移動速度",
                                        c.Properties.jump_vy.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.jump_vy.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "ジャンプの初速",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            break;
                        case 66: // マリリ（左右移動）
                            ConfView.Rows.Add(
                            [
                                        "移動速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "移動距離",
                                        c.Properties.distance.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.distance.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "行動間隔",
                                        c.Properties.interval.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.interval.ToString()
                            };
                            break;
                        case 67: // マリリ（体当たり）
                            ConfView.Rows.Add(
                            [
                                        "体当たりの速度",
                                        c.Properties.attack_speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.attack_speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "戻る速度",
                                        c.Properties.return_speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.return_speed.ToString()
                            };
                            break;
                        case 90: // エアームズ（壁に当たると止まる）
                        case 95: // エアームズ（壁に当たると向きを変える）
                            ConfView.Rows.Add(
                            [
                                        "飛行速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "行動間隔",
                                        c.Properties.interval.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.interval.ToString()
                            };
                            break;
                        case 105: // タイキング（はねる）
                            ConfView.Rows.Add(
                            [
                                        "ジャンプの初速",
                                        c.Properties.jump_vy.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.jump_vy.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "行動間隔",
                                        c.Properties.interval.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.interval.ToString()
                            };
                            break;
                        case 106: // タイキング（縄張りをまもる）
                        case 116: // クラゲッソ（近づくと落ちる）
                            ConfView.Rows.Add(
                            [
                                        "移動速度（横方向）",
                                        c.Properties.speed_x.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed_x.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "移動速度（縦方向）",
                                        c.Properties.speed_y.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed_y.ToString()
                            };
                            break;
                        case 107: // タイキング（左回り）
                        case 108: // タイキング（右回り）
                        case 117: // クラゲッソ（左回り）
                        case 118: // クラゲッソ（右回り）
                            ConfView.Rows.Add(
                            [
                                        "角速度",
                                        c.Properties.speed.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.speed.ToString()
                            };
                            ConfView.Rows.Add(
                            [
                                        "回転半径",
                                        c.Properties.radius.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.radius.ToString()
                            };
                            break;
                        case 115: // クラゲッソ（近づくと落ちる）
                            ConfView.Rows.Add(
                            [
                                        "落下の初速",
                                        c.Properties.init_vy.ToString()
                            ]);
                            ConfView[1, ConfView.Rows.Count - 1] = new DataGridViewNumericUpdownCell
                            {
                                Value = c.Properties.init_vy.ToString()
                            };
                            break;
                    }
                }
            }
        }

        protected void ConfView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                return;
            }
            int num = e.RowIndex;
            if (num < 3 || num > ConfView.Rows.Count - 2)
            {
                return;
            }
            int i, cs_i = Global.MainWnd.GuiCustomPartsChipList.SelectedIndex;
            for (i = 0; i < CustomizeParts.Count; i++)
            {
                if (i == BasePartsTypes.Items.IndexOf(BasePartsTypes.Value))
                {
                    break;
                }
            }
            ChipsData c = CustomizeParts[i];
            if (int.TryParse(c.code, out int code))
            {
                if ((code - 5000) / 10 == 30) // チコリン（はっぱカッター）
                {
                    ConfView.Rows.RemoveAt(num);
                    Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing.RemoveAt(num - 3);
                    Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[cs_i];
                    Global.state.EditFlag = true;
                }
            }
        }

        protected override void ConfView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (ConfView.CurrentCellAddress.X == 1 && ConfView.IsCurrentCellDirty)
            {
                ConfView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                if (ConfView.CurrentCellAddress.Y == 1)
                {
                    ConfigSelector_SelectedIndexChanged();
                }
            }
        }

        protected override void ConfView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                return;
            }
            if (e.RowIndex < 0)
            {
                return;
            }
            if (Global.cpd.CustomPartsChip == null || Global.cpd.CustomPartsChip.Length < 1)
            {
                return;
            }
            int num = e.RowIndex, num2;
            int i, cs_i = Global.MainWnd.GuiCustomPartsChipList.SelectedIndex;
            if (num == 0) // カスタムパーツの名前
            {
                if (e.ColumnIndex == 1)
                {
                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        ConfView[e.ColumnIndex, e.RowIndex].Value = "";
                    }
                    string row_text = ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        ConfView[e.ColumnIndex, e.RowIndex].Value = "";
                    }
                    if (Global.state.CurrentCustomPartsChip.GetCSChip().name == row_text)
                    {
                        return;
                    }
                    for (i = 0; i < Global.cpd.CustomPartsChip[cs_i].Chips.Length; i++)
                    {
                        Global.cpd.CustomPartsChip[cs_i].Chips[i].name = row_text;
                    }
                    Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[cs_i];
                    Global.MainWnd.MainDesigner.DrawItemCodeRef[Global.cpd.CustomPartsChip[cs_i].code] = Global.cpd.CustomPartsChip[cs_i];
                    Global.MainWnd.RefreshAll();
                }
            }
            else
            {
                for (i = 0; i < CustomizeParts.Count; i++)
                {
                    if (i == BasePartsTypes.Items.IndexOf(BasePartsTypes.Value))
                    {
                        break;
                    }
                }
                if (num == 1)
                {
                    if (e.ColumnIndex == 1)
                    {
                        if (Global.state.CurrentCustomPartsChip.basecode == CustomizeParts[i].code)
                        {
                            return;
                        }
                        string name_temp = Global.state.CurrentCustomPartsChip.GetCSChip().name;
                        string code_temp = Global.state.CurrentCustomPartsChip.code;
                        string color_temp = Global.state.CurrentCustomPartsChip.idColor;
                        Global.cpd.CustomPartsChip[cs_i] = CustomizeParts[i];
                        Global.cpd.CustomPartsChip[cs_i].Chips = (ChipData[])CustomizeParts[i].Chips.Clone(); // 配列は個別に複製
                        for (int j = 0; j < CustomizeParts[i].Chips.Length; j++)
                        {
                            Global.cpd.CustomPartsChip[cs_i].Chips[j].name = name_temp;
                            Global.cpd.CustomPartsChip[cs_i].Chips[j].description = $"{CustomizeParts[i].Chips[j].name} {CustomizeParts[i].Chips[j].description}";
                        }
                        Global.cpd.CustomPartsChip[cs_i].basecode = CustomizeParts[i].code;
                        Global.cpd.CustomPartsChip[cs_i].code = code_temp;
                        Global.cpd.CustomPartsChip[cs_i].idColor = color_temp;
                        Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[cs_i];
                        Global.MainWnd.MainDesigner.DrawItemCodeRef[Global.cpd.CustomPartsChip[cs_i].code] = Global.cpd.CustomPartsChip[cs_i];
                        Global.MainWnd.RefreshAll();
                    }
                }
                else
                {
                    ChipsData c = CustomizeParts[i];
                    if (int.TryParse(c.code, out int code))
                    {
                        if (code != 5701) // ヤチャモ（何もしない）を除く
                        {
                            switch ((code - 5000) / 10)
                            {
                                case 10: // 亀（向きを変える）
                                case 40: // ヒノララシ
                                case 80: // ミズタロウ
                                case 120: // 追跡亀
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                    {
                                        MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                        if (c.Properties.walk_speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                        {
                                            return;
                                        }
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.walk_speed.ToString();
                                        return;
                                    }
                                    else
                                    {
                                        Global.cpd.CustomPartsChip[cs_i].Properties.walk_speed = num2;
                                    }
                                    break;
                                case 11: // 亀（落ちる）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.walk_speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.walk_speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.walk_speed = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.fall_speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.fall_speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.fall_speed = num2;
                                        }
                                    }
                                    break;
                                case 20: // ピカチー
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.jump_vy.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.jump_vy.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.jump_vy = num2;
                                        }
                                    }
                                    else if (num == 3)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.search_range.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.search_range.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.search_range = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.interval.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.interval.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.interval = num2;
                                        }
                                    }
                                    break;
                                case 30: // チコリン（はっぱカッター）
                                    if (num == 2)
                                    {
                                        if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                        {
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                        }
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.period.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.period.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.period = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (e.ColumnIndex == 1)
                                        {
                                            if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                            {
                                                ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                            }
                                            if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                            {
                                                MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                                if (c.Properties.attack_timing[e.RowIndex - 3].AttackFrame.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                                {
                                                    return;
                                                }
                                                ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.attack_timing[e.RowIndex - 3].AttackFrame.ToString();
                                                return;
                                            }
                                            else
                                            {
                                                var attack_timing = Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing[e.RowIndex - 3];
                                                attack_timing.AttackFrame = num2;
                                                Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing[e.RowIndex - 3] = attack_timing;
                                            }
                                        }
                                        else
                                        {
                                            if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                            {
                                                ConfView[e.ColumnIndex, e.RowIndex].Value = false.ToString();
                                            }
                                            if (!bool.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out bool flag))
                                            {
                                                MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                                if (c.Properties.attack_timing[e.RowIndex - 3].IsPlaySoundFrame.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                                {
                                                    return;
                                                }
                                                ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.attack_timing[e.RowIndex - 3].IsPlaySoundFrame.ToString();
                                                return;
                                            }
                                            else
                                            {
                                                var attack_timing = Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing[e.RowIndex - 3];
                                                attack_timing.IsPlaySoundFrame = flag;
                                                Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing[e.RowIndex - 3] = attack_timing;
                                            }
                                        }
                                    }
                                    break;
                                case 31: // チコリン（敵を投げる）
                                case 33: // チコリン（ソーラービーム）
                                case 52: // ポッピー（火の粉）
                                case 54: // ポッピー（バブル光線３発）
                                case 55: // ポッピー（ハリケンブラスト）
                                case 70: // ヤチャモ（一定タイミングで攻撃）
                                case 71: // ヤチャモ（火の粉 速射 / 3連射）
                                case 72: // ヤチャモ（破壊光線）
                                case 92: // エアームズ（その場で投下）
                                case 110: // クラゲッソ（バブル光線 水中専用）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                    {
                                        MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                        if (c.Properties.interval.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                        {
                                            return;
                                        }
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.interval.ToString();
                                        return;
                                    }
                                    else
                                    {
                                        Global.cpd.CustomPartsChip[cs_i].Properties.interval = num2;
                                    }
                                    break;
                                case 32: // チコリン（はっぱカッター 乱れ射ち）
                                case 53: // ポッピー（火の粉 ３連射）
                                    break;
                                case 50: // ポッピー（上下移動）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.accel.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.accel.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.accel = num2;
                                        }
                                    }
                                    break;
                                case 51: // ポッピー（直進）
                                case 93: // エアームズ（左右に動いて爆弾投下）
                                case 100: // タイキング（左右移動 水中専用）
                                case 140: // 重力無視の追跡ピカチー等
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                    {
                                        MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                        if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                        {
                                            return;
                                        }
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                        return;
                                    }
                                    else
                                    {
                                        Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                    }
                                    break;
                                case 60: // マリリ（ジャンプ）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.jump_vy.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.jump_vy.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.jump_vy = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                        }
                                    }
                                    break;
                                case 66: // マリリ（左右移動）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                        }
                                    }
                                    else if (num == 3)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.distance.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.distance.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.distance = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.interval.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.interval.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.interval = num2;
                                        }
                                    }
                                    break;
                                case 67: // マリリ（体当たり）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.attack_speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.attack_speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.attack_speed = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.return_speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.return_speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.return_speed = num2;
                                        }
                                    }
                                    break;
                                case 90: // エアームズ（壁に当たると止まる）
                                case 95: // エアームズ（壁に当たると向きを変える）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.interval.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.interval.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.interval = num2;
                                        }
                                    }
                                    break;
                                case 105: // タイキング（はねる）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.jump_vy.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.jump_vy.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.jump_vy = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.interval.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.interval.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.interval = num2;
                                        }
                                    }
                                    break;
                                case 106: // タイキング（縄張りをまもる）
                                case 116: // クラゲッソ（近づくと落ちる）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed_x.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed_x.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed_x = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed_y.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed_y.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed_y = num2;
                                        }
                                    }
                                    break;
                                case 107: // タイキング（左回り）
                                case 108: // タイキング（右回り）
                                case 117: // クラゲッソ（左回り）
                                case 118: // クラゲッソ（右回り）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (num == 2)
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.speed.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.speed.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.speed = num2;
                                        }
                                    }
                                    else
                                    {
                                        if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                        {
                                            MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                            if (c.Properties.radius.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                            {
                                                return;
                                            }
                                            ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.radius.ToString();
                                            return;
                                        }
                                        else
                                        {
                                            Global.cpd.CustomPartsChip[cs_i].Properties.radius = num2;
                                        }
                                    }
                                    break;
                                case 115: // クラゲッソ（近づくと落ちる）
                                    if (ConfView[e.ColumnIndex, e.RowIndex].Value == null)
                                    {
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                                    }
                                    if (!int.TryParse(ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
                                    {
                                        MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                        if (c.Properties.init_vy.ToString() == ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
                                        {
                                            return;
                                        }
                                        ConfView[e.ColumnIndex, e.RowIndex].Value = c.Properties.init_vy.ToString();
                                        return;
                                    }
                                    else
                                    {
                                        Global.cpd.CustomPartsChip[cs_i].Properties.init_vy = num2;
                                    }
                                    break;
                            }
                            Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[cs_i];
                        }
                    }
                }
            }
            Global.state.EditFlag = true;
        }

        protected override void ConfView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                return;
            }
            if (Global.cpd.CustomPartsChip == null || Global.cpd.CustomPartsChip.Length < 1)
            {
                return;
            }
            ConfView.BeginEdit(true);
            int num = e.RowIndex;
            if (num != ConfView.Rows.Count - 1)
            {
                return;
            }
            int i, cs_i = Global.MainWnd.GuiCustomPartsChipList.SelectedIndex;
            for (i = 0; i < CustomizeParts.Count; i++)
            {
                if (i == BasePartsTypes.Items.IndexOf(BasePartsTypes.Value))
                {
                    break;
                }
            }
            ChipsData c = CustomizeParts[i];
            if (int.TryParse(c.code, out int code))
            {
                if ((code - 5000) / 10 == 30) // チコリン（はっぱカッター）
                {
                    int next = 0;
                    if (int.TryParse(ConfView[e.ColumnIndex, num - 1].Value.ToString(), out int num2))
                    {
                        next = num2 + 1;
                    }
                    ConfView.Rows.Insert(ConfView.Rows.Count - 1,
                    [
                        "葉っぱを投げるタイミング",
                        next.ToString()
                    ]);
                    ConfView[1, ConfView.Rows.Count - 2] = new DataGridViewNumericUpdownCell
                    {
                        Value = next.ToString()
                    };
                    DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new()
                    {
                        TrueValue = "true",
                        FalseValue = "false",
                        Value = false
                    };
                    ConfView[2, ConfView.Rows.Count - 2] = dataGridViewCheckBoxCell;
                    Global.cpd.CustomPartsChip[cs_i].Properties.attack_timing.Add(new attack_timing { AttackFrame = next, IsPlaySoundFrame = false });
                    Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[cs_i];
                    Global.state.EditFlag = true;
                }
            }
        }

        protected override void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            buttonPanel = new Panel();
            btnCopyParts = new Button();
            btnDeleteParts = new Button();
            btnAddParts = new Button();
            ConfView = new DataGridView();
            CNames = new DataGridViewTextBoxColumn();
            CValues = new DataGridViewTextBoxColumn();
            SE = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)ConfView).BeginInit();
            SuspendLayout();

            // ボタンパネルの設定
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = LogicalToDeviceUnits(30);
            buttonPanel.Padding = new Padding(2);

            // ボタンの設定
            int buttonWidth = LogicalToDeviceUnits(70);
            int buttonHeight = LogicalToDeviceUnits(24);
            int buttonSpacing = LogicalToDeviceUnits(2);

            btnAddParts.Text = "追加";
            btnAddParts.Size = new Size(buttonWidth, buttonHeight);
            btnAddParts.Location = new Point(buttonSpacing, LogicalToDeviceUnits(3));
            btnAddParts.Image = new IconImageView(DeviceDpi, Resources.shape_square_add).View();
            btnAddParts.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAddParts.Click += BtnAddParts_Click;

            btnCopyParts.Text = "複製";
            btnCopyParts.Size = new Size(buttonWidth, buttonHeight);
            btnCopyParts.Location = new Point(buttonWidth + buttonSpacing * 2, LogicalToDeviceUnits(3));
            btnCopyParts.Image = new IconImageView(DeviceDpi, Resources.copy).View();
            btnCopyParts.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCopyParts.Click += BtnCopyParts_Click;

            btnDeleteParts.Text = "削除";
            btnDeleteParts.Size = new Size(buttonWidth, buttonHeight);
            btnDeleteParts.Location = new Point((buttonWidth + buttonSpacing) * 2 + buttonSpacing, LogicalToDeviceUnits(3));
            btnDeleteParts.Image = new IconImageView(DeviceDpi, Resources.cross).View();
            btnDeleteParts.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDeleteParts.Click += BtnDeleteParts_Click;

            buttonPanel.Controls.AddRange([btnAddParts, btnCopyParts, btnDeleteParts]);

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
                SE
            ]);
            ConfView.Dock = DockStyle.Fill;
            ConfView.EditMode = DataGridViewEditMode.EditOnEnter;
            ConfView.Location = new Point(0, LogicalToDeviceUnits(20));
            ConfView.MultiSelect = false;
            ConfView.Name = "CustomPartsConfView";
            ConfView.RowHeadersVisible = false;
            ConfView.RowTemplate.Height = 21;
            ConfView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ConfView.Size = LogicalToDeviceUnits(new Size(298, 338));
            ConfView.TabIndex = 4;
            ConfView.CellValueChanged += ConfView_CellValueChanged;
            ConfView.CellClick += ConfView_CellClick;
            ConfView.EditingControlShowing += ConfView_EditingControlShowing;
            ConfView.CurrentCellDirtyStateChanged += ConfView_CurrentCellDirtyStateChanged;
            ConfView.CellDoubleClick += ConfView_CellDoubleClick;

            CNames.FillWeight = 30f;
            CNames.HeaderText = "項目名";
            CNames.Name = "CNames";
            CNames.ReadOnly = true;
            CNames.SortMode = DataGridViewColumnSortMode.NotSortable;
            CValues.FillWeight = 50f;
            CValues.HeaderText = "値";
            CValues.Name = "CValues";
            CValues.SortMode = DataGridViewColumnSortMode.NotSortable;
            SE.FillWeight = 15f;
            SE.HeaderText = "効果音";
            SE.Name = "SE";
            SE.SortMode = DataGridViewColumnSortMode.NotSortable;
            SE.ReadOnly = true;
            ConfView.Rows.Add(
            [
                "カスタムパーツ名",
                "名前"
            ]);
            ConfView.Rows.Add(
            [
                "ベース",
                "ベースパーツ名"
            ]);
            ConfView[1, 0].Value = "";
            ConfView[1, 1].Value = "";
            ConfView[1, 0].ReadOnly = true;
            ConfView[1, 1].ReadOnly = true;

            //AutoScaleDimensions = new SizeF(6f, 12f);
            //AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ConfView);
            Controls.Add(buttonPanel);
            Controls.Add(ConfigSelector);
            Name = "CustomPartsConfigList";
            Size = LogicalToDeviceUnits(new Size(298, 358));
            ((ISupportInitialize)ConfView).EndInit();
            ResumeLayout(false);
        }

        private readonly List<ChipsData> CustomizeParts = [];

        private DataGridViewComboBoxCell BasePartsTypes;

        private DataGridViewTextBoxColumn SE;

        private void BtnCopyParts_Click(object sender, EventArgs e)
        {
            string name = $"{Global.state.CurrentCustomPartsChip.Chips[0].name}（コピー）";
            CopyCurrentParts(name);
        }

        private void BtnDeleteParts_Click(object sender, EventArgs e)
        {
            DeleteCurrentParts();
        }

        private void BtnAddParts_Click(object sender, EventArgs e)
        {
            // 最初のカスタムパーツ対象のチップを探す
            for (int i = 0; i < Global.cpd.VarietyChip.Length; i++)
            {
                if (int.Parse(Global.cpd.VarietyChip[i].code) > 5000)
                {
                    int num2 = Global.cpd.CustomPartsChip?.Length ?? 0;
                    CreateNewParts(Global.cpd.VarietyChip[i], $"カスタムパーツ{num2 + 1}");
                    break;
                }
            }
        }

        private void CopyCurrentParts(string name)
        {
            if (Global.cpd.CustomPartsChip == null)
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, 1);
            }
            else
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, Global.cpd.CustomPartsChip.Length + 1);
            }

            ChipsData basedata = Global.state.CurrentCustomPartsChip;
            Global.cpd.CustomPartsChip[^1] = basedata;
            Global.cpd.CustomPartsChip[^1].Chips = (ChipData[])basedata.Chips.Clone();

            var r = new Random();
            const string PWS_CHARS = "abcdefghijklmnopqrstuvwxyz";

            for (int j = 0; j < Global.cpd.CustomPartsChip[^1].Chips.Length; j++)
            {
                Global.cpd.CustomPartsChip[^1].Chips[j].name = name;
            }

            Global.cpd.CustomPartsChip[^1].basecode = basedata.basecode;
            Global.cpd.CustomPartsChip[^1].code = string.Join("", Enumerable.Range(0, 10).Select(_ => PWS_CHARS[r.Next(PWS_CHARS.Length)]));
            Global.cpd.CustomPartsChip[^1].idColor = $"#{Guid.NewGuid().ToString("N")[..6]}";

            Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[^1];
            Global.MainWnd.MainDesigner.DrawItemCodeRef[Global.state.CurrentCustomPartsChip.code] = Global.state.CurrentCustomPartsChip;
            Global.cpd.project.CustomPartsDefinition = Global.cpd.CustomPartsChip;

            Global.MainWnd.RefreshAll();
            Global.state.EditFlag = true;
            ConfigSelector_SelectedIndexChanged();
        }

        public void DeleteCurrentParts()
        {
            string currentCode = Global.state.CurrentCustomPartsChip.code;
            int currentIndex = Array.FindIndex(Global.cpd.CustomPartsChip, x => x.code == currentCode);

            if (currentIndex == -1) return;

            void UpdateStageData(LayerObject stagedata)
            {
                for (int i = 0; i < stagedata.Length; i++)
                {
                    stagedata[i] = stagedata[i].Replace(currentCode, "0");
                }
            }

            UpdateStageData(Global.cpd.project.StageData);
            UpdateStageData(Global.cpd.project.StageData2);
            UpdateStageData(Global.cpd.project.StageData3);
            UpdateStageData(Global.cpd.project.StageData4);

            Global.MainWnd.MainDesigner.DrawItemCodeRef.Remove(currentCode);
            Global.cpd.CustomPartsChip = [.. Global.cpd.CustomPartsChip.Where((_, index) => index != currentIndex)];

            if (Global.cpd.CustomPartsChip.Length > 0)
            {
                Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[0];
            }
            else
            {
                Global.state.CurrentCustomPartsChip = default;
            }

            Global.cpd.project.CustomPartsDefinition = Global.cpd.CustomPartsChip;
            Global.MainWnd.RefreshAll();
            Global.state.EditFlag = true;
            ConfigSelector_SelectedIndexChanged();
        }

        public void CreateNewParts(ChipsData basedata, string name)
        {
            // GUICustomPartsChipList.Create()の実装をこちらに移動
            ChipsData data;
            int i;
            if (basedata.basecode != null)
            {
                for (i = 0; i < Global.cpd.VarietyChip.Length; i++)
                {
                    if (Global.cpd.VarietyChip[i].code == basedata.basecode)
                    {
                        break;
                    }
                }
                data = Global.cpd.VarietyChip[i];
            }
            else
            {
                data = basedata;
            }

            if (Global.cpd.CustomPartsChip == null)
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, 1);
            }
            else
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, Global.cpd.CustomPartsChip.Length + 1);
            }

            Global.cpd.CustomPartsChip[^1] = data;
            Global.cpd.CustomPartsChip[^1].Chips = (ChipData[])data.Chips.Clone();

            var r = new Random();
            const string PWS_CHARS = "abcdefghijklmnopqrstuvwxyz";

            for (int j = 0; j < Global.cpd.CustomPartsChip[^1].Chips.Length; j++)
            {
                Global.cpd.CustomPartsChip[^1].Chips[j].name = name;
            }

            Global.cpd.CustomPartsChip[^1].basecode = data.basecode;
            Global.cpd.CustomPartsChip[^1].code = string.Join("", Enumerable.Range(0, 10).Select(_ => PWS_CHARS[r.Next(PWS_CHARS.Length)]));
            Global.cpd.CustomPartsChip[^1].idColor = $"#{Guid.NewGuid().ToString("N")[..6]}";

            Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[^1];
            Global.MainWnd.MainDesigner.DrawItemCodeRef[Global.state.CurrentCustomPartsChip.code] = Global.state.CurrentCustomPartsChip;
            Global.cpd.project.CustomPartsDefinition = Global.cpd.CustomPartsChip;

            Global.MainWnd.RefreshAll();
            Global.state.EditFlag = true;
            ConfigSelector_SelectedIndexChanged();
        }

        // 右クリックメニューなどから呼ばれる
        public void CreateOrCopyParts(bool isCopy = false)
        {
            if (isCopy && (Global.cpd.CustomPartsChip == null || Global.cpd.CustomPartsChip.Length < 1))
                return;

            if (isCopy)
            {
                string name = $"{Global.state.CurrentCustomPartsChip.Chips[0].name}（コピー）";
                CopyCurrentParts(name);
            }
            else
            {
                // 最初のカスタムパーツ対象のチップを探す
                for (int i = 0; i < Global.cpd.VarietyChip.Length; i++)
                {
                    if (int.Parse(Global.cpd.VarietyChip[i].code) > 5000)
                    {
                        int num2 = Global.cpd.CustomPartsChip?.Length ?? 0;
                        CreateNewParts(Global.cpd.VarietyChip[i], $"カスタムパーツ{num2 + 1}");
                        break;
                    }
                }
            }
        }
    }
}

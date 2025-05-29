using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace MasaoPlus.Controls
{
    public class DataGridViewNumericUpdownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        public DataGridViewNumericUpdownEditingControl()
        {
            TabStop = false;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
            DataGridViewContentAlignment alignment = dataGridViewCellStyle.Alignment;
            if (alignment <= DataGridViewContentAlignment.MiddleCenter)
            {
                switch (alignment)
                {
                    case DataGridViewContentAlignment.TopCenter:
                        break;
                    case (DataGridViewContentAlignment)3:
                        goto IL_72;
                    case DataGridViewContentAlignment.TopRight:
                        goto IL_6A;
                    default:
                        if (alignment != DataGridViewContentAlignment.MiddleCenter)
                        {
                            goto IL_72;
                        }
                        break;
                }
            }
            else
            {
                if (alignment == DataGridViewContentAlignment.MiddleRight)
                {
                    goto IL_6A;
                }
                if (alignment != DataGridViewContentAlignment.BottomCenter)
                {
                    if (alignment != DataGridViewContentAlignment.BottomRight)
                    {
                        goto IL_72;
                    }
                    goto IL_6A;
                }
            }
            TextAlign = HorizontalAlignment.Center;
            return;
        IL_6A:
            TextAlign = HorizontalAlignment.Right;
            return;
        IL_72:
            TextAlign = HorizontalAlignment.Left;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object EditingControlFormattedValue
        {
            get
            {
                return GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                Text = (string)value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return (keyData & Keys.KeyCode) switch
            {
                Keys.End or Keys.Home or Keys.Left or Keys.Up or Keys.Right or Keys.Down => true,
                _ => false,
            };
        }

        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Value.ToString();
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            Focus();
        }

        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            valueChanged = true;
            dataGridView.NotifyCurrentCellDirty(true);
        }

        private DataGridView dataGridView;

        private int rowIndex;

        private bool valueChanged;
    }
}

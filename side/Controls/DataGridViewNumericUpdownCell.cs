using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
    public class DataGridViewNumericUpdownCell : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            if (DataGridView.EditingControl is DataGridViewNumericUpdownEditingControl dataGridViewNumericUpdownEditingControl)
            {
                if (OwningColumn is not DataGridViewNumericUpdownColumn dataGridViewNumericUpdownColumn)
                {
                    dataGridViewNumericUpdownEditingControl.Minimum = -9999999m;
                    dataGridViewNumericUpdownEditingControl.Maximum = 9999999m;
                }
                else
                {
                    dataGridViewNumericUpdownEditingControl.Maximum = dataGridViewNumericUpdownColumn.Max;
                    dataGridViewNumericUpdownEditingControl.Minimum = dataGridViewNumericUpdownColumn.Min;
                }
                dataGridViewNumericUpdownEditingControl.Value = (Value != dataGridViewNumericUpdownEditingControl) ? int.Parse(Value.ToString()) : 0;
            }
        }

        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewNumericUpdownEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(object);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return 0;
            }
        }
    }
}

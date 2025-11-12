using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace MasaoPlus.Controls
{
    public class DataGridViewNumericUpdownColumn : DataGridViewColumn
    {
        public DataGridViewNumericUpdownColumn() : base(new DataGridViewNumericUpdownCell())
        {
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value is not DataGridViewNumericUpdownCell)
                {
                    throw new InvalidCastException("DataGridViewNumericUpdownCellオブジェクトを指定してください。");
                }
                base.CellTemplate = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Max
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Min
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
            }
        }

        public override object Clone()
        {
            DataGridViewNumericUpdownColumn dataGridViewNumericUpdownColumn = (DataGridViewNumericUpdownColumn)base.Clone();
            dataGridViewNumericUpdownColumn.Max = Max;
            dataGridViewNumericUpdownColumn.Min = Min;
            return dataGridViewNumericUpdownColumn;
        }

        private int maxValue = 9999999;

        private int minValue = -9999999;
    }
}

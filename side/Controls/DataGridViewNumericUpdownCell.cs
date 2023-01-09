using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	public class DataGridViewNumericUpdownCell : DataGridViewTextBoxCell
	{
		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
			DataGridViewNumericUpdownEditingControl dataGridViewNumericUpdownEditingControl = base.DataGridView.EditingControl as DataGridViewNumericUpdownEditingControl;
			if (dataGridViewNumericUpdownEditingControl != null)
			{
				DataGridViewNumericUpdownColumn dataGridViewNumericUpdownColumn = base.OwningColumn as DataGridViewNumericUpdownColumn;
				if (dataGridViewNumericUpdownColumn == null)
				{
					dataGridViewNumericUpdownEditingControl.Minimum = -9999999m;
					dataGridViewNumericUpdownEditingControl.Maximum = 9999999m;
				}
				else
				{
					dataGridViewNumericUpdownEditingControl.Maximum = dataGridViewNumericUpdownColumn.Max;
					dataGridViewNumericUpdownEditingControl.Minimum = dataGridViewNumericUpdownColumn.Min;
				}
				dataGridViewNumericUpdownEditingControl.Value = ((base.Value != dataGridViewNumericUpdownEditingControl) ? int.Parse(base.Value.ToString()) : 0);
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

using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	public class DataGridViewNumericUpdownColumn : DataGridViewColumn
	{
		public DataGridViewNumericUpdownColumn() : base(new DataGridViewNumericUpdownCell())
		{
		}

		public override DataGridViewCell CellTemplate
		{
			get
			{
				return base.CellTemplate;
			}
			set
			{
				if (!(value is DataGridViewNumericUpdownCell))
				{
					throw new InvalidCastException("DataGridViewNumericUpdownCellオブジェクトを指定してください。");
				}
				base.CellTemplate = value;
			}
		}

		public int Max
		{
			get
			{
				return this.maxValue;
			}
			set
			{
				this.maxValue = value;
			}
		}

		public int Min
		{
			get
			{
				return this.minValue;
			}
			set
			{
				this.minValue = value;
			}
		}

		public override object Clone()
		{
			DataGridViewNumericUpdownColumn dataGridViewNumericUpdownColumn = (DataGridViewNumericUpdownColumn)base.Clone();
			dataGridViewNumericUpdownColumn.Max = this.Max;
			dataGridViewNumericUpdownColumn.Min = this.Min;
			return dataGridViewNumericUpdownColumn;
		}

		private int maxValue = 9999999;

		private int minValue = -9999999;
	}
}

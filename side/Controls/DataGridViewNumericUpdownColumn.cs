using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	// Token: 0x02000009 RID: 9
	public class DataGridViewNumericUpdownColumn : DataGridViewColumn
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00002441 File Offset: 0x00000641
		public DataGridViewNumericUpdownColumn() : base(new DataGridViewNumericUpdownCell())
		{
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002464 File Offset: 0x00000664
		// (set) Token: 0x06000051 RID: 81 RVA: 0x0000246C File Offset: 0x0000066C
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002488 File Offset: 0x00000688
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002490 File Offset: 0x00000690
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002499 File Offset: 0x00000699
		// (set) Token: 0x06000055 RID: 85 RVA: 0x000024A1 File Offset: 0x000006A1
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

		// Token: 0x06000056 RID: 86 RVA: 0x0000B310 File Offset: 0x00009510
		public override object Clone()
		{
			DataGridViewNumericUpdownColumn dataGridViewNumericUpdownColumn = (DataGridViewNumericUpdownColumn)base.Clone();
			dataGridViewNumericUpdownColumn.Max = this.Max;
			dataGridViewNumericUpdownColumn.Min = this.Min;
			return dataGridViewNumericUpdownColumn;
		}

		// Token: 0x04000074 RID: 116
		private int maxValue = 9999999;

		// Token: 0x04000075 RID: 117
		private int minValue = -9999999;
	}
}

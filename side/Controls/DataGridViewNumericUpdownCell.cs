using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	// Token: 0x0200000A RID: 10
	public class DataGridViewNumericUpdownCell : DataGridViewTextBoxCell
	{
		// Token: 0x06000058 RID: 88 RVA: 0x0000B344 File Offset: 0x00009544
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000024B2 File Offset: 0x000006B2
		public override Type EditType
		{
			get
			{
				return typeof(DataGridViewNumericUpdownEditingControl);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000024BE File Offset: 0x000006BE
		public override Type ValueType
		{
			get
			{
				return typeof(object);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600005B RID: 91 RVA: 0x000024CA File Offset: 0x000006CA
		public override object DefaultNewRowValue
		{
			get
			{
				return 0;
			}
		}
	}
}

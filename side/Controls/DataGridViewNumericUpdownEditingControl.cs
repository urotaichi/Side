using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	// Token: 0x0200000B RID: 11
	public class DataGridViewNumericUpdownEditingControl : NumericUpDown, IDataGridViewEditingControl
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000024D2 File Offset: 0x000006D2
		public DataGridViewNumericUpdownEditingControl()
		{
			base.TabStop = false;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000B3E8 File Offset: 0x000095E8
		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			this.Font = dataGridViewCellStyle.Font;
			this.ForeColor = dataGridViewCellStyle.ForeColor;
			this.BackColor = dataGridViewCellStyle.BackColor;
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
			base.TextAlign = HorizontalAlignment.Center;
			return;
			IL_6A:
			base.TextAlign = HorizontalAlignment.Right;
			return;
			IL_72:
			base.TextAlign = HorizontalAlignment.Left;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000024E1 File Offset: 0x000006E1
		// (set) Token: 0x0600005F RID: 95 RVA: 0x000024E9 File Offset: 0x000006E9
		public DataGridView EditingControlDataGridView
		{
			get
			{
				return this.dataGridView;
			}
			set
			{
				this.dataGridView = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000024F2 File Offset: 0x000006F2
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000024FB File Offset: 0x000006FB
		public object EditingControlFormattedValue
		{
			get
			{
				return this.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
			}
			set
			{
				this.Text = (string)value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002509 File Offset: 0x00000709
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002511 File Offset: 0x00000711
		public int EditingControlRowIndex
		{
			get
			{
				return this.rowIndex;
			}
			set
			{
				this.rowIndex = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000251A File Offset: 0x0000071A
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00002522 File Offset: 0x00000722
		public bool EditingControlValueChanged
		{
			get
			{
				return this.valueChanged;
			}
			set
			{
				this.valueChanged = value;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000B470 File Offset: 0x00009670
		public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			switch (keyData & Keys.KeyCode)
			{
			case Keys.End:
			case Keys.Home:
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
			case Keys.Down:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000252B File Offset: 0x0000072B
		public Cursor EditingPanelCursor
		{
			get
			{
				return base.Cursor;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000B4AC File Offset: 0x000096AC
		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return base.Value.ToString();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002533 File Offset: 0x00000733
		public void PrepareEditingControlForEdit(bool selectAll)
		{
			base.Focus();
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600006A RID: 106 RVA: 0x0000253C File Offset: 0x0000073C
		public bool RepositionEditingControlOnValueChange
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000253F File Offset: 0x0000073F
		protected override void OnValueChanged(EventArgs e)
		{
			base.OnValueChanged(e);
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
		}

		// Token: 0x04000076 RID: 118
		private DataGridView dataGridView;

		// Token: 0x04000077 RID: 119
		private int rowIndex;

		// Token: 0x04000078 RID: 120
		private bool valueChanged;
	}
}

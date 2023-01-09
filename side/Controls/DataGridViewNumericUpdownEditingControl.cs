using System;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	public class DataGridViewNumericUpdownEditingControl : NumericUpDown, IDataGridViewEditingControl
	{
		public DataGridViewNumericUpdownEditingControl()
		{
			base.TabStop = false;
		}

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

		public Cursor EditingPanelCursor
		{
			get
			{
				return base.Cursor;
			}
		}

		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return base.Value.ToString();
		}

		public void PrepareEditingControlForEdit(bool selectAll)
		{
			base.Focus();
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
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
		}

		private DataGridView dataGridView;

		private int rowIndex;

		private bool valueChanged;
	}
}

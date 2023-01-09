using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Controls;

namespace MasaoPlus
{
	public partial class OverViewWindow : Form
	{
		public OverViewWindow()
		{
			this.InitializeComponent();
			this.UpdateView();
			Global.MainWnd.MainDesigner.ChangeBufferInvoke += this.MainDesigner_ChangeBufferInvoke;
			Global.MainWnd.MainDesignerScroll += this.MainWnd_MainDesignerScroll;
		}

		private void MainWnd_MainDesignerScroll()
		{
			this.UpdateView();
		}

		private void MainDesigner_ChangeBufferInvoke()
		{
			this.UpdateView();
		}

		public void UpdateView()
		{
			this.OverViewViewer.UpdateDrawSource();
			this.mWndSize = new Size((int)((double)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * this.OverViewViewer.ppb) / Global.config.draw.ZoomIndex), (int)((double)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * this.OverViewViewer.ppb) / Global.config.draw.ZoomIndex));
			this.cPoint = new Point(this.mWndSize.Width / 2, this.mWndSize.Height / 2);
		}

		private void OverViewWindow_Load(object sender, EventArgs e)
		{
			base.ClientSize = this.OverViewViewer.Size;
			if (Global.MainWnd.ovw == null)
			{
				this.Text += "(Spaceで閉じる)";
			}
		}

		private void OverViewWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None)
			{
				Keys keyCode = e.KeyCode;
				if (keyCode != Keys.Space)
				{
					return;
				}
				if (Global.MainWnd.ovw == null)
				{
					base.Close();
				}
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData)
			{
			case Keys.Left:
				Global.MainWnd.MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width * -1, 0));
				break;
			case Keys.Up:
				Global.MainWnd.MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height * -1));
				break;
			case Keys.Right:
				Global.MainWnd.MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width, 0));
				break;
			case Keys.Down:
				Global.MainWnd.MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height));
				break;
			default:
				return base.ProcessDialogKey(keyData);
			}
			this.Refresh();
			return true;
		}

		private void OverViewViewer_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Point mapPointTranslatedMap = new Point((e.X - this.cPoint.X) / this.OverViewViewer.ppb, (e.Y - this.cPoint.Y) / this.OverViewViewer.ppb);
				Global.state.MapPointTranslatedMap = mapPointTranslatedMap;
				Global.state.AdjustMapPoint();
				Global.MainWnd.CommitScrollbar();
				Global.MainWnd.MainDesigner.Refresh();
				this.Refresh();
				if (Global.MainWnd.ovw != null)
				{
					Global.MainWnd.MainDesigner.Focus();
				}
			}
		}

		private void OverViewViewer_MouseDown(object sender, MouseEventArgs e)
		{
			this.OverViewViewer_MouseMove(sender, e);
		}

		private void OverViewWindow_Activated(object sender, EventArgs e)
		{
			if (Global.MainWnd.ovw != null)
			{
				this.UpdateView();
				this.Refresh();
			}
		}

		private Size mWndSize;

		private Point cPoint;
	}
}

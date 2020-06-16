using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Controls;

namespace MasaoPlus
{
	// Token: 0x0200002E RID: 46
	public partial class OverViewWindow : Form
	{
		// Token: 0x060001AF RID: 431 RVA: 0x00024050 File Offset: 0x00022250
		public OverViewWindow()
		{
			this.InitializeComponent();
			this.UpdateView();
			Global.MainWnd.MainDesigner.ChangeBufferInvoke += this.MainDesigner_ChangeBufferInvoke;
			Global.MainWnd.MainDesignerScroll += this.MainWnd_MainDesignerScroll;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00003192 File Offset: 0x00001392
		private void MainWnd_MainDesignerScroll()
		{
			this.UpdateView();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00003192 File Offset: 0x00001392
		private void MainDesigner_ChangeBufferInvoke()
		{
			this.UpdateView();
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x000240A0 File Offset: 0x000222A0
		public void UpdateView()
		{
			this.OverViewViewer.UpdateDrawSource();
			this.mWndSize = new Size((int)((double)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * this.OverViewViewer.ppb) / Global.config.draw.ZoomIndex), (int)((double)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * this.OverViewViewer.ppb) / Global.config.draw.ZoomIndex));
			this.cPoint = new Point(this.mWndSize.Width / 2, this.mWndSize.Height / 2);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000319A File Offset: 0x0000139A
		private void OverViewWindow_Load(object sender, EventArgs e)
		{
			base.ClientSize = this.OverViewViewer.Size;
			if (Global.MainWnd.ovw == null)
			{
				this.Text += "(Spaceで閉じる)";
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00024188 File Offset: 0x00022388
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

		// Token: 0x060001B5 RID: 437 RVA: 0x000241BC File Offset: 0x000223BC
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

		// Token: 0x060001B6 RID: 438 RVA: 0x000242AC File Offset: 0x000224AC
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

		// Token: 0x060001B7 RID: 439 RVA: 0x000031CF File Offset: 0x000013CF
		private void OverViewViewer_MouseDown(object sender, MouseEventArgs e)
		{
			this.OverViewViewer_MouseMove(sender, e);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000031D9 File Offset: 0x000013D9
		private void OverViewWindow_Activated(object sender, EventArgs e)
		{
			if (Global.MainWnd.ovw != null)
			{
				this.UpdateView();
				this.Refresh();
			}
		}

		// Token: 0x04000240 RID: 576
		private Size mWndSize;

		// Token: 0x04000241 RID: 577
		private Point cPoint;
	}
}

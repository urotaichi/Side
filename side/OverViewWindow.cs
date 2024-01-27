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
            InitializeComponent();
            UpdateView();
            Global.MainWnd.MainDesigner.ChangeBufferInvoke += MainDesigner_ChangeBufferInvoke;
            Global.MainWnd.MainDesignerScroll += MainWnd_MainDesignerScroll;
        }

        private void MainWnd_MainDesignerScroll()
        {
            UpdateView();
        }

        private void MainDesigner_ChangeBufferInvoke()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            OverViewViewer.UpdateDrawSource();
            ClientSize = OverViewViewer.Size;
            mWndSize = new Size((int)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * OverViewViewer.ppb / Global.config.draw.ZoomIndex), (int)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * OverViewViewer.ppb / Global.config.draw.ZoomIndex));
            cPoint = new Point(mWndSize.Width / 2, mWndSize.Height / 2);
        }

        private void OverViewWindow_Load(object sender, EventArgs e)
        {
            ClientSize = OverViewViewer.Size;
            if (Global.MainWnd.ovw == null)
            {
                Text += "(Spaceで閉じる)";
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
                    Close();
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
            Refresh();
            return true;
        }

        private void OverViewViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mapPointTranslatedMap = new((e.X - cPoint.X) / OverViewViewer.ppb, (e.Y - cPoint.Y) / OverViewViewer.ppb);
                Global.state.MapPointTranslatedMap = mapPointTranslatedMap;
                Global.state.AdjustMapPoint();
                Global.MainWnd.CommitScrollbar();
                Global.MainWnd.MainDesigner.Refresh();
                Refresh();
                if (Global.MainWnd.ovw != null)
                {
                    Global.MainWnd.MainDesigner.Focus();
                }
            }
        }

        private void OverViewViewer_MouseDown(object sender, MouseEventArgs e)
        {
            OverViewViewer_MouseMove(sender, e);
        }

        private void OverViewWindow_Activated(object sender, EventArgs e)
        {
            if (Global.MainWnd.ovw != null)
            {
                UpdateView();
                Refresh();
            }
        }

        private Size mWndSize;

        private Point cPoint;
    }
}

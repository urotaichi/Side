using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace MasaoPlus
{
    public class GUIChipList : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (Global.cpd.runtime == null)
                {
                    return;
                }
                if (Global.state.MapEditMode)
                {
                    if (value >= Global.cpd.Worldchip.Length || value < 0)
                    {
                        return;
                    }
                    Global.state.CurrentChip = Global.cpd.Worldchip[value];
                }
                else if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                {
                    ChipsData[] array = Global.cpd.Mapchip;
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        array = [.. array, .. Global.cpd.VarietyChip];
                        if (Global.cpd.CustomPartsChip != null)
                        {
                            array = [.. array, .. Global.cpd.CustomPartsChip];
                        }
                    }

                    if (value >= array.Length || value < 0)
                    {
                        return;
                    }
                    Global.state.CurrentChip = array[value];
                }
                else
                {
                    if (value >= Global.cpd.Layerchip.Length || value < 0)
                    {
                        return;
                    }
                    Global.state.CurrentChip = Global.cpd.Layerchip[value];
                }
                selectedIndex = value;
                Refresh();
            }
        }

        protected int vPosition
        {
            get
            {
                return vScr.Value;
            }
            set
            {
                vScr.Value = vPosition;
                vScr.Refresh();
                MainPanel.Refresh();
            }
        }

        protected int hMaxChip
        {
            get
            {
                int num = (int)Math.Floor(MainPanel.Width / ((double)LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width)));
                if (num <= 0)
                {
                    return 1;
                }
                return num;
            }
        }

        protected void vScr_Scroll(object sender, ScrollEventArgs e)
        {
            MainPanel.Refresh();
        }

        protected void SetMaxValue()
        {
            vScr.LargeChange = MainPanel.Height;
            vScr.SmallChange = LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height);
            vScr.Maximum = GetVirtSize() - MainPanel.Height + vScr.LargeChange - 1;
        }

        public GUIChipList()
        {
            InitializeComponent();
        }

        public Point GetPosition(int idx)
        {
            int num = MainPanel.Width / LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width);
            if (num <= 0)
            {
                num = 1;
            }
            return new Point(idx % num, (int)Math.Floor(idx / (double)num));
        }

        public int GetVirtSize()
        {
            return GetVirtSize(MainPanel.Width);
        }

        public virtual int GetVirtSize(int wid)
        {
            if (Global.cpd.runtime == null)
            {
                return 0;
            }
            int num = wid / LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width);
            if (num <= 0)
            {
                num = 1;
            }
            if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
            {
                int num2 = Global.cpd.Mapchip.Length;
                if (Global.cpd.project.Use3rdMapData)
                {
                    num2 += Global.cpd.VarietyChip.Length;
                    if (Global.cpd.CustomPartsChip != null)
                    {
                        num2 += Global.cpd.CustomPartsChip.Length;
                    }
                }
                return (int)Math.Ceiling(num2 / (double)num) * LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height);
            }
            return (int)Math.Ceiling(Global.cpd.Layerchip.Length / (double)num) * LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height);
        }

        public void ResizeInvoke()
        {
            GUIChipList_Resize(this, new EventArgs());
        }

        protected void GUIChipList_Resize(object sender, EventArgs e)
        {
            if (GetVirtSize(Width) < Height)
            {
                vScr.Value = 0;
                vScr.Visible = false;
            }
            else
            {
                vScr.Visible = true;
                SetMaxValue();
            }
            MainPanel.Refresh();
        }

        protected virtual void AddChipData(ChipsData[] chipsData, int num, PaintEventArgs e, int inital = 0)
        {
            bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
            for (int i = inital; i < num; i++)
            {
                ChipsData chipData = chipsData[i - inital];
                Point point = GetPosition(i);
                Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
                Rectangle rectangle = new(new Point(LogicalToDeviceUnits(point.X * chipsize.Width), LogicalToDeviceUnits(point.Y * chipsize.Height)), LogicalToDeviceUnits(chipsize));
                rectangle.Y -= vPosition;
                if (rectangle.Top > MainPanel.Height) break;

                if (rectangle.Bottom >= 0)
                {
                    ChipData cschip = chipData.GetCSChip();
                    if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd) // チップ裏に拡張画像を描画
                    {
                        ChipRenderer.DrawExtendChip(e.Graphics, rectangle, cschip.xdraw, chipsize);
                    }
                    if (!CurrentProjectData.UseLayer || Global.state.EditingForeground) // パターンマップチップ
                    {
                        if (DeviceDpi / 96 >= 2 && (cschip.size == default || cschip.size.Width / cschip.size.Height == 1))
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }
                        else if (Global.config.draw.ClassicChipListInterpolation) e.Graphics.InterpolationMode = InterpolationMode.High;
                        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                        GraphicsState transState = e.Graphics.Save();
                        e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                        if (oriboss_view && chipData.character == "Z")
                        {
                            if (Global.MainWnd.MainDesigner.DrawOribossOrig != null) e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, rectangle.Width, rectangle.Height);
                        }
                        else if (ChipRenderer.IsAthleticChip(cschip.name))
                        {
                            AthleticView.list[cschip.name].Main(this, cschip, e.Graphics, chipsize);
                        }
                        else
                        {
                            e.Graphics.TranslateTransform(LogicalToDeviceUnits(chipsize.Width) / 2, LogicalToDeviceUnits(chipsize.Height) / 2);
                            var rect = new Rectangle(-LogicalToDeviceUnits(chipsize.Width) / 2, -LogicalToDeviceUnits(chipsize.Height) / 2, rectangle.Width, rectangle.Height);
                            if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                            // 水の半透明処理
                            if (ChipRenderer.ShouldApplyWaterTransparency(out float waterLevel) && chipData.character == "4")
                            {
                                ChipRenderer.ApplyWaterTransparency(e.Graphics, Global.MainWnd.MainDesigner.DrawChipOrig, rect, cschip.pattern, chipsize, waterLevel);
                            }
                            else e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, rect, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                        }
                        e.Graphics.Restore(transState);
                    }
                    else // レイヤーマップチップ
                    {
                        if (DeviceDpi / 96 >= 2 && (cschip.size == default || cschip.size.Width / cschip.size.Height == 1))
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }
                        else if (Global.config.draw.ClassicChipListInterpolation) e.Graphics.InterpolationMode = InterpolationMode.High;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawLayerOrig[Global.state.EdittingLayerIndex], rectangle, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                    }
                    if (chipData.character == "Z" && oriboss_view &&
                        Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki) && Global.config.draw.ExtendDraw)
                    {
                        Point p = ChipRenderer.GetOribossExtensionPoint(int.Parse(oriboss_ugoki));
                        // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
                    }
                    else if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
                    {
                        // 前面拡張画像描画
                        ChipRenderer.DrawExtendChip(e.Graphics, rectangle, cschip.xdraw, chipsize);
                    }
                    e.Graphics.PixelOffsetMode = default;
                    if (chipData.idColor != null)
                    {
                        ChipRenderer.DrawIdColorMark(e.Graphics, new Point(rectangle.X, rectangle.Y), chipData.idColor, this);
                    }
                    if (Global.state.MapEditMode && Global.state.CurrentChip.character == chipData.character
                        || !Global.state.MapEditMode
                            && (Global.cpd.project.Use3rdMapData && Global.state.CurrentChip.code == chipData.code
                            || !Global.cpd.project.Use3rdMapData && Global.state.CurrentChip.character == chipData.character))
                    {
                        if (i != selectedIndex) selectedIndex = i;
                        DrawSelectionFrame(e.Graphics, rectangle);
                    }
                }
            }
        }

        // 共通の選択フレーム描画処理
        protected static void DrawSelectionFrame(Graphics g, Rectangle rectangle)
        {
            switch (Global.config.draw.SelDrawMode)
            {
                case Config.Draw.SelectionDrawMode.SideOriginal:
                    using (Brush brush2 = new SolidBrush(Color.FromArgb(100, DrawEx.GetForegroundColor(Global.state.Background))))
                    {
                        ControlPaint.DrawFocusRectangle(g, rectangle);
                        g.FillRectangle(brush2, rectangle);
                    }
                    break;
                case Config.Draw.SelectionDrawMode.mtpp:
                    ControlPaint.DrawFocusRectangle(g, rectangle);
                    ControlPaint.DrawSelectionFrame(g, true, rectangle, new Rectangle(0, 0, 0, 0), Color.Blue);
                    break;
                case Config.Draw.SelectionDrawMode.MTool:
                    g.DrawRectangle(Pens.Black, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1));
                    g.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 3, rectangle.Height - 3));
                    g.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 5, rectangle.Height - 5));
                    g.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 3, rectangle.Y + 3, rectangle.Width - 7, rectangle.Height - 7));
                    g.DrawRectangle(Pens.Black, new Rectangle(rectangle.X + 4, rectangle.Y + 4, rectangle.Width - 9, rectangle.Height - 9));
                    break;
                default:
                    break;
            }
        }

        // クラシックチップリスト
        protected virtual void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (Global.MainWnd.MainDesigner.DrawChipOrig != null && Global.cpd.project != null)
                {
                    if (Global.config.draw.ClassicChipListInterpolation)
                        e.Graphics.InterpolationMode = InterpolationMode.High;
                    else
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    using (Brush brush = new SolidBrush(Global.state.Background))
                    {
                        e.Graphics.FillRectangle(brush, e.ClipRectangle); // 背景色で塗りつぶす
                    }
                    int num = 0, num2 = 0;
                    if (Global.state.MapEditMode)
                    {
                        num = Global.cpd.Worldchip.Length;
                        AddChipData(Global.cpd.Worldchip, num, e);
                    }
                    else if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
                    {
                        num = Global.cpd.Mapchip.Length;
                        AddChipData(Global.cpd.Mapchip, num, e);
                        if (Global.cpd.project.Use3rdMapData)
                        {
                            num2 = Global.cpd.VarietyChip.Length;
                            if (Global.cpd.CustomPartsChip != null)
                            {
                                num2 += Global.cpd.CustomPartsChip.Length;
                                AddChipData([.. Global.cpd.VarietyChip, .. Global.cpd.CustomPartsChip], num + num2, e, num);
                            }
                            AddChipData(Global.cpd.VarietyChip, num + num2, e, num);
                        }
                    }
                    else
                    {
                        num = Global.cpd.Layerchip.Length;
                        AddChipData(Global.cpd.Layerchip, num, e);
                    }

                    if (!Enabled)
                    {
                        using Brush brush3 = new SolidBrush(Color.FromArgb(160, Color.White));
                        e.Graphics.FillRectangle(brush3, e.ClipRectangle);
                    }
                }
            }
            catch
            {
            }
        }

        protected virtual void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();
            if (!Enabled)
            {
                return;
            }
            int hMaxChip = this.hMaxChip;
            if (e.X > hMaxChip * LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width))
            {
                return;
            }
            int num = (int)Math.Floor(e.X / ((double)LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width)));
            num += (int)Math.Floor((e.Y + vPosition) / ((double)LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height))) * hMaxChip;
            int num2;
            if (Global.state.MapEditMode)
            {
                num2 = Global.cpd.Worldchip.Length;
            }
            else if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
            {
                num2 = Global.cpd.Mapchip.Length;
                if (Global.cpd.project.Use3rdMapData)
                {
                    num2 += Global.cpd.VarietyChip.Length;
                    if (Global.cpd.CustomPartsChip != null)
                    {
                        num2 += Global.cpd.CustomPartsChip.Length;
                    }
                }
            }
            else
            {
                num2 = Global.cpd.Layerchip.Length;
            }
            if (num >= num2)
            {
                return;
            }
            SelectedIndex = num;
        }

        protected void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MainPanel_MouseDown(sender, e);
            }
        }

        protected void GUIChipList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    e.IsInputKey = true;
                    return;
                default:
                    return;
            }
        }

        protected void GUIChipList_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    SelectedIndex--;
                    return;
                case Keys.Up:
                    SelectedIndex -= hMaxChip;
                    return;
                case Keys.Right:
                    SelectedIndex++;
                    return;
                case Keys.Down:
                    SelectedIndex += hMaxChip;
                    return;
                default:
                    e.Handled = false;
                    return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            vScr = new VScrollBar();
            MainPanel = new PictureBox();
            ((ISupportInitialize)MainPanel).BeginInit();
            SuspendLayout();
            vScr.Dock = DockStyle.Right;
            vScr.Location = new Point(LogicalToDeviceUnits(265), 0);
            vScr.Name = "vScr";
            vScr.Size = LogicalToDeviceUnits(new Size(20, 264));
            vScr.TabIndex = 0;
            vScr.Scroll += vScr_Scroll;
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = LogicalToDeviceUnits(new Size(265, 264));
            MainPanel.TabIndex = 1;
            MainPanel.TabStop = false;
            MainPanel.MouseMove += MainPanel_MouseMove;
            MainPanel.MouseDown += MainPanel_MouseDown;
            MainPanel.Paint += MainPanel_Paint;
            // AutoScaleDimensions = new SizeF(6f, 12f);
            // AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainPanel);
            Controls.Add(vScr);
            Name = "GUIChipList";
            Size = LogicalToDeviceUnits(new Size(285, 264));
            PreviewKeyDown += GUIChipList_PreviewKeyDown;
            Resize += GUIChipList_Resize;
            KeyDown += GUIChipList_KeyDown;
            ((ISupportInitialize)MainPanel).EndInit();
            ResumeLayout(false);
        }

        protected int selectedIndex;

        protected IContainer components;

        protected VScrollBar vScr;

        protected PictureBox MainPanel;
    }
}

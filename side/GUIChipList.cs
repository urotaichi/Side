using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MasaoPlus
{
    public class GUIChipList : UserControl
    {
        public int SelectedIndex
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
                else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
                {
                    int n = Global.cpd.Mapchip.Length;

                    if (!Global.cpd.project.Use3rdMapData && value >= n
                        || Global.cpd.project.Use3rdMapData && value >= n + Global.cpd.VarietyChip.Length
                        || value < 0)
                    {
                        return;
                    }

                    if (Global.cpd.project.Use3rdMapData)
                    {
                        if (value < n)
                        {
                            Global.state.CurrentChip = Global.cpd.Mapchip[value];
                        }
                        else
                        {
                            Global.state.CurrentChip = Global.cpd.VarietyChip[value - n];
                        }
                    }
                    else
                    {
                        Global.state.CurrentChip = Global.cpd.Mapchip[value];
                    }
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

        private int vPosition
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

        private int hMaxChip
        {
            get
            {
                int num = (int)Math.Floor(MainPanel.Width / (double)Global.cpd.runtime.Definitions.ChipSize.Width);
                if (num <= 0)
                {
                    return 1;
                }
                return num;
            }
        }

        private void vScr_Scroll(object sender, ScrollEventArgs e)
        {
            MainPanel.Refresh();
        }

        private void SetMaxValue()
        {
            vScr.LargeChange = MainPanel.Height;
            vScr.SmallChange = Global.cpd.runtime.Definitions.ChipSize.Height;
            vScr.Maximum = GetVirtSize() - MainPanel.Height + vScr.LargeChange - 1;
        }

        public GUIChipList()
        {
            InitializeComponent();
        }

        public Point GetPosition(int idx)
        {
            int num = MainPanel.Width / Global.cpd.runtime.Definitions.ChipSize.Width;
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

        public int GetVirtSize(int wid)
        {
            if (Global.cpd.runtime == null)
            {
                return 0;
            }
            int num = wid / Global.cpd.runtime.Definitions.ChipSize.Width;
            if (num <= 0)
            {
                num = 1;
            }
            if (!Global.cpd.UseLayer || Global.state.EditingForeground)
            {
                int num2 = Global.cpd.Mapchip.Length;
                if (Global.cpd.project.Use3rdMapData)
                {
                    num2 += Global.cpd.VarietyChip.Length;
                }
                return (int)Math.Ceiling(num2 / (double)num) * Global.cpd.runtime.Definitions.ChipSize.Height;
            }
            return (int)Math.Ceiling(Global.cpd.Layerchip.Length / (double)num) * Global.cpd.runtime.Definitions.ChipSize.Height;
        }

        public void ResizeInvoke()
        {
            GUIChipList_Resize(this, new EventArgs());
        }

        private void GUIChipList_Resize(object sender, EventArgs e)
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

        private void AddChipData(ChipsData[] chipsData, int num, PaintEventArgs e, int inital = 0)
        {
            for (int i = inital; i < num; i++)
            {
                ChipsData chipData = chipsData[i - inital];
                Point point = GetPosition(i);
                Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
                Rectangle rectangle = new Rectangle(new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsize);
                rectangle.Y -= vPosition;
                if (rectangle.Top > MainPanel.Height) break;

                if (rectangle.Bottom >= 0)
                {
                    ChipData cschip = chipData.GetCSChip();
                    if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd)
                    {
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
                    }
                    if (!Global.cpd.UseLayer || Global.state.EditingForeground)
                    {
                        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                        GraphicsState transState = e.Graphics.Save();
                        e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                        if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipData.character == "Z")
                        {
                            e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, chipsize.Width, chipsize.Height);
                        }
                        else
                        {
                            switch (cschip.name)
                            {
                                case "一方通行":
                                case "左右へ押せるドッスンスンのゴール":
                                case "シーソー":
                                case "ブランコ":
                                case "スウィングバー":
                                case "動くＴ字型":
                                case "ロープ":
                                case "長いロープ":
                                case "ゆれる棒":
                                case "人間大砲":
                                case "曲線による上り坂":
                                case "曲線による下り坂":
                                case "乗れる円":
                                case "跳ねる円":
                                case "円":
                                case "半円":
                                case "ファイヤーバー":
                                case "スウィングファイヤーバー":
                                case "人口太陽":
                                case "ファイヤーリング":
                                case "ファイヤーウォール":
                                case "スイッチ式ファイヤーバー":
                                case "スイッチ式動くＴ字型":
                                case "スイッチ式速く動くＴ字型":
                                    AthleticView.list[cschip.name].Main(cschip, e.Graphics, chipsize);
                                    break;
                                default:
                                    e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
                                    if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                                    // 水の半透明処理
                                    if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && chipData.character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
                                    {
                                        float water_clear_level = float.Parse(Global.state.ChipRegister["water_clear_level"]);
                                        var colorMatrix = new ColorMatrix
                                        {
                                            Matrix00 = 1f,
                                            Matrix11 = 1f,
                                            Matrix22 = 1f,
                                            Matrix33 = water_clear_level / 255f,
                                            Matrix44 = 1f
                                        };
                                        using var imageAttributes = new ImageAttributes();
                                        imageAttributes.SetColorMatrix(colorMatrix);
                                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
                                    }
                                    else e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                                    break;
                            }
                        }
                        e.Graphics.Restore(transState);
                    }
                    else
                    {
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawLayerOrig, rectangle, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                    }
                    if (chipData.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 &&
                        Global.state.ChipRegister.ContainsKey("oriboss_ugoki") && Global.config.draw.ExtendDraw)
                    {
                        Point p = default;
                        switch (int.Parse(Global.state.ChipRegister["oriboss_ugoki"]))
                        {
                            case 1:
                                p = new Point(352, 256);
                                break;
                            case 2:
                                p = new Point(96, 0);
                                break;
                            case 3:
                                p = new Point(64, 0);
                                break;
                            case 4:
                                p = new Point(256, 0);
                                break;
                            case 5:
                                p = new Point(288, 0);
                                break;
                            case 6:
                                p = new Point(288, 448);
                                break;
                            case 7:
                                p = new Point(320, 448);
                                break;
                            case 8:
                                p = new Point(32, 32);
                                break;
                            case 9:
                                p = new Point(96, 0);
                                break;
                            case 10:
                                p = new Point(0, 32);
                                break;
                            case 11:
                                p = new Point(64, 0);
                                break;
                            case 12:
                                p = new Point(96, 32);
                                break;
                            case 13:
                                p = new Point(64, 0);
                                break;
                            case 14:
                                p = new Point(352, 448);
                                break;
                            case 15:
                                p = new Point(416, 448);
                                break;
                            case 16:
                                p = new Point(288, 448);
                                break;
                            case 17:
                                p = new Point(320, 448);
                                break;
                            case 18:
                                p = new Point(96, 0);
                                break;
                            case 19:
                                p = new Point(96, 0);
                                break;
                            case 20:
                                p = new Point(256, 0);
                                break;
                            case 21:
                                p = new Point(256, 0);
                                break;
                            case 22:
                                p = new Point(352, 448);
                                break;
                            case 23:
                                p = new Point(384, 448);
                                break;
                            case 24:
                                p = new Point(32, 32);
                                break;
                            case 25:
                                p = new Point(32, 32);
                                break;
                            case 26:
                                p = new Point(32, 128);
                                break;
                            case 27:
                                p = new Point(32, 128);
                                break;
                        }
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
                    }
                    else if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
                    {
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
                    }
                    e.Graphics.PixelOffsetMode = default;
                    if (Global.state.MapEditMode && Global.state.CurrentChip.character == chipData.character
                        || !Global.state.MapEditMode
                            && (Global.cpd.project.Use3rdMapData && Global.state.CurrentChip.code == chipData.code
                            || !Global.cpd.project.Use3rdMapData && Global.state.CurrentChip.character == chipData.character))
                    {
                        if (i != selectedIndex) selectedIndex = i;
                        switch (Global.config.draw.SelDrawMode)
                        {
                            case Config.Draw.SelectionDrawMode.SideOriginal:
                                using (Brush brush2 = new SolidBrush(Color.FromArgb(100, DrawEx.GetForegroundColor(Global.state.Background))))
                                {
                                    ControlPaint.DrawFocusRectangle(e.Graphics, rectangle);
                                    e.Graphics.FillRectangle(brush2, rectangle);
                                    break;
                                }
                            case Config.Draw.SelectionDrawMode.mtpp:
                                ControlPaint.DrawFocusRectangle(e.Graphics, rectangle);
                                ControlPaint.DrawSelectionFrame(e.Graphics, true, rectangle, new Rectangle(0, 0, 0, 0), Color.Blue);
                                break;
                            case Config.Draw.SelectionDrawMode.MTool:
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1));
                                e.Graphics.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 3, rectangle.Height - 3));
                                e.Graphics.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 5, rectangle.Height - 5));
                                e.Graphics.DrawRectangle(Pens.White, new Rectangle(rectangle.X + 3, rectangle.Y + 3, rectangle.Width - 7, rectangle.Height - 7));
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(rectangle.X + 4, rectangle.Y + 4, rectangle.Width - 9, rectangle.Height - 9));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        // クラシックチップリスト
        private void MainPanel_Paint(object sender, PaintEventArgs e)
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
                        e.Graphics.FillRectangle(brush, e.ClipRectangle);
                    }
                    int num = 0, num2 = 0;
                    if (Global.state.MapEditMode)
                    {
                        num = Global.cpd.Worldchip.Length;
                        AddChipData(Global.cpd.Worldchip, num, e);
                    }
                    else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
                    {
                        num = Global.cpd.Mapchip.Length;
                        AddChipData(Global.cpd.Mapchip, num, e);
                        if (Global.cpd.project.Use3rdMapData)
                        {
                            num2 = Global.cpd.VarietyChip.Length;
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

        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();
            if (!Enabled)
            {
                return;
            }
            int hMaxChip = this.hMaxChip;
            if (e.X > hMaxChip * Global.cpd.runtime.Definitions.ChipSize.Width)
            {
                return;
            }
            int num = (int)Math.Floor(e.X / (double)Global.cpd.runtime.Definitions.ChipSize.Width);
            num += (int)Math.Floor((e.Y + vPosition) / (double)Global.cpd.runtime.Definitions.ChipSize.Height) * hMaxChip;
            int num2;
            if (Global.state.MapEditMode)
            {
                num2 = Global.cpd.Worldchip.Length;
            }
            else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
            {
                num2 = Global.cpd.Mapchip.Length;
                if (Global.cpd.project.Use3rdMapData)
                {
                    num2 += Global.cpd.VarietyChip.Length;
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

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MainPanel_MouseDown(sender, e);
            }
        }

        private void GUIChipList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void GUIChipList_KeyDown(object sender, KeyEventArgs e)
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

        private void InitializeComponent()
        {
            vScr = new VScrollBar();
            MainPanel = new PictureBox();
            ((ISupportInitialize)MainPanel).BeginInit();
            SuspendLayout();
            vScr.Dock = DockStyle.Right;
            vScr.Location = new Point(265, 0);
            vScr.Name = "vScr";
            vScr.Size = new Size(20, 264);
            vScr.TabIndex = 0;
            vScr.Scroll += vScr_Scroll;
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(265, 264);
            MainPanel.TabIndex = 1;
            MainPanel.TabStop = false;
            MainPanel.MouseMove += MainPanel_MouseMove;
            MainPanel.MouseDown += MainPanel_MouseDown;
            MainPanel.Paint += MainPanel_Paint;
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainPanel);
            Controls.Add(vScr);
            Name = "GUIChipList";
            Size = new Size(285, 264);
            PreviewKeyDown += GUIChipList_PreviewKeyDown;
            Resize += GUIChipList_Resize;
            KeyDown += GUIChipList_KeyDown;
            ((ISupportInitialize)MainPanel).EndInit();
            ResumeLayout(false);
        }

        private int selectedIndex;

        private IContainer components;

        private VScrollBar vScr;

        private PictureBox MainPanel;
    }
}

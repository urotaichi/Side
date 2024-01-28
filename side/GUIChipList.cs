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
                        if(Global.cpd.CustomPartsChip != null)
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
                    if(Global.cpd.CustomPartsChip != null)
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
                        // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
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
                            e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, rectangle.Width, rectangle.Height);
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
                                    AthleticView.list[cschip.name].Main(DeviceDpi, cschip, e.Graphics, chipsize);
                                    break;
                                default:
                                    e.Graphics.TranslateTransform(LogicalToDeviceUnits(chipsize.Width) / 2, LogicalToDeviceUnits(chipsize.Height) / 2);
                                    var rect = new Rectangle(-LogicalToDeviceUnits(chipsize.Width) / 2, -LogicalToDeviceUnits(chipsize.Height) / 2, rectangle.Width, rectangle.Height);
                                    if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                                    // 水の半透明処理
                                    if (Global.state.ChipRegister.TryGetValue("water_clear_switch", out string water_clear_switch) && bool.Parse(water_clear_switch) == false && chipData.character == "4" && Global.state.ChipRegister.TryGetValue("water_clear_level", out string water_clear_level_value))
                                    {
                                        float water_clear_level = float.Parse(water_clear_level_value);
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
                                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, rect, cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
                                    }
                                    else e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, rect, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                                    break;
                            }
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
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawLayerOrig, rectangle, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                    }
                    if (chipData.character == "Z" && oriboss_view &&
                        Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki) && Global.config.draw.ExtendDraw)
                    {
                        Point p = int.Parse(oriboss_ugoki) switch
                        {
                            1 => new Point(352, 256),
                            2 => new Point(96, 0),
                            3 => new Point(64, 0),
                            4 => new Point(256, 0),
                            5 => new Point(288, 0),
                            6 => new Point(288, 448),
                            7 => new Point(320, 448),
                            8 => new Point(32, 32),
                            9 => new Point(96, 0),
                            10 => new Point(0, 32),
                            11 => new Point(64, 0),
                            12 => new Point(96, 32),
                            13 => new Point(64, 0),
                            14 => new Point(352, 448),
                            15 => new Point(416, 448),
                            16 => new Point(288, 448),
                            17 => new Point(320, 448),
                            18 => new Point(96, 0),
                            19 => new Point(96, 0),
                            20 => new Point(256, 0),
                            21 => new Point(256, 0),
                            22 => new Point(352, 448),
                            23 => new Point(384, 448),
                            24 => new Point(32, 32),
                            25 => new Point(32, 32),
                            26 => new Point(32, 128),
                            27 => new Point(32, 128),
                            _ => throw new ArgumentException(),
                        };
                        // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
                    }
                    else if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
                    {
                        // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
                    }
                    e.Graphics.PixelOffsetMode = default;
                    if (chipData.idColor != null)
                    {
                        GraphicsState transState = e.Graphics.Save();
                        e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                        Color col = ColorTranslator.FromHtml(chipData.idColor);
                        using Brush brush = new SolidBrush(Color.FromArgb(240, col));
                        e.Graphics.FillRectangle(brush, new Rectangle(new Point(0, 0), LogicalToDeviceUnits(new Size(10, 5))));
                        e.Graphics.Restore(transState);
                    }
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

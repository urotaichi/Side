using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using MasaoPlus.Properties;

namespace MasaoPlus
{
    public class GUICustomPartsChipList : GUIChipList
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (Global.cpd.runtime == null || !Global.cpd.project.Use3rdMapData)
                {
                    return;
                }
                if (value >= Global.cpd.CustomPartsChip.Length || value < 0)
                {
                    return;
                }
                Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[value];
                selectedIndex = value;
                Global.MainWnd.CustomPartsConfigList.PrepareCurrentCustomPartsParam();
                Refresh();
            }
        }

        public override int GetVirtSize(int wid)
        {
            if (Global.cpd.runtime == null)
            {
                return 0;
            }
            if(Global.cpd.CustomPartsChip == null)
            {
                return Global.cpd.runtime.Definitions.ChipSize.Height;
            }
            int num = wid / Global.cpd.runtime.Definitions.ChipSize.Width;
            if (num <= 0)
            {
                num = 1;
            }
            return (int)Math.Ceiling(Global.cpd.CustomPartsChip.Length / (double)num) * Global.cpd.runtime.Definitions.ChipSize.Height;
        }

        protected override void AddChipData(ChipsData[] chipsData, int num, PaintEventArgs e, int inital = 0)
        {
            Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
            int i;
            for (i = inital; i < num; i++)
            {
                ChipsData chipData = chipsData[i - inital];
                Point point = GetPosition(i);
                Rectangle rectangle = new(new Point(point.X * LogicalToDeviceUnits(chipsize.Width), point.Y * LogicalToDeviceUnits(chipsize.Height)), LogicalToDeviceUnits(chipsize));
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
                    if (DeviceDpi / 96 >= 2 && (cschip.size == default || cschip.size.Width / cschip.size.Height == 1))
                    {
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    else if (Global.config.draw.ClassicChipListInterpolation) e.Graphics.InterpolationMode = InterpolationMode.High;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    GraphicsState transState = e.Graphics.Save();
                    e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                    if (ChipRenderer.IsAthleticChip(cschip.name))
                    {
                        AthleticView.list[cschip.name].Main(this, cschip, e.Graphics, chipsize);
                    }
                    else
                    {
                        e.Graphics.TranslateTransform(LogicalToDeviceUnits(chipsize.Width) / 2, LogicalToDeviceUnits(chipsize.Height) / 2);
                        var rect = new Rectangle(-LogicalToDeviceUnits(chipsize.Width) / 2, -LogicalToDeviceUnits(chipsize.Height) / 2, rectangle.Width, rectangle.Height);
                        if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, rect, new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                    }
                    e.Graphics.Restore(transState);
                    if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
                    {
                        // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
                        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
                    }
                    e.Graphics.PixelOffsetMode = default;
                    if (chipData.idColor != null)
                    {
                        transState = e.Graphics.Save();
                        e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                        Color col = ColorTranslator.FromHtml(chipData.idColor);
                        using Brush brush = new SolidBrush(Color.FromArgb(240, col));
                        e.Graphics.FillRectangle(brush, new Rectangle(new Point(0, 0), LogicalToDeviceUnits(new Size(10, 5))));
                        e.Graphics.Restore(transState);
                    }
                    if (Global.state.CurrentCustomPartsChip.code == chipData.code)
                    {
                        if (i != selectedIndex) selectedIndex = i;
                        DrawSelectionFrame(e.Graphics, rectangle);
                    }
                }
            }
            Point point2 = GetPosition(i);
            Rectangle rectangle2 = new(new Point(point2.X * LogicalToDeviceUnits(chipsize.Width), point2.Y * LogicalToDeviceUnits(chipsize.Height)), LogicalToDeviceUnits(chipsize));
            rectangle2.Y -= vPosition;
            e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle2, new Rectangle(new Point(448,448), chipsize), GraphicsUnit.Pixel);
        }

        // クラシックチップリスト
        protected override void MainPanel_Paint(object sender, PaintEventArgs e)
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
                    if(Global.cpd.CustomPartsChip == null)
                    {
                        AddChipData(default, 0, e);
                    }
                    else
                    {
                        AddChipData(Global.cpd.CustomPartsChip, Global.cpd.CustomPartsChip.Length, e);
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

        protected override void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (Global.cpd.runtime.Definitions.Package.Contains("28") || !Global.cpd.project.Use3rdMapData)
            {
                return;
            }
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
            int num = (int)Math.Floor(e.X / (double)LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width));
            num += (int)Math.Floor((e.Y + vPosition) / (double)LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height)) * hMaxChip;
            int num2;
            if (Global.cpd.CustomPartsChip == null)
            {
                num2 = 0;
            }
            else
            {
                num2 = Global.cpd.CustomPartsChip.Length;
            }
            if (num >= num2)
            {
                if (num == num2)
                {
                    int i;
                    for (i = 0; i < Global.cpd.VarietyChip.Length; i++)
                    {
                        if (int.Parse(Global.cpd.VarietyChip[i].code) > 5000)
                        {
                            break;
                        }
                    }
                    Create(Global.cpd.VarietyChip[i], $"カスタムパーツ{num2 + 1}");
                }
                return;
            }
            SelectedIndex = num;
            if (e.Button == MouseButtons.Right)
            { // 右クリック時
                MouseStartPoint = default;
                MouseStartPoint.X = (e.X + Global.state.MapPoint.X) / LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width);
                MouseStartPoint.Y = (e.Y + Global.state.MapPoint.Y) / LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height);
                CursorContextMenu.Show(this, new Point(e.X, e.Y));
                return;
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            string name = $"{Global.state.CurrentCustomPartsChip.Chips[0].name}（コピー）";
            Create(Global.state.CurrentCustomPartsChip, name);
            Global.MainWnd.CustomPartsConfigList.ConfView[1, 0].Value = name;
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            void func(string[] stagedata)
            {
                for (int i = 0; i < stagedata.Length; i++)
                {
                    stagedata[i] = stagedata[i].Replace(Global.cpd.CustomPartsChip[selectedIndex].code, "0");
                }
            }
            func(Global.cpd.project.StageData);
            func(Global.cpd.project.StageData2);
            func(Global.cpd.project.StageData3);
            func(Global.cpd.project.StageData4);
            Global.MainWnd.MainDesigner.DrawItemCodeRef.Remove(Global.cpd.CustomPartsChip[selectedIndex].code);
            Global.cpd.CustomPartsChip = [.. Global.cpd.CustomPartsChip.Where((_, index) => index != selectedIndex)];
            if(Global.cpd.CustomPartsChip.Length > 0)
            {
                Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[0];
                SelectedIndex = 0;
            }
            else
            {
                Global.state.CurrentCustomPartsChip = default;
                Global.MainWnd.CustomPartsConfigList.ConfigSelector_SelectedIndexChanged();
            }
            Global.cpd.project.CustomPartsDefinition = Global.cpd.CustomPartsChip;
            Global.MainWnd.RefreshAll();
            Global.state.EditFlag = true;
        }

        private static void Create(ChipsData basedata, string name)
        {
            ChipsData data;
            int i;
            if(basedata.basecode != null)
            {
                for (i = 0; i < Global.cpd.VarietyChip.Length; i++)
                {
                    if (Global.cpd.VarietyChip[i].code == basedata.basecode)
                    {
                        break;
                    }
                }
                data = Global.cpd.VarietyChip[i];
            }
            else
            {
                data = basedata;
            }
            var r = new Random();
            const string PWS_CHARS = "abcdefghijklmnopqrstuvwxyz";
            if(Global.cpd.CustomPartsChip == null)
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, 1);
            }
            else
            {
                Array.Resize(ref Global.cpd.CustomPartsChip, Global.cpd.CustomPartsChip.Length + 1);
            }
            Global.cpd.CustomPartsChip[^1] = basedata;
            Global.cpd.CustomPartsChip[^1].Chips = (ChipData[])basedata.Chips.Clone(); // 配列は個別に複製
            for (int j = 0; j < Global.cpd.CustomPartsChip[^1].Chips.Length; j++)
            {
                Global.cpd.CustomPartsChip[^1].Chips[j].name = name;
                Global.cpd.CustomPartsChip[^1].Chips[j].description = $"{data.Chips[j].name} {data.Chips[j].description}";
            }
            Global.cpd.CustomPartsChip[^1].basecode = data.code;
            Global.cpd.CustomPartsChip[^1].code = string.Join("", Enumerable.Range(0, 10).Select(_ => PWS_CHARS[r.Next(PWS_CHARS.Length)]));
            Global.cpd.CustomPartsChip[^1].idColor = $"#{Guid.NewGuid().ToString("N")[..6]}";
            Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[^1];
            Global.MainWnd.MainDesigner.DrawItemCodeRef[Global.state.CurrentCustomPartsChip.code] = Global.state.CurrentCustomPartsChip;
            if (Global.cpd.project.CustomPartsDefinition == null)
            {
                Array.Resize(ref Global.cpd.project.CustomPartsDefinition, 1);
            }
            else
            {
                Array.Resize(ref Global.cpd.project.CustomPartsDefinition, Global.cpd.project.CustomPartsDefinition.Length + 1);
            }
            Global.cpd.project.CustomPartsDefinition = Global.cpd.CustomPartsChip;
            Global.MainWnd.RefreshAll();
            Global.state.EditFlag = true;
            Global.MainWnd.CustomPartsConfigList.ConfigSelector_SelectedIndexChanged();
        }

        protected override void InitializeComponent()
        {
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            vScr = new VScrollBar();
            MainPanel = new PictureBox();
            components = new Container();
            CursorContextMenu = new ContextMenuStrip(components);
            Copy = new ToolStripMenuItem();
            Delete = new ToolStripMenuItem();
            ((ISupportInitialize)MainPanel).BeginInit();
            CursorContextMenu.SuspendLayout();
            SuspendLayout();
            CursorContextMenu.Items.AddRange(
            [
                Copy,
                Delete
            ]);
            CursorContextMenu.Name = "CursorContextMenu";
            CursorContextMenu.Size = LogicalToDeviceUnits(new Size(275, 170));
            Copy.Image = new IconImageView(DeviceDpi, Resources.copy).View();
            Copy.Name = "Copy";
            Copy.Size = LogicalToDeviceUnits(new Size(274, 22));
            Copy.Text = "コピー(&C)";
            Copy.Click += Copy_Click;
            Delete.Image = new IconImageView(DeviceDpi, Resources.cross).View();
            Delete.Name = "Copy";
            Delete.Size = LogicalToDeviceUnits(new Size(274, 22));
            Delete.Text = "削除";
            Delete.Click += Delete_Click;

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
            //AutoScaleDimensions = new SizeF(6f, 12f);
            //AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainPanel);
            Controls.Add(vScr);
            Name = "GUICustomPartsChipList";
            Size = LogicalToDeviceUnits(new Size(285, 264));
            PreviewKeyDown += GUIChipList_PreviewKeyDown;
            Resize += GUIChipList_Resize;
            KeyDown += GUIChipList_KeyDown;
            ((ISupportInitialize)MainPanel).EndInit();
            CursorContextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Point MouseStartPoint = default;

        private ContextMenuStrip CursorContextMenu;

        private ToolStripMenuItem Copy;

        private ToolStripMenuItem Delete;
    }
}

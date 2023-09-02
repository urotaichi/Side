﻿using System;
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
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    GraphicsState transState = e.Graphics.Save();
                    e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
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

                            e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), new Rectangle(cschip.pattern, (cschip.size == default) ? chipsize : cschip.size), GraphicsUnit.Pixel);
                            break;
                    }
                    e.Graphics.Restore(transState);
                    if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
                    {
                        e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
                    }
                    e.Graphics.PixelOffsetMode = default;
                    if (chipData.idColor != null)
                    {
                        transState = e.Graphics.Save();
                        e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
                        Color col = ColorTranslator.FromHtml(chipData.idColor);
                        using Brush brush = new SolidBrush(Color.FromArgb(240, col));
                        e.Graphics.FillRectangle(brush, 0, 0, 10, 5);
                        e.Graphics.Restore(transState);
                    }
                    if (Global.state.CurrentCustomPartsChip.code == chipData.code)
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
            Point point2 = GetPosition(i);
            Rectangle rectangle2 = new Rectangle(new Point(point2.X * chipsize.Width, point2.Y * chipsize.Height), chipsize);
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
                        e.Graphics.FillRectangle(brush, e.ClipRectangle);
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
                    Create(Global.cpd.VarietyChip[i]);
                }
                return;
            }
            SelectedIndex = num;
            if (e.Button == MouseButtons.Right)
            { // 右クリック時
                MouseStartPoint = default;
                MouseStartPoint.X = (e.X + Global.state.MapPoint.X) / Global.cpd.runtime.Definitions.ChipSize.Width;
                MouseStartPoint.Y = (e.Y + Global.state.MapPoint.Y) / Global.cpd.runtime.Definitions.ChipSize.Height;
                CursorContextMenu.Show(this, new Point(e.X, e.Y));
                return;
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Create(Global.state.CurrentCustomPartsChip);
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
            Global.cpd.CustomPartsChip = Global.cpd.CustomPartsChip.Where((_, index) => index != selectedIndex).ToArray();
            if(Global.cpd.CustomPartsChip.Length > 0)
            {
                Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[0];
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

        private void Create(ChipsData basedata)
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
            Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1] = basedata;
            Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].Chips = (ChipData[])basedata.Chips.Clone(); // 配列は個別に複製
            for (int j = 0; j < Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].Chips.Length; j++)
            {
                Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].Chips[j].name = $"カスタムパーツ{Global.cpd.CustomPartsChip.Length}";
                Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].Chips[j].description = $"{data.Chips[j].name} {data.Chips[j].description}";
            }
            Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].basecode = data.code;
            Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].code = string.Join("", Enumerable.Range(0, 10).Select(_ => PWS_CHARS[r.Next(PWS_CHARS.Length)]));
            Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1].idColor = $"#{Guid.NewGuid().ToString("N").Substring(0, 6)}";
            Global.state.CurrentCustomPartsChip = Global.cpd.CustomPartsChip[Global.cpd.CustomPartsChip.Length - 1];
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
            vScr = new VScrollBar();
            MainPanel = new PictureBox();
            components = new Container();
            CursorContextMenu = new ContextMenuStrip(components);
            Copy = new ToolStripMenuItem();
            Delete = new ToolStripMenuItem();
            ((ISupportInitialize)MainPanel).BeginInit();
            CursorContextMenu.SuspendLayout();
            SuspendLayout();
            CursorContextMenu.Items.AddRange(new ToolStripItem[]
            {
                Copy,
                Delete
            });
            CursorContextMenu.Name = "CursorContextMenu";
            CursorContextMenu.Size = new Size(275, 170);
            Copy.Image = Resources.copy;
            Copy.Name = "Copy";
            Copy.Size = new Size(274, 22);
            Copy.Text = "コピー(&C)";
            Copy.Click += Copy_Click;
            Delete.Name = "Copy";
            Delete.Size = new Size(274, 22);
            Delete.Text = "削除";
            Delete.Click += Delete_Click;

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
            Name = "GUICustomPartsChipList";
            Size = new Size(285, 264);
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
﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MasaoPlus
{
    readonly struct Athletic
    {
        public Athletic(MainFunc Main, MaxFunc Max, MainFunc Min, SmallFunc Small, MainFunc Large)
        {
            this.Main = Main;
            this.Max = Max;
            this.Min = Min;
            this.Small = Small;
            this.Large = Large;
        }
        public delegate void MainFunc(ChipData cschip, Graphics g, Size chipsize);
        public delegate void MaxFunc(ChipData cschip, Graphics g, Size chipsize, GUIDesigner gd, int base_y);
        public delegate void SmallFunc(ChipData cschip, Graphics g, Size chipsize, int height);
        public MainFunc Main { get; } // チップ - クラシック、チップリストの左上 
        public MaxFunc Max { get; } // グラフィカルデザイナ
        public MainFunc Min { get; } // ステータスバーっぽいところに表示される小さいアイコン
        public SmallFunc Small { get; } // チップ - サムネイル
        public MainFunc Large { get; } // チップ - チップ
    }
    internal static class AthleticView
    {
        static AthleticView() {
            Pen pen, pen2;
            SolidBrush brush;
            double rad = 0;
            const double math_pi = 3.1415926535897931;
            PointF[] vo_pa;

            list = new Dictionary<string, Athletic>(){
                {"一方通行", new Athletic(
                    (cschip, g, chipsize) => {
                        if (!cschip.description.Contains("表示なし")){
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            if (cschip.description.Contains("右"))
                                g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
                            else if (cschip.description.Contains("左"))
                                g.DrawLine(pen, 1, 0, 1, chipsize.Height);
                            else if (cschip.description.Contains("上"))
                                g.DrawLine(pen, 0, 1, chipsize.Width, 1);
                            else if (cschip.description.Contains("下"))
                                g.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
                            pen.Dispose();
                        }
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        if (!cschip.description.Contains("表示なし")){
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            if (cschip.description.Contains("右"))
                                g.DrawLine(pen, cschip.view_size.Width - 1, 0, cschip.view_size.Width - 1, cschip.view_size.Height);
                            else if (cschip.description.Contains("左"))
                                g.DrawLine(pen, 1, 0, 1, cschip.view_size.Height);
                            else if (cschip.description.Contains("上"))
                                g.DrawLine(pen, 0, 1, cschip.view_size.Width, 1);
                            else if (cschip.description.Contains("下"))
                                g.DrawLine(pen, 0, cschip.view_size.Height - 1, cschip.view_size.Width, cschip.view_size.Height - 1);
                            pen.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        if (!cschip.description.Contains("表示なし")){
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            if (cschip.description.Contains("右"))
                                g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Width);
                            else if (cschip.description.Contains("左"))
                                g.DrawLine(pen, 0, 0, 0, chipsize.Width);
                            else if (cschip.description.Contains("上"))
                                g.DrawLine(pen, 0, 0, chipsize.Width, 0);
                            else if (cschip.description.Contains("下"))
                                g.DrawLine(pen, 0, chipsize.Width - 1, chipsize.Width, chipsize.Width - 1);
                            pen.Dispose();
                        }
                    },
                    (cschip, g, chipsize, height) => {
                        if (!cschip.description.Contains("表示なし")){
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2 / height);
                            if (cschip.description.Contains("右"))
                                g.DrawLine(pen, height - 1, 0, height - 1, height);
                            else if (cschip.description.Contains("左"))
                                g.DrawLine(pen, 1, 0, 1, height);
                            else if (cschip.description.Contains("上"))
                                g.DrawLine(pen, 0, 1, height, 1);
                            else if (cschip.description.Contains("下"))
                                g.DrawLine(pen, 0, height - 1, height, height - 1);
                            pen.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        if (!cschip.description.Contains("表示なし")){
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            if (cschip.description.Contains("右"))
                                g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
                            else if (cschip.description.Contains("左"))
                                g.DrawLine(pen, 1, 0, 1, chipsize.Height);
                            else if (cschip.description.Contains("上"))
                                g.DrawLine(pen, 0, 1, chipsize.Width, 1);
                            else if (cschip.description.Contains("下"))
                                g.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
                            pen.Dispose();
                        }
                    })
                },
                {"左右へ押せるドッスンスンのゴール", new Athletic(
                    (cschip, g, chipsize) => {
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
                        g.TranslateTransform(1, 1);
                        g.DrawRectangle(pen, 0, 11, chipsize.Width - 1, chipsize.Height - 1 - 11);
                        g.TranslateTransform(-1, -1);
                        g.DrawLine(pen, 0, 11, chipsize.Width, chipsize.Height);
                        g.DrawLine(pen, 0, chipsize.Height, chipsize.Width, 11);
                        pen.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(-cschip.center.X + 1, -cschip.center.Y + 1);
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.DrawRectangle(pen, 0, 0, 94, 62);
                        g.DrawLine(pen, 0, 0, 94, 62);
                        g.DrawLine(pen, 0, 62, 94, 0);
                        pen.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.TranslateTransform(1, 1);
                        g.DrawRectangle(pen, 0, 11, chipsize.Width - 2, chipsize.Width - 2 - 11);
                        g.TranslateTransform(-1, 0);
                        g.DrawLine(pen, 0, 11, chipsize.Width, chipsize.Width);
                        g.DrawLine(pen, 0, chipsize.Width, chipsize.Width, 11);
                        pen.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
                        g.TranslateTransform(1, 1);
                        g.DrawRectangle(pen, 0, 5, height - 1, height - 1 - 5);
                        g.TranslateTransform(-1, -1);
                        g.DrawLine(pen, 0, 5, height, height);
                        g.DrawLine(pen, 0, height, height, 5);
                        pen.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
                        g.TranslateTransform(1, 1);
                        g.DrawRectangle(pen, 0, 11, chipsize.Width - 1, chipsize.Height - 1 - 11);
                        g.TranslateTransform(-1, -1);
                        g.DrawLine(pen, 0, 11, chipsize.Width, chipsize.Height);
                        g.DrawLine(pen, 0, chipsize.Height, chipsize.Width, 11);
                        pen.Dispose();
                    })
                },
                {"シーソー", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 17);
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
                        else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[1].X = (float)Math.Cos(rad) * chipsize.Width / 2;
                        vo_pa[1].Y = (float)Math.Sin(rad) * chipsize.Width / 2;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = 0;
                        vo_pa[0].Y = -2;
                        vo_pa[1].X = -4;
                        vo_pa[1].Y = 15;
                        vo_pa[2].X = 4;
                        vo_pa[2].Y = 15;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 0);
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
                        else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * 160;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * 160;
                        vo_pa[1].X = (float)Math.Cos(rad) * 160;
                        vo_pa[1].Y = (float)Math.Sin(rad) * 160;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 12;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 12;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 12;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 12;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = 0;
                        vo_pa[0].Y = 0;
                        vo_pa[1].X = -16;
                        vo_pa[1].Y = 128;
                        vo_pa[2].X = 16;
                        vo_pa[2].Y = 128;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                        pen = new Pen(Color.White, 2);
                        if (cschip.description.Contains("左")) rad = -56;
                        else if (cschip.description.Contains("右")) rad = 56;
                        else rad = 0;
                        g.DrawLine(pen,
                            (float)(Math.Floor(Math.Cos(((rad + 180) * math_pi) / 180) * 160) + Math.Floor(Math.Cos(((rad + 270) * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Sin(((rad + 180) * math_pi) / 180) * 160) + Math.Floor(Math.Sin(((rad + 270) * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Cos((rad * math_pi) / 180) * 160) + Math.Floor(Math.Cos(((rad - 90) * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Sin((rad * math_pi) / 180) * 160) + Math.Floor(Math.Sin(((rad - 90) * math_pi) / 180) * 12))
                        );
                        pen.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 17);
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
                        else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[1].X = (float)Math.Cos(rad) * chipsize.Width / 2;
                        vo_pa[1].Y = (float)Math.Sin(rad) * chipsize.Width / 2;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = 0;
                        vo_pa[0].Y = -2;
                        vo_pa[1].X = -4;
                        vo_pa[1].Y = 15;
                        vo_pa[2].X = 4;
                        vo_pa[2].Y = 15;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(height / 2, height / 2 + 1);
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
                        else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * height / 2;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * height / 2;
                        vo_pa[1].X = (float)Math.Cos(rad) * height / 2;
                        vo_pa[1].Y = (float)Math.Sin(rad) * height / 2;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * height / 6;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * height / 6;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * height / 6;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * height / 6;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = 0;
                        vo_pa[0].Y = 0;
                        vo_pa[1].X = -2;
                        vo_pa[1].Y = height / 2;
                        vo_pa[2].X = 2;
                        vo_pa[2].Y = height / 2;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 17);
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
                        else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * chipsize.Width / 2;
                        vo_pa[1].X = (float)Math.Cos(rad) * chipsize.Width / 2;
                        vo_pa[1].Y = (float)Math.Sin(rad) * chipsize.Width / 2;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = 0;
                        vo_pa[0].Y = -2;
                        vo_pa[1].X = -4;
                        vo_pa[1].Y = 15;
                        vo_pa[2].X = 4;
                        vo_pa[2].Y = 15;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"ブランコ", new Athletic(
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, -9);
                        rad = 90 * Math.PI / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
                        double dx = Math.Cos(rad) * 21;
                        double dy = Math.Sin(rad) * 21;
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen.Dispose();
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.DrawImage(gd.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 16);
                        rad = (90 + Math.Floor((double)(30 + 5) / 10)) * Math.PI / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * 192;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * 192;
                        vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * 192;
                        vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * 192;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 12;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 12;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 12;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 12;
                        double dx = Math.Cos(rad) * 80;
                        double dy = Math.Sin(rad) * 80;
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.DrawLine(pen, (float)Math.Cos(rad) * 12, (float)Math.Sin(rad) * 12, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen2 = new Pen(Color.White, 2);
                        rad = 90 + Math.Floor((double)(30 + 5) / 10);
                        g.DrawLine(pen2,
                            (float)Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192),
                            (float)Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192),
                            (float)Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192),
                            (float)Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192)
                        );
                        if (cschip.description == "２個連続")
                        {
                            g.TranslateTransform(384, 0);
                            g.DrawImage(gd.DrawChipOrig,
                                new Rectangle(-16, -16, chipsize.Width, chipsize.Height),
                                new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                            rad = (90 + Math.Floor((double)(-30 - 5) / 10)) * Math.PI / 180;
                            vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * 192;
                            vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * 192;
                            vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * 192;
                            vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * 192;
                            vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 12;
                            vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 12;
                            vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 12;
                            vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 12;
                            dx = Math.Cos(rad) * 80;
                            dy = Math.Sin(rad) * 80;
                            g.DrawLine(pen, (float)Math.Cos(rad) * 12, (float)Math.Sin(rad) * 12, (float)dx, (float)dy);
                            g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
                            g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
                            g.FillPolygon(brush, vo_pa);
                            rad = 90 + Math.Floor((double)(-30 - 5) / 10);
                            g.DrawLine(pen2,
                                (float)Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192),
                                (float)Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192),
                                (float)Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192),
                                (float)Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192)
                            );
                        }
                        pen2.Dispose();
                        pen.Dispose();
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, -9);
                        rad = 90 * Math.PI / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
                        double dx = Math.Cos(rad) * 21;
                        double dy = Math.Sin(rad) * 21;
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen.Dispose();
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, height, height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(height / 2, -height / 2 + 3);
                        rad = 90 * Math.PI / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 8) * height * (float)1.1;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 8) * height * (float)1.1;
                        vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 8) * height * (float)1.1;
                        vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 8) * height * (float)1.1;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 2;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 2;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 2;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 2;
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
                        g.DrawLine(pen, 0, 5, 0, 8);
                        g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, 0, 8);
                        g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, 0, 8);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen.Dispose();
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, -9);
                        rad = 90 * Math.PI / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
                        double dx = Math.Cos(rad) * 21;
                        double dy = Math.Sin(rad) * 21;
                        pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
                        g.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
                        g.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen.Dispose();
                        brush.Dispose();
                    })
                },
                {"スウィングバー", new Athletic(
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, 14);
                        if (cschip.description.Contains("左"))
                        {
                            g.TranslateTransform(32, 0);
                            rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                        }
                        else if (cschip.description.Contains("右"))
                        {
                            g.TranslateTransform(-32, 0);
                            rad = 360 + Math.Floor((double)(26 + 5) / 10);
                        }
                        rad = (rad * Math.PI) / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
                        vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.DrawImage(gd.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 16);
                        if (cschip.description.Contains("左")) rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                        else if (cschip.description.Contains("右")) rad = 360 + Math.Floor((double)(26 + 5) / 10);
                        rad = (rad * Math.PI) / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos(rad) * 192 + Math.Cos(rad + Math.PI / 2) * 12);
                        vo_pa[0].Y = (float)(Math.Sin(rad) * 192 + Math.Sin(rad + Math.PI / 2) * 12);
                        vo_pa[1].X = (float)(Math.Cos(rad) * 60 + Math.Cos(rad + Math.PI / 2) * 12);
                        vo_pa[1].Y = (float)(Math.Sin(rad) * 60 + Math.Sin(rad + Math.PI / 2) * 12);
                        vo_pa[2].X = (float)(Math.Cos(rad) * 60 + Math.Cos(rad - Math.PI / 2) * 12);
                        vo_pa[2].Y = (float)(Math.Sin(rad) * 60 + Math.Sin(rad - Math.PI / 2) * 12);
                        vo_pa[3].X = (float)(Math.Cos(rad) * 192 + Math.Cos(rad - Math.PI / 2) * 12);
                        vo_pa[3].Y = (float)(Math.Sin(rad) * 192 + Math.Sin(rad - Math.PI / 2) * 12);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                        pen = new Pen(Color.White, 2);
                        if (cschip.description.Contains("左"))
                        {
                            rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                            g.DrawLine(pen, (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 192) + Math.Floor(Math.Cos((rad + 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 192) + Math.Floor(Math.Sin((rad + 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 60) + Math.Floor(Math.Cos((rad + 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 60) + Math.Floor(Math.Sin((rad + 90) * math_pi / 180) * 12)));
                        }
                        else if (cschip.description.Contains("右"))
                        {
                            rad = 360 + Math.Floor((double)(26 + 5) / 10);
                            g.DrawLine(pen, (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 60) + Math.Floor(Math.Cos((rad - 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 60) + Math.Floor(Math.Sin((rad - 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 192) + Math.Floor(Math.Cos((rad - 90) * math_pi / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 192) + Math.Floor(Math.Sin((rad - 90) * math_pi / 180) * 12)));
                        }
                        pen.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, 14);
                        if (cschip.description.Contains("左"))
                        {
                            g.TranslateTransform(32, 0);
                            rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                        }
                        else if (cschip.description.Contains("右"))
                        {
                            g.TranslateTransform(-32, 0);
                            rad = 360 + Math.Floor((double)(26 + 5) / 10);
                        }
                        rad = (rad * Math.PI) / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
                        vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, height, height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(0, 5);
                        if (cschip.description.Contains("左"))
                        {
                            g.TranslateTransform(20, 0);
                            rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                        }
                        else if (cschip.description.Contains("右"))
                        {
                            g.TranslateTransform(-10, 0);
                            rad = 360 + Math.Floor((double)(26 + 5) / 10);
                        }
                        rad = (rad * Math.PI) / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 1);
                        vo_pa[0].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 1);
                        vo_pa[1].X = (float)(Math.Cos(rad) * 10 + Math.Cos(rad + Math.PI / 2) * 1);
                        vo_pa[1].Y = (float)(Math.Sin(rad) * 10 + Math.Sin(rad + Math.PI / 2) * 1);
                        vo_pa[2].X = (float)(Math.Cos(rad) * 10 + Math.Cos(rad - Math.PI / 2) * 1);
                        vo_pa[2].Y = (float)(Math.Sin(rad) * 10 + Math.Sin(rad - Math.PI / 2) * 1);
                        vo_pa[3].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 1);
                        vo_pa[3].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 1);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.TranslateTransform(16, 14);
                        if (cschip.description.Contains("左"))
                        {
                            g.TranslateTransform(32, 0);
                            rad = 180 + Math.Floor((double)(-26 - 5) / 10);
                        }
                        else if (cschip.description.Contains("右"))
                        {
                            g.TranslateTransform(-32, 0);
                            rad = 360 + Math.Floor((double)(26 + 5) / 10);
                        }
                        rad = (rad * Math.PI) / 180;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
                        vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
                        vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
                        vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
                        vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"動くＴ字型", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 37);
                        rad = 270;
                        vo_pa = new PointF[3];
                        vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = 0;
                        vo_pa[2].Y = 0;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 48);
                        rad = 270 + Math.Floor((double)(-30 - 5) / 10);
                        vo_pa = new PointF[3];
                        vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * 182;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * 182;
                        vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * 182;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * 182;
                        vo_pa[2].X = 0;
                        vo_pa[2].Y = 0;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * 192;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * 192;
                        vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * 192;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * 192;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 12;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 12;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 12;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 12;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        pen = new Pen(Color.White, 2);
                        g.DrawLine(pen,
                            (float)(Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
                            (float)(Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12))
                        );
                        if (cschip.description == "２個連続")
                        {
                            g.TranslateTransform(416, 0);
                            rad = 270 + Math.Floor((double)(30 + 5) / 10);
                            vo_pa = new PointF[3];
                            vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * 182;
                            vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * 182;
                            vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * 182;
                            vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * 182;
                            vo_pa[2].X = 0;
                            vo_pa[2].Y = 0;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * 192;
                            vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * 192;
                            vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * 192;
                            vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * 192;
                            vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 12;
                            vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 12;
                            vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 12;
                            vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 12;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);
                            g.DrawLine(pen,
                                (float)(Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12)),
                                (float)(Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
                                (float)(Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12))
                            );
                        }
                        pen.Dispose();
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 37);
                        rad = 270;
                        vo_pa = new PointF[3];
                        vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = 0;
                        vo_pa[2].Y = 0;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(height / 2, height + 2);
                        rad = 270;
                        vo_pa = new PointF[3];
                        vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * height;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * height;
                        vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * height;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * height;
                        vo_pa[2].X = 0;
                        vo_pa[2].Y = 0;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * height * (float)1.3;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * height;
                        vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * height * (float)1.3;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * height;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 2;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 2;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 2;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 2;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(16, 37);
                        rad = 270;
                        vo_pa = new PointF[3];
                        vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = 0;
                        vo_pa[2].Y = 0;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
                        vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * chipsize.Width;
                        vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
                        vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"ロープ", new Athletic(
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        int length;
                        if (cschip.name == "ロープ") length = 2;
                        else length = 1;
                        if (cschip.description == "つかまると左から動く")
                        {
                            g.TranslateTransform(39, 5);
                            rad = 168;
                        }
                        else
                        {
                            g.TranslateTransform(16, -9);
                            rad = 90;
                        }
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.DrawImage(gd.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 16);
                        int length;
                        if (cschip.name == "ロープ") length = 182;
                        else length = 226;
                        if (cschip.description == "つかまると左から動く") rad = 168;
                        else if (cschip.name == "ゆれる棒")
                        {
                            if (cschip.description.Contains("左から")) rad = 270 + Math.Floor((double)(-22 - 5) / 10);
                            else rad = 270 + Math.Floor((double)(22 + 5) / 10);
                        }
                        else if (cschip.name == "長いロープ")
                        {
                            if (cschip.description == "つかまると動く") rad = 90;
                            else if (cschip.description == "右から") rad = 90 + Math.Floor((double)(-22 - 5) / 10);
                            else rad = 90 + Math.Floor((double)(22 + 5) / 10);
                        }
                        else rad = 90 + Math.Floor((double)(30 + 5) / 10);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * 5);
                        vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * 5);
                        vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * 5);
                        vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * 5);
                        vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * length + Math.Cos(((rad - 90) * Math.PI) / 180) * 5);
                        vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * length + Math.Sin(((rad - 90) * Math.PI) / 180) * 5);
                        vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * length + Math.Cos(((rad + 90) * Math.PI) / 180) * 5);
                        vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * length + Math.Sin(((rad + 90) * Math.PI) / 180) * 5);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        if (cschip.description.Contains("２本連続"))
                        {
                            g.TranslateTransform(320, 0);
                            g.DrawImage(gd.DrawChipOrig,
                                new Rectangle(-16, -16, chipsize.Width, chipsize.Height),
                                new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                            rad = 90 + Math.Floor((double)(-30 - 5) / 10);
                            vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * 5);
                            vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * 5);
                            vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * 5);
                            vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * 5);
                            vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * length + Math.Cos(((rad - 90) * Math.PI) / 180) * 5);
                            vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * length + Math.Sin(((rad - 90) * Math.PI) / 180) * 5);
                            vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * length + Math.Cos(((rad + 90) * Math.PI) / 180) * 5);
                            vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * length + Math.Sin(((rad + 90) * Math.PI) / 180) * 5);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        int length;
                        if (cschip.name == "ロープ") length = 2;
                        else length = 1;
                        if (cschip.description == "つかまると左から動く")
                        {
                            g.TranslateTransform(39, 5);
                            rad = 168;
                        }
                        else
                        {
                            g.TranslateTransform(16, -9);
                            rad = 90;
                        }
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, height, height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        double length;
                        if (cschip.name == "ロープ") length = 1;
                        else length = 0.5;
                        if (cschip.description == "つかまると左から動く")
                        {
                            g.TranslateTransform(height * 2 - 3, 0);
                            rad = 168;
                        }
                        else
                        {
                            g.TranslateTransform(height / 2, -height);
                            rad = 90;
                        }
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * height * 1.8 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * height * 1.8 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * height * 1.8 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * height * 1.8 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        int length;
                        if (cschip.name == "ロープ") length = 2;
                        else length = 1;
                        if (cschip.description == "つかまると左から動く")
                        {
                            g.TranslateTransform(39, 5);
                            rad = 168;
                        }
                        else
                        {
                            g.TranslateTransform(16, -9);
                            rad = 90;
                        }
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
                        vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
                        vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"人間大砲", new Athletic(
                    (cschip, g, chipsize) => {
                        if (cschip.description == "右向き") { rad = 330; g.TranslateTransform(-9, 3); }
                        else if (cschip.description == "左向き") { rad = 225; g.TranslateTransform(9, 3); }
                        else if (cschip.description == "天井") { rad = 30; g.TranslateTransform(-9, 0); }
                        else if (cschip.description == "右の壁") { rad = 270; g.TranslateTransform(0, 9); }
                        else if (cschip.description == "左の壁") { rad = 300; g.TranslateTransform(0, 9); }
                        brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
                        g.FillEllipse(brush, 16 - 7, 16 - 7, 14, 14);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        g.FillPolygon(brush, vo_pa);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.description == "天井")
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 + 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 + 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = 0;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = 0;
                        }
                        else if (cschip.description == "右の壁")
                        {
                            vo_pa[0].X = 16 - 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 - 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = chipsize.Width;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = chipsize.Width;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else if (cschip.description == "左の壁")
                        {
                            vo_pa[0].X = 16 + 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 + 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = 0;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = 0;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 - 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 - 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = chipsize.Width - 3;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = chipsize.Width - 3;
                        }
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        if (cschip.description.Contains("向き")) g.TranslateTransform(0, -12);
                        brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
                        g.FillEllipse(brush, 16 - 19, 16 - 19, 38, 38);
                        if (cschip.description == "右向き") rad = 330;
                        else if (cschip.description == "左向き") rad = 225;
                        else if (cschip.description == "天井") rad = 30;
                        else if (cschip.description == "右の壁") rad = 270;
                        else if (cschip.description == "左の壁") rad = 300;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 20;
                        vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 20;
                        vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 20;
                        vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 20;
                        vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 68 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 20;
                        vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 68 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 20;
                        vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 68 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 20;
                        vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 68 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 20;
                        g.FillPolygon(brush, vo_pa);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.description == "天井")
                        {
                            vo_pa[0].X = 16 - 6;
                            vo_pa[0].Y = 16 + 4;
                            vo_pa[1].X = 16 + 6;
                            vo_pa[1].Y = 16 + 4;
                            vo_pa[2].X = 16 + 12;
                            vo_pa[2].Y = -32;
                            vo_pa[3].X = 16 - 12;
                            vo_pa[3].Y = -32;
                        }
                        else if (cschip.description == "右の壁")
                        {
                            vo_pa[0].X = 16 - 4;
                            vo_pa[0].Y = 16 - 6;
                            vo_pa[1].X = 16 - 4;
                            vo_pa[1].Y = 16 + 6;
                            vo_pa[2].X = 64;
                            vo_pa[2].Y = 16 + 12;
                            vo_pa[3].X = 64;
                            vo_pa[3].Y = 16 - 12;
                        }
                        else if (cschip.description == "左の壁")
                        {
                            vo_pa[0].X = 16 + 4;
                            vo_pa[0].Y = 16 - 6;
                            vo_pa[1].X = 16 + 4;
                            vo_pa[1].Y = 16 + 6;
                            vo_pa[2].X = -32;
                            vo_pa[2].Y = 16 + 12;
                            vo_pa[3].X = -32;
                            vo_pa[3].Y = 16 - 12;
                        }
                        else
                        {
                            vo_pa[0].X = 16 - 6;
                            vo_pa[0].Y = 16 - 4;
                            vo_pa[1].X = 16 + 6;
                            vo_pa[1].Y = 16 - 4;
                            vo_pa[2].X = 16 + 12;
                            vo_pa[2].Y = 32 + 12;
                            vo_pa[3].X = 16 - 12;
                            vo_pa[3].Y = 32 + 12;
                        }
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        if (cschip.description == "右向き") { rad = 330; g.TranslateTransform(-9, 3); }
                        else if (cschip.description == "左向き") { rad = 225; g.TranslateTransform(9, 3); }
                        else if (cschip.description == "天井") { rad = 30; g.TranslateTransform(-9, 0); }
                        else if (cschip.description == "右の壁") { rad = 270; g.TranslateTransform(0, 9); }
                        else if (cschip.description == "左の壁") { rad = 300; g.TranslateTransform(0, 9); }
                        brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
                        g.FillEllipse(brush, 16 - 7, 16 - 7, 14, 14);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        g.FillPolygon(brush, vo_pa);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.description == "天井")
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 + 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 + 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = 0;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = 0;
                        }
                        else if (cschip.description == "右の壁")
                        {
                            vo_pa[0].X = 16 - 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 - 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = chipsize.Width;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = chipsize.Width;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else if (cschip.description == "左の壁")
                        {
                            vo_pa[0].X = 16 + 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 + 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = 0;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = 0;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 - 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 - 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = chipsize.Width - 3;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = chipsize.Width - 3;
                        }
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        if (cschip.description == "右向き") { rad = 330; g.TranslateTransform(0, 3); }
                        else if (cschip.description == "左向き") { rad = 225; g.TranslateTransform(0, 3); }
                        else if (cschip.description == "天井") { rad = 30; g.TranslateTransform(0, 0); }
                        else if (cschip.description == "右の壁") { rad = 270; g.TranslateTransform(0, 0); }
                        else if (cschip.description == "左の壁") { rad = 300; g.TranslateTransform(0, 0); }
                        brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
                        g.FillEllipse(brush, height / 2 - 3, height / 2 - 3, 6, 6);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = height / 2 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 3;
                        vo_pa[0].Y = height / 2 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 3;
                        vo_pa[1].X = height / 2 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 3;
                        vo_pa[1].Y = height / 2 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 3;
                        vo_pa[2].X = height / 2 + (float)Math.Cos((rad * Math.PI) / 180) * 6 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 3;
                        vo_pa[2].Y = height / 2 + (float)Math.Sin((rad * Math.PI) / 180) * 6 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 3;
                        vo_pa[3].X = height / 2 + (float)Math.Cos((rad * Math.PI) / 180) * 6 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 3;
                        vo_pa[3].Y = height / 2 + (float)Math.Sin((rad * Math.PI) / 180) * 6 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 3;
                        g.FillPolygon(brush, vo_pa);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.description == "天井")
                        {
                            vo_pa[0].X = height / 2 - 1;
                            vo_pa[0].Y = height / 2;
                            vo_pa[1].X = height / 2 + 1;
                            vo_pa[1].Y = height / 2;
                            vo_pa[2].X = height / 2 + 2;
                            vo_pa[2].Y = 0;
                            vo_pa[3].X = height / 2 - 2;
                            vo_pa[3].Y = 0;
                        }
                        else if (cschip.description == "右の壁")
                        {
                            vo_pa[0].X = height / 2;
                            vo_pa[0].Y = height / 2 - 1;
                            vo_pa[1].X = height / 2;
                            vo_pa[1].Y = height / 2 + 1;
                            vo_pa[2].X = height;
                            vo_pa[2].Y = height / 2 + 2;
                            vo_pa[3].X = height;
                            vo_pa[3].Y = height / 2 - 2;
                        }
                        else if (cschip.description == "左の壁")
                        {
                            vo_pa[0].X = height / 2;
                            vo_pa[0].Y = height / 2 - 1;
                            vo_pa[1].X = height / 2;
                            vo_pa[1].Y = height / 2 + 1;
                            vo_pa[2].X = 0;
                            vo_pa[2].Y = height / 2 + 2;
                            vo_pa[3].X = 0;
                            vo_pa[3].Y = height / 2 - 2;
                        }
                        else
                        {
                            vo_pa[0].X = height / 2 - 1;
                            vo_pa[0].Y = height / 2;
                            vo_pa[1].X = height / 2 + 1;
                            vo_pa[1].Y = height / 2;
                            vo_pa[2].X = height / 2 + 2;
                            vo_pa[2].Y = height - 3;
                            vo_pa[3].X = height / 2 - 2;
                            vo_pa[3].Y = height - 3;
                        }
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        if (cschip.description == "右向き") { rad = 330; g.TranslateTransform(-9, 3); }
                        else if (cschip.description == "左向き") { rad = 225; g.TranslateTransform(9, 3); }
                        else if (cschip.description == "天井") { rad = 30; g.TranslateTransform(-9, 0); }
                        else if (cschip.description == "右の壁") { rad = 270; g.TranslateTransform(0, 9); }
                        else if (cschip.description == "左の壁") { rad = 300; g.TranslateTransform(0, 9); }
                        brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
                        g.FillEllipse(brush, 16 - 7, 16 - 7, 14, 14);
                        vo_pa = new PointF[4];
                        vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
                        vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
                        vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
                        g.FillPolygon(brush, vo_pa);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.description == "天井")
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 + 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 + 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = 0;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = 0;
                        }
                        else if (cschip.description == "右の壁")
                        {
                            vo_pa[0].X = 16 - 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 - 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = chipsize.Width;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = chipsize.Width;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else if (cschip.description == "左の壁")
                        {
                            vo_pa[0].X = 16 + 1;
                            vo_pa[0].Y = 16 - 2;
                            vo_pa[1].X = 16 + 1;
                            vo_pa[1].Y = 16 + 2;
                            vo_pa[2].X = 0;
                            vo_pa[2].Y = 16 + 5;
                            vo_pa[3].X = 0;
                            vo_pa[3].Y = 16 - 5;
                        }
                        else
                        {
                            vo_pa[0].X = 16 - 2;
                            vo_pa[0].Y = 16 - 1;
                            vo_pa[1].X = 16 + 2;
                            vo_pa[1].Y = 16 - 1;
                            vo_pa[2].X = 16 + 5;
                            vo_pa[2].Y = chipsize.Width - 3;
                            vo_pa[3].X = 16 - 5;
                            vo_pa[3].Y = chipsize.Width - 3;
                        }
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"曲線による上り坂", new Athletic(
                    (cschip, g, chipsize) => {
                        var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
                        if (cschip.description.Contains("線のみ"))
                        {
                            vo_pa = new PointF[11];
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) - 1;
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            g.DrawLines(pen, vo_pa);
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) + 1;
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }
                            g.DrawLines(pen, vo_pa);
                            vo_pa = new PointF[2];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            g.DrawLines(pen, vo_pa);
                            pen.Dispose();
                        }
                        else
                        {
                            vo_pa = new PointF[13];
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = j20;
                            vo_pa[k21].Y = chipsize.Height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = chipsize.Width;
                            vo_pa[k21].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[13];
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = l20;
                            vo_pa[k21].Y = chipsize.Height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = chipsize.Width;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
                            vo_pa[k21].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            vo_pa[2].X = l20;
                            vo_pa[2].Y = chipsize.Height;
                            vo_pa[3].X = j20;
                            vo_pa[3].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
                        if (cschip.description.Contains("線のみ"))
                        {
                            vo_pa = new PointF[11];
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * 160);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(256 - Math.Sin((i1 * math_pi) / 180) * 160);
                                vo_pa[k21].Y = (float)Math.Floor(-32 + Math.Cos((i1 * math_pi) / 180) * 160) - 1;
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            g.DrawLines(pen, vo_pa);
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(256 - Math.Sin((i1 * math_pi) / 180) * 160);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * 160);
                                vo_pa[k21].Y = (float)Math.Floor(160 - Math.Cos((i1 * math_pi) / 180) * 160) + 1;
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }
                            g.DrawLines(pen, vo_pa);
                            vo_pa = new PointF[2];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            g.DrawLines(pen, vo_pa);
                            pen.Dispose();
                        }
                        else
                        {
                            vo_pa = new PointF[12];
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * 160);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(256 - Math.Sin((i1 * math_pi) / 180) * 160);
                                vo_pa[k21].Y = (float)Math.Floor(-32 + Math.Cos((i1 * math_pi) / 180) * 160);
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = j20;
                            vo_pa[k21].Y = 128;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[13];
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(256 - Math.Sin((i1 * math_pi) / 180) * 160);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * 160);
                                vo_pa[k21].Y = (float)Math.Floor(160 - Math.Cos((i1 * math_pi) / 180) * 160);
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = l20;
                            vo_pa[k21].Y = 128;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = 256;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
                            vo_pa[k21].Y = 128;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            vo_pa[2].X = l20;
                            vo_pa[2].Y = 128;
                            vo_pa[3].X = j20;
                            vo_pa[3].Y = 128;
                            g.FillPolygon(brush, vo_pa);
                            if (128 + base_y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
                                g.FillRectangle(brush, 0, 128, 256, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (128 + base_y * chipsize.Height));
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
                        if (cschip.description.Contains("線のみ"))
                        {
                            vo_pa = new PointF[11];
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Width * 5 / 8) - 1;
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            g.DrawLines(pen, vo_pa);
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Width * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Width * 5 / 8) + 1;
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }
                            g.DrawLines(pen, vo_pa);
                            vo_pa = new PointF[2];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            g.DrawLines(pen, vo_pa);
                            pen.Dispose();
                        }
                        else
                        {
                            vo_pa = new PointF[13];
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = j20;
                            vo_pa[k21].Y = chipsize.Width;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = chipsize.Width;
                            vo_pa[k21].Y = chipsize.Width;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[13];
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Width * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = l20;
                            vo_pa[k21].Y = chipsize.Width;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = chipsize.Width;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
                            vo_pa[k21].Y = chipsize.Width;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            vo_pa[2].X = l20;
                            vo_pa[2].Y = chipsize.Width;
                            vo_pa[3].X = j20;
                            vo_pa[3].Y = chipsize.Width;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize, height) => {
                        var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
                        if (cschip.description.Contains("線のみ"))
                        {
                            vo_pa = new PointF[11];
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(height - Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * height * 5 / 8) - 1;
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            g.DrawLines(pen, vo_pa);
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(height - Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * height * 5 / 8) + 1;
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }
                            g.DrawLines(pen, vo_pa);
                            vo_pa = new PointF[2];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            g.DrawLines(pen, vo_pa);
                            pen.Dispose();
                        }
                        else
                        {
                            vo_pa = new PointF[13];
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(height - Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * height * 5 / 8);
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = j20;
                            vo_pa[k21].Y = height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = height;
                            vo_pa[k21].Y = height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[13];
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(height - Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * height * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * height * 5 / 8);
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = l20;
                            vo_pa[k21].Y = height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = height;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
                            vo_pa[k21].Y = height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            vo_pa[2].X = l20;
                            vo_pa[2].Y = height;
                            vo_pa[3].X = j20;
                            vo_pa[3].Y = height;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
                        if (cschip.description.Contains("線のみ"))
                        {
                            vo_pa = new PointF[11];
                            pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) - 1;
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            g.DrawLines(pen, vo_pa);
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) + 1;
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }
                            g.DrawLines(pen, vo_pa);
                            vo_pa = new PointF[2];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            g.DrawLines(pen, vo_pa);
                            pen.Dispose();
                        }
                        else
                        {
                            vo_pa = new PointF[13];
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
                                if (i1 == 50)
                                {
                                    j20 = vo_pa[k21].X;
                                    k20 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = j20;
                            vo_pa[k21].Y = chipsize.Height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = chipsize.Width;
                            vo_pa[k21].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[13];
                            k21 = 0;
                            for (var i1 = 0; i1 <= 50; i1 += 5)
                            {
                                if (cschip.name.Contains("上"))
                                    vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                else if (cschip.name.Contains("下"))
                                    vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
                                vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
                                if (i1 == 50)
                                {
                                    l20 = vo_pa[k21].X;
                                    i21 = vo_pa[k21].Y;
                                }
                                k21++;
                            }

                            vo_pa[k21].X = l20;
                            vo_pa[k21].Y = chipsize.Height;
                            k21++;
                            if (cschip.name.Contains("上")) vo_pa[k21].X = chipsize.Width;
                            else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
                            vo_pa[k21].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            vo_pa = new PointF[4];
                            vo_pa[0].X = j20;
                            vo_pa[0].Y = k20;
                            vo_pa[1].X = l20;
                            vo_pa[1].Y = i21;
                            vo_pa[2].X = l20;
                            vo_pa[2].Y = chipsize.Height;
                            vo_pa[3].X = j20;
                            vo_pa[3].Y = chipsize.Height;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    })
                },
                {"乗れる円", new Athletic(
                    (cschip, g, chipsize) => {
                        int radius = default;
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        if (cschip.name == "円")
                        {
                            brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
                            if (cschip.description.Contains("乗ると下がる"))
                            {
                                g.TranslateTransform(0, 3);
                                radius = 80 / 10;
                            }
                            else
                            {
                                radius = 112 / 10;
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            if (cschip.description.Contains("大"))
                            {
                                if (cschip.name == "乗れる円")
                                {
                                    radius = 144 / 10;
                                }
                                else if (cschip.name == "跳ねる円")
                                {
                                    radius = 128 / 10;
                                }
                            }
                            else
                            {
                                radius = 96 / 10;
                            }
                        }
                        g.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        int radius = default;
                        g.TranslateTransform(0, 16);
                        if (cschip.name == "円")
                        {
                            brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
                            if (cschip.description.Contains("乗ると下がる")){
                                radius = 80;
                            } else {
                                radius = 112; g.TranslateTransform(0, 24);
                                if(cschip.description.Contains("下から")) g.TranslateTransform(0, 106);
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            if (cschip.description.Contains("大")) {
                                if (cschip.name == "乗れる円") {
                                    radius = 144;
                                } else if (cschip.name == "跳ねる円") {
                                    radius = 128; g.TranslateTransform(16, 0);
                                }
                            }
                            else
                            {
                                radius = 96; g.TranslateTransform(16, 0);
                            }
                        }
                        g.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        int radius = default;
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        if (cschip.name == "円")
                        {
                            brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
                            if (cschip.description.Contains("乗ると下がる"))
                            {
                                radius = 80 / 10;
                            }
                            else
                            {
                                radius = 112 / 10;
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            if (cschip.description.Contains("大"))
                            {
                                if (cschip.name == "乗れる円")
                                {
                                    radius = 144 / 10;
                                }
                                else if (cschip.name == "跳ねる円")
                                {
                                    radius = 128 / 10;
                                }
                            }
                            else
                            {
                                radius = 96 / 10;
                            }
                        }
                        g.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        int radius = default;
                        g.TranslateTransform(height / 2, height / 2);
                        if (cschip.name == "円")
                        {
                            brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
                            if (cschip.description.Contains("乗ると下がる"))
                            {
                                radius = 80 / 25;
                            }
                            else
                            {
                                radius = 112 / 25;
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            if (cschip.description.Contains("大"))
                            {
                                if (cschip.name == "乗れる円")
                                {
                                    radius = 144 / 25;
                                }
                                else if (cschip.name == "跳ねる円")
                                {
                                    radius = 128 / 25;
                                }
                            }
                            else
                            {
                                radius = 96 / 25;
                            }
                        }
                        g.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        int radius = default;
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        if (cschip.name == "円")
                        {
                            brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
                            if (cschip.description.Contains("乗ると下がる"))
                            {
                                radius = 80 / 10;
                            }
                            else
                            {
                                radius = 112 / 10;
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            if (cschip.description.Contains("大"))
                            {
                                if (cschip.name == "乗れる円")
                                {
                                    radius = 144 / 10;
                                }
                                else if (cschip.name == "跳ねる円")
                                {
                                    radius = 128 / 10;
                                }
                            }
                            else
                            {
                                radius = 96 / 10;
                            }
                        }
                        g.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
                        brush.Dispose();
                    })
                },
                {"半円", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(1, 2);
                        if (cschip.description.Contains("乗れる"))
                        {
                            if (cschip.description.Contains("線のみ"))
                            {
                                pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                                vo_pa = new PointF[12];
                                var j21 = 0;
                                vo_pa[j21].X = 1 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                g.DrawLines(pen, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                g.DrawLines(pen, vo_pa);
                                pen.Dispose();
                            }
                            else
                            {
                                brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                vo_pa = new PointF[13];
                                var j21 = 0;
                                vo_pa[j21].X = 0;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                brush.Dispose();
                            }
                        }
                        else
                        {
                            g.TranslateTransform(0, 10);
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillRectangle(brush, (120 - 20) / 8, 64 / 8, 40 / 8, 12);
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa = new PointF[13];
                            var j21 = 0;
                            vo_pa[j21].X = 0;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            for (var j = 140; j >= 90; j -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            j21 = 0;
                            for (var k2 = 90; k2 >= 40; k2 -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 240 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        if (cschip.description.Contains("乗れる"))
                        {
                            if (cschip.description.Contains("線のみ"))
                            {
                                pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                                vo_pa = new PointF[12];
                                var j21 = 0;
                                vo_pa[j21].X = 1;
                                vo_pa[j21].Y = 63;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144);
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144);
                                    j21++;
                                }
                                g.DrawLines(pen, vo_pa);
                                j21 = 0;
                                for (var k = 90; k >= 40; k -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k * math_pi) / 180) * 144);
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k * math_pi) / 180) * 144);
                                    j21++;
                                }
                                vo_pa[j21].X = 240;
                                vo_pa[j21].Y = 63;
                                g.DrawLines(pen, vo_pa);
                                pen.Dispose();
                            }
                            else
                            {
                                brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                vo_pa = new PointF[13];
                                var j21 = 0;
                                vo_pa[j21].X = 0;
                                vo_pa[j21].Y = 64;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144);
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144);
                                    j21++;
                                }

                                vo_pa[j21].X = 120;
                                vo_pa[j21].Y = 64;
                                g.FillPolygon(brush, vo_pa);
                                j21 = 0;
                                for (var k = 90; k >= 40; k -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k * math_pi) / 180) * 144);
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k * math_pi) / 180) * 144);
                                    j21++;
                                }

                                vo_pa[j21].X = 240;
                                vo_pa[j21].Y = 64;
                                j21++;
                                vo_pa[j21].X = 120;
                                vo_pa[j21].Y = 64;
                                g.FillPolygon(brush, vo_pa);
                                brush.Dispose();
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            if (64 + base_y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
                                g.FillRectangle(brush, 120 - 20, 64, 40, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (64 + base_y * chipsize.Height));
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa = new PointF[13];
                            var j21 = 0;
                            vo_pa[j21].X = 0;
                            vo_pa[j21].Y = 64;
                            j21++;
                            for (var j = 140; j >= 90; j -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144);
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144);
                                j21++;
                            }

                            vo_pa[j21].X = 120;
                            vo_pa[j21].Y = 64;
                            g.FillPolygon(brush, vo_pa);
                            j21 = 0;
                            for (var k = 90; k >= 40; k -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k * math_pi) / 180) * 144);
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k * math_pi) / 180) * 144);
                                j21++;
                            }

                            vo_pa[j21].X = 240;
                            vo_pa[j21].Y = 64;
                            j21++;
                            vo_pa[j21].X = 120;
                            vo_pa[j21].Y = 64;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(1, 2);
                        if (cschip.description.Contains("乗れる"))
                        {
                            if (cschip.description.Contains("線のみ"))
                            {
                                pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                                vo_pa = new PointF[12];
                                var j21 = 0;
                                vo_pa[j21].X = 1 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                g.DrawLines(pen, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                g.DrawLines(pen, vo_pa);
                                pen.Dispose();
                            }
                            else
                            {
                                brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                vo_pa = new PointF[13];
                                var j21 = 0;
                                vo_pa[j21].X = 0;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                brush.Dispose();
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillRectangle(brush, (120 - 20) / 8, 64 / 8 - 1, 40 / 8, 22);
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa = new PointF[13];
                            var j21 = 0;
                            vo_pa[j21].X = 0;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            for (var j = 140; j >= 90; j -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            j21 = 0;
                            for (var k2 = 90; k2 >= 40; k2 -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 240 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(1, 2);
                        if (cschip.description.Contains("乗れる"))
                        {
                            if (cschip.description.Contains("線のみ"))
                            {
                                pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                                vo_pa = new PointF[12];
                                var j21 = 0;
                                vo_pa[j21].X = 1 / 20;
                                vo_pa[j21].Y = 63 / 20;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 20;
                                    j21++;
                                }
                                g.DrawLines(pen, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
                                    j21++;
                                }
                                vo_pa[j21].X = 240 / 20;
                                vo_pa[j21].Y = 63 / 20;
                                g.DrawLines(pen, vo_pa);
                                pen.Dispose();
                            }
                            else
                            {
                                brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                vo_pa = new PointF[13];
                                var j21 = 0;
                                vo_pa[j21].X = 0;
                                vo_pa[j21].Y = 64 / 20;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 20;
                                    j21++;
                                }

                                vo_pa[j21].X = 120 / 20;
                                vo_pa[j21].Y = 64 / 20;
                                g.FillPolygon(brush, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
                                    j21++;
                                }

                                vo_pa[j21].X = 240 / 20;
                                vo_pa[j21].Y = 64 / 20;
                                j21++;
                                vo_pa[j21].X = 120 / 20;
                                vo_pa[j21].Y = 64 / 20;
                                g.FillPolygon(brush, vo_pa);
                                brush.Dispose();
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillRectangle(brush, (120 - 20) / 20, 64 / 20 - 1, 40 / 20, 7);
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa = new PointF[13];
                            var j21 = 0;
                            vo_pa[j21].X = 0;
                            vo_pa[j21].Y = 64 / 20;
                            j21++;
                            for (var j = 140; j >= 90; j -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 20;
                                j21++;
                            }

                            vo_pa[j21].X = 120 / 20;
                            vo_pa[j21].Y = 64 / 20;
                            g.FillPolygon(brush, vo_pa);
                            j21 = 0;
                            for (var k2 = 90; k2 >= 40; k2 -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
                                j21++;
                            }

                            vo_pa[j21].X = 240 / 20;
                            vo_pa[j21].Y = 64 / 20;
                            j21++;
                            vo_pa[j21].X = 120 / 20;
                            vo_pa[j21].Y = 64 / 20;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(1, 2);
                        if (cschip.description.Contains("乗れる"))
                        {
                            if (cschip.description.Contains("線のみ"))
                            {
                                pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
                                vo_pa = new PointF[12];
                                var j21 = 0;
                                vo_pa[j21].X = 1 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                g.DrawLines(pen, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }
                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 63 / 8;
                                g.DrawLines(pen, vo_pa);
                                pen.Dispose();
                            }
                            else
                            {
                                brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                vo_pa = new PointF[13];
                                var j21 = 0;
                                vo_pa[j21].X = 0;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                for (var j = 140; j >= 90; j -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                j21 = 0;
                                for (var k2 = 90; k2 >= 40; k2 -= 5)
                                {
                                    vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                    vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                    j21++;
                                }

                                vo_pa[j21].X = 240 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                j21++;
                                vo_pa[j21].X = 120 / 8;
                                vo_pa[j21].Y = 64 / 8;
                                g.FillPolygon(brush, vo_pa);
                                brush.Dispose();
                            }
                        }
                        else
                        {
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillRectangle(brush, (120 - 20) / 8, 64 / 8 - 1, 40 / 8, 22);
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa = new PointF[13];
                            var j21 = 0;
                            vo_pa[j21].X = 0;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            for (var j = 140; j >= 90; j -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            j21 = 0;
                            for (var k2 = 90; k2 >= 40; k2 -= 5)
                            {
                                vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
                                vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
                                j21++;
                            }

                            vo_pa[j21].X = 240 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            j21++;
                            vo_pa[j21].X = 120 / 8;
                            vo_pa[j21].Y = 64 / 8;
                            g.FillPolygon(brush, vo_pa);
                            brush.Dispose();
                        }
                    })
                },
                {"ファイヤーバー", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width / 2, chipsize.Height / 2),
                            new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Height / 2)), GraphicsUnit.Pixel);
                        int v = 225;
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        rad = ((v + 90) * Math.PI) / 180;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
                        vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
                        vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
                        vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
                        vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
                        vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
                        vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
                        vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
						// 内側の色を描画
						brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
                        vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
                        vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
                        vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
                        vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
                        vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
                        vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
                        vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        int width = default;
                        g.DrawImage(gd.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width, chipsize.Height),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(16, 16);

                        int v = default;
                        if (cschip.name == "ファイヤーバー2本" || cschip.name == "ファイヤーバー3本　左回り") v = 360 - 2;
                        else if (cschip.name == "ファイヤーバー3本　右回り") v = 2;
                        else if (cschip.name == "スウィングファイヤーバー")
                        {
                            if (cschip.description.Contains("左")) v = 160 + 2;
                            else v = 380 - 2;
                        }
                        else if (cschip.description == "左回り") v = 360 - 3;
                        else if (cschip.description == "右回り") v = 3;
                        rad = ((v + 90) * Math.PI) / 180;

                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];

                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        if (cschip.name == "ファイヤーバー") width = 140;
                        else width = 172;
                        vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 16);
                        vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 16);
                        vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 16);
                        vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 16);
                        vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * width) - Math.Cos(rad) * 16);
                        vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * width) - Math.Sin(rad) * 16);
                        vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * width) + Math.Cos(rad) * 16);
                        vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * width) + Math.Sin(rad) * 16);
                        g.FillPolygon(brush, vo_pa);

						// 内側の色を描画
						brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        if (cschip.name == "ファイヤーバー") width = 134;
                        else width = 166;
                        vo_pa[0].X = (float)(Math.Cos(v * d) * 31 + Math.Cos(rad) * 10);
                        vo_pa[0].Y = (float)(Math.Sin(v * d) * 31 + Math.Sin(rad) * 10);
                        vo_pa[1].X = (float)(Math.Cos(v * d) * 31 - Math.Cos(rad) * 10);
                        vo_pa[1].Y = (float)(Math.Sin(v * d) * 31 - Math.Sin(rad) * 10);
                        vo_pa[2].X = (float)(Math.Cos(v * d) * width - Math.Cos(rad) * 10);
                        vo_pa[2].Y = (float)(Math.Sin(v * d) * width - Math.Sin(rad) * 10);
                        vo_pa[3].X = (float)(Math.Cos(v * d) * width + Math.Cos(rad) * 10);
                        vo_pa[3].Y = (float)(Math.Sin(v * d) * width + Math.Sin(rad) * 10);
                        g.FillPolygon(brush, vo_pa);
                        if (cschip.name != "ファイヤーバー" && cschip.name != "スウィングファイヤーバー")
                        {
                            if (cschip.name == "ファイヤーバー2本") v = 2;
                            else if (cschip.name == "ファイヤーバー3本　左回り") v = 360 - 2 + 120;
                            else if (cschip.name == "ファイヤーバー3本　右回り") v = 2 + 120;
                            rad = ((v + 90) * Math.PI) / 180;

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            width = 172;
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 16);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 16);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 16);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 16);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * width) - Math.Cos(rad) * 16);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * width) - Math.Sin(rad) * 16);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * width) + Math.Cos(rad) * 16);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * width) + Math.Sin(rad) * 16);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            width = 166;
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 31 + Math.Cos(rad) * 10);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 31 + Math.Sin(rad) * 10);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 31 - Math.Cos(rad) * 10);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 31 - Math.Sin(rad) * 10);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * width - Math.Cos(rad) * 10);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * width - Math.Sin(rad) * 10);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * width + Math.Cos(rad) * 10);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * width + Math.Sin(rad) * 10);
                            g.FillPolygon(brush, vo_pa);

                            if (cschip.name != "ファイヤーバー2本")
                            {
                                if (cschip.name == "ファイヤーバー3本　左回り") v = 360 - 2 + 240;
                                else if (cschip.name == "ファイヤーバー3本　右回り") v = 2 + 240;
                                rad = ((v + 90) * Math.PI) / 180;

                                brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                                width = 172;
                                vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 16);
                                vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 16);
                                vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 16);
                                vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 16);
                                vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * width) - Math.Cos(rad) * 16);
                                vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * width) - Math.Sin(rad) * 16);
                                vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * width) + Math.Cos(rad) * 16);
                                vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * width) + Math.Sin(rad) * 16);
                                g.FillPolygon(brush, vo_pa);

								// 内側の色を描画
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                                width = 166;
                                vo_pa[0].X = (float)(Math.Cos(v * d) * 31 + Math.Cos(rad) * 10);
                                vo_pa[0].Y = (float)(Math.Sin(v * d) * 31 + Math.Sin(rad) * 10);
                                vo_pa[1].X = (float)(Math.Cos(v * d) * 31 - Math.Cos(rad) * 10);
                                vo_pa[1].Y = (float)(Math.Sin(v * d) * 31 - Math.Sin(rad) * 10);
                                vo_pa[2].X = (float)(Math.Cos(v * d) * width - Math.Cos(rad) * 10);
                                vo_pa[2].Y = (float)(Math.Sin(v * d) * width - Math.Sin(rad) * 10);
                                vo_pa[3].X = (float)(Math.Cos(v * d) * width + Math.Cos(rad) * 10);
                                vo_pa[3].Y = (float)(Math.Sin(v * d) * width + Math.Sin(rad) * 10);
                                g.FillPolygon(brush, vo_pa);
                            }
                        }
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width / 2, chipsize.Width / 2),
                            new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Width / 2)), GraphicsUnit.Pixel);
                        int v = 225;
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        rad = ((v + 90) * Math.PI) / 180;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
                        vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
                        vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
                        vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
                        vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
                        vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
                        vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
                        vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
						// 内側の色を描画
						brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
                        vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
                        vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
                        vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
                        vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
                        vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
                        vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
                        vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(height / 2, height / 2);
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, height / 2, height / 2),
                            new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Height / 2)), GraphicsUnit.Pixel);
                        int v = 225;
                        g.TranslateTransform(height / 2, height / 2);
                        rad = ((v + 90) * Math.PI) / 180;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 9) + Math.Cos(rad) * 2);
                        vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 9) + Math.Sin(rad) * 2);
                        vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 9) - Math.Cos(rad) * 2);
                        vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 9) - Math.Sin(rad) * 2);
                        vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 2);
                        vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 2);
                        vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 2);
                        vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 2);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
						// 内側の色を描画
						brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        vo_pa[0].X = (float)(Math.Cos(v * d) * 11 + Math.Cos(rad) * 1);
                        vo_pa[0].Y = (float)(Math.Sin(v * d) * 11 + Math.Sin(rad) * 1);
                        vo_pa[1].X = (float)(Math.Cos(v * d) * 11 - Math.Cos(rad) * 1);
                        vo_pa[1].Y = (float)(Math.Sin(v * d) * 11 - Math.Sin(rad) * 1);
                        vo_pa[2].X = (float)(Math.Cos(v * d) * 14 - Math.Cos(rad) * 1);
                        vo_pa[2].Y = (float)(Math.Sin(v * d) * 14 - Math.Sin(rad) * 1);
                        vo_pa[3].X = (float)(Math.Cos(v * d) * 14 + Math.Cos(rad) * 1);
                        vo_pa[3].Y = (float)(Math.Sin(v * d) * 14 + Math.Sin(rad) * 1);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
                        g.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
                            new Rectangle(0, 0, chipsize.Width / 2, chipsize.Height / 2),
                            new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Height / 2)), GraphicsUnit.Pixel);
                        int v = 225;
                        g.TranslateTransform(chipsize.Width / 2 + 1, chipsize.Width / 2 + 1);
                        rad = ((v + 90) * Math.PI) / 180;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
                        vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
                        vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
                        vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
                        vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
                        vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
                        vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
                        vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillPolygon(brush, vo_pa);
						// 内側の色を描画
						brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
                        vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
                        vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
                        vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
                        vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
                        vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
                        vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
                        vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
                        g.FillPolygon(brush, vo_pa);
                        brush.Dispose();
                    })
                },
                {"人口太陽", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("棒５本")) n = 5;
                        else n = 3;
                        if (cschip.description.Contains("左")) v = 360 - 2;
                        else v = 2;

                        for (var ii = 0; ii < n; ii++)
                        {
                            v += 360 / n;
                            rad = ((v + 90) * Math.PI) / 180;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillEllipse(brush, -6, -6, 12, 12);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillEllipse(brush, -2, -2, 4, 4);

                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        int v = default, n = default;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("棒５本")) n = 5;
                        else n = 3;
                        if (cschip.description.Contains("左")) v = 360 - 2;
                        else v = 2;

                        for (var i = 0; i < n; i++)
                        {
                            v += 360 / n;
                            rad = ((v + 90) * Math.PI) / 180;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 16);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 16);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 16);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 16);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 172) - Math.Cos(rad) * 16);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 172) - Math.Sin(rad) * 16);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 172) + Math.Cos(rad) * 16);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 172) + Math.Sin(rad) * 16);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 31 + Math.Cos(rad) * 10);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 31 + Math.Sin(rad) * 10);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 31 - Math.Cos(rad) * 10);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 31 - Math.Sin(rad) * 10);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * 166 - Math.Cos(rad) * 10);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * 166 - Math.Sin(rad) * 10);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * 166 + Math.Cos(rad) * 10);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * 166 + Math.Sin(rad) * 10);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillEllipse(brush, -64, -64 + 8, 128, 128);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillEllipse(brush, -20, -20 + 8, 40, 40);

                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("棒５本")) n = 5;
                        else n = 3;
                        if (cschip.description.Contains("左")) v = 360 - 2;
                        else v = 2;

                        for (var ii = 0; ii < n; ii++)
                        {
                            v += 360 / n;
                            rad = ((v + 90) * Math.PI) / 180;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillEllipse(brush, -6, -6, 12, 12);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillEllipse(brush, -2, -2, 4, 4);

                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(height / 2, height / 2);

                        int v = default, n = default;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("棒５本")) n = 5;
                        else n = 3;
                        if (cschip.description.Contains("左")) v = 360 - 2;
                        else v = 2;

                        for (var ii = 0; ii < n; ii++)
                        {
                            v += 360 / n;
                            rad = ((v + 90) * Math.PI) / 180;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 1) + Math.Cos(rad) * 1);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 1) + Math.Sin(rad) * 1);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 1) - Math.Cos(rad) * 1);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 1) - Math.Sin(rad) * 1);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 6) - Math.Cos(rad) * 1);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 6) - Math.Sin(rad) * 1);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 6) + Math.Cos(rad) * 1);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 6) + Math.Sin(rad) * 1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 2 + Math.Cos(rad) * 0.5);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 2 + Math.Sin(rad) * 0.5);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 2 - Math.Cos(rad) * 0.5);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 2 - Math.Sin(rad) * 0.5);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * 5 - Math.Cos(rad) * 0.5);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * 5 - Math.Sin(rad) * 0.5);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * 5 + Math.Cos(rad) * 0.5);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * 5 + Math.Sin(rad) * 0.5);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillEllipse(brush, -2, -2, 4, 4);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillEllipse(brush, -1, -1, 2, 2);

                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        const double d = 0.017453292519943295;
                        vo_pa = new PointF[4];
                        if (cschip.description.Contains("棒５本")) n = 5;
                        else n = 3;
                        if (cschip.description.Contains("左")) v = 360 - 2;
                        else v = 2;

                        for (var ii = 0; ii < n; ii++)
                        {
                            v += 360 / n;
                            rad = ((v + 90) * Math.PI) / 180;
                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
                            vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
                            vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
                            vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
                            vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
                            vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
                            vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
                            vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
                            vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
                            vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
                            vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
                            vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
                            vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
                            vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
                            vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
                            g.FillPolygon(brush, vo_pa);
                        }
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                        g.FillEllipse(brush, -6, -6, 12, 12);
                        brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                        g.FillEllipse(brush, -2, -2, 4, 4);

                        brush.Dispose();
                    })
                },
                {"ファイヤーリング", new Athletic(
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        if (cschip.description.Contains("2本")) n = 2;
                        else n = 3;
                        if (cschip.description.Contains("左回り"))
                        {
                            if (cschip.description.Contains("高速")) v = -4 + 360;
                            else v = -2 + 360;
                        }
                        else
                        {
                            if (cschip.description.Contains("高速")) v = 4;
                            else v = 2;
                        }

                        brush = default;

                        for (var ii = 0; ii < n; ii++)
                        {
                            var k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[26];
                                for (var i4 = 0; i4 >= -120; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -120; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }
                            else
                            {
                                vo_pa = new PointF[12];
                                for (var i4 = 0; i4 >= -50; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -50; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[24];
                                for (var k4 = -5; k4 >= -115; k4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -115; l4 <= -5; l4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }
                            else
                            {
                                for (var k4 = -5; k4 >= -45; k4 -= 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -45; l4 <= -5; l4 += 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);

                            v += 360 / n;
                        }

                        brush.Dispose();
                    },
                    (cschip, g, chipsize, gd, base_y) => {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TranslateTransform(32*5,32*5);

                        int v = default, n = default;
                        if (cschip.description.Contains("2本")) n = 2;
                        else n = 3;
                        if (cschip.description.Contains("左回り")) {
                            if(cschip.description.Contains("高速")) v = -4 + 360;
                            else v = -2 + 360;
                        }
                        else
                        {
                            if (cschip.description.Contains("高速")) v = 4;
                            else v = 2;
                        }

                        brush = default;

                        for (var i = 0; i < n; i++)
                        {
                            var k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[26];
                                for (var i4 = 0; i4 >= -120; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * 160);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * 160);
                                    k6++;
                                }

                                for (var j4 = -120; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * 112);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * 112);
                                    k6++;
                                }
                            }
                            else
                            {
                                vo_pa = new PointF[12];
                                for (var i4 = 0; i4 >= -50; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * 160);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * 160);
                                    k6++;
                                }

                                for (var j4 = -50; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * 112);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * 112);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[24];
                                for (var k4 = -5; k4 >= -115; k4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * 148);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * 148);
                                    k6++;
                                }

                                for (var l4 = -115; l4 <= -5; l4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * 124);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * 124);
                                    k6++;
                                }
                            }
                            else
                            {
                                for (var k4 = -5; k4 >= -45; k4 -= 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * 148);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * 148);
                                    k6++;
                                }

                                for (var l4 = -45; l4 <= -5; l4 += 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * 124);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * 124);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);

                            v += 360 / n;
                        }

                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        if (cschip.description.Contains("2本")) n = 2;
                        else n = 3;
                        if (cschip.description.Contains("左回り"))
                        {
                            if (cschip.description.Contains("高速")) v = -4 + 360;
                            else v = -2 + 360;
                        }
                        else
                        {
                            if (cschip.description.Contains("高速")) v = 4;
                            else v = 2;
                        }

                        brush = default;

                        for (var ii = 0; ii < n; ii++)
                        {
                            var k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[26];
                                for (var i4 = 0; i4 >= -120; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -120; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }
                            else
                            {
                                vo_pa = new PointF[12];
                                for (var i4 = 0; i4 >= -50; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -50; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[24];
                                for (var k4 = -5; k4 >= -115; k4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -115; l4 <= -5; l4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }
                            else
                            {
                                for (var k4 = -5; k4 >= -45; k4 -= 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -45; l4 <= -5; l4 += 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);

                            v += 360 / n;
                        }

                        brush.Dispose();
                    },
                    (cschip, g, chipsize, height) => {
                        g.TranslateTransform(height / 2, height / 2);

                        int v = default, n = default;
                        if (cschip.description.Contains("2本")) n = 2;
                        else n = 3;
                        if (cschip.description.Contains("左回り"))
                        {
                            if (cschip.description.Contains("高速")) v = -4 + 360;
                            else v = -2 + 360;
                        }
                        else
                        {
                            if (cschip.description.Contains("高速")) v = 4;
                            else v = 2;
                        }

                        brush = default;

                        for (var ii = 0; ii < n; ii++)
                        {
                            var k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[26];
                                for (var i4 = 0; i4 >= -120; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * height / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * height / 2);
                                    k6++;
                                }

                                for (var j4 = -120; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * height / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * height / 2 * 0.3);
                                    k6++;
                                }
                            }
                            else
                            {
                                vo_pa = new PointF[12];
                                for (var i4 = 0; i4 >= -50; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * height / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * height / 2);
                                    k6++;
                                }

                                for (var j4 = -50; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * height / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * height / 2 * 0.3);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[24];
                                for (var k4 = -5; k4 >= -115; k4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * height / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * height / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -115; l4 <= -5; l4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * height / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * height / 2 * 0.5);
                                    k6++;
                                }
                            }
                            else
                            {
                                for (var k4 = -5; k4 >= -45; k4 -= 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * height / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * height / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -45; l4 <= -5; l4 += 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * height / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * height / 2 * 0.5);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);

                            v += 360 / n;
                        }

                        brush.Dispose();
                    },
                    (cschip, g, chipsize) => {
                        g.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

                        int v = default, n = default;
                        if (cschip.description.Contains("2本")) n = 2;
                        else n = 3;
                        if (cschip.description.Contains("左回り"))
                        {
                            if (cschip.description.Contains("高速")) v = -4 + 360;
                            else v = -2 + 360;
                        }
                        else
                        {
                            if (cschip.description.Contains("高速")) v = 4;
                            else v = 2;
                        }

                        brush = default;

                        for (var ii = 0; ii < n; ii++)
                        {
                            var k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[26];
                                for (var i4 = 0; i4 >= -120; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -120; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }
                            else
                            {
                                vo_pa = new PointF[12];
                                for (var i4 = 0; i4 >= -50; i4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
                                    k6++;
                                }

                                for (var j4 = -50; j4 <= 0; j4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
                            g.FillPolygon(brush, vo_pa);

							// 内側の色を描画
							k6 = 0;
                            if (cschip.description.Contains("2本"))
                            {
                                vo_pa = new PointF[24];
                                for (var k4 = -5; k4 >= -115; k4 -= 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -115; l4 <= -5; l4 += 10)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }
                            else
                            {
                                for (var k4 = -5; k4 >= -45; k4 -= 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
                                    k6++;
                                }

                                for (var l4 = -45; l4 <= -5; l4 += 8)
                                {
                                    vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
                                    k6++;
                                }
                            }

                            brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
                            g.FillPolygon(brush, vo_pa);

                            v += 360 / n;
                        }

                        brush.Dispose();
                    })
                },
            };
            list.Add("長いロープ", list["ロープ"]);
            list.Add("ゆれる棒", list["ロープ"]);
            list.Add("曲線による下り坂", list["曲線による上り坂"]);
            list.Add("跳ねる円", list["乗れる円"]);
            list.Add("円", list["乗れる円"]);
            list.Add("スウィングファイヤーバー", list["ファイヤーバー"]);
            list.Add("ファイヤーバー2本", list["ファイヤーバー"]);
            list.Add("ファイヤーバー3本　左回り", list["ファイヤーバー"]);
            list.Add("ファイヤーバー3本　右回り", list["ファイヤーバー"]);
        }
        public static Dictionary<string, Athletic> list;
    }
}
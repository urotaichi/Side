using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
    public class OverViewer : Control
    {
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
            SuspendLayout();
            Paint += OverViewer_Paint;
            ResumeLayout(false);
        }

        public new Size Size
        {
            get
            {
                if (Global.cpd.runtime == null)
                {
                    return new Size(180, 30);
                }
                return new Size(Source.Size.Width * ppb, Source.Size.Height * ppb);
            }
            set
            {
            }
        }

        public OverViewer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            CreateDrawSource();
        }

        public void CreateDrawSource()
        {
            if (Global.cpd.runtime == null)
            {
                return;
            }
            UpdateDrawSource();
        }

        public unsafe void UpdateDrawSource()
        {
            if (Global.state.MapEditMode)
            {
                Source = new Bitmap(Global.cpd.runtime.Definitions.MapSize.x, Global.cpd.runtime.Definitions.MapSize.y);
            }
            else
            {
                Source = new Bitmap(Global.MainWnd.MainDesigner.CurrentStageSize.x, Global.MainWnd.MainDesigner.CurrentStageSize.y);
            }
            BitmapData bitmapData = Source.LockBits(new Rectangle(new Point(0, 0), Source.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            try
            {
                byte* ptr = null;
                int num = 765 / Global.cpd.Mapchip.Length;
                Color color = default;
                ChipsData[] array = Global.cpd.Mapchip;
                if (!Global.state.EditingForeground)
                {
                    array = Global.cpd.Layerchip;
                }
                else if (Global.state.MapEditMode)
                {
                    array = Global.cpd.Worldchip;
                }
                Dictionary<string, ChipsData> dictionary = Global.MainWnd.MainDesigner.DrawItemRef;
                if (Global.cpd.project.Use3rdMapData)
                {
                    dictionary = Global.MainWnd.MainDesigner.DrawItemCodeRef;
                }
                if (!Global.state.EditingForeground)
                {
                    dictionary = Global.MainWnd.MainDesigner.DrawLayerRef;
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        dictionary = Global.MainWnd.MainDesigner.DrawLayerCodeRef;
                    }
                }
                else if (Global.state.MapEditMode)
                {
                    dictionary = Global.MainWnd.MainDesigner.DrawWorldRef;
                }
                if (Global.state.MapEditMode)
                {
                    for (int i = 0; i < Global.cpd.runtime.Definitions.MapSize.x; i++)
                    {
                        for (int j = 0; j < Global.cpd.runtime.Definitions.MapSize.y; j++)
                        {
                            ptr = (byte*)(void*)bitmapData.Scan0;
                            int num2 = i * 3 + bitmapData.Stride * j;
                            string key = Global.cpd.EditingMap[j].Substring(i * Global.cpd.runtime.Definitions.MapSize.bytesize, Global.cpd.runtime.Definitions.MapSize.bytesize);
                            if (dictionary.ContainsKey(key))
                            {
                                ChipsData chipsData = dictionary[key];
                                if (chipsData.color == "" || chipsData.color == null)
                                {
                                    if (chipsData.character.Equals(array[0].character))
                                    {
                                        color = Color.Black;
                                    }
                                    else
                                    {
                                        color = Color.White;
                                    }
                                }
                                else
                                {
                                    color = Color.FromName(chipsData.color);
                                }
                            }
                            else
                            {
                                color = Color.Black;
                            }
                            ptr[num2] = color.B;
                            ptr[num2 + 1] = color.G;
                            ptr[num2 + 2] = color.R;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Global.MainWnd.MainDesigner.CurrentStageSize.x; i++)
                    {
                        for (int j = 0; j < Global.MainWnd.MainDesigner.CurrentStageSize.y; j++)
                        {
                            ptr = (byte*)(void*)bitmapData.Scan0;
                            int num2 = i * 3 + bitmapData.Stride * j;
                            string key;
                            if (!Global.cpd.UseLayer || Global.state.EditingForeground)
                            {
                                if (Global.cpd.project.Use3rdMapData)
                                {
                                    key = Global.cpd.EditingMap[j].Split(',')[i];
                                }
                                else
                                {
                                    key = Global.cpd.EditingMap[j].Substring(i * Global.cpd.runtime.Definitions.StageSize.bytesize, Global.cpd.runtime.Definitions.StageSize.bytesize);
                                }
                            }
                            else
                            {
                                if (Global.cpd.project.Use3rdMapData)
                                {
                                    key = Global.cpd.EditingLayer[j].Split(',')[i];
                                }
                                else
                                {
                                    key = Global.cpd.EditingLayer[j].Substring(i * Global.cpd.runtime.Definitions.LayerSize.bytesize, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                                }
                            }
                            if (dictionary.ContainsKey(key))
                            {
                                ChipsData chipsData = dictionary[key];
                                if (chipsData.color == "" || chipsData.color == null)
                                {
                                    if (Global.cpd.project.Use3rdMapData)
                                    {
                                        if (chipsData.code == array[0].code)
                                        {
                                            color = Color.Black;
                                        }
                                    }
                                    else if (chipsData.character == array[0].character)
                                    {
                                        color = Color.Black;
                                    }
                                    else
                                    {
                                        color = Color.White;
                                    }
                                }
                                else
                                {
                                    color = Color.FromName(chipsData.color);
                                }
                            }
                            else
                            {
                                color = Color.Black;
                            }
                            ptr[num2] = color.B;
                            ptr[num2 + 1] = color.G;
                            ptr[num2 + 2] = color.R;
                        }
                    }
                }
            }
            finally
            {
                Source.UnlockBits(bitmapData);
            }
            Refresh();
        }

        private void OverViewer_Paint(object sender, PaintEventArgs e)
        {
            if (Source == null)
            {
                e.Graphics.FillRectangle(Brushes.Black, new Rectangle(default, Size));
                return;
            }
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(Source, new Rectangle(0, 0, Source.Width * ppb, Source.Height * ppb));
            Point mapPointTranslatedMap = Global.state.MapPointTranslatedMap;
            mapPointTranslatedMap.X *= ppb;
            mapPointTranslatedMap.Y *= ppb;
            using Pen pen = new Pen(Brushes.White, ppb);
            e.Graphics.DrawRectangle(pen, new Rectangle(mapPointTranslatedMap, new Size((int)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * ppb / Global.config.draw.ZoomIndex), (int)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * ppb / Global.config.draw.ZoomIndex))));
        }

        private readonly IContainer components;

        private Bitmap Source;

        public int ppb = 2;
    }
}

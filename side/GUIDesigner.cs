﻿using MasaoPlus.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MasaoPlus
{
    public class GUIDesigner : UserControl, IDisposable
    {
        private const string DEFAULT_PATTERN_IMAGE = "pattern.gif";
        private const string DEFAULT_LAYER_IMAGE = "mapchip.gif";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EditTool CurrentTool
        {
            get
            {
                return curTool;
            }
            set
            {
                BufferingDraw = value != EditTool.Pen;
                curTool = value;
                Refresh();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CopyPasteTool CopyPaste
        {
            get
            {
                return cpaste;
            }
            set
            {
                if (value == CopyPasteTool.None)
                {
                    BufferingDraw = CurrentTool != EditTool.Pen;
                }
                else
                {
                    BufferingDraw = true;
                }
                cpaste = value;
                Refresh();
            }
        }

        public event ChangeBuffer ChangeBufferInvoke;

        private bool BufferingDraw
        {
            get
            {
                return bdraw;
            }
            set
            {
                if (value && !bdraw)
                {
                    bufpos = -1;
                    Refresh();
                }
                bdraw = value;
            }
        }

        public void PaintStage(Graphics g, bool EnableExDraw)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            bool extendDraw = Global.config.draw.ExtendDraw;
            Global.config.draw.ExtendDraw = EnableExDraw;
            UpdateForegroundBuffer();
            if (CurrentProjectData.UseLayer)
            {
                UpdateBackgroundBuffer();
                if (Global.cpd.LayerCount <= Global.cpd.MainOrder)
                {
                    g.DrawImage(ForeLayerBmp, 0, 0, ForeLayerBmp.Width / DeviceDpi * 96, ForeLayerBmp.Height / DeviceDpi * 96);
                }
                for (int i = Global.cpd.LayerCount - 1; i >= 0; i--)
                {
                    g.DrawImage(BackLayerBmp[i], 0, 0, ForeLayerBmp.Width / DeviceDpi * 96, ForeLayerBmp.Height / DeviceDpi * 96);
                    if (i == Global.cpd.MainOrder)
                    {
                        g.DrawImage(ForeLayerBmp, 0, 0, ForeLayerBmp.Width / DeviceDpi * 96, ForeLayerBmp.Height / DeviceDpi * 96);
                    }
                }
            }
            else
            {
                g.DrawImage(ForeLayerBmp, 0, 0, ForeLayerBmp.Width / DeviceDpi * 96, ForeLayerBmp.Height / DeviceDpi * 96);
            }
            Global.config.draw.ExtendDraw = extendDraw;
            UpdateForegroundBuffer();
            UpdateBackgroundBuffer();
            InitTransparent();
            bufpos = -1;
            Refresh();
        }

        public void ClearBuffer()
        {
            StageBuffer.Clear();
            ChangeBufferInvoke?.Invoke();
        }

        public void AddBuffer()
        {
            if (StageBuffer.Count > BufferCurrent + 1)
            {
                StageBuffer.RemoveRange(BufferCurrent + 1, StageBuffer.Count - (BufferCurrent + 1));
            }
            if (Global.state.EditingForeground)
            {
                StageBuffer.Add((LayerObject)Global.cpd.EditingMap.Clone());
            }
            else
            {
                StageBuffer.Add((LayerObject)Global.cpd.EditingLayer.Clone());
            }
            BufferCurrent = StageBuffer.Count - 1;
            if (BufferCurrent != 0)
            {
                Global.state.EditFlag = true;
            }
            ChangeBufferInvoke?.Invoke();
        }

        private void ApplyBuffer(int newBufferCurrent)
        {
            Global.state.EditFlag = true;
            Global.MainWnd.UpdateStatus("描画しています...");
            BufferCurrent = newBufferCurrent;
            LayerObject clonedBuffer = (LayerObject)StageBuffer[BufferCurrent].Clone();

            if (Global.state.EditingForeground)
            {
                switch (Global.state.EdittingStage)
                {
                    case 0:
                        Global.cpd.project.StageData = clonedBuffer;
                        Global.cpd.EditingMap = Global.cpd.project.StageData;
                        break;
                    case 1:
                        Global.cpd.project.StageData2 = clonedBuffer;
                        Global.cpd.EditingMap = Global.cpd.project.StageData2;
                        break;
                    case 2:
                        Global.cpd.project.StageData3 = clonedBuffer;
                        Global.cpd.EditingMap = Global.cpd.project.StageData3;
                        break;
                    case 3:
                        Global.cpd.project.StageData4 = clonedBuffer;
                        Global.cpd.EditingMap = Global.cpd.project.StageData4;
                        break;
                    case 4:
                        Global.cpd.project.MapData = clonedBuffer;
                        Global.cpd.EditingMap = Global.cpd.project.MapData;
                        break;
                }
            }
            else
            {
                switch (Global.state.EdittingStage)
                {
                    case 0:
                        Global.cpd.project.LayerData[Global.state.EdittingLayerIndex] = clonedBuffer;
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData[Global.state.EdittingLayerIndex];
                        break;
                    case 1:
                        Global.cpd.project.LayerData2[Global.state.EdittingLayerIndex] = clonedBuffer;
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData2[Global.state.EdittingLayerIndex];
                        break;
                    case 2:
                        Global.cpd.project.LayerData3[Global.state.EdittingLayerIndex] = clonedBuffer;
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData3[Global.state.EdittingLayerIndex];
                        break;
                    case 3:
                        Global.cpd.project.LayerData4[Global.state.EdittingLayerIndex] = clonedBuffer;
                        Global.cpd.EditingLayer = Global.cpd.project.LayerData4[Global.state.EdittingLayerIndex];
                        break;
                }
            }
            ChangeBufferInvoke?.Invoke();
            StageSourceToDrawBuffer();
            Refresh();
            Global.MainWnd.UpdateStatus("完了");
        }

        public void Undo()
        {
            if (BufferCurrent <= 0)
            {
                return;
            }
            ApplyBuffer(BufferCurrent - 1);
        }

        public void Redo()
        {
            if (BufferCurrent >= StageBuffer.Count - 1)
            {
                return;
            }
            ApplyBuffer(BufferCurrent + 1);
        }

        public Size BufferSize
        {
            get
            {
                if (Global.state.EditingForeground)
                {
                    if (ForeLayerBmp != null)
                    {
                        return ForeLayerBmp.Size;
                    }
                }
                else if (BackLayerBmp[Global.state.EdittingLayerIndex] != null)
                {
                    return BackLayerBmp[Global.state.EdittingLayerIndex].Size;
                }
                return default;
            }
        }

        public Size DisplaySize
        {
            get
            {
                if (Global.state.EditingForeground)
                {
                    if (ForeLayerBmp != null)
                    {
                        return new Size((int)(ForeLayerBmp.Width * Global.config.draw.ZoomIndex), (int)(ForeLayerBmp.Height * Global.config.draw.ZoomIndex));
                    }
                }
                else if (BackLayerBmp[Global.state.EdittingLayerIndex] != null)
                {
                    return new Size((int)(BackLayerBmp[Global.state.EdittingLayerIndex].Width * Global.config.draw.ZoomIndex), (int)(BackLayerBmp[Global.state.EdittingLayerIndex].Height * Global.config.draw.ZoomIndex));
                }
                return default;
            }
        }

        public Size CurrentChipSize
        {
            get
            {
                Size result = default;
                if (Global.cpd.project == null)
                {
                    return result;
                }
                result.Width = (int)(Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
                result.Height = (int)(Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
                return LogicalToDeviceUnits(result);
            }
        }

        public static Runtime.DefinedData.StageSizeData CurrentStageSize
        {
            get
            {
                if (Global.state.EdittingStage == 0) return Global.cpd.project.Runtime.Definitions.StageSize;
                else if (Global.state.EdittingStage == 1) return Global.cpd.project.Runtime.Definitions.StageSize2;
                else if (Global.state.EdittingStage == 2) return Global.cpd.project.Runtime.Definitions.StageSize3;
                else return Global.cpd.project.Runtime.Definitions.StageSize4;
            }
        }

        public static Runtime.DefinedData.LayerSizeData CurrentLayerSize
        {
            get
            {
                if (Global.state.EdittingStage == 0) return Global.cpd.project.Runtime.Definitions.LayerSize;
                else if (Global.state.EdittingStage == 1) return Global.cpd.project.Runtime.Definitions.LayerSize2;
                else if (Global.state.EdittingStage == 2) return Global.cpd.project.Runtime.Definitions.LayerSize3;
                else return Global.cpd.project.Runtime.Definitions.LayerSize4;
            }
        }

        public GUIDesigner()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void StageSourceToDrawBuffer()
        {
            if (Global.state.EditingForeground)
            {
                UpdateForegroundBuffer();
                return;
            }
            UpdateBackgroundBuffer();
        }

        private void SavePrevDrawnLayer(bool editingForeground)
        {
            if (editingForeground)
            {
                ForePrevDrawn = (LayerObject)Global.cpd.EditingMap.Clone();
            }
            else
            {
                BackPrevDrawn = (LayerObject)Global.cpd.EditingLayer.Clone();
            }
        }

        public void InitTransparent()
        {
            if (CurrentProjectData.UseLayer && Global.state.TransparentUnactiveLayer)
            {
                if (Global.state.EditingForeground)
                {
                    InitTransparentForForeground();
                }
                else
                {
                    HalfTransparentBitmap2(ref ForeLayerBmp);
                    InitTransparentForBackground();
                }
            }
        }

        private void InitTransparentForForeground()
        {
            ProcessLayersTransparency(layerIndex => true);
        }

        public void InitTransparentForBackground()
        {
            ProcessLayersTransparency(layerIndex => 
            {
                if (layerIndex != Global.state.EdittingLayerIndex)
                {
                    UpdateSingleLayerBuffer(layerIndex);
                    return true;
                }
                return false;
            });
        }

        private void ProcessLayersTransparency(Func<int, bool> shouldProcess)
        {
            for (int i = 0; i < BackLayerBmp.Count; i++)
            {
                if (BackLayerBmp[i] != null && shouldProcess(i))
                {
                    Bitmap temp = BackLayerBmp[i];
                    HalfTransparentBitmap2(ref temp);
                    BackLayerBmp[i] = temp;
                }
            }
        }

        public void ForceBufferResize()
        {
            if (ForeLayerBmp != null)
            {
                ForeLayerBmp.Dispose();
                ForeLayerBmp = null;
            }
            for (int i = 0; i < BackLayerBmp.Count; i++)
            {
                if (BackLayerBmp[i] != null)
                {
                    BackLayerBmp[i].Dispose();
                    BackLayerBmp[i] = null;
                }
            }
            bufpos = -1;
        }

        private void DrawExtendSizeMap(ChipData cschip, Graphics g, Point p, bool foreground, string chara, int layerIndex)
        {
            bool oriboss_view = Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 3;
            GraphicsState transState;
            Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
            Rectangle rectangle = new(new Point(LogicalToDeviceUnits(p.X * chipsize.Width - cschip.center.X), LogicalToDeviceUnits(p.Y * chipsize.Height - cschip.center.Y)), LogicalToDeviceUnits(chipsize));
            if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd)
            { // 拡張画像　背面
                ChipRenderer.DrawExtendChip(g, rectangle, cschip.xdraw, chipsize);
            }
            if (foreground)
            { // 標準パターン画像
                if (oriboss_view && chara == "Z")
                {
                    if (DrawOribossOrig != null) g.DrawImage(DrawOribossOrig, new Rectangle(new(LogicalToDeviceUnits(p.X * chipsize.Width), LogicalToDeviceUnits(p.Y * chipsize.Height)), LogicalToDeviceUnits(DrawOribossOrig.Size)));
                }
                else
                {
                    transState = g.Save();
                    if (cschip.view_size != default)
                        TranslateTransform(g, p.X * chipsize.Width - cschip.center.X + cschip.view_size.Width / 2, p.Y * chipsize.Height - cschip.center.Y + cschip.view_size.Height / 2);
                    else
                        TranslateTransform(g, p.X * chipsize.Width - cschip.center.X + cschip.size.Width / 2, p.Y * chipsize.Height - cschip.center.Y + cschip.size.Height / 2);
                    g.RotateTransform(cschip.rotate);
                    if (cschip.scale != default) g.ScaleTransform(cschip.scale, cschip.scale);
                    g.DrawImage(DrawChipOrig,
                        new Rectangle(new Point(-LogicalToDeviceUnits(cschip.size.Width / 2), -LogicalToDeviceUnits(cschip.size.Height / 2)), LogicalToDeviceUnits(cschip.size)),
                        new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
                    g.Restore(transState);
                }
            }
            else
            { // 背景レイヤー画像
                g.DrawImage(DrawLayerOrig[layerIndex], rectangle, new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
            }
            if (chara == "Z" && oriboss_view &&
                Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string value) && Global.config.draw.ExtendDraw)
            {
                Point point = ChipRenderer.GetOribossExtensionPoint(int.Parse(value));
                g.DrawImage(DrawExOrig, new Rectangle(new Point(LogicalToDeviceUnits(p.X * chipsize.Width), LogicalToDeviceUnits(p.Y * chipsize.Height)), rectangle.Size), new Rectangle(point, chipsize), GraphicsUnit.Pixel);
            }
            else if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
            { // 拡張画像　前面
                ChipRenderer.DrawExtendChip(g, rectangle, cschip.xdraw, chipsize);
            }
        }
        private void DrawNormalSizeMap(ChipData cschip, Graphics g, Point p, bool foreground, string chara, int x, int layerIndex)
        {
            GraphicsState transState;
            Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
            Rectangle rectangle = new(new Point(LogicalToDeviceUnits(p.X * chipsize.Width), LogicalToDeviceUnits(p.Y * chipsize.Height)), LogicalToDeviceUnits(chipsize));
            if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd)
            { // 拡張画像　背面
                ChipRenderer.DrawExtendChip(g, rectangle, cschip.xdraw, chipsize);
            }
            if (foreground)
            { // 標準パターン画像
                transState = g.Save();
                TranslateTransform(g, p.X * chipsize.Width, p.Y * chipsize.Height);
                if (ChipRenderer.IsAthleticChip(cschip.name))
                {
                    AthleticView.list[cschip.name].Max(this, cschip, g, chipsize, this, p.Y);
                }
                else
                {
                    var rect = new Rectangle(new Point(-LogicalToDeviceUnits(chipsize.Width / 2), -LogicalToDeviceUnits(chipsize.Height / 2)), rectangle.Size);
                    TranslateTransform(g, chipsize.Width / 2, chipsize.Height / 2);
                    if (chara == Global.cpd.Mapchip[1].character)
                    {
                        g.ScaleTransform(-1, 1); // 基本主人公は逆向き
                        if (Global.state.MapEditMode && x > Global.cpd.runtime.Definitions.MapSize.x / 2 ||
                            Global.state.ChipRegister.ContainsKey("view_move_type") && int.Parse(Global.state.ChipRegister["view_move_type"]) == 2)
                        {
                            g.ScaleTransform(-1, 1);// 特殊条件下では元の向き
                        }
                    }
                    g.RotateTransform(cschip.rotate);
                    if (cschip.repeat != default)
                    {
                        for (int j = 0; j < cschip.repeat; j++)
                        {
                            g.DrawImage(DrawChipOrig,
                                new Rectangle(new Point(LogicalToDeviceUnits(-chipsize.Width / 2 + j * chipsize.Width * Math.Sign(cschip.rotate)), -LogicalToDeviceUnits(chipsize.Height / 2)), rectangle.Size),
                                new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        }
                    }
                    else if (cschip.repeat_x != default)
                    {
                        for (int j = 0; j < cschip.repeat_x; j++)
                        {
                            g.DrawImage(DrawChipOrig,
                                new Rectangle(new Point(LogicalToDeviceUnits(-chipsize.Width / 2 + j * chipsize.Width), -LogicalToDeviceUnits(chipsize.Height / 2)), rectangle.Size),
                                new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        }
                    }
                    else if (cschip.repeat_y != default)
                    {
                        for (int j = 0; j < cschip.repeat_y; j++)
                        {
                            g.DrawImage(DrawChipOrig,
                                new Rectangle(new Point(-LogicalToDeviceUnits(chipsize.Width / 2), LogicalToDeviceUnits(-chipsize.Height / 2 + j * chipsize.Height)), rectangle.Size),
                                new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                        }
                    }
                    else if (ChipRenderer.ShouldApplyWaterTransparency(out float water_clear_level) && chara == "4")
                    {// 水の半透明処理
                        ChipRenderer.ApplyWaterTransparency(g, DrawChipOrig, rect, cschip.pattern, chipsize, water_clear_level);
                    }
                    else g.DrawImage(DrawChipOrig, rect, new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                }
                g.Restore(transState);
            }
            else
            { // 背景レイヤー画像
                g.DrawImage(DrawLayerOrig[layerIndex], rectangle, new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
            }
            if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
            { // 拡張画像　前面
                ChipRenderer.DrawExtendChip(g, rectangle, cschip.xdraw, chipsize);
            }
        }

        public void UpdateForegroundBuffer()
        {
            bool flag = false;
            if (ForeLayerBmp == null)
            {
                if (Global.state.MapEditMode)
                {
                    ForeLayerBmp = new Bitmap(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.MapSize.x * Global.cpd.runtime.Definitions.ChipSize.Width), LogicalToDeviceUnits(Global.cpd.runtime.Definitions.MapSize.y * Global.cpd.runtime.Definitions.ChipSize.Height), PixelFormat.Format32bppArgb);
                }
                else
                {
                    ForeLayerBmp = new Bitmap(LogicalToDeviceUnits(CurrentStageSize.x * Global.cpd.runtime.Definitions.ChipSize.Width), CurrentStageSize.y * LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height), PixelFormat.Format32bppArgb);
                }
            }
            else if (!Global.state.UseBuffered)
            {
                flag = true;
            }
            using (Graphics graphics = Graphics.FromImage(ForeLayerBmp))
            {
                if (flag)
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, ForeLayerBmp.Width, ForeLayerBmp.Height));
                    graphics.CompositingMode = CompositingMode.SourceOver;
                }
                MakeDrawBuffer(graphics, true);
            }
            bufpos = -1;
        }

        public void UpdateBackgroundBuffer(int layerIndex = -1)
        {
            if (!CurrentProjectData.UseLayer)
            {
                return;
            }
            
            if (layerIndex == -1)
            {
                for (int i = Global.cpd.LayerCount - 1; i >= 0; i--)
                {
                    UpdateSingleLayerBuffer(i);
                }
            }
            else
            {
                UpdateSingleLayerBuffer(layerIndex);
            }
            
            bufpos = -1;
        }
        
        private void UpdateSingleLayerBuffer(int layerIndex)
        {
            bool flag = false;
            
            if (BackLayerBmp[layerIndex] == null)
            {
                BackLayerBmp[layerIndex] = new Bitmap(LogicalToDeviceUnits(CurrentLayerSize.x * Global.cpd.runtime.Definitions.ChipSize.Width), 
                    CurrentLayerSize.y * LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height), PixelFormat.Format32bppArgb);
            }
            else if (!Global.state.UseBuffered)
            {
                flag = true;
            }
            
            using Graphics graphics = Graphics.FromImage(BackLayerBmp[layerIndex]);
            if (flag)
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, BackLayerBmp[layerIndex].Width, BackLayerBmp[layerIndex].Height));
                graphics.CompositingMode = CompositingMode.SourceOver;
            }
            
            MakeDrawBuffer(graphics, false, layerIndex);
        }

        private void MakeDrawBuffer(Graphics g, bool foreground, int layerIndex = 0)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            void viewSecondHaikei(int type)
            {
                if (Global.state.ChipRegister.TryGetValue("second_gazou_scroll", out string second_gazou_scroll) && int.Parse(second_gazou_scroll) == 8 &&
                Global.state.ChipRegister.TryGetValue("second_gazou_priority", out string second_gazou_priority) && int.Parse(second_gazou_priority) == type)
                {
                    int second_gazou_scroll_x = default, second_gazou_scroll_y = default;
                    if (Global.state.ChipRegister.TryGetValue("second_gazou_scroll_x", out string second_gazou_scroll_x_value)) second_gazou_scroll_x = int.Parse(second_gazou_scroll_x_value);
                    if (Global.state.ChipRegister.TryGetValue("second_gazou_scroll_y", out string second_gazou_scroll_y_value)) second_gazou_scroll_y = int.Parse(second_gazou_scroll_y_value);

                    Image SecondHaikeiOrig = DrawSecondHaikeiOrig;
                    if (Global.state.EdittingStage == 1) SecondHaikeiOrig = DrawSecondHaikei2Orig;
                    else if (Global.state.EdittingStage == 2) SecondHaikeiOrig = DrawSecondHaikei3Orig;
                    else if (Global.state.EdittingStage == 3) SecondHaikeiOrig = DrawSecondHaikei4Orig;
                    if (SecondHaikeiOrig != null) g.DrawImage(SecondHaikeiOrig, new Rectangle(new Point(LogicalToDeviceUnits(second_gazou_scroll_x), LogicalToDeviceUnits(second_gazou_scroll_y)), LogicalToDeviceUnits(SecondHaikeiOrig.Size)));
                }
            }
            if (!Global.state.MapEditMode)
            {
                // セカンド背景画像固定表示
                viewSecondHaikei(1);
                // 背景画像固定表示
                if (Global.state.ChipRegister.TryGetValue("gazou_scroll", out string gazou_scroll) && int.Parse(gazou_scroll) == 11)
                {
                    int gazou_scroll_x = default, gazou_scroll_y = default;
                    if (Global.state.ChipRegister.TryGetValue("gazou_scroll_x", out string gazou_scroll_x_value)) gazou_scroll_x = int.Parse(gazou_scroll_x_value);
                    if (Global.state.ChipRegister.TryGetValue("gazou_scroll_y", out string gazou_scroll_y_value)) gazou_scroll_y = int.Parse(gazou_scroll_y_value);

                    Image HaikeiOrig = DrawHaikeiOrig;
                    if (Global.state.EdittingStage == 1) HaikeiOrig = DrawHaikei2Orig;
                    else if (Global.state.EdittingStage == 2) HaikeiOrig = DrawHaikei3Orig;
                    else if (Global.state.EdittingStage == 3) HaikeiOrig = DrawHaikei4Orig;
                    if (HaikeiOrig != null) g.DrawImage(HaikeiOrig, new Rectangle(new Point(LogicalToDeviceUnits(gazou_scroll_x), LogicalToDeviceUnits(gazou_scroll_y)), LogicalToDeviceUnits(HaikeiOrig.Size)));
                }
            }
            else if (DrawChizuOrig != null)
            {
                g.DrawImage(DrawChizuOrig, new Rectangle(new Point(LogicalToDeviceUnits(-16), LogicalToDeviceUnits(-24)), LogicalToDeviceUnits(DrawChizuOrig.Size)));
            }

            ChipsData chipsData;
            ChipData c;
            List<KeepDrawData> list = [];
            int num = 0;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
            Runtime.DefinedData.StageSizeData MapSize = Global.cpd.project.Runtime.Definitions.MapSize;
            Runtime.DefinedData.StageSizeData StageSize = CurrentStageSize;
            Runtime.DefinedData.LayerSizeData LayerSize = CurrentLayerSize;

            if (Global.state.Use3rdMapDataCurrently)
            {
                while (num < StageSize.y)
                {
                    int num2 = 0;
                    while (num2 < StageSize.x)
                    {
                        string text;
                        if (foreground)
                        {
                            text = Global.cpd.EditingMap[num].Split(',')[num2];
                        }
                        else
                        {
                            text = Global.cpd.EditingLayers[layerIndex][num].Split(',')[num2];
                        }
                        if (
                            (!Global.state.UseBuffered
                                || (
                                    (!foreground
                                        || ForePrevDrawn == null
                                        || !(ForePrevDrawn[num].Split(',')[num2] == text)
                                    ) && (foreground
                                        || BackPrevDrawn == null
                                        || !(BackPrevDrawn[num].Split(',')[num2] == text)
                                    )
                                   )
                            ) && (!Global.config.draw.SkipFirstChip
                                || (
                                    (!foreground || !text.Equals(Global.cpd.Mapchip[0].code))
                                    && (foreground || !text.Equals(Global.cpd.Layerchip[0].code))
                                    )
                            ) && (
                                (foreground && DrawItemCodeRef.ContainsKey(text))
                                || (!foreground && DrawLayerCodeRef.ContainsKey(text))
                            )
                        )
                        {
                            if (foreground)
                            {
                                chipsData = DrawItemCodeRef[text];
                            }
                            else
                            {
                                chipsData = DrawLayerCodeRef[text];
                            }
                            c = chipsData.GetCSChip();
                            if (c.size != default) // 標準サイズより大きい
                            {
                                if (Global.state.UseBuffered)
                                {
                                    g.CompositingMode = CompositingMode.SourceCopy;
                                    g.FillRectangle(Brushes.Transparent, new Rectangle(new Point(LogicalToDeviceUnits(num2 * chipsize.Width - c.center.X), LogicalToDeviceUnits(num * chipsize.Height - c.center.Y)), LogicalToDeviceUnits(c.size)));
                                    g.CompositingMode = CompositingMode.SourceOver;
                                }
                                DrawExtendSizeMap(c, g, new Point(num2, num), foreground, chipsData.character, layerIndex);
                            }
                            else
                            { // 標準サイズの画像はリストに追加後、↓で描画
                                list.Add(new KeepDrawData(c, new Point(num2, num), chipsData.character, chipsData.idColor));
                            }
                        }
                        num2++;
                    }
                    num++;
                }
            }
            else
            {
                while (Global.state.MapEditMode ? (num < MapSize.y) : (num < StageSize.y))
                {
                    int num2 = 0;
                    while (Global.state.MapEditMode ? (num2 < MapSize.x) : (num2 < StageSize.x))
                    {
                        string text;
                        if (foreground)
                        {
                            if (Global.state.MapEditMode)
                            {
                                text = Global.cpd.EditingMap[num].Substring(num2 * MapSize.bytesize, MapSize.bytesize);
                            }
                            else
                            {
                                text = Global.cpd.EditingMap[num].Substring(num2 * StageSize.bytesize, StageSize.bytesize);
                            }
                        }
                        else
                        {
                            text = Global.cpd.EditingLayer[num].Substring(num2 * LayerSize.bytesize, LayerSize.bytesize);
                        }
                        if (
                            (!Global.state.UseBuffered
                                || (
                                    (!foreground
                                        || ForePrevDrawn == null
                                        || !(ForePrevDrawn[num].Substring(Global.state.MapEditMode ? (num2 * MapSize.bytesize) : (num2 * StageSize.bytesize), Global.state.MapEditMode ? MapSize.bytesize : StageSize.bytesize) == text)
                                     ) && (foreground
                                        || BackPrevDrawn == null
                                        || !(BackPrevDrawn[num].Substring(num2 * LayerSize.bytesize, LayerSize.bytesize) == text)
                                     )
                                    )
                            ) && (!Global.config.draw.SkipFirstChip
                                || (
                                    (!foreground || !text.Equals(Global.cpd.Mapchip[0].character))
                                    && (foreground || !text.Equals(Global.cpd.Layerchip[0].character))
                                    )
                            ) && (
                                (foreground && DrawItemRef.ContainsKey(text))
                                || (!foreground && DrawLayerRef.ContainsKey(text))
                            )
                        )
                        {
                            if (Global.state.MapEditMode) chipsData = DrawWorldRef[text];
                            else if (foreground)
                            {
                                chipsData = DrawItemRef[text];
                            }
                            else
                            {
                                chipsData = DrawLayerRef[text];
                            }
                            c = chipsData.GetCSChip();
                            if (c.size != default) // 標準サイズより大きい
                            {
                                if (Global.state.UseBuffered)
                                {
                                    g.CompositingMode = CompositingMode.SourceCopy;
                                    g.FillRectangle(Brushes.Transparent, new Rectangle(new Point(LogicalToDeviceUnits(num2 * chipsize.Width - c.center.X), LogicalToDeviceUnits(num * chipsize.Height - c.center.Y)), LogicalToDeviceUnits(c.size)));
                                    g.CompositingMode = CompositingMode.SourceOver;
                                }
                                DrawExtendSizeMap(c, g, new Point(num2, num), foreground, chipsData.character, layerIndex);
                            }
                            else
                            { // 標準サイズの画像はリストに追加後、↓で描画
                                list.Add(new KeepDrawData(c, new Point(num2, num), chipsData.character));
                            }
                        }
                        num2++;
                    }
                    num++;
                }
            }
            foreach (KeepDrawData keepDrawData in list)
            {
                ChipData cschip = keepDrawData.cd;
                if (Global.state.UseBuffered)
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.FillRectangle(Brushes.Transparent, new Rectangle(new Point(LogicalToDeviceUnits(keepDrawData.pos.X * chipsize.Width), LogicalToDeviceUnits(keepDrawData.pos.Y * chipsize.Height)), LogicalToDeviceUnits(chipsize)));
                    g.CompositingMode = CompositingMode.SourceOver;
                }
                DrawNormalSizeMap(cschip, g, keepDrawData.pos, foreground, keepDrawData.chara, keepDrawData.pos.X, layerIndex);
                if (keepDrawData.idColor != null)
                {
                    ChipRenderer.DrawIdColorMark(g, new Point(LogicalToDeviceUnits(keepDrawData.pos.X * chipsize.Width), LogicalToDeviceUnits(keepDrawData.pos.Y * chipsize.Height)), keepDrawData.idColor, this);
                }
            }
            if (!Global.state.MapEditMode)
            {
                // オリジナルボスを座標で設置する場合に描画
                if (Global.state.ChipRegister.TryGetValue("oriboss_v", out string oriboss_v) && int.Parse(oriboss_v) == 2 && foreground)
                {
                    int oriboss_x = default, oriboss_y = default;
                    if (Global.state.ChipRegister.TryGetValue("oriboss_x", out string oriboss_x_value)) oriboss_x = int.Parse(oriboss_x_value);
                    if (Global.state.ChipRegister.TryGetValue("oriboss_y", out string oriboss_y_value)) oriboss_y = int.Parse(oriboss_y_value);
                    if (DrawOribossOrig != null) g.DrawImage(DrawOribossOrig, new Rectangle(new Point(LogicalToDeviceUnits(oriboss_x * chipsize.Width), LogicalToDeviceUnits(oriboss_y * chipsize.Height)), LogicalToDeviceUnits(DrawOribossOrig.Size)));
                    if (Global.state.ChipRegister.TryGetValue("oriboss_ugoki", out string oriboss_ugoki) && Global.config.draw.ExtendDraw)
                    {
                        Point p = ChipRenderer.GetOribossExtensionPoint(int.Parse(oriboss_ugoki));
                        g.DrawImage(DrawExOrig, LogicalToDeviceUnits(oriboss_x * chipsize.Width), LogicalToDeviceUnits(oriboss_y * chipsize.Height), new Rectangle(p, chipsize), GraphicsUnit.Pixel);
                    }
                }
                // セカンド前景画像固定表示
                viewSecondHaikei(2);
            }
            SavePrevDrawnLayer(foreground);
            list.Clear();
        }

        public void TransparentStageClip(Bitmap b, Rectangle stagearea)
        {
            Size chipSize = Global.cpd.runtime.Definitions.ChipSize;
            TransparentClip(b, new Rectangle(stagearea.X * chipSize.Width, stagearea.Y * chipSize.Height, stagearea.Width * chipSize.Width, stagearea.Height * chipSize.Height));
        }

        public unsafe void TransparentClip(Bitmap b, Rectangle area)
        {
            try
            {
                BitmapData bitmapData = b.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                byte* ptr = (byte*)(void*)bitmapData.Scan0;
                for (int i = 0; i < area.Height; i++)
                {
                    for (int j = 0; j < area.Width; j++)
                    {
                        ptr[j * 4 + 3 + i * bitmapData.Stride] = 0;
                    }
                }
                b.UnlockBits(bitmapData);
            }
            catch
            {
                StageSourceToDrawBuffer();
            }
        }

        public unsafe void HalfTransparentBitmap(Bitmap b)
        {
            byte b2 = 125;
            byte b3 = 0;
            BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)(void*)bitmapData.Scan0;
            for (int i = 0; i < b.Height; i++)
            {
                int num = i * bitmapData.Stride;
                for (int j = 0; j < b.Width; j++)
                {
                    if (ptr[j * 4 + 3 + num] != b3)
                    {
                        ptr[j * 4 + 3 + num] = b2;
                    }
                }
            }
            b.UnlockBits(bitmapData);
        }

        public static void HalfTransparentBitmap2(ref Bitmap b)
        {
            Bitmap bitmap = new(b.Width, b.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                ColorMatrix colorMatrix = new()
                {
                    Matrix00 = 1f,
                    Matrix11 = 1f,
                    Matrix22 = 1f,
                    Matrix33 = 0.5f,
                    Matrix44 = 1f
                };
                using ImageAttributes imageAttributes = new();
                imageAttributes.SetColorMatrix(colorMatrix);
                graphics.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            b.Dispose();
            b = bitmap;
        }

        public void CreateDrawItemReference()
        {
            DrawItemRef.Clear();
            foreach (ChipsData value in Global.cpd.Mapchip)
            {
                DrawItemRef.Add(value.character, value);
            }
            DrawWorldRef.Clear();
            foreach (ChipsData value2 in Global.cpd.Worldchip)
            {
                DrawWorldRef.Add(value2.character, value2);
            }
            if (CurrentProjectData.UseLayer)
            {
                DrawLayerRef.Clear();
                foreach (ChipsData value3 in Global.cpd.Layerchip)
                {
                    DrawLayerRef.Add(value3.character, value3);
                }
            }
            if (Global.cpd.project.Use3rdMapData)
            {
                CreateDrawItemCodeReference();
            }
        }
        public void CreateDrawItemCodeReference()
        {
            DrawItemCodeRef.Clear();
            foreach (ChipsData value in Global.cpd.Mapchip)
            {
                DrawItemCodeRef.Add(value.code, value);
            }
            foreach (ChipsData value in Global.cpd.VarietyChip)
            {
                DrawItemCodeRef.Add(value.code, value);
            }
            if (Global.cpd.CustomPartsChip != null)
            {
                foreach (ChipsData value in Global.cpd.CustomPartsChip)
                {
                    DrawItemCodeRef.Add(value.code, value);
                }
            }
            if (CurrentProjectData.UseLayer)
            {
                DrawLayerCodeRef.Clear();
                foreach (ChipsData value3 in Global.cpd.Layerchip)
                {
                    DrawLayerCodeRef.Add(value3.code, value3);
                }
            }
        }

        // 画像準備
        public void PrepareImages()
        {
            DisposeImageResources();
            string filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.PatternImage);
            if (!File.Exists(filename))
            {
                filename = Path.Combine(Global.cpd.where, DEFAULT_PATTERN_IMAGE);
                Global.cpd.project.Config.PatternImage = DEFAULT_PATTERN_IMAGE;
            }

            // パターン画像の読み込みと処理
            DrawChipOrig = LoadImageFromFile(filename);
            // DrawMask = CreateMaskFromImage(DrawChipOrig);
            LayerObject CurrentStageData = Global.state.EdittingStage switch
            {
                0 => Global.cpd.project.StageData,
                1 => Global.cpd.project.StageData2,
                2 => Global.cpd.project.StageData3,
                3 => Global.cpd.project.StageData4,
                _ => Global.cpd.project.StageData
            };
            var patternValue = CurrentStageData.Source;
            if (!string.IsNullOrEmpty(patternValue))
            {
                filename = Path.Combine(Global.cpd.where, patternValue);
                if (File.Exists(filename))
                {
                    DrawChipOrig = LoadImageFromFile(filename);
                }
            }

            if (Global.cpd.runtime.Definitions.LayerSize.bytesize != 0)
            {
                // レイヤー画像の読み込みと処理
                filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.LayerImage);
                if (!File.Exists(filename))
                {
                    filename = Path.Combine(Global.cpd.where, DEFAULT_LAYER_IMAGE);
                    Global.cpd.project.Config.LayerImage = DEFAULT_LAYER_IMAGE;
                }
                DrawLayerOrig = new List<Image>(Global.cpd.LayerCount);
                DrawLayerOrigDefault = LoadImageFromFile(filename);
                // DrawLayerMask = CreateMaskFromImage(DrawLayerOrig[0]);
                List<LayerObject> CurrentLayerData = Global.state.EdittingStage switch
                {
                    0 => Global.cpd.project.LayerData,
                    1 => Global.cpd.project.LayerData2,
                    2 => Global.cpd.project.LayerData3,
                    3 => Global.cpd.project.LayerData4,
                    _ => Global.cpd.project.LayerData
                };
                for (int i = 0; i < Global.cpd.LayerCount; i++)
                {
                    DrawLayerOrig.Add(DrawLayerOrigDefault);  // レイヤー画像が設定されていない場合はデフォルトのレイヤー画像を使用
                    var layerValue = CurrentLayerData[i].Source;
                    if (!string.IsNullOrEmpty(layerValue))
                    {
                        filename = Path.Combine(Global.cpd.where, layerValue);
                        if (File.Exists(filename))
                        {
                            DrawLayerOrig[i] = LoadImageFromFile(filename);
                        }
                    }
                }
            }

            // オリジナルボス画像の読み込み（設定されている場合）
            if (Global.cpd.project.Config.OribossImage != null)
            {
                filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.OribossImage);
                try
                {
                    DrawOribossOrig = LoadImageFromFile(filename);
                    // DrawOribossMask = CreateMaskFromImage(DrawOribossOrig);
                }
                catch
                {
                    DrawOribossOrig = null;
                }
            }

            // 拡張チップ画像の読み込みと処理
            filename = Path.Combine(Global.cpd.where, Global.cpd.runtime.Definitions.ChipExtender);
            DrawExOrig = LoadImageFromFile(filename);
            DrawExMask = CreateMaskFromImage(DrawExOrig);
            using (Bitmap drawExMask = DrawExMask)
            {
                DrawExMask = DrawEx.MakeMask(drawExMask);
            }
            DrawHaikeiOrig = setImage(Global.cpd.project.Config.HaikeiImage);// ステージ1背景画像
            DrawHaikei2Orig = setImage(Global.cpd.project.Config.HaikeiImage2);// ステージ2背景画像
            DrawHaikei3Orig = setImage(Global.cpd.project.Config.HaikeiImage3);// ステージ3背景画像
            DrawHaikei4Orig = setImage(Global.cpd.project.Config.HaikeiImage4);// ステージ4背景画像
            DrawSecondHaikeiOrig = setImage(Global.cpd.project.Config.SecondHaikeiImage);// ステージ1背景画像
            DrawSecondHaikei2Orig = setImage(Global.cpd.project.Config.SecondHaikeiImage2);// ステージ1背景画像
            DrawSecondHaikei3Orig = setImage(Global.cpd.project.Config.SecondHaikeiImage3);// ステージ1背景画像
            DrawSecondHaikei4Orig = setImage(Global.cpd.project.Config.SecondHaikeiImage4);// ステージ1背景画像
            DrawChizuOrig = setImage(Global.cpd.project.Config.ChizuImage);// 地図画面の背景
        }

        private static Image setImage(string source)
        {
            if (source != null)
            {
                var filename = Path.Combine(Global.cpd.where, source);
                try
                {
                    var fs = File.OpenRead(filename);

                    return Image.FromStream(fs, false, false);
                }
                catch
                {
                    return null;
                }
            }
            else { return null; }
        }

        // ヘルパーメソッドの定義（クラス内に追加）
        private static Image LoadImageFromFile(string filePath)
        {
            return Image.FromStream(File.OpenRead(filePath), false, false);
        }

        private static Bitmap CreateMaskFromImage(Image sourceImage)
        {
            var mask = new Bitmap(sourceImage.Width, sourceImage.Height);
            ColorMap[] remapTable =
            [
                new()
            ];
            
            using (ImageAttributes imageAttributes = new())
            {
                imageAttributes.SetRemapTable(remapTable);
                using Graphics graphics = Graphics.FromImage(mask);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, mask.Width, mask.Height));
                graphics.DrawImage(sourceImage, new Rectangle(0, 0, mask.Width, mask.Height), 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            
            return mask;
        }

        private void DisposeImageResources()
        {
            DisposeAndSetNull(ref DrawChipOrig);
            // DisposeAndSetNull(ref DrawMask);
            DisposeAndSetNull(ref DrawLayerOrig);
            // DisposeAndSetNull(ref DrawLayerMask);
            DisposeAndSetNull(ref DrawOribossOrig);
            // DisposeAndSetNull(ref DrawOribossMask);
            DisposeAndSetNull(ref DrawExOrig);
            DisposeAndSetNull(ref DrawExMask);
            DisposeAndSetNull(ref DrawHaikeiOrig);
            DisposeAndSetNull(ref DrawHaikei2Orig);
            DisposeAndSetNull(ref DrawHaikei3Orig);
            DisposeAndSetNull(ref DrawHaikei4Orig);
            DisposeAndSetNull(ref DrawSecondHaikeiOrig);
            DisposeAndSetNull(ref DrawSecondHaikei2Orig);
            DisposeAndSetNull(ref DrawSecondHaikei3Orig);
            DisposeAndSetNull(ref DrawSecondHaikei4Orig);
            DisposeAndSetNull(ref DrawChizuOrig);
        }

        private static void DisposeAndSetNull<T>(ref T disposable) where T : class, IDisposable
        {
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }
        }

        private static void DisposeAndSetNull<T>(ref List<T> disposableList) where T : class, IDisposable
        {
            if (disposableList != null)
            {
                foreach (var item in disposableList)
                {
                    item?.Dispose();
                }
                disposableList.Clear();
                disposableList = null;
            }
        }

        protected unsafe override void OnPaint(PaintEventArgs e)
        {
            if (ForeLayerBmp == null)
            {
                return;
            }
            int num = (Width < (int)(ForeLayerBmp.Width * Global.config.draw.ZoomIndex)) ? Width : ((int)(ForeLayerBmp.Width * Global.config.draw.ZoomIndex));
            int num2 = (Height < (int)(ForeLayerBmp.Height * Global.config.draw.ZoomIndex)) ? Height : ((int)(ForeLayerBmp.Height * Global.config.draw.ZoomIndex));
            if (Global.config.draw.UseBufferingDraw && BufferingDraw && (bufpos != BufferCurrent || zi != Global.config.draw.ZoomIndex || DULayer != Global.state.DrawUnactiveLayer || TULayer != Global.state.TransparentUnactiveLayer || FLayer != Global.state.EditingForeground || EditMap != Global.state.EdittingStage))
            {
                bufpos = BufferCurrent;
                DULayer = Global.state.DrawUnactiveLayer;
                TULayer = Global.state.TransparentUnactiveLayer;
                FLayer = Global.state.EditingForeground;
                if (zi != Global.config.draw.ZoomIndex || EditMap != Global.state.EdittingStage)
                {
                    zi = Global.config.draw.ZoomIndex;
                    ForegroundBuffer.Dispose();
                    ForegroundBuffer = new Bitmap((int)(ForeLayerBmp.Width * zi), (int)(ForeLayerBmp.Height * zi), PixelFormat.Format24bppRgb);
                }
                else if (Global.state.StageSizeChanged)
                {
                    ForegroundBuffer.Dispose();
                    ForegroundBuffer = new Bitmap((int)(ForeLayerBmp.Width * zi), (int)(ForeLayerBmp.Height * zi), PixelFormat.Format24bppRgb);
                    Global.state.StageSizeChanged = false;
                }
                EditMap = Global.state.EdittingStage;
                using Graphics graphics = Graphics.FromImage(ForegroundBuffer);
                using (Brush brush = new SolidBrush(Global.state.Background))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, ForegroundBuffer.Width, ForegroundBuffer.Height));
                }
                SetGraphicsInterpolationMode(graphics);
                DrawLayersToBuffer(graphics);
                if (Global.config.draw.UseBufferingMemoryDraw)
                {
                    ForegroundBuffer.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
            }
            if (Global.config.draw.UseBufferingDraw && BufferingDraw)
            {
                if (Global.config.draw.UseBufferingMemoryDraw)
                {
                    IntPtr hdc = e.Graphics.GetHdc();
                    BitmapData bitmapData = new();
                    ForegroundBuffer.LockBits(new Rectangle(0, 0, ForegroundBuffer.Width, ForegroundBuffer.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb, bitmapData);
                    Native.GDI32.BITMAPINFOHEADER bitmapinfoheader = default;
                    bitmapinfoheader.biSize = (uint)sizeof(Native.GDI32.BITMAPINFOHEADER);
                    bitmapinfoheader.biWidth = bitmapData.Width;
                    bitmapinfoheader.biHeight = bitmapData.Height;
                    bitmapinfoheader.biPlanes = 1;
                    bitmapinfoheader.biBitCount = 24;
                    int ysrc = (Global.state.MapMoveMax.Height > 0) ? (Global.state.MapMoveMax.Height - Global.state.MapPoint.Y) : 0;
                    Native.GDI32.StretchDIBits(hdc, 0, 0, num, num2, Global.state.MapPoint.X, ysrc, num, num2, bitmapData.Scan0, (IntPtr)(void*)&bitmapinfoheader, 0U, 13369376U);
                    ForegroundBuffer.UnlockBits(bitmapData);
                    e.Graphics.ReleaseHdc(hdc);
                }
                else
                {
                    e.Graphics.DrawImage(ForegroundBuffer, new Rectangle(0, 0, num, num2), new Rectangle(Global.state.MapPoint, new Size(num, num2)), GraphicsUnit.Pixel);
                }
            }
            else
            {
                using (Brush brush2 = new SolidBrush(Global.state.Background))
                {
                    e.Graphics.FillRectangle(brush2, new Rectangle(0, 0, num, num2));
                }
                SetGraphicsInterpolationMode(e.Graphics);
                double num3 = 1.0 / Global.config.draw.ZoomIndex;
                DrawLayersToGraphics(e.Graphics, num, num2, num3);
            }
            if (DrawMode != DirectDrawMode.None)
            {
                Rectangle drawRectangle = DrawRectangle;
                drawRectangle.X *= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
                drawRectangle.Y *= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);
                drawRectangle.Width *= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
                drawRectangle.Height *= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);
                drawRectangle.X -= Global.state.MapPoint.X;
                drawRectangle.Y -= Global.state.MapPoint.Y;
                using Brush brush3 = new SolidBrush(Color.FromArgb(Global.config.draw.AlphaBlending ? 160 : 255, DrawEx.GetForegroundColor(Global.state.Background)));
                if (DrawMode == DirectDrawMode.Rectangle)
                {
                    e.Graphics.FillRectangle(brush3, drawRectangle);
                }
                else
                {
                    using Pen pen = new(brush3, (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex) / 4);
                    drawRectangle.X += (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex) / 2;
                    drawRectangle.Y += (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex) / 2;
                    drawRectangle.Width -= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
                    drawRectangle.Height -= (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);
                    if (DrawMode == DirectDrawMode.RevLine)
                    {
                        e.Graphics.DrawLine(pen, new Point(drawRectangle.X, drawRectangle.Bottom), new Point(drawRectangle.Right, drawRectangle.Top));
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, drawRectangle.Location, new Point(drawRectangle.Right, drawRectangle.Bottom));
                    }
                }
            }
            if (Global.config.draw.DrawGrid)
            { // グリッドを表示

                // 横方向のグリッド
                int num4 = (int)(Global.state.MapPointTranslated.X % LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
                // 縦方向のグリッド
                int num5 = (int)(Global.state.MapPointTranslated.Y % LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);

                DrawEx.DrawGridEx(e.Graphics, new Rectangle(0, (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex) - num5, num, num2), new Size(Global.definition.GridInterval, (int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex)), Global.state.Background);

                DrawEx.DrawGridEx(e.Graphics, new Rectangle((int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex) - num4, 0, num, num2), new Size((int)(LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex), Global.definition.GridInterval), Global.state.Background);
            }
        }
        
        private static void SetGraphicsInterpolationMode(Graphics graphics)
        {
            if (Global.config.draw.StageInterpolation)
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            else
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
        }
        
        private void DrawLayersToBuffer(Graphics graphics)
        {
            DrawLayers(graphics, 
                destRect => new Rectangle(new Point(0, 0), ForegroundBuffer.Size),
                (bitmap, destRect) => new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }
        
        private void DrawLayersToGraphics(Graphics graphics, int num, int num2, double scale)
        {
            DrawLayers(graphics,
                destRect => new Rectangle(0, 0, num, num2),
                (bitmap, destRect) => new Rectangle(Global.state.MapPointTranslated, new Size((int)(num * scale), (int)(num2 * scale))));
        }
        
        private void DrawLayers(Graphics graphics, 
            Func<Bitmap, Rectangle> getDestRect, 
            Func<Bitmap, Rectangle, Rectangle> getSrcRect)
        {
            if (CurrentProjectData.UseLayer && (!Global.state.EditingForeground || Global.state.DrawUnactiveLayer))
            {
                if (Global.state.EdittingLayerIndex == -1 || Global.state.DrawUnactiveLayer)
                {
                    if (Global.cpd.LayerCount <= Global.cpd.MainOrder)
                    {
                        DrawImageLayer(graphics, ForeLayerBmp, getDestRect, getSrcRect);
                    }
                    for (int i = Global.cpd.LayerCount - 1; i >= 0; i--)
                    {
                        DrawImageLayer(graphics, BackLayerBmp[i], getDestRect, getSrcRect);
                        if (i == Global.cpd.MainOrder)
                        {
                            DrawImageLayer(graphics, ForeLayerBmp, getDestRect, getSrcRect);
                        }
                    }
                }
                else
                {
                    DrawImageLayer(graphics, BackLayerBmp[Global.state.EdittingLayerIndex], getDestRect, getSrcRect);
                }
            }
            if (!CurrentProjectData.UseLayer || Global.state.EditingForeground)
            {
                DrawImageLayer(graphics, ForeLayerBmp, getDestRect, getSrcRect);
            }
        }
        
        private static void DrawImageLayer(Graphics graphics, Bitmap bitmap, 
            Func<Bitmap, Rectangle> getDestRect, 
            Func<Bitmap, Rectangle, Rectangle> getSrcRect)
        {
            var destRect = getDestRect(bitmap);
            var srcRect = getSrcRect(bitmap, destRect);
            graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        void IDisposable.Dispose()
        {
            DisposeImageResources();
            DisposeAndSetNull(ref ForeLayerBmp);
            for (int i = 0; i < BackLayerBmp.Count; i++)
            {
                if (BackLayerBmp[i] != null)
                {
                    BackLayerBmp[i].Dispose();
                    BackLayerBmp[i] = null;
                }
            }
            GC.SuppressFinalize(this);
        }

        private void GUIDesigner_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                if (e.Button == MouseButtons.Right)
                { // 右クリック時
                    if (CopyPaste == CopyPasteTool.Paste)
                    {
                        Global.MainWnd.CopyPasteInit();
                        Refresh();
                        return;
                    }
                    if (MousePressed)
                    {
                        MousePressed = false;
                        switch (CopyPaste)
                        {
                            case CopyPasteTool.Copy:
                            case CopyPasteTool.Cut:
                                Global.MainWnd.CopyPasteInit();
                                Refresh();
                                return;
                            default:
                                switch (CurrentTool)
                                {
                                    case EditTool.Line:
                                        EnsureScroll(e.X, e.Y);
                                        DrawMode = DirectDrawMode.None;
                                        Refresh();
                                        return;
                                    case EditTool.Rect:
                                        EnsureScroll(e.X, e.Y);
                                        DrawMode = DirectDrawMode.None;
                                        Refresh();
                                        return;
                                    default:
                                        return;
                                }
                        }
                    }
                    else
                    {
                        if (CurrentTool == EditTool.Cursor && Global.config.draw.RightClickMenu)
                        {
                            MouseStartPoint = default;
                            MouseStartPoint.X = (int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex));
                            MouseStartPoint.Y = (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex));
                            CursorContextMenu.Show(this, new Point(e.X, e.Y));
                            return;
                        }

                        // チップを拾う
                        string stageChar = StageText.GetStageChar(new Point
                        {
                            X = (int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)),
                            Y = (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex))
                        });
                        if (Global.state.MapEditMode)
                        {
                            if (DrawItemRef.ContainsKey(stageChar))
                            {
                                Global.state.CurrentChip = DrawWorldRef[stageChar];
                                return;
                            }
                        }
                        else if (Global.state.EditingForeground)
                        {
                            if (Global.cpd.project.Use3rdMapData)
                            {
                                if (DrawItemCodeRef.TryGetValue(stageChar, out ChipsData value))
                                {
                                    Global.state.CurrentChip = value;
                                    return;
                                }
                            }
                            else if (DrawItemRef.TryGetValue(stageChar, out ChipsData value))
                            {
                                Global.state.CurrentChip = value;
                                return;
                            }
                        }
                        else
                        {
                            if (Global.cpd.project.Use3rdMapData)
                            {
                                if (DrawLayerCodeRef.TryGetValue(stageChar, out ChipsData value))
                                {
                                    Global.state.CurrentChip = value;
                                    return;
                                }
                            }
                            else if (DrawLayerRef.TryGetValue(stageChar, out ChipsData value))
                            {
                                Global.state.CurrentChip = value;
                                return;
                            }
                        }
                    }
                }
                else if (e.Button == MouseButtons.Middle && Global.config.testRun.QuickTestrun)
                { // 中央クリック時
                    Point p = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
                    if (StageText.IsOverflow(p))
                    {
                        return;
                    }
                    List<string> list = [];
                    foreach (string text in Global.cpd.EditingMap)
                    {
                        if (Global.state.Use3rdMapDataCurrently)
                        {
                            string[] t = text.Split(',');
                            for (int i = 0; i < t.Length; i++)
                            {
                                if (t[i] == Global.cpd.Mapchip[1].code) t[i] = Global.cpd.Mapchip[0].code;
                            }
                            list.Add(string.Join(",", t));
                        }
                        else
                        {
                            list.Add(text.Replace(Global.cpd.Mapchip[1].character, Global.cpd.Mapchip[0].character));
                        }
                    }
                    if (Global.state.Use3rdMapDataCurrently)
                    {
                        string[] l = list[p.Y].Split(',');
                        l[p.X] = Global.cpd.Mapchip[1].code;
                        list[p.Y] = string.Join(",", l);
                    }
                    else
                    {
                        int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : Global.cpd.runtime.Definitions.StageSize.bytesize;
                        list[p.Y] = list[p.Y].Remove(p.X * num, num);
                        list[p.Y] = list[p.Y].Insert(p.X * num, Global.cpd.Mapchip[1].character);
                    }
                    Global.state.QuickTestrunSource = [.. list];
                    Global.MainWnd.Testrun();
                }
                return;
            }
            MousePressed = true;
            switch (CopyPaste)
            {
                case CopyPasteTool.Copy:
                case CopyPasteTool.Cut:
                    MouseStartPoint = new Point((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
                    MouseLastPoint = MouseStartPoint;
                    return;
                case CopyPasteTool.Paste:
                    {
                        EnsureScroll(e.X, e.Y);
                        Global.MainWnd.UpdateStatus("描画しています...");
                        Rectangle rectangle = new(MouseLastPoint, GetBufferSize());
                        string[] array = ClipedString.Split(
                        [
                    Environment.NewLine
                        ], StringSplitOptions.None);
                        ChipsData cd = default;
                        Point point = new(0, 0);
                        for (int j = rectangle.Top; j < rectangle.Bottom; j++)
                        {
                            point.X = 0;
                            if (Global.state.Use3rdMapDataCurrently)
                            {
                                string[] array2 = PutItemTextCodeStart(j);
                                if (array2 != null)
                                {
                                    string[] arr = array[point.Y].Split(',');
                                    for (int k = rectangle.Left; k < rectangle.Right; k++)
                                    {
                                        cd.code = arr[point.X];
                                        PutItemText(ref array2, new Point(k, j), cd);
                                        point.X++;
                                    }
                                    PutItemTextEnd(array2, j);
                                    point.Y++;
                                }
                            }
                            else
                            {
                                char[] array2 = PutItemTextStart(j);
                                if (array2 != null)
                                {
                                    for (int k = rectangle.Left; k < rectangle.Right; k++)
                                    {
                                        cd.character = array[point.Y].Substring(point.X * Global.state.GetCByte, Global.state.GetCByte);
                                        PutItemText(ref array2, new Point(k, j), cd);
                                        point.X++;
                                    }
                                    PutItemTextEnd(array2, j);
                                    point.Y++;
                                }
                            }
                        }
                        Global.MainWnd.CopyPasteInit();
                        StageSourceToDrawBuffer();
                        AddBuffer();
                        MousePressed = false;
                        DrawMode = DirectDrawMode.None;
                        Global.MainWnd.UpdateStatus("完了");
                        Refresh();
                        return;
                    }
                default:
                    switch (CurrentTool)
                    {
                        case EditTool.Cursor:
                            MouseStartPoint = new Point(e.X, e.Y);
                            return;
                        case EditTool.Pen:
                            MouseLastPoint = new Point(-1, -1);
                            GUIDesigner_MouseMove(this, e);
                            return;
                        case EditTool.Line:
                        case EditTool.Rect:
                            MouseStartPoint = new Point(
                                (int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)),
                                (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex))
                            );
                            MouseLastPoint = MouseStartPoint;
                            return;
                        case EditTool.Fill:
                            Global.MainWnd.UpdateStatus("塗り潰しています...");
                            FillStart(Global.state.CurrentChip,
                                new Point(
                                    (int)(
                                        (e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)
                                    ),
                                    (int)(
                                        (e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)
                                    )
                                )
                            );
                            StageSourceToDrawBuffer();
                            AddBuffer();
                            Refresh();
                            Global.MainWnd.UpdateStatus("完了");
                            return;
                        default:
                            return;
                    }
            }
        }

        // 塗りつぶすチップデータ、塗りつぶし開始座標
        private void FillStart(ChipsData repl, Point pt)
        {
            fillToolHelper.FillStart(repl, pt);
        }

        private void GUIDesigner_MouseMove(object sender, MouseEventArgs e)
        {
            if (CopyPaste != CopyPasteTool.Paste || MousePressed)
            {
                if (MousePressed)
                {
                    switch (CopyPaste)
                    {
                        case CopyPasteTool.Copy:
                        case CopyPasteTool.Cut:
                            {
                                EnsureScroll(e.X, e.Y);
                                Point point = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
                                if (point == MouseLastPoint)
                                {
                                    return;
                                }
                                Rectangle drawRectangle = default;
                                MouseLastPoint = point;
                                if (MouseStartPoint.X > MouseLastPoint.X)
                                {
                                    drawRectangle.X = MouseLastPoint.X;
                                    drawRectangle.Width = MouseStartPoint.X - MouseLastPoint.X;
                                }
                                else
                                {
                                    drawRectangle.X = MouseStartPoint.X;
                                    drawRectangle.Width = MouseLastPoint.X - MouseStartPoint.X;
                                }
                                if (MouseStartPoint.Y > MouseLastPoint.Y)
                                {
                                    drawRectangle.Y = MouseLastPoint.Y;
                                    drawRectangle.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                }
                                else
                                {
                                    drawRectangle.Y = MouseStartPoint.Y;
                                    drawRectangle.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                }
                                drawRectangle.Width++;
                                drawRectangle.Height++;
                                DrawRectangle = drawRectangle;
                                DrawMode = DirectDrawMode.Rectangle;
                                Refresh();
                                return;
                            }
                        default:
                            switch (CurrentTool)
                            {
                                case EditTool.Cursor:
                                    {
                                        Size size = new(MouseStartPoint.X - e.X, MouseStartPoint.Y - e.Y);
                                        MouseStartPoint = new Point(e.X, e.Y);
                                        State state = Global.state;
                                        state.MapPoint.X += size.Width;
                                        State state2 = Global.state;
                                        state2.MapPoint.Y += size.Height;
                                        Global.state.AdjustMapPoint();
                                        Refresh();
                                        Global.MainWnd.CommitScrollbar();
                                        return;
                                    }
                                case EditTool.Pen:
                                    {
                                        Point point2 = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex)));
                                        if (point2 == MouseLastPoint)
                                        {
                                            return;
                                        }
                                        PutItem(new Point
                                        {
                                            X = (int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)),
                                            Y = (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex))
                                        });
                                        MouseLastPoint = point2;
                                        return;
                                    }
                                case EditTool.Line:
                                    {
                                        EnsureScroll(e.X, e.Y);
                                        Point point3 = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
                                        if (point3 == MouseLastPoint)
                                        {
                                            return;
                                        }
                                        Rectangle drawRectangle2 = default;
                                        MouseLastPoint = point3;
                                        bool flag = false;
                                        if (MouseStartPoint.X > MouseLastPoint.X)
                                        {
                                            drawRectangle2.X = MouseLastPoint.X;
                                            drawRectangle2.Width = MouseStartPoint.X - MouseLastPoint.X;
                                            flag = true;
                                        }
                                        else
                                        {
                                            drawRectangle2.X = MouseStartPoint.X;
                                            drawRectangle2.Width = MouseLastPoint.X - MouseStartPoint.X;
                                        }
                                        if (MouseStartPoint.Y > MouseLastPoint.Y)
                                        {
                                            drawRectangle2.Y = MouseLastPoint.Y;
                                            drawRectangle2.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                            flag = !flag;
                                        }
                                        else
                                        {
                                            drawRectangle2.Y = MouseStartPoint.Y;
                                            drawRectangle2.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                        }
                                        drawRectangle2.Width++;
                                        drawRectangle2.Height++;
                                        DrawRectangle = drawRectangle2;
                                        if (flag)
                                        {
                                            DrawMode = DirectDrawMode.RevLine;
                                        }
                                        else
                                        {
                                            DrawMode = DirectDrawMode.Line;
                                        }
                                        Refresh();
                                        return;
                                    }
                                case EditTool.Rect:
                                    {
                                        EnsureScroll(e.X, e.Y);
                                        Point point4 = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
                                        if (point4 == MouseLastPoint)
                                        {
                                            return;
                                        }
                                        Rectangle drawRectangle3 = default;
                                        MouseLastPoint = point4;
                                        if (MouseStartPoint.X > MouseLastPoint.X)
                                        {
                                            drawRectangle3.X = MouseLastPoint.X;
                                            drawRectangle3.Width = MouseStartPoint.X - MouseLastPoint.X;
                                        }
                                        else
                                        {
                                            drawRectangle3.X = MouseStartPoint.X;
                                            drawRectangle3.Width = MouseLastPoint.X - MouseStartPoint.X;
                                        }
                                        if (MouseStartPoint.Y > MouseLastPoint.Y)
                                        {
                                            drawRectangle3.Y = MouseLastPoint.Y;
                                            drawRectangle3.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                        }
                                        else
                                        {
                                            drawRectangle3.Y = MouseStartPoint.Y;
                                            drawRectangle3.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                        }
                                        drawRectangle3.Width++;
                                        drawRectangle3.Height++;
                                        DrawRectangle = drawRectangle3;
                                        DrawMode = DirectDrawMode.Rectangle;
                                        Refresh();
                                        break;
                                    }
                                default:
                                    return;
                            }
                            break;
                    }
                }
                return;
            }
            EnsureScroll(e.X, e.Y);
            Point point5 = new((int)((e.X + Global.state.MapPoint.X) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)), (int)((e.Y + Global.state.MapPoint.Y) / (LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex)));
            if (point5 == MouseLastPoint)
            {
                return;
            }
            GetBufferSize();
            int width = Global.cpd.runtime.Definitions.ChipSize.Width;
            double zoomIndex = Global.config.draw.ZoomIndex;
            double num = 1.0 / Global.config.draw.ZoomIndex;
            MouseLastPoint = point5;
            Rectangle drawRectangle4 = new(MouseLastPoint, GetBufferSize());
            DrawRectangle = drawRectangle4;
            DrawMode = DirectDrawMode.Rectangle;
            Refresh();
        }

        public bool CheckBuffer()
        {
            if (ClipedString == null || ClipedString.Length == 0)
            {
                return false;
            }
            int num = -1;
            string[] cp = ClipedString.Split(
                [
                Environment.NewLine
                ], StringSplitOptions.None);
            if (Global.state.Use3rdMapDataCurrently)
            {
                foreach (string text in cp)
                {
                    int l = text.Split(',').Length;
                    if (num != -1 && l != num)
                    {
                        return false;
                    }
                    num = l;
                }
                return num != 0 && num <= Global.state.GetCByteWidth;
            }
            else
            {
                foreach (string text in cp)
                {
                    if (num != -1 && text.Length != num)
                    {
                        return false;
                    }
                    num = text.Length;
                }
                return num != 0 && num <= Global.state.GetCByteWidth;
            }
        }

        private Size GetBufferSize()
        {
            string[] array = ClipedString.Split(
            [
                Environment.NewLine
            ], StringSplitOptions.None);
            if (Global.state.Use3rdMapDataCurrently)
            {
                return new Size(array[0].Split(',').Length, array.Length);
            }
            else
            {
                return new Size(array[0].Length / Global.state.GetCByte, array.Length);
            }
        }

        public void EnsureScroll(int x, int y)
        {
            Size chipSize = Global.cpd.runtime.Definitions.ChipSize;
            Point point = default;
            if (x < Width / 2)
            {
                if (x < chipSize.Width)
                {
                    point.X--;
                }
                if (x < chipSize.Width / 2)
                {
                    point.X -= 2;
                }
            }
            else
            {
                if (Width - x < chipSize.Width)
                {
                    point.X++;
                }
                if (Width - x < chipSize.Width / 2)
                {
                    point.X += 2;
                }
            }
            if (y < Height / 2)
            {
                if (y < chipSize.Height)
                {
                    point.Y--;
                }
                if (y < chipSize.Height / 2)
                {
                    point.Y -= 2;
                }
            }
            else
            {
                if (Height - y < chipSize.Height)
                {
                    point.Y++;
                }
                if (Height - y < chipSize.Height / 2)
                {
                    point.Y += 2;
                }
            }
            State state = Global.state;
            state.MapPoint.X += point.X * chipSize.Width;
            State state2 = Global.state;
            state2.MapPoint.Y += point.Y * chipSize.Height;
            Global.state.AdjustMapPoint();
            Refresh();
            Global.MainWnd.CommitScrollbar();
        }

        private void GUIDesigner_MouseUp(object sender, MouseEventArgs e)
        {
            if (MousePressed)
            {
                MousePressed = false;
                switch (CopyPaste)
                {
                    case CopyPasteTool.Copy:
                    case CopyPasteTool.Cut:
                        {
                            EnsureScroll(e.X, e.Y);
                            int width = Global.cpd.runtime.Definitions.ChipSize.Width;
                            double zoomIndex = Global.config.draw.ZoomIndex;
                            double num = 1.0 / Global.config.draw.ZoomIndex;
                            using (Graphics.FromImage(ForeLayerBmp))
                            {
                                Rectangle rectangle = default;
                                if (MouseStartPoint.X > MouseLastPoint.X)
                                {
                                    rectangle.X = MouseLastPoint.X;
                                    rectangle.Width = MouseStartPoint.X - MouseLastPoint.X;
                                }
                                else
                                {
                                    rectangle.X = MouseStartPoint.X;
                                    rectangle.Width = MouseLastPoint.X - MouseStartPoint.X;
                                }
                                if (MouseStartPoint.Y > MouseLastPoint.Y)
                                {
                                    rectangle.Y = MouseLastPoint.Y;
                                    rectangle.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                }
                                else
                                {
                                    rectangle.Y = MouseStartPoint.Y;
                                    rectangle.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                }
                                StringBuilder stringBuilder = new();
                                try
                                {
                                    for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
                                    {
                                        if (Global.state.Use3rdMapDataCurrently)
                                        {
                                            if (Global.state.EditingForeground)
                                            {
                                                stringBuilder.Append(string.Join(",", Global.cpd.EditingMap[i].Split(',').Skip(rectangle.Left).Take(rectangle.Width + 1)));
                                            }
                                            else
                                            {
                                                stringBuilder.Append(string.Join(",", Global.cpd.EditingLayer[i].Split(',').Skip(rectangle.Left).Take(rectangle.Width + 1)));
                                            }
                                        }
                                        else
                                        {
                                            if (Global.state.EditingForeground)
                                            {
                                                stringBuilder.Append(Global.cpd.EditingMap[i].AsSpan(rectangle.Left * Global.state.GetCByte, (rectangle.Width + 1) * Global.state.GetCByte));
                                            }
                                            else
                                            {
                                                stringBuilder.Append(Global.cpd.EditingLayer[i].AsSpan(rectangle.Left * Global.state.GetCByte, (rectangle.Width + 1) * Global.state.GetCByte));
                                            }
                                        }
                                        if (i != rectangle.Bottom)
                                        {
                                            stringBuilder.AppendLine();
                                        }
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    Global.MainWnd.CopyPasteInit();
                                    Refresh();
                                    Global.MainWnd.UpdateStatus("選択範囲が異常です。処理を実行できません");
                                    break;
                                }
                                Clipboard.SetText(stringBuilder.ToString());
                                if (CopyPaste == CopyPasteTool.Cut)
                                {
                                    Global.MainWnd.UpdateStatus("切り取りしています...");
                                    for (int j = rectangle.Top; j <= rectangle.Bottom; j++)
                                    {
                                        if (Global.state.Use3rdMapDataCurrently)
                                        {
                                            string[] array = PutItemTextCodeStart(j);
                                            if (array != null)
                                            {
                                                for (int k = rectangle.Left; k <= rectangle.Right; k++)
                                                {
                                                    if (Global.state.EditingForeground)
                                                    {
                                                        PutItemText(ref array, new Point(k, j), Global.cpd.Mapchip[0]);
                                                    }
                                                    else
                                                    {
                                                        PutItemText(ref array, new Point(k, j), Global.cpd.Layerchip[0]);
                                                    }
                                                }
                                                PutItemTextEnd(array, j);
                                            }
                                        }
                                        else
                                        {
                                            char[] array = PutItemTextStart(j);
                                            if (array != null)
                                            {
                                                for (int k = rectangle.Left; k <= rectangle.Right; k++)
                                                {
                                                    if (Global.state.EditingForeground)
                                                    {
                                                        PutItemText(ref array, new Point(k, j), Global.cpd.Mapchip[0]);
                                                    }
                                                    else
                                                    {
                                                        PutItemText(ref array, new Point(k, j), Global.cpd.Layerchip[0]);
                                                    }
                                                }
                                                PutItemTextEnd(array, j);
                                            }
                                        }
                                    }
                                    StageSourceToDrawBuffer();
                                    AddBuffer();
                                    DrawMode = DirectDrawMode.None;
                                }
                                else
                                {
                                    DrawMode = DirectDrawMode.None;
                                }
                            }
                            Global.MainWnd.CopyPasteInit();
                            Global.MainWnd.UpdateStatus("完了");
                            Refresh();
                            return;
                        }
                    case CopyPasteTool.Paste:
                        break;
                    default:
                        switch (CurrentTool)
                        {
                            case EditTool.Pen:
                                GUIDesigner_MouseMove(sender, e);
                                AddBuffer();
                                SavePrevDrawnLayer(Global.state.EditingForeground);
                                Global.MainWnd.UpdateStatus("完了");
                                return;
                            case EditTool.Line:
                                {
                                    DrawMode = DirectDrawMode.None;
                                    Rectangle dr = default;
                                    int num2 = 0;
                                    if (MouseStartPoint.X > MouseLastPoint.X)
                                    {
                                        dr.X = MouseLastPoint.X;
                                        dr.Width = MouseStartPoint.X - MouseLastPoint.X;
                                        num2++;
                                    }
                                    else
                                    {
                                        dr.X = MouseStartPoint.X;
                                        dr.Width = MouseLastPoint.X - MouseStartPoint.X;
                                        num2--;
                                    }
                                    if (MouseStartPoint.Y > MouseLastPoint.Y)
                                    {
                                        dr.Y = MouseLastPoint.Y;
                                        dr.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                        num2++;
                                    }
                                    else
                                    {
                                        dr.Y = MouseStartPoint.Y;
                                        dr.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                        num2--;
                                    }
                                    Global.MainWnd.UpdateStatus("線を描画しています...");
                                    if (Global.state.EditingForeground)
                                    {
                                        using (Graphics graphics2 = Graphics.FromImage(ForeLayerBmp))
                                        {
                                            DrawLine(graphics2, dr, num2 != 0);
                                            goto IL_5AE;
                                        }
                                    }
                                    using (Graphics graphics3 = Graphics.FromImage(BackLayerBmp[Global.state.EdittingLayerIndex]))
                                    {
                                        DrawLine(graphics3, dr, num2 != 0);
                                    }
                                IL_5AE:
                                    AddBuffer();
                                    Global.MainWnd.UpdateStatus("完了");
                                    Refresh();
                                    return;
                                }
                            case EditTool.Rect:
                                {
                                    EnsureScroll(e.X, e.Y);
                                    Global.MainWnd.UpdateStatus("描画しています...");
                                    Rectangle rectangle2 = default;
                                    if (MouseStartPoint.X > MouseLastPoint.X)
                                    {
                                        rectangle2.X = MouseLastPoint.X;
                                        rectangle2.Width = MouseStartPoint.X - MouseLastPoint.X;
                                    }
                                    else
                                    {
                                        rectangle2.X = MouseStartPoint.X;
                                        rectangle2.Width = MouseLastPoint.X - MouseStartPoint.X;
                                    }
                                    if (MouseStartPoint.Y > MouseLastPoint.Y)
                                    {
                                        rectangle2.Y = MouseLastPoint.Y;
                                        rectangle2.Height = MouseStartPoint.Y - MouseLastPoint.Y;
                                    }
                                    else
                                    {
                                        rectangle2.Y = MouseStartPoint.Y;
                                        rectangle2.Height = MouseLastPoint.Y - MouseStartPoint.Y;
                                    }
                                    for (int l = rectangle2.Top; l <= rectangle2.Bottom; l++)
                                    {
                                        if (Global.state.Use3rdMapDataCurrently)
                                        {
                                            string[] array2 = PutItemTextCodeStart(l);
                                            if (array2 != null)
                                            {
                                                for (int m = rectangle2.Left; m <= rectangle2.Right; m++)
                                                {
                                                    PutItemText(ref array2, new Point(m, l), Global.state.CurrentChip);
                                                }
                                                PutItemTextEnd(array2, l);
                                            }
                                        }
                                        else
                                        {
                                            char[] array2 = PutItemTextStart(l);
                                            if (array2 != null)
                                            {
                                                for (int m = rectangle2.Left; m <= rectangle2.Right; m++)
                                                {
                                                    PutItemText(ref array2, new Point(m, l), Global.state.CurrentChip);
                                                }
                                                PutItemTextEnd(array2, l);
                                            }
                                        }
                                    }
                                    StageSourceToDrawBuffer();
                                    AddBuffer();
                                    DrawMode = DirectDrawMode.None;
                                    Global.MainWnd.UpdateStatus("完了");
                                    Refresh();
                                    return;
                                }
                            default:
                                GUIDesigner_MouseMove(sender, e);
                                break;
                        }
                        break;
                }
            }
        }

        public void DrawLine(Graphics g, Rectangle dr, bool L2R)
        {
            Point point = new(0, 0);
            PointF pointF = new(0f, 0f);
            if (dr.Height > dr.Width)
            {
                point.Y = 1;
                if (L2R)
                {
                    pointF.X = dr.Width / (float)dr.Height;
                }
                else
                {
                    pointF.X = dr.Width / (float)dr.Height * -1f;
                }
            }
            else
            {
                if (L2R)
                {
                    point.X = 1;
                }
                else
                {
                    point.X = -1;
                }
                pointF.Y = dr.Height / (float)dr.Width;
            }
            double num = 0.0;
            double num2 = 0.0;
            Point point2 = new(L2R ? dr.Left : dr.Right, dr.Top);
            Point mapPos = point2;
            while (Math.Abs((int)Math.Round(num)) <= dr.Width && (int)Math.Round(num2) <= dr.Height)
            {
                PutItem(g, mapPos, Global.state.CurrentChip);
                num += point.X + (double)pointF.X;
                num2 += point.Y + (double)pointF.Y;
                mapPos.X = point2.X + (int)Math.Round(num);
                mapPos.Y = point2.Y + (int)Math.Round(num2);
            }
            SavePrevDrawnLayer(Global.state.EditingForeground);
            Refresh();
        }

        public void PutItem(Point MapPos)
        {
            PutItem(MapPos, Global.state.CurrentChip);
        }

        public void PutItem(Point MapPos, ChipsData cd)
        {
            Graphics graphics = null;
            try
            {
                if (Global.state.EditingForeground)
                {
                    graphics = Graphics.FromImage(ForeLayerBmp);
                }
                else
                {
                    graphics = Graphics.FromImage(BackLayerBmp[Global.state.EdittingLayerIndex]);
                }
                PutItem(graphics, MapPos, cd);
            }
            finally
            {
                graphics.Dispose();
            }
            Refresh();
        }

        public static void PutItemText(ref string[] ca, Point MapPos, ChipsData cd)
        {
            if (StageText.IsOverflow(MapPos))
            {
                return;
            }
            ca[MapPos.X] = cd.code;
        }

        public static void PutItemText(ref char[] ca, Point MapPos, ChipsData cd)
        {
            if (StageText.IsOverflow(MapPos))
            {
                return;
            }
            for (int i = 0; i < Global.state.GetCByte; i++)
            {
                ca[MapPos.X * Global.state.GetCByte + i] = cd.character[i];
            }
        }

        public static string[] PutItemTextCodeStart(int Y)
        {
            if (Y < 0 || Y >= Global.state.GetCSSize.y || !Global.cpd.project.Use3rdMapData && Global.state.MapEditMode)
            {
                return null;
            }
            if (Global.state.EditingForeground)
            {
                return Global.cpd.EditingMap[Y].Split(',');
            }
            return Global.cpd.EditingLayer[Y].Split(',');
        }

        public static char[] PutItemTextStart(int Y)
        {
            if (Y < 0 || Y >= Global.state.GetCSSize.y)
            {
                return null;
            }
            if (Global.state.EditingForeground)
            {
                return Global.cpd.EditingMap[Y].ToCharArray();
            }
            return Global.cpd.EditingLayer[Y].ToCharArray();
        }

        public static void PutItemTextEnd(string[] item, int Y)
        {
            string result = string.Join(",", item);
            if (Global.state.EditingForeground)
            {
                Global.cpd.EditingMap[Y] = result;
                return;
            }
            Global.cpd.EditingLayer[Y] = result;
        }

        public static void PutItemTextEnd(char[] item, int Y)
        {
            if (Global.state.EditingForeground)
            {
                Global.cpd.EditingMap[Y] = new string(item);
                return;
            }
            Global.cpd.EditingLayer[Y] = new string(item);
        }

        public static Point GetLargerPoint(Point fst, Point snd)
        {
            return new Point
            {
                X = (fst.X > snd.X) ? fst.X : snd.X,
                Y = (fst.Y > snd.Y) ? fst.Y : snd.Y
            };
        }

        public static Size GetLargerSize(Size fst, Size snd)
        {
            return new Size
            {
                Width = (fst.Width > snd.Width) ? fst.Width : snd.Width,
                Height = (fst.Height > snd.Height) ? fst.Height : snd.Height
            };
        }

        // チップを置いたとき
        public void PutItem(Graphics g, Point MapPos, ChipsData cd)
        {
            if (StageText.IsOverflow(MapPos)) return;
            string stageChar = StageText.GetStageChar(MapPos);
            if (Global.state.MapEditMode && cd.character.Equals(stageChar)
                || !Global.state.MapEditMode && (!Global.cpd.project.Use3rdMapData && cd.character.Equals(stageChar)
                    || Global.cpd.project.Use3rdMapData && cd.code == stageChar)) return;
            if (Global.state.Use3rdMapDataCurrently)
            {
                if (Global.state.EditingForeground)
                {
                    string[] array = Global.cpd.EditingMap[MapPos.Y].Split(',');
                    array[MapPos.X] = cd.code;
                    Global.cpd.EditingMap[MapPos.Y] = string.Join(",", array);
                }
                else
                {
                    string[] array2 = Global.cpd.EditingLayer[MapPos.Y].Split(',');
                    array2[MapPos.X] = cd.code;
                    Global.cpd.EditingLayer[MapPos.Y] = string.Join(",", array2);
                }
            }
            else
            {
                for (int i = 0; i < Global.state.GetCByte; i++)
                {
                    if (Global.state.EditingForeground)
                    {
                        char[] array = Global.cpd.EditingMap[MapPos.Y].ToCharArray();
                        array[MapPos.X * Global.state.GetCByte + i] = cd.character[i];
                        Global.cpd.EditingMap[MapPos.Y] = new string(array);
                    }
                    else
                    {
                        char[] array2 = Global.cpd.EditingLayer[MapPos.Y].ToCharArray();
                        array2[MapPos.X * Global.state.GetCByte + i] = cd.character[i];
                        Global.cpd.EditingLayer[MapPos.Y] = new string(array2);
                    }
                }
            }
            Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
            if (Global.state.EditingForeground)
            { // 標準レイヤー
                Size size = default;
                if (cd.character != null || cd.code != null)
                {
                    if (Global.state.ChipRegister.TryGetValue("oriboss_v", out string value) && int.Parse(value) == 3 && cd.character == "Z" && DrawOribossOrig != null)
                    {
                        size = DrawOribossOrig.Size;
                    }
                    else
                    {
                        size = (cd.GetCSChip().view_size != default) ? cd.GetCSChip().view_size : cd.GetCSChip().size; // 置きたいチップデータのサイズ

                        if (cd.GetCSChip().name.Contains("曲線による") && cd.GetCSChip().name.Contains('坂') &&
                            128 + MapPos.Y * chipsize.Height < CurrentStageSize.y * chipsize.Height)
                            size.Height += CurrentStageSize.y * chipsize.Height - (128 + MapPos.Y * chipsize.Height);
                        else if (cd.GetCSChip().name == "半円" && !cd.GetCSChip().description.Contains("乗れる") &&
                            64 + MapPos.Y * chipsize.Height < CurrentStageSize.y * chipsize.Height)
                            size.Height += CurrentStageSize.y * chipsize.Height - (64 + MapPos.Y * chipsize.Height);
                    }
                }
                ChipsData chipsData = default; // 置く前から元々あったチップデータのサイズ
                ChipData chipData = default;
                if (DrawItemRef.ContainsKey(stageChar) || DrawItemCodeRef.ContainsKey(stageChar))
                {
                    if (Global.state.MapEditMode) chipsData = DrawWorldRef[stageChar];
                    else
                    {
                        if (Global.cpd.project.Use3rdMapData) chipsData = DrawItemCodeRef[stageChar];
                        else chipsData = DrawItemRef[stageChar];
                    }
                    chipData = chipsData.GetCSChip();
                    if (Global.state.ChipRegister.TryGetValue("oriboss_v", out string value) && int.Parse(value) == 3 && chipsData.character == "Z" && DrawOribossOrig != null)
                        size = GetLargerSize(size, DrawOribossOrig.Size);
                    else if (chipData.name.Contains("曲線による") && chipData.name.Contains('坂') || chipData.name == "半円" && !chipData.description.Contains("乗れる"))
                        size = GetLargerSize(size, new Size(chipData.view_size.Width, CurrentStageSize.y * chipsize.Height));
                    else size = GetLargerSize(size, (chipData.view_size != default) ? chipData.view_size : chipData.size);  //サイズを比較して、大きい方に合わせる
                }
                if (size == default) RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
                else
                {
                    Point largerPoint = GetLargerPoint(cd.GetCSChip().center, chipData.center);
                    if (Global.state.ChipRegister.TryGetValue("oriboss_v", out string value) && int.Parse(value) == 3)
                    {
                        if (cd.character == "Z") largerPoint = GetLargerPoint(new Point(0, 0), chipData.center);
                        else if (chipsData.character == "Z") largerPoint = GetLargerPoint(cd.GetCSChip().center, new Point(0, 0));
                    }
                    Rectangle rectangle = new(MapPos.X * chipsize.Width - largerPoint.X, MapPos.Y * chipsize.Height - largerPoint.Y, size.Width, size.Height);
                    RedrawMap(g,
                        new Rectangle((int)Math.Ceiling((decimal)rectangle.X / chipsize.Width), (int)Math.Ceiling((decimal)rectangle.Y / chipsize.Height),
                            (int)Math.Ceiling((decimal)rectangle.Width / chipsize.Width), (int)Math.Ceiling((decimal)rectangle.Height / chipsize.Height)));
                }
            }
            else
            { // 背景レイヤー
                ChipData cschip = cd.GetCSChip();
                Size size2 = cschip.size;
                ChipData cschip2 = default(ChipsData).GetCSChip();
                if (DrawLayerRef.ContainsKey(stageChar) || DrawLayerCodeRef.ContainsKey(stageChar))
                {
                    size2 = GetLargerSize(size2, cschip2.size);
                }
                if (size2 == default) RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
                else
                {
                    Point largerPoint2 = GetLargerPoint(cschip.center, cschip2.center);
                    Rectangle rectangle2 = new(MapPos.X * chipsize.Width - largerPoint2.X, MapPos.Y * chipsize.Height - largerPoint2.Y, size2.Width, size2.Height);
                    RedrawMap(g,
                        new Rectangle(rectangle2.X / chipsize.Width, rectangle2.Y / chipsize.Height,
                            rectangle2.Width / chipsize.Width, rectangle2.Height / chipsize.Height));
                }
            }
        }

        public void RedrawMap(Graphics g, Rectangle rect)
        {
            Size chipsize = LogicalToDeviceUnits(Global.cpd.runtime.Definitions.ChipSize);
            using Bitmap bitmap = new(rect.Width * chipsize.Width, rect.Height * chipsize.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.FillRectangle(Brushes.Transparent, new Rectangle(new Point(0, 0), bitmap.Size));
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                Point point = new(-1, -1);
                for (int i = rect.Y; i < rect.Bottom; i++)
                {
                    point.Y++;
                    point.X = -1;
                    for (int j = rect.X; j < rect.Right; j++)
                    {
                        point.X++;
                        if (!StageText.IsOverflow(new Point(j, i)))
                        {
                            string stageChar = StageText.GetStageChar(new Point(j, i));
                            ChipsData chipsData;
                            if (Global.state.EditingForeground)
                            {
                                if (!DrawItemRef.ContainsKey(stageChar) && !DrawItemCodeRef.ContainsKey(stageChar))
                                {
                                    goto IL_514;
                                }
                                if (Global.state.MapEditMode)
                                {
                                    chipsData = DrawWorldRef[stageChar];
                                }
                                else
                                {
                                    if (Global.cpd.project.Use3rdMapData) chipsData = DrawItemCodeRef[stageChar];
                                    else chipsData = DrawItemRef[stageChar];
                                }

                                if (Global.state.MapEditMode && chipsData.character.Equals(Global.cpd.Mapchip[0].character)
                                    || !Global.state.MapEditMode
                                        && (Global.cpd.project.Use3rdMapData && chipsData.code == Global.cpd.Mapchip[0].code
                                        || !Global.cpd.project.Use3rdMapData && chipsData.character.Equals(Global.cpd.Mapchip[0].character)))
                                {
                                    goto IL_514;
                                }
                            }
                            else
                            {
                                if (!DrawLayerRef.ContainsKey(stageChar) && !DrawLayerCodeRef.ContainsKey(stageChar))
                                {
                                    goto IL_514;
                                }
                                if (Global.cpd.project.Use3rdMapData) chipsData = DrawLayerCodeRef[stageChar];
                                else chipsData = DrawLayerRef[stageChar];

                                if (Global.cpd.project.Use3rdMapData && chipsData.code == Global.cpd.Layerchip[0].code
                                   || !Global.cpd.project.Use3rdMapData && chipsData.character.Equals(Global.cpd.Layerchip[0].character))
                                {
                                    goto IL_514;
                                }
                            }
                            ChipData cschip = chipsData.GetCSChip();
                            if (cschip.size != default) DrawExtendSizeMap(cschip, graphics, point, Global.state.EditingForeground, chipsData.character, Global.state.EdittingLayerIndex);
                        }
                    IL_514:;
                    }
                }
                point = new Point(-1, -1); // こっちは何のためにあるのか不明
                for (int k = rect.Y; k < rect.Bottom; k++)
                {
                    point.Y++;
                    point.X = -1;
                    for (int l = rect.X; l < rect.Right; l++)
                    {
                        point.X++;
                        if (!StageText.IsOverflow(new Point(l, k)))
                        {
                            string stageChar = StageText.GetStageChar(new Point(l, k));
                            ChipsData chipsData;
                            if (Global.state.EditingForeground)
                            {
                                if (!DrawItemRef.ContainsKey(stageChar) && !DrawItemCodeRef.ContainsKey(stageChar))
                                {
                                    goto IL_9DD;
                                }
                                if (Global.state.MapEditMode)
                                {
                                    chipsData = DrawWorldRef[stageChar];
                                }
                                else
                                {
                                    if (Global.cpd.project.Use3rdMapData) chipsData = DrawItemCodeRef[stageChar];
                                    else chipsData = DrawItemRef[stageChar];
                                }

                                if (Global.state.MapEditMode && chipsData.character.Equals(Global.cpd.Mapchip[0].character)
                                    || !Global.state.MapEditMode
                                        && (Global.cpd.project.Use3rdMapData && chipsData.code == Global.cpd.Mapchip[0].code
                                        || !Global.cpd.project.Use3rdMapData && chipsData.character.Equals(Global.cpd.Mapchip[0].character)))
                                {
                                    goto IL_9DD;
                                }
                            }
                            else
                            {
                                if (!DrawLayerRef.ContainsKey(stageChar) && !DrawLayerCodeRef.ContainsKey(stageChar))
                                {
                                    goto IL_9DD;
                                }
                                if (Global.cpd.project.Use3rdMapData) chipsData = DrawLayerCodeRef[stageChar];
                                else chipsData = DrawLayerRef[stageChar];

                                if (Global.cpd.project.Use3rdMapData && chipsData.code == Global.cpd.Layerchip[0].code
                                   || !Global.cpd.project.Use3rdMapData && chipsData.character.Equals(Global.cpd.Layerchip[0].character))
                                {
                                    goto IL_9DD;
                                }
                            }
                            ChipData cschip = chipsData.GetCSChip();
                            if (cschip.size == default)
                            {
                                DrawNormalSizeMap(cschip, graphics, point, Global.state.EditingForeground, chipsData.character, rect.X, Global.state.EdittingLayerIndex);
                                if (chipsData.idColor != null)
                                {
                                    ChipRenderer.DrawIdColorMark(graphics, new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsData.idColor, this);
                                }
                            }
                        }
                    IL_9DD:;
                    }
                }
            }
            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawImage(bitmap, rect.X * chipsize.Width, rect.Y * chipsize.Height);
            g.CompositingMode = CompositingMode.SourceOver;
        }

        private void GUIDesigner_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (MousePressed)
            {
                GUIDesigner_MouseUp(sender, new MouseEventArgs(MouseButtons.None, 1, Cursor.Position.X, Cursor.Position.Y, 0));
            }
        }

        private void GUIDesigner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Y:
                        Redo();
                        break;
                    case Keys.Z:
                        Undo();
                        return;
                    default:
                        return;
                }
            }
        }

        private void QuickTestRun_Click(object sender, EventArgs e)
        {
            Point mouseStartPoint = MouseStartPoint;
            if (StageText.IsOverflow(mouseStartPoint))
            {
                return;
            }
            List<string> list = [];
            foreach (string text in Global.cpd.EditingMap)
            {
                if (Global.state.Use3rdMapDataCurrently)
                {
                    string[] t = text.Split(',');
                    for (int i = 0; i < t.Length; i++)
                    {
                        if (t[i] == Global.cpd.Mapchip[1].code) t[i] = Global.cpd.Mapchip[0].code;
                    }
                    list.Add(string.Join(",", t));
                }
                else
                {
                    list.Add(text.Replace(Global.cpd.Mapchip[1].character, Global.cpd.Mapchip[0].character));
                }
            }
            if (Global.state.Use3rdMapDataCurrently)
            {
                string[] l = list[mouseStartPoint.Y].Split(',');
                l[mouseStartPoint.X] = Global.cpd.Mapchip[1].code;
                list[mouseStartPoint.Y] = string.Join(",", l);
            }
            else
            {
                int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : Global.cpd.runtime.Definitions.StageSize.bytesize;
                list[mouseStartPoint.Y] = list[mouseStartPoint.Y].Remove(mouseStartPoint.X * num, num);
                list[mouseStartPoint.Y] = list[mouseStartPoint.Y].Insert(mouseStartPoint.X * num, Global.cpd.Mapchip[1].character);
            }
            Global.state.QuickTestrunSource = [.. list];
            Global.MainWnd.Testrun();
        }

        private void PickChip_Click(object sender, EventArgs e)
        {
            Point mouseStartPoint = MouseStartPoint;
            string stageChar = StageText.GetStageChar(mouseStartPoint);
            if (Global.state.MapEditMode)
            {
                if (DrawWorldRef.TryGetValue(stageChar, out ChipsData value))
                {
                    Global.state.CurrentChip = value;
                    return;
                }
            }
            else if (Global.state.EditingForeground)
            {
                if (Global.cpd.project.Use3rdMapData)
                {
                    if (DrawItemCodeRef.TryGetValue(stageChar, out ChipsData value))
                    {
                        Global.state.CurrentChip = value;
                        return;
                    }
                }
                else if (DrawItemRef.TryGetValue(stageChar, out ChipsData value))
                {
                    Global.state.CurrentChip = value;
                    return;
                }
            }
            else
            {
                if (Global.cpd.project.Use3rdMapData)
                {
                    if (DrawLayerCodeRef.TryGetValue(stageChar, out ChipsData value))
                    {
                        Global.state.CurrentChip = value;
                        return;
                    }
                }
                else if (DrawLayerRef.TryGetValue(stageChar, out ChipsData value))
                {
                    Global.state.CurrentChip = value;
                    return;
                }
            }
        }

        private void MenuCut_Click(object sender, EventArgs e)
        {
            Global.MainWnd.GuiCut.Checked = true;
            Global.MainWnd.GuiCut_Click(sender, e);
            Refresh();
        }

        private void MenuCopy_Click(object sender, EventArgs e)
        {
            Global.MainWnd.GuiCopy.Checked = true;
            Global.MainWnd.GuiCopy_Click(sender, e);
            Refresh();
        }

        private void MenuPaste_Click(object sender, EventArgs e)
        {
            Global.MainWnd.GuiPaste.Checked = true;
            Global.MainWnd.GuiPaste_Click(sender, e);
            Refresh();
        }

        private void DoTestrun_Click(object sender, EventArgs e)
        {
            Global.MainWnd.Testrun();
        }

        private void ProjectConfig_Click(object sender, EventArgs e)
        {
            Global.MainWnd.ProjectConfig_Click(this, new EventArgs());
        }

        protected override void WndProc(ref Message m)
        {
            // WM_MOUSEHWHEEL = 0x020E
            if (m.Msg == 0x020E)
            {
                // 親指ホイールのイベントを処理
                int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
                OnMouseHorizontalWheel(new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
                return;
            }
            base.WndProc(ref m);
        }

        protected virtual void OnMouseHorizontalWheel(MouseEventArgs e)
        {
            // 横スクロール量を計算
            int scrollAmount = e.Delta / 120 * Global.cpd.runtime.Definitions.ChipSize.Width;
            
            // 横スクロール処理を実行
            State state = Global.state;
            state.MapPoint.X -= scrollAmount;
            
            Global.state.AdjustMapPoint();
            Global.MainWnd.CommitScrollbar();
            Refresh();
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
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            components = new Container();
            CursorContextMenu = new ContextMenuStrip(components);
            QuickTestRun = new ToolStripMenuItem();
            PickChip = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripSeparator();
            MenuCut = new ToolStripMenuItem();
            MenuCopy = new ToolStripMenuItem();
            MenuPaste = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            DoTestrun = new ToolStripMenuItem();
            ProjectConfig = new ToolStripMenuItem();
            CursorContextMenu.SuspendLayout();
            SuspendLayout();
            CursorContextMenu.Items.AddRange(
            [
                QuickTestRun,
                PickChip,
                toolStripMenuItem3,
                MenuCut,
                MenuCopy,
                MenuPaste,
                toolStripMenuItem2,
                DoTestrun,
                ProjectConfig
            ]);
            CursorContextMenu.Name = "CursorContextMenu";
            CursorContextMenu.Size = LogicalToDeviceUnits(new Size(275, 170));
            QuickTestRun.Image = new IconImageView(DeviceDpi, Resources.testrunplace).View();
            QuickTestRun.Name = "QuickTestRun";
            QuickTestRun.ShortcutKeyDisplayString = "MiddleClick";
            QuickTestRun.Size = LogicalToDeviceUnits(new Size(274, 22));
            QuickTestRun.Text = "ここからテスト実行(&R)";
            QuickTestRun.Click += QuickTestRun_Click;
            PickChip.Name = "PickChip";
            PickChip.ShortcutKeyDisplayString = "";
            PickChip.Size = LogicalToDeviceUnits(new Size(274, 22));
            PickChip.Text = "直下のチップを拾う(&P)";
            PickChip.Click += PickChip_Click;
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = LogicalToDeviceUnits(new Size(271, 6));
            MenuCut.Image = new IconImageView(DeviceDpi, Resources.cut).View();
            MenuCut.Name = "MenuCut";
            MenuCut.ShortcutKeyDisplayString = "Ctrl+X";
            MenuCut.Size = LogicalToDeviceUnits(new Size(274, 22));
            MenuCut.Text = "切り取り(&X)";
            MenuCut.Click += MenuCut_Click;
            MenuCopy.Image = new IconImageView(DeviceDpi, Resources.copy).View();
            MenuCopy.Name = "MenuCopy";
            MenuCopy.ShortcutKeyDisplayString = "Ctrl+C";
            MenuCopy.Size = LogicalToDeviceUnits(new Size(274, 22));
            MenuCopy.Text = "コピー(&C)";
            MenuCopy.Click += MenuCopy_Click;
            MenuPaste.Image = new IconImageView(DeviceDpi, Resources.paste).View();
            MenuPaste.Name = "MenuPaste";
            MenuPaste.ShortcutKeyDisplayString = "Ctrl+V";
            MenuPaste.Size = LogicalToDeviceUnits(new Size(274, 22));
            MenuPaste.Text = "貼り付け(&V)";
            MenuPaste.Click += MenuPaste_Click;
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = LogicalToDeviceUnits(new Size(271, 6));
            DoTestrun.Image = new IconImageView(DeviceDpi, Resources.testrunstage).View();
            DoTestrun.Name = "DoTestrun";
            DoTestrun.ShortcutKeyDisplayString = "Ctrl+Space";
            DoTestrun.Size = LogicalToDeviceUnits(new Size(274, 22));
            DoTestrun.Text = "テスト実行(&T)";
            DoTestrun.Click += DoTestrun_Click;
            ProjectConfig.Image = new IconImageView(DeviceDpi, Resources.map_edit).View();
            ProjectConfig.Name = "ProjectConfig";
            ProjectConfig.ShortcutKeyDisplayString = "F2";
            ProjectConfig.Size = LogicalToDeviceUnits(new Size(274, 22));
            ProjectConfig.Text = "プロジェクト設定(&P)";
            ProjectConfig.Click += ProjectConfig_Click;
            Name = "GUIDesigner";
            Size = LogicalToDeviceUnits(new Size(315, 177));
            MouseCaptureChanged += GUIDesigner_MouseCaptureChanged;
            MouseMove += GUIDesigner_MouseMove;
            MouseDown += GUIDesigner_MouseDown;
            MouseUp += GUIDesigner_MouseUp;
            KeyDown += GUIDesigner_KeyDown;
            CursorContextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        public Image DrawChipOrig;

        // public Bitmap DrawMask;

        public Image DrawLayerOrigDefault;

        public List<Image> DrawLayerOrig;

        // public Bitmap DrawLayerMask;

        public Image DrawOribossOrig;

        // public Bitmap DrawOribossMask;

        public Image DrawExOrig;

        public Bitmap DrawExMask;

        public Image DrawHaikeiOrig, DrawHaikei2Orig, DrawHaikei3Orig, DrawHaikei4Orig, DrawSecondHaikeiOrig,
            DrawSecondHaikei2Orig, DrawSecondHaikei3Orig, DrawSecondHaikei4Orig,
            DrawChizuOrig;

        public Dictionary<string, ChipsData> DrawItemRef = [];

        public Dictionary<string, ChipsData> DrawLayerRef = [];

        public Dictionary<string, ChipsData> DrawWorldRef = [];

        public Dictionary<string, ChipsData> DrawItemCodeRef = [];

        public Dictionary<string, ChipsData> DrawLayerCodeRef = [];

        private Bitmap ForeLayerBmp;

        public List<Bitmap> BackLayerBmp = [];

        private EditTool curTool;

        private CopyPasteTool cpaste;

        public List<LayerObject> StageBuffer = [];

        public int BufferCurrent = -1;

        public string ClipedString;

        public Rectangle DrawRectangle = default;

        public DirectDrawMode DrawMode;

        private bool bdraw = true;

        private LayerObject ForePrevDrawn;

        private LayerObject BackPrevDrawn;

        private Bitmap ForegroundBuffer = new(1, 1);

        private int bufpos = -1;

        private double zi;

        private bool FLayer;

        private bool DULayer;

        private bool TULayer;

        private int EditMap = -1;

        private Point MouseStartPoint = default;

        private Point MouseLastPoint = default;

        private bool MousePressed;

        private readonly FillToolHelper fillToolHelper = new();

        private IContainer components;

        private ContextMenuStrip CursorContextMenu;

        private ToolStripMenuItem QuickTestRun;

        private ToolStripMenuItem DoTestrun;

        private ToolStripMenuItem MenuCut;

        private ToolStripMenuItem MenuCopy;

        private ToolStripMenuItem MenuPaste;

        private ToolStripSeparator toolStripMenuItem2;

        private ToolStripMenuItem ProjectConfig;

        private ToolStripMenuItem PickChip;

        private ToolStripSeparator toolStripMenuItem3;

        public class StageText
        {
            // 座標が画面内にあるかチェック
            public static bool IsOverflow(Point p)
            {
                return p.X < 0 || p.Y < 0 || p.X >= Global.state.GetCSSize.x || p.Y >= Global.state.GetCSSize.y;
            }

            public static string GetStageChar(Point p)
            {
                if (IsOverflow(p))
                {
                    if (Global.state.Use3rdMapDataCurrently) return Global.cpd.Mapchip[0].code;
                    else return Global.cpd.Mapchip[0].character;
                }
                if (Global.state.EditingForeground)
                {
                    if (Global.state.Use3rdMapDataCurrently) return Global.cpd.EditingMap[p.Y].Split(',')[p.X];
                    else return Global.cpd.EditingMap[p.Y].Substring(p.X * Global.state.GetCByte, Global.state.GetCByte);
                }
                if (Global.cpd.project.Use3rdMapData) return Global.cpd.EditingLayer[p.Y].Split(',')[p.X];
                else return Global.cpd.EditingLayer[p.Y].Substring(p.X * Global.state.GetCByte, Global.state.GetCByte);
            }
        }

        public delegate void ChangeBuffer();

        public enum DirectDrawMode
        {
            None,
            Line,
            RevLine,
            Rectangle
        }

        public enum EditTool
        {
            Cursor,
            Pen,
            Line,
            Rect,
            Fill
        }

        public enum CopyPasteTool
        {
            None,
            Copy,
            Cut,
            Paste
        }

        public struct KeepDrawData(ChipData c, Point p, string chara, string idColor = null)
        {
            public ChipData cd = c;

            public Point pos = p;

            public string chara = chara;

            public string idColor = idColor;
        }

        private void TranslateTransform(Graphics g, int dx, int dy)
        {
            g.TranslateTransform(LogicalToDeviceUnits(dx), LogicalToDeviceUnits(dy));
        }
    }
}

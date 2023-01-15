﻿using MasaoPlus.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MasaoPlus
{
    public class GUIDesigner : UserControl, IDisposable
	{
		public GUIDesigner.EditTool CurrentTool
		{
			get
			{
				return this.curTool;
			}
			set
			{
				this.BufferingDraw = value != GUIDesigner.EditTool.Pen;
				this.curTool = value;
				this.Refresh();
			}
		}

		public GUIDesigner.CopyPasteTool CopyPaste
		{
			get
			{
				return this.cpaste;
			}
			set
			{
				if (value == GUIDesigner.CopyPasteTool.None)
				{
					this.BufferingDraw = this.CurrentTool != GUIDesigner.EditTool.Pen;
				}
				else
				{
					this.BufferingDraw = true;
				}
				this.cpaste = value;
				this.Refresh();
			}
		}

		public event GUIDesigner.ChangeBuffer ChangeBufferInvoke;

		private bool BufferingDraw
		{
			get
			{
				return this.bdraw;
			}
			set
			{
				if (value && !this.bdraw)
				{
					this.bufpos = -1;
					this.Refresh();
				}
				this.bdraw = value;
			}
		}

		public void PaintStage(Graphics g, bool EnableExDraw)
		{
			bool extendDraw = Global.config.draw.ExtendDraw;
			Global.config.draw.ExtendDraw = EnableExDraw;
			if (Global.cpd.UseLayer)
			{
				this.UpdateBackgroundBuffer();
				g.DrawImage(this.BackLayerBmp, new Point(0, 0));
			}
			this.UpdateForegroundBuffer();
			g.DrawImage(this.ForeLayerBmp, new Point(0, 0));
			Global.config.draw.ExtendDraw = extendDraw;
			this.UpdateForegroundBuffer();
			this.UpdateBackgroundBuffer();
			this.InitTransparent();
			this.bufpos = -1;
			this.Refresh();
		}

		public void ClearBuffer()
		{
			this.StageBuffer.Clear();
			if (this.ChangeBufferInvoke != null)
			{
				this.ChangeBufferInvoke();
			}
		}

		public void AddBuffer()
		{
			if (this.StageBuffer.Count > this.BufferCurrent + 1)
			{
				this.StageBuffer.RemoveRange(this.BufferCurrent + 1, this.StageBuffer.Count - (this.BufferCurrent + 1));
			}
			if (Global.state.EditingForeground)
			{
				this.StageBuffer.Add((string[])Global.cpd.EditingMap.Clone());
			}
			else
			{
				this.StageBuffer.Add((string[])Global.cpd.EditingLayer.Clone());
			}
			this.BufferCurrent = this.StageBuffer.Count - 1;
			if (this.BufferCurrent != 0)
			{
				Global.state.EditFlag = true;
			}
			if (this.ChangeBufferInvoke != null)
			{
				this.ChangeBufferInvoke();
			}
		}

		public void Undo()
		{
			if (this.BufferCurrent <= 0)
			{
				return;
			}
			Global.state.EditFlag = true;
			Global.MainWnd.UpdateStatus("描画しています...");
			this.BufferCurrent--;
			if (Global.state.EditingForeground)
			{
				switch (Global.state.EdittingStage)
				{
				case 0:
					Global.cpd.project.StageData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData;
					break;
				case 1:
					Global.cpd.project.StageData2 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData2;
					break;
				case 2:
					Global.cpd.project.StageData3 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData3;
					break;
				case 3:
					Global.cpd.project.StageData4 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData4;
					break;
				case 4:
					Global.cpd.project.MapData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.MapData;
					break;
				}
			}
			else
			{
				switch (Global.state.EdittingStage)
				{
				case 0:
					Global.cpd.project.LayerData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData;
					break;
				case 1:
					Global.cpd.project.LayerData2 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData2;
					break;
				case 2:
					Global.cpd.project.LayerData3 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData3;
					break;
				case 3:
					Global.cpd.project.LayerData4 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData4;
					break;
				}
			}
			if (this.ChangeBufferInvoke != null)
			{
				this.ChangeBufferInvoke();
			}
			this.StageSourceToDrawBuffer();
			this.Refresh();
			Global.MainWnd.UpdateStatus("完了");
		}

		public void Redo()
		{
			if (this.BufferCurrent >= this.StageBuffer.Count - 1)
			{
				return;
			}
			Global.state.EditFlag = true;
			Global.MainWnd.UpdateStatus("描画しています...");
			this.BufferCurrent++;
			if (Global.state.EditingForeground)
			{
				switch (Global.state.EdittingStage)
				{
				case 0:
					Global.cpd.project.StageData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData;
					break;
				case 1:
					Global.cpd.project.StageData2 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData2;
					break;
				case 2:
					Global.cpd.project.StageData3 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData3;
					break;
				case 3:
					Global.cpd.project.StageData4 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.StageData4;
					break;
				case 4:
					Global.cpd.project.MapData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingMap = Global.cpd.project.MapData;
					break;
				}
			}
			else
			{
				switch (Global.state.EdittingStage)
				{
				case 0:
					Global.cpd.project.LayerData = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData;
					break;
				case 1:
					Global.cpd.project.LayerData2 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData2;
					break;
				case 2:
					Global.cpd.project.LayerData3 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData3;
					break;
				case 3:
					Global.cpd.project.LayerData4 = (string[])this.StageBuffer[this.BufferCurrent].Clone();
					Global.cpd.EditingLayer = Global.cpd.project.LayerData4;
					break;
				}
			}
			if (this.ChangeBufferInvoke != null)
			{
				this.ChangeBufferInvoke();
			}
			this.StageSourceToDrawBuffer();
			this.Refresh();
			Global.MainWnd.UpdateStatus("完了");
		}

		public Size BufferSize
		{
			get
			{
				if (Global.state.EditingForeground)
				{
					if (this.ForeLayerBmp != null)
					{
						return this.ForeLayerBmp.Size;
					}
				}
				else if (this.BackLayerBmp != null)
				{
					return this.BackLayerBmp.Size;
				}
				return default(Size);
			}
		}

		public Size DisplaySize
		{
			get
			{
				if (Global.state.EditingForeground)
				{
					if (this.ForeLayerBmp != null)
					{
						return new Size((int)((double)this.ForeLayerBmp.Width * Global.config.draw.ZoomIndex), (int)((double)this.ForeLayerBmp.Height * Global.config.draw.ZoomIndex));
					}
				}
				else if (this.BackLayerBmp != null)
				{
					return new Size((int)((double)this.BackLayerBmp.Width * Global.config.draw.ZoomIndex), (int)((double)this.BackLayerBmp.Height * Global.config.draw.ZoomIndex));
				}
				return default(Size);
			}
		}

		public Size CurrentChipSize
		{
			get
			{
				Size result = default(Size);
				if (Global.cpd.project == null)
				{
					return result;
				}
				result.Width = (int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
				result.Height = (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
				return result;
			}
		}

		public GUIDesigner()
		{
			this.InitializeComponent();
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}

		public void StageSourceToDrawBuffer()
		{
			if (Global.state.EditingForeground)
			{
				this.UpdateForegroundBuffer();
				return;
			}
			this.UpdateBackgroundBuffer();
		}

		public void InitTransparent()
		{
			if (Global.cpd.UseLayer && Global.state.TransparentUnactiveLayer)
			{
				if (Global.state.EditingForeground)
				{
					this.HalfTransparentBitmap2(ref this.BackLayerBmp);
					return;
				}
				this.HalfTransparentBitmap2(ref this.ForeLayerBmp);
			}
		}

		public void ForceBufferResize()
		{
			if (this.ForeLayerBmp != null)
			{
				this.ForeLayerBmp.Dispose();
				this.ForeLayerBmp = null;
			}
			if (this.BackLayerBmp != null)
			{
				this.BackLayerBmp.Dispose();
				this.BackLayerBmp = null;
			}
			this.bufpos = -1;
		}

		public void UpdateForegroundBuffer()
		{
			bool flag = false;
			if (this.ForeLayerBmp == null)
			{
				if (Global.state.MapEditMode)
				{
					this.ForeLayerBmp = new Bitmap(Global.cpd.runtime.Definitions.MapSize.x * Global.cpd.runtime.Definitions.ChipSize.Width, Global.cpd.runtime.Definitions.MapSize.y * Global.cpd.runtime.Definitions.ChipSize.Height, PixelFormat.Format32bppArgb);
				}
				else
				{
					this.ForeLayerBmp = new Bitmap(Global.cpd.runtime.Definitions.StageSize.x * Global.cpd.runtime.Definitions.ChipSize.Width, Global.cpd.runtime.Definitions.StageSize.y * Global.cpd.runtime.Definitions.ChipSize.Height, PixelFormat.Format32bppArgb);
				}
			}
			else if (!Global.state.UseBuffered)
			{
				flag = true;
			}
			using (Graphics graphics = Graphics.FromImage(this.ForeLayerBmp))
			{
				if (flag)
				{
					graphics.CompositingMode = CompositingMode.SourceCopy;
					graphics.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, this.ForeLayerBmp.Width, this.ForeLayerBmp.Height));
					graphics.CompositingMode = CompositingMode.SourceOver;
				}
				this.MakeDrawBuffer(graphics, true);
			}
			this.bufpos = -1;
		}

		public void UpdateBackgroundBuffer()
		{
			bool flag = false;
			if (!Global.cpd.UseLayer)
			{
				return;
			}
			if (this.BackLayerBmp == null)
			{
				this.BackLayerBmp = new Bitmap(Global.cpd.runtime.Definitions.LayerSize.x * Global.cpd.runtime.Definitions.ChipSize.Width, Global.cpd.runtime.Definitions.LayerSize.y * Global.cpd.runtime.Definitions.ChipSize.Height, PixelFormat.Format32bppArgb);
			}
			else if (!Global.state.UseBuffered)
			{
				flag = true;
			}
			using (Graphics graphics = Graphics.FromImage(this.BackLayerBmp))
			{
				if (flag)
				{
					graphics.CompositingMode = CompositingMode.SourceCopy;
					graphics.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, this.BackLayerBmp.Width, this.BackLayerBmp.Height));
					graphics.CompositingMode = CompositingMode.SourceOver;
				}
				this.MakeDrawBuffer(graphics, false);
			}
			this.bufpos = -1;
		}

		private void MakeDrawBuffer(Graphics g, bool foreground)
		{
			if (!Global.state.MapEditMode)
			{
				// セカンド背景画像固定表示
				if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll") && int.Parse(Global.state.ChipRegister["second_gazou_scroll"]) == 8 &&
				Global.state.ChipRegister.ContainsKey("second_gazou_priority") && int.Parse(Global.state.ChipRegister["second_gazou_priority"]) == 1)
				{
					int second_gazou_scroll_x = default, second_gazou_scroll_y = default;
					if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll_x")) second_gazou_scroll_x = int.Parse(Global.state.ChipRegister["second_gazou_scroll_x"]);
					if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll_y")) second_gazou_scroll_y = int.Parse(Global.state.ChipRegister["second_gazou_scroll_y"]);

					Image SecondHaikeiOrig = this.DrawSecondHaikeiOrig;
					if (Global.state.EdittingStage == 1) SecondHaikeiOrig = this.DrawSecondHaikei2Orig;
					else if (Global.state.EdittingStage == 2) SecondHaikeiOrig = this.DrawSecondHaikei3Orig;
					else if (Global.state.EdittingStage == 3) SecondHaikeiOrig = this.DrawSecondHaikei4Orig;
					g.DrawImage(SecondHaikeiOrig, second_gazou_scroll_x, second_gazou_scroll_y);
				}
				// 背景画像固定表示
				if (Global.state.ChipRegister.ContainsKey("gazou_scroll") && int.Parse(Global.state.ChipRegister["gazou_scroll"]) == 11)
				{
					int gazou_scroll_x = default, gazou_scroll_y = default;
					if (Global.state.ChipRegister.ContainsKey("gazou_scroll_x")) gazou_scroll_x = int.Parse(Global.state.ChipRegister["gazou_scroll_x"]);
					if (Global.state.ChipRegister.ContainsKey("gazou_scroll_y")) gazou_scroll_y = int.Parse(Global.state.ChipRegister["gazou_scroll_y"]);

					Image HaikeiOrig = this.DrawHaikeiOrig;
					if (Global.state.EdittingStage == 1) HaikeiOrig = this.DrawHaikei2Orig;
					else if (Global.state.EdittingStage == 2) HaikeiOrig = this.DrawHaikei3Orig;
					else if (Global.state.EdittingStage == 3) HaikeiOrig = this.DrawHaikei4Orig;
					g.DrawImage(HaikeiOrig, gazou_scroll_x, gazou_scroll_y);
				}
			}
            else
			{
				g.DrawImage(Global.MainWnd.MainDesigner.DrawChizuOrig, -16, -24);
			}

			ChipsData chipsData = default(ChipsData);
			ChipData c = default(ChipData);
			new List<GUIDesigner.KeepDrawData>();
			List<GUIDesigner.KeepDrawData> list = new List<GUIDesigner.KeepDrawData>();
			int num = 0;
			GraphicsState transState;
			g.PixelOffsetMode = PixelOffsetMode.Half;
			Size chipsize = Global.cpd.runtime.Definitions.ChipSize;

			while (Global.state.MapEditMode ? (num < Global.cpd.runtime.Definitions.MapSize.y) : (num < Global.cpd.runtime.Definitions.StageSize.y))
			{
				int num2 = 0;
				while (Global.state.MapEditMode ? (num2 < Global.cpd.runtime.Definitions.MapSize.x) : (num2 < Global.cpd.runtime.Definitions.StageSize.x))
				{
					string text;
					if (foreground)
					{
						if (Global.state.MapEditMode)
						{
							text = Global.cpd.EditingMap[num].Substring(num2 * Global.cpd.project.Runtime.Definitions.MapSize.bytesize, Global.cpd.project.Runtime.Definitions.MapSize.bytesize);
						}
						else
						{
							text = Global.cpd.EditingMap[num].Substring(num2 * Global.cpd.project.Runtime.Definitions.StageSize.bytesize, Global.cpd.project.Runtime.Definitions.StageSize.bytesize);
						}
					}
					else
					{
						text = Global.cpd.EditingLayer[num].Substring(num2 * Global.cpd.project.Runtime.Definitions.LayerSize.bytesize, Global.cpd.project.Runtime.Definitions.LayerSize.bytesize);
					}
					if ((!Global.state.UseBuffered || ((!foreground || this.ForePrevDrawn == null || !(this.ForePrevDrawn[num].Substring(Global.state.MapEditMode ? (num2 * Global.cpd.project.Runtime.Definitions.MapSize.bytesize) : (num2 * Global.cpd.project.Runtime.Definitions.StageSize.bytesize), Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : Global.cpd.project.Runtime.Definitions.StageSize.bytesize) == text)) && (foreground || this.BackPrevDrawn == null || !(this.BackPrevDrawn[num].Substring(num2 * Global.cpd.project.Runtime.Definitions.LayerSize.bytesize, Global.cpd.project.Runtime.Definitions.LayerSize.bytesize) == text)))) && (!Global.config.draw.SkipFirstChip || ((!foreground || !text.Equals(Global.cpd.Mapchip[0].character)) && (foreground || !text.Equals(Global.cpd.Layerchip[0].character)))) && ((foreground && this.DrawItemRef.ContainsKey(text)) || (!foreground && this.DrawLayerRef.ContainsKey(text))))
					{
						if (Global.state.MapEditMode) chipsData = this.DrawWorldRef[text];
						else if (foreground) chipsData = this.DrawItemRef[text];
						else chipsData = this.DrawLayerRef[text];
						c = chipsData.GetCSChip();
						if (c.size != default(Size)) // 標準サイズより大きい
						{
							if (Global.state.UseBuffered)
							{
								g.CompositingMode = CompositingMode.SourceCopy;
								g.FillRectangle(Brushes.Transparent, new Rectangle(num2 * chipsize.Width - c.center.X, num * chipsize.Height - c.center.Y, c.size.Width, c.size.Height));
								g.CompositingMode = CompositingMode.SourceOver;
							}
							if (Global.config.draw.ExtendDraw && c.xdraw != default(Point) && c.xdbackgrnd)
							{ // 拡張画像　背面
								g.DrawImage(this.DrawExOrig,
									new Rectangle(new Point(num2 * chipsize.Width - c.center.X, num * chipsize.Height - c.center.Y), chipsize),
									new Rectangle(c.xdraw, chipsize), GraphicsUnit.Pixel);
							}
							if (foreground)
							{ // 標準パターン画像
								if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
								{
									g.DrawImage(this.DrawOribossOrig, num2 * chipsize.Width, num * chipsize.Height);
								}
								else
								{
									transState = g.Save();
									if (c.view_size != default)
										g.TranslateTransform(num2 * chipsize.Width - c.center.X + c.view_size.Width / 2, num * chipsize.Height - c.center.Y + c.view_size.Height / 2);
									else
										g.TranslateTransform(num2 * chipsize.Width - c.center.X + c.size.Width / 2, num * chipsize.Height - c.center.Y + c.size.Height / 2);
									g.RotateTransform(c.rotate);
									if (c.scale != default) g.ScaleTransform(c.scale, c.scale);
									g.DrawImage(this.DrawChipOrig,
										new Rectangle(-c.size.Width / 2, -c.size.Height / 2, c.size.Width, c.size.Height),
										new Rectangle(c.pattern, c.size), GraphicsUnit.Pixel);
									g.Restore(transState);
								}
							}
							else
							{ // 背景レイヤー画像
								g.DrawImage(this.DrawLayerOrig,
									new Rectangle(num2 * chipsize.Width - c.center.X, num * chipsize.Height - c.center.Y, c.size.Width, c.size.Height),
									new Rectangle(c.pattern, c.size), GraphicsUnit.Pixel);
							}
							if (chipsData.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 &&
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
								g.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, num2 * chipsize.Width, num * chipsize.Height, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
							}
							else if (Global.config.draw.ExtendDraw && c.xdraw != default(Point) && !c.xdbackgrnd)
							{ // 拡張画像　前面
								g.DrawImage(this.DrawExOrig,
									new Rectangle(new Point(num2 * chipsize.Width - c.center.X, num * chipsize.Height - c.center.Y), chipsize),
									new Rectangle(c.xdraw, chipsize), GraphicsUnit.Pixel);
							}
						}
						else
						{ // 標準サイズの画像はリストに追加後、↓で描画
							list.Add(new GUIDesigner.KeepDrawData(c, new Point(num2, num), chipsData.character));
						}
					}
					num2++;
				}
				num++;
			}
			foreach (GUIDesigner.KeepDrawData keepDrawData in list)
			{
				ChipData cschip = keepDrawData.cd;
				if (Global.state.UseBuffered)
				{
					g.CompositingMode = CompositingMode.SourceCopy;
					g.FillRectangle(Brushes.Transparent, new Rectangle(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height,
						chipsize.Width, chipsize.Height));
					g.CompositingMode = CompositingMode.SourceOver;
				}
				if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && cschip.xdbackgrnd)
				{ // 拡張画像　背面
					g.DrawImage(this.DrawExOrig,
						new Rectangle(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height, chipsize.Width, chipsize.Height),
						new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
				}
				if (foreground)
				{ // 標準パターン画像
					transState = g.Save();
					g.TranslateTransform(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height);
					Pen pen, pen2;
					SolidBrush brush;
					PointF[] vo_pa;
					double rad = 0;
					const double math_pi = 3.1415926535897931;
					switch (cschip.name)
					{
						case "一方通行":
							if (cschip.description.Contains("表示なし")) break;
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
							break;
						case "左右へ押せるドッスンスンのゴール":
							g.SmoothingMode = SmoothingMode.AntiAlias;
							g.TranslateTransform(-cschip.center.X + 1, -cschip.center.Y + 1);
							pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
							g.DrawRectangle(pen, 0, 0, 94, 62);
							g.DrawLine(pen, 0, 0, 94, 62);
							g.DrawLine(pen, 0, 62, 94, 0);
							pen.Dispose();
							break;
						case "シーソー":
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
							break;
						case "ブランコ":
							g.DrawImage(this.DrawChipOrig,
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
								g.DrawImage(this.DrawChipOrig,
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
							break;
						case "スウィングバー":
							g.DrawImage(this.DrawChipOrig,
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
							break;
						case "動くＴ字型":
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
							break;
						case "ロープ":
						case "長いロープ":
						case "ゆれる棒":
							g.DrawImage(this.DrawChipOrig,
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
								g.DrawImage(this.DrawChipOrig,
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
							break;
						case "人間大砲":
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
							break;
						case "曲線による上り坂":
						case "曲線による下り坂":
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
								if (128 + keepDrawData.pos.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
									g.FillRectangle(brush, 0, 128, 256, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (128 + keepDrawData.pos.Y * chipsize.Height));
								brush.Dispose();
							}
							break;
						case "乗れる円":
						case "跳ねる円":
						case "円":
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
							break;
						case "半円":
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
								if (64 + keepDrawData.pos.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
									g.FillRectangle(brush, 120 - 20, 64, 40, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (64 + keepDrawData.pos.Y * chipsize.Height));
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
							break;
						case "ファイヤーバー":
						case "ファイヤーバー2本":
						case "ファイヤーバー3本　左回り":
						case "ファイヤーバー3本　右回り":
						case "スウィングファイヤーバー":
							{
								int width = default;
								g.DrawImage(this.DrawChipOrig,
									new Rectangle(0, 0, chipsize.Width, chipsize.Height),
									new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
								g.SmoothingMode = SmoothingMode.AntiAlias;
								g.TranslateTransform(16, 16);

								int v = default;
								if (cschip.name == "ファイヤーバー2本" || cschip.name == "ファイヤーバー3本　左回り") v = 360 - 2;
								else if (cschip.name == "ファイヤーバー3本　右回り") v = 2;
								else if (cschip.name == "スウィングファイヤーバー") {
									if(cschip.description.Contains("左")) v = 160 + 2;
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
										else if (cschip.name == "ファイヤーバー3本　右回り")v = 2 + 240;
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
							}
							break;
						case "人口太陽":
							{
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
							}
							break;
						case "ファイヤーリング":
							{
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
							}
							break;
						default:
							g.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
							if (keepDrawData.chara == Global.cpd.Mapchip[1].character)
							{
								g.ScaleTransform(-1, 1); // 基本主人公は逆向き
								if (Global.state.MapEditMode && keepDrawData.pos.X > Global.cpd.runtime.Definitions.MapSize.x / 2 ||
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
									g.DrawImage(this.DrawChipOrig,
										new Rectangle(-chipsize.Width / 2 + j * chipsize.Width * Math.Sign(cschip.rotate), -chipsize.Height / 2, chipsize.Width, chipsize.Height),
										new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
								}
							}
							else if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && keepDrawData.chara == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
							{// 水の半透明処理
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
								g.DrawImage(this.DrawChipOrig, new Rectangle(-chipsize.Width / 2, -chipsize.Height / 2, chipsize.Width, chipsize.Height), cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
							}
							else g.DrawImage(this.DrawChipOrig,
								new Rectangle(-chipsize.Width / 2, -chipsize.Height / 2, chipsize.Width, chipsize.Height),
								new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
							break;
					}
					g.Restore(transState);
				}
				else
				{ // 背景レイヤー画像
					g.DrawImage(this.DrawLayerOrig,
						new Rectangle(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height, chipsize.Width, chipsize.Height),
						new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
				}
				if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && !cschip.xdbackgrnd)
				{ // 拡張画像　前面
					g.DrawImage(this.DrawExOrig,
						 new Rectangle(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height, chipsize.Width, chipsize.Height),
						 new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
				}
			}
			if (!Global.state.MapEditMode)
			{
				// オリジナルボスを座標で設置する場合に描画
				if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 2 && foreground)
				{
					int oriboss_x = default, oriboss_y = default;
					if (Global.state.ChipRegister.ContainsKey("oriboss_x")) oriboss_x = int.Parse(Global.state.ChipRegister["oriboss_x"]);
					if (Global.state.ChipRegister.ContainsKey("oriboss_y")) oriboss_y = int.Parse(Global.state.ChipRegister["oriboss_y"]);
					g.DrawImage(this.DrawOribossOrig, oriboss_x * chipsize.Width, oriboss_y * chipsize.Height);
					if (Global.state.ChipRegister.ContainsKey("oriboss_ugoki") && Global.config.draw.ExtendDraw)
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
						g.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, oriboss_x * chipsize.Width, oriboss_y * chipsize.Height, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
					}
				}
				// セカンド前景画像固定表示
				if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll") && int.Parse(Global.state.ChipRegister["second_gazou_scroll"]) == 8 &&
					Global.state.ChipRegister.ContainsKey("second_gazou_priority") && int.Parse(Global.state.ChipRegister["second_gazou_priority"]) == 2)
				{
					int second_gazou_scroll_x = default, second_gazou_scroll_y = default;
					if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll_x")) second_gazou_scroll_x = int.Parse(Global.state.ChipRegister["second_gazou_scroll_x"]);
					if (Global.state.ChipRegister.ContainsKey("second_gazou_scroll_y")) second_gazou_scroll_y = int.Parse(Global.state.ChipRegister["second_gazou_scroll_y"]);

					Image SecondHaikeiOrig = this.DrawSecondHaikeiOrig;
					if (Global.state.EdittingStage == 2) SecondHaikeiOrig = this.DrawSecondHaikei2Orig;
					else if (Global.state.EdittingStage == 3) SecondHaikeiOrig = this.DrawSecondHaikei3Orig;
					else if (Global.state.EdittingStage == 4) SecondHaikeiOrig = this.DrawSecondHaikei4Orig;
					g.DrawImage(SecondHaikeiOrig, second_gazou_scroll_x, second_gazou_scroll_y);
				}
			}
			if (foreground)
			{
				this.ForePrevDrawn = (string[])Global.cpd.EditingMap.Clone();
			}
			else
			{
				this.BackPrevDrawn = (string[])Global.cpd.EditingLayer.Clone();
			}
			list.Clear();
		}

		public void TransparentStageClip(Bitmap b, Rectangle stagearea)
		{
			Size chipSize = Global.cpd.runtime.Definitions.ChipSize;
			this.TransparentClip(b, new Rectangle(stagearea.X * chipSize.Width, stagearea.Y * chipSize.Height, stagearea.Width * chipSize.Width, stagearea.Height * chipSize.Height));
		}

		public unsafe void TransparentClip(Bitmap b, Rectangle area)
		{
			try
			{
				BitmapData bitmapData = b.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				byte* ptr = (byte*)((void*)bitmapData.Scan0);
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
				this.StageSourceToDrawBuffer();
			}
		}

		public unsafe void HalfTransparentBitmap(Bitmap b)
		{
			byte b2 = 125;
			byte b3 = 0;
			BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			byte* ptr = (byte*)((void*)bitmapData.Scan0);
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

		public void HalfTransparentBitmap2(ref Bitmap b)
		{
			Bitmap bitmap = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 1f;
				colorMatrix.Matrix11 = 1f;
				colorMatrix.Matrix22 = 1f;
				colorMatrix.Matrix33 = 0.5f;
				colorMatrix.Matrix44 = 1f;
				using (ImageAttributes imageAttributes = new ImageAttributes())
				{
					imageAttributes.SetColorMatrix(colorMatrix);
					graphics.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imageAttributes);
				}
			}
			b.Dispose();
			b = bitmap;
		}

		public void CreateDrawItemReference()
		{
			this.DrawItemRef.Clear();
			foreach (ChipsData value in Global.cpd.Mapchip)
			{
				this.DrawItemRef.Add(value.character, value);
			}
			this.DrawWorldRef.Clear();
			foreach (ChipsData value2 in Global.cpd.Worldchip)
			{
				this.DrawWorldRef.Add(value2.character, value2);
			}
			if (Global.cpd.UseLayer)
			{
				this.DrawLayerRef.Clear();
				foreach (ChipsData value3 in Global.cpd.Layerchip)
				{
					this.DrawLayerRef.Add(value3.character, value3);
				}
			}
		}

		// 画像準備
		public void PrepareImages()
		{
			if (this.DrawChipOrig != null)
			{
				this.DrawChipOrig.Dispose();
				this.DrawChipOrig = null;
			}
			if (this.DrawMask != null)
			{
				this.DrawMask.Dispose();
				this.DrawMask = null;
			}
			if (this.DrawLayerOrig != null)
			{
				this.DrawLayerOrig.Dispose();
				this.DrawLayerOrig = null;
			}
			if (this.DrawLayerMask != null)
			{
				this.DrawLayerMask.Dispose();
				this.DrawLayerMask = null;
			}
			if (this.DrawOribossOrig != null)
			{
				this.DrawOribossOrig.Dispose();
				this.DrawOribossOrig = null;
			}
			if (this.DrawOribossMask != null)
			{
				this.DrawOribossMask.Dispose();
				this.DrawOribossMask = null;
			}
			if (this.DrawExOrig != null)
			{
				this.DrawExOrig.Dispose();
				this.DrawExOrig = null;
			}
			if (this.DrawExMask != null)
			{
				this.DrawExMask.Dispose();
				this.DrawExMask = null;
			}
			if (this.DrawHaikeiOrig != null)
			{
				this.DrawHaikeiOrig.Dispose();
				this.DrawHaikeiOrig = null;
			}
			if (this.DrawHaikei2Orig != null)
			{
				this.DrawHaikei2Orig.Dispose();
				this.DrawHaikei2Orig = null;
			}
			if (this.DrawHaikei3Orig != null)
			{
				this.DrawHaikei3Orig.Dispose();
				this.DrawHaikei3Orig = null;
			}
			if (this.DrawHaikei4Orig != null)
			{
				this.DrawHaikei4Orig.Dispose();
				this.DrawHaikei4Orig = null;
			}
			if (this.DrawSecondHaikeiOrig != null)
			{
				this.DrawSecondHaikeiOrig.Dispose();
				this.DrawSecondHaikeiOrig = null;
			}
			if (this.DrawSecondHaikei2Orig != null)
			{
				this.DrawSecondHaikei2Orig.Dispose();
				this.DrawSecondHaikei2Orig = null;
			}
			if (this.DrawSecondHaikei3Orig != null)
			{
				this.DrawSecondHaikei3Orig.Dispose();
				this.DrawSecondHaikei3Orig = null;
			}
			if (this.DrawSecondHaikei4Orig != null)
			{
				this.DrawSecondHaikei4Orig.Dispose();
				this.DrawSecondHaikei4Orig = null;
			}
			if (this.DrawChizuOrig != null)
			{
				this.DrawChizuOrig.Dispose();
				this.DrawChizuOrig = null;
			}
			string filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.PatternImage);
			FileStream fs;
			this.DrawChipOrig = Image.FromFile(filename);
			this.DrawMask = new Bitmap(this.DrawChipOrig.Width, this.DrawChipOrig.Height);
			ColorMap[] remapTable = new ColorMap[]
			{
				new ColorMap()
			};
			using (ImageAttributes imageAttributes = new ImageAttributes())
			{
				imageAttributes.SetRemapTable(remapTable);
				using (Graphics graphics = Graphics.FromImage(this.DrawMask))
				{
					graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawMask.Width, this.DrawMask.Height));
					graphics.DrawImage(this.DrawChipOrig, new Rectangle(0, 0, this.DrawMask.Width, this.DrawMask.Height), 0, 0, this.DrawMask.Width, this.DrawMask.Height, GraphicsUnit.Pixel, imageAttributes);
				}
			}

			if (Global.cpd.runtime.Definitions.LayerSize.bytesize != 0)
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.LayerImage);
				this.DrawLayerOrig = Image.FromFile(filename);
				this.DrawLayerMask = new Bitmap(this.DrawLayerOrig.Width, this.DrawLayerOrig.Height);
				using (ImageAttributes imageAttributes2 = new ImageAttributes())
				{
					imageAttributes2.SetRemapTable(remapTable);
					using (Graphics graphics2 = Graphics.FromImage(this.DrawLayerMask))
					{
						graphics2.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height));
						graphics2.DrawImage(this.DrawLayerOrig, new Rectangle(0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height), 0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height, GraphicsUnit.Pixel, imageAttributes2);
					}
				}
			}

			if (Global.cpd.project.Config.OribossImage != null)//オリジナルボスを使うとき
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.OribossImage);
				fs = File.OpenRead(filename);

				this.DrawOribossOrig = Image.FromStream(fs, false, false);
				this.DrawOribossMask = new Bitmap(this.DrawOribossOrig.Width, this.DrawOribossOrig.Height);
                using ImageAttributes imageAttributes4 = new ImageAttributes();
                imageAttributes4.SetRemapTable(remapTable);
                using Graphics graphics4 = Graphics.FromImage(this.DrawOribossMask);
                graphics4.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawOribossMask.Width, this.DrawOribossMask.Height));
                graphics4.DrawImage(this.DrawOribossOrig, new Rectangle(0, 0, this.DrawOribossMask.Width, this.DrawOribossMask.Height), 0, 0, this.DrawOribossMask.Width, this.DrawOribossMask.Height, GraphicsUnit.Pixel, imageAttributes4);
            }

			filename = Path.Combine(Global.cpd.where, Global.cpd.runtime.Definitions.ChipExtender);
			this.DrawExOrig = Image.FromFile(filename);
			this.DrawExMask = new Bitmap(this.DrawExOrig.Width, this.DrawExOrig.Height);
			using (ImageAttributes imageAttributes3 = new ImageAttributes())
			{
				imageAttributes3.SetRemapTable(remapTable);
				using (Graphics graphics3 = Graphics.FromImage(this.DrawExMask))
				{
					graphics3.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawExMask.Width, this.DrawExMask.Height));
					graphics3.DrawImage(this.DrawExOrig, new Rectangle(0, 0, this.DrawExMask.Width, this.DrawExMask.Height), 0, 0, this.DrawExMask.Width, this.DrawExMask.Height, GraphicsUnit.Pixel, imageAttributes3);
				}
			}
			using (Bitmap drawExMask = this.DrawExMask)
			{
				this.DrawExMask = DrawEx.MakeMask(drawExMask);
			}
			if (Global.cpd.project.Config.HaikeiImage != null)//ステージ1背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.HaikeiImage);
				fs = File.OpenRead(filename);

				this.DrawHaikeiOrig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.HaikeiImage2 != null)//ステージ2背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.HaikeiImage2);
				fs = File.OpenRead(filename);

				this.DrawHaikei2Orig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.HaikeiImage3 != null)//ステージ3背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.HaikeiImage3);
				fs = File.OpenRead(filename);

				this.DrawHaikei3Orig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.HaikeiImage4 != null)//ステージ4背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.HaikeiImage4);
				fs = File.OpenRead(filename);

				this.DrawHaikei4Orig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.SecondHaikeiImage != null)//ステージ1セカンド背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.SecondHaikeiImage);
				fs = File.OpenRead(filename);

				this.DrawSecondHaikeiOrig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.SecondHaikeiImage2 != null)//ステージ2セカンド背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.SecondHaikeiImage2);
				fs = File.OpenRead(filename);

				this.DrawSecondHaikei2Orig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.SecondHaikeiImage3 != null)//ステージ3セカンド背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.SecondHaikeiImage3);
				fs = File.OpenRead(filename);

				this.DrawSecondHaikei3Orig = Image.FromStream(fs, false, false);
			}
			if (Global.cpd.project.Config.SecondHaikeiImage4 != null)//ステージ4セカンド背景画像
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.SecondHaikeiImage4);
				fs = File.OpenRead(filename);

				this.DrawSecondHaikei4Orig = Image.FromStream(fs, false, false);
			}
			// 地図画面の背景
			filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.ChizuImage);
			fs = File.OpenRead(filename);

			this.DrawChizuOrig = Image.FromStream(fs, false, false);
		}

		protected unsafe override void OnPaint(PaintEventArgs e)
		{
			if (this.ForeLayerBmp == null)
			{
				return;
			}
			int num = (base.Width < (int)((double)this.ForeLayerBmp.Width * Global.config.draw.ZoomIndex)) ? base.Width : ((int)((double)this.ForeLayerBmp.Width * Global.config.draw.ZoomIndex));
			int num2 = (base.Height < (int)((double)this.ForeLayerBmp.Height * Global.config.draw.ZoomIndex)) ? base.Height : ((int)((double)this.ForeLayerBmp.Height * Global.config.draw.ZoomIndex));
			if (Global.config.draw.UseBufferingDraw && this.BufferingDraw && (this.bufpos != this.BufferCurrent || this.zi != Global.config.draw.ZoomIndex || this.DULayer != Global.state.DrawUnactiveLayer || this.TULayer != Global.state.TransparentUnactiveLayer || this.FLayer != Global.state.EditingForeground || this.EditMap != Global.state.EdittingStage))
			{
				this.bufpos = this.BufferCurrent;
				this.DULayer = Global.state.DrawUnactiveLayer;
				this.TULayer = Global.state.TransparentUnactiveLayer;
				this.FLayer = Global.state.EditingForeground;
				if (this.zi != Global.config.draw.ZoomIndex || this.EditMap != Global.state.EdittingStage)
				{
					this.zi = Global.config.draw.ZoomIndex;
					this.ForegroundBuffer.Dispose();
					this.ForegroundBuffer = new Bitmap((int)((double)this.ForeLayerBmp.Width * this.zi), (int)((double)this.ForeLayerBmp.Height * this.zi), PixelFormat.Format24bppRgb);
				}
				this.EditMap = Global.state.EdittingStage;
				using (Graphics graphics = Graphics.FromImage(this.ForegroundBuffer))
				{
					using (Brush brush = new SolidBrush(Global.state.Background))
					{
						graphics.FillRectangle(brush, new Rectangle(0, 0, this.ForegroundBuffer.Width, this.ForegroundBuffer.Height));
					}
					if (Global.config.draw.StageInterpolation)
					{
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					}
					else
					{
						graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					}
					if (Global.cpd.UseLayer && (!Global.state.EditingForeground || Global.state.DrawUnactiveLayer))
					{
						graphics.DrawImage(this.BackLayerBmp, new Rectangle(new Point(0, 0), this.ForegroundBuffer.Size), new Rectangle(0, 0, this.BackLayerBmp.Width, this.BackLayerBmp.Height), GraphicsUnit.Pixel);
					}
					if (!Global.cpd.UseLayer || Global.state.EditingForeground || Global.state.DrawUnactiveLayer)
					{
						graphics.DrawImage(this.ForeLayerBmp, new Rectangle(new Point(0, 0), this.ForegroundBuffer.Size), new Rectangle(0, 0, this.ForeLayerBmp.Width, this.ForeLayerBmp.Height), GraphicsUnit.Pixel);
					}
					if (Global.config.draw.UseBufferingMemoryDraw)
					{
						this.ForegroundBuffer.RotateFlip(RotateFlipType.Rotate180FlipX);
					}
				}
			}
			if (Global.config.draw.UseBufferingDraw && this.BufferingDraw)
			{
				if (Global.config.draw.UseBufferingMemoryDraw)
				{
					IntPtr hdc = e.Graphics.GetHdc();
					BitmapData bitmapData = new BitmapData();
					this.ForegroundBuffer.LockBits(new Rectangle(0, 0, this.ForegroundBuffer.Width, this.ForegroundBuffer.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb, bitmapData);
					Native.GDI32.BITMAPINFOHEADER bitmapinfoheader = default(Native.GDI32.BITMAPINFOHEADER);
					bitmapinfoheader.biSize = (uint)sizeof(Native.GDI32.BITMAPINFOHEADER);
					bitmapinfoheader.biWidth = bitmapData.Width;
					bitmapinfoheader.biHeight = bitmapData.Height;
					bitmapinfoheader.biPlanes = 1;
					bitmapinfoheader.biBitCount = 24;
					int ysrc = (Global.state.MapMoveMax.Height > 0) ? (Global.state.MapMoveMax.Height - Global.state.MapPoint.Y) : 0;
					Native.GDI32.StretchDIBits(hdc, 0, 0, num, num2, Global.state.MapPoint.X, ysrc, num, num2, bitmapData.Scan0, (IntPtr)((void*)(&bitmapinfoheader)), 0U, 13369376U);
					this.ForegroundBuffer.UnlockBits(bitmapData);
					e.Graphics.ReleaseHdc(hdc);
				}
				else
				{
					e.Graphics.DrawImage(this.ForegroundBuffer, new Rectangle(0, 0, num, num2), new Rectangle(Global.state.MapPoint, new Size(num, num2)), GraphicsUnit.Pixel);
				}
			}
			else
			{
				using (Brush brush2 = new SolidBrush(Global.state.Background))
				{
					e.Graphics.FillRectangle(brush2, new Rectangle(0, 0, num, num2));
				}
				if (Global.config.draw.StageInterpolation)
				{
					e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				}
				else
				{
					e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				}
				double num3 = 1.0 / Global.config.draw.ZoomIndex;
				if (Global.cpd.UseLayer && (!Global.state.EditingForeground || Global.state.DrawUnactiveLayer))
				{
					e.Graphics.DrawImage(this.BackLayerBmp, new Rectangle(0, 0, num, num2), new Rectangle(Global.state.MapPointTranslated, new Size((int)((double)num * num3), (int)((double)num2 * num3))), GraphicsUnit.Pixel);
				}
				if (!Global.cpd.UseLayer || Global.state.EditingForeground || Global.state.DrawUnactiveLayer)
				{
					e.Graphics.DrawImage(this.ForeLayerBmp, new Rectangle(0, 0, num, num2), new Rectangle(Global.state.MapPointTranslated, new Size((int)((double)num * num3), (int)((double)num2 * num3))), GraphicsUnit.Pixel);
				}
			}
			if (this.DrawMode != GUIDesigner.DirectDrawMode.None)
			{
				Rectangle drawRectangle = this.DrawRectangle;
				drawRectangle.X *= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
				drawRectangle.Y *= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
				drawRectangle.Width *= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
				drawRectangle.Height *= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
				drawRectangle.X -= Global.state.MapPoint.X;
				drawRectangle.Y -= Global.state.MapPoint.Y;
				using (Brush brush3 = new SolidBrush(Color.FromArgb(Global.config.draw.AlphaBlending ? 160 : 255, DrawEx.GetForegroundColor(Global.state.Background))))
				{
					if (this.DrawMode == GUIDesigner.DirectDrawMode.Rectangle)
					{
						e.Graphics.FillRectangle(brush3, drawRectangle);
					}
					else
					{
						using (Pen pen = new Pen(brush3, (float)((int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex) / 4)))
						{
							drawRectangle.X += (int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex) / 2;
							drawRectangle.Y += (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex) / 2;
							drawRectangle.Width -= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
							drawRectangle.Height -= (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
							if (this.DrawMode == GUIDesigner.DirectDrawMode.RevLine)
							{
								e.Graphics.DrawLine(pen, new Point(drawRectangle.X, drawRectangle.Bottom), new Point(drawRectangle.Right, drawRectangle.Top));
							}
							else
							{
								e.Graphics.DrawLine(pen, drawRectangle.Location, new Point(drawRectangle.Right, drawRectangle.Bottom));
							}
						}
					}
				}
			}
			if (Global.config.draw.DrawGrid)
			{ // グリッドを表示

				// 横方向のグリッド
				int num4 = (int)((double)(Global.state.MapPointTranslated.X % Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
				// 縦方向のグリッド
				int num5 = (int)((double)(Global.state.MapPointTranslated.Y % Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);

				DrawEx.DrawGridEx(e.Graphics, new Rectangle(0, (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex) - num5, num, num2), new Size(Global.definition.GridInterval, (int)((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex)), Global.state.Background);

				DrawEx.DrawGridEx(e.Graphics, new Rectangle((int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex) - num4, 0, num, num2), new Size((int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex), Global.definition.GridInterval), Global.state.Background);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}

		void IDisposable.Dispose()
		{
			if (this.DrawMask != null)
			{
				this.DrawMask.Dispose();
				this.DrawMask = null;
			}
			if (this.DrawChipOrig != null)
			{
				this.DrawChipOrig.Dispose();
				this.DrawChipOrig = null;
			}
			if (this.DrawLayerMask != null)
			{
				this.DrawLayerMask.Dispose();
				this.DrawLayerMask = null;
			}
			if (this.DrawLayerOrig != null)
			{
				this.DrawLayerOrig.Dispose();
				this.DrawLayerOrig = null;
			}
			if (this.ForeLayerBmp != null)
			{
				this.ForeLayerBmp.Dispose();
				this.ForeLayerBmp = null;
			}
			if (this.BackLayerBmp != null)
			{
				this.BackLayerBmp.Dispose();
				this.BackLayerBmp = null;
			}
			if (this.DrawOribossMask != null)
			{
				this.DrawOribossMask.Dispose();
				this.DrawOribossMask = null;
			}
			if (this.DrawOribossOrig != null)
			{
				this.DrawOribossOrig.Dispose();
				this.DrawOribossOrig = null;
			}
			if (this.DrawExOrig != null)
			{
				this.DrawExOrig.Dispose();
				this.DrawExOrig = null;
			}
			if (this.DrawExMask != null)
			{
				this.DrawExMask.Dispose();
				this.DrawExMask = null;
			}
			if (this.DrawHaikeiOrig != null)
			{
				this.DrawHaikeiOrig.Dispose();
				this.DrawHaikeiOrig = null;
			}
			if (this.DrawHaikei2Orig != null)
			{
				this.DrawHaikei2Orig.Dispose();
				this.DrawHaikei2Orig = null;
			}
			if (this.DrawHaikei3Orig != null)
			{
				this.DrawHaikei3Orig.Dispose();
				this.DrawHaikei3Orig = null;
			}
			if (this.DrawHaikei4Orig != null)
			{
				this.DrawHaikei4Orig.Dispose();
				this.DrawHaikei4Orig = null;
			}
			if (this.DrawSecondHaikeiOrig != null)
			{
				this.DrawSecondHaikeiOrig.Dispose();
				this.DrawSecondHaikeiOrig = null;
			}
			if (this.DrawSecondHaikei2Orig != null)
			{
				this.DrawSecondHaikei2Orig.Dispose();
				this.DrawSecondHaikei2Orig = null;
			}
			if (this.DrawSecondHaikei3Orig != null)
			{
				this.DrawSecondHaikei3Orig.Dispose();
				this.DrawSecondHaikei3Orig = null;
			}
			if (this.DrawSecondHaikei4Orig != null)
			{
				this.DrawSecondHaikei4Orig.Dispose();
				this.DrawSecondHaikei4Orig = null;
			}
			if (this.DrawChizuOrig != null)
			{
				this.DrawChizuOrig.Dispose();
				this.DrawChizuOrig = null;
			}
		}

		private void GUIDesigner_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				if (e.Button == MouseButtons.Right)
				{ // 右クリック時
					if (this.CopyPaste == GUIDesigner.CopyPasteTool.Paste)
					{
						Global.MainWnd.CopyPasteInit();
						this.Refresh();
						return;
					}
					if (this.MousePressed)
					{
						this.MousePressed = false;
						switch (this.CopyPaste)
						{
						case GUIDesigner.CopyPasteTool.Copy:
						case GUIDesigner.CopyPasteTool.Cut:
							Global.MainWnd.CopyPasteInit();
							this.Refresh();
							return;
						default:
							switch (this.CurrentTool)
							{
							case GUIDesigner.EditTool.Line:
								this.EnsureScroll(e.X, e.Y);
								this.DrawMode = GUIDesigner.DirectDrawMode.None;
								this.Refresh();
								return;
							case GUIDesigner.EditTool.Rect:
								this.EnsureScroll(e.X, e.Y);
								this.DrawMode = GUIDesigner.DirectDrawMode.None;
								this.Refresh();
								return;
							default:
								return;
							}
							break;
						}
					}
					else
					{
						if (this.CurrentTool == GUIDesigner.EditTool.Cursor && Global.config.draw.RightClickMenu)
						{
							this.MouseStartPoint = default(Point);
							this.MouseStartPoint.X = (int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex));
							this.MouseStartPoint.Y = (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex));
							this.CursorContextMenu.Show(this, new Point(e.X, e.Y));
							return;
						}

						// チップを拾う
						string stageChar = GUIDesigner.StageText.GetStageChar(new Point
						{
							X = (int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)),
							Y = (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex))
						});
						if (Global.state.MapEditMode)
						{
							if (this.DrawItemRef.ContainsKey(stageChar))
							{
								Global.state.CurrentChip = this.DrawWorldRef[stageChar];
								return;
							}
						}
						else if (Global.state.EditingForeground)
						{
							if (this.DrawItemRef.ContainsKey(stageChar))
							{
								Global.state.CurrentChip = this.DrawItemRef[stageChar];
								return;
							}
						}
						else if (this.DrawLayerRef.ContainsKey(stageChar))
						{
							Global.state.CurrentChip = this.DrawLayerRef[stageChar];
							return;
						}
					}
				}
				else if (e.Button == MouseButtons.Middle && Global.config.testRun.QuickTestrun)
				{ // 中央クリック時
					Point p = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
					if (GUIDesigner.StageText.IsOverflow(p))
					{
						return;
					}
					List<string> list = new List<string>();
					foreach (string text in Global.cpd.EditingMap)
					{
						list.Add(text.Replace(Global.cpd.Mapchip[1].character, Global.cpd.Mapchip[0].character));
					}
					int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : Global.cpd.runtime.Definitions.StageSize.bytesize;
					list[p.Y] = list[p.Y].Remove(p.X * num, num);
					list[p.Y] = list[p.Y].Insert(p.X * num, Global.cpd.Mapchip[1].character);
					Global.state.QuickTestrunSource = list.ToArray();
					Global.MainWnd.Testrun();
				}
				return;
			}
			this.MousePressed = true;
			switch (this.CopyPaste)
			{
			case GUIDesigner.CopyPasteTool.Copy:
			case GUIDesigner.CopyPasteTool.Cut:
				this.MouseStartPoint = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
				this.MouseLastPoint = this.MouseStartPoint;
				return;
			case GUIDesigner.CopyPasteTool.Paste:
			{
				this.EnsureScroll(e.X, e.Y);
				Global.MainWnd.UpdateStatus("描画しています...");
				Rectangle rectangle = new Rectangle(this.MouseLastPoint, this.GetBufferSize());
				string[] array = this.ClipedString.Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.None);
				ChipsData cd = default(ChipsData);
				Point point = new Point(0, 0);
				for (int j = rectangle.Top; j < rectangle.Bottom; j++)
				{
					point.X = 0;
					char[] array2 = this.PutItemTextStart(j);
					if (array2 != null)
					{
						for (int k = rectangle.Left; k < rectangle.Right; k++)
						{
							cd.character = array[point.Y].Substring(point.X * Global.state.GetCByte, Global.state.GetCByte);
							this.PutItemText(ref array2, new Point(k, j), cd);
							point.X++;
						}
						this.PutItemTextEnd(array2, j);
						point.Y++;
					}
				}
				Global.MainWnd.CopyPasteInit();
				this.StageSourceToDrawBuffer();
				this.AddBuffer();
				this.MousePressed = false;
				this.DrawMode = GUIDesigner.DirectDrawMode.None;
				Global.MainWnd.UpdateStatus("完了");
				this.Refresh();
				return;
			}
			default:
				switch (this.CurrentTool)
				{
				case GUIDesigner.EditTool.Cursor:
					this.MouseStartPoint = new Point(e.X, e.Y);
					return;
				case GUIDesigner.EditTool.Pen:
					this.MouseLastPoint = new Point(-1, -1);
					this.GUIDesigner_MouseMove(this, e);
					return;
				case GUIDesigner.EditTool.Line:
				case GUIDesigner.EditTool.Rect:
					this.MouseStartPoint = new Point(
						(int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)),
						(int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex))
					);
					this.MouseLastPoint = this.MouseStartPoint;
					return;
				case GUIDesigner.EditTool.Fill:
					Global.MainWnd.UpdateStatus("塗り潰しています...");
					this.FillStart(Global.state.CurrentChip,
						new Point(
							(int)(
								(double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)
							),
							(int)(
								(double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)
							)
						)
					);
					this.StageSourceToDrawBuffer();
					this.AddBuffer();
					this.Refresh();
					Global.MainWnd.UpdateStatus("完了");
					return;
				default:
					return;
				}
				break;
			}
		}

		// 塗りつぶすチップデータ、塗りつぶし開始座標
		private void FillStart(ChipsData repl, Point pt)
		{
			// 画面外なら終了
			if (GUIDesigner.StageText.IsOverflow(pt))
			{
				return;
			}
			this.repls.Clear();
			if (Global.state.EditingForeground)
			{
				int num = 0;
				while (Global.state.MapEditMode ? (num < Global.cpd.project.Runtime.Definitions.MapSize.y) : (num < Global.cpd.runtime.Definitions.StageSize.y))
				{
					char[] array = this.PutItemTextStart(num);
					if (array != null)
					{
						this.repls.Add(array);
					}
					num++;
				}
			}
			else
			{
				for (int i = 0; i < Global.cpd.runtime.Definitions.LayerSize.y; i++)
				{
					char[] array2 = this.PutItemTextStart(i);
					if (array2 != null)
					{
						this.repls.Add(array2);
					}
				}
			}
			string stageChar = GUIDesigner.StageText.GetStageChar(pt);
            // 塗りつぶしたマスと同じなら終了
            if (stageChar == repl.character)
			{
				return;
			}
			if (Global.state.MapEditMode)
			{
				if (this.DrawWorldRef.ContainsKey(stageChar) && this.CheckChar(pt, this.DrawWorldRef[stageChar]))
				{
					this.FillThis(this.DrawWorldRef[stageChar], repl, pt);
				}
			}
			else if (Global.state.EditingForeground)
			{
				if (this.DrawItemRef.ContainsKey(stageChar) && this.CheckChar(pt, this.DrawItemRef[stageChar]))
				{
					this.FillThis(this.DrawItemRef[stageChar], repl, pt);
				}
			}
			else if (this.DrawLayerRef.ContainsKey(stageChar) && this.CheckChar(pt, this.DrawLayerRef[stageChar]))
			{
				this.FillThis(this.DrawLayerRef[stageChar], repl, pt);
			}

			for (int j = 0; j < Global.state.GetCSSize.y; j++)
			{
				this.PutItemTextEnd(this.repls[j], j);
			}
		}

        // 塗りつぶす前のチップデータ、塗りつぶすチップデータ、塗りつぶし開始座標
        private void FillThis(ChipsData old, ChipsData repl, Point pt)
        {
			var queue = new Queue<BufStr>();

            queue.Enqueue(new BufStr(
				this.scanLeft(pt, this.repls, old),
				this.scanRight(pt, this.repls, old),
                pt.Y
			));

            var newmap = new List<char[]>(this.repls);

            while (queue.Count > 0) {
				int left = queue.Peek().left;
				int right = queue.Peek().right;
				int y = queue.Peek().y;
				queue.Dequeue();

                this.updateLine(left, right, y, repl);

				// 上下を探索
				if (0 < y)
				{
					this.searchLine(left, right, y - 1, newmap, old, queue);
				}
				if (y < Global.state.GetCSSize.y - 1)
				{
					this.searchLine(left, right, y + 1, newmap, old, queue);
				}
			}
        }

		private int scanLeft(Point pt, List<char[]> newmap, ChipsData old)
		{
			int result = pt.X;

            while (0 < result && this.getMapChipString(result - 1, pt.Y, newmap).Equals(old.character))
            {
                result--;
            }
            return result;
		}

		private int scanRight(Point pt, List<char[]> newmap, ChipsData old)
		{
			int result = pt.X;

            while (result < Global.state.GetCSSize.x - 1 && this.getMapChipString(result + 1, pt.Y, newmap).Equals(old.character))
            {
                result++;
            }
            return result;
		}

		private void updateLine(int left, int right, int y, ChipsData repl)
		{
			//マップの文字を書き換える
			for (int x = left; x <= right; x++)
			{
				for (int i = 0; i < Global.state.GetCByte; i++)
				{
					this.repls[y][x * Global.state.GetCByte + i] = repl.character[i];
				}
			}
		}

		private void searchLine(int left, int right, int y, List<char[]> newmap, ChipsData old, Queue<BufStr> queue)
		{
			int l = -1;
			for (int x = left; x <= right; x++)
			{
				var c = this.getMapChipString(x, y, newmap);

				if (c.Equals(old.character) && l == -1)
				{
					if (x == left)
					{
						l = this.scanLeft(new Point(x, y), newmap, old);
					}
					else
					{
						l = x;
					}
				}
				else if (!c.Equals(old.character) && l != -1)
				{
					int r = x - 1;
					queue.Enqueue(new BufStr(l, r, y));
					l = -1;
				}
			}
			if (l != -1)
			{
				int r = this.scanRight(new Point(right, y), newmap, old);
				queue.Enqueue(new BufStr(l, r, y));
			}
		}

		private bool CheckChar(Point pt, ChipsData cd)
		{
			if (GUIDesigner.StageText.IsOverflow(pt))
			{
				return false;
			}
			for (int i = 0; i < Global.state.GetCByte; i++)
			{
				if (this.repls[pt.Y][pt.X * Global.state.GetCByte + i] != cd.character[i])
				{
					return false;
				}
			}
			return true;
        }

		// マップ文字列から特定の座標の文字(String型)を取り出す。通常は1文字。レイヤーは2文字。
        private String getMapChipString(int x, int y, List<char[]> newmap)
        {
            var c = new char[Global.state.GetCByte];

            for (int i = 0; i < Global.state.GetCByte; i++)
            {
                c[i] = newmap[y][x * Global.state.GetCByte + i];
            }

            return new String(c);
        }

        private void GUIDesigner_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.CopyPaste != GUIDesigner.CopyPasteTool.Paste || this.MousePressed)
			{
				if (this.MousePressed)
				{
					switch (this.CopyPaste)
					{
					case GUIDesigner.CopyPasteTool.Copy:
					case GUIDesigner.CopyPasteTool.Cut:
					{
						this.EnsureScroll(e.X, e.Y);
						Point point = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
						if (point == this.MouseLastPoint)
						{
							return;
						}
						Rectangle drawRectangle = default(Rectangle);
						this.MouseLastPoint = point;
						if (this.MouseStartPoint.X > this.MouseLastPoint.X)
						{
							drawRectangle.X = this.MouseLastPoint.X;
							drawRectangle.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
						}
						else
						{
							drawRectangle.X = this.MouseStartPoint.X;
							drawRectangle.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
						}
						if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
						{
							drawRectangle.Y = this.MouseLastPoint.Y;
							drawRectangle.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
						}
						else
						{
							drawRectangle.Y = this.MouseStartPoint.Y;
							drawRectangle.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
						}
						drawRectangle.Width++;
						drawRectangle.Height++;
						this.DrawRectangle = drawRectangle;
						this.DrawMode = GUIDesigner.DirectDrawMode.Rectangle;
						this.Refresh();
						return;
					}
					default:
						switch (this.CurrentTool)
						{
						case GUIDesigner.EditTool.Cursor:
						{
							Size size = new Size(this.MouseStartPoint.X - e.X, this.MouseStartPoint.Y - e.Y);
							this.MouseStartPoint = new Point(e.X, e.Y);
							State state = Global.state;
							state.MapPoint.X = state.MapPoint.X + size.Width;
							State state2 = Global.state;
							state2.MapPoint.Y = state2.MapPoint.Y + size.Height;
							Global.state.AdjustMapPoint();
							this.Refresh();
							Global.MainWnd.CommitScrollbar();
							return;
						}
						case GUIDesigner.EditTool.Pen:
						{
							Point point2 = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
							if (point2 == this.MouseLastPoint)
							{
								return;
							}
							this.PutItem(new Point
							{
								X = (int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)),
								Y = (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex))
							});
							this.MouseLastPoint = point2;
							return;
						}
						case GUIDesigner.EditTool.Line:
						{
							this.EnsureScroll(e.X, e.Y);
							Point point3 = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
							if (point3 == this.MouseLastPoint)
							{
								return;
							}
							Rectangle drawRectangle2 = default(Rectangle);
							this.MouseLastPoint = point3;
							bool flag = false;
							if (this.MouseStartPoint.X > this.MouseLastPoint.X)
							{
								drawRectangle2.X = this.MouseLastPoint.X;
								drawRectangle2.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
								flag = true;
							}
							else
							{
								drawRectangle2.X = this.MouseStartPoint.X;
								drawRectangle2.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
							}
							if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
							{
								drawRectangle2.Y = this.MouseLastPoint.Y;
								drawRectangle2.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
								flag = !flag;
							}
							else
							{
								drawRectangle2.Y = this.MouseStartPoint.Y;
								drawRectangle2.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
							}
							drawRectangle2.Width++;
							drawRectangle2.Height++;
							this.DrawRectangle = drawRectangle2;
							if (flag)
							{
								this.DrawMode = GUIDesigner.DirectDrawMode.RevLine;
							}
							else
							{
								this.DrawMode = GUIDesigner.DirectDrawMode.Line;
							}
							this.Refresh();
							return;
						}
						case GUIDesigner.EditTool.Rect:
						{
							this.EnsureScroll(e.X, e.Y);
							Point point4 = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
							if (point4 == this.MouseLastPoint)
							{
								return;
							}
							Rectangle drawRectangle3 = default(Rectangle);
							this.MouseLastPoint = point4;
							if (this.MouseStartPoint.X > this.MouseLastPoint.X)
							{
								drawRectangle3.X = this.MouseLastPoint.X;
								drawRectangle3.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
							}
							else
							{
								drawRectangle3.X = this.MouseStartPoint.X;
								drawRectangle3.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
							}
							if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
							{
								drawRectangle3.Y = this.MouseLastPoint.Y;
								drawRectangle3.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
							}
							else
							{
								drawRectangle3.Y = this.MouseStartPoint.Y;
								drawRectangle3.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
							}
							drawRectangle3.Width++;
							drawRectangle3.Height++;
							this.DrawRectangle = drawRectangle3;
							this.DrawMode = GUIDesigner.DirectDrawMode.Rectangle;
							this.Refresh();
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
			this.EnsureScroll(e.X, e.Y);
			Point point5 = new Point((int)((double)(e.X + Global.state.MapPoint.X) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)), (int)((double)(e.Y + Global.state.MapPoint.Y) / ((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex)));
			if (point5 == this.MouseLastPoint)
			{
				return;
			}
			this.GetBufferSize();
			int width = Global.cpd.runtime.Definitions.ChipSize.Width;
			double zoomIndex = Global.config.draw.ZoomIndex;
			double num = 1.0 / Global.config.draw.ZoomIndex;
			this.MouseLastPoint = point5;
			Rectangle drawRectangle4 = new Rectangle(this.MouseLastPoint, this.GetBufferSize());
			this.DrawRectangle = drawRectangle4;
			this.DrawMode = GUIDesigner.DirectDrawMode.Rectangle;
			this.Refresh();
		}

		public bool CheckBuffer()
		{
			if (this.ClipedString == null || this.ClipedString.Length == 0)
			{
				return false;
			}
			int num = -1;
			foreach (string text in this.ClipedString.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.None))
			{
				if (num != -1 && text.Length != num)
				{
					return false;
				}
				num = text.Length;
			}
			return num != 0 && num < Global.state.GetCByteWidth;
		}

		private Size GetBufferSize()
		{
			string[] array = this.ClipedString.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.None);
			return new Size(array[0].Length / Global.state.GetCByte, array.Length);
		}

		public void EnsureScroll(int x, int y)
		{
			Size chipSize = Global.cpd.runtime.Definitions.ChipSize;
			Point point = default(Point);
			if (x < base.Width / 2)
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
				if (base.Width - x < chipSize.Width)
				{
					point.X++;
				}
				if (base.Width - x < chipSize.Width / 2)
				{
					point.X += 2;
				}
			}
			if (y < base.Height / 2)
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
				if (base.Height - y < chipSize.Height)
				{
					point.Y++;
				}
				if (base.Height - y < chipSize.Height / 2)
				{
					point.Y += 2;
				}
			}
			State state = Global.state;
			state.MapPoint.X = state.MapPoint.X + point.X * chipSize.Width;
			State state2 = Global.state;
			state2.MapPoint.Y = state2.MapPoint.Y + point.Y * chipSize.Height;
			Global.state.AdjustMapPoint();
			this.Refresh();
			Global.MainWnd.CommitScrollbar();
		}

		private void GUIDesigner_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.MousePressed)
			{
				this.MousePressed = false;
				switch (this.CopyPaste)
				{
				case GUIDesigner.CopyPasteTool.Copy:
				case GUIDesigner.CopyPasteTool.Cut:
				{
					this.EnsureScroll(e.X, e.Y);
					int width = Global.cpd.runtime.Definitions.ChipSize.Width;
					double zoomIndex = Global.config.draw.ZoomIndex;
					double num = 1.0 / Global.config.draw.ZoomIndex;
					using (Graphics.FromImage(this.ForeLayerBmp))
					{
						Rectangle rectangle = default(Rectangle);
						if (this.MouseStartPoint.X > this.MouseLastPoint.X)
						{
							rectangle.X = this.MouseLastPoint.X;
							rectangle.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
						}
						else
						{
							rectangle.X = this.MouseStartPoint.X;
							rectangle.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
						}
						if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
						{
							rectangle.Y = this.MouseLastPoint.Y;
							rectangle.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
						}
						else
						{
							rectangle.Y = this.MouseStartPoint.Y;
							rectangle.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
						}
						StringBuilder stringBuilder = new StringBuilder();
						try
						{
							for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
							{
								if (Global.state.EditingForeground)
								{
									stringBuilder.Append(Global.cpd.EditingMap[i].Substring(rectangle.Left * Global.state.GetCByte, (rectangle.Width + 1) * Global.state.GetCByte));
								}
								else
								{
									stringBuilder.Append(Global.cpd.EditingLayer[i].Substring(rectangle.Left * Global.state.GetCByte, (rectangle.Width + 1) * Global.state.GetCByte));
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
							this.Refresh();
							Global.MainWnd.UpdateStatus("選択範囲が異常です。処理を実行できません");
							break;
						}
						Clipboard.SetText(stringBuilder.ToString());
						if (this.CopyPaste == GUIDesigner.CopyPasteTool.Cut)
						{
							Global.MainWnd.UpdateStatus("切り取りしています...");
							for (int j = rectangle.Top; j <= rectangle.Bottom; j++)
							{
								char[] array = this.PutItemTextStart(j);
								if (array != null)
								{
									for (int k = rectangle.Left; k <= rectangle.Right; k++)
									{
										if (Global.state.EditingForeground)
										{
											this.PutItemText(ref array, new Point(k, j), Global.cpd.Mapchip[0]);
										}
										else
										{
											this.PutItemText(ref array, new Point(k, j), Global.cpd.Layerchip[0]);
										}
									}
									this.PutItemTextEnd(array, j);
								}
							}
							this.StageSourceToDrawBuffer();
							this.AddBuffer();
							this.DrawMode = GUIDesigner.DirectDrawMode.None;
						}
						else
						{
							this.DrawMode = GUIDesigner.DirectDrawMode.None;
						}
					}
					Global.MainWnd.CopyPasteInit();
					Global.MainWnd.UpdateStatus("完了");
					this.Refresh();
					return;
				}
				case GUIDesigner.CopyPasteTool.Paste:
					break;
				default:
					switch (this.CurrentTool)
					{
					case GUIDesigner.EditTool.Pen:
						this.GUIDesigner_MouseMove(sender, e);
						this.AddBuffer();
						if (Global.state.EditingForeground)
						{
							this.ForePrevDrawn = (string[])Global.cpd.EditingMap.Clone();
						}
						else
						{
							this.BackPrevDrawn = (string[])Global.cpd.EditingLayer.Clone();
						}
						Global.MainWnd.UpdateStatus("完了");
						return;
					case GUIDesigner.EditTool.Line:
					{
						this.DrawMode = GUIDesigner.DirectDrawMode.None;
						Rectangle dr = default(Rectangle);
						int num2 = 0;
						if (this.MouseStartPoint.X > this.MouseLastPoint.X)
						{
							dr.X = this.MouseLastPoint.X;
							dr.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
							num2++;
						}
						else
						{
							dr.X = this.MouseStartPoint.X;
							dr.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
							num2--;
						}
						if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
						{
							dr.Y = this.MouseLastPoint.Y;
							dr.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
							num2++;
						}
						else
						{
							dr.Y = this.MouseStartPoint.Y;
							dr.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
							num2--;
						}
						Global.MainWnd.UpdateStatus("線を描画しています...");
						if (Global.state.EditingForeground)
						{
							using (Graphics graphics2 = Graphics.FromImage(this.ForeLayerBmp))
							{
								this.DrawLine(graphics2, dr, num2 != 0);
								goto IL_5AE;
							}
						}
						using (Graphics graphics3 = Graphics.FromImage(this.BackLayerBmp))
						{
							this.DrawLine(graphics3, dr, num2 != 0);
						}
						IL_5AE:
						this.AddBuffer();
						Global.MainWnd.UpdateStatus("完了");
						this.Refresh();
						return;
					}
					case GUIDesigner.EditTool.Rect:
					{
						this.EnsureScroll(e.X, e.Y);
						Global.MainWnd.UpdateStatus("描画しています...");
						Rectangle rectangle2 = default(Rectangle);
						if (this.MouseStartPoint.X > this.MouseLastPoint.X)
						{
							rectangle2.X = this.MouseLastPoint.X;
							rectangle2.Width = this.MouseStartPoint.X - this.MouseLastPoint.X;
						}
						else
						{
							rectangle2.X = this.MouseStartPoint.X;
							rectangle2.Width = this.MouseLastPoint.X - this.MouseStartPoint.X;
						}
						if (this.MouseStartPoint.Y > this.MouseLastPoint.Y)
						{
							rectangle2.Y = this.MouseLastPoint.Y;
							rectangle2.Height = this.MouseStartPoint.Y - this.MouseLastPoint.Y;
						}
						else
						{
							rectangle2.Y = this.MouseStartPoint.Y;
							rectangle2.Height = this.MouseLastPoint.Y - this.MouseStartPoint.Y;
						}
						for (int l = rectangle2.Top; l <= rectangle2.Bottom; l++)
						{
							char[] array2 = this.PutItemTextStart(l);
							if (array2 != null)
							{
								for (int m = rectangle2.Left; m <= rectangle2.Right; m++)
								{
									this.PutItemText(ref array2, new Point(m, l), Global.state.CurrentChip);
								}
								this.PutItemTextEnd(array2, l);
							}
						}
						this.StageSourceToDrawBuffer();
						this.AddBuffer();
						this.DrawMode = GUIDesigner.DirectDrawMode.None;
						Global.MainWnd.UpdateStatus("完了");
						this.Refresh();
						return;
					}
					default:
						this.GUIDesigner_MouseMove(sender, e);
						break;
					}
					break;
				}
			}
		}

		public void DrawLine(Graphics g, Rectangle dr, bool L2R)
		{
			Point point = new Point(0, 0);
			PointF pointF = new PointF(0f, 0f);
			if (dr.Height > dr.Width)
			{
				point.Y = 1;
				if (L2R)
				{
					pointF.X = (float)dr.Width / (float)dr.Height;
				}
				else
				{
					pointF.X = (float)dr.Width / (float)dr.Height * -1f;
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
				pointF.Y = (float)dr.Height / (float)dr.Width;
			}
			double num = 0.0;
			double num2 = 0.0;
			Point point2 = new Point(L2R ? dr.Left : dr.Right, dr.Top);
			Point mapPos = point2;
			while (Math.Abs((int)Math.Round(num)) <= dr.Width && (int)Math.Round(num2) <= dr.Height)
			{
				this.PutItem(g, mapPos, Global.state.CurrentChip);
				num += (double)point.X + (double)pointF.X;
				num2 += (double)point.Y + (double)pointF.Y;
				mapPos.X = point2.X + (int)Math.Round(num);
				mapPos.Y = point2.Y + (int)Math.Round(num2);
			}
			if (Global.state.EditingForeground)
			{
				this.ForePrevDrawn = (string[])Global.cpd.EditingMap.Clone();
			}
			else
			{
				this.BackPrevDrawn = (string[])Global.cpd.EditingLayer.Clone();
			}
			this.Refresh();
		}

		public void PutItem(Point MapPos)
		{
			this.PutItem(MapPos, Global.state.CurrentChip);
		}

		public void PutItem(Point MapPos, ChipsData cd)
		{
			Graphics graphics = null;
			try
			{
				if (Global.state.EditingForeground)
				{
					graphics = Graphics.FromImage(this.ForeLayerBmp);
				}
				else
				{
					graphics = Graphics.FromImage(this.BackLayerBmp);
				}
				this.PutItem(graphics, MapPos, cd);
			}
			finally
			{
				graphics.Dispose();
			}
			this.Refresh();
		}

		public void PutItemText(ref char[] ca, Point MapPos, ChipsData cd)
		{
			if (GUIDesigner.StageText.IsOverflow(MapPos))
			{
				return;
			}
			for (int i = 0; i < Global.state.GetCByte; i++)
			{
				ca[MapPos.X * Global.state.GetCByte + i] = cd.character[i];
			}
		}

		public char[] PutItemTextStart(int Y)
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

		public void PutItemTextEnd(char[] item, int Y)
		{
			if (Global.state.EditingForeground)
			{
				Global.cpd.EditingMap[Y] = new string(item);
				return;
			}
			Global.cpd.EditingLayer[Y] = new string(item);
		}

		public Point GetLargerPoint(Point fst, Point snd)
		{
			return new Point
			{
				X = ((fst.X > snd.X) ? fst.X : snd.X),
				Y = ((fst.Y > snd.Y) ? fst.Y : snd.Y)
			};
		}

		public Size GetLargerSize(Size fst, Size snd)
		{
			return new Size
			{
				Width = ((fst.Width > snd.Width) ? fst.Width : snd.Width),
				Height = ((fst.Height > snd.Height) ? fst.Height : snd.Height)
			};
		}

		// チップを置いたとき
		public void PutItem(Graphics g, Point MapPos, ChipsData cd)
		{
			if (GUIDesigner.StageText.IsOverflow(MapPos)) return;
			string stageChar = GUIDesigner.StageText.GetStageChar(MapPos);
			if (cd.character.Equals(stageChar)) return;
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
			Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
			if (Global.state.EditingForeground)
			{ // 標準レイヤー
				Size size = default(Size);
				if (cd.character != null)
				{
					if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && cd.character == "Z")
					{
						size = this.DrawOribossOrig.Size;
					}
					else {
						size = (cd.GetCSChip().view_size != default) ? cd.GetCSChip().view_size : cd.GetCSChip().size; // 置きたいチップデータのサイズ

						if (cd.GetCSChip().name.Contains("曲線による") && cd.GetCSChip().name.Contains("坂") &&
							128 + MapPos.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
								size.Height += Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (128 + MapPos.Y * chipsize.Height);
						else if(cd.GetCSChip().name == "半円" && !cd.GetCSChip().description.Contains("乗れる") &&
							64 + MapPos.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
							size.Height += Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (64 + MapPos.Y * chipsize.Height);
					}
				}
				ChipsData chipsData = default; // 置く前から元々あったチップデータのサイズ
				ChipData chipData = default(ChipData);
				if (this.DrawItemRef.ContainsKey(stageChar))
				{
					if (Global.state.MapEditMode) chipsData = this.DrawWorldRef[stageChar];
					else chipsData = this.DrawItemRef[stageChar];
					chipData = chipsData.GetCSChip();
					if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
						size = this.GetLargerSize(size, this.DrawOribossOrig.Size);
					else if (chipData.name.Contains("曲線による") && chipData.name.Contains("坂") || chipData.name == "半円" && !chipData.description.Contains("乗れる"))
						size = this.GetLargerSize(size, new Size(chipData.view_size.Width, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height));
					else size = this.GetLargerSize(size, (chipData.view_size != default)?chipData.view_size:chipData.size);	//サイズを比較して、大きい方に合わせる
				}
				if (size == default(Size)) this.RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
				else
				{
					Point largerPoint = this.GetLargerPoint(cd.GetCSChip().center, chipData.center);
					if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3)
					{
						if (cd.character == "Z") largerPoint = this.GetLargerPoint(new Point(0, 0), chipData.center);
						else if (chipsData.character == "Z") largerPoint = this.GetLargerPoint(cd.GetCSChip().center, new Point(0, 0));
					}
					Rectangle rectangle = new Rectangle(MapPos.X * chipsize.Width - largerPoint.X, MapPos.Y * chipsize.Height - largerPoint.Y, size.Width, size.Height);
					this.RedrawMap(g,
						new Rectangle((int)Math.Ceiling((decimal)rectangle.X / chipsize.Width), (int)Math.Ceiling((decimal)rectangle.Y / chipsize.Height),
							(int)Math.Ceiling((decimal)rectangle.Width / chipsize.Width), (int)Math.Ceiling((decimal)rectangle.Height / chipsize.Height)));
				}
			}
			else
			{ // 背景レイヤー
				ChipData cschip = cd.GetCSChip();
				Size size2 = cschip.size;
				ChipData cschip2 = default(ChipsData).GetCSChip();
				if (this.DrawLayerRef.ContainsKey(stageChar))
				{
					ChipsData chipsData2 = this.DrawLayerRef[stageChar];
					size2 = this.GetLargerSize(size2, cschip2.size);
				}
				if (size2 == default(Size)) this.RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
				else
				{
					Point largerPoint2 = this.GetLargerPoint(cschip.center, cschip2.center);
					Rectangle rectangle2 = new Rectangle(MapPos.X * chipsize.Width - largerPoint2.X, MapPos.Y * chipsize.Height - largerPoint2.Y, size2.Width, size2.Height);
					this.RedrawMap(g,
						new Rectangle(rectangle2.X / chipsize.Width, rectangle2.Y / chipsize.Height,
							rectangle2.Width / chipsize.Width, rectangle2.Height / chipsize.Height));
				}
			}
		}

		public void RedrawMap(Graphics g, Rectangle rect)
		{
			GraphicsState transState;
			Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
			using (Bitmap bitmap = new Bitmap(rect.Width * chipsize.Width, rect.Height * chipsize.Height, PixelFormat.Format32bppArgb))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.PixelOffsetMode = PixelOffsetMode.Half;
					graphics.FillRectangle(Brushes.Transparent, new Rectangle(new Point(0, 0), bitmap.Size));
					Point point = new Point(-1, -1);
					for (int i = rect.Y; i < rect.Bottom; i++)
					{
						point.Y++;
						point.X = -1;
						for (int j = rect.X; j < rect.Right; j++)
						{
							point.X++;
							if (!GUIDesigner.StageText.IsOverflow(new Point(j, i)))
							{
								string stageChar = GUIDesigner.StageText.GetStageChar(new Point(j, i));
								ChipsData chipsData;
								if (Global.state.EditingForeground)
								{
									if (!this.DrawItemRef.ContainsKey(stageChar))
									{
										goto IL_514;
									}
									if (Global.state.MapEditMode)
									{
										chipsData = this.DrawWorldRef[stageChar];
									}
									else
									{
										chipsData = this.DrawItemRef[stageChar];
									}
								}
								else
								{
									if (!this.DrawLayerRef.ContainsKey(stageChar))
									{
										goto IL_514;
									}
									chipsData = this.DrawLayerRef[stageChar];
								}
								if (Global.state.EditingForeground)
								{
									if (chipsData.character.Equals(Global.cpd.Mapchip[0].character))
									{
										goto IL_514;
									}
								}
								else if (chipsData.character.Equals(Global.cpd.Layerchip[0].character))
								{
									goto IL_514;
								}
								ChipData cschip = chipsData.GetCSChip();
								if (cschip.size != default(Size))
								{
									if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && cschip.xdbackgrnd)
									{
										graphics.DrawImage(this.DrawExOrig,
											new Rectangle(new Point(point.X * chipsize.Width - cschip.center.X, point.Y * chipsize.Height - cschip.center.Y), chipsize),
											new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
									}
									if (Global.state.EditingForeground)
									{
										if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
										{
											graphics.DrawImage(this.DrawOribossOrig, point.X * chipsize.Width, point.Y * chipsize.Height);
										}
										else
										{
											transState = graphics.Save();
											if (cschip.view_size != default)
												graphics.TranslateTransform(point.X * chipsize.Width - cschip.center.X + cschip.view_size.Width / 2,
												point.Y * chipsize.Height - cschip.center.Y + cschip.view_size.Height / 2);
											else
												graphics.TranslateTransform(point.X * chipsize.Width - cschip.center.X + cschip.size.Width / 2,
												point.Y * chipsize.Height - cschip.center.Y + cschip.size.Height / 2);
											graphics.RotateTransform(cschip.rotate);
											if (cschip.scale != default) graphics.ScaleTransform(cschip.scale, cschip.scale);
											graphics.DrawImage(this.DrawChipOrig,
												new Rectangle(-cschip.size.Width / 2, -cschip.size.Height / 2, cschip.size.Width, cschip.size.Height),
												new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
											graphics.Restore(transState);
										}
									}
									else
									{
										graphics.DrawImage(this.DrawLayerOrig,
											new Rectangle(new Point(point.X * chipsize.Width - cschip.center.X, point.Y * chipsize.Height - cschip.center.Y), chipsize),
											new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
									}

									if (chipsData.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 &&
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
										graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, point.X * chipsize.Width, point.Y * chipsize.Height, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
									}
									else if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && !cschip.xdbackgrnd)
									{
										graphics.DrawImage(this.DrawExOrig,
											new Rectangle(new Point(point.X * chipsize.Width - cschip.center.X, point.Y * chipsize.Height - cschip.center.Y), chipsize),
											new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
									}
								}
							}
							IL_514:;
						}
					}
					point = new Point(-1, -1);
					for (int k = rect.Y; k < rect.Bottom; k++)
					{
						point.Y++;
						point.X = -1;
						for (int l = rect.X; l < rect.Right; l++)
						{
							point.X++;
							if (!GUIDesigner.StageText.IsOverflow(new Point(l, k)))
							{
								string stageChar = GUIDesigner.StageText.GetStageChar(new Point(l, k));
								ChipsData chipsData;
								if (Global.state.EditingForeground)
								{
									if (!this.DrawItemRef.ContainsKey(stageChar))
									{
										goto IL_9DD;
									}
									if (Global.state.MapEditMode)
									{
										chipsData = this.DrawWorldRef[stageChar];
									}
									else
									{
										chipsData = this.DrawItemRef[stageChar];
									}
								}
								else
								{
									if (!this.DrawLayerRef.ContainsKey(stageChar))
									{
										goto IL_9DD;
									}
									chipsData = this.DrawLayerRef[stageChar];
								}
								if (Global.state.EditingForeground)
								{
									if (chipsData.character.Equals(Global.cpd.Mapchip[0].character))
									{
										goto IL_9DD;
									}
								}
								else if (chipsData.character.Equals(Global.cpd.Layerchip[0].character))
								{
									goto IL_9DD;
								}
								ChipData cschip = chipsData.GetCSChip();
								if (cschip.size == default(Size))
								{
									if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && cschip.xdbackgrnd)
									{
										graphics.DrawImage(this.DrawExOrig,
											new Rectangle(new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsize),
											new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
									}
									if (Global.state.EditingForeground)
									{
										transState = graphics.Save();
										graphics.TranslateTransform(point.X * chipsize.Width, point.Y * chipsize.Height);
										Pen pen, pen2;
										SolidBrush brush;
										PointF[] vo_pa;
										double rad = 0;
										const double math_pi = 3.1415926535897931;
										switch (cschip.name)
										{
											case "一方通行":
												if (cschip.description.Contains("表示なし")) break;
												pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
												if (cschip.description.Contains("右"))
													graphics.DrawLine(pen, cschip.view_size.Width - 1, 0, cschip.view_size.Width - 1, cschip.view_size.Height);
												else if (cschip.description.Contains("左"))
													graphics.DrawLine(pen, 1, 0, 1, cschip.view_size.Height);
												else if (cschip.description.Contains("上"))
													graphics.DrawLine(pen, 0, 1, cschip.view_size.Width, 1);
												else if (cschip.description.Contains("下"))
													graphics.DrawLine(pen, 0, cschip.view_size.Height - 1, cschip.view_size.Width, cschip.view_size.Height - 1);
												pen.Dispose();
												break;
											case "左右へ押せるドッスンスンのゴール":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(-cschip.center.X + 1, -cschip.center.Y + 1);
												pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
												graphics.DrawRectangle(pen, 0, 0, 94, 62);
												graphics.DrawLine(pen, 0, 0, 94, 62);
												graphics.DrawLine(pen, 0, 62, 94, 0);
												pen.Dispose();
												break;
											case "シーソー":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(16, 0);
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
												graphics.FillPolygon(brush, vo_pa);
												vo_pa = new PointF[3];
												vo_pa[0].X = 0;
												vo_pa[0].Y = 0;
												vo_pa[1].X = -16;
												vo_pa[1].Y = 128;
												vo_pa[2].X = 16;
												vo_pa[2].Y = 128;
												brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
												graphics.FillPolygon(brush, vo_pa);
												brush.Dispose();
												pen = new Pen(Color.White, 2);
												if (cschip.description.Contains("左")) rad = -56;
												else if (cschip.description.Contains("右")) rad = 56;
												else rad = 0;
												graphics.DrawLine(pen,
													(float)(Math.Floor(Math.Cos(((rad + 180) * math_pi) / 180) * 160) + Math.Floor(Math.Cos(((rad + 270) * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Sin(((rad + 180) * math_pi) / 180) * 160) + Math.Floor(Math.Sin(((rad + 270) * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Cos((rad * math_pi) / 180) * 160) + Math.Floor(Math.Cos(((rad - 90) * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Sin((rad * math_pi) / 180) * 160) + Math.Floor(Math.Sin(((rad - 90) * math_pi) / 180) * 12))
												);
												pen.Dispose();
												break;
											case "ブランコ":
												graphics.DrawImage(this.DrawChipOrig,
													new Rectangle(0, 0, chipsize.Width, chipsize.Height),
													new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(16, 16);
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
												graphics.DrawLine(pen, (float)Math.Cos(rad) * 12, (float)Math.Sin(rad) * 12, (float)dx, (float)dy);
												graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
												graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
												brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
												graphics.FillPolygon(brush, vo_pa);
												pen2 = new Pen(Color.White, 2);
												rad = 90 + Math.Floor((double)(30 + 5) / 10);
												graphics.DrawLine(pen2,
													(float)Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192),
													(float)Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192),
													(float)Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192),
													(float)Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192)
												);
												if (cschip.description == "２個連続")
												{
													graphics.TranslateTransform(384, 0);
													graphics.DrawImage(this.DrawChipOrig,
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
													graphics.DrawLine(pen, (float)Math.Cos(rad) * 12, (float)Math.Sin(rad) * 12, (float)dx, (float)dy);
													graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
													graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
													graphics.FillPolygon(brush, vo_pa);
													rad = 90 + Math.Floor((double)(-30 - 5) / 10);
													graphics.DrawLine(pen2,
														(float)Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192),
														(float)Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192),
														(float)Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192),
														(float)Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192)
													);
												}
												pen2.Dispose();
												pen.Dispose();
												brush.Dispose();
												break;
											case "スウィングバー":
												graphics.DrawImage(this.DrawChipOrig,
													new Rectangle(0, 0, chipsize.Width, chipsize.Height),
													new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(16, 16);
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
												graphics.FillPolygon(brush, vo_pa);
												brush.Dispose();
												pen = new Pen(Color.White, 2);
												if (cschip.description.Contains("左"))
												{
													rad = 180 + Math.Floor((double)(-26 - 5) / 10);
													graphics.DrawLine(pen, (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 192) + Math.Floor(Math.Cos((rad + 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 192) + Math.Floor(Math.Sin((rad + 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 60) + Math.Floor(Math.Cos((rad + 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 60) + Math.Floor(Math.Sin((rad + 90) * math_pi / 180) * 12)));
												}
												else if (cschip.description.Contains("右"))
												{
													rad = 360 + Math.Floor((double)(26 + 5) / 10);
													graphics.DrawLine(pen, (float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 60) + Math.Floor(Math.Cos((rad - 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 60) + Math.Floor(Math.Sin((rad - 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Cos(rad * math_pi / 180) * 192) + Math.Floor(Math.Cos((rad - 90) * math_pi / 180) * 12)),
														(float)(Math.Floor(Math.Sin(rad * math_pi / 180) * 192) + Math.Floor(Math.Sin((rad - 90) * math_pi / 180) * 12)));
												}
												pen.Dispose();
												break;
											case "動くＴ字型":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(16, 48);
												rad = 270 + Math.Floor((double)(-30 - 5) / 10);
												vo_pa = new PointF[3];
												vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * 182;
												vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * 182;
												vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * 182;
												vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * 182;
												vo_pa[2].X = 0;
												vo_pa[2].Y = 0;
												brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
												graphics.FillPolygon(brush, vo_pa);
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
												graphics.FillPolygon(brush, vo_pa);
												pen = new Pen(Color.White, 2);
												graphics.DrawLine(pen,
													(float)(Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
													(float)(Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12))
												);
												if (cschip.description == "２個連続")
												{
													graphics.TranslateTransform(416, 0);
													rad = 270 + Math.Floor((double)(30 + 5) / 10);
													vo_pa = new PointF[3];
													vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * 182;
													vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * 182;
													vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * 182;
													vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * 182;
													vo_pa[2].X = 0;
													vo_pa[2].Y = 0;
													brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
													graphics.FillPolygon(brush, vo_pa);
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
													graphics.FillPolygon(brush, vo_pa);
													graphics.DrawLine(pen,
														(float)(Math.Floor(Math.Cos(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
														(float)(Math.Floor(Math.Sin(((rad + 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12)),
														(float)(Math.Floor(Math.Cos(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Cos((rad * math_pi) / 180) * 12)),
														(float)(Math.Floor(Math.Sin(((rad - 20) * math_pi) / 180) * 192) + Math.Floor(Math.Sin((rad * math_pi) / 180) * 12))
													);
												}
												pen.Dispose();
												brush.Dispose();
												break;
											case "ロープ":
											case "長いロープ":
											case "ゆれる棒":
												graphics.DrawImage(this.DrawChipOrig,
													new Rectangle(0, 0, chipsize.Width, chipsize.Height),
													new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												graphics.TranslateTransform(16, 16);
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
												graphics.FillPolygon(brush, vo_pa);
												if (cschip.description.Contains("２本連続"))
												{
													graphics.TranslateTransform(320, 0);
													graphics.DrawImage(this.DrawChipOrig,
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
													graphics.FillPolygon(brush, vo_pa);
												}
												brush.Dispose();
												break;
											case "人間大砲":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												if (cschip.description.Contains("向き")) graphics.TranslateTransform(0, -12);
												brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
												graphics.FillEllipse(brush, 16 - 19, 16 - 19, 38, 38);
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
												graphics.FillPolygon(brush, vo_pa);
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
												graphics.FillPolygon(brush, vo_pa);
												brush.Dispose();
												break;
											case "曲線による上り坂":
											case "曲線による下り坂":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
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

													graphics.DrawLines(pen, vo_pa);
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
													graphics.DrawLines(pen, vo_pa);
													vo_pa = new PointF[2];
													vo_pa[0].X = j20;
													vo_pa[0].Y = k20;
													vo_pa[1].X = l20;
													vo_pa[1].Y = i21;
													graphics.DrawLines(pen, vo_pa);
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
													graphics.FillPolygon(brush, vo_pa);
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
													graphics.FillPolygon(brush, vo_pa);
													vo_pa = new PointF[4];
													vo_pa[0].X = j20;
													vo_pa[0].Y = k20;
													vo_pa[1].X = l20;
													vo_pa[1].Y = i21;
													vo_pa[2].X = l20;
													vo_pa[2].Y = 128;
													vo_pa[3].X = j20;
													vo_pa[3].Y = 128;
													graphics.FillPolygon(brush, vo_pa);
													if (128 + point.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
														graphics.FillRectangle(brush, 0, 128, 256, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (128 + point.Y * chipsize.Height));
													brush.Dispose();
												}
												break;
											case "乗れる円":
											case "跳ねる円":
											case "円":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
												int radius = default;
												graphics.TranslateTransform(0, 16);
												if (cschip.name == "円")
												{
													brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
													if (cschip.description.Contains("乗ると下がる"))
													{
														radius = 80;
													}
													else
													{
														radius = 112; graphics.TranslateTransform(0, 24);
														if (cschip.description.Contains("下から")) graphics.TranslateTransform(0, 106);
													}
												}
												else
												{
													brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
													if (cschip.description.Contains("大"))
													{
														if (cschip.name == "乗れる円")
														{
															radius = 144;
														}
														else if (cschip.name == "跳ねる円")
														{
															radius = 128; graphics.TranslateTransform(16, 0);
														}
													}
													else
													{
														radius = 96;
													}
												}
												graphics.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
												brush.Dispose();
												break;
											case "半円":
												graphics.SmoothingMode = SmoothingMode.AntiAlias;
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
														graphics.DrawLines(pen, vo_pa);
														j21 = 0;
														for (var k2 = 90; k2 >= 40; k2 -= 5)
														{
															vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144);
															vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144);
															j21++;
														}
														vo_pa[j21].X = 240;
														vo_pa[j21].Y = 63;
														graphics.DrawLines(pen, vo_pa);
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
														graphics.FillPolygon(brush, vo_pa);
														j21 = 0;
														for (var k2 = 90; k2 >= 40; k2 -= 5)
														{
															vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144);
															vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144);
															j21++;
														}

														vo_pa[j21].X = 240;
														vo_pa[j21].Y = 64;
														j21++;
														vo_pa[j21].X = 120;
														vo_pa[j21].Y = 64;
														graphics.FillPolygon(brush, vo_pa);
														brush.Dispose();
													}
												}
												else
												{
													brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
													if (64 + point.Y * chipsize.Height < Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height)
														graphics.FillRectangle(brush, 120 - 20, 64, 40, Global.cpd.runtime.Definitions.StageSize.y * chipsize.Height - (64 + point.Y * chipsize.Height));
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
													graphics.FillPolygon(brush, vo_pa);
													j21 = 0;
													for (var k2 = 90; k2 >= 40; k2 -= 5)
													{
														vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144);
														vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144);
														j21++;
													}

													vo_pa[j21].X = 240;
													vo_pa[j21].Y = 64;
													j21++;
													vo_pa[j21].X = 120;
													vo_pa[j21].Y = 64;
													graphics.FillPolygon(brush, vo_pa);
													brush.Dispose();
												}
												break;
											case "ファイヤーバー":
											case "ファイヤーバー2本":
											case "ファイヤーバー3本　左回り":
											case "ファイヤーバー3本　右回り":
											case "スウィングファイヤーバー":
												{
													int width = default;
													graphics.DrawImage(this.DrawChipOrig,
														new Rectangle(0, 0, chipsize.Width, chipsize.Height),
														new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
													graphics.SmoothingMode = SmoothingMode.AntiAlias;
													graphics.TranslateTransform(16, 16);

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
													graphics.FillPolygon(brush, vo_pa);

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
													graphics.FillPolygon(brush, vo_pa);
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
														graphics.FillPolygon(brush, vo_pa);

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
														graphics.FillPolygon(brush, vo_pa);

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
															graphics.FillPolygon(brush, vo_pa);

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
															graphics.FillPolygon(brush, vo_pa);
														}
													}
													brush.Dispose();
												}
												break;
											case "人口太陽":
												{
													graphics.SmoothingMode = SmoothingMode.AntiAlias;

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
														graphics.FillPolygon(brush, vo_pa);

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
														graphics.FillPolygon(brush, vo_pa);
													}
													brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
													graphics.FillEllipse(brush, -64, -64 + 8, 128, 128);
													brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
													graphics.FillEllipse(brush, -20, -20 + 8, 40, 40);

													brush.Dispose();
												}
												break;
											case "ファイヤーリング":
												{
													graphics.SmoothingMode = SmoothingMode.AntiAlias;
													graphics.TranslateTransform(32 * 5, 32 * 5);

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
														graphics.FillPolygon(brush, vo_pa);

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
														graphics.FillPolygon(brush, vo_pa);

														v += 360 / n;
													}

													brush.Dispose();
												}
												break;
											default:
												graphics.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
												if (chipsData.character == Global.cpd.Mapchip[1].character)
												{
													graphics.ScaleTransform(-1, 1); // 基本主人公は逆向き
													if (Global.state.MapEditMode && rect.X > Global.cpd.runtime.Definitions.MapSize.x / 2 ||
														Global.state.ChipRegister.ContainsKey("view_move_type") && int.Parse(Global.state.ChipRegister["view_move_type"]) == 2)
													{
														graphics.ScaleTransform(-1, 1);// 特殊条件下では元の向き
													}
												}
												graphics.RotateTransform(cschip.rotate);
												if (cschip.repeat != default)
												{
													for (int j = 0; j < cschip.repeat; j++)
													{
														graphics.DrawImage(this.DrawChipOrig,
															new Rectangle(-chipsize.Width / 2 + j * chipsize.Width * Math.Sign(cschip.rotate), -chipsize.Height / 2, chipsize.Width, chipsize.Height),
															new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
													}
												}
												else if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && chipsData.character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
												{// 水の半透明処理
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
													graphics.DrawImage(this.DrawChipOrig, new Rectangle(-chipsize.Width / 2, -chipsize.Height / 2, chipsize.Width, chipsize.Height),
														cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
												}
												else graphics.DrawImage(this.DrawChipOrig,
													new Rectangle(-chipsize.Width / 2, -chipsize.Height / 2, chipsize.Width, chipsize.Height),
													new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
												break;
										}
										graphics.Restore(transState);
									}
									else
									{
										graphics.DrawImage(this.DrawLayerOrig,
											new Rectangle(new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsize),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
									}
									if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && !cschip.xdbackgrnd)
									{
										graphics.DrawImage(this.DrawExOrig,
											new Rectangle(new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsize),
											new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
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
		}

		private void GUIDesigner_MouseCaptureChanged(object sender, EventArgs e)
		{
			if (this.MousePressed)
			{
				this.GUIDesigner_MouseUp(sender, new MouseEventArgs(MouseButtons.None, 1, Cursor.Position.X, Cursor.Position.Y, 0));
			}
		}

		private void GUIDesigner_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
				case Keys.Y:
					this.Redo();
					break;
				case Keys.Z:
					this.Undo();
					return;
				default:
					return;
				}
			}
		}

		private void QuickTestRun_Click(object sender, EventArgs e)
		{
			Point mouseStartPoint = this.MouseStartPoint;
			if (GUIDesigner.StageText.IsOverflow(mouseStartPoint))
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (string text in Global.cpd.EditingMap)
			{
				list.Add(text.Replace(Global.cpd.Mapchip[1].character, Global.cpd.Mapchip[0].character));
			}
			int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : Global.cpd.runtime.Definitions.StageSize.bytesize;
			list[mouseStartPoint.Y] = list[mouseStartPoint.Y].Remove(mouseStartPoint.X * num, num);
			list[mouseStartPoint.Y] = list[mouseStartPoint.Y].Insert(mouseStartPoint.X * num, Global.cpd.Mapchip[1].character);
			Global.state.QuickTestrunSource = list.ToArray();
			Global.MainWnd.Testrun();
		}

		private void PickChip_Click(object sender, EventArgs e)
		{
			Point mouseStartPoint = this.MouseStartPoint;
			string stageChar = GUIDesigner.StageText.GetStageChar(mouseStartPoint);
			if (Global.state.MapEditMode)
			{
				if (this.DrawWorldRef.ContainsKey(stageChar))
				{
					Global.state.CurrentChip = this.DrawWorldRef[stageChar];
					return;
				}
			}
			else if (Global.state.EditingForeground)
			{
				if (this.DrawItemRef.ContainsKey(stageChar))
				{
					Global.state.CurrentChip = this.DrawItemRef[stageChar];
					return;
				}
			}
			else if (this.DrawLayerRef.ContainsKey(stageChar))
			{
				Global.state.CurrentChip = this.DrawLayerRef[stageChar];
			}
		}

		private void MenuCut_Click(object sender, EventArgs e)
		{
			Global.MainWnd.GuiCut.Checked = true;
			Global.MainWnd.GuiCut_Click(sender, e);
			this.Refresh();
		}

		private void MenuCopy_Click(object sender, EventArgs e)
		{
			Global.MainWnd.GuiCopy.Checked = true;
			Global.MainWnd.GuiCopy_Click(sender, e);
			this.Refresh();
		}

		private void MenuPaste_Click(object sender, EventArgs e)
		{
			Global.MainWnd.GuiPaste.Checked = true;
			Global.MainWnd.GuiPaste_Click(sender, e);
			this.Refresh();
		}

		private void DoTestrun_Click(object sender, EventArgs e)
		{
			Global.MainWnd.Testrun();
		}

		private void ProjectConfig_Click(object sender, EventArgs e)
		{
			Global.MainWnd.ProjectConfig_Click(this, new EventArgs());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.CursorContextMenu = new ContextMenuStrip(this.components);
			this.QuickTestRun = new ToolStripMenuItem();
			this.PickChip = new ToolStripMenuItem();
			this.toolStripMenuItem3 = new ToolStripSeparator();
			this.MenuCut = new ToolStripMenuItem();
			this.MenuCopy = new ToolStripMenuItem();
			this.MenuPaste = new ToolStripMenuItem();
			this.toolStripMenuItem2 = new ToolStripSeparator();
			this.DoTestrun = new ToolStripMenuItem();
			this.ProjectConfig = new ToolStripMenuItem();
			this.CursorContextMenu.SuspendLayout();
			base.SuspendLayout();
			this.CursorContextMenu.Items.AddRange(new ToolStripItem[]
			{
				this.QuickTestRun,
				this.PickChip,
				this.toolStripMenuItem3,
				this.MenuCut,
				this.MenuCopy,
				this.MenuPaste,
				this.toolStripMenuItem2,
				this.DoTestrun,
				this.ProjectConfig
			});
			this.CursorContextMenu.Name = "CursorContextMenu";
			this.CursorContextMenu.Size = new Size(275, 170);
			this.QuickTestRun.Image = Resources.testrunplace;
			this.QuickTestRun.Name = "QuickTestRun";
			this.QuickTestRun.ShortcutKeyDisplayString = "MiddleClick";
			this.QuickTestRun.Size = new Size(274, 22);
			this.QuickTestRun.Text = "ここからテスト実行(&R)";
			this.QuickTestRun.Click += this.QuickTestRun_Click;
			this.PickChip.Name = "PickChip";
			this.PickChip.ShortcutKeyDisplayString = "";
			this.PickChip.Size = new Size(274, 22);
			this.PickChip.Text = "直下のチップを拾う(&P)";
			this.PickChip.Click += this.PickChip_Click;
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new Size(271, 6);
			this.MenuCut.Image = Resources.cut;
			this.MenuCut.Name = "MenuCut";
			this.MenuCut.ShortcutKeyDisplayString = "Ctrl+X";
			this.MenuCut.Size = new Size(274, 22);
			this.MenuCut.Text = "切り取り(&X)";
			this.MenuCut.Click += this.MenuCut_Click;
			this.MenuCopy.Image = Resources.copy;
			this.MenuCopy.Name = "MenuCopy";
			this.MenuCopy.ShortcutKeyDisplayString = "Ctrl+C";
			this.MenuCopy.Size = new Size(274, 22);
			this.MenuCopy.Text = "コピー(&C)";
			this.MenuCopy.Click += this.MenuCopy_Click;
			this.MenuPaste.Image = Resources.paste;
			this.MenuPaste.Name = "MenuPaste";
			this.MenuPaste.ShortcutKeyDisplayString = "Ctrl+V";
			this.MenuPaste.Size = new Size(274, 22);
			this.MenuPaste.Text = "貼り付け(&V)";
			this.MenuPaste.Click += this.MenuPaste_Click;
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new Size(271, 6);
			this.DoTestrun.Image = Resources.testrunstage;
			this.DoTestrun.Name = "DoTestrun";
			this.DoTestrun.ShortcutKeyDisplayString = "Ctrl+Space";
			this.DoTestrun.Size = new Size(274, 22);
			this.DoTestrun.Text = "テスト実行(&T)";
			this.DoTestrun.Click += this.DoTestrun_Click;
			this.ProjectConfig.Image = Resources.map_edit;
			this.ProjectConfig.Name = "ProjectConfig";
			this.ProjectConfig.ShortcutKeyDisplayString = "F2";
			this.ProjectConfig.Size = new Size(274, 22);
			this.ProjectConfig.Text = "プロジェクト設定(&P)";
			this.ProjectConfig.Click += this.ProjectConfig_Click;
			base.Name = "GUIDesigner";
			base.Size = new Size(315, 177);
			base.MouseCaptureChanged += this.GUIDesigner_MouseCaptureChanged;
			base.MouseMove += this.GUIDesigner_MouseMove;
			base.MouseDown += this.GUIDesigner_MouseDown;
			base.MouseUp += this.GUIDesigner_MouseUp;
			base.KeyDown += this.GUIDesigner_KeyDown;
			this.CursorContextMenu.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		public Image DrawChipOrig;

		public Bitmap DrawMask;

		public Image DrawLayerOrig;

		public Bitmap DrawLayerMask;

		public Image DrawOribossOrig;

		public Bitmap DrawOribossMask;

		public Image DrawExOrig;

		public Bitmap DrawExMask;

		public Image DrawHaikeiOrig, DrawHaikei2Orig, DrawHaikei3Orig, DrawHaikei4Orig, DrawSecondHaikeiOrig,
			DrawSecondHaikei2Orig, DrawSecondHaikei3Orig, DrawSecondHaikei4Orig,
			DrawChizuOrig;

		public Dictionary<string, ChipsData> DrawItemRef = new Dictionary<string, ChipsData>();

		public Dictionary<string, ChipsData> DrawLayerRef = new Dictionary<string, ChipsData>();

		public Dictionary<string, ChipsData> DrawWorldRef = new Dictionary<string, ChipsData>();

		private Bitmap ForeLayerBmp;

		private Bitmap BackLayerBmp;

		private GUIDesigner.EditTool curTool;

		private GUIDesigner.CopyPasteTool cpaste;

		public List<string[]> StageBuffer = new List<string[]>();

		public int BufferCurrent = -1;

		public string ClipedString;

		public Rectangle DrawRectangle = default(Rectangle);

		public GUIDesigner.DirectDrawMode DrawMode;

		private bool bdraw = true;

		private string[] ForePrevDrawn;

		private string[] BackPrevDrawn;

		private Bitmap ForegroundBuffer = new Bitmap(1, 1);

		private int bufpos = -1;

		private double zi;

		private bool FLayer;

		private bool DULayer;

		private bool TULayer;

		private int EditMap = -1;

		private Point MouseStartPoint = default(Point);

		private Point MouseLastPoint = default(Point);

		private bool MousePressed;

		private List<char[]> repls = new List<char[]>();

        private struct BufStr
        {
            public int left;
            public int right;
            public int y;

            public BufStr(int left, int right, int y)
			{
				this.left = left;
				this.right = right;
				this.y = y;
			}
        };

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
				if (GUIDesigner.StageText.IsOverflow(p))
				{
					return Global.cpd.Mapchip[0].character;
				}
				if (Global.state.EditingForeground)
				{
					return Global.cpd.EditingMap[p.Y].Substring(p.X * Global.state.GetCByte, Global.state.GetCByte);
				}
				return Global.cpd.EditingLayer[p.Y].Substring(p.X * Global.state.GetCByte, Global.state.GetCByte);
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

		public struct KeepDrawData
		{
			public KeepDrawData(ChipData c, Point p, string chara)
			{
				this.cd = c;
				this.pos = p;
				this.chara = chara;
			}

			public ChipData cd;

			public Point pos;

			public string chara;
		}
	}
}

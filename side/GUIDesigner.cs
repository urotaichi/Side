using MasaoPlus.Properties;
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
		public EditTool CurrentTool
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

		public CopyPasteTool CopyPaste
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

		public event ChangeBuffer ChangeBufferInvoke;

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
				return default;
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

		private void DrawExtendSizeMap(ChipData cschip, Graphics g, Point p, bool foreground, string chara){
			GraphicsState transState;
			Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
			if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd)
			{ // 拡張画像　背面
				g.DrawImage(this.DrawExOrig,
					new Rectangle(new Point(p.X * chipsize.Width - cschip.center.X, p.Y * chipsize.Height - cschip.center.Y), chipsize),
					new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
			}
			if (foreground)
			{ // 標準パターン画像
				if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chara == "Z")
				{
					g.DrawImage(this.DrawOribossOrig, p.X * chipsize.Width, p.Y * chipsize.Height);
				}
				else
				{
					transState = g.Save();
					if (cschip.view_size != default)
						g.TranslateTransform(p.X * chipsize.Width - cschip.center.X + cschip.view_size.Width / 2, p.Y * chipsize.Height - cschip.center.Y + cschip.view_size.Height / 2);
					else
						g.TranslateTransform(p.X * chipsize.Width - cschip.center.X + cschip.size.Width / 2, p.Y * chipsize.Height - cschip.center.Y + cschip.size.Height / 2);
					g.RotateTransform(cschip.rotate);
					if (cschip.scale != default) g.ScaleTransform(cschip.scale, cschip.scale);
					g.DrawImage(this.DrawChipOrig,
						new Rectangle(-cschip.size.Width / 2, -cschip.size.Height / 2, cschip.size.Width, cschip.size.Height),
						new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
					g.Restore(transState);
				}
			}
			else
			{ // 背景レイヤー画像
				g.DrawImage(this.DrawLayerOrig,
					new Rectangle(p.X * chipsize.Width - cschip.center.X, p.Y * chipsize.Height - cschip.center.Y, cschip.size.Width, cschip.size.Height),
					new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
			}
			if (chara == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 &&
				Global.state.ChipRegister.ContainsKey("oriboss_ugoki") && Global.config.draw.ExtendDraw)
			{
				Point point = default;
				switch (int.Parse(Global.state.ChipRegister["oriboss_ugoki"]))
				{
					case 1:
						point = new Point(352, 256);
						break;
					case 2:
						point = new Point(96, 0);
						break;
					case 3:
						point = new Point(64, 0);
						break;
					case 4:
						point = new Point(256, 0);
						break;
					case 5:
						point = new Point(288, 0);
						break;
					case 6:
						point = new Point(288, 448);
						break;
					case 7:
						point = new Point(320, 448);
						break;
					case 8:
						point = new Point(32, 32);
						break;
					case 9:
						point = new Point(96, 0);
						break;
					case 10:
						point = new Point(0, 32);
						break;
					case 11:
						point = new Point(64, 0);
						break;
					case 12:
						point = new Point(96, 32);
						break;
					case 13:
						point = new Point(64, 0);
						break;
					case 14:
						point = new Point(352, 448);
						break;
					case 15:
						point = new Point(416, 448);
						break;
					case 16:
						point = new Point(288, 448);
						break;
					case 17:
						point = new Point(320, 448);
						break;
					case 18:
						point = new Point(96, 0);
						break;
					case 19:
						point = new Point(96, 0);
						break;
					case 20:
						point = new Point(256, 0);
						break;
					case 21:
						point = new Point(256, 0);
						break;
					case 22:
						point = new Point(352, 448);
						break;
					case 23:
						point = new Point(384, 448);
						break;
					case 24:
						point = new Point(32, 32);
						break;
					case 25:
						point = new Point(32, 32);
						break;
					case 26:
						point = new Point(32, 128);
						break;
					case 27:
						point = new Point(32, 128);
						break;
				}
				g.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, p.X * chipsize.Width, p.Y * chipsize.Height, new Rectangle(point, chipsize), GraphicsUnit.Pixel);
			}
			else if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
			{ // 拡張画像　前面
				g.DrawImage(this.DrawExOrig,
					new Rectangle(new Point(p.X * chipsize.Width - cschip.center.X, p.Y * chipsize.Height - cschip.center.Y), chipsize),
					new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
			}
		}
		private void DrawNormalSizeMap(ChipData cschip, Graphics g, Point p, bool foreground, string chara, int x){
			GraphicsState transState;
			Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
			if (Global.config.draw.ExtendDraw && cschip.xdraw != default && cschip.xdbackgrnd)
			{ // 拡張画像　背面
				g.DrawImage(this.DrawExOrig,
					new Rectangle(p.X * chipsize.Width, p.Y * chipsize.Height, chipsize.Width, chipsize.Height),
					new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
			}
			if (foreground)
			{ // 標準パターン画像
				transState = g.Save();
				g.TranslateTransform(p.X * chipsize.Width, p.Y * chipsize.Height);
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
					case "ファイヤーバー2本":
					case "ファイヤーバー3本　左回り":
					case "ファイヤーバー3本　右回り":
					case "スウィングファイヤーバー":
					case "人口太陽":
					case "ファイヤーリング":
					case "ファイヤーウォール":
                    case "スイッチ式ファイヤーバー":
                    case "スイッチ式動くＴ字型":
                    case "スイッチ式速く動くＴ字型":
                        AthleticView.list[cschip.name].Max(cschip, g, chipsize, this, p.Y);
						break;
					default:
						g.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
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
								g.DrawImage(this.DrawChipOrig,
									new Rectangle(-chipsize.Width / 2 + j * chipsize.Width * Math.Sign(cschip.rotate), -chipsize.Height / 2, chipsize.Width, chipsize.Height),
									new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
							}
						}
						else if(cschip.repeat_x != default)
						{
							for (int j = 0; j < cschip.repeat_x; j++)
							{
								g.DrawImage(this.DrawChipOrig,
									new Rectangle(-chipsize.Width / 2 + j * chipsize.Width, -chipsize.Height / 2, chipsize.Width, chipsize.Height),
									new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
							}
						}
						else if (cschip.repeat_y != default)
						{
							for (int j = 0; j < cschip.repeat_y; j++)
							{
								g.DrawImage(this.DrawChipOrig,
									new Rectangle(-chipsize.Width / 2, -chipsize.Height / 2 + j * chipsize.Height, chipsize.Width, chipsize.Height),
									new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
							}
						}
						else if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && chara == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
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
					new Rectangle(p.X * chipsize.Width, p.Y * chipsize.Height, chipsize.Width, chipsize.Height),
					new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
			}
			if (Global.config.draw.ExtendDraw && cschip.xdraw != default && !cschip.xdbackgrnd)
			{ // 拡張画像　前面
				g.DrawImage(this.DrawExOrig,
						new Rectangle(p.X * chipsize.Width, p.Y * chipsize.Height, chipsize.Width, chipsize.Height),
						new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
			}
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

			ChipsData chipsData;
			ChipData c;
			new List<KeepDrawData>();
			List<KeepDrawData> list = new List<KeepDrawData>();
			int num = 0;
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
						if (c.size != default) // 標準サイズより大きい
						{
							if (Global.state.UseBuffered)
							{
								g.CompositingMode = CompositingMode.SourceCopy;
								g.FillRectangle(Brushes.Transparent, new Rectangle(num2 * chipsize.Width - c.center.X, num * chipsize.Height - c.center.Y, c.size.Width, c.size.Height));
								g.CompositingMode = CompositingMode.SourceOver;
							}
							DrawExtendSizeMap(c, g, new Point(num2, num), foreground, chipsData.character);
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
			foreach (KeepDrawData keepDrawData in list)
			{
				ChipData cschip = keepDrawData.cd;
				if (Global.state.UseBuffered)
				{
					g.CompositingMode = CompositingMode.SourceCopy;
					g.FillRectangle(Brushes.Transparent, new Rectangle(keepDrawData.pos.X * chipsize.Width, keepDrawData.pos.Y * chipsize.Height,
						chipsize.Width, chipsize.Height));
					g.CompositingMode = CompositingMode.SourceOver;
				}
				DrawNormalSizeMap(cschip, g, keepDrawData.pos, foreground, keepDrawData.chara, keepDrawData.pos.X);
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
                using ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                graphics.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imageAttributes);
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
			this.DrawChipOrig = Image.FromStream(File.OpenRead(filename), false, false);
            this.DrawMask = new Bitmap(this.DrawChipOrig.Width, this.DrawChipOrig.Height);
			ColorMap[] remapTable = new ColorMap[]
			{
				new ColorMap()
			};
			using (ImageAttributes imageAttributes = new ImageAttributes())
			{
				imageAttributes.SetRemapTable(remapTable);
                using Graphics graphics = Graphics.FromImage(this.DrawMask);
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawMask.Width, this.DrawMask.Height));
                graphics.DrawImage(this.DrawChipOrig, new Rectangle(0, 0, this.DrawMask.Width, this.DrawMask.Height), 0, 0, this.DrawMask.Width, this.DrawMask.Height, GraphicsUnit.Pixel, imageAttributes);
            }

			if (Global.cpd.runtime.Definitions.LayerSize.bytesize != 0)
			{
				filename = Path.Combine(Global.cpd.where, Global.cpd.project.Config.LayerImage);
				this.DrawLayerOrig = Image.FromStream(File.OpenRead(filename), false, false);
                this.DrawLayerMask = new Bitmap(this.DrawLayerOrig.Width, this.DrawLayerOrig.Height);
                using ImageAttributes imageAttributes2 = new ImageAttributes();
                imageAttributes2.SetRemapTable(remapTable);
                using Graphics graphics2 = Graphics.FromImage(this.DrawLayerMask);
                graphics2.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height));
                graphics2.DrawImage(this.DrawLayerOrig, new Rectangle(0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height), 0, 0, this.DrawLayerMask.Width, this.DrawLayerMask.Height, GraphicsUnit.Pixel, imageAttributes2);
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
			this.DrawExOrig = Image.FromStream(File.OpenRead(filename), false, false);
			this.DrawExMask = new Bitmap(this.DrawExOrig.Width, this.DrawExOrig.Height);
			using (ImageAttributes imageAttributes3 = new ImageAttributes())
			{
				imageAttributes3.SetRemapTable(remapTable);
                using Graphics graphics3 = Graphics.FromImage(this.DrawExMask);
                graphics3.FillRectangle(Brushes.White, new Rectangle(0, 0, this.DrawExMask.Width, this.DrawExMask.Height));
                graphics3.DrawImage(this.DrawExOrig, new Rectangle(0, 0, this.DrawExMask.Width, this.DrawExMask.Height), 0, 0, this.DrawExMask.Width, this.DrawExMask.Height, GraphicsUnit.Pixel, imageAttributes3);
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
                using Graphics graphics = Graphics.FromImage(this.ForegroundBuffer);
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
			if (Global.config.draw.UseBufferingDraw && this.BufferingDraw)
			{
				if (Global.config.draw.UseBufferingMemoryDraw)
				{
					IntPtr hdc = e.Graphics.GetHdc();
					BitmapData bitmapData = new BitmapData();
					this.ForegroundBuffer.LockBits(new Rectangle(0, 0, this.ForegroundBuffer.Width, this.ForegroundBuffer.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb, bitmapData);
					Native.GDI32.BITMAPINFOHEADER bitmapinfoheader = default;
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
                using Brush brush3 = new SolidBrush(Color.FromArgb(Global.config.draw.AlphaBlending ? 160 : 255, DrawEx.GetForegroundColor(Global.state.Background)));
                if (this.DrawMode == GUIDesigner.DirectDrawMode.Rectangle)
                {
                    e.Graphics.FillRectangle(brush3, drawRectangle);
                }
                else
                {
                    using Pen pen = new Pen(brush3, (float)((int)((double)Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex) / 4));
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
							this.MouseStartPoint = default;
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
				ChipsData cd = default;
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
						Rectangle drawRectangle = default;
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
							Rectangle drawRectangle2 = default;
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
							Rectangle drawRectangle3 = default;
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
			Point point = default;
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
						Rectangle rectangle = default;
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
						Rectangle dr = default;
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
						Rectangle rectangle2 = default;
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
			if (Global.state.MapEditMode && cd.character.Equals(stageChar)
                || !Global.state.MapEditMode && (!Global.cpd.project.Use3rdMapData && cd.character.Equals(stageChar)
					|| Global.cpd.project.Use3rdMapData && cd.code.Equals(stageChar))) return;
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
				Size size = default;
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
				ChipData chipData = default;
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
				if (size == default) this.RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
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
				if (size2 == default) this.RedrawMap(g, new Rectangle(MapPos, new Size(1, 1)));
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
            using Bitmap bitmap = new Bitmap(rect.Width * chipsize.Width, rect.Height * chipsize.Height, PixelFormat.Format32bppArgb);
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
                            if (cschip.size != default) DrawExtendSizeMap(cschip, graphics, point, Global.state.EditingForeground, chipsData.character);
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
                            if (cschip.size == default) DrawNormalSizeMap(cschip, graphics, point, Global.state.EditingForeground, chipsData.character, rect.X);
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

		private EditTool curTool;

		private CopyPasteTool cpaste;

		public List<string[]> StageBuffer = new List<string[]>();

		public int BufferCurrent = -1;

		public string ClipedString;

		public Rectangle DrawRectangle = default;

		public DirectDrawMode DrawMode;

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

		private Point MouseStartPoint = default;

		private Point MouseLastPoint = default;

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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MasaoPlus
{
	// Token: 0x02000006 RID: 6
	public class GUIChipList : UserControl
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002283 File Offset: 0x00000483
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00006DB4 File Offset: 0x00004FB4
		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
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
					if (value >= Global.cpd.Mapchip.Length || value < 0)
					{
						return;
					}
					Global.state.CurrentChip = Global.cpd.Mapchip[value];
				}
				else
				{
					if (value >= Global.cpd.Layerchip.Length || value < 0)
					{
						return;
					}
					Global.state.CurrentChip = Global.cpd.Layerchip[value];
				}
				this.selectedIndex = value;
				this.Refresh();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000027 RID: 39 RVA: 0x0000228B File Offset: 0x0000048B
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002298 File Offset: 0x00000498
		private int vPosition
		{
			get
			{
				return this.vScr.Value;
			}
			set
			{
				this.vScr.Value = this.vPosition;
				this.vScr.Refresh();
				this.MainPanel.Refresh();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00006EA0 File Offset: 0x000050A0
		private int hMaxChip
		{
			get
			{
				int num = (int)Math.Floor((double)this.MainPanel.Width / (double)Global.cpd.runtime.Definitions.ChipSize.Width);
				if (num <= 0)
				{
					return 1;
				}
				return num;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000022C1 File Offset: 0x000004C1
		private void vScr_Scroll(object sender, ScrollEventArgs e)
		{
			this.MainPanel.Refresh();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006EE4 File Offset: 0x000050E4
		private void SetMaxValue()
		{
			this.vScr.LargeChange = this.MainPanel.Height;
			this.vScr.SmallChange = Global.cpd.runtime.Definitions.ChipSize.Height;
			this.vScr.Maximum = this.GetVirtSize() - this.MainPanel.Height + this.vScr.LargeChange - 1;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000022CE File Offset: 0x000004CE
		public GUIChipList()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00006F58 File Offset: 0x00005158
		public Point GetPosition(int idx)
		{
			int num = this.MainPanel.Width / Global.cpd.runtime.Definitions.ChipSize.Width;
			if (num <= 0)
			{
				num = 1;
			}
			return new Point(idx % num, (int)Math.Floor((double)idx / (double)num));
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000022DC File Offset: 0x000004DC
		public int GetVirtSize()
		{
			return this.GetVirtSize(this.MainPanel.Width);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00006FA4 File Offset: 0x000051A4
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
				return (int)Math.Ceiling((double)Global.cpd.Mapchip.Length / (double)num) * Global.cpd.runtime.Definitions.ChipSize.Height;
			}
			return (int)Math.Ceiling((double)Global.cpd.Layerchip.Length / (double)num) * Global.cpd.runtime.Definitions.ChipSize.Height;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000022EF File Offset: 0x000004EF
		public void ResizeInvoke()
		{
			this.GUIChipList_Resize(this, new EventArgs());
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000705C File Offset: 0x0000525C
		private void GUIChipList_Resize(object sender, EventArgs e)
		{
			if (this.GetVirtSize(base.Width) < base.Height)
			{
				this.vScr.Value = 0;
				this.vScr.Visible = false;
			}
			else
			{
				this.vScr.Visible = true;
				this.SetMaxValue();
			}
			this.MainPanel.Refresh();
		}

        // Token: 0x06000032 RID: 50 RVA: 0x000070B4 File Offset: 0x000052B4
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
					Point point = default(Point);
					ChipsData chipsData = default(ChipsData);
					Rectangle rectangle = default(Rectangle);
					GraphicsState transState;
					int num = 0;
					if (Global.state.MapEditMode)
						num = Global.cpd.Worldchip.Length;
					else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
						num = Global.cpd.Mapchip.Length;
					else
						num = Global.cpd.Layerchip.Length;
					for (int i = 0; i < num; i++)
					{
						if (Global.state.MapEditMode)
							chipsData = Global.cpd.Worldchip[i];
						else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
							chipsData = Global.cpd.Mapchip[i];
						else
							chipsData = Global.cpd.Layerchip[i];

						point = this.GetPosition(i);
						Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
						rectangle = new Rectangle(new Point(point.X * chipsize.Width, point.Y * chipsize.Height), chipsize);
						rectangle.Y -= this.vPosition;
						if (rectangle.Top > this.MainPanel.Height) break;

						if (rectangle.Bottom >= 0)
						{
							ChipData cschip = chipsData.GetCSChip();
							if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && cschip.xdbackgrnd)
							{
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
							}
							if (!Global.cpd.UseLayer || Global.state.EditingForeground)
							{
								e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
								transState = e.Graphics.Save();
								e.Graphics.TranslateTransform(rectangle.X, rectangle.Y);
								if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
								{
									e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, chipsize.Width, chipsize.Height);
								}
								else
								{
									Pen pen;
									SolidBrush brush;
									PointF[] vo_pa;
									double rad = 0;
									switch (cschip.name)
									{
										case "一方通行":
											if (cschip.description.Contains("表示なし")) break;
											pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
											if (cschip.description.Contains("右"))
												e.Graphics.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
											else if (cschip.description.Contains("左"))
												e.Graphics.DrawLine(pen, 1, 0, 1, chipsize.Height);
											else if (cschip.description.Contains("上"))
												e.Graphics.DrawLine(pen, 0, 1, chipsize.Width, 1);
											else if (cschip.description.Contains("下"))
												e.Graphics.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
											pen.Dispose();
											break;
										case "左右へ押せるドッスンスンのゴール":
											pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
											e.Graphics.TranslateTransform(1, 1);
											e.Graphics.DrawRectangle(pen, 0, 11, chipsize.Width - 1, chipsize.Height - 1 - 11);
											e.Graphics.TranslateTransform(-1, -1);
											e.Graphics.DrawLine(pen, 0, 11, chipsize.Width, chipsize.Height);
											e.Graphics.DrawLine(pen, 0, chipsize.Height, chipsize.Width, 11);
											pen.Dispose();
											break;
										case "シーソー":
											e.Graphics.TranslateTransform(16, 17);
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
											e.Graphics.FillPolygon(brush, vo_pa);
											vo_pa = new PointF[3];
											vo_pa[0].X = 0;
											vo_pa[0].Y = -2;
											vo_pa[1].X = -4;
											vo_pa[1].Y = 15;
											vo_pa[2].X = 4;
											vo_pa[2].Y = 15;
											brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillPolygon(brush, vo_pa);
											brush.Dispose();
											break;
										case "ブランコ":
											e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
												new Rectangle(0, 0, chipsize.Width, chipsize.Height),
												new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
											e.Graphics.TranslateTransform(16, -9);
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
											e.Graphics.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
											e.Graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
											e.Graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
											brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
											e.Graphics.FillPolygon(brush, vo_pa);
											pen.Dispose();
											brush.Dispose();
											break;
										case "動くＴ字型":
											e.Graphics.TranslateTransform(16, 37);
											rad = 270;
											vo_pa = new PointF[3];
											vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * chipsize.Width;
											vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * chipsize.Width;
											vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * chipsize.Width;
											vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * chipsize.Width;
											vo_pa[2].X = 0;
											vo_pa[2].Y = 0;
											brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillPolygon(brush, vo_pa);
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
											e.Graphics.FillPolygon(brush, vo_pa);
											brush.Dispose();
											break;
										case "ロープ":
										case "長いロープ":
										case "ゆれる棒":
											e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
												new Rectangle(0, 0, chipsize.Width, chipsize.Height),
												new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
											int length;
											if (cschip.name == "ロープ") length = 2;
											else length = 1;
											if (cschip.description == "つかまると左から動く")
											{
												e.Graphics.TranslateTransform(39, 5);
												rad = 168;
											}
											else
											{
												e.Graphics.TranslateTransform(16, -9);
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
											e.Graphics.FillPolygon(brush, vo_pa);
											brush.Dispose();
											break;
										case "人間大砲":
											if (cschip.description == "右向き") { rad = 330; e.Graphics.TranslateTransform(-9, 3); }
											else if (cschip.description == "左向き") { rad = 225; e.Graphics.TranslateTransform(9, 3); }
											else if (cschip.description == "天井") { rad = 30; e.Graphics.TranslateTransform(-9, 0); }
											else if (cschip.description == "右の壁") { rad = 270; e.Graphics.TranslateTransform(0, 9); }
											else if (cschip.description == "左の壁") { rad = 300; e.Graphics.TranslateTransform(0, 9); }
											brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
											e.Graphics.FillEllipse(brush, 16 - 7, 16 - 7, 14, 14);
											vo_pa = new PointF[4];
											vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
											vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
											vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
											vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
											vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
											vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
											vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
											vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
											e.Graphics.FillPolygon(brush, vo_pa);
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
											e.Graphics.FillPolygon(brush, vo_pa);
											brush.Dispose();
											break;
										case "曲線による上り坂":
										case "曲線による下り坂":
											var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
											if (cschip.description.Contains("線のみ"))
											{
												vo_pa = new PointF[11];
												pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
												for (var i1 = 0; i1 <= 50; i1 += 5)
												{
													if (cschip.name.Contains("上"))
														vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													else if (cschip.name.Contains("下"))
														vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * 3.1415926535897931) / 180) * chipsize.Height * 5 / 8) - 1;
													if (i1 == 50)
													{
														j20 = vo_pa[k21].X;
														k20 = vo_pa[k21].Y;
													}
													k21++;
												}

												e.Graphics.DrawLines(pen, vo_pa);
												k21 = 0;
												for (var i1 = 0; i1 <= 50; i1 += 5)
												{
													if (cschip.name.Contains("上"))
														vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													else if (cschip.name.Contains("下"))
														vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * 3.1415926535897931) / 180) * chipsize.Height * 5 / 8) + 1;
													if (i1 == 50)
													{
														l20 = vo_pa[k21].X;
														i21 = vo_pa[k21].Y;
													}
													k21++;
												}
												e.Graphics.DrawLines(pen, vo_pa);
												vo_pa = new PointF[2];
												vo_pa[0].X = j20;
												vo_pa[0].Y = k20;
												vo_pa[1].X = l20;
												vo_pa[1].Y = i21;
												e.Graphics.DrawLines(pen, vo_pa);
												pen.Dispose();
											}
											else
											{
												vo_pa = new PointF[13];
												brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
												for (var i1 = 0; i1 <= 50; i1 += 5)
												{
													if (cschip.name.Contains("上"))
														vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													else if (cschip.name.Contains("下"))
														vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * 3.1415926535897931) / 180) * chipsize.Height * 5 / 8);
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
												e.Graphics.FillPolygon(brush, vo_pa);
												vo_pa = new PointF[13];
												k21 = 0;
												for (var i1 = 0; i1 <= 50; i1 += 5)
												{
													if (cschip.name.Contains("上"))
														vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													else if (cschip.name.Contains("下"))
														vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * chipsize.Width * 5 / 8);
													vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * 3.1415926535897931) / 180) * chipsize.Height * 5 / 8);
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
												e.Graphics.FillPolygon(brush, vo_pa);
												vo_pa = new PointF[4];
												vo_pa[0].X = j20;
												vo_pa[0].Y = k20;
												vo_pa[1].X = l20;
												vo_pa[1].Y = i21;
												vo_pa[2].X = l20;
												vo_pa[2].Y = chipsize.Height;
												vo_pa[3].X = j20;
												vo_pa[3].Y = chipsize.Height;
												e.Graphics.FillPolygon(brush, vo_pa);
												brush.Dispose();
											}
											break;
										case "乗れる円":
										case "跳ねる円":
										case "円":
											int radius = default;
											e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
											if (cschip.name == "円")
											{
												brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
												if (cschip.description.Contains("乗ると下がる"))
												{
													e.Graphics.TranslateTransform(0, 3);
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
											e.Graphics.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
											brush.Dispose();
											break;
										case "半円":
											e.Graphics.TranslateTransform(1, 2);
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
														vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
														vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
														j21++;
													}
													e.Graphics.DrawLines(pen, vo_pa);
													j21 = 0;
													for (var k2 = 90; k2 >= 40; k2 -= 5)
													{
														vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
														vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
														j21++;
													}
													vo_pa[j21].X = 240 / 8;
													vo_pa[j21].Y = 63 / 8;
													e.Graphics.DrawLines(pen, vo_pa);
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
														vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
														vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
														j21++;
													}

													vo_pa[j21].X = 120 / 8;
													vo_pa[j21].Y = 64 / 8;
													e.Graphics.FillPolygon(brush, vo_pa);
													j21 = 0;
													for (var k2 = 90; k2 >= 40; k2 -= 5)
													{
														vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
														vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
														j21++;
													}

													vo_pa[j21].X = 240 / 8;
													vo_pa[j21].Y = 64 / 8;
													j21++;
													vo_pa[j21].X = 120 / 8;
													vo_pa[j21].Y = 64 / 8;
													e.Graphics.FillPolygon(brush, vo_pa);
													brush.Dispose();
												}
											}
											else
											{
												e.Graphics.TranslateTransform(0, 10);
												brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
													e.Graphics.FillRectangle(brush, (120 - 20) / 8, 64 / 8, 40 / 8, 12);
												brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
												vo_pa = new PointF[13];
												var j21 = 0;
												vo_pa[j21].X = 0;
												vo_pa[j21].Y = 64 / 8;
												j21++;
												for (var j = 140; j >= 90; j -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
													j21++;
												}

												vo_pa[j21].X = 120 / 8;
												vo_pa[j21].Y = 64 / 8;
												e.Graphics.FillPolygon(brush, vo_pa);
												j21 = 0;
												for (var k2 = 90; k2 >= 40; k2 -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
													j21++;
												}

												vo_pa[j21].X = 240 / 8;
												vo_pa[j21].Y = 64 / 8;
												j21++;
												vo_pa[j21].X = 120 / 8;
												vo_pa[j21].Y = 64 / 8;
												e.Graphics.FillPolygon(brush, vo_pa);
												brush.Dispose();
											}
											break;
										default:
											e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
											e.Graphics.RotateTransform(cschip.rotate);

											// 水の半透明処理
											if(Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && chipsData.character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
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
											else e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? chipsize : cschip.size), GraphicsUnit.Pixel);
											break;
									}
								}
								e.Graphics.Restore(transState);
							}
							else
							{
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawLayerOrig, rectangle, new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? chipsize : cschip.size), GraphicsUnit.Pixel);
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
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(p, chipsize), GraphicsUnit.Pixel);
							}
							else if (Global.config.draw.ExtendDraw && cschip.xdraw != default(Point) && !cschip.xdbackgrnd)
							{
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(cschip.xdraw, chipsize), GraphicsUnit.Pixel);
							}
							e.Graphics.PixelOffsetMode = default;
							if (Global.state.CurrentChip.character == chipsData.character)
							{
								if (i != this.selectedIndex) this.selectedIndex = i;
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
					if (!base.Enabled)
					{
						using (Brush brush3 = new SolidBrush(Color.FromArgb(160, Color.White)))
						{
							e.Graphics.FillRectangle(brush3, e.ClipRectangle);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000076F8 File Offset: 0x000058F8
		private void MainPanel_MouseDown(object sender, MouseEventArgs e)
		{
			base.Focus();
			if (!base.Enabled)
			{
				return;
			}
			int hMaxChip = this.hMaxChip;
			if (e.X > hMaxChip * Global.cpd.runtime.Definitions.ChipSize.Width)
			{
				return;
			}
			int num = (int)Math.Floor((double)e.X / (double)Global.cpd.runtime.Definitions.ChipSize.Width);
			num += (int)Math.Floor((double)(e.Y + this.vPosition) / (double)Global.cpd.runtime.Definitions.ChipSize.Height) * hMaxChip;
			int num2;
			if (Global.state.MapEditMode)
			{
				num2 = Global.cpd.Worldchip.Length;
			}
			else if (!Global.cpd.UseLayer || Global.state.EditingForeground)
			{
				num2 = Global.cpd.Mapchip.Length;
			}
			else
			{
				num2 = Global.cpd.Layerchip.Length;
			}
			if (num >= num2)
			{
				return;
			}
			this.SelectedIndex = num;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000022FD File Offset: 0x000004FD
		private void MainPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.MainPanel_MouseDown(sender, e);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000077FC File Offset: 0x000059FC
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

		// Token: 0x06000036 RID: 54 RVA: 0x00007834 File Offset: 0x00005A34
		private void GUIChipList_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			switch (e.KeyCode)
			{
			case Keys.Left:
				this.SelectedIndex--;
				return;
			case Keys.Up:
				this.SelectedIndex -= this.hMaxChip;
				return;
			case Keys.Right:
				this.SelectedIndex++;
				return;
			case Keys.Down:
				this.SelectedIndex += this.hMaxChip;
				return;
			default:
				e.Handled = false;
				return;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002314 File Offset: 0x00000514
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000078B8 File Offset: 0x00005AB8
		private void InitializeComponent()
		{
			this.vScr = new VScrollBar();
			this.MainPanel = new PictureBox();
			((ISupportInitialize)this.MainPanel).BeginInit();
			base.SuspendLayout();
			this.vScr.Dock = DockStyle.Right;
			this.vScr.Location = new Point(265, 0);
			this.vScr.Name = "vScr";
			this.vScr.Size = new Size(20, 264);
			this.vScr.TabIndex = 0;
			this.vScr.Scroll += this.vScr_Scroll;
			this.MainPanel.Dock = DockStyle.Fill;
			this.MainPanel.Location = new Point(0, 0);
			this.MainPanel.Name = "MainPanel";
			this.MainPanel.Size = new Size(265, 264);
			this.MainPanel.TabIndex = 1;
			this.MainPanel.TabStop = false;
			this.MainPanel.MouseMove += this.MainPanel_MouseMove;
			this.MainPanel.MouseDown += this.MainPanel_MouseDown;
			this.MainPanel.Paint += this.MainPanel_Paint;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.MainPanel);
			base.Controls.Add(this.vScr);
			base.Name = "GUIChipList";
			base.Size = new Size(285, 264);
			base.PreviewKeyDown += this.GUIChipList_PreviewKeyDown;
			base.Resize += this.GUIChipList_Resize;
			base.KeyDown += this.GUIChipList_KeyDown;
			((ISupportInitialize)this.MainPanel).EndInit();
			base.ResumeLayout(false);
		}

		// Token: 0x04000033 RID: 51
		private int selectedIndex;

		// Token: 0x04000034 RID: 52
		private IContainer components;

		// Token: 0x04000035 RID: 53
		private VScrollBar vScr;

		// Token: 0x04000036 RID: 54
		private PictureBox MainPanel;
	}
}

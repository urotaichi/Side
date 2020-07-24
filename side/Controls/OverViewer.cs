using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	// Token: 0x02000034 RID: 52
	public class OverViewer : Control
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x000033BC File Offset: 0x000015BC
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000033DB File Offset: 0x000015DB
		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.Paint += this.OverViewer_Paint;
			base.ResumeLayout(false);
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x00025C94 File Offset: 0x00023E94
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x000033FC File Offset: 0x000015FC
		public new Size Size
		{
			get
			{
				if (Global.cpd.runtime == null)
				{
					return new Size(180, 30);
				}
				return new Size(this.Source.Size.Width * this.ppb, this.Source.Size.Height * this.ppb);
			}
			set
			{
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00025CF4 File Offset: 0x00023EF4
		public OverViewer()
		{
			this.InitializeComponent();
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.CreateDrawSource();
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00025D44 File Offset: 0x00023F44
		public void CreateDrawSource()
		{
			if (Global.cpd.runtime == null)
			{
				return;
			}
			this.Source = new Bitmap(Global.cpd.runtime.Definitions.StageSize.x, Global.cpd.runtime.Definitions.StageSize.y);
			this.UpdateDrawSource();
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00025DA4 File Offset: 0x00023FA4
		public unsafe void UpdateDrawSource()
		{
			BitmapData bitmapData = this.Source.LockBits(new Rectangle(new Point(0, 0), this.Source.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			try
			{
				byte* ptr = null;
				int num = 765 / Global.cpd.Mapchip.Length;
				Color color = default(Color);
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
				if (!Global.state.EditingForeground)
				{
					dictionary = Global.MainWnd.MainDesigner.DrawLayerRef;
				}
				else if (Global.state.MapEditMode)
				{
					dictionary = Global.MainWnd.MainDesigner.DrawWorldRef;
				}
				for (int i = 0; i < Global.cpd.runtime.Definitions.StageSize.x; i++)
				{
					for (int j = 0; j < Global.cpd.runtime.Definitions.StageSize.y; j++)
					{
						ptr = (byte*)((void*)bitmapData.Scan0);
						int num2 = i * 3 + bitmapData.Stride * j;
						string key = Global.cpd.project.StageData[j].Substring(i * Global.cpd.runtime.Definitions.StageSize.bytesize, Global.cpd.runtime.Definitions.StageSize.bytesize);
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
			finally
			{
				this.Source.UnlockBits(bitmapData);
			}
			this.Refresh();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00026014 File Offset: 0x00024214
		private void OverViewer_Paint(object sender, PaintEventArgs e)
		{
			if (this.Source == null)
			{
				e.Graphics.FillRectangle(Brushes.Black, new Rectangle(default(Point), this.Size));
				return;
			}
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.DrawImage(this.Source, new Rectangle(0, 0, this.Source.Width * this.ppb, this.Source.Height * this.ppb));
			Point mapPointTranslatedMap = Global.state.MapPointTranslatedMap;
			mapPointTranslatedMap.X *= this.ppb;
			mapPointTranslatedMap.Y *= this.ppb;

			// 白枠
			using (Pen pen = new Pen(Brushes.White, (float)this.ppb))
			{
				e.Graphics.DrawRectangle(pen, new Rectangle(mapPointTranslatedMap,
					new Size(
						(int)((double)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * this.ppb) / Global.config.draw.ZoomIndex),
					(int)((double)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * this.ppb) / Global.config.draw.ZoomIndex)
					)
					));
			}
		}

		// Token: 0x04000287 RID: 647
		private IContainer components;

		// Token: 0x04000288 RID: 648
		private Bitmap Source;

		// Token: 0x04000289 RID: 649
		public int ppb = 2;
	}
}

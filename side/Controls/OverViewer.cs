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
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.Paint += this.OverViewer_Paint;
			base.ResumeLayout(false);
		}

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

		public OverViewer()
		{
			this.InitializeComponent();
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.CreateDrawSource();
		}

		public void CreateDrawSource()
		{
			if (Global.cpd.runtime == null)
			{
				return;
			}
			this.Source = new Bitmap(Global.cpd.runtime.Definitions.StageSize.x, Global.cpd.runtime.Definitions.StageSize.y);
			this.UpdateDrawSource();
		}

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
			using (Pen pen = new Pen(Brushes.White, (float)this.ppb))
			{
				e.Graphics.DrawRectangle(pen, new Rectangle(mapPointTranslatedMap, new Size((int)((double)(Global.MainWnd.MainDesigner.Size.Width / Global.cpd.runtime.Definitions.ChipSize.Width * this.ppb) / Global.config.draw.ZoomIndex), (int)((double)(Global.MainWnd.MainDesigner.Size.Height / Global.cpd.runtime.Definitions.ChipSize.Height * this.ppb) / Global.config.draw.ZoomIndex))));
			}
		}

		private IContainer components;

		private Bitmap Source;

		public int ppb = 2;
	}
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MasaoPlus
{
	// Token: 0x02000013 RID: 19
	public static class DrawEx
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x00011DA4 File Offset: 0x0000FFA4
		public static Bitmap MakeMask(Image img)
		{
			Bitmap bitmap = new Bitmap(img.Width, img.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(img, 0, 0);
				BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				int[] array = new int[bitmap.Size.Width * bitmap.Size.Height];
				Marshal.Copy(bitmapData.Scan0, array, 0, bitmap.Size.Width * bitmap.Size.Height);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != -1)
					{
						array[i] = -16777216;
					}
				}
				Marshal.Copy(array, 0, bitmapData.Scan0, bitmap.Size.Width * bitmap.Size.Height);
				bitmap.UnlockBits(bitmapData);
			}
			return bitmap;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00011EC8 File Offset: 0x000100C8
		public static void DrawGridEx(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor)
		{
			TextureBrush textureBrush = null;
			bool flag = false;
			Size size = default(Size);
			bool flag2 = (double)backColor.GetBrightness() < 0.5;
			lock (typeof(DrawEx))
			{
				if (textureBrush == null || size.Width != pixelsBetweenDots.Width || size.Height != pixelsBetweenDots.Height || flag2 != flag)
				{
					if (textureBrush != null)
					{
						textureBrush.Dispose();
						textureBrush = null;
					}
					int num = 16;
					Color color = flag2 ? Color.White : Color.Black;
					int num2 = area.X % pixelsBetweenDots.Width;
					int num3 = area.Y % pixelsBetweenDots.Height;
					int num4 = (num / pixelsBetweenDots.Width + 1) * pixelsBetweenDots.Width;
					int num5 = (num / pixelsBetweenDots.Height + 1) * pixelsBetweenDots.Height;
					Bitmap bitmap = new Bitmap(num4, num5);
					for (int i = 0; i < num4; i += pixelsBetweenDots.Width)
					{
						for (int j = 0; j < num5; j += pixelsBetweenDots.Height)
						{
							bitmap.SetPixel(i, j, color);
						}
					}
					textureBrush = new TextureBrush(bitmap);
					textureBrush.TranslateTransform((float)num2, (float)num3);
					bitmap.Dispose();
				}
			}
			graphics.FillRectangle(textureBrush, area);
			textureBrush.Dispose();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0001202C File Offset: 0x0001022C
		public static Color GetForegroundColor(Color c)
		{
			int red = DrawEx.plus128((int)c.R);
			int green = DrawEx.plus128((int)c.G);
			int blue = DrawEx.plus128((int)c.B);
			return Color.FromArgb(red, green, blue);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000278D File Offset: 0x0000098D
		private static int plus128(int n)
		{
			n += 128;
			if (n > 255)
			{
				n -= 256;
			}
			return n;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00012068 File Offset: 0x00010268
		public static double ToHalfAdjust(double dValue, int iDigits)
		{
			double num = Math.Pow(10.0, (double)iDigits);
			if (dValue <= 0.0)
			{
				return Math.Ceiling(dValue * num - 0.5) / num;
			}
			return Math.Floor(dValue * num + 0.5) / num;
		}
	}
}

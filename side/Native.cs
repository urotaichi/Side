using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MasaoPlus
{
	// Token: 0x0200003B RID: 59
	internal static class Native
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0002A190 File Offset: 0x00028390
		public static Rectangle GetNormalWindowLocation(Form form)
		{
			Native.USER32.WINDOWPLACEMENT windowplacement = default(Native.USER32.WINDOWPLACEMENT);
			windowplacement.Length = Marshal.SizeOf(windowplacement);
			Native.USER32.GetWindowPlacement((int)form.Handle, ref windowplacement);
			return new Rectangle(windowplacement.rcNormalPosition.left, windowplacement.rcNormalPosition.top, windowplacement.rcNormalPosition.right - windowplacement.rcNormalPosition.left, windowplacement.rcNormalPosition.bottom - windowplacement.rcNormalPosition.top);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0002A21C File Offset: 0x0002841C
		public static void SetSideFileRelate()
		{
			Native.SetFileRelate(Global.definition.ProjExt, Global.definition.ProjFileType, Global.definition.ProjFileDescription);
			Native.SetFileRelate(Global.definition.RuntimeArchiveExt, Global.definition.RuntimeFileType, Global.definition.RuntimeFileDescription);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0002A270 File Offset: 0x00028470
		public static void SetFileRelate(string ext, string FileType, string Description)
		{
			if (!Native.CheckRegistryAddmittion())
			{
				MessageBox.Show("関連付けする権限がありません。", "関連付け失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				string value = "\"" + Application.ExecutablePath + "\" %1";
				string text = Global.definition.AppName + " " + FileType;
				string str = "open";
				string value2 = Global.definition.AppName + "で開く(&O)";
				string executablePath = Application.ExecutablePath;
				int num = 0;
				RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(ext);
				registryKey.SetValue("", text);
				registryKey.Close();
				RegistryKey registryKey2 = Registry.ClassesRoot.CreateSubKey(text);
				registryKey2.SetValue("", Description);
				registryKey2 = registryKey2.CreateSubKey("shell\\" + str);
				registryKey2.SetValue("", value2);
				registryKey2 = registryKey2.CreateSubKey("command");
				registryKey2.SetValue("", value);
				registryKey2.Close();
				RegistryKey registryKey3 = Registry.ClassesRoot.CreateSubKey(text + "\\DefaultIcon");
				registryKey3.SetValue("", executablePath + "," + num.ToString());
				registryKey3.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("関連付けに失敗しました。" + Environment.NewLine + ex.Message, "関連付け処理失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00003796 File Offset: 0x00001996
		public static void DeleteSideFileRelate()
		{
			Native.DeleteFileRelate(Global.definition.ProjExt, Global.definition.ProjFileType);
			Native.DeleteFileRelate(Global.definition.RuntimeArchiveExt, Global.definition.RuntimeFileType);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0002A3F4 File Offset: 0x000285F4
		public static void DeleteFileRelate(string ext, string FileType)
		{
			if (!Native.CheckRegistryAddmittion())
			{
				MessageBox.Show("関連付けする権限がありません。", "関連付け失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			string subkey = Global.definition.AppName + " " + FileType;
			try
			{
				Registry.ClassesRoot.DeleteSubKeyTree(ext);
				Registry.ClassesRoot.DeleteSubKeyTree(subkey);
			}
			catch (Exception ex)
			{
				MessageBox.Show("関連付けの解除に失敗しました。" + Environment.NewLine + ex.Message, "関連付け処理失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0002A484 File Offset: 0x00028684
		public static bool CheckRegistryAddmittion()
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
			return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator) || windowsPrincipal.IsInRole(WindowsBuiltInRole.PowerUser);
		}

		// Token: 0x0200003C RID: 60
		public static class USER32
		{
			// Token: 0x06000258 RID: 600
			[DllImport("user32.dll")]
			public static extern bool GetWindowPlacement(int hWnd, ref Native.USER32.WINDOWPLACEMENT lpwndpl);

			// Token: 0x06000259 RID: 601
			[DllImport("user32.dll")]
			public static extern IntPtr GetDC(IntPtr hWnd);

			// Token: 0x0600025A RID: 602
			[DllImport("user32.dll")]
			public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

			// Token: 0x0600025B RID: 603
			[DllImport("user32.dll")]
			public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

			// Token: 0x040002BE RID: 702
			public const uint EM_GETLINECOUNT = 186U;

			// Token: 0x040002BF RID: 703
			public const uint EM_LINEINDEX = 187U;

			// Token: 0x040002C0 RID: 704
			public const uint EM_LINEFROMCHAR = 201U;

			// Token: 0x040002C1 RID: 705
			public const uint EM_GETFIRSTVISIBLELINE = 206U;

			// Token: 0x0200003D RID: 61
			public struct POINT
			{
				// Token: 0x040002C2 RID: 706
				public int x;

				// Token: 0x040002C3 RID: 707
				public int y;
			}

			// Token: 0x0200003E RID: 62
			public struct RECT
			{
				// Token: 0x040002C4 RID: 708
				public int left;

				// Token: 0x040002C5 RID: 709
				public int top;

				// Token: 0x040002C6 RID: 710
				public int right;

				// Token: 0x040002C7 RID: 711
				public int bottom;
			}

			// Token: 0x0200003F RID: 63
			public struct WINDOWPLACEMENT
			{
				// Token: 0x040002C8 RID: 712
				public int Length;

				// Token: 0x040002C9 RID: 713
				public int flags;

				// Token: 0x040002CA RID: 714
				public int showCmd;

				// Token: 0x040002CB RID: 715
				public Native.USER32.POINT ptMinPosition;

				// Token: 0x040002CC RID: 716
				public Native.USER32.POINT ptMaxPosition;

				// Token: 0x040002CD RID: 717
				public Native.USER32.RECT rcNormalPosition;
			}
		}

		// Token: 0x02000040 RID: 64
		public static class GDI32
		{
			// Token: 0x0600025C RID: 604
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);

			// Token: 0x0600025D RID: 605
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

			// Token: 0x0600025E RID: 606
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

			// Token: 0x0600025F RID: 607
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

			// Token: 0x06000260 RID: 608
			[DllImport("gdi32.dll")]
			public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDst, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rasterOp);

			// Token: 0x06000261 RID: 609
			[DllImport("gdi32.dll")]
			public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, int rasterOp);

			// Token: 0x06000262 RID: 610
			[DllImport("gdi32.dll")]
			public static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, int dwRop);

			// Token: 0x06000263 RID: 611
			[DllImport("gdi32.dll")]
			public static extern IntPtr DeleteDC(IntPtr hDC);

			// Token: 0x06000264 RID: 612
			[DllImport("gdi32.dll")]
			public static extern IntPtr DeleteObject(IntPtr hObj);

			// Token: 0x06000265 RID: 613
			[DllImport("gdi32.dll")]
			public static extern bool SetDIBits(IntPtr hdc, IntPtr hBitmap, uint uStartScan, uint cScanLines, IntPtr lpvBits, IntPtr lpbmi, uint fuColorUse);

			// Token: 0x06000266 RID: 614
			[DllImport("gdi32.dll")]
			public static extern bool StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, IntPtr lpBits, IntPtr lpBitsInfo, uint iUsage, uint dwRop);

			// Token: 0x040002CE RID: 718
			public const int SRCCOPY = 13369376;

			// Token: 0x040002CF RID: 719
			public const int SRCAND = 8913094;

			// Token: 0x040002D0 RID: 720
			public const int SRCPAINT = 15597702;

			// Token: 0x040002D1 RID: 721
			public const int SRCINVERT = 6684742;

			// Token: 0x040002D2 RID: 722
			public const int NOTSRCCOPY = 3342344;

			// Token: 0x040002D3 RID: 723
			public const int DIB_RGB_COLORS = 0;

			// Token: 0x02000041 RID: 65
			public struct BITMAPINFOHEADER
			{
				// Token: 0x040002D4 RID: 724
				public uint biSize;

				// Token: 0x040002D5 RID: 725
				public int biWidth;

				// Token: 0x040002D6 RID: 726
				public int biHeight;

				// Token: 0x040002D7 RID: 727
				public ushort biPlanes;

				// Token: 0x040002D8 RID: 728
				public ushort biBitCount;

				// Token: 0x040002D9 RID: 729
				public uint biCompression;

				// Token: 0x040002DA RID: 730
				public uint biSizeImage;

				// Token: 0x040002DB RID: 731
				public int biXPelsPerMeter;

				// Token: 0x040002DC RID: 732
				public int biYPelsPerMeter;

				// Token: 0x040002DD RID: 733
				public uint biClrUsed;

				// Token: 0x040002DE RID: 734
				public uint biClrImportant;
			}
		}
	}
}

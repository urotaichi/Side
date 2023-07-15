using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MasaoPlus
{
    internal static class Native
    {
        public static Rectangle GetNormalWindowLocation(Form form)
        {
            USER32.WINDOWPLACEMENT windowplacement = default;
            windowplacement.Length = Marshal.SizeOf(windowplacement);
            USER32.GetWindowPlacement((int)form.Handle, ref windowplacement);
            return new Rectangle(windowplacement.rcNormalPosition.left, windowplacement.rcNormalPosition.top, windowplacement.rcNormalPosition.right - windowplacement.rcNormalPosition.left, windowplacement.rcNormalPosition.bottom - windowplacement.rcNormalPosition.top);
        }

        public static void SetSideFileRelate()
        {
            SetFileRelate(Global.definition.ProjExt, Global.definition.ProjFileType, Global.definition.ProjFileDescription);
            SetFileRelate(Global.definition.RuntimeArchiveExt, Global.definition.RuntimeFileType, Global.definition.RuntimeFileDescription);
        }

        public static void SetFileRelate(string ext, string FileType, string Description)
        {
            if (!CheckRegistryAddmittion())
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

        public static void DeleteSideFileRelate()
        {
            DeleteFileRelate(Global.definition.ProjExt, Global.definition.ProjFileType);
            DeleteFileRelate(Global.definition.RuntimeArchiveExt, Global.definition.RuntimeFileType);
        }

        public static void DeleteFileRelate(string ext, string FileType)
        {
            if (!CheckRegistryAddmittion())
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

        public static bool CheckRegistryAddmittion()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator) || windowsPrincipal.IsInRole(WindowsBuiltInRole.PowerUser);
        }

        public static class USER32
        {
            [DllImport("user32.dll")]
            public static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

            public const uint EM_GETLINECOUNT = 186U;

            public const uint EM_LINEINDEX = 187U;

            public const uint EM_LINEFROMCHAR = 201U;

            public const uint EM_GETFIRSTVISIBLELINE = 206U;

            public struct POINT
            {
                public int x;

                public int y;
            }

            public struct RECT
            {
                public int left;

                public int top;

                public int right;

                public int bottom;
            }

            public struct WINDOWPLACEMENT
            {
                public int Length;

                public int flags;

                public int showCmd;

                public POINT ptMinPosition;

                public POINT ptMaxPosition;

                public RECT rcNormalPosition;
            }
        }

        public static class GDI32
        {
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDst, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rasterOp);

            [DllImport("gdi32.dll")]
            public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, int rasterOp);

            [DllImport("gdi32.dll")]
            public static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern IntPtr DeleteObject(IntPtr hObj);

            [DllImport("gdi32.dll")]
            public static extern bool SetDIBits(IntPtr hdc, IntPtr hBitmap, uint uStartScan, uint cScanLines, IntPtr lpvBits, IntPtr lpbmi, uint fuColorUse);

            [DllImport("gdi32.dll")]
            public static extern bool StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, IntPtr lpBits, IntPtr lpBitsInfo, uint iUsage, uint dwRop);

            public const int SRCCOPY = 13369376;

            public const int SRCAND = 8913094;

            public const int SRCPAINT = 15597702;

            public const int SRCINVERT = 6684742;

            public const int NOTSRCCOPY = 3342344;

            public const int DIB_RGB_COLORS = 0;

            public struct BITMAPINFOHEADER
            {
                public uint biSize;

                public int biWidth;

                public int biHeight;

                public ushort biPlanes;

                public ushort biBitCount;

                public uint biCompression;

                public uint biSizeImage;

                public int biXPelsPerMeter;

                public int biYPelsPerMeter;

                public uint biClrUsed;

                public uint biClrImportant;
            }
        }
    }
}

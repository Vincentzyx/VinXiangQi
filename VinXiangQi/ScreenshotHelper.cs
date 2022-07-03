using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace VinXiangQi
{
    class ScreenshotHelper
    {
        #region GetWindowCapture的dll引用
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rectangle rect);
        [DllImport("user32.dll")]
        private static extern IntPtr GetClientRect(IntPtr hWnd, ref Rectangle rect);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(
         IntPtr hdc // handle to DC
         );
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(
         IntPtr hdc,         // handle to DC
         int nWidth,      // width of bitmap, in pixels
         int nHeight      // height of bitmap, in pixels
         );
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(
         IntPtr hdc,           // handle to DC
         IntPtr hgdiobj    // handle to object
         );
        [DllImport("gdi32.dll")]
        private static extern int DeleteDC(
         IntPtr hdc           // handle to DC
         );
        [DllImport("gdi32.dll")]
        private static extern int DeleteObject(
 IntPtr bitmap       // handle to DC
 );
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(
         IntPtr hwnd,                // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,              // HDC to print into,Handle to the device context.
         UInt32 nFlags               // Optional flags,Specifies the drawing options. It can be one of the following values.
         );
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );
        #endregion

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        // findwindowex

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);


        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Flags]
        private enum DwmWindowAttribute : uint
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        }

        public static Rectangle GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect;
            int size = Marshal.SizeOf(typeof(RECT));
            DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

            return new Rectangle(rect.Left, rect.Top, (int)((rect.Right - rect.Left) * Mainform.Settings.ScaleFactor), (int)((rect.Bottom - rect.Top) * Mainform.Settings.ScaleFactor));
        }

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        public static float GetScalingFactor(IntPtr handle)
        {
            Graphics g = Graphics.FromHwnd(handle);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

        public static Rectangle GetWindowRectangleWithShadow(IntPtr hWnd)
        {
            float scaleFactor = Mainform.Settings.ScaleFactor; // GetScalingFactor(hWnd);
            Rectangle windowRect = new Rectangle();
            GetWindowRect(hWnd, ref windowRect);
            int width = Math.Abs(windowRect.X - windowRect.Width);
            int height = Math.Abs(windowRect.Y - windowRect.Height);
            width = (int)(scaleFactor * width);
            height = (int)(scaleFactor * height);
            windowRect.Width = width;
            windowRect.Height = height;
            return windowRect;
        }

        public static Rectangle GetWindowRectWithoutTitle(IntPtr hWnd)
        {
            float scaleFactor = Mainform.Settings.ScaleFactor; // GetScalingFactor(hWnd);
            Rectangle windowRect = GetWindowRectangle(hWnd);
            Rectangle clientRect = new Rectangle();
            GetClientRect(hWnd, ref clientRect);
            int clientWidth = clientRect.Width - clientRect.X;
            int clientHeight = clientRect.Height - clientRect.Y;
            windowRect.X = windowRect.X + windowRect.Width - clientWidth;
            windowRect.Y = windowRect.Y + windowRect.Height - clientHeight;
            windowRect.Width = clientWidth;
            windowRect.Height = clientHeight;
            windowRect.Width = (int)(scaleFactor * windowRect.Width);
            windowRect.Height = (int)(scaleFactor * windowRect.Height);
            return windowRect;
        }

        public static Bitmap GetWindowCapture(IntPtr hWnd)
        {
            IntPtr hscrdc = GetWindowDC(hWnd);
            Rectangle windowRect = GetWindowRectangleWithShadow(hWnd);
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, windowRect.Width, windowRect.Height);
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = Image.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);//删除用过的对象
            DeleteDC(hmemdc);//删除用过的对象
            DeleteObject(hbitmap);
            return bmp;
            //Rectangle clientRect = GetWindowRectWithoutTitle(hWnd);
            //int borderSize = (windowRect.Width - clientRect.Width) / 2;
            //int titleBarSize = (windowRect.Height - clientRect.Height) - borderSize;
            //return bmp.Clone(new Rectangle(borderSize, titleBarSize, bmp.Width - 2 * borderSize, bmp.Height - titleBarSize - borderSize), bmp.PixelFormat);
        }
    }
}

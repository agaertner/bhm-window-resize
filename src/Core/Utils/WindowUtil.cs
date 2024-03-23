using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Nekres.WindowResize.Core {
    internal static class WindowUtil {

        internal enum WindowSize {
            [EnumMember(Value = "1024x768")]
            _1024x768,
            [EnumMember(Value = "1152x864")]
            _1152x864,
            [EnumMember(Value = "1176x664")]
            _1176x664,
            [EnumMember(Value = "1280x720")]
            _1280x720,
            [EnumMember(Value = "1280x768")]
            _1280x768,
            [EnumMember(Value = "1280x800")]
            _1280x800,
            [EnumMember(Value = "1280x960")]
            _1280x960,
            [EnumMember(Value = "1280x1024")]
            _1280x1024,
            [EnumMember(Value = "1360x768")]
            _1360x768,
            [EnumMember(Value = "1366x768")]
            _1366x768,
            [EnumMember(Value = "1440x1080")]
            _1440x1080,
            [EnumMember(Value = "1600x900")]
            _1600x900,
            [EnumMember(Value = "1600x1024")]
            _1600x1024,
            [EnumMember(Value = "1600x1200")]
            _1600x1200,
            [EnumMember(Value = "1680x1050")]
            _1680x1050,
            [EnumMember(Value = "1720x1440")]
            _1720x1440,
            [EnumMember(Value = "1920x1080")]
            _1920x1080,
            [EnumMember(Value = "1920x1200")]
            _1920x1200,
            [EnumMember(Value = "1920x1440")]
            _1920x1440,
            [EnumMember(Value = "2048x1152")]
            _2048x1152,
            [EnumMember(Value = "2560x1440")]
            _2560x1440,
            [EnumMember(Value = "3440x1440")]
            _3440x1440
        }

        private const int SWP_NOSIZE       = 0x0001;
        private const int SWP_NOMOVE       = 0x0002;
        private const int SWP_NOZORDER     = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int GWL_STYLE        = -16;
        private const int WS_POPUP         = unchecked((int)0x80000000);
        private const int WS_MAXIMIZE      = 0x01000000;
        private const int WS_BORDER        = 0x00800000;
        public const  int WS_DLGFRAME      = 0x00400000;
        public const  int WS_CAPTION       = WS_BORDER | WS_DLGFRAME;
        private const int WS_OVERLAPPED    = 0x00000000;
        private const int WS_SYSMENU       = 0x00080000;
        private const int WS_THICKFRAME    = 0x00040000;
        private const int WS_MINIMIZEBOX   = 0x00020000;
        private const int WS_MAXIMIZEBOX   = 0x00010000;
        private const int WS_SIZEBOX       = 0x00040000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static bool IsWindowedMode(IntPtr hWnd) {
            if (hWnd == IntPtr.Zero) {
                throw new ArgumentException($"{nameof(hWnd)} was zero.");
            }

            int style = GetWindowLong(hWnd, GWL_STYLE);

            return (style & WS_POPUP) == 0 && (style & WS_BORDER) != 0;
        }

        public static void ResizeAndCenterWindow(IntPtr hWnd, int width, int height) {
            var screen = Screen.FromHandle(hWnd);
            int x      = (int)Math.Round((screen.Bounds.Width  - width)  / 2.0);
            int y      = (int)Math.Round((screen.Bounds.Height - height) / 2.0);
            SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        public static Size ParseSize(WindowSize size) {
            var str = size.ToString().TrimStart('_').Split('x');
            return new Size(int.Parse(str[0]), int.Parse(str[1]));
        }

        public static void RemoveBorder(IntPtr hWnd) {
            if (hWnd == IntPtr.Zero) {
                return;
            }

            int currentStyle = GetWindowLong(hWnd, GWL_STYLE);
            int newStyle     = currentStyle & ~(WS_CAPTION | WS_SIZEBOX | WS_OVERLAPPED | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_BORDER | WS_THICKFRAME);
            SetWindowLong(hWnd, GWL_STYLE, newStyle);
        }

        public static void AddBorder(IntPtr hWnd) {
            if (hWnd == IntPtr.Zero) {
                return;
            }
            int currentStyle = GetWindowLong(hWnd, GWL_STYLE);
            int newStyle     = currentStyle | WS_CAPTION | WS_SIZEBOX | WS_OVERLAPPED | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_BORDER | WS_THICKFRAME;
            SetWindowLong(hWnd, GWL_STYLE, newStyle);
        }
    }
}

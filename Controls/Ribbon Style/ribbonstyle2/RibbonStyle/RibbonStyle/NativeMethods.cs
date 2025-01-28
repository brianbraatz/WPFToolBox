using System;
using System.Security;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace RibbonStyle
{
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public NativeMethods.POINT ptReserved;
            public NativeMethods.POINT ptMaxSize;
            public NativeMethods.POINT ptMaxPosition;
            public NativeMethods.POINT ptMinTrackSize;
            public NativeMethods.POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public Int32 x;
            public Int32 y;
            public POINT()
            {
                x = y = 0;
            }
            public POINT(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
            public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public Int32 cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public Int32 dwFlags = 0;
        }

        /// <summary>When resizing a window, this is the side of the window being resized.</summary>
        public enum SizingWindowSide
        {
            /// <summary>WMSZ_LEFT: Left edge</summary>
            Left = 1,
            /// <summary>WMSZ_RIGHT: Right edge</summary>
            Right = 2,
            /// <summary>WMSZ_TOP: Top edge</summary>
            Top = 3,
            /// <summary>WMSZ_TOPLEFT: Top-left corner</summary>
            TopLeft = 4,
            /// <summary>WMSZ_TOPRIGHT: Top-right corner</summary>
            TopRight = 5,
            /// <summary>WMSZ_BOTTOM: Bottom edge</summary>
            Bottom = 6,
            /// <summary>WMSZ_BOTTOMLEFT: Bottom-left corner</summary>
            BottomLeft = 7,
            /// <summary>WMSZ_BOTTOMRIGHT: Bottom-right corner</summary>
            BottomRight = 8,
        }

        public enum HitTestResult
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }

        public const Int32 WM_MOVE = 0x0003;
        public const Int32 WM_SIZING = 0x0214;
        public const Int32 WM_NCHITTEST = 0x0084;
        public const Int32 WM_GETMINMAXINFO = 0x0024;
        public const Int32 MONITOR_DEFAULTTONEAREST = 0x0002;

        public static Int32 SignedLOWORD(IntPtr intPtr)
        {
            return (Int32)(intPtr.ToInt64() & 0xffff);
        }

        public static Int32 SignedHIWORD(IntPtr intPtr)
        {
            return (Int32)((intPtr.ToInt64() >> 16) & 0xffff);
        }

        [SecurityCritical]
        public static Object PtrToStructure(IntPtr lparam, Type clrType)
        {
            return Marshal.PtrToStructure(lparam, clrType);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, Int32 dwFlags);

        [DllImport("user32.dll")]
        public static extern Boolean GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hwnd, Boolean bRevert);

        [DllImport("user32.dll")]
        public static extern Boolean SetMenu(IntPtr hwnd, IntPtr hMenu);

        [DllImport("user32.dll", EntryPoint = "GetClientRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern Boolean IntGetClientRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);
        [SecurityCritical, SecurityTreatAsSafe]
        public static void GetClientRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect)
        {
            if (!IntGetClientRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "ClientToScreen", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 IntClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);
        [SecurityCritical]
        public static void ClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt)
        {
            if (IntClientToScreen(hWnd, pt) == 0)
            {
                throw new Win32Exception();
            }
        }
    }
}

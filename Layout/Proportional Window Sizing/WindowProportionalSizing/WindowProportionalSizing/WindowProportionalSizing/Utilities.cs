using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace WindowProportionalSizing
{
    internal static class Utilities
    {
        internal const int WM_MOVING = 0x0216;
        internal const int WM_SIZING = 0x0214;
        internal const int WMSZ_LEFT = 1;
        internal const int WMSZ_RIGHT = 2;
        internal const int WMSZ_TOPLEFT = 4;
        internal const int WMSZ_BOTTOMLEFT = 7;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }
        }
    }
}

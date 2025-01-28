using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Security;
using System.Runtime.InteropServices;

namespace WindowProportionalSizing
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        private void SourceInitializedHandler(object sender, EventArgs e)
        {
            HwndSource hs = PresentationSource.FromVisual(this) as HwndSource;
            if (hs == null)
            {
                // This should never happen.  If you're here, something is 
                //terribly wrong.  Handle appropriately.
            }
            hs.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr retInt = IntPtr.Zero;

            switch (msg)
            {
                case Utilities.WM_SIZING:
                    Utilities.RECT rc1 = (Utilities.RECT)Marshal.PtrToStructure(lParam, typeof(Utilities.RECT));
                    int sizingEdge = wParam.ToInt32();

                    // The logic below modifies the height to equal the new width, if the window
                    // is being dragged from either the left or right border.  When the window
                    // is dragged from any other border/edge, the width is modified to equal the
                    // height.  You may want to change this behavior to fit your app's requirements.
                    if ((sizingEdge == Utilities.WMSZ_LEFT)
                        || (sizingEdge == Utilities.WMSZ_RIGHT))
                    {
                        rc1.bottom = rc1.top + (rc1.right - rc1.left);
                    }
                    else if ((sizingEdge == Utilities.WMSZ_BOTTOMLEFT) 
                        || (sizingEdge == Utilities.WMSZ_TOPLEFT))
                    {
                        rc1.left = rc1.right - (rc1.bottom - rc1.top);
                    }
                    else
                    {
                        rc1.right = rc1.left + (rc1.bottom - rc1.top);
                    }

                    Marshal.StructureToPtr(rc1, lParam, true);
                    break;
                default:
                    break;
            }
            return retInt;
        }
    }
}
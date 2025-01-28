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
using System.Security;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace WindowMoveRestricted
{
    /// <summary>
    /// Interaction logic for RestrictedWindow.xaml
    /// </summary>

    public partial class RestrictedWindow : System.Windows.Window
    {

        public RestrictedWindow()
        {
            InitializeComponent();
        }

        internal void ShowRestrictedDialog()
        {
            // Restrict the size of the dialog window to that 
            // of the Owner window.
            //
            // Apps can customize this behavior to suit their needs
            //
            Binding bindingHeight = new Binding();
            bindingHeight.Source = Owner;
            bindingHeight.Path = new PropertyPath("ActualHeight");
            SetBinding(Window.MaxHeightProperty, bindingHeight);

            Binding bindingWidth = new Binding();
            bindingWidth.Source = Owner;
            bindingWidth.Path = new PropertyPath("ActualWidth");
            SetBinding(Window.MaxWidthProperty, bindingWidth);

            ShowDialog();
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
                case Utilities.WM_MOVING:
                    Utilities.RECT rc = (Utilities.RECT)Marshal.PtrToStructure(lParam, typeof(Utilities.RECT));
                    int currentWidth = rc.right - rc.left;
                    int currentHeight = rc.bottom - rc.top;
                    bool updated = false;

                    // owner dimensions in device units
                    if (Owner == null)
                    {
                        break;
                    }

                    HwndSource ownerHwndSource = PresentationSource.FromVisual(Owner) as HwndSource;
                    if (ownerHwndSource == null)
                    {
                        // This should never happen.  If you're here, something is 
                        //terribly wrong.  Handle appropriately.
                    }

                    Point ownerSizeDeviceUnits = ownerHwndSource.CompositionTarget.TransformToDevice.Transform(new Point(Owner.ActualWidth, Owner.ActualHeight));
                    Point ownerPositionDeviceUnits = ownerHwndSource.CompositionTarget.TransformToDevice.Transform(new Point(Owner.Left, Owner.Top));

                    // Ensure owned window is smaller or equal to the owner window
                    if ((currentWidth > (int)ownerSizeDeviceUnits.X)
                        || (currentHeight > (int)ownerSizeDeviceUnits.Y))
                    {
                        break;
                    }

                    // Enforce left boundary
                    if (rc.left < (int)ownerPositionDeviceUnits.X)
                    {
                        rc.left = (int)ownerPositionDeviceUnits.X;
                        rc.right = rc.left + currentWidth;
                        updated = true;
                    }

                    // Enforce top boundary
                    if (rc.top < (int)ownerPositionDeviceUnits.Y)
                    {
                        rc.top = (int)ownerPositionDeviceUnits.Y;
                        rc.bottom = rc.top + currentHeight;
                        updated = true;
                    }

                    // Enforce right boundary
                    if (rc.right > (int)(ownerPositionDeviceUnits.X + ownerSizeDeviceUnits.X))
                    {
                        rc.right = (int)(ownerPositionDeviceUnits.X + ownerSizeDeviceUnits.X);
                        rc.left = rc.right - currentWidth;
                        updated = true;
                    }

                    // Enforce bottom boundary
                    if (rc.bottom > (int)(ownerPositionDeviceUnits.Y + ownerSizeDeviceUnits.Y))
                    {
                        rc.bottom = (int)(ownerPositionDeviceUnits.Y + ownerSizeDeviceUnits.Y);
                        rc.top = rc.bottom - currentHeight;
                        updated = true;
                    }

                    if (updated == true)
                    {
                        Marshal.StructureToPtr(rc, lParam, true);
                    }
                    break;
                default:
                    break;
            }
            return retInt;
        }
    }
}
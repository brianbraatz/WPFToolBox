using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace RibbonStyle
{
    public class OfficeWindow : Window
    {
        private Button minimizeButton;
        private Button maximizeButton;
        private Button closeButton;
        private HwndSource currentHwndSource;
        private ContentControl windowIcon;
        private FrameworkElement moveGripper;
        private FrameworkElement bottomLeftGripper;
        private FrameworkElement topLeftGripper;
        private FrameworkElement topRightGripper;
        private FrameworkElement leftGripper;
        private FrameworkElement rightGripper;
        private FrameworkElement topGripper;
        private FrameworkElement bottomGripper;
        static OfficeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OfficeWindow), new FrameworkPropertyMetadata(typeof(OfficeWindow)));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IntPtr currentHwnd = (new WindowInteropHelper(this)).Handle;
            currentHwndSource = HwndSource.FromHwnd(currentHwnd);
            currentHwndSource.AddHook(new HwndSourceHook(this.WinProc));
            base.OnSourceInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            minimizeButton = this.GetTemplateChild("PART_MinimizeButton") as Button;
            maximizeButton = this.GetTemplateChild("PART_MaximizeButton") as Button;
            closeButton = this.GetTemplateChild("PART_CloseButton") as Button;
            windowIcon = this.GetTemplateChild("WindowIcon") as ContentControl;
            moveGripper = this.GetTemplateChild("WindowMoveGripper") as FrameworkElement;

            bottomLeftGripper = this.GetTemplateChild("BottomLeftGripper") as FrameworkElement;
            topLeftGripper = this.GetTemplateChild("TopLeftGripper") as FrameworkElement;
            topRightGripper = this.GetTemplateChild("TopRightGripper") as FrameworkElement;

            leftGripper = this.GetTemplateChild("LeftGripper") as FrameworkElement;
            rightGripper = this.GetTemplateChild("RightGripper") as FrameworkElement;
            topGripper = this.GetTemplateChild("TopGripper") as FrameworkElement;
            bottomGripper = this.GetTemplateChild("BottomGripper") as FrameworkElement;
            if (minimizeButton != null)
            {
                minimizeButton.Click += OnWindowMinimizing;
            }
            if (maximizeButton != null)
            {
                maximizeButton.Click += OnWindowStateRestoring;
            }
            if (closeButton != null)
            {
                closeButton.Click += OnWindowClosing;
            }
            base.OnApplyTemplate();
        }

        private void OnWindowStateRestoring(Object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnWindowMinimizing(Object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnWindowClosing(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        [SecurityCritical]
        private IntPtr WinProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            IntPtr result = IntPtr.Zero;
            switch (msg)
            {
                case NativeMethods.WM_GETMINMAXINFO:
                    handled = this.WmGetMinMaxInfo(lParam);
                    break;
                case NativeMethods.WM_SIZING:
                    handled = this.WmSizing(wParam, lParam);
                    break;
                case NativeMethods.WM_NCHITTEST:
                    handled = this.WmNcHitTest(lParam, ref result);
                    break;
            }

            return result;
        }

        private Int32 DoubleToInt32(Double value)
        {
            return (Int32)Math.Ceiling(value);
        }

        [SecurityCritical]
        private Boolean WmGetMinMaxInfo(IntPtr lParam)
        {
            NativeMethods.MINMAXINFO minmaxinfo = (NativeMethods.MINMAXINFO)NativeMethods.PtrToStructure(lParam, typeof(NativeMethods.MINMAXINFO));
            IntPtr hMonitor = NativeMethods.MonitorFromWindow(this.currentHwndSource.Handle, NativeMethods.MONITOR_DEFAULTTONEAREST);
            NativeMethods.MONITORINFO monitorinfo = new NativeMethods.MONITORINFO();
            NativeMethods.GetMonitorInfo(hMonitor, monitorinfo);
            minmaxinfo.ptMaxPosition.x = Math.Abs(monitorinfo.rcMonitor.left - monitorinfo.rcWork.left);
            minmaxinfo.ptMaxPosition.y = Math.Abs(monitorinfo.rcMonitor.top - monitorinfo.rcWork.top);
            minmaxinfo.ptMaxSize.x = Math.Abs(monitorinfo.rcWork.left - monitorinfo.rcWork.right);
            minmaxinfo.ptMaxSize.y = Math.Abs(monitorinfo.rcWork.top - monitorinfo.rcWork.bottom);
            System.Runtime.InteropServices.Marshal.StructureToPtr(minmaxinfo, lParam, true);

            return true;
        }

        [SecurityCritical]
        private Boolean WmSizing(IntPtr wParam, IntPtr lParam)
        {
            NativeMethods.RECT rect = (NativeMethods.RECT)NativeMethods.PtrToStructure(lParam, typeof(NativeMethods.RECT));
            Int32 width = Math.Abs(rect.left - rect.right);
            Int32 height = Math.Abs(rect.bottom - rect.top);
            Int32 minWidth = this.DoubleToInt32(SystemParameters.MinimizedWindowWidth);
            Int32 minHeight = this.DoubleToInt32(SystemParameters.MinimumWindowHeight);
            if (width < minWidth || height < minHeight)
            {
                switch ((NativeMethods.SizingWindowSide)wParam)
                {
                    case NativeMethods.SizingWindowSide.Top:
                        if (height < minHeight)
                        {
                            rect.top = rect.bottom - minHeight;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.Bottom:
                        if (height < minHeight)
                        {
                            rect.bottom = rect.top + minHeight;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.Left:
                        if (width < minWidth)
                        {
                            rect.left = rect.right - minWidth;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.Right:
                        if (width < minWidth)
                        {
                            rect.right = rect.left + minWidth;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.BottomLeft:
                        if (height < minHeight)
                        {
                            rect.bottom = rect.top + minHeight;
                        }
                        if (width < minWidth)
                        {
                            rect.right = rect.left + minWidth;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.BottomRight:
                        if (width < minWidth)
                        {
                            rect.right = rect.left + minWidth;
                        }
                        if (height < minHeight)
                        {
                            rect.bottom = rect.top + minHeight;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.TopLeft:
                        if (width < minWidth)
                        {
                            rect.left = rect.right - minWidth;
                        }
                        if (height < minHeight)
                        {
                            rect.top = rect.bottom - minHeight;
                        }
                        break;
                    case NativeMethods.SizingWindowSide.TopRight:
                        if (width < minWidth)
                        {
                            rect.right = rect.left + minWidth;
                        }
                        if (height < minHeight)
                        {
                            rect.top = rect.bottom - minHeight;
                        }
                        break;
                }
                System.Runtime.InteropServices.Marshal.StructureToPtr(rect, lParam, true);
            }
            return true;
        }

        private Boolean WmNcHitTest(IntPtr lParam, ref IntPtr result)
        {
            Int32 x = NativeMethods.SignedLOWORD(lParam);
            Int32 y = NativeMethods.SignedHIWORD(lParam);
            NativeMethods.POINT devicePoint = this.GetPointRelativeToWindow(x, y);
            Point logicalPoint = this.DeviceToLogicalUnits(new Point((Double)devicePoint.x, (Double)devicePoint.y));
            if (this.IsPointWithExtent(logicalPoint, this.windowIcon))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTSYSMENU;
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.topLeftGripper))
            {
                if (base.FlowDirection == FlowDirection.LeftToRight)
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTTOPLEFT;
                }
                else
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTTOPRIGHT;
                }
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.topRightGripper))
            {
                if (base.FlowDirection == FlowDirection.LeftToRight)
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTTOPRIGHT;
                }
                else
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTTOPLEFT;
                }
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.bottomLeftGripper))
            {
                if (base.FlowDirection == FlowDirection.LeftToRight)
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTBOTTOMLEFT;
                }
                else
                {
                    result = (IntPtr)NativeMethods.HitTestResult.HTBOTTOMRIGHT;
                }
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.topGripper))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTTOP;
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.bottomGripper))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTBOTTOM;
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.leftGripper))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTLEFT;
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.rightGripper))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTRIGHT;
                return true;
            }
            else if (this.IsPointWithExtent(logicalPoint, this.moveGripper))
            {
                result = (IntPtr)NativeMethods.HitTestResult.HTCAPTION;
                return true;
            }
            return false;
        }

        [SecurityCritical]
        public Boolean IsDisposed()
        {
            return currentHwndSource.Handle == IntPtr.Zero;
        }

        public Boolean IsPointWithExtent(Point point, FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }
            GeneralTransform transform = base.TransformToDescendant(element);
            Point resultPoint = point;
            if (transform != null && !transform.TryTransform(point, out resultPoint))
            {
                return false;
            }
            if (((resultPoint.X < 0) || (resultPoint.Y < 0)) || ((resultPoint.X > element.RenderSize.Width) || (resultPoint.Y > element.RenderSize.Height)))
            {
                return false;
            }

            return true;
        }

        [SecurityCritical]
        private NativeMethods.POINT GetWindowScreenLocation(FlowDirection flowDirection)
        {
            NativeMethods.POINT point = new NativeMethods.POINT(0, 0);
            if (flowDirection == FlowDirection.RightToLeft)
            {
                NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, 0, 0);
                NativeMethods.GetClientRect(new HandleRef(this, this.currentHwndSource.Handle), ref rect);
                point.x = rect.right;
                point.y = rect.top;
            }
            NativeMethods.ClientToScreen(new HandleRef(this, this.currentHwndSource.Handle), point);
            return point;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal NativeMethods.POINT GetPointRelativeToWindow(Int32 x, Int32 y)
        {
            NativeMethods.POINT point = this.GetWindowScreenLocation(base.FlowDirection);
            return new NativeMethods.POINT(x - point.x, y - point.y);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal Point DeviceToLogicalUnits(Point ptDeviceUnits)
        {
            return this.currentHwndSource.CompositionTarget.TransformFromDevice.Transform(ptDeviceUnits);
        }
    }
}

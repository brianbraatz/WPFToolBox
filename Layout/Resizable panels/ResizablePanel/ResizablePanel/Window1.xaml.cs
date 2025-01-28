using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace ResizablePanel
{
	public partial class Window1
	{
        double newLeft;
        double newTop;
        double newWidth;
        double newHeight;

		public Window1()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
            EventManager.RegisterClassHandler(typeof(PanelControl), PanelControl.OnPanelDragEvent, new RoutedEventHandler(this.OnDragPanel));
            EventManager.RegisterClassHandler(typeof(PanelControl), PanelControl.OnPanelMoveEvent, new RoutedEventHandler(this.OnMovePanel));
            EventManager.RegisterClassHandler(typeof(PanelControl), PanelControl.OnPanelResizeEvent, new RoutedEventHandler(this.OnResizePanel));
		}

        private void OnMovePanel(object o, RoutedEventArgs args)
        {
            PanelControl control = (PanelControl)o;
            PanelDeltaEventArgs panelDragEventArgs = (PanelDeltaEventArgs)args;

            newLeft = Math.Max(0, Canvas.GetLeft(control) + panelDragEventArgs.OffsetX);
            newLeft = Math.Min(newLeft, this.DocumentRoot.ActualWidth - control.Width);
            Canvas.SetLeft(control, newLeft);

            newTop = Math.Max(0, Canvas.GetTop(control) + panelDragEventArgs.OffsetY);
            newTop = Math.Min(newTop, this.DocumentRoot.ActualHeight - control.Height);
            Canvas.SetTop(control, newTop);
        }

        private void OnResizePanel(object o, RoutedEventArgs args)
        {
            PanelControl control = (PanelControl)o;
            PanelDeltaEventArgs resizeDragEventArgs = (PanelDeltaEventArgs)args;

            newWidth = control.ActualWidth + resizeDragEventArgs.OffsetX;
            if (newWidth > 0)
                control.Width = newWidth;

            newHeight = control.ActualHeight + resizeDragEventArgs.OffsetY;
            if(newHeight > 0)
                control.Height = newHeight;
        }

        private void OnDragPanel(object o, RoutedEventArgs args)
        {
            PanelControl control = (PanelControl)o;
            this.DocumentRoot.Children.Remove(control);
            this.DocumentRoot.Children.Add(control);
        }
	}
}

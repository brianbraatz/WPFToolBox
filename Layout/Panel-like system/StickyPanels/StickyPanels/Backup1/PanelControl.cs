namespace StickyPanels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class PanelControl : HeaderedContentControl
    {
		private Point mouseOffsetForDrag;

		public event RoutedEventHandler OnPanelDrop
		{
			add
			{
				base.AddHandler(PanelControl.OnPanelDropEvent, value);
			}
			remove
			{
				base.RemoveHandler(PanelControl.OnPanelDropEvent, value);
			}
		}

		public static readonly RoutedEvent OnPanelDropEvent = EventManager.RegisterRoutedEvent("OnPanelDrop", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelControl));

		public event RoutedEventHandler OnPanelDrag
		{
			add
			{
				base.AddHandler(PanelControl.OnPanelDragEvent, value);
			}
			remove
			{
				base.RemoveHandler(PanelControl.OnPanelDragEvent, value);
			}
		}

		public static readonly RoutedEvent OnPanelDragEvent = EventManager.RegisterRoutedEvent("OnPanelDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelControl));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			FrameworkElement header = this.Template.FindName("PART_Header", this) as FrameworkElement;
			if (header != null)
			{
				header.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Header_MouseLeftButtonDown);
			}
		}

        void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			FrameworkElement header = sender as FrameworkElement;
			mouseOffsetForDrag = e.MouseDevice.GetPosition(this as IInputElement);

			Application.Current.MainWindow.MouseMove += new System.Windows.Input.MouseEventHandler(Parent_MouseMove);
			Application.Current.MainWindow.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Parent_MouseLeftButtonUp);

			base.RaiseEvent(new RoutedEventArgs(PanelControl.OnPanelDragEvent, this));

			e.MouseDevice.Capture(this as IInputElement);
        }

        void Parent_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			this.RenderTransform = null;
	        FrameworkElement header = sender as FrameworkElement;

            Application.Current.MainWindow.MouseMove -= Parent_MouseMove;
			Application.Current.MainWindow.MouseLeftButtonUp -= Parent_MouseLeftButtonUp;

            e.MouseDevice.Capture(null);

			base.RaiseEvent(new RoutedEventArgs(PanelControl.OnPanelDropEvent, this));
        }

        void Parent_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
			e.MouseDevice.Synchronize();

            Point mouseOffsetRelativeToParent = e.MouseDevice.GetPosition(Application.Current.MainWindow as IInputElement);
            Point offsetTansformDistance = new Point(mouseOffsetRelativeToParent.X - mouseOffsetForDrag.X, mouseOffsetRelativeToParent.Y - mouseOffsetForDrag.Y);

            this.RenderTransform = new TranslateTransform(offsetTansformDistance.X, offsetTansformDistance.Y);
        }
    }
}
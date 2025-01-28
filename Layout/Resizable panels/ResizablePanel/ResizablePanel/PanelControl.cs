namespace ResizablePanel
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;

    public class PanelDeltaEventArgs : RoutedEventArgs
    {
        private double offsetX;

        public double OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        private double offsetY;

        public double OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }
    }

    public class PanelControl : HeaderedContentControl
    {
        Point currentPosition;
        bool isResizing = false;
        bool isMoving = false;
        FrameworkElement header;
        FrameworkElement resizeGrip;
        double offsetX;
        double offsetY;

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


        public event RoutedEventHandler OnPanelMove
        {
            add
            {
                base.AddHandler(PanelControl.OnPanelMoveEvent, value);
            }
            remove
            {
                base.RemoveHandler(PanelControl.OnPanelMoveEvent, value);
            }
        }

        public static readonly RoutedEvent OnPanelMoveEvent = EventManager.RegisterRoutedEvent("OnPanelMove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelControl));

        public event RoutedEventHandler OnPanelResize
        {
            add
            {
                base.AddHandler(PanelControl.OnPanelResizeEvent, value);
            }
            remove
            {
                base.RemoveHandler(PanelControl.OnPanelResizeEvent, value);
            }
        }

        public static readonly RoutedEvent OnPanelResizeEvent = EventManager.RegisterRoutedEvent("OnPanelResize", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelControl));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			header = this.Template.FindName("PART_Header", this) as FrameworkElement;
			if (header != null)
			{
				header.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseLeftButtonDown);
			}

            resizeGrip = this.Template.FindName("PART_ResizeGrip", this) as FrameworkElement;
            if (resizeGrip != null)
            {
                resizeGrip.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseLeftButtonDown);
            }
		}

        void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            currentPosition = e.GetPosition(Application.Current.MainWindow);

            if (senderElement == header)
            {
                isMoving = true;
                base.RaiseEvent(new RoutedEventArgs(PanelControl.OnPanelDragEvent, this));
            }

            if (senderElement == resizeGrip)
            {
                isResizing = true;
            }

			Application.Current.MainWindow.MouseMove += new System.Windows.Input.MouseEventHandler(Parent_MouseMove);
            Application.Current.MainWindow.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Parent_MouseLeftButtonUp);
        }

        void Parent_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.MouseMove -= Parent_MouseMove;
			Application.Current.MainWindow.MouseLeftButtonUp -= Parent_MouseLeftButtonUp;

            this.Cursor = Cursors.Arrow;
            isResizing = false;
            isMoving = false;
        }

        void Parent_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point newPosition = e.GetPosition(Application.Current.MainWindow);

            offsetX = newPosition.X - currentPosition.X;
            offsetY = newPosition.Y - currentPosition.Y;

            currentPosition = newPosition;

            if ((offsetX >= 1) || (offsetX <= -1) || (offsetY >= 1) || (offsetY <= -1))
            {
                PanelDeltaEventArgs args = new PanelDeltaEventArgs(); ;

                if (isMoving)
                {
                   args.RoutedEvent = PanelControl.OnPanelMoveEvent;
                }

                if (isResizing)
                {
                    args.RoutedEvent = PanelControl.OnPanelResizeEvent;
                    this.Cursor = Cursors.SizeNWSE;
                }

                args.OffsetX = offsetX;
                args.OffsetY = offsetY;
                args.Source = this;

                base.RaiseEvent(args);
            }
        }
    }
}
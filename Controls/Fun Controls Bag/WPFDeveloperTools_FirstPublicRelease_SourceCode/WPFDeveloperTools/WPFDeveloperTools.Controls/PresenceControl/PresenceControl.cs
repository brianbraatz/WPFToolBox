using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeveloperTools.Controls.PresenceControl
{
    /// <summary>
    /// PresenceControl is a control used to represent the status of user (Online, Offline, etc...).
    /// It could be used with Communicator or other IM application.
    /// </summary>
    public class PresenceControl : Control
    {
        #region Constructors

        static PresenceControl()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(PresenceControl), new FrameworkPropertyMetadata((typeof(PresenceControl))));
        }

        public PresenceControl()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the status for the user.
        /// </summary>
        public Status UserStatus
        {
            get { return (Status)GetValue(UserStatusProperty); }
            set { SetValue(UserStatusProperty, value); }
        }

        public static readonly DependencyProperty UserStatusProperty =
            DependencyProperty.Register("UserStatus", typeof(Status), typeof(PresenceControl), new FrameworkPropertyMetadata(Status.Unknow, new PropertyChangedCallback(OnUserStatusChanged)));


        /// <summary>
        /// Indicate the X-coordonate of the ellipse used to draw the control.
        /// </summary>
        public double XCoordonate
        {
            get { return (double)GetValue(XCoordonateProperty); }
            set { SetValue(XCoordonateProperty, value); }
        }

        public static readonly DependencyProperty XCoordonateProperty =
            DependencyProperty.Register("XCoordonate", typeof(double), typeof(PresenceControl), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Indicate the Y-coordonate of the ellipse used to draw the control.
        /// </summary>
        public double YCoordonate
        {
            get { return (double)GetValue(YCoordonateProperty); }
            set { SetValue(YCoordonateProperty, value); }
        }

        public static readonly DependencyProperty YCoordonateProperty =
            DependencyProperty.Register("YCoordonate", typeof(double), typeof(PresenceControl), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Indicate the horizontal radius of the ellipse used to draw the control.
        /// </summary>
        public double XRadius
        {
            get { return (double)GetValue(XRadiusProperty); }
            set { SetValue(XRadiusProperty, value); }
        }

        public static readonly DependencyProperty XRadiusProperty =
            DependencyProperty.Register("XRadius", typeof(double), typeof(PresenceControl), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Indicate the vertical radius of the ellipse used to draw the control.
        /// </summary>
        public double YRadius
        {
            get { return (double)GetValue(YRadiusProperty); }
            set { SetValue(YRadiusProperty, value); }
        }

        public static readonly DependencyProperty YRadiusProperty =
            DependencyProperty.Register("YRadius", typeof(double), typeof(PresenceControl), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// RoutedEvent associated to the event UserStatusChanged which allows to get the old and the new status.
        /// </summary>
        public static readonly RoutedEvent UserStatusChangedEvent =
            EventManager.RegisterRoutedEvent("UserStatusChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<string>), typeof(PresenceControl));

        /// <summary>
        /// UserStatusChanged event allows you to be notified when the status for a user is changing.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> UserStatusChanged
        {
            add { AddHandler(UserStatusChangedEvent, value); }
            remove { RemoveHandler(UserStatusChangedEvent, value); }
        }

        protected virtual void OnUserStatusChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> args = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            args.RoutedEvent = PresenceControl.UserStatusChangedEvent;
            RaiseEvent(args);
        }

        /// <summary>
        /// Tooltip to display for the PresenceControl.
        /// </summary>
        public object ToolTipToDisplay
        {
            get { return (string)GetValue(ToolTipToDisplayProperty); }
            set { SetValue(ToolTipToDisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipToDisplayProperty =
            DependencyProperty.Register("ToolTipToDisplay", typeof(object), typeof(PresenceControl), new FrameworkPropertyMetadata(null));

        #endregion

        private static void OnUserStatusChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Status oldStatus = (Status)args.OldValue;
            Status newStatus = (Status)args.NewValue;

            PresenceControl control = element as PresenceControl;

            if (control != null)
            {
                control.SetValue(UserStatusProperty, newStatus);
                control.InvalidateVisual();

                control.OnUserStatusChanged(oldStatus.ToString(), newStatus.ToString());
            }
        }

        /// <summary>
        /// Enum used to specify the status.
        /// </summary>
        public enum Status
        {
            Away,
            Block,
            Busy,
            DoNotDisturb,
            Maybe,
            Offline,
            Online,
            Unknow
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            ImageBrush renderBrush = null;
            Pen renderPen = null;

            BitmapImage img = new BitmapImage();
            img.BeginInit();

           switch (this.UserStatus)
            {
                case Status.Away:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/away.png", UriKind.Absolute);
                    break;

                case Status.Block:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/block.png", UriKind.Absolute);
                    break;

                case Status.Busy:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/busy.png", UriKind.Absolute);
                    break;

                case Status.DoNotDisturb:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/dnd.png", UriKind.Absolute);
                    break;

                case Status.Maybe:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/maybe.png", UriKind.Absolute);
                    break;

                case Status.Offline:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/offline.png", UriKind.Absolute);
                    break;

                case Status.Online:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/online.png", UriKind.Absolute);
                    break;

                case Status.Unknow:
                default:
                    img.UriSource = new Uri("pack://application:,,,/WPFDeveloperTools.Controls;component/PresenceControl/Images/unknow.png", UriKind.Absolute);
                    break;
            }

            img.EndInit();

            renderBrush = new ImageBrush(img);
            renderPen = new Pen(renderBrush, 5);

            this.ToolTip = this.ToolTipToDisplay;

            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(this.XCoordonate, this.YCoordonate), this.XRadius, this.YRadius);
        }
    }
}

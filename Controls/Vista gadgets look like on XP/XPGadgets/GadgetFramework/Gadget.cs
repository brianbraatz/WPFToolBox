using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace GadgetFramework
{
    public enum GadgetState
    {
        Docked,
        Floating,
		Gallery
    }

    public class Gadget : UserControl, INotifyPropertyChanged
    {
        //Mark abstract if possible
        public virtual string GadgetName { get { return "Foo"; } }
        public virtual string Description { get { return "Foo"; } }

        public const double DockedGadgetHeight = 150;
        public const double DockedGadgetWidth = 150;

        private Point mouseDownPoint;
        private int mouseDragDelta = 3;
        private bool isDragging = false;
        private bool isPropertyPageShowing = false;
        private Storyboard opacityAnimationStoryboard = new Storyboard();

        public object PropertyPageContent
        {
            get { return base.GetValue(PropertyPageContentProperty); }
            set
            { 
                base.SetValue(PropertyPageContentProperty, value);
                if (this.PropertyPageContent != null && this.GadgetState == GadgetState.Floating)
                {
                    this.ShowHidePropertiesPageCommand.IsEnabled = true;
                }
            }
        }
        public static readonly DependencyProperty PropertyPageContentProperty = DependencyProperty.Register("PropertyPageContent", typeof(object), typeof(Gadget));

        public double DesiredUndockedGadgetHeight
        {
            get { return (double)base.GetValue(DesiredUndockedGadgetHeightProperty); }
            set
            {
                base.SetValue(DesiredUndockedGadgetHeightProperty, value);
            }
        }
        public static readonly DependencyProperty DesiredUndockedGadgetHeightProperty = DependencyProperty.Register("DesiredUndockedGadgetHeight", typeof(double), typeof(Gadget), new PropertyMetadata(150.0));

        public double DesiredUndockedGadgetWidth
        {
            get { return (double)base.GetValue(DesiredUndockedGadgetWidthProperty); }
            set 
            {
                base.SetValue(DesiredUndockedGadgetWidthProperty, value);
            }
        }
        public static readonly DependencyProperty DesiredUndockedGadgetWidthProperty = DependencyProperty.Register("DesiredUndockedGadgetWidth", typeof(double), typeof(Gadget), new PropertyMetadata(150.0));

        private double actualGadgetHeight = Gadget.DockedGadgetHeight;
        public double ActualGadgetHeight
        {
            get { return this.actualGadgetHeight; }
            set
            {
                this.actualGadgetHeight = value;
                this.OnNotifyPropertyChanged("ActualGadgetHeight");
            }
        }

        private double actualGadgetWidth = Gadget.DockedGadgetHeight;
        public double ActualGadgetWidth
        {
            get { return this.actualGadgetWidth; }
            set
            {
                this.actualGadgetWidth = value;
                this.OnNotifyPropertyChanged("ActualGadgetWidth");
            }
        }
	
        public GadgetState GadgetState
        {
            get { return (GadgetState)base.GetValue(GadgetStateProperty); }
            set 
            {
                base.SetValue(GadgetStateProperty, value);

                if (value == GadgetState.Floating)
                {
                    this.ActualGadgetWidth = this.DesiredUndockedGadgetWidth;
                    this.ActualGadgetHeight = this.DesiredUndockedGadgetHeight;
                }
                else
                {
                    this.ActualGadgetWidth = Gadget.DockedGadgetWidth;
                    this.ActualGadgetHeight = Gadget.DockedGadgetHeight;
                }

                if (value == GadgetState.Floating && this.PropertyPageContent != null)
                {
                    this.ShowHidePropertiesPageCommand.IsEnabled = true;
                }
                else
                {
                    if (this.isPropertyPageShowing)
                    {
                        this.OnShowHidePropertiesPage(null);
                    }
                    this.ShowHidePropertiesPageCommand.IsEnabled = false;
                }
            }
        }
        public static readonly DependencyProperty GadgetStateProperty = DependencyProperty.Register("GadgetState", typeof(GadgetState), typeof(Gadget));

        private double userSelectedTransparency = 1;
        private double UserSelectedTransparency
        {
            get { return userSelectedTransparency; }
            set
            { 
                userSelectedTransparency = value;
                AnimateToUserSelectedTransparency();
            }
        }

        private DelegateCommand showHidePropertiesCommand;
        public DelegateCommand ShowHidePropertiesPageCommand
        {
            get
            {
                if (this.showHidePropertiesCommand == null)
                {
                    this.showHidePropertiesCommand = new DelegateCommand(this.OnShowHidePropertiesPage);
                }
                return this.showHidePropertiesCommand;
            }
        }

        public static readonly RoutedEvent OnGadgetDragEvent = EventManager.RegisterRoutedEvent("OnGadgetDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Gadget));
        public event RoutedEventHandler OnGadgetDrag
        {
            add
            {
                base.AddHandler(Gadget.OnGadgetDragEvent, value);
            }
            remove
            {
                base.RemoveHandler(Gadget.OnGadgetDragEvent, value);
            }
        }

        private GadgetTransprencyModel gadgetTransparencyModel;
        public GadgetTransprencyModel GadgetTransparencyModel
        {
            get
            {
                if (gadgetTransparencyModel == null)
                {
                    gadgetTransparencyModel = new GadgetTransprencyModel(this);
                }
                return this.gadgetTransparencyModel; 
            }
        }


        public static readonly RoutedCommand AlwaysOnTopCommand  = new RoutedCommand("AlwaysOnTop", typeof(Gadget));

        static Gadget()
        {
            CommandManager.RegisterClassCommandBinding(typeof(Gadget), new CommandBinding(AlwaysOnTopCommand, Gadget.OnAlwaysOnTopCommand, Gadget.OnQueryAlwaysOnTopCommand));
        }

        public Gadget()
        {
            this.DataContext = this;

            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("GadgetFramework;component/GadgetResources.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceDictionary);

            this.ContextMenu = (ContextMenu)this.Resources["GadgetContextMenu"];

            this.ShowHidePropertiesPageCommand.IsEnabled = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
			if (this.GadgetState != GadgetState.Gallery)
			{
				base.OnMouseLeftButtonDown(e);
				mouseDownPoint = e.GetPosition(this);

				this.MouseLeftButtonUp += new MouseButtonEventHandler(Gadget_MouseLeftButtonUp);
				this.MouseMove += new MouseEventHandler(Gadget_MouseMove);
			}

        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            opacityAnimationStoryboard.Children.Clear();

            DoubleAnimation animation = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            opacityAnimationStoryboard.Children.Add(animation);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0)", UIElement.OpacityProperty));
            opacityAnimationStoryboard.Begin(this);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            AnimateToUserSelectedTransparency();
        }

        private static void OnAlwaysOnTopCommand(object target, ExecutedRoutedEventArgs args)
        {
        }

        private static void OnQueryAlwaysOnTopCommand(object target, CanExecuteRoutedEventArgs args)
        {
            Gadget gadget = (Gadget)target;
            if (gadget.GadgetState == GadgetState.Floating)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
            args.Handled = true;
        }

        private void OnShowHidePropertiesPage(object arg)
        {
            if (!this.isPropertyPageShowing)
            {
                this.isPropertyPageShowing = true;
                Storyboard s = this.Template.Resources["OnShowPropertiesAnimation"] as Storyboard;
                s.Begin(this, this.Template);
            }
            else
            {
                this.isPropertyPageShowing = false;
                Storyboard s = this.Template.Resources["OnHidePropertiesAnimation"] as Storyboard;
                s.Begin(this, this.Template);
            }
        }
 
        private void AnimateToUserSelectedTransparency()
        {
            opacityAnimationStoryboard.Children.Clear();

            DoubleAnimation animation = new DoubleAnimation(this.UserSelectedTransparency, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            opacityAnimationStoryboard.Children.Add(animation);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0)", UIElement.OpacityProperty));
            opacityAnimationStoryboard.Begin(this);
        }

        void Gadget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonUp -= this.Gadget_MouseLeftButtonUp;
            if (isDragging)
            {
                isDragging = false;
            }
            else
            {
                this.MouseMove -= this.Gadget_MouseMove;
            }
        }

        void Gadget_MouseMove(object sender, MouseEventArgs e)
        {
            Point newMousePosition = e.GetPosition(this);
            if (Math.Abs(newMousePosition.X - mouseDownPoint.X) >= mouseDragDelta || Math.Abs(newMousePosition.Y - mouseDownPoint.Y) >= mouseDragDelta)
            {
                if (!isDragging)
                {
                    this.MouseMove -= this.Gadget_MouseMove;
                    base.RaiseEvent(new RoutedEventArgs(Gadget.OnGadgetDragEvent, this));
                    isDragging = true;
                }
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class GadgetTransprencyModel
        {
            private Gadget gadget;

            private ObservableCollection<GadgetTransparency> gadgetTransparencies = new ObservableCollection<GadgetTransparency>();
            public ObservableCollection<GadgetTransparency> GadgetTransparencies
            {
                get { return this.gadgetTransparencies; }
            }

            public GadgetTransprencyModel(Gadget gadget)
            {
                this.gadget = gadget;

                GadgetTransparencies.Add(new GadgetTransparency(0.2, "20%", false, this.OpacityChanged));
                GadgetTransparencies.Add(new GadgetTransparency(0.4, "40%", false, this.OpacityChanged));
                GadgetTransparencies.Add(new GadgetTransparency(0.6, "60%", false, this.OpacityChanged));
                GadgetTransparencies.Add(new GadgetTransparency(0.8, "80%", false, this.OpacityChanged));
                GadgetTransparencies.Add(new GadgetTransparency(1.0, "100%", true, this.OpacityChanged));
            }

            private void OpacityChanged(object arg)
            {
                gadget.UserSelectedTransparency = (double)arg;

                for (int i = 0; i < this.GadgetTransparencies.Count; ++i)
                {
                    GadgetTransparency gadgetTransparency = this.GadgetTransparencies[i];
                    if (gadgetTransparency.TransparencyValue == gadget.userSelectedTransparency)
                    {
                        if (!gadgetTransparency.IsChecked)
                        {
                            this.GadgetTransparencies[i] = new GadgetTransparency(gadgetTransparency.TransparencyValue, gadgetTransparency.Display, true, this.OpacityChanged);
                            this.gadget.Opacity = gadget.userSelectedTransparency;
                        }
                    }
                    else
                    {
                        if (gadgetTransparency.IsChecked)
                        {
                            this.GadgetTransparencies[i] = new GadgetTransparency(gadgetTransparency.TransparencyValue, gadgetTransparency.Display, false, this.OpacityChanged);
                        }
                    }
                }
            }
        }

        public class GadgetTransparency : INotifyPropertyChanged
        {
            private double transparencyValue;

            public double TransparencyValue
            {
                get { return transparencyValue; }
                set { transparencyValue = value; }
            }

            private string display;
            public string Display
            {
                get { return display; }
                set { display = value; }
            }

            private bool isChecked;

            public bool IsChecked
            {
                get { return isChecked; }
                set { isChecked = value; }
            }

            public GadgetTransparency(double transparencyValue, string display, bool isChecked, DelegateCommand.SimpleEventHandler handler)
            {
                this.TransparencyValue = transparencyValue;
                this.Display = display;
                this.IsChecked = isChecked;
                this.handler = handler;
            }

            private DelegateCommand.SimpleEventHandler handler;
            public DelegateCommand OpacityChangedCommand
            {
                get { return new DelegateCommand(this.handler); }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            private void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}


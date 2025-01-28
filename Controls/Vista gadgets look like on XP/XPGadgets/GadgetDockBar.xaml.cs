using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.Generic;
using GadgetFramework;
using System.Windows.Input;

namespace XPGadgets
{
	public partial class GadgetDockBar
	{
        private int previousInsertionIndex = -1;
        private double undockedWindowHeight = 400;
        private double undockedWindowWidth = 350;

        private Window windowBeingDragged;
        private Point mousePointRelativeToWindowBeingDragged;

        private WrapPanel itemsHostPanel;
        private double actualItemsHostPanelWidth = 0;
        private int currentPage;
        private Storyboard animation = new Storyboard();

        private DelegateCommand scrollLeftCommand;
        public DelegateCommand ScrollLeftCommand
        {
            get
            {
                if (scrollLeftCommand == null)
                {
                    scrollLeftCommand = new DelegateCommand(this.ScrollLeft);
                }
                return scrollLeftCommand; 
            }
        }

        private DelegateCommand scrollRightCommand;
        public DelegateCommand ScrollRightCommand
        {
            get
            {
                if (scrollRightCommand == null)
                {
                    scrollRightCommand = new DelegateCommand(this.ScrollRight);
                }
                return scrollRightCommand; 
            }
        }

        public GadgetDockBar()
		{
            EventManager.RegisterClassHandler(typeof(Gadget), Gadget.OnGadgetDragEvent, new RoutedEventHandler(this.OnDragGadget));

			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
            this.ResizeMode = ResizeMode.CanMinimize;
            this.SizeChanged += new SizeChangedEventHandler(Window1_SizeChanged);
            this.LoadGadgets();

            this.Gadgets.Loaded += new RoutedEventHandler(Gadgets_Loaded);
		}

        private void Gadgets_Loaded(object sender, RoutedEventArgs e)
        {
            this.itemsHostPanel = this.Gadgets.Template.FindName("PART_ItemsHost", this.Gadgets) as WrapPanel;
            this.itemsHostPanel.RenderTransform = new TranslateTransform(0, 0);
            this.itemsHostPanel.LayoutUpdated += new EventHandler(itemsHostPanel_LayoutUpdated);

            this.animation = new Storyboard();
            Storyboard.SetTargetProperty(this.animation, new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.XProperty }));
            Storyboard.SetTargetName(this.animation, this.itemsHostPanel.Name);
        }

        private void itemsHostPanel_LayoutUpdated(object sender, EventArgs e)
        {
            actualItemsHostPanelWidth = this.itemsHostPanel.ActualWidth;

            bool isLeftScrollingEnabled = this.determineIfLeftScrollable();
            if (isLeftScrollingEnabled != this.ScrollLeftCommand.IsEnabled)
            {
                this.scrollLeftCommand.IsEnabled = isLeftScrollingEnabled;
            }

            bool isRightScrollingEnabled = this.determineIfRightScrollable();
            if (isRightScrollingEnabled != this.ScrollRightCommand.IsEnabled)
            {
                this.ScrollRightCommand.IsEnabled = isRightScrollingEnabled;
            }
        }

        private void ScrollLeft(object arg)
        {
            TranslateTransform transform = this.itemsHostPanel.RenderTransform as TranslateTransform;
            if (transform != null)
            {
                this.AnimateScrolling(this.currentPage - 1);
            }
        }

        private void ScrollRight(object arg)
        {
            TranslateTransform transform = this.itemsHostPanel.RenderTransform as TranslateTransform;
            if (transform != null)
            {
                this.AnimateScrolling(this.currentPage + 1);
            }
        }

        private void AnimateScrolling(int page)
        {
            this.ScrollLeftCommand.IsEnabled = this.determineIfLeftScrollable();
            this.ScrollRightCommand.IsEnabled = this.determineIfRightScrollable();

            DoubleAnimation animateX = new DoubleAnimation(-1 * page * this.Gadgets.ActualWidth, new Duration(TimeSpan.FromSeconds(.4)));
            this.currentPage = page;
            animation.Children.Clear();
            animation.Children.Add(animateX);
            animation.Begin(this.Gadgets, this.Gadgets.Template, HandoffBehavior.SnapshotAndReplace);
        }

        private bool determineIfRightScrollable()
        {
            if (((this.currentPage + 1) * this.Gadgets.ActualWidth) < this.actualItemsHostPanelWidth)
            {
                return true;
            }

            return false;
        }

        private bool determineIfLeftScrollable()
        {
            if (this.currentPage > 0)
            {
                return true;
            }

            return false;
        }

        private void Window1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = SystemParameters.WorkArea.Height;
            this.Width = 200;
            this.Left = SystemParameters.WorkArea.Width - 200;
            this.Top = 0;
        }

        private void LoadGadgets()
        {
            foreach (Type gadgetType in KnownGadgets.Gadgets)
            {
                Gadget gadget = (Gadget)Activator.CreateInstance(gadgetType);
                gadget.GadgetState = GadgetState.Docked;
                gadget.Height = Gadget.DockedGadgetHeight;
                gadget.Width = Gadget.DockedGadgetWidth;
                this.Gadgets.Items.Add(gadget);
            }
        }

        private void OnDragGadget(object o, RoutedEventArgs e)
        {
            Gadget gadgetBeingDragged = (Gadget)o;
           
            if (gadgetBeingDragged.GadgetState == GadgetState.Docked)
            {
                Point mousePosition = Mouse.GetPosition(gadgetBeingDragged);
                int index = this.GetIndexOfGadgetBeingDragged(gadgetBeingDragged);

                this.Gadgets.Items.RemoveAt(index);
                AddDummyElement(index);
                previousInsertionIndex = index;
               
                windowBeingDragged = new Window();
                windowBeingDragged.Background = new SolidColorBrush(Colors.Transparent);
                windowBeingDragged.Height = undockedWindowHeight;
                windowBeingDragged.Width = undockedWindowWidth;
                gadgetBeingDragged.GadgetState = GadgetState.Floating;
                gadgetBeingDragged.Width = Double.NaN;
                gadgetBeingDragged.Height = Double.NaN;
                windowBeingDragged.WindowStyle = WindowStyle.None;
                windowBeingDragged.AllowsTransparency = true;
                windowBeingDragged.Content = gadgetBeingDragged;
                windowBeingDragged.Owner = this;
                windowBeingDragged.ShowInTaskbar = false;

                mousePointRelativeToWindowBeingDragged.X = mousePosition.X + (undockedWindowWidth - Gadget.DockedGadgetWidth);
                mousePointRelativeToWindowBeingDragged.Y = mousePosition.Y;

                windowBeingDragged.Show();
            }
            else
            {
                windowBeingDragged = (Window)gadgetBeingDragged.Parent;
                mousePointRelativeToWindowBeingDragged = Mouse.GetPosition(windowBeingDragged);
            }

            this.CaptureMouse();
            this.MouseMove += new MouseEventHandler(GadgetDockBar_MouseMove);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(GadgetDockBar_PreviewMouseLeftButtonUp);
        }

        private void GadgetDockBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.PreviewMouseLeftButtonUp -= GadgetDockBar_PreviewMouseLeftButtonUp;
            this.MouseMove -= GadgetDockBar_MouseMove;
            e.MouseDevice.Capture(null);
                       
            ClearPreviousDummyElement(previousInsertionIndex);
            previousInsertionIndex = -1;

            if (windowBeingDragged != null)
            {
                Point mousePositionRelativeToDockArea = Mouse.GetPosition(this.Gadgets);
                Gadget gadgetBeingDragged = (Gadget)windowBeingDragged.Content;

                if (mousePositionRelativeToDockArea.X >= 0 && mousePositionRelativeToDockArea.X <= this.Gadgets.ActualWidth
                        && mousePositionRelativeToDockArea.Y >= 0 && mousePositionRelativeToDockArea.Y <= this.Gadgets.ActualHeight)
                {

                    int newInsertionIndex = this.GetInsertionIndex(mousePositionRelativeToDockArea.Y);
                    windowBeingDragged.Content = null;

                    gadgetBeingDragged.Height = Gadget.DockedGadgetHeight;
                    gadgetBeingDragged.Width = Gadget.DockedGadgetWidth;
                    gadgetBeingDragged.GadgetState = GadgetState.Docked;

                    gadgetBeingDragged.UpdateLayout();
                    this.Gadgets.Items.Insert(newInsertionIndex, gadgetBeingDragged);

                    windowBeingDragged = null;
                }
                else
                {
                    gadgetBeingDragged.GadgetState = GadgetState.Floating;
                }
            }
        }

        private int GetInsertionIndex(double mousePointY)
        {
            int newInsertionIndex = (int)(mousePointY / Gadget.DockedGadgetHeight);
            if (this.currentPage != 0)
            {
                newInsertionIndex = newInsertionIndex + (this.currentPage * ((int)(this.Gadgets.ActualHeight / Gadget.DockedGadgetHeight)));
            }

            if (newInsertionIndex > this.Gadgets.Items.Count)
            {
                newInsertionIndex = this.Gadgets.Items.Count;
            }

            return newInsertionIndex;
        }

        private void GadgetDockBar_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(this);

            windowBeingDragged.Left = this.Left + mousePosition.X - mousePointRelativeToWindowBeingDragged.X;
            windowBeingDragged.Top = this.Top + mousePosition.Y - mousePointRelativeToWindowBeingDragged.Y;

            Point mousePositionRelativeToDockArea = Mouse.GetPosition(this.Gadgets);
            if (mousePositionRelativeToDockArea.X >= 0 && mousePositionRelativeToDockArea.X <= this.Gadgets.ActualWidth
                    && mousePositionRelativeToDockArea.Y >= 0 && mousePositionRelativeToDockArea.Y <= this.Gadgets.ActualHeight)
            {
                int newInsertionIndex = GetInsertionIndex(mousePositionRelativeToDockArea.Y);
                if (newInsertionIndex >= 0 && newInsertionIndex <= (this.Gadgets.Items.Count - 1))
                {
                    if (newInsertionIndex != previousInsertionIndex)
                    {
                        ClearPreviousDummyElement(previousInsertionIndex);
                        AddDummyElement(newInsertionIndex);
                        previousInsertionIndex = newInsertionIndex;
                    }
                }
            }
            else
            {
                ClearPreviousDummyElement(previousInsertionIndex);
                previousInsertionIndex = -1;
            }
        }

		private void OnShowGadgetGallery(object sender, RoutedEventArgs e)
		{
			GadgetGallery gallery = new GadgetGallery();
			gallery.Show();
		}

        private void AddDummyElement(int index)
        {
            ContentControl item = new ContentControl();
            item.Height = Gadget.DockedGadgetHeight;
            item.Width = Gadget.DockedGadgetWidth;
            this.Gadgets.Items.Insert(index, item);
        }

        private void ClearPreviousDummyElement(int index)
        {
            if((index >= 0) && (index <= (this.Gadgets.Items.Count - 1)))
            {
                this.Gadgets.Items.RemoveAt(index);
            }
        }

        private int GetIndexOfGadgetBeingDragged(Gadget o)
        {
            for (int i = 0; i < this.Gadgets.Items.Count; ++i)
            {
                if (this.Gadgets.Items[i] == o)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Cannot determine index of Gadget");
        }
    }
}
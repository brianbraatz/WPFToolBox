using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace StickyPanels
{
	public partial class Window1
	{
		private bool neverRendered = true;
		PanelControl panel = null;
		ItemsControl knownParent = null;
		ItemsControl targetItemsControl;
	
		public Window1()
		{
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			// this.InitializeComponent();
			
			// Insert code required on object creation below this point.
			EventManager.RegisterClassHandler(typeof(PanelControl), PanelControl.OnPanelDragEvent, new RoutedEventHandler(this.OnDragPanel));
			EventManager.RegisterClassHandler(typeof(PanelControl), PanelControl.OnPanelDropEvent, new RoutedEventHandler(this.OnDropPanel));
		}

		private void OnDragPanel(object o, RoutedEventArgs args)
		{
			panel = o as PanelControl;
			knownParent = this.GetItemsControlContainingPanel(panel);
			if (knownParent != null)
			{
				knownParent.Items.Remove(panel);
				this.DocumentRoot.Children.Add(panel);

				panel.Width = panel.RenderSize.Width;
				panel.IsHitTestVisible = false;
				panel.HorizontalAlignment = HorizontalAlignment.Left;
				panel.VerticalAlignment = VerticalAlignment.Top;
				Grid.SetColumnSpan(panel, this.DocumentRoot.ColumnDefinitions.Count);
			}
		}

		private void OnDropPanel(object o, RoutedEventArgs args)
		{
			panel.Width = Double.NaN;
			panel.HorizontalAlignment = HorizontalAlignment.Stretch;
			panel.IsHitTestVisible = true;
			this.DocumentRoot.Children.Remove(panel);

			//Hit-test to find out the ItemsControl under the mouse-pointer.
			targetItemsControl = null;
			VisualTreeHelper.HitTest(this.DocumentRoot, new HitTestFilterCallback(this.FilterHitTestResultsCallback), new HitTestResultCallback(this.HitTestResultCallback), new PointHitTestParameters(Mouse.PrimaryDevice.GetPosition(this.DocumentRoot)));
			if (targetItemsControl != null)
			{
				targetItemsControl.Items.Add(panel);
			}
			else
			{
				//Dropped over some random area - put it back into the known Parent
				knownParent.Items.Add(panel);
			}
		}

		private HitTestFilterBehavior FilterHitTestResultsCallback(DependencyObject target)
		{
			if (target.GetType().IsAssignableFrom(typeof(ItemsControl)))
			{
				targetItemsControl = target as ItemsControl;
				return HitTestFilterBehavior.Stop;
			}
			else
			{
				return HitTestFilterBehavior.Continue;
			}
		}

		private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
		{
			return HitTestResultBehavior.Stop;
		}

		protected override void OnContentRendered(EventArgs e)
		{
			if (this.neverRendered)
			{
				// The window takes the size of its content because SizeToContent
				// is set to WidthAndHeight in the markup. We then allow
				// it to be set by the user, and have the content take the size
				// of the window.
				this.SizeToContent = SizeToContent.Manual;

				FrameworkElement root = this.Content as FrameworkElement;
				if (root != null)
				{
					root.Width = double.NaN;
					root.Height = double.NaN;
				}

				this.neverRendered = false;
			}

			base.OnContentRendered(e);
		}

		private ItemsControl GetItemsControlContainingPanel(PanelControl panel)
		{
			if (this.IsPanelInItemsControl(panel, this.LeftPanelContainer))
			{
				return this.LeftPanelContainer;
			}
			else if (this.IsPanelInItemsControl(panel, this.RightPanelContainer))
			{
				return this.RightPanelContainer;
			}
			else
			{
				return null;
			}
		}

		private bool IsPanelInItemsControl(PanelControl panel, ItemsControl itemsControl)
		{
			foreach (PanelControl item in itemsControl.Items)
			{
				if (item == panel)
				{
					return true;
				}
			}

			return false;
		}
	}
}

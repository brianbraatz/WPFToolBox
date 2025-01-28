using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UntitledProject1
{
	public partial class Window1
	{
		public Window1()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = new ObservableCollection<string>(new List<string>(new string[] { "Red", "Green", "Blue" }));
            ListBox2.ItemsSource = new ObservableCollection<string>(new List<string>(new string[] { "North", "South", "East", "West" }));

            PrepareListBoxForDragDrop(ListBox1);
            PrepareListBoxForDragDrop(ListBox2);
        }
        private void PrepareListBoxForDragDrop(ListBox listbox)
        {
            listbox.AllowDrop = true;
            listbox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ListboxPreviewMouseLeftButtonDown);
            listbox.PreviewMouseMove += new MouseEventHandler(ListboxPreviewMouseMove);
            listbox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListboxPreviewMouseLeftButtonUp);
            listbox.Drop += new DragEventHandler(ListboxDrop);
        }
        private static DependencyProperty DraggingElementProperty = DependencyProperty.RegisterAttached(
            "DraggingElement", typeof(string), typeof(Window1), new FrameworkPropertyMetadata(null));
        private static DependencyProperty IsDownProperty = DependencyProperty.RegisterAttached(
            "IsDown", typeof(bool), typeof(Window1), new FrameworkPropertyMetadata(false));
        private static DependencyProperty StartPointProperty = DependencyProperty.RegisterAttached(
            "StartPoint", typeof(Point), typeof(Window1), new FrameworkPropertyMetadata(default(Point)));

        private void ListboxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            string s = GetBoundItemFromPoint(listbox, e.GetPosition(listbox));
            if (s != null)
            {
                listbox.SetValue(IsDownProperty, true);
                listbox.SetValue(DraggingElementProperty, s);
                listbox.SetValue(StartPointProperty, e.GetPosition(listbox));
            }
        }

        private string GetBoundItemFromPoint(ListBox box, Point point)
        {
            UIElement element = box.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == box)
                {
                    return null;
                }
                object item = box.ItemContainerGenerator.ItemFromContainer(element);
                bool itemFound = !object.ReferenceEquals(item, DependencyProperty.UnsetValue);
                if (itemFound)
                {
                    return (string)item;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
            return null;
        }
        private void ListboxPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox listbox = (ListBox)sender;

            bool isDown = (bool)listbox.GetValue(IsDownProperty);
            if (!isDown)
            {
                return;
            }

            Point startPoint = (Point)listbox.GetValue(StartPointProperty);

            if (Math.Abs(e.GetPosition(listbox).X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(e.GetPosition(listbox).Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                DragStarted(listbox);
            }
        }

        private void DragStarted(ListBox listbox)
        {
            listbox.ClearValue(IsDownProperty); // SetValue to false would also work.

            // Add the bound item, available as DraggingElement, to a DataObject, however we see fit.
            string draggingElement = (string)listbox.GetValue(DraggingElementProperty);
            DataObject d = new DataObject(DataFormats.UnicodeText, draggingElement);
            DragDropEffects effects = DragDrop.DoDragDrop(listbox, d, DragDropEffects.Copy | DragDropEffects.Move);
            if ((effects & DragDropEffects.Move) != 0)
            {
                // Move rather than copy, so we should remove from bound list.
                ObservableCollection<string> collection = (ObservableCollection<string>)listbox.ItemsSource;
                collection.Remove(draggingElement);
            }
        }
        private void ListboxDrop(object sender, DragEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            ObservableCollection<string> collection = (ObservableCollection<string>)listbox.ItemsSource;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                collection.Add((string)e.Data.GetData(DataFormats.UnicodeText, true));
                e.Effects =
                    ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                    DragDropEffects.Copy : DragDropEffects.Move;
                e.Handled = true;
            }
        }
        private void ListboxPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            listbox.ClearValue(IsDownProperty);
            listbox.ClearValue(DraggingElementProperty);
            listbox.ClearValue(StartPointProperty);
        }
    }
}
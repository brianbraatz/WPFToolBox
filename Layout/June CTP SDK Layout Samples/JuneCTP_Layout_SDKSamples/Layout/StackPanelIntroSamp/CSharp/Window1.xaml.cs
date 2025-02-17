using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StackPanel_layout
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window
	{

    // <Snippet2>
	public void changeOrientation(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem li = ((sender as ListBox).SelectedItem as ListBoxItem);
            if (li.Content.ToString() == "Horizontal")
            {
		    sp1.Orientation = System.Windows.Controls.Orientation.Horizontal;
            }
            else if (li.Content.ToString() == "Vertical")
            {
		    sp1.Orientation = System.Windows.Controls.Orientation.Vertical;
            }

        }

        public void changeHorAlign(object sender, RoutedEventArgs e)
        {
            ListBoxItem li = ((sender as ListBox).SelectedItem as ListBoxItem);
            if (li.Content.ToString() == "Left")
            {
		    sp1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            }
            else if (li.Content.ToString() == "Right")
            {
		    sp1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            }
            else if (li.Content.ToString() == "Center")
            {
		    sp1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            }
            else if (li.Content.ToString() == "Stretch")
            {
		    sp1.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            }
        }

        public void changeVertAlign(object sender, RoutedEventArgs e)
        {
            ListBoxItem li = ((sender as ListBox).SelectedItem as ListBoxItem);
            if (li.Content.ToString() == "Top")
            {
		    sp1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            }
            else if (li.Content.ToString() == "Bottom")
            {
		    sp1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            }
            else if (li.Content.ToString() == "Center")
            {
		    sp1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
            else if (li.Content.ToString() == "Stretch")
            {
		    sp1.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            }
        }
        //</Snippet2>
        
    }
}
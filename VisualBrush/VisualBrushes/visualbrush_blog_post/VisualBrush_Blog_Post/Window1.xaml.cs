
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace preview_VisualBrush_Blog_Post
{
	public partial class Window1
	{
		public Window1()
		{
			this.InitializeComponent();

            	// Insert code required on object creation below this point.
		}

        private void archTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
        }

        private void insectTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }

        private void CampfireTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
        }

        private void SunsetTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 3;
        }

        private void GGBTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 4;
        }

        private void MarketTab(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 5;
        }

        private void renderAll(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            tabControl.SelectedIndex = 2;
            tabControl.SelectedIndex = 3;
            tabControl.SelectedIndex = 4;
            tabControl.SelectedIndex = 5;
            tabControl.SelectedIndex = 0;

        }

	}
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookDemoApp {
    /// <summary>
    /// Interaction logic for UCAbout.xaml
    /// </summary>

    public partial class UCAbout : System.Windows.Controls.UserControl 
    {
        public UCAbout() 
        {
            InitializeComponent();
        }
        private void hlClick(object sender, RoutedEventArgs args) 
        {
            System.Diagnostics.Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }
        private void MediaClick(object sender, RoutedEventArgs args) 
        {
            System.Diagnostics.Process.Start("http://www.microsoft.com/silverlight/");
        }
    }
}
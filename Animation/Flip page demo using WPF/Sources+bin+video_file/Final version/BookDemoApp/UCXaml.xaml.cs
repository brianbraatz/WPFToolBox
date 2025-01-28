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
using System.Windows.Markup;
using System.IO;
using System.Xml;

namespace BookDemoApp
{
    /// <summary>
    /// Interaction logic for UCXaml.xaml
    /// </summary>

    public partial class UCXaml : System.Windows.Controls.UserControl
    {
        public UCXaml()
        {
            InitializeComponent();
        }
        private void OnLoaded(object sender, RoutedEventArgs args) 
        {
            tbXaml.Text =
                @"<Button xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">Test button</Button>";
        }
        private void DeserializeClick(object sender, RoutedEventArgs args) 
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(tbXaml.Text));
            host.Child = (UIElement) XamlReader.Load(reader);
        }
    }
}
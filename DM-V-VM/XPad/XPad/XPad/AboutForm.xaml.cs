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
using System.Windows.Shapes;

namespace XPad
{
    /// <summary>
    /// Interaction logic for AboutForm.xaml
    /// </summary>

    public partial class AboutForm : System.Windows.Window
    {

        public AboutForm()
        {
            InitializeComponent();
        }
        private void OnMouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
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

namespace WpfDesigns3.Controls
{
    /// <summary>
    /// Interaction logic for OperationsUserControl.xaml
    /// </summary>
    public partial class OperationsUserControl : UserControl
    {
        public OperationsUserControl()
        {
            InitializeComponent();
        }

        public void ShowAssemblies_Click(object sender, RoutedEventArgs e)
        {
            this.ReflectionHelper.FindAssemblies();
        }
        public void ShowTypes_Click(object sender, RoutedEventArgs e)
        {
            this.ReflectionHelper.FindTypes();
        }

        private ReflectionHelper ReflectionHelper
        {
            get
            {
                return this.DataContext as ReflectionHelper;
            }
        }
    }
}

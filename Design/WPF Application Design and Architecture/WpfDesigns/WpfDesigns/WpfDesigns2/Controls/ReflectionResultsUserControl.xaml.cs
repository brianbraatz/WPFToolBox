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

namespace WpfDesigns2.Controls
{
    /// <summary>
    /// Interaction logic for ReflectionDisplayUserControl.xaml
    /// </summary>
    public partial class ReflectionResultsUserControl : UserControl
    {
        public List<string> ReflectionResults
        {
            get { return (List<string>)GetValue(ReflectionItemsProperty); }
            set { SetValue(ReflectionItemsProperty, value); }
        }

        public static readonly DependencyProperty ReflectionItemsProperty =
            DependencyProperty.Register("ReflectionResults", typeof(List<string>), typeof(ReflectionResultsUserControl), new UIPropertyMetadata(null));


        public ReflectionResultsUserControl()
        {
            InitializeComponent();
        }
    }
}

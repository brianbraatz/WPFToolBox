using System.Windows;
using Wpf.Controls;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            Loaded += delegate
            {
                // set the frist aero buttons context menu's MinWidth to the width of the button
                //aeroButton.ContextMenu.MinWidth = aeroButton.ActualWidth;
            };

        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            if (((SplitButton)sender).Mode == SplitButtonMode.Dropdown)
                return;

            MessageBox.Show("Click event handler");
        }


    }
}

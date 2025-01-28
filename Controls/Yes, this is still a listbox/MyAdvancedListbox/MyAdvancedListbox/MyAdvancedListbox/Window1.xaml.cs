using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MyAdvancedListbox
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Result_GotFocus(object sender, RoutedEventArgs e)
        {
            giveFocus(sender);
        }
        
        private void Comment_GotFocus(object sender, RoutedEventArgs e)
        {
            giveFocus(sender);
        }

        private void giveFocus(object sender)
        {
            TextBox txt = (TextBox)sender;
            StackPanel stp = (StackPanel)txt.Parent;

            lstResults.SelectedItem = (StudentResult)stp.Tag;
        }

        private void Comment_KeyDown(object sender, KeyEventArgs e)
        {
            // if the TAB is pressed, select the next element
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                // only if not the last element is selected
                if (lstResults.SelectedIndex + 1 < lstResults.Items.Count)
                {
                    lstResults.SelectedIndex += 1;
                }
                else
                {
                    // else select the first element
                    lstResults.SelectedIndex = 0;
                }
            }
        }
    }
}

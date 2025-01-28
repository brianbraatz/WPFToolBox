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
using WPFMitsuControls;

namespace BookDemoApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
        }
        private void NextClick(object sender, RoutedEventArgs args)
        {
            if (myBook.CurrentSheetIndex < myBook.GetItemsCount() / 2)
                myBook.CurrentSheetIndex++;
        }
        private void PreviousClick(object sender, RoutedEventArgs args)
        {
            if (myBook.CurrentSheetIndex > 0)
                myBook.CurrentSheetIndex--;
        }
        
        public static string CombineFileInCurrentDirectory(string fileName)
        {
            return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
        }

        private void AutoNextClick(object sender, RoutedEventArgs e)
        {
            myBook.AnimateToNextPage(cbFromTop.SelectedIndex == 0, 700);
            myBook.Focus();
        }

        private void AutoPreviousClick(object sender, RoutedEventArgs e)
        {
            myBook.AnimateToPreviousPage(cbFromTop.SelectedIndex == 0, 700);
            myBook.Focus();
        }

        private void DisplayModeChecked(object sender, RoutedEventArgs e)
        {
            bool result = (sender as CheckBox).IsChecked.Value;
            myBook.DisplayMode = result ? BookDisplayMode.ZoomOnPage : BookDisplayMode.Normal;
        }

        private void AutoPreviousPageClick(object sender, RoutedEventArgs e)
        {
            myBook.MoveToPreviousPage();
        }

        private void AutoNextPageClick(object sender, RoutedEventArgs e)
        {
            myBook.MoveToNextPage();
        }
    }
}

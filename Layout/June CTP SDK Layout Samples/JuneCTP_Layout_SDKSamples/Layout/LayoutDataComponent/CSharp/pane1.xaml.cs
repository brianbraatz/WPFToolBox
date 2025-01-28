using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Data;

namespace ExpenseIt
{
    public partial class Pane1 : Page
    {
        NavigationWindow myWindow;
        Application app;

        private void OnLoaded(object sender, EventArgs e)
        {
            app = (Application)System.Windows.Application.Current;
            myWindow = (NavigationWindow)app.MainWindow;
        }

        private void CreateReport(object sender, RoutedEventArgs args)
        {
            Pane2 pane2 = new Pane2();
            pane2.DataContext = ListBox1.SelectedItem;
            pane2.InitializeComponent();
            myWindow.Navigate(pane2);

        }
    }
}
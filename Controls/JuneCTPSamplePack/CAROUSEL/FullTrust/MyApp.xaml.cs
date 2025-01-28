using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FullTrust
{
    public partial class AppClass: Application
    {
        private void ApplicationStartingUp(object sender, StartupEventArgs e)
        {
            Window1 mainWindow = new Window1();
            mainWindow.InitializeComponent();
            mainWindow.Show();
        }
    }
}

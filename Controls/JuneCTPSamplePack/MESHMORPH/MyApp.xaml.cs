using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;

namespace Ribbon
{
    /// <summary>
    /// Interaction logic for MyApp.xaml
    /// </summary>

    public partial class MyApp : Application
    {

        void OnStartup(object sender, StartupEventArgs e)
        {
            Window1 mainWindow = new Window1();
            mainWindow.Show();
        }

    }
}
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LEDClock
{
	public partial class MainApplication : Application
	{
		void OnLoadCompleted(object sender, StartupEventArgs e)
        {
            Window clockWindow = new LEDClock.Scene1();
            clockWindow.Title = "The LED Clock";
		clockWindow.Height = 48;
		clockWindow.Width = 188;
            clockWindow.Show();
		}
	}
}

using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;

namespace Arthur
{
	/// <summary>
	/// Interaction logic for MyApp.xaml
	/// </summary>

	public partial class MyApp : Application
	{
		void AppStartingUp(object sender, EventArgs e)
		{
			Window mainWindow = new Window1();
			mainWindow.Show();
		}
	}
}
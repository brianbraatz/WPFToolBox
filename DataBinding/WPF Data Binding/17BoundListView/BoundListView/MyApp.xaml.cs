using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace BoundListView
{
	public partial class MyApp : Application
	{
		void AppStartingUp(object sender, EventArgs e)
		{
			Window1 mainWindow = new Window1();
			mainWindow.Show();
		}

	}
}
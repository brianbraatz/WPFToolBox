using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;

namespace WrapPanel_Demo
{
	public class app : System.Windows.Application
	{
        System.Windows.Controls.Button btn1;
        System.Windows.Controls.Button btn2;
        System.Windows.Controls.Button btn3;
        System.Windows.Controls.WrapPanel myWrapPanel;
		System.Windows.Window mainWindow;

		protected override void OnStartup (StartupEventArgs e)
		{
			base.OnStartup (e);
			CreateAndShowMainWindow ();
		}

		private void CreateAndShowMainWindow ()
		{
            // <Snippet1>
        
			// Create the application's main window
			mainWindow = new System.Windows.Window();
            mainWindow.Title = "WrapPanel Sample";

            // <Snippet2>
            
			// Instantiate a new WrapPanel and set properties
			myWrapPanel = new WrapPanel();
            myWrapPanel.Background = System.Windows.Media.Brushes.Azure;
            myWrapPanel.Orientation = Orientation.Horizontal;
            // <Snippet4>
            myWrapPanel.ItemHeight = 25;
            // </Snippet4>
            
            // <Snippet3>
            myWrapPanel.ItemWidth = 75;
            // </Snippet3>
            myWrapPanel.Width = 150;
            myWrapPanel.HorizontalAlignment = HorizontalAlignment.Left;
            myWrapPanel.VerticalAlignment = VerticalAlignment.Top;

            // Define 3 button elements. Each button is sized at width of 75, so the third button wraps to the next line.
            btn1 = new Button();
            btn1.Content = "Button 1";
            btn2 = new Button();
            btn2.Content = "Button 2";
            btn3 = new Button();
            btn3.Content = "Button 3";

            // Add the buttons to the parent WrapPanel using the Children.Add method.
            myWrapPanel.Children.Add(btn1);
            myWrapPanel.Children.Add(btn2);
            myWrapPanel.Children.Add(btn3);

            // Add the WrapPanel to the MainWindow as Content
            mainWindow.Content = myWrapPanel;
			mainWindow.Show();
            // </Snippet2>
            
            //</Snippet1>
		}
	}

	internal static class EntryClass
	{
		[System.STAThread()]
		private static void Main ()
		{
			app app = new app ();
			app.Run ();
		}
	}
}

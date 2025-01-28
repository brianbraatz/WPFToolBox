using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;

namespace SDKSample
{
	public class app : Application
	{
		Window mainWindow;

        protected override void OnStartup (StartupEventArgs e)
        {
            base.OnStartup (e);
            CreateAndShowMainWindow ();
        }
        private void CreateAndShowMainWindow ()
        {
            // Create the application's main window
            mainWindow = new Window();
            mainWindow.Title = "StackPanel Sample";

            // <Snippet2>
            // Create a StackPanel and Add children
            StackPanel myStackPanel = new StackPanel();
            Border myBorder1 = new Border();
            myBorder1.Background = Brushes.SkyBlue;
            myBorder1.BorderBrush = Brushes.Black;
            myBorder1.BorderThickness = new Thickness(1);
            TextBlock txt1 = new TextBlock();
            txt1.Foreground = Brushes.Black;
            txt1.FontSize = 12;
            txt1.Text = "Stacked Item #1";
            myBorder1.Child = txt1;

            Border myBorder2 = new Border();
            myBorder2.Background = Brushes.CadetBlue;
            myBorder2.Width = 400;
            myBorder2.BorderBrush = Brushes.Black;
            myBorder2.BorderThickness = new Thickness(1);
            TextBlock txt2 = new TextBlock();
            txt2.Foreground = Brushes.Black;
            txt2.FontSize = 14;
            txt2.Text = "Stacked Item #2";
            myBorder2.Child = txt2;

            Border myBorder3 = new Border();
            myBorder3.Background = Brushes.LightGoldenrodYellow;
            myBorder3.BorderBrush = Brushes.Black;
            myBorder3.BorderThickness = new Thickness(1);
            TextBlock txt3 = new TextBlock();
            txt3.Foreground = Brushes.Black;
            txt3.FontSize = 16;
            txt3.Text = "Stacked Item #3";
            myBorder3.Child = txt3;

            Border myBorder4 = new Border();
            myBorder4.Background = Brushes.PaleGreen;
            myBorder4.Width = 200;
            myBorder4.BorderBrush = Brushes.Black;
            myBorder4.BorderThickness = new Thickness(1);
            TextBlock txt4 = new TextBlock();
            txt4.Foreground = Brushes.Black;
            txt4.FontSize = 18;
            txt4.Text = "Stacked Item #4";
            myBorder4.Child = txt4;

            Border myBorder5 = new Border();
            myBorder5.Background = Brushes.White;
            myBorder5.BorderBrush = Brushes.Black;
            myBorder5.BorderThickness = new Thickness(1);
            TextBlock txt5 = new TextBlock();
            txt5.Foreground = Brushes.Black;
            txt5.FontSize = 20;
            txt5.Text = "Stacked Item #5";
            myBorder5.Child = txt5;

            // Add the Borders to the StackPanel Children Collection
            myStackPanel.Children.Add(myBorder1);
            myStackPanel.Children.Add(myBorder2);
            myStackPanel.Children.Add(myBorder3);
            myStackPanel.Children.Add(myBorder4);
            myStackPanel.Children.Add(myBorder5);
            mainWindow.Content = myStackPanel;
            //</Snippet2>
            mainWindow.Show();
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


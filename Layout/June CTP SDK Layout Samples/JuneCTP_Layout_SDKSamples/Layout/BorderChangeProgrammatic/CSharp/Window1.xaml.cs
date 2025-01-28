using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;


namespace Border_change_programmatic
{
	public partial class Window1 : Window
	{
        // <Snippet2>
		void ChangeBG(object sender, System.Windows.RoutedEventArgs e)   
        	{
        	root.Background = System.Windows.Media.Brushes.LightSteelBlue;
	    	btn.Content = "Clicked!";
	    	Text1.Text = "The background is now LightSteelBlue"; 
        	}
        //</Snippet2>
	}
}
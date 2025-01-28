//This is a list of commonly used namespaces for a pane.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;

namespace ControlProps
{
	/// <summary>
	/// Interaction logic for Pane1.xaml
	/// </summary>

	
        public partial class Pane1 : StackPanel
	{
	        
                string str;
                FontFamily ffamily;
                Double fsize;

		// To use PaneLoaded put Loaded="PaneLoaded" in root element of .xaml file.
		// private void PaneLoaded(object sender, EventArgs e) {}
		// Sample event handler:  
		void ChangeBackground(object sender, RoutedEventArgs e)
		{
                        //<Snippet1>
            if (btn.Background == Brushes.Red)
			{
				btn.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90);
				btn.Content = "Background";
			}
			else
			{
				btn.Background = Brushes.Red;
				btn.Content = "Control background changes from red to a blue gradient.";
			}
                        //</Snippet1>
		}
		void ChangeForeground(object sender, RoutedEventArgs e)
		{
			//<Snippet2>
            if (btn1.Foreground == Brushes.Green)
			{
				btn1.Foreground = Brushes.Black;
				btn1.Content = "Foreground";
			}
			else
			{
				btn1.Foreground = Brushes.Green;
				btn1.Content = "Control foreground(text) changes from black to green.";
			}
                        //</Snippet2>
		}
		void ChangeFontFamily(object sender, RoutedEventArgs e)
		{
                        //<Snippet3>

            ffamily = btn2.FontFamily;
            str = ffamily.ToString();
			if (str == ("Arial Black"))
			{
				btn2.FontFamily = new FontFamily("Arial");
				btn2.Content = "FontFamily";
			}
			else
			{
				btn2.FontFamily = new FontFamily("Arial Black");
				btn2.Content = "Control font family changes from Arial to Arial Black.";

			}
                        //</Snippet3>
		}
		void ChangeFontSize(object sender, RoutedEventArgs e)
		{
                        //<Snippet4>
            fsize = btn3.FontSize;			
			if (fsize == 16.0)
			{
				btn3.FontSize = 10.0;
				btn3.Content = "FontSize";
			}
			else
			{
				btn3.FontSize = 16.0;
				btn3.Content = "Control font size changes from 10 to 16.";
			}
                        //</Snippet4>
		}
		void ChangeFontStyle(object sender, RoutedEventArgs e)
		{
		        //<Snippet5>
            if (btn4.FontStyle == FontStyles.Italic)
			{
				btn4.FontStyle = FontStyles.Normal;
				btn4.Content = "FontStyle";
			}
			else
			{
			btn4.FontStyle = FontStyles.Italic;
			btn4.Content = "Control font style changes from Normal to Italic.";
		    }
                        //</Snippet5>
		}
		void ChangeFontWeight(object sender, RoutedEventArgs e)
		{
		        //<Snippet6>
            if (btn5.FontWeight == FontWeights.Bold)
			{
				btn5.FontWeight = FontWeights.Normal;
				btn5.Content = "FontWeight";
			}
			else
			{
				btn5.FontWeight = FontWeights.Bold;
				btn5.Content = "Control font weight changes from Normal to Bold.";
			}
                        //</Snippet6>
		}
		void ChangeBorderBrush(object sender, RoutedEventArgs e)
		{
			//<Snippet7>
            if (btn6.BorderBrush == Brushes.Red)
			{
				btn6.BorderBrush = Brushes.Black;
				btn6.Content = "BorderBrush";

			}
			else
			{
				btn6.BorderBrush = Brushes.Red;
				btn6.Content = "Control border brush changes from red to black.";
			}
                        //</Snippet7>
		}
                void ChangeHorizontalContentAlignment(object sender, RoutedEventArgs e)
		{
			//<Snippet8>
            if (btn7.HorizontalContentAlignment == HorizontalAlignment.Left)
			{
				btn7.HorizontalContentAlignment = HorizontalAlignment.Right;
				btn7.Content = "HorizontalContentAlignment";

			}
			else
			{
				btn7.HorizontalContentAlignment=HorizontalAlignment.Left;
				btn7.Content = "Control horizontal alignment changes from left to right.";
			}
                        //</Snippet8>
		}
	         void ChangeVerticalContentAlignment(object sender, RoutedEventArgs e)
		{
			//<Snippet9>
            if (btn8.VerticalContentAlignment == VerticalAlignment.Top)
			{
				btn8.VerticalContentAlignment = VerticalAlignment.Bottom;
				btn8.Content = "VerticalContentAlignment";

			}
			else
			{
				btn8.VerticalContentAlignment=VerticalAlignment.Top;
				btn8.Content = "Control vertical alignment changes from top to bottom.";
			}
                        //</Snippet9>
		}

    }
}
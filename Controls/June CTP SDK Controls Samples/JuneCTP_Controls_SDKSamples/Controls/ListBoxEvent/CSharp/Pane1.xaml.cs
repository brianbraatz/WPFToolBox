//This is a list of commonly used namespaces for a pane.
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace ListBoxEvent
{
	
        //<Snippet6> 
        public class myColors : ObservableCollection<string>
        {
            public myColors()
            {
            Add("LightBlue");
            Add("Pink");
            Add("Red");
            Add("Purple");
            Add("Blue");
            Add("Green");
            }
        }
        //</Snippet6>

	public partial class Pane1 : Canvas
	{
                
          //<Snippet2>
          void PrintText(object sender, SelectionChangedEventArgs args)
          {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            tb.Text = "   You selected " + lbi.Content.ToString() + ".";
          }
         //</Snippet2>
		
        }
        
 }
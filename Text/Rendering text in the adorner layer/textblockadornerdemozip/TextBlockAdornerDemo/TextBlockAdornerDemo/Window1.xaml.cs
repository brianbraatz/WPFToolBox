using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextBlockAdornerDemo
{
	public partial class Window1 : System.Windows.Window
	{
		public Window1()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			AdornerLayer layer = 
				AdornerLayer.GetAdornerLayer(this.viewBox);

			TextBlock txtBlk = new TextBlock();
			txtBlk.Text = "Text in an adorner...";

			layer.Add(new TextBlockAdorner(this.viewBox, txtBlk));
		}
	}
}
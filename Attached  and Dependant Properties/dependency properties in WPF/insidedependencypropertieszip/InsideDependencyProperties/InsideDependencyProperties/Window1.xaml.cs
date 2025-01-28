using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InsideDependencyProperties
{
	public partial class Window1 : System.Windows.Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		void chkLocalValue_Checked( object sender, RoutedEventArgs e )
		{
			this.textBlk.Text = "This is the Local Value";
		}

		void chkLocalValue_Unchecked( 
			object sender, RoutedEventArgs e )
		{
			// Setting the TextBlock's Text to 
			// null or an empty string does not
			// remove the local value.  Any value 
			// which you set a dependency property 
			// to is considered its local value.
			//this.textBlk.Text = null;

			// This removes the Text property's 
			// local value so that its value can 
			// come from elsewhere, such as a Style.
			this.textBlk.ClearValue( TextBlock.TextProperty );
		}

		void chkApplyStyle_Checked( object sender, RoutedEventArgs e )
		{
			Style style = this.textBlk.FindResource( "TestStyle" ) as Style;
			this.textBlk.Style = style;
		}

		void chkApplyStyle_Unchecked( object sender, RoutedEventArgs e )
		{
			this.textBlk.Style = null;
		}
	}

	public class TextBlockToTextPropertySourceConverter 
		: IMultiValueConverter
	{
		public object Convert( 
			object[] values,  Type targetType, 
			object parameter, CultureInfo culture )
		{
			TextBlock tb = values[0] as TextBlock;

			ValueSource source = 
				DependencyPropertyHelper.GetValueSource( 
				  tb, TextBlock.TextProperty );

			return source.BaseValueSource.ToString();
		}

		public object[] ConvertBack( 
			object value,     Type[] targetTypes, 
			object parameter, CultureInfo culture )
		{
			throw new Exception( "Boom!" );
		}
	}
}
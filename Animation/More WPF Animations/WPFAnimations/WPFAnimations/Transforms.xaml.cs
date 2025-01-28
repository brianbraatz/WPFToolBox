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

namespace WPFAnimations
{
	/// <summary>
	/// Interaction logic for Transforms.xaml
	/// </summary>

	public partial class Transforms : System.Windows.Window
	{
		private bool _scaleUp = true;
		private bool _skewX = true;

		public Transforms()
		{
			InitializeComponent();
		}

		void RotatingButton_Click(object sender, RoutedEventArgs e)
		{
			if (Trans_RotatingButton.Angle == 315)
				Trans_RotatingButton.Angle = 0;
			else
				Trans_RotatingButton.Angle += 45;
		}

		void ScaleButton_Click(object sender, RoutedEventArgs e)
		{
			double scale = _scaleUp ? 0.25 : -0.25;
			Trans_ScaleButton.ScaleX += scale;
			Trans_ScaleButton.ScaleY += scale;

			if (Trans_ScaleButton.ScaleX == 2 && _scaleUp)
				_scaleUp = false;
			else if (Trans_ScaleButton.ScaleY == 0.25 && !_scaleUp)
				_scaleUp = true;
		}

		void SkewButton_Click(object sender, RoutedEventArgs e)
		{
			double skew = 30;
			if (Trans_SkewButton.AngleX == 30)
				skew = 0;

			Trans_SkewButton.AngleX = skew;
			Trans_SkewButton.AngleY = skew;
		} 
	}
}
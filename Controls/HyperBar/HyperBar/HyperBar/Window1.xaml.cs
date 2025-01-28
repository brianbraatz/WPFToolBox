using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Expression.Samples;

namespace HyperBar
{
	public partial class Window1
	{
		public Window1()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}

		private void HyperBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			// Make HyperBar respond only when the mouse is over it.
			HyperBar.Distribution = DistributionSlider.Value;
			HyperBar.Zoom = ZoomSlider.Value;
			HyperBar.UseTransparency = (bool)TransparencyCheckBox.IsChecked;
		}


	}
}
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace UntitledProject1
{
	public partial class Window1
	{
		private bool neverRendered = true;

		public Window1()
		{
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			// this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		protected override void OnContentRendered(EventArgs e)
		{
			if (this.neverRendered)
			{
				// The window takes the size of its content because SizeToContent
				// is set to WidthAndHeight in the markup. We then allow
				// it to be set by the user, and have the content take the size
				// of the window.
				this.SizeToContent = SizeToContent.Manual;

				FrameworkElement root = this.Content as FrameworkElement;
				if (root != null)
				{
					root.Width = double.NaN;
					root.Height = double.NaN;
				}

				this.neverRendered = false;
			}

			base.OnContentRendered(e);
		}
	}
}

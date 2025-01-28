using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Interop;

namespace StockWatch
{
	public partial class MainApplication: System.Windows.Application
	{
		HwndSource testHwnd;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Rect workArea = SystemParameters.WorkArea;

			HwndSourceParameters parameters = new HwndSourceParameters("MSN Stock Watch");
			parameters.PositionX = (int)((workArea.Width  / 2) - 125);
			parameters.PositionY = (int)((workArea.Height / 2) - 175 );
			parameters.UsesPerPixelOpacity = true;
			testHwnd = new HwndSource(parameters);

			object o = Application.LoadComponent(new Uri("Scene1.baml", UriKind.Relative));
			FrameworkElement child = o as FrameworkElement;

			testHwnd.RootVisual = child;
			testHwnd.Disposed += new EventHandler(testHwnd_Disposed);
		}

		void testHwnd_Disposed(object sender, EventArgs e)
		{
			this.Shutdown();
		}
	}
}

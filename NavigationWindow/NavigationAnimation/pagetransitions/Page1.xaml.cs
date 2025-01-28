using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace PageTransitions
{
	public partial class Page1
	{
		public Page1()
		{
			
		}
		private void OnExitEnded(object sender, EventArgs e)
        		{

            		Clock clock = (Clock)sender;

            		if (clock.CurrentState != ClockState.Active)
            		{
                		NavigationService ns = NavigationService.GetNavigationService(this);
                		ns.Navigate(new Uri("Page2.xaml", UriKind.RelativeOrAbsolute));
            		}
            		}
	}
}

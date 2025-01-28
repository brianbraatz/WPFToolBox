using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LEDClock
{
	public partial class Scene1 : Window
	{
        ClockMon.ClockFX ClockSource = new ClockMon.ClockFX();
        
        Binding TwentyFourHrBinding = new Binding("StrTwentyFourTime");
        Binding TwelveHrBinding = new Binding("StrTwelveTime");
        Binding USDateBinding = new Binding("StrUSDate");
        Binding EurDateBinding = new Binding("StrEurDate");

		public Scene1()
		{
			this.InitializeComponent();
            TwentyFourHrBinding.Mode = BindingMode.Default;
            TwentyFourHrBinding.Source = ClockSource;
            TwelveHrBinding.Mode = BindingMode.Default;
            TwelveHrBinding.Source = ClockSource;
            USDateBinding.Mode = BindingMode.Default;
            USDateBinding.Source = ClockSource;
            EurDateBinding.Mode = BindingMode.Default;
            EurDateBinding.Source = ClockSource;
		}

        private void DragAttempt(object sender, RoutedEventArgs e)
        {
            RootWindow.DragMove();
            e.Handled = true;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            MyApplication.Shutdown();
        }

        private void Set12HrTime(object sender, RoutedEventArgs e)
        {
            TimeText.SetBinding(TextBlock.TextProperty, TwelveHrBinding);
        }
     
        private void Set24HrTime(object sender, RoutedEventArgs e)
        { 
            TimeText.SetBinding(TextBlock.TextProperty, TwentyFourHrBinding);
        }

        private void SetUSDate(object sender, RoutedEventArgs e)
        {
            DateText.SetBinding(TextBlock.TextProperty, USDateBinding);
        }

        private void SetEurDate(object sender, RoutedEventArgs e)
        {
            DateText.SetBinding(TextBlock.TextProperty, EurDateBinding);
        }

        public void BtnSwitch1(object sender, RoutedEventArgs e)
        {
            this.OptsButton.Visibility = Visibility.Collapsed;
            this.ShowTimeDispBtn.Visibility = Visibility.Visible;
        }

        public void BtnSwitch2(object sender, RoutedEventArgs e)
        {
            this.OptsButton.Visibility = Visibility.Visible;
            this.ShowTimeDispBtn.Visibility = Visibility.Collapsed;
        }
	}
}

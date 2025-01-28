using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

namespace FloatPanelNS
{
	public partial class OrangeBauble
	{
        TimeMon ClockSource = new TimeMon();

        Binding TwentyFourHrBinding = new Binding("StrTwentyFourTime");
        Binding TwelveHrBinding = new Binding("StrTwelveTime");

		public OrangeBauble()
		{
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			this.InitializeComponent();
			// Insert code required on object creation below this point.
            TwentyFourHrBinding.Mode = BindingMode.Default;
            TwentyFourHrBinding.Source = ClockSource;
            TwelveHrBinding.Mode = BindingMode.Default;
            TwelveHrBinding.Source = ClockSource;
            
            
		}

        private void OnSceneLoaded(object obj, RoutedEventArgs args)
        {
            MainText.SetBinding(TextBlock.TextProperty, TwelveHrBinding);
        }


        private void Set12HrTime(object sender, RoutedEventArgs e)
        {
            MainText.SetBinding(TextBlock.TextProperty, TwelveHrBinding);
        }

        private void Set24HrTime(object sender, RoutedEventArgs e)
        {
            MainText.SetBinding(TextBlock.TextProperty, TwentyFourHrBinding);
        }


        public class TimeMon : INotifyPropertyChanged
        {
            protected Timer timer;
            protected DateTime now = DateTime.Now;
            protected long period = 1000;
            protected string strTwelveTime = DateTime.Now.ToString("h:mm");
            protected string strTwentyFourTime = DateTime.Now.ToString("HH:mm");

            public TimeMon()
            {
                this.timer = new Timer(new TimerCallback(this.TimerCallback), null, 0, this.Period);
            }

            public DateTime Now
            {
                get { return this.now; }
                set
                {
                    if (this.now != value)
                    {
                        this.now = value;
                        this.OnPropertyChanged("Now");
                    }
                }
            }

            public long Period
            {
                get { return this.period; }
                set
                {
                    if (this.period != value)
                    {
                        this.period = value;
                        this.OnPropertyChanged("Period");

                        this.timer.Change(0, this.Period);
                    }
                }
            }

            public string StrTwelveTime
            {
                get { return this.strTwelveTime; }
                set
                {
                    if (this.strTwelveTime != value)
                    {
                        this.strTwelveTime = value;
                        this.OnPropertyChanged("StrTwelveTime");
                    }
                }
            }

            public string StrTwentyFourTime
            {
                get { return this.strTwentyFourTime; }
                set
                {
                    if (this.strTwentyFourTime != value)
                    {
                        this.strTwentyFourTime = value;
                        this.OnPropertyChanged("StrTwentyFourTime");
                    }
                }
            }

            protected void TimerCallback(object state)
            {
                this.Now = DateTime.Now;
                this.StrTwelveTime = DateTime.Now.ToString("h:mm");
                this.StrTwentyFourTime = DateTime.Now.ToString("HH:mm");
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }




	}
}

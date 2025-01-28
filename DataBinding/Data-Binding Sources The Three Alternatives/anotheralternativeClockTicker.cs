//--------------------------------------------
// ClockTicker.cs (c) 2006 by Charles Petzold
//--------------------------------------------
using System;
using System.Windows;
using System.Windows.Threading;

namespace Petzold.DigitalClock
{
    public class ClockTicker : DependencyObject
    {
        // Define DependencyProperty.
        public static DependencyProperty DateTimeProperty = 
                DependencyProperty.Register("DateTime", typeof(DateTime), 
                                            typeof(ClockTicker));
        
        // Expose DependencyProperty as CLR property.
        public DateTime DateTime
        {
            set { SetValue(DateTimeProperty, value); }
            get { return (DateTime) GetValue(DateTimeProperty); }
        }

        // Constructor sets timer.
        public ClockTicker()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        // Timer event handler sets DateTime property.
        void TimerOnTick(object sender, EventArgs args)
        {
            DateTime = DateTime.Now;
        }
    }
}

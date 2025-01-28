//--------------------------------------------
// ClockTicker.cs (c) 2006 by Charles Petzold
//-------------------------------------------
using System;
using System.Windows;
using System.Windows.Threading;

namespace Petzold.DigitalClock
{
    public class ClockTicker
    {
        // Public event is named after property + "Changed".
        public event EventHandler DateTimeChanged;

        // Public property.
        public DateTime DateTime
        {
            get { return DateTime.Now; }
        }

        // Constructor sets timer.
        public ClockTicker()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        // Timer event handler triggers PropertyChanged event.
        void TimerOnTick(object sender, EventArgs args)
        {
            if (DateTimeChanged != null)
                DateTimeChanged(this, new EventArgs());
        }
    }
}


//--------------------------------------------
// ClockTicker.cs (c) 2006 by Charles Petzold
//--------------------------------------------
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Petzold.DigitalClock
{
    public class ClockTicker : INotifyPropertyChanged
    {
        // Event required by INotifyPropertyChanged interface.
        public event PropertyChangedEventHandler PropertyChanged;

        // Public property.
        public DateTime DateTime
        {
            get { return DateTime.Now; }
        }

        // Constructor.
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
            if (PropertyChanged != null)
                PropertyChanged(this, 
                    new PropertyChangedEventArgs("DateTime"));
        }
    }
}

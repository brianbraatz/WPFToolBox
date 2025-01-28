using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Timers;
using GadgetFramework;

namespace ClockGadget
{
    public partial class Gadget : GadgetFramework.Gadget, INotifyPropertyChanged
	{
        private DateTime currentTime;
        Timer timer = new Timer(1000);

        public DateTime CurrentTime
        {
            get { return currentTime; }
            set
            { 
                currentTime = value;
                this.OnNotifyPropertyChanged("CurrentTime");
            }
        }
	
		public Gadget() : base()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
		}

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new OnTimerTick(this.OnTimerTickCallback));
        }

        private delegate void OnTimerTick();

        private void OnTimerTickCallback()
        {
            this.CurrentTime = DateTime.Now;
        }

        public override string GadgetName
        {
            get { return "Clock"; }
        }

        public override string Description
        {
            get { return "A beautiful vector graphics clock that displays the system time"; }
        }
    }
}
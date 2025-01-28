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
using System.Windows.Forms;

namespace BatteryGadget
{
    public partial class Gadget : GadgetFramework.Gadget
    {
        System.Timers.Timer timer = new System.Timers.Timer(5000);
        PowerStatus powerStatus;

        private bool isCharging = false;
        public bool IsCharging
        {
            get 
            { 
                return isCharging; 
            }
            set
            {
                if (isCharging != value)
                {
                    isCharging = value;
                    this.OnNotifyPropertyChanged("IsCharging");
                }
            }
        }

        private int batteryStatus;
        public int BatteryStatus
        {
            get
            {
                return batteryStatus;
            }
            set
            {
                if (batteryStatus != value)
                {
                    batteryStatus = value;
                    this.OnNotifyPropertyChanged("BatteryStatus");
                }
            }
        }

        private double batteryLevel;
        public double BatteryLevel
        {
            get
            { 
                return batteryLevel; 
            }
            set
            {
                if (batteryLevel != value)
                {
                    batteryLevel = value;
                    this.OnNotifyPropertyChanged("BatteryLevel");
                }
            }
        }
	

        public Gadget()
            : base()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            UpdateData();

            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new OnTimerTick(this.OnTimerTickCallback));
        }

        private delegate void OnTimerTick();

        private void OnTimerTickCallback()
        {
            this.UpdateData();
        }

        private void UpdateData()
        {
            powerStatus = SystemInformation.PowerStatus;
            this.IsCharging = (powerStatus.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online) ? true : false;
            float batteryPercent = powerStatus.BatteryLifePercent;
            if (batteryPercent >= 0.6)
            {
                this.BatteryStatus = 1;
            }
            else if (batteryPercent >= 0.2)
            {
                this.BatteryStatus = 2;
            }
            else
            {
                this.BatteryStatus = 3;
            }

            this.BatteryLevel = (batteryPercent * 0.7) + 0.3;
        }

        public override string GadgetName
        {
            get { return "Battery"; }
        }

        public override string Description
        {
            get { return "A beautiful vector graphics battery that displays the system power statistics"; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace WPFDeveloperTools.Controls.RealTimeClockTextBlock
{
    /// <summary>
    /// RealTimeClockTextBlock is a simple textblock that will be used to display the current time.
    /// </summary>
    public class RealTimeClockTextBlock : TextBlock
    {
        #region Constructors

        static RealTimeClockTextBlock()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(RealTimeClockTextBox), new FrameworkPropertyMetadata((typeof(RealTimeClockTextBox))));
        }

        public RealTimeClockTextBlock()
            : base()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.IsEnabled = true;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);

            timer.Start();
        }

        #endregion

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
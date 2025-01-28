/* Thanks to Peter Blois in
*  microsoft.public.expression.interactivedesigner newsgroup
*  for original demo code this file is based upon  */

namespace ClockMon
{
	using System;
	using System.Windows;
	using System.Threading;
	using System.Windows.Threading;
	using System.ComponentModel;

	public class ClockFX: INotifyPropertyChanged
	{
		protected Timer timer;
		protected DateTime now = DateTime.Now;
		protected long period = 1000;
		protected string strSeconds = DateTime.Now.ToString("%s");
		protected string strTwelveTime = DateTime.Now.ToString("h:mm:ss");
		protected string strTwentyFourTime = DateTime.Now.ToString("HH:mm:ss");
		protected string strUSDate = DateTime.Now.ToString("MM:dd:yy");
		protected string strEurDate = DateTime.Now.ToString("dd:MM:yy");
		
		public ClockFX()
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
		
		public string StrSeconds
		{
			get { return this.strSeconds; }
			set
			{
				if (this.strSeconds != value)
				{
					this.strSeconds = value;
					this.OnPropertyChanged("StrSeconds");
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
		
		public string StrUSDate
		{
			get { return this.strUSDate; }
			set
			{
				if (this.strUSDate != value)
				{
					this.strUSDate = value;
					this.OnPropertyChanged("StrUSDate");
				}
			}
		}
		
		public string StrEurDate
		{
			get { return this.strEurDate; }
			set
			{
				if (this.strEurDate != value)
				{
					this.strEurDate = value;
					this.OnPropertyChanged("StrEurDate");
				}
			}
		}				

		protected void TimerCallback(object state)
		{
			this.Now = DateTime.Now;
			this.StrSeconds = DateTime.Now.ToString("%s");;
			this.StrTwelveTime = DateTime.Now.ToString("h:mm:ss");
			this.StrTwentyFourTime = DateTime.Now.ToString("HH:mm:ss");
			this.StrUSDate = DateTime.Now.ToString("MM:dd:yy");
			this.StrEurDate = DateTime.Now.ToString("dd:MM:yy");
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

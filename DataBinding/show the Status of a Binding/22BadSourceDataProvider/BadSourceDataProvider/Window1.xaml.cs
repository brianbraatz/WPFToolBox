using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Windows.Input;

namespace BadSourceDataProvider
{
	public partial class Window1 : Window, INotifyPropertyChanged
	{
        // status bar messages
        private const string openingMessage = "Opening feed...";
        private const string readyMessage = "Ready";
        
        private XmlDataProvider xdp;

		public Window1()
		{
            this.StatusMessage = openingMessage;
            InitializeComponent();
        }

		private string statusMessage;

        public string StatusMessage
		{
			get { return statusMessage; }
			set
			{
				statusMessage = value;
                if (statusMessage == openingMessage)
                {
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    this.Cursor = null;
                }
                OnPropertyChanged("StatusMessage");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            xdp = (XmlDataProvider)(this.Resources["404Provider"]);
            ((INotifyPropertyChanged)xdp).PropertyChanged += SourceChanged;
            xdp.DataChanged += DataChanged;
        }
        
        void FixSource(object sender, RoutedEventArgs args)
		{
			xdp.Source = new Uri("http://xml.weather.yahoo.com/forecastrss?p=98052");
			this.StatusMessage = openingMessage;
		}

		void Refresh(object sender, RoutedEventArgs args)
		{
			xdp.Refresh();
			this.StatusMessage = openingMessage;
        }

		void SourceChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Error")
			{
                if (xdp.Error != null)
                {
                    refreshButton.Visibility = Visibility.Collapsed;
                    fixButton.Visibility = Visibility.Visible;
                    this.StatusMessage = xdp.Error.Message;
                }
                else
                {
                    fixButton.Visibility = Visibility.Collapsed;
                    refreshButton.Visibility = Visibility.Visible;
                }
			}
		}

        void DataChanged(object sender, EventArgs e)
        {
            this.StatusMessage = readyMessage;
        }

	}
}
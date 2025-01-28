using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace StockWatch
{
    public class Quote : INotifyPropertyChanged
    {
        private string symbol;

        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double currentValue;

        public double CurrentValue
        {
            get { return Math.Round(currentValue, 2); }
            set { currentValue = value; }
        }

        private double change;

        public double Change
        {
            get { return Math.Round(change, 2); }
            set { change = value; }
        }

        public bool PostiveChange
        {
            get
            {
                if (this.change >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }
	
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

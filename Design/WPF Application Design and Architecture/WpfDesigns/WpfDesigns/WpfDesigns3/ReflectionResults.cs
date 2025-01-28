using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace WpfDesigns3
{
    public class ReflectionResults : INotifyPropertyChanged
    {
        private List<string> _results;

        public List<string> Results
        {
            get { return _results; }
            set { _results = value; OnPropertyChanged("Results"); }
        }

        public ReflectionResults()
        {
            this._results = new List<string>();
        }

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

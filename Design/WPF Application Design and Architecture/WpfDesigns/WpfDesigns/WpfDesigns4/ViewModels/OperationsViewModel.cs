using System;
using System.Collections.Generic;
using System.Text;
using WpfDesigns4.DataModels;
using System.ComponentModel;

namespace WpfDesigns4.ViewModels
{
    public class OperationsViewModel : INotifyPropertyChanged
    {
        private ReflectionHelper _reflectionHelper;
        private string _matchText;

        public OperationsViewModel()
        {
            this._matchText = "*";
        }

        public void AttachDataModel(ReflectionHelper reflectionHelper)
        {
            this._reflectionHelper = reflectionHelper;
        }

        public void FindAssemblies()
        {
            this._reflectionHelper.FindAssemblies(this._matchText);
        }

        public void FindTypes()
        {
            this._reflectionHelper.FindTypes(this._matchText);
        }

        public string MatchText
        {
            get
            {
                return this._matchText;
            }
            set
            {
                this._matchText = value;
                OnPropertyChanged("MatchText");
            }
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

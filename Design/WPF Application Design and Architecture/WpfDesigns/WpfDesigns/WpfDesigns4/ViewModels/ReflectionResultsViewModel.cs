using System;
using System.Collections.Generic;
using System.Text;
using WpfDesigns4.DataModels;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;

namespace WpfDesigns4.ViewModels
{
    public class ReflectionResultsViewModel : INotifyPropertyChanged
    {
        private ReflectionResults _reflectionResults;
        private ObservableCollection<string> _activeResults;

        public ReflectionResultsViewModel()
        {
            this._activeResults = new ObservableCollection<string>();
        }

        public void AttachDataModel(ReflectionResults reflectionResults)
        {
            this._reflectionResults = reflectionResults;
            this._reflectionResults.AssembliesChanged += new EventHandler(_reflectionResults_AssembliesChanged);
            this._reflectionResults.TypesChanged += new EventHandler(_reflectionResults_TypesChanged);
        }

        void _reflectionResults_AssembliesChanged(object sender, EventArgs e)
        {
            this._activeResults.Clear();

            //put the results data into a format for the UI
            foreach (Assembly assembly in this._reflectionResults.Assemblies)
            {
                this._activeResults.Add(assembly.FullName);
            }
        }

        void _reflectionResults_TypesChanged(object sender, EventArgs e)
        {
            this._activeResults.Clear();

            //put the results data into a format for the UI
            foreach (Type type in this._reflectionResults.Types)
            {
                this._activeResults.Add(type.FullName);
            }
        }

        /// <summary>
        /// Demonstrates how a viewmodel takes data from the data model and puts it in a format for the view
        /// </summary>
        public ObservableCollection<string> ActiveResults
        {
            get
            {
                return this._activeResults;
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace WpfDesigns3
{
    public class ReflectionHelper : INotifyPropertyChanged
    {
        public event EventHandler<ReflectionOperationEventArgs<Assembly>> AssembliesFound;
        public event EventHandler<ReflectionOperationEventArgs<Type>> TypesFound;

        private string _matchText;

        public ReflectionHelper()
        {
            this._matchText = "*";
        }

        public string MatchText
        {
            get { return _matchText; }
            set { _matchText = value; OnPropertyChanged("MatchText"); }
        }

        public void FindAssemblies()
        {
            List<Assembly> assemblyResults = new List<Assembly>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (this._matchText == "*" || assembly.FullName.Contains(this._matchText))
                {
                    assemblyResults.Add(assembly);
                }
            }

            OnAssembliesFound(new ReflectionOperationEventArgs<Assembly>(assemblyResults));
        }

        public void FindTypes()
        {
            List<Type> typeResults = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (this._matchText == "*" || type.FullName.Contains(this._matchText))
                    {
                        typeResults.Add(type);
                    }
                }
            }

            OnTypesFound(new ReflectionOperationEventArgs<Type>(typeResults));
        }

        protected virtual void OnAssembliesFound(ReflectionOperationEventArgs<Assembly> e)
        {
            if (this.AssembliesFound != null)
            {
                this.AssembliesFound(this, e);
            }
        }

        protected virtual void OnTypesFound(ReflectionOperationEventArgs<Type> e)
        {
            if (this.TypesFound != null)
            {
                this.TypesFound(this, e);
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

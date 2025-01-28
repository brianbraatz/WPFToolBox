using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace WpfDesigns4.DataModels
{
    public class ReflectionResults
    {
        public event EventHandler AssembliesChanged;
        public event EventHandler TypesChanged;

        private List<Assembly> _assemblies;
        private List<Type> _types;

        public ReflectionResults()
        {
            this._assemblies = new List<Assembly>();
            this._types = new List<Type>();
        }

        public List<Assembly> Assemblies
        {
            get { return _assemblies; }
            set
            {
                this._assemblies = value;
                OnAssembliesChanged(EventArgs.Empty);
            }
        }

        public List<Type> Types
        {
            get { return _types; }
            set
            {
                this._types = value;
                OnTypesChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnTypesChanged(EventArgs e)
        {
            if (this.TypesChanged != null)
            {
                this.TypesChanged(this, e);
            }
        }

        protected virtual void OnAssembliesChanged(EventArgs e)
        {
            if (this.AssembliesChanged != null)
            {
                this.AssembliesChanged(this, e);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace WpfDesigns4.DataModels
{
    public class ReflectionHelper
    {
        public event EventHandler<ReflectionOperationEventArgs<Assembly>> AssembliesFound;
        public event EventHandler<ReflectionOperationEventArgs<Type>> TypesFound;

        public ReflectionHelper()
        {
        }

        public void FindAssemblies(string matchText)
        {
            List<Assembly> assemblyResults = new List<Assembly>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (matchText == "*" || assembly.FullName.Contains(matchText))
                {
                    assemblyResults.Add(assembly);
                }
            }

            OnAssembliesFound(new ReflectionOperationEventArgs<Assembly>(assemblyResults));
        }

        public void FindTypes(string matchText)
        {
            List<Type> typeResults = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (matchText == "*" || type.FullName.Contains(matchText))
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
    }
}

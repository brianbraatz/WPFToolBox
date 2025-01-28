using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace WpfDesigns2
{
    public class ReflectionHelper
    {
        public ReflectionHelper()
        {
        }

        public List<string> FindAssemblyNames(string matchText)
        {
            List<string> assemblyNames = new List<string>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (matchText == "*" || assembly.FullName.Contains(matchText))
                {
                    assemblyNames.Add(assembly.FullName);
                }
            }

            return assemblyNames;
        }

        public List<string> FindTypeNames(string matchText)
        {
            List<string> typeNames = new List<string>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (matchText == "*" || type.FullName.Contains(matchText))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            return typeNames;
        }
    }
}

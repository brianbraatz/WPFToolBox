using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace WpfDesigns3
{
    public class ApplicationManager
    {
        private ReflectionHelper _reflectionHelper;
        private ReflectionResults _reflectionResults;

        /// <summary>
        /// For purposes of this example, the app manager is managing the flow of
        /// data between data models
        /// </summary>
        public ApplicationManager()
        {
            this._reflectionHelper = new ReflectionHelper();
            this._reflectionResults = new ReflectionResults();

            this._reflectionHelper.AssembliesFound += new EventHandler<ReflectionOperationEventArgs<Assembly>>(_reflectionHelper_AssembliesFound);
            this._reflectionHelper.TypesFound += new EventHandler<ReflectionOperationEventArgs<Type>>(_reflectionHelper_TypesFound);
        }

        void _reflectionHelper_AssembliesFound(object sender, ReflectionOperationEventArgs<Assembly> e)
        {
            List<string> results = new List<string>();
            foreach (Assembly assembly in e.Results)
            {
                results.Add(assembly.FullName);
            }

            this._reflectionResults.Results = results;
        }

        void _reflectionHelper_TypesFound(object sender, ReflectionOperationEventArgs<Type> e)
        {
            List<string> results = new List<string>();
            foreach (Type type in e.Results)
            {
                results.Add(type.FullName);
            }

            this._reflectionResults.Results = results;
        }

        public ReflectionHelper ReflectionHelper
        {
            get { return _reflectionHelper; }
        }
        public ReflectionResults ReflectionResults
        {
            get { return _reflectionResults; }
        }
    }
}

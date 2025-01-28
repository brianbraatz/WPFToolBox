using System;
using System.Collections.Generic;
using System.Text;
using WpfDesigns4.DataModels;
using WpfDesigns4.ViewModels;
using System.Reflection;

namespace WpfDesigns4
{
    public class ApplicationManager
    {
        private ApplicationViewState _applicationViewState;

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
            InitializeViewState();

            this._reflectionHelper.AssembliesFound += new EventHandler<ReflectionOperationEventArgs<Assembly>>(_reflectionHelper_AssembliesFound);
            this._reflectionHelper.TypesFound += new EventHandler<ReflectionOperationEventArgs<Type>>(_reflectionHelper_TypesFound);
        }

        void _reflectionHelper_AssembliesFound(object sender, ReflectionOperationEventArgs<Assembly> e)
        {
            this._reflectionResults.Assemblies = e.Results;
        }

        void _reflectionHelper_TypesFound(object sender, ReflectionOperationEventArgs<Type> e)
        {
            this._reflectionResults.Types = e.Results;
        }

        public void InitializeViewState()
        {
            this._applicationViewState = new ApplicationViewState();

            OperationsViewModel operationsViewModel = new OperationsViewModel();
            operationsViewModel.AttachDataModel(this._reflectionHelper); //could easily have been done using constructor
            this._applicationViewState.LeftSideView = operationsViewModel;

            ReflectionResultsViewModel reflectionResultsViewModel = new ReflectionResultsViewModel();
            reflectionResultsViewModel.AttachDataModel(this._reflectionResults);
            this._applicationViewState.RightSideView = reflectionResultsViewModel;
        }

        public ApplicationViewState ViewState
        {
            get
            {
                return this._applicationViewState;
            }
        }
    }
}

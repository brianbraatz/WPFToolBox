//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdHierarchy class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// The IHierarchy implementation for Adomd.NET model.
    /// </summary>
    internal class AdomdHierarchy : IHierarchy
    {
        #region Fields
        private AdomdDataModel _model;
        private AdomdObjectsRepository _repository;
        private Hierarchy _sourceHierarchy;
        private IFilterAxis _axis;
        private ObservableCollection<ILevel> _levels;
        private ReadOnlyObservableCollection<ILevel> _readOnlyLevelsView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceHierarchy">Source Adomd.NET hierarchy object.</param>
        /// <param name="repository">Repository used to associate Adomd.NET and 
        /// model objects.</param>
        public AdomdHierarchy(AdomdDataModel model, Hierarchy sourceHierarchy, AdomdObjectsRepository repository)
        {
            _model = model;
            _sourceHierarchy = sourceHierarchy;
            _repository = repository;
            _levels = new ObservableCollection<ILevel>();
            foreach (Level sourceLevel in _sourceHierarchy.Levels)
            {
                _levels.Add(new AdomdLevel(sourceLevel, this, _repository));
            }
            _readOnlyLevelsView = new ReadOnlyObservableCollection<ILevel>(_levels);
        }
       
        #region IHierarchy Members

        /// <summary>
        /// Gets caption of the hierarchy.
        /// </summary>
        public string Caption
        {
            get { return _sourceHierarchy.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the hierarchy.
        /// </summary>
        public string UniqueName
        {
            get { return _sourceHierarchy.UniqueName; }
        }

        /// <summary>
        /// Gets display folder used to group hierarchies.
        /// </summary>
        public string DisplayFolder
        {
            get { return _sourceHierarchy.DisplayFolder; }
        }

        /// <summary>
        /// Gets or sets axis that contains the hierarchy.
        /// </summary>
        public IFilterAxis Axis
        {
            get { return _axis; }
            set
            {
                Assert.ArgumentValid((value == null) || (value is AdomdAxis) || (value is AdomdFilterAxis),
                    "value", "AdomdHierarchy can be used only with AdomdAxis or AdomdFilterAxis.");
                _axis = value;
            }
        }

        /// <summary>
        /// Gets all levels within the hierarchy.
        /// </summary>
        public ReadOnlyObservableCollection<ILevel> Levels
        {
            get { return _readOnlyLevelsView; }
        }

        /// <summary>
        /// Gets model that contains this hierarchy.
        /// </summary>
        public IDataModel Model 
        {
            get { return _model; }
        }

        #endregion
    }
}

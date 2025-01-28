//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdMeasuresHierarchy class.
//  Creator: t-tomkm
//  Date Created: 7/26/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.AnalysisServices.AdomdClient;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IHierarchy implmentation for Adomd.NET model, designed to store only measures members.
    /// </summary>
    internal class AdomdMeasuresHierarchy : IHierarchy
    {
        #region Fields
        private AdomdDataModel _model;
        private string _caption;
        private string _uniqueName;
        private IFilterAxis _axis;
        private ObservableCollection<ILevel> _levels;
        private ReadOnlyObservableCollection<ILevel> _readOnlyLevelsView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Model that contains this hierarchy.</param>
        /// <param name="caption">Caption of the hierarchy.</param>
        /// <param name="uniqueName">Unique name (key) of the hierarchy.</param>
        /// <param name="measures">Collection of measures that belong to this hierarchy.</param>
        /// <param name="repository">Repository that maps Adomd.NET and PivotTable objects.</param>
        public AdomdMeasuresHierarchy(AdomdDataModel model, string caption, string uniqueName, MeasureCollection measures, AdomdObjectsRepository repository)
        {
            _model = model;
            _caption = caption;
            _uniqueName = uniqueName;
            _levels = new ObservableCollection<ILevel>();
            // For now, the measures level has the same caption and name as its hierarchy.
            _levels.Add(new AdomdMeasuresLevel(_caption, _uniqueName, this, measures, repository));
            _readOnlyLevelsView = new ReadOnlyObservableCollection<ILevel>(_levels);
        }

        #region IHierarchy Members
        /// <summary>
        /// Gets caption of the hierarchy.
        /// </summary>
        public string Caption
        {
            get { return _caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the hierarchy.
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
        }

        /// <summary>
        /// Gets display folder used to group hierarchies.
        /// </summary>
        public string DisplayFolder
        {
            get { return "Measures"; }
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
                    "value", "AdomdMeasuresHierarchy can be used only with AdomdAxis or AdomdFilterAxis.");
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

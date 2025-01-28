//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdMeasuresLevel class.
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
    /// ILevel implementation for Adomd.NET model, design to store only measures members.
    /// </summary>
    internal class AdomdMeasuresLevel : ILevel
    {
        #region Fields
        private string _caption;
        private string _uniqueName;
        private AdomdMeasuresHierarchy _hierarchy;
        private ObservableCollection<IMember> _members;
        private ReadOnlyObservableCollection<IMember> _readOnlyMembersView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="caption">Caption of this level.</param>
        /// <param name="uniqueName">Unique name (key) of this level.</param>
        /// <param name="hierarchy">Hierarchy that contains this level.</param>
        /// <param name="measures">Collection of measures that belong to this level.</param>
        public AdomdMeasuresLevel(string caption, string uniqueName,
            AdomdMeasuresHierarchy hierarchy, MeasureCollection measures, AdomdObjectsRepository repository)
        {
            _caption = caption;
            _uniqueName = uniqueName;
            _hierarchy = hierarchy;
            _members = new ObservableCollection<IMember>();
            foreach (Measure measure in measures)
            {
                _members.Add(new AdomdMeasuresMember(measure, this, repository));
            }
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
        }

        #region ILevel Members

        /// <summary>
        /// Gets caption of the level.
        /// </summary>
        public string Caption
        {
            get { return _caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the level.
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
        }

        /// <summary>
        /// Gets the ordinal position of the level within the parent hierarchy.
        /// It always returns zero for AdomMeasuresLevel.
        /// </summary>
        public int LevelNumber
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the hierarchy that contains this level.
        /// </summary>
        public IHierarchy Hierarchy
        {
            get { return _hierarchy; }
        }

        /// <summary>
        /// Gets all members within this level.
        /// </summary>
        public ReadOnlyObservableCollection<IMember> Members
        {
            get { return _readOnlyMembersView; }
        }
        #endregion
    }
}

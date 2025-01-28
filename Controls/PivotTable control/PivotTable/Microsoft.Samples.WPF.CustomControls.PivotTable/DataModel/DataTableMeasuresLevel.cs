//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableMeasuresLevel class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// ILevel implementation that stores measures members for DataTable model.
    /// </summary>
    public class DataTableMeasuresLevel : ILevel
    {
        #region Fields
        private IHierarchy _hierarchy;
        private ObservableCollection<IMember> _members;
        private ReadOnlyObservableCollection<IMember> _readOnlyMembersView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hierarchy">Hierarchy that contains this level.</param>
        /// <param name="columns">Collection of columns that define measures. DataType of each
        /// column must be numeric.</param>
        public DataTableMeasuresLevel(IHierarchy hierarchy, IEnumerable<DataColumn> columns)
        {
            Assert.ArgumentNotNull(hierarchy, "hierarchy", "Hierarchy cannot be null.");
            Assert.ArgumentNotNull(columns, "columns", "Columns collection cannot be null.");
            _hierarchy = hierarchy;
            _members = new ObservableCollection<IMember>();
            foreach (DataColumn column in columns)
            {
                _members.Add(new DataTableMeasuresMember(this, column));
            }
            
            Assert.ArgumentValid(_members.Count > 0, "columns", "Columns collection must contain at least one item.");
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
        }

        #region ILevel Members

        /// <summary>
        /// Gets caption of this level.
        /// </summary>
        public string Caption
        {
            get { return "Measures"; }
        }

        /// <summary>
        /// Gets unique name (key) of this level.
        /// </summary>
        public string UniqueName
        {
            get { return string.Format("{0}.[{1}]", _hierarchy.UniqueName, Caption); }
        }

        /// <summary>
        /// Gets the ordinal position of this level within the parent hierarchy.
        /// Always returns 0.
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

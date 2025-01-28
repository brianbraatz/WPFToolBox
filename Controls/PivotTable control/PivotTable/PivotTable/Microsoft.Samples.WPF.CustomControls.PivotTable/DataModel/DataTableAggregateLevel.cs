//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableAggregateLevel class.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// ILevel implementation for DataTable model. It contains only one, aggregate member.
    /// </summary>
    public class DataTableAggregateLevel : ILevel
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
        /// <param name="nextLevelMembers">Collection that stores children for this level's member.</param>
        public DataTableAggregateLevel(IHierarchy hierarchy, IEnumerable<DataTableMember> nextLevelMembers)
        {
            Assert.ArgumentNotNull(hierarchy, "hierarchy", "Hierarchy cannot be null.");
            Assert.ArgumentNotNull(nextLevelMembers, "nextLevelMembers", "Next level members cannot be null.");
            _hierarchy = hierarchy;
            _members = new ObservableCollection<IMember>();
            _members.Add(new DataTableAggregateMember(this, nextLevelMembers));
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
        }

        #region ILevel Members

        /// <summary>
        /// Gets caption of this level.
        /// </summary>
        public string Caption
        {
            get { return "All members"; }
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

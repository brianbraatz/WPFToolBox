//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableAggregateMember class.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IMember implementation for DataTable model. It stores aggregate member for 
    /// first level.
    /// </summary>
    public class DataTableAggregateMember : IDataTableDimensionMember
    {
        #region Fields
        private ILevel _level;
        private ObservableCollection<IMember> _children;
        private ReadOnlyObservableCollection<IMember> _readOnlyChildrenView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">Level that contains this member.</param>
        /// <param name="children">Collection that stores children of this member.</param>
        public DataTableAggregateMember(ILevel level, IEnumerable<DataTableMember> children)
        {
            Assert.ArgumentNotNull(level, "level", "Level cannot be null.");
            Assert.ArgumentNotNull(children, "children", "Children collection cannot be null.");
            _level = level;
            _children = new ObservableCollection<IMember>();
            foreach (DataTableMember member in children)
            {
                member.ParentMember = this;
                _children.Add(member);
            }
            _readOnlyChildrenView = new ReadOnlyObservableCollection<IMember>(_children);
        }

        #region IMember Members
        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        public string Caption
        {
            get { return "All members"; }
        }

        /// <summary>
        /// Gets unique name (key) of the member.
        /// </summary>
        public string UniqueName
        {
            get { return string.Format("{0}.[{1}]", _level.UniqueName, Caption); }
        }

        /// <summary>
        /// Gets the level that contains this member.
        /// </summary>
        public ILevel Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Gets the parent member of this member.
        /// </summary>
        public IMember ParentMember
        {
            get { return null; }
        }

        /// <summary>
        /// Gets all members that are children of this member (i.e. this member
        /// is their parent member).
        /// </summary>
        public ReadOnlyObservableCollection<IMember> Children
        {
            get { return _readOnlyChildrenView; }
        }

        /// <summary>
        /// Gets number of children. It allows to check if the member is not leaf without
        /// retreiving all children.
        /// </summary>
        public bool HasChildren
        {
            get { return _readOnlyChildrenView.Count > 0; }
        }

        #endregion

        #region IDataTableMember Members

        public void FilterRows(ISet<DataRow> rows)
        { }

        #endregion
    }
}

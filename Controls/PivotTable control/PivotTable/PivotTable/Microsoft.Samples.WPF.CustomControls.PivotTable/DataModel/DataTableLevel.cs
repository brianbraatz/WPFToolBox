//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableLevel class.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// ILevel implementation for DataTable model. It contains dimension members.
    /// </summary>
    public class DataTableLevel : ILevel
    {
        #region Fields
        private IHierarchy _hierarchy;
        private DataColumn _column;
        private ObservableCollection<IMember> _members;
        private ReadOnlyObservableCollection<IMember> _readOnlyMembersView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hierarchy">Hierarchy that contains this level.</param>
        /// <param name="column">Column that defines the hierarchy.</param>
        public DataTableLevel(IHierarchy hierarchy, DataColumn column)
        {
            Assert.ArgumentNotNull(hierarchy, "hierarchy", "Hierarchy cannot be null.");
            Assert.ArgumentNotNull(column, "column", "Column cannot be null.");
            
            _hierarchy = hierarchy;
            _column = column;
            _members = new ObservableCollection<IMember>();
            FillMembers();
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
        }

        private void FillMembers()
        {
            Dictionary<object, DataTableMember> membersDictionary = new Dictionary<object, DataTableMember>();
            foreach (DataRow row in _column.Table.Rows)
            {
                object rowValue = row[_column];
                if (membersDictionary.ContainsKey(rowValue))
                {
                    membersDictionary[rowValue].AddRow(row);
                }
                else
                {
                    membersDictionary[rowValue] = new DataTableMember(this, _column, row);
                }
            }
            foreach (DataTableMember member in membersDictionary.Values)
            {
                _members.Add(member);
            }
        }

        #region ILevel Members
        /// <summary>
        /// Gets caption of this level.
        /// </summary>
        public string Caption
        {
            get { return _column.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of this level.
        /// </summary>
        public string UniqueName
        {
            get 
            {
                string memberUniqueName = Caption.Replace('.', '_');
                return string.Format("{0}.[{1}]", _hierarchy.UniqueName, memberUniqueName);
            }
        }

        /// <summary>
        /// Gets the ordinal position of this level within the parent hierarchy.
        /// </summary>
        public int LevelNumber
        {
            get { return 1; }
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

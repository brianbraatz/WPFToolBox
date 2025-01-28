//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableMember class.
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
    /// IMember implementation for DataTable model. It stores members from a DataColumn.
    /// </summary>
    public class DataTableMember : IDataTableDimensionMember
    {
        #region Fields
        private ILevel _level;
        private DataColumn _column;
        private IMember _parentMember;
        private Set<DataRow> _rows;
        private DataRow _firstRow;
        private ReadOnlyObservableCollection<IMember> _readOnlyChildrenView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">Level that contains this member.</param>
        /// <param name="column">Column from DataTable that defines the dimension hierarchy.</param>
        /// <param name="firstRow">Row from DataTable that defines value for the member.</param>
        public DataTableMember(ILevel level, DataColumn column, DataRow firstRow)
        {
            Assert.ArgumentNotNull(level, "level", "Level cannot be null.");
            Assert.ArgumentNotNull(column, "column", "DataColumn cannot be null.");
            Assert.ArgumentNotNull(firstRow, "firstRow", "First row cannot be null.");

            _level = level;
            _column = column;
            _rows = new Set<DataRow>();
            _rows.Add(firstRow);
            _firstRow = firstRow;

            ObservableCollection<IMember> children = new ObservableCollection<IMember>();
            _readOnlyChildrenView = new ReadOnlyObservableCollection<IMember>(children);
        }

        /// <summary>
        /// Adds new row that define the member.
        /// </summary>
        /// <param name="row">Row to add. For the column that defines the hierarchy,
        /// the value of this row must be the same as the value of first row, passed 
        /// in the constructor.</param>
        public void AddRow(DataRow row)
        {
            Assert.ArgumentNotNull(row, "row", "Row cannot be null.");
            Assert.ArgumentValid(Object.Equals(row[_column], _firstRow[_column]), "row",
                "Given row doesn't have the same value as other rows in this member.");
            _rows.Add(row);
        }

        #region IMember Members

        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        public string Caption
        {
            get { return _firstRow[_column] != null ? _firstRow[_column].ToString() : "(Null)"; }
        }

        /// <summary>
        /// Gets unique name (key) of the member.
        /// </summary>
        public string UniqueName
        {
            get 
            {
                string memberUniqueName = Caption.Replace('.', '_');
                return string.Format("{0}.[{1}]", _level.UniqueName, memberUniqueName);
            }
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
            get { return _parentMember; }
            set { _parentMember = value; }
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
            get { return false; }
        }

        #endregion

        #region IDataTableMember Members

        /// <summary>
        /// Removes all rows that don't belong to this members.
        /// </summary>
        /// <param name="rows">Rows set to filter.</param>
        public void FilterRows(ISet<DataRow> rows)
        {
            rows.Intersect(_rows);
        }

        #endregion
    }
}

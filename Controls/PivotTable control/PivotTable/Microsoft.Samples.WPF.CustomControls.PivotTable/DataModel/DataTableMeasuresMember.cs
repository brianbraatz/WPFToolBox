//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableMeasuresMember class.
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
    /// IMeasuresMember implementation for DataTable model.
    /// </summary>
    public class DataTableMeasuresMember : IMeasuresMember
    {
        #region Fields
        private ILevel _level;
        private DataColumn _column;
        private AggregateFunction _aggregateFunction;
        private ReadOnlyObservableCollection<IMember> _readOnlyChildrenView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">Level that contains this member.</param>
        /// <param name="column">Columns that define this measure. DataType
        /// of this column must be numeric.</param>
        public DataTableMeasuresMember(ILevel level, DataColumn column)
        {
            Assert.ArgumentNotNull(level, "level", "Level cannot be null.");
            Assert.ArgumentNotNull(column, "column", "Column cannot be null.");
            _level = level;
            _column = column;
            _aggregateFunction = AggregateFunction.Sum;
            ObservableCollection<IMember> children = new ObservableCollection<IMember>();
            _readOnlyChildrenView = new ReadOnlyObservableCollection<IMember>(children);
        }

        /// <summary>
        /// DataColumn that defines this measure.
        /// </summary>
        public DataColumn DataColumn
        {
            get { return _column; }
        }

        #region IMeasuresMember Members

        /// <summary>
        /// Gets folder that is used to group measures.
        /// </summary>
        public string DisplayFolder
        {
            get { return "Measures"; }
        }

        /// <summary>
        /// Gets a value indicating whether this member supports custom aggregate function.
        /// </summary>
        public bool CanSetAggregateFunction
        {
            get { return true; }
        }

        /// <summary>
        /// Gets and sets aggregate function for this member.
        /// </summary>
        public AggregateFunction AggregateFunction
        {
            get { return _aggregateFunction; }
            set
            {
                Assert.ArgumentNotNull(value, "value", "Aggregate function cannot be null.");
                _aggregateFunction = value;
            }
        }

        #endregion

        #region IMember Members

        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        public string Caption
        {
            get { return _column.Caption; }
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
            get { return false; }
        }

        #endregion
    }
}

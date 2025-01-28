//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableFilterAxis class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IFilterAxis implementation for DataTable model.
    /// </summary>
    public class DataTableFilterAxis : FilterAxis
    {
        #region Fields
        private DataTable _table;
        private DataTableHierarchiesObservableCollection _hierarchies;
        private Set<DataRow> _possibleRows;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataTable">DataTable from the model.</param>
        public DataTableFilterAxis(DataTable dataTable)
        {
            _table = dataTable;
            _possibleRows = new Set<DataRow>();
            _hierarchies = new DataTableHierarchiesObservableCollection();
            ((INotifyCollectionChanged)_hierarchies).CollectionChanged += OnHierarchiesChanged;
            ((INotifyCollectionChanged)Tuples).CollectionChanged += OnTuplesChanged;
            RecreatePossibleRows();
        }

        private void OnTuplesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecreatePossibleRows();
        }

        /// <summary>
        /// Gets all hierarchies within the axis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be used to add new hierarchies to the axis.
        /// </para>
        /// <para>
        /// A hierarchy can belong only to one axis. When user tries to add
        /// hierarchy that already belongs to different axis, the Add method
        /// should throw an InvalidOperationException.
        /// </para>
        /// </remarks>
        public override ObservableCollection<IHierarchy> Hierarchies
        {
            get { return _hierarchies; }
        }

        /// <summary>
        /// Removes from the given set rows that should not be used to calculate aggregate
        /// function.
        /// </summary>
        /// <param name="rows">Set of rows to filter.</param>
        public void FilterRows(ISet<DataRow> rows)
        {
            rows.Intersect(_possibleRows);
        }

        private void RecreatePossibleRows()
        {
            if (Tuples.Count > 0)
            {
                _possibleRows.Clear();
                foreach (Tuple tuple in Tuples)
                {
                    _possibleRows.Union(GetRowsForTuple(tuple));
                }
            }
            else
            {
                _possibleRows.Add(_table.Rows);
            }
        }

        private ISet<DataRow> GetRowsForTuple(Tuple tuple)
        {
            Set<DataRow> result = new Set<DataRow>(_table.Rows);
            foreach (IDataTableDimensionMember member in tuple)
            {
                member.FilterRows(result);
            }
            return result;
        }
    }
}

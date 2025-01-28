//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableDataModel class.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IDataModel implementation that uses DataTable.
    /// </summary>
    public class DataTableDataModel : IDataModel
    {
        #region Fields
        private const int RegularAxesCount = 16;
        private readonly Type[] NumericTypes = new Type[] {
            typeof(Byte), typeof(SByte), typeof(Int32), typeof(Int16), typeof(UInt32), typeof(UInt16),
            typeof(Single), typeof(Double), typeof(Int64), typeof(UInt64), typeof(Decimal)
        };

        private DataTable _table;
        private DataTableFilterAxis _filterAxis;
        private ObservableCollection<IAxis> _regularAxes;
        private ReadOnlyObservableCollection<IAxis> _readOnlyRegularAxesView;

        private ObservableCollection<IHierarchy> _dimensionHierarchies;
        private ReadOnlyObservableCollection<IHierarchy> _readOnlyDimensionHierarchiesView;
        private IHierarchy _measuresHierarchy;
        #endregion

        #region Initializing
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="table">DataTable for the model. It must contain at least one
        /// numeric column.</param>
        public DataTableDataModel(DataTable table)
        {
            _table = table;
            CreateAxes();
            CreateDimensionHierarchies();
            CreateMeasureHierarchy();
        }

        private void CreateAxes()
        {
            _filterAxis = new DataTableFilterAxis(_table);
            ((INotifyCollectionChanged)_filterAxis.Tuples).CollectionChanged += OnAxisChanged;
            _regularAxes = new ObservableCollection<IAxis>();
            for (int i = 0; i < RegularAxesCount; i++)
            {
                DataTableAxis axis = new DataTableAxis();
                ((INotifyCollectionChanged)axis.Tuples).CollectionChanged += OnAxisChanged;
                _regularAxes.Add(axis);
            }
            _readOnlyRegularAxesView = new ReadOnlyObservableCollection<IAxis>(_regularAxes);
        }

        private void OnAxisChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCellsChanged(EventArgs.Empty);
        }

        private void CreateDimensionHierarchies()
        {
            _dimensionHierarchies = new ObservableCollection<IHierarchy>();
            foreach (DataColumn column in _table.Columns)
            {
                if (!IsNumericType(column.DataType))
                {
                    _dimensionHierarchies.Add(new DataTableHierarchy(this, column));
                }
            }
            _readOnlyDimensionHierarchiesView = new ReadOnlyObservableCollection<IHierarchy>(_dimensionHierarchies);
        }

        private void CreateMeasureHierarchy()
        {
            List<DataColumn> measureColumns = new List<DataColumn>();
            foreach (DataColumn column in _table.Columns)
            {
                if (IsNumericType(column.DataType))
                {
                    measureColumns.Add(column);
                }
            }
            Assert.ArgumentValid(measureColumns.Count > 0, "table", 
                "DataTable must have at least one column with numeric type");
            _measuresHierarchy = new DataTableMeasuresHierarchy(this, measureColumns);
        }

        private bool IsNumericType(Type type)
        {
            foreach (Type numericType in NumericTypes)
            {
                if (type == numericType)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IDataModel Members
        /// <summary>
        /// Gets all regular (not filter) axes within the model.
        /// </summary>
        public ReadOnlyObservableCollection<IAxis> RegularAxes
        {
            get { return _readOnlyRegularAxesView; }
        }

        /// <summary>
        /// Gets filter axis within the model.
        /// </summary>
        public IFilterAxis FilterAxis
        {
            get { return _filterAxis; }
        }

        /// <summary>
        /// Gets all dimensions hierarchies within the model.
        /// </summary>
        public ReadOnlyObservableCollection<IHierarchy> DimensionsHierarchies
        {
            get { return _readOnlyDimensionHierarchiesView; }
        }

        /// <summary>
        /// Gets measures hierarchy within the model.
        /// </summary>
        public IHierarchy MeasuresHierarchy
        {
            get { return _measuresHierarchy; }
        }

        /// <summary>
        /// Gets cells at given position.
        /// </summary>
        /// <param name="cellPosition">Collection of tuples, that uniquely identify the cell.
        /// First tuple must be from first axis, second must be from second axis, and so on.
        /// </param>
        /// <returns></returns>
        public Cell GetCell(IEnumerable<Tuple> cellPosition)
        {
            DataTableMeasuresMember measuresMember = FindMeasuresMember(cellPosition);
            if (measuresMember == null)
            {
                return null;
            }

            Set<DataRow> rowsSet = new Set<DataRow>(_table.Rows);

            _filterAxis.FilterRows(rowsSet);
            FilterRows(rowsSet, cellPosition);

            object cellValue = CalculateAggregateFunction(rowsSet, measuresMember);
            int[] cellCoordinates = GetCellCoordinates(cellPosition);
            return new Cell(cellValue, cellCoordinates);
        }

        /// <summary>
        /// Event raised when cells were changed.
        /// </summary>
        public event EventHandler CellsChanged;
        #endregion

        #region Private methods
        private object CalculateAggregateFunction(IEnumerable<DataRow> rows, DataTableMeasuresMember measuresMember)
        {
            List<object> arguments = new List<object>();
            foreach (DataRow row in rows)
            {
                arguments.Add(row[measuresMember.DataColumn]);
            }
            if (arguments.Count == 0)
            {
                return null;
            }
            return measuresMember.AggregateFunction.ComputeValue(arguments);
        }

        private void FilterRows(ISet<DataRow> rows, IEnumerable<Tuple> tuples)
        {
            foreach (Tuple tuple in tuples)
            {
                foreach (IMember member in tuple)
                {
                    if (member is DataTableMember)
                    {
                        DataTableMember dataTableMember = (DataTableMember)member;
                        dataTableMember.FilterRows(rows);
                    }
                }
            }
        }

        private DataTableMeasuresMember FindMeasuresMember(IEnumerable<Tuple> tuples)
        {
            foreach (Tuple tuple in tuples)
            {
                foreach (IMember member in tuple)
                {
                    if (member is DataTableMeasuresMember)
                    {
                        return (DataTableMeasuresMember)member;
                    }
                }
            }
            return null;
        }

        private int[] GetCellCoordinates(IEnumerable<Tuple> tuples)
        {
            List<int> result = new List<int>();
            int axisNumber = 0;
            foreach (Tuple tuple in tuples)
            {
                IAxis axis = _regularAxes[axisNumber++];
                result.Add(axis.Tuples.IndexOf(tuple));
            }
            return result.ToArray();
        }

        private void OnCellsChanged(EventArgs e)
        {
            if (CellsChanged != null)
            {
                CellsChanged(this, e);
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        { }

        #endregion
    }
}

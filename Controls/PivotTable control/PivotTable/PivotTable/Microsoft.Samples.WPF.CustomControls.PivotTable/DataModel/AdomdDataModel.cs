//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdDataModel class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Adomd = Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// The PivotTable data model implementation for Adomd.NET.
    /// </summary>
    public class AdomdDataModel : IDataModel
    {
        private const int MaximumSupportedAxes = 16;

        #region Fields
        private AdomdItemsSource _itemsSource;
        private AdomdObjectsRepository _repository;
        private Adomd.CellSet _cellSet;

        private ObservableCollection<IAxis> _axes;
        private ReadOnlyObservableCollection<IAxis> _readOnlyAxesView;
        private AdomdFilterAxis _filterAxis;
        private ObservableCollection<IHierarchy> _hierarchies;
        private ReadOnlyObservableCollection<IHierarchy> _readOnlyHierarchiesView;
        private AdomdMeasuresHierarchy _measuresHierarchy;

        private bool _disposed = false;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemsSource">Object that stores connection to Analysis Services and cube name.</param>
        public AdomdDataModel(AdomdItemsSource itemsSource)
        {
            Assert.ArgumentNotNull(itemsSource, "itemsSource", "Items source cannot be null.");

            _itemsSource = itemsSource;
            _repository = new AdomdObjectsRepository();            
            _itemsSource.Connection.Open();
            
            CreateDimensionsHierarchies();
            CreateMeasuresHierarchy();
            CreateAxes();
        }

        #region Initialization
        private void CreateDimensionsHierarchies()
        {
            _hierarchies = new ObservableCollection<IHierarchy>();
            Adomd.CubeDef cube = _itemsSource.Connection.Cubes[_itemsSource.CubeName];
            foreach (Adomd.Dimension dimension in cube.Dimensions)
            {
                foreach (Adomd.Hierarchy hierarchy in dimension.Hierarchies)
                {
                    _hierarchies.Add(new AdomdHierarchy(this, hierarchy, _repository));
                }
            }
            _readOnlyHierarchiesView = new ReadOnlyObservableCollection<IHierarchy>(_hierarchies);
        }

        private void CreateMeasuresHierarchy()
        {
            Adomd.CubeDef cube = _itemsSource.Connection.Cubes[_itemsSource.CubeName];
            _measuresHierarchy = new AdomdMeasuresHierarchy(this, "Measures", "Measures", cube.Measures, _repository);
        }

        private void CreateAxes()
        {
            _axes = new ObservableCollection<IAxis>();
            for (int i = 0; i < MaximumSupportedAxes; i++)
            {
                AdomdAxis axis = new AdomdAxis();
                NotifyCollectionChangedEventHandler handler = null;
                
                ((INotifyCollectionChanged)axis.Tuples).CollectionChanged += OnTuplesChanged;
                _axes.Add(axis);
            }
            _readOnlyAxesView = new ReadOnlyObservableCollection<IAxis>(_axes);

            _filterAxis = new AdomdFilterAxis();
            ((INotifyCollectionChanged)_filterAxis.Tuples).CollectionChanged += OnTuplesChanged;
        }
        #endregion

        #region Handling changes

        /// <summary>
        /// Event raised when cells were changed.
        /// </summary>
        /// <remarks>
        /// This event is invoked before all other handlers for axes' Tuples.CollectionChanged
        /// events.
        /// </remarks>
        public event EventHandler CellsChanged;

        private void OnCellsChanged(EventArgs e)
        {
            if (CellsChanged != null)
            {
                CellsChanged(this, e);
            }
        }

        private void OnTuplesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _cellSet = null;
            if (CheckIfAnyAxisContainsHierarchy())
            {
                string mdxQuery = CreateMdxQuery();
                Trace.WriteLine(mdxQuery);
                Adomd.AdomdCommand command = new Adomd.AdomdCommand(mdxQuery, _itemsSource.Connection);
                _cellSet = command.ExecuteCellSet();
                _repository.RegistersTuplesFromCellSet(_cellSet);
            }
            OnCellsChanged(EventArgs.Empty);
        }

        private bool CheckIfAnyAxisContainsHierarchy()
        {
            foreach (IAxis axis in _axes)
            {
                if (axis.Hierarchies.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private string CreateMdxQuery()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("SELECT");
            bool firstAxisWritten = false;
            int notEmptyAxisNumber = 0;
            for (int i = 0; i < _axes.Count; i++)
            {                
                AdomdAxis axis = (AdomdAxis)_axes[i];
                if (axis.Hierarchies.Count > 0)
                {
                    if (firstAxisWritten)
                    {
                        result.AppendLine(", ");
                    }
                    else
                    {
                        firstAxisWritten = true;
                    }
                    axis.GenerateMdxAxisExpression(result, notEmptyAxisNumber++);
                }
            }
            result.AppendLine();
            result.AppendFormat("FROM [{0}]", _itemsSource.CubeName);
            result.AppendLine();
            _filterAxis.GenerateMdxWhereExpression(result);
            return result.ToString();
        }
        #endregion

        #region IDataModel Members

        /// <summary>
        /// Gets all axes within the model.
        /// </summary>
        public ReadOnlyObservableCollection<IAxis> RegularAxes
        {
            get { return _readOnlyAxesView; }
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
            get { return _readOnlyHierarchiesView; }
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
        /// First not-null tuple must be from first not-empty axis, second must be from second 
        /// not empty axis, and so on. Null values are ignored.
        /// </param>
        /// <returns></returns>
        public Cell GetCell(IEnumerable<Tuple> cellPosition)
        {
            Assert.ArgumentNotNull(cellPosition, "cellPosition", "Cell position cannot be null.");

            if (_cellSet == null)
            {
                return null;
            }
            List<int> indexes = new List<int>();
            List<int> cellCoordinates = new List<int>();
            foreach (Tuple tuple in cellPosition)
            {
                if (tuple == null)
                {
                    cellCoordinates.Add(-1);
                    continue;
                }
                int tupleOrdinalPosition = _repository.GetTupleOrdinalPosition(tuple);
                if (tupleOrdinalPosition == -1)
                {
                    return null;
                }
                indexes.Add(tupleOrdinalPosition);
                cellCoordinates.Add(tupleOrdinalPosition);
            }
            return new Cell(_cellSet.Cells[indexes].Value, cellCoordinates.ToArray());
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was called from 
        /// IDisposable.Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _itemsSource.Connection.Dispose();
            }
            _disposed = true;
        }

        #endregion
    }
}

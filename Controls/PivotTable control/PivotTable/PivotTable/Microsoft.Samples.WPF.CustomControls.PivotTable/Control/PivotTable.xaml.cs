//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: PivotTable control.
//  Creator: t-tomkm
//  Date Created: 08/21/2006
//---------------------------------------------------------------------
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// PivotTable control.
    /// </summary>
    public partial class PivotTable : UserControl
    {
        private object _itemsSource;
        private IDataModel _model;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PivotTable()
        {
            InitializeComponent();
        }

        #region Properties
        /// <summary>
        /// Gets or sets items source for the PivotTable.
        /// </summary>
        /// <remarks>
        /// Currently PivotTable supports only AdomdItemsSource and DataTable as items source.
        /// </remarks>
        public object ItemsSource
        {
            get { return _itemsSource; }
            set 
            {
                IDataModel newModel = CreateModelFromItemsSource(value);
                if (newModel == null)
                {
                    throw new ArgumentException("PivotTable cannot work with given items source");
                }
                if (_model != null)
                {
                    _model.Dispose();
                }
                _itemsSource = value;
                _model = newModel;
                AssignPartsToModel();
            }
        }

        /// <summary>
        /// Gets all regular (not filter) axes within the pivot table. If items source is not set,
        /// returns null.
        /// </summary>
        public IList<IAxis> RegularAxes
        {
            get 
            {   
                if (_model == null)
                {
                    return null;
                }

                List<IAxis> result = new List<IAxis>();
                result.Add(_model.RegularAxes[0]);
                result.Add(_model.RegularAxes[1]);
                return result;
            }
        }

        /// <summary>
        /// Gets filter axis within the PivotTable. If items source is not set, returns null.
        /// </summary>
        public IFilterAxis FilterAxis
        {
            get { return _model != null ? _model.FilterAxis : null; }
        }

        /// <summary>
        /// Gets all dimensions hierarchies within the model. If items source is not set,
        /// returns null.
        /// </summary>
        public IList<IHierarchy> DimensionsHierarchies
        {
            get { return _model != null ? _model.DimensionsHierarchies : null; }
        }

        /// <summary>
        /// Gets measures hierarchy within the model.
        /// </summary>
        public IHierarchy MeasuresHierarchy
        {
            get { return _model != null ? _model.MeasuresHierarchy : null; }
        }

        /// <summary>
        /// Gets underlying data model for this control. If items source is not set,
        /// returns null.
        /// </summary>
        public IDataModel Model
        {
            get { return _model; }
        }

        /// <summary>
        /// Returns collection of all selected cells.
        /// </summary>
        public IEnumerable<Cell> SelectedCells
        {
            get { return Cells.SelectedCells; }
        }

        #endregion

        #region Handling model changes
        private void AssignPartsToModel()
        {
            if (_model != null)
            {
                FilterAxisControl.FilterAxis = _model.FilterAxis;
                IAxis rowsAxis = _model.RegularAxes[0];
                IAxis columnAxis = _model.RegularAxes[1];
                RowsHierarchies.Axis = rowsAxis;
                RowsMembers.Axis = rowsAxis;
                ColumnsHierarchies.Axis = columnAxis;
                ColumnsMembers.Axis = columnAxis;

                ((INotifyCollectionChanged)_model.FilterAxis.Tuples).CollectionChanged += OnCellsChanged;
                ((INotifyCollectionChanged)rowsAxis.Tuples).CollectionChanged += OnCellsChanged;
                ((INotifyCollectionChanged)columnAxis.Tuples).CollectionChanged += OnCellsChanged;
            }
            else
            {
                FilterAxisControl.FilterAxis = null;
                RowsHierarchies.Axis = null;
                RowsMembers.Axis = null;
                ColumnsHierarchies.Axis = null;
                ColumnsMembers.Axis = null;
            }

            DimensionsAndMeasures.Model = _model;
            Cells.Model = _model;
        }

        private void OnCellsChanged(object sender, EventArgs e)
        {
            Cells.OnCellsChanged();
        }

        /// <summary>
        /// Creates data model from given items source.
        /// </summary>
        /// <param name="itemsSource">Items source.</param>
        /// <returns>Returns model for the given data source. If type of the data source
        /// is not supported, returns null.</returns>
        protected virtual IDataModel CreateModelFromItemsSource(object itemsSource)
        {
            if (itemsSource == null)
            {
                return null;
            }
            if (itemsSource is AdomdItemsSource)
            {
                return new AdomdDataModel((AdomdItemsSource)itemsSource);
            }
            if (itemsSource is DataTable)
            {
                return new DataTableDataModel((DataTable)itemsSource);
            }
            return null;
        }
        #endregion

        #region Selecting cells
        /// <summary>
        /// Selects cells.
        /// </summary>
        /// <param name="cells">Cells to select.</param>
        public void SelectCells(IEnumerable<Cell> cells)
        {
            Cells.SelectCells(cells);
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
        /// <param name="disposing">Value that indicates whether this method was called
        /// by the IDisposable.Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_model != null)
                {
                    _model.Dispose();
                }
            }
        }

        #endregion

        public void UpdateCellsLayout()
        {
            Cells.UpdateLayout();
        }
    }
}
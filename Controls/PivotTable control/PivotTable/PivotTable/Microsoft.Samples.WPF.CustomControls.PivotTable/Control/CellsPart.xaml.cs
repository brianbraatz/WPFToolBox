//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: CellsPart control.
//  Creator: t-tomkm
//  Date Created: 08/21/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
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

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Control that displays all cells into the PivotTable.
    /// </summary>
    public partial class CellsPart : ModelDisplayControl
    {
        #region Static members
        /// <summary>
        /// RowTuplesPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty RowTuplesPositionProperty;

        /// <summary>
        /// ColumnsTuplesPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnTuplesPositionProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CellsPart()
        {
            RowTuplesPositionProperty = DependencyProperty.Register("RowTuplesPosition",
                typeof(TuplesPositionRepository), typeof(CellsPart));
            ColumnTuplesPositionProperty = DependencyProperty.Register("ColumnTuplesPosition",
                typeof(TuplesPositionRepository), typeof(CellsPart));
        }
        #endregion

        private CellsSelectionHandler _selectionHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CellsPart()
        {
            InitializeComponent();
            _selectionHandler = new CellsSelectionHandler(this);
        }

        #region Properties
        /// <summary>
        /// Gets or sets value of the RowTuplesPosition dependency property. The property
        /// is used to synchronize tuple row number between rows members part and the 
        /// cells part.
        /// </summary>
        public TuplesPositionRepository RowTuplesPosition
        {
            get { return (TuplesPositionRepository)GetValue(RowTuplesPositionProperty); }
            set { SetValue(RowTuplesPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets value of the ColumnTuplesPosition dependency property. The property
        /// is used to synchronize tuple column number between columns members part and
        /// the cells part.
        /// </summary>
        public TuplesPositionRepository ColumnTuplesPosition
        {
            get { return (TuplesPositionRepository)GetValue(ColumnTuplesPositionProperty); }
            set { SetValue(ColumnTuplesPositionProperty, value); }
        }

        private IAxis RowsAxis
        {
            get { return Model != null ? Model.RegularAxes[0] : null; }
        }

        private IAxis ColumnsAxis
        {
            get { return Model != null ? Model.RegularAxes[1] : null; }
        }
        #endregion

        /// <summary>
        /// Handles model changes.
        /// </summary>
        /// <param name="e">Stores old and new value of the Model property.</param>
        protected override void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            RecreateCells();
        }

        #region Recreating cells

        /// <summary>
        /// Recreates cells.
        /// </summary>
        public void OnCellsChanged()
        {
            RecreateCells();
        }

        private void RecreateCells()
        {
            CellsGrid.RowDefinitions.Clear();
            CellsGrid.ColumnDefinitions.Clear();
            CellsGrid.Children.Clear();
            CreateRowsAndColumnsDefinitions();
            CreateCells();
        }

        private void CreateRowsAndColumnsDefinitions()
        {
            if ((Model == null) || (RowTuplesPosition == null) || (ColumnTuplesPosition == null))
            {
                return;
            }
            for (int i = 0; i < RowsAxis.Tuples.Count; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                CellsGrid.RowDefinitions.Add(rowDefinition);
            }
            for (int i = 0; i < ColumnsAxis.Tuples.Count; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = GridLength.Auto;
                columnDefinition.MaxWidth = double.MaxValue;
                CellsGrid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void CreateCells()
        {
            if (Model == null)
            {
                return;
            }

            for (int rowNumber = 0; rowNumber < RowsAxis.Tuples.Count; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < ColumnsAxis.Tuples.Count; columnNumber++)
                {
                    AddCell(rowNumber, columnNumber);
                }
            }
        }

        private void AddCell(int rowNumber, int columnNumber)
        {
            Tuple rowTuple = RowsAxis.Tuples[rowNumber];
            Tuple columnTuple = ColumnsAxis.Tuples[columnNumber];
            Cell cell = Model.GetCell(new Tuple[] { rowTuple, columnTuple });
            if (cell != null)
            {
                CellDescriptionControl element = new CellDescriptionControl();
                CellsGrid.Children.Add(element);
                element.DataContext = new CellDescription(Model, cell, RowTuplesPosition.GetTuplePosition(rowTuple),
                    ColumnTuplesPosition.GetTuplePosition(columnTuple));
            }
        }
        #endregion

        #region Cells selection
        /// <summary>
        /// Returns collection of selected cells.
        /// </summary>
        public IEnumerable<Cell> SelectedCells
        {
            get
            {
                foreach (CellDescriptionControl cellDescription in CellsGrid.Children)
                {
                    if (cellDescription.IsSelected)
                    {
                        yield return ((CellDescription)cellDescription.DataContext).Cell;
                    }
                }
            }
        }

        /// <summary>
        /// Selects cells. Cells that are already selected, but are not in the
        /// <see cref="cells"/> collection, will be unselected.
        /// </summary>
        /// <param name="cells">Cells that should be selected.</param>
        public void SelectCells(IEnumerable<Cell> cells)
        {
            Dictionary<Cell, object> cellSet = new Dictionary<Cell, object>();
            foreach (Cell cell in cells)
            {
                cellSet.Add(cell, null);
            }
            foreach (CellDescriptionControl cellDescriptionControl in CellsGrid.Children)
            {
                CellDescription cellDescription = cellDescriptionControl.DataContext as CellDescription;
                cellDescriptionControl.IsSelected = cellSet.ContainsKey(cellDescription.Cell);
            }
        }
        #endregion

        /// <summary>
        /// Private class that handles mouse events and selects appropriate cells.
        /// </summary>
        private class CellsSelectionHandler
        {
            private CellsPart _cellsPart;
            private Grid _cellsGrid;
            private bool _selectMultipleItems;
            private CellDescriptionControl _firstSelectedControl;
            private CellDescriptionControl _lastSelectedControl;
            private Dictionary<Cell, object> _previouslySelectedItems;

            public CellsSelectionHandler(CellsPart cellsPart)
            {
                _cellsPart = cellsPart;
                _cellsGrid = _cellsPart.CellsGrid;
                _cellsGrid.MouseDown += OnMouseDown;
                _cellsGrid.MouseUp += OnMouseUp;
                _cellsGrid.MouseMove += OnMouseMove;
                _previouslySelectedItems = new Dictionary<Cell, object>();
            }

            public void OnMouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SetSelectMultipleItems();
                    FillPreviouslySelectedItems();
                    CellDescriptionControl firstSelectedControl = e.Source as CellDescriptionControl;
                    if (firstSelectedControl != null)
                    {
                        _firstSelectedControl = firstSelectedControl;
                        SelectControlsBetween(_firstSelectedControl, _firstSelectedControl);
                    }
                }
            }

            private void FillPreviouslySelectedItems()
            {
                _previouslySelectedItems.Clear();
                foreach (Cell selectedCell in _cellsPart.SelectedCells)
                {
                    _previouslySelectedItems.Add(selectedCell, null);
                }
            }

            private void SetSelectMultipleItems()
            {
                _selectMultipleItems = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                                       Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightAlt);
            }

            public void OnMouseUp(object sender, MouseButtonEventArgs e)
            {
                _firstSelectedControl = null;
            }

            public void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && _firstSelectedControl != null)
                {
                    CellDescriptionControl lastControl = e.Source as CellDescriptionControl;
                    if ((lastControl != null) && (lastControl != _lastSelectedControl))
                    {
                        SelectControlsBetween(_firstSelectedControl, lastControl);
                    }
                    _lastSelectedControl = lastControl;
                }
            }

            private void SelectControlsBetween(CellDescriptionControl firstControl, CellDescriptionControl lastControl)
            {
                int firstControlRow = Grid.GetRow(firstControl);
                int firstControlColumn = Grid.GetColumn(firstControl);
                int lastControlRow = Grid.GetRow(lastControl);
                int lastControlColumn = Grid.GetColumn(lastControl);
                bool firstItemWasPreviouslySelected = _previouslySelectedItems.ContainsKey(
                    ((CellDescription)_firstSelectedControl.DataContext).Cell);

                foreach (CellDescriptionControl control in _cellsGrid.Children)
                {
                    int controlRow = Grid.GetRow(control);
                    int controlColumn = Grid.GetColumn(control);
                    Cell cell = ((CellDescription)control.DataContext).Cell;
                    bool isControlBetweenFirstAndLast = IsNumberBetween(firstControlRow, lastControlRow, controlRow) &&
                                                        IsNumberBetween(firstControlColumn, lastControlColumn, controlColumn);

                    if (_selectMultipleItems)
                    {
                        if (isControlBetweenFirstAndLast)
                        {
                            control.IsSelected = !firstItemWasPreviouslySelected;
                        }
                        else
                        {
                            control.IsSelected = _previouslySelectedItems.ContainsKey(cell);
                        }                        
                    }
                    else
                    {
                        control.IsSelected = isControlBetweenFirstAndLast;
                    }
                }
            }

            private bool IsNumberBetween(int rangeStart, int rangeEnd, int number)
            {
                return (number >= rangeStart && number <= rangeEnd) ||
                       (number >= rangeEnd && number <= rangeStart);
            }
        }
    }
}
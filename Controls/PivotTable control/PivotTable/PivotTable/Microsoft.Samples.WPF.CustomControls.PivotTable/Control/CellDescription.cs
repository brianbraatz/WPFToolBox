//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
// 
//  Description: CellDescription class.
//  Creator: t-tomkm
//  Date Created: 08/21/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Describes a single cell in the Cells part.
    /// </summary>
    internal class CellDescription
    {
        #region Fields
        private Cell _cell;
        private int _rowNumber;
        private int _columnNumber;
        private string _sharedWidthGroup;
        private string _sharedHeightGroup;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The data model.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="rowNumber">Row number into the cells grid.</param>
        /// <param name="columnNumber">Column number into the cells grid.</param>
        public CellDescription(IDataModel model, Cell cell, int rowNumber, int columnNumber)
        {
            _cell = cell;
            _rowNumber = rowNumber;
            _columnNumber = columnNumber;
            Tuple rowTuple = model.RegularAxes[0].Tuples[cell.Coordinates[0]];
            Tuple columnTuple = model.RegularAxes[1].Tuples[cell.Coordinates[1]];
            _sharedHeightGroup = rowTuple[rowTuple.Length - 1].UniqueName;
            _sharedWidthGroup = columnTuple[columnTuple.Length - 1].UniqueName;
        }

        #region Properties
        /// <summary>
        /// The cell.
        /// </summary>
        public Cell Cell
        {
            get { return _cell; }
        }

        /// <summary>
        /// Row number into the cells grid.
        /// </summary>
        public int RowNumber
        {
            get { return _rowNumber; }
        }

        /// <summary>
        /// Column number into the cells grid.
        /// </summary>
        public int ColumnNumber
        {
            get { return _columnNumber; }
        }

        /// <summary>
        /// Name of the shared width group.
        /// </summary>
        public string SharedWidthGroup
        {
            get { return _sharedWidthGroup; }
            set { _sharedWidthGroup = value; }
        }

        /// <summary>
        /// Name of the shared height group.
        /// </summary>
        public string SharedHeightGroup
        {
            get { return _sharedHeightGroup; }
            set { _sharedHeightGroup = value; }
        }
        #endregion
    }
}

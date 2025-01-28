//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: Cell class.
//  Creator: t-tomkm
//  Date Created: 8/21/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represnets a single cell within the PivotTable Data Model.
    /// </summary>
    public class Cell
    {
        private object _value;
        private ReadOnlyArray<int> _coordinates;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value of the cell.</param>
        /// <param name="coordinates">Coordinates (ordinal positions of tuples) of the cell.</param>
        public Cell(object value, int[] coordinates)
        {
            Assert.ArgumentNotNull(coordinates, "coordinates", "Coordinates cannot be null.");
            _value = value;

            _coordinates = new ReadOnlyArray<int>(coordinates);
        }

        /// <summary>
        /// Value of the cell.
        /// </summary>
        public object Value 
        {
            get { return _value; }
        }

        /// <summary>
        /// Coordinates (ordinal positions of tuples) of the cell.
        /// </summary>
        public ReadOnlyArray<int> Coordinates 
        {
            get { return _coordinates; }
        }
    }

    /// <summary>
    /// A read-only array.
    /// </summary>
    /// <typeparam name="T">Type of items in the array.</typeparam>
    public class ReadOnlyArray<T>
    {
        private T[] _values;

        /// <summary>
        /// Constructor. It doesn't create copy of the given array.
        /// </summary>
        /// <param name="values">Original array.</param>
        public ReadOnlyArray(T[] values)
        {
            Assert.ArgumentNotNull(values, "value", "Inner array cannot be null.");
            _values = values;
        }
        
        /// <summary>
        /// Length of the array.
        /// </summary>
        public int Length
        {
            get { return _values.Length; }
        }

        /// <summary>
        /// Gets item at the given position.
        /// </summary>
        /// <param name="index">Position of item to return.</param>
        /// <returns>Item at the given position.</returns>
        public T this[int index]
        {
            get { return _values[index]; }
        }
    }
}

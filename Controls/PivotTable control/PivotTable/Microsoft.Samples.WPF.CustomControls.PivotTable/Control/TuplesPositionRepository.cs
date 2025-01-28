//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: TuplesPositionRepository class.
//  Creator: t-tomkm
//  Date Created: 08/24/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Dictionary that sets or gets ordinal position of each tuple.
    /// It is used by RowsMembers part, ColumnsMembers part and CellsPart
    /// to synchronize cells positions.
    /// </summary>
    public class TuplesPositionRepository
    {
        private Dictionary<Tuple, int> _positions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TuplesPositionRepository()
        {
            _positions = new Dictionary<Tuple, int>();
        }

        /// <summary>
        /// Removes all entries from the repository.
        /// </summary>
        public void Clear()
        {
            _positions.Clear();
        }

        /// <summary>
        /// Sets ordinal position for the given tuple.
        /// </summary>
        /// <param name="tuple">A tuple.</param>
        /// <param name="position">Ordinal position of the tuple.</param>
        public void SetTuplePosition(Tuple tuple, int position)
        {
            Assert.ArgumentNotNull(tuple, "tuple", "Cannot set ordinal position for a null tuple.");
            Assert.ArgumentValid(position >= 0, "position", "Position of a tuple must be not-negative.");
            _positions[tuple] = position;
        }

        /// <summary>
        /// Gets position of the given tuple. If position of the given tuple was not set,
        /// returns -1.
        /// </summary>
        /// <param name="tuple">A tuple.</param>
        /// <returns>If position for the given tuple was set, returns it; otherwise, returns -1.
        /// </returns>
        public int GetTuplePosition(Tuple tuple)
        {
            Assert.ArgumentNotNull(tuple, "tuple", "Cannot return position for a null tuple.");
            return _positions.ContainsKey(tuple) ? _positions[tuple] : -1;
        }
    }
}

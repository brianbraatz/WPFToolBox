//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IDataModel interface.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Interface for multidimensional data model for PivotTable control.
    /// </summary>
    public interface IDataModel : IDisposable
    {
        /// <summary>
        /// Gets all regular (not filter) axes within the model.
        /// </summary>
        ReadOnlyObservableCollection<IAxis> RegularAxes { get; }

        /// <summary>
        /// Gets filter axis within the model.
        /// </summary>
        IFilterAxis FilterAxis { get; }

        /// <summary>
        /// Gets all dimensions hierarchies within the model.
        /// </summary>
        ReadOnlyObservableCollection<IHierarchy> DimensionsHierarchies { get; }

        /// <summary>
        /// Gets measures hierarchy within the model.
        /// </summary>
        IHierarchy MeasuresHierarchy { get; }

        /// <summary>
        /// Gets cells at given position.
        /// </summary>
        /// <param name="cellPosition">Collection of tuples, that uniquely identify the cell.
        /// First tuple must be from first axis, second must be from second axis, and so on.
        /// </param>
        /// <returns></returns>
        Cell GetCell(IEnumerable<Tuple> cellPosition);

        /// <summary>
        /// Event raised when cells were changed.
        /// </summary>
        event EventHandler CellsChanged;
    }

    /// <summary>
    /// State of a member in a tuple.
    /// </summary>
    public enum MemberState
    {
        /// <summary>
        /// The member in the tuple is visible and collapsed.
        /// </summary>
        Collapsed,

        /// <summary>
        /// The member in the tuple is visible and expanded.
        /// </summary>
        Expanded,

        /// <summary>
        /// One of the member's ancestors within the tuple is collapsed.
        /// </summary>
        Hidden
    }
}

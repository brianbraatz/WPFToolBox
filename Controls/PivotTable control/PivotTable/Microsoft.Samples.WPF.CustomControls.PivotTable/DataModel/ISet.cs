//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: ISet interface.
//  Creator: t-tomkm
//  Date Created: 09/07/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Interface for set data structure.
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    public interface ISet<T> : ICollection<T>
    {
        /// <summary>
        /// Adds all items to the set.
        /// </summary>
        /// <param name="items">Items to add.</param>
        void Add(IEnumerable<T> items);

        /// <summary>
        /// Removes from the current set all items that exist in the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains elements to remove.</param>
        void Subtract(ISet<T> otherSet);

        /// <summary>
        /// Adds to the current set all items from the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains elements to add.</param>
        void Union(ISet<T> otherSet);

        /// <summary>
        /// Removes from the current set all items that are not in the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains element that shouldn't be removed.</param>
        void Intersect(ISet<T> otherSet);
    }
}

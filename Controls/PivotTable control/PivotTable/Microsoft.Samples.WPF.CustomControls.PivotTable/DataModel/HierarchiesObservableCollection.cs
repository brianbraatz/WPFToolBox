//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: HierarchiesObservableCollection class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Adds additional validation to ObservableCollection&lt;IHierarchy&gt;.
    /// </summary>
    internal abstract class HierarchiesObservableCollection : ObservableCollection<IHierarchy>
    {
        /// <summary>
        /// Constructor. Creates empty hierarchies collection.
        /// </summary>
        protected HierarchiesObservableCollection()
        { }

        /// <summary>
        /// Inserts item into the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="item"/> 
        /// should be inserted.</param>
        /// <param name="item">The hierarchy to insert into the collection.</param>
        /// <exception cref="ArgumentException">Item is not valid.</exception>
        /// <exception cref="ArgumentNullException">Item is null.</exception>
        /// <exception cref="InvalidOperationException">Item already belongs to an axis.</exception>
        protected override void InsertItem(int index, IHierarchy item)
        {
            ValidateHierarchy(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Sets item at given index.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="item"/>
        /// should be set.</param>
        /// <param name="item">The new hierarchy.</param>
        /// <exception cref="ArgumentException">Item is not valid.</exception>
        /// <exception cref="ArgumentNullException">Item is null.</exception>
        /// <exception cref="InvalidOperationException">Item already belongs to an axis.</exception>
        protected override void SetItem(int index, IHierarchy item)
        {
            ValidateHierarchy(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Validates hierarchy.
        /// </summary>
        /// <param name="item">Hierarchy to validate.</param>
        protected abstract void ValidateHierarchy(IHierarchy item);
    }
}

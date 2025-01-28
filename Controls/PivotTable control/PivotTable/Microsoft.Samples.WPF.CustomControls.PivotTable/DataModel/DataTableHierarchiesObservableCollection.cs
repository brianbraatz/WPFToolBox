//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableHierarchiesObservableCollection.
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
    /// <remarks>
    /// This collection accepts only items of DataTableHierarchy or DataTableMeasuresHierarchy 
    /// class that don't belong to any axis.
    /// </remarks>
    internal class DataTableHierarchiesObservableCollection : HierarchiesObservableCollection
    {
        /// <summary>
        /// Validates hierarchy.
        /// </summary>
        /// <param name="item">Hierarchy to validate.</param>
        protected override void ValidateHierarchy(IHierarchy item)
        {
            Assert.ArgumentNotNull(item, "item", "Cannot insert null hierarchy to an axis.");
            Assert.ArgumentValid((item is DataTableHierarchy) || (item is DataTableMeasuresHierarchy), "item",
                "DataTable Axis supports only DataTableHierarchy or DataTableMeasuresHierarchy.");
            Assert.ArgumentValid(item.Axis == null, "item", "Given hierarchy already belongs to an axis.");
        }
    }

    
}

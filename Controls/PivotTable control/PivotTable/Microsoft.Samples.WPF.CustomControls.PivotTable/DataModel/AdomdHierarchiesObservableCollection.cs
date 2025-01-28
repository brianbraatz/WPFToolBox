//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdHierarchiesObservableCollection
//  Creator: t-tomkm
//  Date Created: 7/27/2006
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
    /// This collection accepts only items of AdomdHierarchy or AdomdMeasuresHierarchy class 
    /// that don't belong to any axis.
    /// </remarks>
    internal class AdomdHierarchiesObservableCollection : HierarchiesObservableCollection
    {
        /// <summary>
        /// Validates hierarchy.
        /// </summary>
        /// <param name="item">Hierarchy to validate.</param>
        protected override void ValidateHierarchy(IHierarchy item)
        {
            Assert.ArgumentNotNull(item, "item", "Cannot insert null hierarchy to an axis.");
            Assert.ArgumentValid((item is AdomdHierarchy) || (item is AdomdMeasuresHierarchy), "item",
                "Adomd.NET data model supports only AdomdHierarchy or AdomdMeasuresHierarchy hierarchies.");
            Assert.ArgumentValid(item.Axis == null, "item", "Given hierarchy already belongs to an axis.");
        }
    }
}

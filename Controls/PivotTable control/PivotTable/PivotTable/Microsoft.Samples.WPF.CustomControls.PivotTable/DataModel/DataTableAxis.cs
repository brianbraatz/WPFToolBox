//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableAxis class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IAxis implementation for DataTable model.
    /// </summary>
    public class DataTableAxis : Axis
    {
        private DataTableHierarchiesObservableCollection _hierarchies;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataTableAxis()
        {
            _hierarchies = new DataTableHierarchiesObservableCollection();
            ((INotifyCollectionChanged)_hierarchies).CollectionChanged += OnHierarchiesChanged;
        }

        /// <summary>
        /// Gets all hierarchies within the axis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be used to add new hierarchies to the axis.
        /// </para>
        /// <para>
        /// A hierarchy can belong only to one axis. When user tries to add
        /// hierarchy that already belongs to different axis, the Add method
        /// should throw an InvalidOperationException.
        /// </para>
        /// </remarks>
        public override ObservableCollection<IHierarchy> Hierarchies
        {
            get { return _hierarchies; }
        }
    }
}

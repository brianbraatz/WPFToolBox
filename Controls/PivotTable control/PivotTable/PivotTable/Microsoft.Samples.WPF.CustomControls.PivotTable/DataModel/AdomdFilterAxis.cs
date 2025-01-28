//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdFilterAxis class.
//  Creator: t-tomkm
//  Date Created: 7/26/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IFilterAxis implementation for Adomd.NET data model.
    /// </summary>
    internal class AdomdFilterAxis : FilterAxis
    {
        private AdomdHierarchiesObservableCollection _hierarchies;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdomdFilterAxis()
        {
            _hierarchies = new AdomdHierarchiesObservableCollection();
            _hierarchies.CollectionChanged += OnHierarchiesChanged;
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

        /// <summary>
        /// Creates a "WHERE" expression in a MDX query.
        /// </summary>
        /// <param name="result">String builder where the result should be written to.</param>
        public void GenerateMdxWhereExpression(StringBuilder result)
        {
            if (_hierarchies.Count > 0)
            {
                result.AppendLine("WHERE");
                MdxUtils.GenerateTuplesSetExpression(result, Tuples);
            }
        }
    }
}

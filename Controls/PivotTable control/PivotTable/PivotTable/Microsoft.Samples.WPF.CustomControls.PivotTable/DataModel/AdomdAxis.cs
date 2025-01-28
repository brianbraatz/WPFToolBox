//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdAxis class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Adomd = Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// The IAxis implementation for Adomd.NET model.
    /// </summary>
    internal class AdomdAxis : Axis
    {
        private AdomdHierarchiesObservableCollection _hierarchies;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdomdAxis()
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
        /// Generates MDX expression for the axis.
        /// </summary>
        /// <param name="result">String builder where the result should be written to.</param>
        /// <param name="axisNumber">Ordinal number of this axis.</param>
        public void GenerateMdxAxisExpression(StringBuilder result, int axisNumber)
        {
            MdxUtils.GenerateTuplesSetExpression(result, Tuples);
            result.AppendFormat(" ON AXIS({0})", axisNumber);            
        }
    }
}

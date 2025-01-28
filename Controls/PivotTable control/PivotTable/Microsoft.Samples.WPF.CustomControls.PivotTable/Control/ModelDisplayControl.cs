//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: 
//  Creator: t-tomkm
//  Date Created: 08/17/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Base class for all controls that display data from the data model.
    /// </summary>
    public abstract class ModelDisplayControl : UserControl
    {
        /// <summary>
        /// The Model dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ModelDisplayControl()
        {
            ModelProperty = DependencyProperty.Register("Model", typeof(IDataModel),
                typeof(ModelDisplayControl), new PropertyMetadata(OnModelChanged));
        }

        private static void OnModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ModelDisplayControl)obj).OnModelChanged(e);
        }

        /// <summary>
        /// Method invoked when Model property changes.
        /// </summary>
        /// <param name="e">Argument that stores previous and current value of the 
        /// Model property.</param>
        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        { }

        /// <summary>
        /// Gets or sets model associated with this part.
        /// </summary>
        public IDataModel Model
        {
            get { return (IDataModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
    }
}

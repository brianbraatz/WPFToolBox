//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AxisDisplayControl class.
//  Creator: t-tomkm
//  Date Created: 08/11/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Abstract, base class for all PivotTable parts that are bound to an axis. It defines 
    /// Axis dependency property.
    /// </summary>
    public abstract class AxisDisplayControl : UserControl
    {
        #region Static Members
        /// <summary>
        /// Axis dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static AxisDisplayControl()
        {
            AxisProperty = DependencyProperty.Register("Axis", typeof(IAxis), 
                typeof(AxisDisplayControl), new PropertyMetadata(OnAxisChanged));            
        }

        private static void OnAxisChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AxisDisplayControl target = sender as AxisDisplayControl;
            if (target != null)
            {
                target.OnAxisChanged(e);
            }
        }
        #endregion

        private DragHelper _dragHelper;
        private DropHelper _dropHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxisDisplayControl()
        {
            AllowDrop = true;
            _dragHelper = new DragHelper(this);
        }

        /// <summary>
        /// Method invoked when Axis property changes.
        /// </summary>
        /// <param name="e">Additional information about the change.</param>
        protected virtual void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_dropHelper != null)
            {
                _dragHelper.Dispose();
            }
            
            if (e.NewValue != null)
            {
                _dropHelper = DropHelper.CreateDropHelper(this, (IFilterAxis)e.NewValue, true);
            }
            else
            {
                _dropHelper = null;
            }
        }

        /// <summary>
        /// Gets or sets value of the Axis dependency property.
        /// </summary>
        public IAxis Axis
        {
            get { return (IAxis)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }
    }
}

//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: The FilterAxisPart control.
//  Creator: t-tomkm
//  Date Created: 08/10/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// The Filter Axis Part of the PivotTable.
    /// </summary>
    public partial class FilterAxisPart : UserControl
    {
        #region Static members
        /// <summary>
        /// FilterAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterAxisProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static FilterAxisPart()
        {
            FilterAxisProperty = DependencyProperty.Register("FilterAxis", typeof(IFilterAxis), 
                typeof(FilterAxisPart), new PropertyMetadata(OnFilterAxisChanged));
        }

        private static void OnFilterAxisChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FilterAxisPart)obj).OnFilterAxisChanged(e);
        }
        #endregion

        private DragHelper _dragHelper;
        private DropHelper _dropHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FilterAxisPart()
        {
            InitializeComponent();
            _dragHelper = new DragHelper(this);
        }

        /// <summary>
        /// Gets or sets value of the FilterAxis dependency property.
        /// </summary>
        public IFilterAxis FilterAxis
        {
            get { return (IFilterAxis)GetValue(FilterAxisProperty); }
            set { SetValue(FilterAxisProperty, value); }
        }

        /// <summary>
        /// Method invoked when the FilterAxis property changes.
        /// </summary>
        /// <param name="e">Additional information about the change.</param>
        protected virtual void OnFilterAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_dropHelper != null)
            {
                _dropHelper.Dispose();
            }
            if (e.NewValue == null)
            {
                HierarchiesGrid.DataContext = null;
                _dropHelper = null;
            }
            else
            {
                HierarchiesGrid.DataContext = (IFilterAxis)e.NewValue;
                _dropHelper = DropHelper.CreateDropHelper(this, (IFilterAxis)e.NewValue, false);
            }
        }
    }
}
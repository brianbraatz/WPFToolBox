//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: CellDescriptionControl class.
//  Creator: t-tomkm
//  Date Created: 08/21/2006
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
    /// Control that displays a single cell into the PivotTable.
    /// </summary>
    public partial class CellDescriptionControl : UserControl
    {
        /// <summary>
        /// IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CellDescriptionControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Constructor.
        /// </summary>
        public CellDescriptionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Indicates whether this cell is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
    }
}
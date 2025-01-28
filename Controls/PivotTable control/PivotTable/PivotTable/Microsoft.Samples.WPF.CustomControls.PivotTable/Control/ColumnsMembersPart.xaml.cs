//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: ColumnsMembersPart control.
//  Creator: t-tomkm
//  Date Created: 08/17/2006
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
    /// Control that displays members from the Columns Axis.
    /// </summary>
    public partial class ColumnsMembersPart : MembersDisplayControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ColumnsMembersPart()
        {
            InitializeComponent();
        }
    }
}
//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: The RowsHierarchiesPart control.
//  Creator: t-tomkm
//  Date Created: 08/10/2006.
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
    /// The Rows Hierarchies Part of the PivotTable.
    /// </summary>
    public partial class RowsHierarchiesPart : AxisDisplayControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RowsHierarchiesPart()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Method invoked when Axis property changes.
        /// </summary>
        /// <param name="e">Additional information about the change.</param>
        protected override void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            HierarchiesItemsControl.DataContext = Axis;
            base.OnAxisChanged(e);
        }

        private void OnListBoxItemSelected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null)
            {
                item.IsSelected = false;
            }
        }
    }

    /// <summary>
    /// Selects template for level. First level is displayed in a different way, because it
    /// allows to filter members.
    /// </summary>
    internal class RowsHierarchiesLevelTemplateSelector : DataTemplateSelector
    {
        /// Returns a DataTemplate for a level.
        /// </summary>
        /// <param name="item">The level for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a data template for given level.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ILevel level = item as ILevel;
            string templateKey = null;
            if (level != null)
            {
                templateKey = level.LevelNumber == 0 ? "FirstLevelTemplate" : "DefaultLevelTemplate";
            }
            return templateKey != null ? (DataTemplate)((FrameworkElement)container).FindResource(templateKey) : null;
        }
    }
}
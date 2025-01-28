//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: The DimensionsAndMeasuresPart control.
//  Creator: t-tomkm
//  Date Created: 08/11/2006
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
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// The Dimensions And Measures Part of the PivotTable.
    /// </summary>
    public partial class DimensionsAndMeasuresPart : ModelDisplayControl
    {
        private DragHelper _dragHelper;
        private DropHelper _dropHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DimensionsAndMeasuresPart()
        {
            InitializeComponent();
            _dragHelper = new DragHelper(this);
            _dropHelper = DropHelper.CreateDropHelper(this, null, true);
        }

        #region Handling model changes.
        protected override void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Model == null)
            {
                HierarchiesAndMeasures.Clear();
            }
            else
            {
                HierarchiesAndMeasures.ReplaceAllItems(GetHierarchiesAndMeasuresFromModel());
            }
        }

        private IEnumerable<object> GetHierarchiesAndMeasuresFromModel()
        {
            foreach (IHierarchy hierarchy in Model.DimensionsHierarchies)
            {
                yield return hierarchy;
            }
            foreach (IMember member in Model.MeasuresHierarchy.Levels[0].Members)
            {
                yield return member;
            }
        }

        private HierarchiesAndMeasuresCollection HierarchiesAndMeasures
        {
            get { return (HierarchiesAndMeasuresCollection)FindResource("HierarchiesAndMeasuresCollection"); }
        }
        #endregion

        #region Showing or hiding this part
        private void ShowPartButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPartPanel.Visibility = Visibility.Collapsed;
            SelectionPanel.Visibility = Visibility.Visible;
        }

        private void HidePartButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPartPanel.Visibility = Visibility.Visible;
            SelectionPanel.Visibility = Visibility.Collapsed;
        }
        #endregion
    }

    /// <summary>
    /// Converter used for grouping measures and hierarchies.
    /// </summary>
    internal class GroupHierarchiesAndMembersConverter : IValueConverter
    {
        /// <summary>
        /// Returns display folder for given hierarchy or measure.
        /// </summary>
        /// <param name="value">The hierarchy or measure.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Name of display folder for given measure or hierarchy.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IMeasuresMember)
            {
                string displayFolder = ((IMeasuresMember)value).DisplayFolder;
                return (displayFolder != "" ? displayFolder : "Other measures");
            }
            else if (value is IHierarchy)
            {
                string displayFolder = ((IHierarchy)value).DisplayFolder;
                return (displayFolder != "" ? displayFolder : "Other dimensions");
            }
            return null;
        }

        /// <summary>
        /// This method is not implemented and should not be used.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    /// <summary>
    /// Selects template for an hierarchy, measure, hierarchies group or measures group.
    /// </summary>
    internal class HierarchiesAndMeasuresTemplateSelector : DataTemplateSelector
    {   
        /// <summary>
        /// Returns a DataTemplate for given item.
        /// </summary>
        /// <param name="item">Hierarchy, measure, hierarchies group or measures group for which
        /// to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns:
        /// <list>
        /// <item>MeasuresGroupTemplate for measures group.</item>
        /// <item>HierarchiesGroupTemplate for hierarchies group.</item>
        /// <item>EmptyGroupTemplate for group that is empty.</item>
        /// <item>MeasuresMemberTemplate for measure.</item>
        /// <item>HierarchyTemplate for hierarchy.</item>
        /// </list>
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string templateKey = null;
            if (item is CollectionViewGroup)
            {
                CollectionViewGroup group = (CollectionViewGroup)item;
                if (group.ItemCount > 0)
                {
                    object firstInnerItem = group.Items[0];
                    if (firstInnerItem is IMeasuresMember)
                    {
                        templateKey = "MeasuresGroupTemplate";
                    }
                    else if (firstInnerItem is IHierarchy)
                    {
                        templateKey = "HierarchiesGroupTemplate";
                    }
                }
                else
                {
                    templateKey = "EmptyGroupTemplate";
                }
            }
            else if (item is IMeasuresMember)
            {
                templateKey = "MeasuresMemberTemplate";
            }
            else if (item is IHierarchy)
            {
                templateKey = "HierarchyTemplate";
            }
            if (templateKey != null)
            {
                return (DataTemplate)((FrameworkElement)container).FindResource(templateKey);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Observable collection that can contain both hierarchies and measures.
    /// </summary>
    internal class HierarchiesAndMeasuresCollection : ExtendedObservableCollection<object>
    { }
}
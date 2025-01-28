//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: MembersDisplayControl class.
//  Creator: t-tomkm
//  Date Created: 08/11/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Base class for controls that display members from an axis.
    /// </summary>
    public abstract class MembersDisplayControl : AxisDisplayControl
    {
        #region Static members
        /// <summary>
        /// TuplesPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty TuplesPositionProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MembersDisplayControl()
        {
            TuplesPositionProperty = DependencyProperty.Register("TuplesPosition", 
                typeof(TuplesPositionRepository), typeof(MembersDisplayControl));
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MembersDisplayControl()
        { }

        #region Properties
        /// <summary>
        /// Gets or sets value of the TuplesPosition dependency property.
        /// </summary>
        /// <remarks>
        /// This property allows to set ordinal position for each displayed tuple. It is
        /// used to synchronize row and column numbers among RowsMembers, ColumnsMembers and
        /// Cells parts.
        /// </remarks>
        public TuplesPositionRepository TuplesPosition
        {
            get { return (TuplesPositionRepository)GetValue(TuplesPositionProperty); }
            set { SetValue(TuplesPositionProperty, value); }
        }
        #endregion

        #region Handling Axis changes.
        /// <summary>
        /// Handles the Axis property changes.
        /// </summary>
        /// <param name="e">Stores old and new value of Axis property.</param>
        protected override void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnAxisChanged(e);
            if (e.OldValue != null)
            {
                IAxis oldAxis = (IAxis)e.OldValue;
                INotifyCollectionChanged tuples = (INotifyCollectionChanged)oldAxis.Tuples;
                tuples.CollectionChanged -= OnTuplesChanged;
            }
            if (e.NewValue != null)
            {
                IAxis newAxis = (IAxis)e.NewValue;
                INotifyCollectionChanged tuples = (INotifyCollectionChanged)newAxis.Tuples;
                tuples.CollectionChanged += OnTuplesChanged;
            }
            BindTuplesToTreeView();
        }

        private void OnTuplesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BindTuplesToTreeView();
        }

        private void BindTuplesToTreeView()
        {
            if (Axis != null)
            {
                ListCollectionView tuplesView = new ListCollectionView(Axis.Tuples);
                for (int i = 0; i < CountLevelsForCurrentAxis(); i++)
                {
                    tuplesView.GroupDescriptions.Add(new TuplesGroupDescription());
                }
                this.DataContext = tuplesView;
                UpdateTuplesPositions(tuplesView);
            }
            else
            {
                this.DataContext = null;
            }
        }

        private int CountLevelsForCurrentAxis()
        {
            int result = 0;
            foreach (IHierarchy hierarchy in Axis.Hierarchies)
            {
                result += hierarchy.Levels.Count;
            }
            return result;
        }

        private void UpdateTuplesPositions(ListCollectionView tuplesView)
        {
            if (TuplesPosition == null)
            {
                return;
            }

            TuplesPosition.Clear();
            int tupleOrdinalPosition = 0;
            
            foreach (Tuple tuple in GetAllTuples(tuplesView))
            {
                TuplesPosition.SetTuplePosition(tuple, tupleOrdinalPosition++);
            }
        }

        private IEnumerable<Tuple> GetAllTuples(ListCollectionView tuplesView)
        {
            if (tuplesView.Groups != null)
            {
                foreach (CollectionViewGroup group in tuplesView.Groups)
                {
                    foreach (Tuple tuple in GetAllTuples(group))
                    {
                        yield return tuple;
                    }
                }
            }
        }

        private IEnumerable<Tuple> GetAllTuples(CollectionViewGroup group)
        {
            if (group.ItemCount > 0)
            {
                object firstItem = group.Items[0];
                if (firstItem is CollectionViewGroup)
                {
                    foreach (CollectionViewGroup innerGroup in group.Items)
                    {
                        foreach (Tuple tuple in GetAllTuples(innerGroup))
                        {
                            yield return tuple;
                        }
                    }
                }
                else if (firstItem is Tuple)
                {
                    foreach (Tuple tuple in group.Items)
                    {
                        yield return tuple;
                    }
                }
            }
        }
        #endregion

        #region ToggleButton events handling
        /// <summary>
        /// Initializes the toggle button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains no event data.</param>
        protected void ToggleButtonInitialized(object sender, EventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            SetToggleButtonIsChecked(button);

            button.Checked += ItemChecked;
            button.Unchecked += ItemUnchecked;
        }

        private void SetToggleButtonIsChecked(ToggleButton button)
        {
            CollectionViewGroup group = button.DataContext as CollectionViewGroup;            
            
            if (group == null)
            {
                button.IsChecked = false;
                return;
            }

            if (group.Items.Count == 1)
            {
                if (!(group.Items[0] is CollectionViewGroup))
                {
                    button.IsChecked = false;
                    return;
                }

                TuplesGroup childGroup = (TuplesGroup)((CollectionViewGroup)group.Items[0]).Name;
                button.IsChecked = ((IAxis)Axis).GetMemberState(childGroup.Member) == MemberState.Expanded;
            }
            else
            {
                button.IsChecked = true;
            }
        }

        /// <summary>
        /// Handles ToggleButton.Checked event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains no event data.</param>
        protected void ItemChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)e.OriginalSource;
            CollectionViewGroup group = button.DataContext as CollectionViewGroup;
            if (group == null)
            {
                return;
            }
            Axis.ExpandMember(((TuplesGroup)group.Name).Member);
        }

        /// <summary>
        /// Handles ToggleButton.Unchecked event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains no event data.</param>
        protected void ItemUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)e.OriginalSource;
            CollectionViewGroup group = button.DataContext as CollectionViewGroup;
            if (group == null)
            {
                return;
            }
            Axis.CollapseMember(((TuplesGroup)group.Name).Member);
        }
        #endregion

    }

    /// <summary>
    /// Converter that finds shared size name for given CollectionViewGroup.
    /// </summary>
    internal class CollectionViewGroupToSharedSizeNameConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Returns a shared size group name for the given CollectionViewGroup.
        /// </summary>
        /// <remarks>
        /// If the group is at the bottom level (its only child is a tuple), returns
        /// unique name of the member from the group's Name (that is of type TuplesGroup).
        /// Otherwise, returns null.
        /// </remarks>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Shared size group name for the given CollectionViewGroup.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CollectionViewGroup group = value as CollectionViewGroup;
            if ((group.ItemCount == 1) && (group.Items[0] is Tuple))
            {
                return ((TuplesGroup)group.Name).Member.UniqueName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This operation is not implemented and should not be used.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion
    }
}

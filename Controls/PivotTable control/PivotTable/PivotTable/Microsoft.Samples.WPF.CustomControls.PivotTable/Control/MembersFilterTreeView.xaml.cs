//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: MembersFilterTreeView, MembersTreeNode, MembersTree classes and
//               IMembersTreeElement interface.
//  Creator: t-tomkm
//  Date Created: 08/09/2006
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
using System.ComponentModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Class that displays members from a given level in a tree and allows to
    /// filter them.
    /// </summary>
    public partial class MembersFilterTreeView : UserControl
    {
        #region Static members
        /// <summary>
        /// Level dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty;

        /// <summary>
        /// Hierarchy dependency property.
        /// </summary>
        public static readonly DependencyProperty HierarchyProperty;

        /// <summary>
        /// SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty;

        static MembersFilterTreeView()
        {
            LevelProperty = DependencyProperty.Register("Level", typeof(ILevel),
                typeof(MembersFilterTreeView), new PropertyMetadata(OnLevelChanged));

            HierarchyProperty = DependencyProperty.Register("Hierarchy", typeof(IHierarchy),
                typeof(MembersFilterTreeView), new PropertyMetadata(OnHierarchyChanged));

            SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(MembersTreeSelectionMode),
                typeof(MembersFilterTreeView), new PropertyMetadata(MembersTreeSelectionMode.ChildrenControlParent, OnSelectionModeChanged));
        }

        private static void OnLevelChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            MembersFilterTreeView filterTreeView = target as MembersFilterTreeView;
            if (filterTreeView != null)
            {
                filterTreeView.OnLevelChanged(e);
            }
        }

        private static void OnHierarchyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            MembersFilterTreeView filterTreeView = target as MembersFilterTreeView;
            if (filterTreeView != null)
            {
                filterTreeView.OnHierarchyChanged(e);
            }
        }

        private static void OnSelectionModeChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            MembersFilterTreeView filterTreeView = target as MembersFilterTreeView;
            if (filterTreeView != null)
            {
                filterTreeView.OnSelectionModeChanged(e);
            }
        }
        #endregion

        private MembersTree _membersTree;

        public MembersFilterTreeView()
        {
            InitializeComponent();
        }

        #region Handling dependency properties changes

        private void OnLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Level == null)
            {
                Hierarchy = null;
                _membersTree = null;
                this.DataContext = null;
                MembersTreeView.ItemsSource = null;
                CaptionTextBlock.Text = "";
            }
            else
            {
                Hierarchy = Level.Hierarchy;
                if (Hierarchy.Axis != null)
                {
                    _membersTree = new MembersTree(Level, SelectionMode);
                    this.DataContext = _membersTree;
                    MembersTreeView.ItemsSource = _membersTree.Children;
                }
                else
                {
                    _membersTree = null;
                    this.DataContext = null;
                    MembersTreeView.ItemsSource = null;
                }
                CaptionTextBlock.Text = Level.Caption;
            }
        }

        private void OnHierarchyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Hierarchy == null)
            {
                Level = null;
                CaptionTextBlock.Text = null;
            }
            else
            {
                Level = Hierarchy.Levels[0];
                CaptionTextBlock.Text = Hierarchy.Caption;
            }
        }

        private void OnSelectionModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_membersTree != null)
            {
                _membersTree.SelectionMode = SelectionMode;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets level associated with this control.
        /// </summary>
        public ILevel Level
        {
            get { return (ILevel)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets hierarchy associated with this control.
        /// </summary>
        public IHierarchy Hierarchy
        {
            get { return (IHierarchy)GetValue(HierarchyProperty); }
            set { SetValue(HierarchyProperty, value); }
        }

        /// <summary>
        /// Gets or sets selection mode for the tree view.
        /// </summary>
        public MembersTreeSelectionMode SelectionMode
        {
            get { return (MembersTreeSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        #endregion

        #region Accepting or rejecting changes

        private void AcceptChanges()
        {
            if (_membersTree != null)
            {
                _membersTree.AcceptChanges();
            }
        }

        private void RejectChanges()
        {
            if (_membersTree != null)
            {
                _membersTree.RejectChanges();
            }
        }

        private void DropDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectionPopup.IsOpen)
            {
                RejectChanges();
            }
            SelectionPopup.IsOpen = !SelectionPopup.IsOpen;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            AcceptChanges();
            SelectionPopup.IsOpen = false;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            RejectChanges();
            SelectionPopup.IsOpen = false;
        }

        private void ControlLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardFocusWithin && !SelectionPopup.IsKeyboardFocusWithin)
            {
                RejectChanges();
                SelectionPopup.IsOpen = false;
            }
        }

        #endregion

        #region Disabling dragging

        private void OnPopupMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }

    /// <summary>
    /// Base class for the members tree and all nodes.
    /// </summary>
    abstract class MembersTreeElement : INotifyPropertyChanged
    {
        #region Fields
        private IList<MembersTreeNode> _children;
        private int _selectedChildrenCount;
        private MembersTreeSelectionMode _selectionMode;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        protected MembersTreeElement(MembersTreeSelectionMode selectionMode)
        {
            _selectedChildrenCount = 0;
        }

        #region Properties
        /// <summary>
        /// Gets child nodes of this element.
        /// </summary>
        public IList<MembersTreeNode> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = InitializeChildren();
                    UpdateChildrenSelection();
                    UpdateSelectedChildrenCount();
                }
                return _children;
            }
        }

        /// <summary>
        /// Gets number of selected children.
        /// </summary>
        public int SelectedChildrenCount
        {
            get { return _selectedChildrenCount; }
        }

        /// <summary>
        /// Gets value that indicates whether children have been already initialized.
        /// </summary>
        protected bool AreChildrenInitialized
        {
            get { return _children != null; }
        }

        /// <summary>
        /// Gets or sets selection mode for the tree view.
        /// </summary>
        public MembersTreeSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;
                if (_children != null)
                {
                    foreach (MembersTreeNode child in _children)
                    {
                        child.SelectionMode = value;
                    }
                }
            }
        }
        #endregion

        #region Initializing and updating children selection
        /// <summary>
        /// Updates selection for children. This method is invoked after children are
        /// initialized.
        /// </summary>
        protected virtual void UpdateChildrenSelection()
        { }

        /// <summary>
        /// Returns all children of this element.
        /// </summary>
        /// <returns>List of all children of this element.</returns>
        protected abstract IList<MembersTreeNode> InitializeChildren();

        private void UpdateSelectedChildrenCount()
        {
            _selectedChildrenCount = 0;
            foreach (MembersTreeNode childNode in Children)
            {
                if (childNode.IsSelected)
                {
                    _selectedChildrenCount++;
                }
            }
        }
        #endregion

        #region Accepting or rejecting changes
        /// <summary>
        /// Accepts changes made by user.
        /// </summary>
        public void AcceptChanges(IList<IMember> newVisibleMembers, IList<IMember> newNotVisibleMembers)
        {
            bool acceptForNotInitializedChildren = AcceptChangesForThisElement(newVisibleMembers,
                newNotVisibleMembers);
            AcceptChangesForChildren(acceptForNotInitializedChildren, newVisibleMembers,
                newNotVisibleMembers);
            UpdateSelectedChildrenCount();
        }

        /// <summary>
        /// Accepts changes for this element and checks whether all children, even
        /// those that are not initialized, should accept changes.
        /// </summary>
        /// <returns>Returns true, if all children should accepts changes. Returns
        /// false if only initialized children should accepts changes.</returns>
        protected virtual bool AcceptChangesForThisElement(IList<IMember> newVisibleMembers,
            IList<IMember> newNotVisibleMembers)
        {
            return false;
        }

        private void AcceptChangesForChildren(bool acceptForNotInitializedChildren,
            IList<IMember> newVisibleMembers, IList<IMember> newNotVisibleMembers)
        {
            if (_children != null || acceptForNotInitializedChildren)
            {
                foreach (MembersTreeNode childNode in Children)
                {
                    childNode.AcceptChanges(newVisibleMembers, newNotVisibleMembers);
                }
            }
        }

        /// <summary>
        /// Rejects changes made by user.
        /// </summary>
        public void RejectChanges()
        {
            RejectChangesForThisElement();
            RejectChangesForChildren();
            UpdateSelectedChildrenCount();
        }

        /// <summary>
        /// Rejects changes for this element (not for its children).
        /// </summary>
        protected virtual void RejectChangesForThisElement()
        { }

        private void RejectChangesForChildren()
        {
            if (_children != null)
            {
                foreach (MembersTreeNode childNode in _children)
                {
                    childNode.RejectChanges();
                }
            }
        }
        #endregion

        #region Updating selected children count.
        /// <summary>
        /// Informs the element that selection of one of its children has been changed.
        /// </summary>
        /// <param name="isSelected">New value of the IsSelected property.</param>
        public void OnChildSelectionChanged(bool isSelected)
        {
            if (isSelected)
            {
                _selectedChildrenCount++;
            }
            else
            {
                _selectedChildrenCount--;
            }
            OnPropetyChanged("SelectedChildrenCount");
            OnChildSelectionChangedOverride(isSelected);
        }

        /// <summary>
        /// Informs the element that selection of one of its children has been changed.
        /// It can be safely overriden by derived classes.
        /// </summary>
        /// <param name="isSelected">New value of the IsSelected property.</param>
        protected virtual void OnChildSelectionChangedOverride(bool isSelected)
        { }
        #endregion

        #region INotifyPropertyChanged members and helper methods
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that has been changed.</param>
        protected virtual void OnPropetyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a node in the members tree.
    /// </summary>
    internal class MembersTreeNode : MembersTreeElement
    {
        #region Fields
        private IFilterAxis _axis;
        private IMember _member;
        private bool _isSelected;

        private MembersTreeElement _parent;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">Parent of this node 
        /// (can be a single node or the whole tree for root nodes).</param>
        /// <param name="member">Member for this node.</param>
        public MembersTreeNode(MembersTreeElement parent, IMember member, MembersTreeSelectionMode selectionMode)
            : base(selectionMode)
        {
            _parent = parent;
            _member = member;
            _axis = _member.Level.Hierarchy.Axis;

            _isSelected = !_axis.IsMemberFiltered(_member);
        }

        #region Properties
        /// <summary>
        /// Gets member for this node.
        /// </summary>
        public IMember Member
        {
            get { return _member; }
        }

        /// <summary>
        /// Gets or sets selection for this node. If the node is selected, the member
        /// is not filtered.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    _parent.OnChildSelectionChanged(_isSelected);
                    UpdateChildrenSelection();
                    OnPropetyChanged("IsSelected");
                }
            }
        }

        private bool SelectionChanged
        {
            get { return _isSelected == _axis.IsMemberFiltered(_member); }
        }

        /// <summary>
        /// Informs whether this node has any children.
        /// </summary>
        public bool HasChildren
        {
            get { return _member.HasChildren; }
        }
        #endregion

        /// <summary>
        /// Updates selection for children. This method is invoked after children are
        /// initialized.
        /// </summary>
        protected override void UpdateChildrenSelection()
        {
            if (SelectionMode == MembersTreeSelectionMode.ChildrenControlParent)
            {
                if (!AreChildrenInitialized || _isSelected)
                {
                    return;
                }

                foreach (MembersTreeNode child in Children)
                {
                    child.IsSelected = false;
                }
            }
            else if (SelectionMode == MembersTreeSelectionMode.ParentControlsChildren)
            {
                if (!AreChildrenInitialized || !_isSelected)
                {
                    return;
                }
                foreach (MembersTreeNode child in Children)
                {
                    child.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Accepts changes for this element and checks whether all children, even
        /// those that are not initialized, should accept changes.
        /// </summary>
        /// <returns>Returns true, if all children should accepts changes. Returns
        /// false if only initialized children should accepts changes.</returns>
        protected override bool AcceptChangesForThisElement(IList<IMember> newVisibleMembers,
            IList<IMember> newNotVisibleMembers)
        {
            bool selectionChanged = SelectionChanged;
            if (selectionChanged)
            {
                if (_isSelected)
                {
                    newVisibleMembers.Add(_member);
                }
                else
                {
                    newNotVisibleMembers.Add(_member);
                }
            }
            return selectionChanged && !IsSelected;
        }

        /// <summary>
        /// Rejects changes for this element (not for its children).
        /// </summary>
        protected override void RejectChangesForThisElement()
        {
            _isSelected = !_axis.IsMemberFiltered(_member);
        }

        /// <summary>
        /// Returns all children for this element.
        /// </summary>
        /// <returns>List of all children of this element.</returns>
        protected override IList<MembersTreeNode> InitializeChildren()
        {
            List<MembersTreeNode> result = new List<MembersTreeNode>();
            foreach (IMember childMember in _member.Children)
            {
                result.Add(new MembersTreeNode(this, childMember, SelectionMode));
            }
            return result;
        }

        /// <summary>
        /// Informs the element that selection of one of its children has been changed.
        /// </summary>
        /// <param name="isSelected">New value of the IsSelected property.</param>
        protected override void OnChildSelectionChangedOverride(bool isSelected)
        {
            if (SelectionMode == MembersTreeSelectionMode.ChildrenControlParent)
            {
                if (isSelected && !this.IsSelected)
                {
                    this.IsSelected = true;
                }
            }
            else if (SelectionMode == MembersTreeSelectionMode.ParentControlsChildren)
            {
                if (isSelected && !this.IsSelected && SelectedChildrenCount == Children.Count)
                {
                    this.IsSelected = true;
                }
            }
        }
    }

    /// <summary>
    /// The whole members tree. It is used as a binding source for the members tree view.
    /// </summary>
    internal class MembersTree : MembersTreeElement
    {
        private ILevel _level;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">Level that contains members for the tree.</param>
        public MembersTree(ILevel level, MembersTreeSelectionMode selectionMode)
            : base(selectionMode)
        {
            _level = level;
        }

        public ILevel Level
        {
            get { return _level; }
        }

        public void AcceptChanges()
        {
            List<IMember> newVisibleMembers = new List<IMember>();
            List<IMember> newNotVisibleMembers = new List<IMember>();
            AcceptChanges(newVisibleMembers, newNotVisibleMembers);
            _level.Hierarchy.Axis.FilterMembers(newVisibleMembers, newNotVisibleMembers);
        }

        /// <summary>
        /// Returns all children of this element.
        /// </summary>
        /// <returns>List of all children of this element.</returns>
        protected override IList<MembersTreeNode> InitializeChildren()
        {
            List<MembersTreeNode> result = new List<MembersTreeNode>();
            foreach (IMember member in _level.Members)
            {
                result.Add(new MembersTreeNode(this, member, SelectionMode));
            }
            return result;
        }
    }

    public enum MembersTreeSelectionMode
    {
        ParentControlsChildren,
        ChildrenControlParent
    }
}
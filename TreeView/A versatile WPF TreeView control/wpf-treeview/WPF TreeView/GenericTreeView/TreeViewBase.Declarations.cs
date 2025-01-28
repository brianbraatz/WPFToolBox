// hardcodet.net WPF TreeView control
// Copyright (c) 2008 Philipp Sumi, Evolve Software Technologies
// Contact and Information: http://www.hardcodet.net
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This software is provided "AS IS" with no warranties of any kind.
// The entire risk arising out of the use or performance of the software
// and source code is with you.
//
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.


using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Hardcodet.Wpf.GenericTreeView
{
  /// <summary>
  /// Encapsulates dependency properties etc.
  /// </summary>
  public abstract partial class TreeViewBase<T>
  {
    #region SelectedItemChanged event

    /// <summary>
    /// A custom routed event which is fired if the
    /// <see cref="SelectedItemProperty"/> of the tree
    /// was changed.
    /// </summary>
    public static RoutedEvent SelectedItemChangedEvent;

    /// <summary>
    /// A custom event which is fired if the <see cref="SelectedItem"/>
    /// of the tree was changed.
    /// </summary>
    public event RoutedTreeItemEventHandler<T> SelectedItemChanged
    {
      add { AddHandler(SelectedItemChangedEvent, value); }
      remove { RemoveHandler(SelectedItemChangedEvent, value); }
    }

    /// <summary>
    /// Fires the <see cref="SelectedItemChangedEvent"/> routed
    /// event.
    /// </summary>
    /// <param name="newItem">The currently selected tree item,
    /// or null, if no item is selected.</param>
    /// <param name="oldItem">The previously selected item, if
    /// any.</param>
    protected void RaiseSelectedItemChangedEvent(T newItem, T oldItem)
    {
      RoutedTreeItemEventArgs<T> args;
      args = new RoutedTreeItemEventArgs<T>(newItem, oldItem);
      args.Source = this;
      RaiseEvent(args);
    }

    #endregion


    #region Items dependency property

    /// <summary>
    /// Gets or sets the items to be bound to three.
    /// </summary>
    public static readonly DependencyProperty ItemsProperty;

    /// <summary>
    /// A property wrapper for the <see cref="ItemsProperty"/>
    /// dependency property:<br/>
    /// Gets or sets the items to be bound to three.
    /// </summary>
    public IEnumerable<T> Items
    {
      get { return (IEnumerable<T>) GetValue(ItemsProperty); }
      set { SetValue(ItemsProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="ItemsProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="Items"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>) d;
      IEnumerable<T> newValue = (IEnumerable<T>) e.NewValue;

      //don't do anything if the control is being created
      if (!owner.IsInitialized) return;

      //get the layout if we preserve it
      TreeLayout layout = owner.PreserveLayoutOnRefresh ? owner.GetTreeLayout() : null;
      owner.RenderTree(newValue, layout);
    }

    #endregion


    #region SelectedItem dependency property

    /// <summary>
    /// Gets or sets the currently selected item, if any. Setting a null
    /// reference deselects the currently selected node.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty;

    /// <summary>
    /// A property wrapper for the <see cref="SelectedItemProperty"/>
    /// dependency property:<br/>
    /// Gets or sets the currently selected item, if any. Setting a null
    /// reference deselects the currently selected node.
    /// </summary>
    public T SelectedItem
    {
      get { return (T) GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="SelectedItemProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="SelectedItem"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>) d;      
      T newValue = (T) e.NewValue;
      T oldValue = (T) e.OldValue;
      TreeView tree = owner.Tree;

      //don't do anything if the control is being created
      if (!owner.IsInitialized) return;

      //if there is no tree, there's nothing to select
      if (tree == null) return;
    
      if (newValue == null)
      {
        //none of the Items should be selected - but we'll select a root node
        //if available
        if (owner.RootNode != null)
        {
          owner.RootNode.IsSelected = true;
        }
        else if (tree.SelectedItem != null)
        {
          //if we have no root to select, clear the current selection
          TreeViewItem item = (TreeViewItem)tree.SelectedItem;
          item.IsSelected = false;
        }
      }
      else if (tree.SelectedItem != null && ((TreeViewItem)tree.SelectedItem).Header == newValue)
      {
        //the item is already selected - we're fine
        TreeViewItem item = (TreeViewItem)tree.SelectedItem;
        item.BringIntoView();
      }
      else
      {
        //select the item, if there is a node available for it
        TreeViewItem item = owner.TryFindItemNode(tree.Items, owner.GetItemKey(newValue), true);
        if (item != null)
        {
          item.IsSelected = true;
          item.BringIntoView();
        }
        else
        {
          //if the tree lazily loads nodes, the ancestors and the node
          //must be created first
          owner.EnsureItemNodeIsSelected(newValue);
        }
      }

      //bubble custom event
      if (!ReferenceEquals(newValue, oldValue))
      {
        owner.RaiseSelectedItemChangedEvent(newValue, oldValue);
      }
    }

    #endregion


    #region IsLazyLoading dependency property

    /// <summary>
    /// Gets or sets whether tree nodes are being created on demand. If set to
    /// true (default value), nodes are being created as soon as they are going
    /// to be displayed the first time because their parent node is being expanded.
    /// </summary>
    public static readonly DependencyProperty IsLazyLoadingProperty;

    /// <summary>
    /// A property wrapper for the <see cref="IsLazyLoadingProperty"/>
    /// dependency property:<br/>
    /// Gets or sets whether tree nodes are being created on demand. If set to
    /// true (default value), nodes are being created as soon as they are going
    /// to be displayed the first time because their parent node is being expanded.
    /// </summary>
    public bool IsLazyLoading
    {
      get { return (bool) GetValue(IsLazyLoadingProperty); }
      set { SetValue(IsLazyLoadingProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="IsLazyLoadingProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="IsLazyLoading"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void IsLazyLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>)d;

      //don't do anything if the control is being created
      if (!owner.IsInitialized) return;

      //recreate the three
      owner.Refresh(owner.GetTreeLayout());
    }

    #endregion


    #region Tree dependency property

    /// <summary>
    /// The underlying WPF <see cref="TreeView"/>
    /// control.
    /// </summary>
    public static readonly DependencyProperty TreeProperty;


    /// <summary>
    /// A property wrapper for the <see cref="TreeProperty"/>
    /// dependency property:<br/>
    /// Gets or sets the underlying WPF <see cref="TreeView"/> control.
    /// </summary>
    public TreeView Tree
    {
      get { return (TreeView)GetValue(TreeProperty); }
      set { SetValue(TreeProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="TreeProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="TreeView"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void TreePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>)d;
      TreeView oldValue = (TreeView) e.OldValue;
      TreeView newValue = (TreeView)e.NewValue;
    
      if (oldValue != null)
      {
        //deregister event listeners
        oldValue.SelectedItemChanged -= owner.OnSelectedItemChanged;
        oldValue.MouseRightButtonUp -= owner.OnRightMouseButtonUp;
      }

      owner.Content = newValue;
      if (newValue != null) newValue.Style = owner.TreeStyle;

      if (newValue != null)
      {
        //register event listeners
        newValue.SelectedItemChanged += owner.OnSelectedItemChanged;
        newValue.MouseRightButtonUp += owner.OnRightMouseButtonUp;
      }

      //don't do anything if the control is being created
      if (!owner.IsInitialized) return;

      //render the tree
      owner.Refresh();
    }

    #endregion


    #region TreeStyle dependency property

    /// <summary>
    /// The style to be attached to the control's <see cref="TreeProperty"/>.
    /// </summary>
    public static readonly DependencyProperty TreeStyleProperty;


    /// <summary>
    /// A property wrapper for the <see cref="TreeStyleProperty"/>
    /// dependency property:<br/>
    /// The style to be attached to the control's <see cref="TreeProperty"/>.
    /// </summary>
    public Style TreeStyle
    {
      get { return (Style)GetValue(TreeStyleProperty); }
      set { SetValue(TreeStyleProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="TreeStyleProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="TreeStyle"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void TreeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>)d;
      Style newValue = (Style)e.NewValue;

      //assign new style
      if (owner.Tree != null) owner.Tree.Style = newValue;
    }

    #endregion


    #region RootNode dependency property

    /// <summary>
    /// A custom root node, which can be declared for the tree. If this
    /// property is not null, All items of the tree will be rendered
    /// as childs of this item.
    /// </summary>
    public static readonly DependencyProperty RootNodeProperty;


    /// <summary>
    /// A property wrapper for the <see cref="RootNodeProperty"/>
    /// dependency property:<br/>
    /// A custom root node, which can be declared for the tree. If this
    /// property is not null, All items of the tree will be rendered
    /// as childs of this item.
    /// </summary>
    public TreeViewItem RootNode
    {
      get { return (TreeViewItem)GetValue(RootNodeProperty); }
      set { SetValue(RootNodeProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="RootNodeProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="RootNode"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void RootNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //just recreate the tree - this takes care of everything
      TreeViewBase<T> owner = (TreeViewBase<T>)d;

      //don't do anything if the control is being created
      if (!owner.IsInitialized) return;

      owner.Refresh(owner.GetTreeLayout());
    }

    #endregion


    #region PreserveLayoutOnRefresh dependency property

    /// <summary>
    /// This property defines whether the tree's layout (selected,
    /// expanded nodes) should be preserved as good as possible,
    /// if the tree is being recreated due to a new data source
    /// or a refresh.
    /// </summary>
    public static readonly DependencyProperty PreserveLayoutOnRefreshProperty;

    /// <summary>
    /// A property wrapper for the <see cref="PreserveLayoutOnRefreshProperty"/>
    /// dependency property:<br/>
    /// Gets or sets  whether the tree's layout (selected,
    /// expanded nodes) should be preserved as good as possible,
    /// if the tree is being recreated due to a new data source
    /// or a refresh.
    /// </summary>
    public bool PreserveLayoutOnRefresh
    {
      get { return (bool)GetValue(PreserveLayoutOnRefreshProperty); }
      set { SetValue(PreserveLayoutOnRefreshProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="PreserveLayoutOnRefreshProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="PreserveLayoutOnRefresh"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void PreserveLayoutOnRefreshPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //currently not in use
    }

    #endregion


    #region TreeNodeStyle dependency property

    /// <summary>
    /// A style which is explicitly applied to every tree node except
    /// the custom <see cref="RootNode"/>.
    /// </summary>
    public static readonly DependencyProperty TreeNodeStyleProperty;

    /// <summary>
    /// A property wrapper for the <see cref="TreeNodeStyleProperty"/>
    /// dependency property:<br/>
    /// A style which is explicitly applied to every <see cref="TreeViewItem"/>
    /// except the custom <see cref="RootNode"/>.
    /// </summary>
    public Style TreeNodeStyle
    {
      get { return (Style)GetValue(TreeNodeStyleProperty); }
      set { SetValue(TreeNodeStyleProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="TreeNodeStyleProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="TreeNodeStyle"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void TreeNodeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>)d;
      Style style = (Style)e.NewValue;
     
      //assign the style to every node of the tree
      foreach (TreeViewItem item in owner.RecursiveNodeList)
      {
        item.Style = style;
      }
    }

    #endregion


    #region SelectNodesOnRightClick dependency property

    /// <summary>
    /// If set to true, treeview items are automatically selected on right clicks,
    /// which simplifies context menu handling.
    /// </summary>
    public static readonly DependencyProperty SelectNodesOnRightClickProperty;

    /// <summary>
    /// A property wrapper for the <see cref="SelectNodesOnRightClickProperty"/>
    /// dependency property:<br/>
    /// If set to true, treeview items are automatically selected on right clicks,
    /// which simplifies context menu handling.
    /// </summary>
    public bool SelectNodesOnRightClick
    {
      get { return (bool)GetValue(SelectNodesOnRightClickProperty); }
      set { SetValue(SelectNodesOnRightClickProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="SelectNodesOnRightClickProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="SelectNodesOnRightClick"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void SelectNodesOnRightClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //not in use
    }

    #endregion


    #region NodeContextMenu dependency property

    /// <summary>
    /// Defines a context menu to be assigned to open on all nodes of
    /// the tree. If this property is set, the context menu will be
    /// displayed if a node of the tree is being right-clicked, *and*
    /// no custom context menu has been assigned to the node.<br/>
    /// When handling menu-related events, the clicked node that
    /// caused the event can be determined by accessing the menu's
    /// <see cref="ContextMenu.PlacementTarget"/> property.
    /// </summary>
    public static readonly DependencyProperty NodeContextMenuProperty;


    /// <summary>
    /// A property wrapper for the <see cref="NodeContextMenuProperty"/>
    /// dependency property:<br/>
    /// Defines a context menu to be assigned to open on all nodes of
    /// the tree. If this property is set, the context menu will be
    /// displayed if a node of the tree is being right-clicked, *and*
    /// no custom context menu has been assigned to the node.<br/>
    /// When handling menu-related events, the clicked node that
    /// caused the event can be determined by accessing the menu's
    /// <see cref="ContextMenu.PlacementTarget"/> property.
    /// </summary>
    public ContextMenu NodeContextMenu
    {
      get { return (ContextMenu)GetValue(NodeContextMenuProperty); }
      set { SetValue(NodeContextMenuProperty, value); }
    }


    /// <summary>
    /// Handles changes on the <see cref="NodeContextMenuProperty"/> dependency property. As
    /// WPF internally uses the dependency property system and bypasses the
    /// <see cref="NodeContextMenu"/> property wrapper, updates should be handled here.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void NodeContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //currently not in use
    }

    #endregion


    #region static constructor

    /// <summary>
    /// Inits the control's properties and events.
    /// </summary>
    static TreeViewBase()
    {
      //register and event listener that gets bubbled tree node expansion events
      EventManager.RegisterClassHandler(typeof(TreeViewBase<T>), TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnTreeNodeExpanded));

      //register item change event
      SelectedItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble,
                                         typeof (RoutedTreeItemEventHandler<T>),
                                         typeof (TreeViewBase<T>));

      FrameworkPropertyMetadata md = new FrameworkPropertyMetadata(true, IsLazyLoadingPropertyChanged);
      IsLazyLoadingProperty = DependencyProperty.Register("IsLazyLoading", typeof (bool), typeof (TreeViewBase<T>), md);                                                      
      
      //register UI dependency properties
      md = new FrameworkPropertyMetadata(null, TreePropertyChanged);
      TreeProperty = DependencyProperty.Register("Tree", typeof(TreeView), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(null, TreeStylePropertyChanged);
      TreeStyleProperty = DependencyProperty.Register("TreeStyle", typeof(Style), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(null, RootNodePropertyChanged);
      RootNodeProperty = DependencyProperty.Register("RootNode", typeof(TreeViewItem), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(true, PreserveLayoutOnRefreshPropertyChanged);
      PreserveLayoutOnRefreshProperty = DependencyProperty.Register("PreserveLayoutOnRefresh", typeof(bool), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(null, TreeNodeStylePropertyChanged);
      TreeNodeStyleProperty = DependencyProperty.Register("TreeNodeStyle", typeof(Style), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(false, SelectNodesOnRightClickPropertyChanged);
      SelectNodesOnRightClickProperty = DependencyProperty.Register("SelectNodesOnRightClick", typeof(bool), typeof(TreeViewBase<T>), md);

      md = new FrameworkPropertyMetadata(null, NodeContextMenuPropertyChanged);
      NodeContextMenuProperty = DependencyProperty.Register("NodeContextMenu", typeof(ContextMenu), typeof(TreeViewBase<T>), md);

      //register item dependency properties
      md = new FrameworkPropertyMetadata(null, SelectedItemPropertyChanged);
      SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof (T), typeof (TreeViewBase<T>), md);                                                      
      
      md = new FrameworkPropertyMetadata(null, ItemsPropertyChanged);
      ItemsProperty = DependencyProperty.Register("Items", typeof (IEnumerable<T>), typeof (TreeViewBase<T>), md);                                                      
    }

    #endregion

  }
}

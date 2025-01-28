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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hardcodet.Wpf.GenericTreeView
{
  /// <summary>
  /// An abstract composite control that hosts a single
  /// <see cref="TreeView"/> control.
  /// </summary>
  /// <typeparam name="T">The type of the tree's items.</typeparam>
  public abstract partial class TreeViewBase<T> : UserControl where T:class
  {
    #region fields

    /// <summary>
    /// Helper flag which indicates that selection
    /// events from the tree should be ignored. Used
    /// during recreation of the tree.
    /// </summary>
    private bool ignoreItemChangeEvents = false;

    /// <summary>
    /// This flag is set to true as soon as the tree has
    /// been rendered the first time.
    /// </summary>
    private bool isTreeRendered = false;

    #endregion


    #region tree initialization

    /// <summary>
    /// Makes sure a tree is present, and renders the control
    /// with the assigned items.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      //if necessary, create a default tree, then render the first time
      if (!isTreeRendered)
      {
        if (Tree == null)
        {
          //set a default tree - this also triggers rendering
          Tree = new TreeView();
        }
        else
        {
          //everything has been created, but the tree has not
          //been rendered yet, because IsInitialized was false
          //-> render now
          Refresh(null);
        }
      }
    }

    #endregion


    #region refresh

    /// <summary>
    /// Triggers a refresh of the tree.
    /// </summary>
    public virtual void Refresh()
    {
      //get the layout if we preserve it
      TreeLayout layout = PreserveLayoutOnRefresh ? GetTreeLayout() : null;
      Refresh(layout);
    }


    /// <summary>
    /// Recreates the tree with a given tree layout.
    /// </summary>
    /// <param name="layout">Defines the layout of the tree.</param>
    public virtual void Refresh(TreeLayout layout)
    {
      RenderTree(Items, layout);
    }

    #endregion


    #region node handling

    /// <summary>
    /// Renders the tree and optionally preserves its current layout.
    /// </summary>
    /// <param name="items">The items to be displayed on the tree.</param>
    /// <param name="layout">The layout to be applied on the tree.</param>
    private void RenderTree(IEnumerable<T> items, TreeLayout layout)
    {
      //if there is no tree, there is nothing to render
      if (Tree == null) return;

      //set rendered flag
      isTreeRendered = true;

      //suppress selection change event if the tree is cleared
      ignoreItemChangeEvents = true;

      //clear all items (root, if available, will be re-added)
      Tree.Items.Clear();
      
      TreeViewItem root = RootNode;
      if (root != null)
      {
        //if we have a root node, clear and expand it
        root.Items.Clear();
        root.IsExpanded = true;
        Tree.Items.Add(root);
      }

      //if a null value was assigned (no items), we're done
      if (items == null) return;

      //recreate root item nodes
      List<TreeViewItem> rootList = new List<TreeViewItem>();
      foreach (T item in items)
      {
        CreateItemNode(item, rootList, layout);
      }

      //assign all items at once to the tree
      //-> render within root, or on the tree itself
      ItemCollection treeNodes = root != null ? root.Items : Tree.Items;
      foreach (TreeViewItem item in rootList) treeNodes.Add(item);

      //verify the selected node - if it does no longer exist, reset
      //the SelectedItem property
      TreeViewItem selectedNode = null;
      if (layout != null)
      {
        string itemId = layout.SelectedItemId;
        selectedNode = TryFindItemNode(Tree.Items, itemId, true);
      }
      if (selectedNode == null) SelectedItem = null;


      if (Tree.IsKeyboardFocusWithin && selectedNode != null)
      {
        //if the tree has the focus, this will auto-select the root node once
        //the tree is rendered - prevent this by explicitely setting the focus
        //to root, than to the selected item
        //-> both select are needed, depending on the currently selected item
        //(direct child of root or not makes a difference)
        if (root != null) Keyboard.Focus(root);
        Keyboard.Focus(selectedNode);
      }

      //reactivate events
      ignoreItemChangeEvents = false;
    }


    /// <summary>
    /// Creates a single <see cref="TreeViewItem"/> node that represents a
    /// given item of the <see cref="Items"/> collection and assigns the
    /// item to the node's <see cref="HeaderedItemsControl.Header"/>
    /// property.
    /// </summary>
    /// <param name="item">The item which is being represented by a tree node.</param>
    /// <param name="parentNodes">The parent collection that contains the created
    /// tree node item.</param>
    /// <param name="layout">Stores a predefined layout for the tree. May be null.</param>
    private void CreateItemNode(T item, IList parentNodes, TreeLayout layout)
    {
      bool hasLayout = layout != null;

      //create a tree node and assign the represented item to the
      //node header
      TreeViewItem treeNode = new TreeViewItem();
      treeNode.Header = item;
      treeNode.Style = TreeNodeStyle;

      //get the unique ID of the item
      string nodeId = GetItemKey(item);

      bool isExpanded = hasLayout && layout.IsNodeExpanded(nodeId);
      if (isExpanded || !IsLazyLoading)
      {
        //render childs if the node is expanded according to the
        //layout information, or if lazy loading is not active
        foreach (T childItem in GetChildItems(item))
        {
          CreateItemNode(childItem, treeNode.Items, layout);
        }

        if (isExpanded) treeNode.IsExpanded = true;
      }
      else if (GetChildItems(item).Count > 0)
      {
        //if the item has child nodes which we don't need to create right
        //now (not expanded and lazy loading is active), insert
        //a dummy node which results in an expansion indicator
        treeNode.Items.Add(new TreeViewItem());
      }

      if (item == SelectedItem || hasLayout && nodeId.Equals(layout.SelectedItemId))
      {
        //select the item and notify
        treeNode.IsSelected = true;
      }

      parentNodes.Add(treeNode);
    }



    /// <summary>
    /// A static event listener which is invoked if a tree's node is being
    /// expanded. This event is being observed because nodes may need to
    /// be created if lazy loading is active.
    /// </summary>
    /// <param name="sender">The processed <see cref="TreeViewBase{T}"/>
    /// control.</param>
    /// <param name="e">Event arguments. The <see cref="RoutedEventArgs.OriginalSource"/>
    /// property provides the expanded tree node.</param>
    private static void OnTreeNodeExpanded(object sender, RoutedEventArgs e)
    {
      TreeViewBase<T> owner = (TreeViewBase<T>)sender;
      TreeViewItem treeNode = (TreeViewItem) e.OriginalSource;
      owner.OnItemNodeExpanded(treeNode);

      //do not mark as handled - the event may bubble up the tree:
      //e.Handled = true;
    }


    /// <summary>
    /// Handles lazy creation of child nodes if a node is being expanded
    /// the first time.
    /// </summary>
    private void OnItemNodeExpanded(TreeViewItem treeNode)
    {
      //the tree has already been created - there is nothing more to do here
      if (!IsLazyLoading)
      {
        //the tree has already been created - there is nothing more to do here
        //however, if the node does not contain anything, don't show as expanded
        if (treeNode.Items.Count == 0)
        {
          treeNode.IsExpanded = false;
        }

        return;
      }

      //the node does not represent one of our bound items
      //(a custom root or some other injected stuff)
      T item = treeNode.Header as T;
      if (item == null) return;

      //if the item has never been expanded yet, it contains a dummy
      //node - replace that one and insert real data
      if (treeNode.Items.Count == 1 && ((TreeViewItem)treeNode.Items[0]).Header == null)
      {
        treeNode.Items.Clear();
      }

      //get the child items
      ICollection<T> childItems = GetChildItems(item);
      if (treeNode.Items.Count == 0)
      {
        foreach (T childItem in childItems)
        {
          //create child items
          CreateItemNode(childItem, treeNode.Items, null);
        }
      }

      if (treeNode.Items.Count == 0)
      {
        //collapse again if there was no data at all
        //unlikely, but the bound item's childs might have changed
        treeNode.IsExpanded = false;
      }
    }


    /// <summary>
    /// Ensures that a node for a given item (and all its
    /// ancestors) has been created and selectes it immediately.<br/>
    /// This method basically expands all ancestors of the submitted
    /// <paramref name="item"/> which makes sure a corresponding
    /// node will be available.
    /// </summary>
    /// <param name="item">The item that should be represented
    /// by an existing tree node.</param>
    protected void EnsureItemNodeIsSelected(T item)
    {
      //make sure all of the item's ancestors which will be expanded
      //-> this ensures our item will be visible on the tree when
      //we render it.
      List<string> parentNodes = new List<string>();
      T parentItem = GetParentItem(item);
      while (parentItem != null)
      {
        string itemKey = GetItemKey(parentItem);
        parentNodes.Insert(0, itemKey);
        parentItem = GetParentItem(parentItem);
      }

      ItemCollection items = RootNode == null ? Tree.Items : RootNode.Items;
      foreach (string nodeId in parentNodes)
      {
        //expand the parent node, than move a level lower
        TreeViewItem parentNode = TryFindItemNode(items, nodeId, false);
        
        //if we don't find the parent, the tree's nodes are not up-to-date
        //which means we should probably refresh - write to debug output...
        if (parentNode == null)
        {
          string msg = "Warning: Unexpected result in method {0}: The tree does not contain a tree node for item '{1}' although it's in the hierarchy of the tree's bound items.";
          msg = String.Format(msg, "TreeViewBase<T>.EnsureItemNodeIsSelected", nodeId);
          Debug.WriteLine(msg);
          break;
        }
        
        parentNode.IsExpanded = true;
        items = parentNode.Items;
      }
    }

    #endregion


    #region get layout

    /// <summary>
    /// Gets the tree's current layout of expanded / selected
    /// nodes.
    /// </summary>
    /// <returns>The tree layout.</returns>
    public virtual TreeLayout GetTreeLayout()
    {
      TreeLayout layout = new TreeLayout();
      T selected = SelectedItem;

      //set selected item
      if (selected != null) layout.SelectedItemId = GetItemKey(selected);

      //if there is no tree yet, we're done
      if (Tree != null)
      {
        //get nodes of all expanded nodes
        GetExpandedNodes(layout.ExpandedNodeIds, Tree.Items);
      }

      return layout;
    }


    /// <summary>
    /// Recursively determines all expanded nodes of the tree, and
    /// stores the qualified IDs of the underlying items in a list.
    /// </summary>
    /// <param name="nodeIds">The list to be populated.</param>
    /// <param name="nodes">The tree nodes to be processed recursively.</param>
    private void GetExpandedNodes(List<string> nodeIds, ItemCollection nodes)
    {
      foreach (TreeViewItem treeNode in nodes)
      {
        //if we're having a dummy node, break
        if (!ReferenceEquals(treeNode, RootNode) && treeNode.Header == null) break;

        if (treeNode.IsExpanded && treeNode.Header != null)
        {
          //cast safely - the root node's header might be anything
          T item = treeNode.Header as T;
          if (item != null) nodeIds.Add(GetItemKey(item));
        }

        //process recursively (always, even if the item is collapsed!)
        GetExpandedNodes(nodeIds, treeNode.Items);
      }
    }

    #endregion


    #region find nodes

    /// <summary>
    /// Gets a given node of the tree. Note that with lazy loading
    /// enabled, the tree returns null, if the corresponding tree
    /// node has not been created yet.
    /// </summary>
    /// <param name="item">The item that is being represented
    /// by the node to be looked up.</param>
    /// <returns>The node that corresponds to the item, if any.
    /// Otherwise null.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="item"/>
    /// is a null reference.</exception>
    public virtual TreeViewItem TryFindNode(T item)
    {
      if (item == null) throw new ArgumentNullException("item");

      //get item key and delegate to overload
      string itemKey = GetItemKey(item);
      return TryFindNodeByKey(itemKey);
    }


    /// <summary>
    /// Returns a node of the tree which represents a given item.
    /// Note that if lazy loading is enabled, the tree returns null,
    /// if the corresponding tree node has not been created yet.
    /// </summary>
    /// <param name="itemKey">The item identifier, as created by
    /// the <see cref="GetItemKey"/> method.</param>
    /// <returns>The node that matches the submitted key, if any.
    /// Otherwise null.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="itemKey"/>
    /// is a null reference.</exception>
    public virtual TreeViewItem TryFindNodeByKey(string itemKey)
    {
      if (itemKey == null) throw new ArgumentNullException("itemKey");
      return TryFindItemNode(Tree.Items, itemKey, true);
    }


    /// <summary>
    /// Recursively searches the tree for a node that represents
    /// a given item starting at any given level of the tree. Note that
    /// with lazy loading enabled, this method returns null if the
    /// matching node has not been created yet.
    /// </summary>
    /// <param name="treeNodes">The items to be browsed recursively.</param>
    /// <param name="itemKey">The unique node ID of the item.</param>
    /// <param name="recurse">Whether to limit the search to the <paramref name="treeNodes"/>
    /// collection or not. If true, the descendants of all items will be searched
    /// recursively.</param>
    /// <returns>The matching node, if any. Otherwise null.</returns>
    protected TreeViewItem TryFindItemNode(ItemCollection treeNodes, string itemKey, bool recurse)
    {
      foreach (TreeViewItem treeNode in treeNodes)
      {
        T nodeItem = treeNode.Header as T;
        if (nodeItem != null)
        {
          //the root item does not provide a matching header...
          string id = GetItemKey(nodeItem);
          if (itemKey == id) return treeNode;
        }

        //browse child items
        if (recurse)
        {
          TreeViewItem match = TryFindItemNode(treeNode.Items, itemKey, true);
          if (match != null) return match;
        }
      }

      return null;
    }

    #endregion


    #region selected item changed

    /// <summary>
    /// Updates the <see cref="SelectedItem"/> property if a new
    /// item has been selected. This causes a very small overhead, as
    /// a user-interaction causes the <see cref="SelectedItemPropertyChanged"/>
    /// method to actively reselect the node, but keeps the <see cref="SelectedItem"/>
    /// entity reference and the visual tree in sync.
    /// </summary>
    private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (ignoreItemChangeEvents)
      {
        //ignore event if we're currently processing the tree structure
        e.Handled = true;
        return;
      }

      if (e.NewValue != null)
      {
        TreeViewItem node = (TreeViewItem)e.NewValue;
        T item = node.Header as T;

        SelectedItem = item;
      }

      e.Handled = true;
    }

    #endregion


    #region right mouse button click

    /// <summary>
    /// Intercepts right mouse button clicks an checks whether a tree
    /// node was clicked. If this is the case, the node will be selected
    /// in case it's not selected an the <see cref="SelectNodesOnRightClick"/>
    /// dependency property is set.<br/>
    /// If the <see cref="NodeContextMenu"/> property is set and no custom
    /// context menu was assigned to the item, the <see cref="NodeContextMenu"/>
    /// will be opened with its <see cref="ContextMenu.PlacementTarget"/> property
    /// set to the clicked tree node. Right clicks on a <see cref="RootNode"/>
    /// will be ignored.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRightMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
      //return if no node was clicked
      TreeViewItem item = e.Source as TreeViewItem;
      if (item == null) return;

      //activate item if necessary
      if (SelectNodesOnRightClick && item.IsSelected == false)
      {
        item.IsSelected = true;
      }

      //context menu handling: don't do anything if no context menu
      //was defined or one was assigned by custom code
      if (NodeContextMenu == null || item.ContextMenu != null) return;

      //also don't show a context menu if the root node was clicked
      if (ReferenceEquals(item, RootNode)) return;

      //temporarily assign the menu to the item - this ensures that
      //a the PlacementTarget property of the context menu points to
      //the item (can be evaluated in a click event or command handler)
      item.ContextMenu = NodeContextMenu;

      //open the context menu for the clicked item
      NodeContextMenu.PlacementTarget = item;
      NodeContextMenu.IsOpen = true;

      //mark as handled - let the event bubble on...
      e.Handled = true;

      //reset the context menu assignment
      item.ContextMenu = null;
    }

    #endregion


    #region iterate tree nodes

    /// <summary>
    /// Gets an enumerator that provides recursive browsing through
    /// all nodes of the tree. Note that this enumerator may not return
    /// nodes for all elements in the bound <see cref="Items"/> collection
    /// if lazy loading is enabled.
    /// </summary>
    public IEnumerable<TreeViewItem> RecursiveNodeList
    {
      get
      {
        ItemCollection nodes = RootNode == null ? Tree.Items : RootNode.Items;
        return BrowseNodes(nodes);
      }
    }


    /// <summary>
    /// Recursively browses all items of a given data point group.
    /// </summary>
    /// <param name="nodes">A collection of <see cref="TreeViewItem"/>
    /// instances to be processed recursively.</param>
    /// <returns>Group enumerator.</returns>
    private static IEnumerable<TreeViewItem> BrowseNodes(ItemCollection nodes)
    {
      //process child groups
      foreach (TreeViewItem node in nodes)
      {
        if (node.Header == null)
        {
          yield break;
        }
        else
        {
          yield return node;

          foreach (TreeViewItem item in BrowseNodes(node.Items))
          {
            yield return item;
          }
        }
      }
    }

    #endregion


    #region expand / collapse

    /// <summary>
    /// Expands all nodes of the tree. This means that nodes
    /// for all items will be created even if <see cref="IsLazyLoading"/>
    /// is set to true.
    /// </summary>
    public void ExpandAll()
    {
      foreach (TreeViewItem item in RecursiveNodeList)
      {
        item.IsExpanded = true;
      }
    }


    /// <summary>
    /// Collapses all nodes of the tree. 
    /// </summary>
    /// <remarks>If <see cref="IsLazyLoading"/> is set to true,
    /// the footprint of the tree may be reduced by invoking
    /// <see cref="Refresh()"/>. This automatically discards all
    /// previously created nodes and only recreates the (visible)
    /// root nodes.</remarks>
    public void CollapseAll()
    {
      foreach (TreeViewItem item in RecursiveNodeList)
      {
        item.IsExpanded = false;
      }
    }



    #endregion
    

    #region abstract item handlers

    /// <summary>
    /// Generates a unique identifier for a given
    /// item that is represented as a node of the
    /// tree.
    /// </summary>
    /// <param name="item">An item which is represented
    /// by a tree node.</param>
    /// <returns>A unique key that represents the item.</returns>
    public abstract string GetItemKey(T item);

    /// <summary>
    /// Gets all child items of a given parent item. The
    /// tree needs this method to properly traverse the
    /// logic tree of a given item.
    /// </summary>
    /// <param name="parent">A currently processed item that
    /// is being represented as a node of the tree.</param>
    /// <returns>All child items to be represented by the
    /// tree.</returns>
    public abstract ICollection<T> GetChildItems(T parent);

    /// <summary>
    /// Gets the parent of a given item, if available. If
    /// the item is a top-level element, this method is supposed
    /// to return a null reference.
    /// </summary>
    /// <param name="item">The currently processed item.</param>
    /// <returns>The parent of the item, if available.</returns>
    public abstract T GetParentItem(T item);

    #endregion

  }
}
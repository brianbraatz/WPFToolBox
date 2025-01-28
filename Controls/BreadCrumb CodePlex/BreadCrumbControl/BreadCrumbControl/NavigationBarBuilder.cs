using System;
using System.Windows.Controls;

namespace Controls.Breadcrumb
{
  /// <summary>
  /// Used for Breadcrumb combobox items
  /// </summary>
  class BreadcrumbItem
  {
    private readonly NavigationNode navigationNode;

    public NavigationNode NavigationNode
    {
      get { return navigationNode; }
    }

    public BreadcrumbItem(NavigationNode navigationNode)
    {
      this.navigationNode = navigationNode;
    }

    public override string ToString()
    {
      return navigationNode.NodeName;
    }
  }

  public class SelectedNavigationNodeChangedEventArgs : EventArgs
  {
    readonly private NavigationNode navigationNode;

    public NavigationNode NavigationNode
    {
      get { return navigationNode; }
    }

    public SelectedNavigationNodeChangedEventArgs(NavigationNode navigationNode)
    {
      this.navigationNode = navigationNode;
    }
  }

  class NavigationBarBuilder
  {
    #region Fields and properties

    readonly private Breadcrumb breadcrumbControl;
    readonly private ItemsControl root;
    private string projectName;

    /// <summary>
    /// Project name, used for root node
    /// </summary>
    public string ProjectName
    {
      get { return projectName; }
      set { projectName = value; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor for NavigationBarBuilder class
    /// </summary>
    /// <param name="breadcrumbControl"></param>
    /// <param name="root"></param>
    public NavigationBarBuilder(Breadcrumb breadcrumbControl, ItemsControl root)
    {
      // Check preconditions  
      if (breadcrumbControl == null) { throw new ArgumentNullException("breadcrumbControl"); }
      if (root == null) { throw new ArgumentNullException("root"); }

      // Store params to fields
      this.breadcrumbControl = breadcrumbControl;
      this.root = root;
    }

    #endregion

    #region SelectedNavigationNodeChanged event

    public event EventHandler<SelectedNavigationNodeChangedEventArgs> SelectedNavigationNodeChanged;

    /// <summary>
    /// SelectedNavigationNodeChanged event caller
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnSelectedNavigationNodeChanged(SelectedNavigationNodeChangedEventArgs e)
    {
      if (SelectedNavigationNodeChanged != null)
        SelectedNavigationNodeChanged(this, e);
    }

    /// <summary>
    /// Fire SelectedNavigationNodeChanged event
    /// </summary>
    private void DoSelectedNavigationNodeChanged(NavigationNode navigationNode)
    {
      OnSelectedNavigationNodeChanged(new SelectedNavigationNodeChangedEventArgs(navigationNode));
    }

    #endregion

    /// <summary>
    /// Rebuild Breadcrumb bar items
    /// </summary>
    /// <param name="itemsControl"></param>
    public void RebuildBreadcrumb(ItemsControl itemsControl)
    {
      breadcrumbControl.BreadcrumbItems.Clear();
      breadcrumbControl.BreadcrumbVisible = (itemsControl != null);

      if (itemsControl != null)
      {
        BuildBreadcrumb(itemsControl);
      }
    }

    /// <summary>
    /// Build Breadcrumb bar items
    /// </summary>
    /// <param name="itemsControl"></param>
    private void BuildBreadcrumb(ItemsControl itemsControl)
    {
      // Check precondition
      if (itemsControl == null) { throw new ArgumentNullException("itemsControl"); }

      // Create Breadcrumb ComboBox with Breadcrumb ComboBox style
      System.Windows.Controls.ComboBox breadcrumbComboBox = breadcrumbControl.CreateBreadcrumbComboBox();
      breadcrumbComboBox.Tag = itemsControl;
      breadcrumbComboBox.Text = (itemsControl is NavigationNode) ? ((NavigationNode)itemsControl).NodeName : projectName;

      // Fill Breadcrumb ComboBox
      foreach (object item in itemsControl.Items)
      {
        NavigationNode childNode = item as NavigationNode;
        if (childNode != null)
        {
          BreadcrumbItem breadcrumbItem = new BreadcrumbItem(childNode);
          breadcrumbComboBox.Items.Add(breadcrumbItem);
        }
      }

      // Link event handlers
      breadcrumbComboBox.PreviewMouseDown += breadcrumbComboBox_PreviewMouseDown;
      breadcrumbComboBox.SelectionChanged += breadcrumbComboBox_SelectionChanged;

      if (itemsControl != root)
      {
        // Get parentNode
        ItemsControl parentNode = ((NavigationNode)itemsControl).ParentNode;
        if (parentNode == null)
        {
          parentNode = root;
        }

        // Build path using recursive call
        BuildBreadcrumb(parentNode);
      }

      // Add Breadcrumb ComboBox to Breadcrumb bar
      breadcrumbControl.BreadcrumbItems.Add(breadcrumbComboBox);
    }

    /// <summary>
    /// Handle PreviewMouseDown event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void breadcrumbComboBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      System.Windows.Controls.ComboBox breadcrumbComboBox = sender as System.Windows.Controls.ComboBox;
      if (breadcrumbComboBox != null)
      {
        if (e.OriginalSource.GetType().FullName.Equals("System.Windows.Controls.TextBoxView"))
        {
          // Navigate to a page using the Breadcrumb bar
          NavigationNode navigationNode = breadcrumbComboBox.Tag as NavigationNode;
          if (navigationNode != null)
          {
            DoSelectedNavigationNodeChanged(navigationNode);
            e.Handled = true;
          }
        }
        else if (e.OriginalSource.GetType().FullName.Equals("System.Windows.Shapes.Path"))
        {
          // Don't show empty itemlist
          NavigationNode navigationNode = breadcrumbComboBox.Tag as NavigationNode;
          if (navigationNode != null)
          {
            e.Handled = !navigationNode.HasItems;
          }
        }

      }
    }

    /// <summary>
    /// Navigate to a page using the Breadcrumb bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void breadcrumbComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      System.Windows.Controls.ComboBox breadcrumbComboBox = sender as System.Windows.Controls.ComboBox;
      if (breadcrumbComboBox != null)
        DoSelectedNavigationNodeChanged(((BreadcrumbItem)breadcrumbComboBox.SelectedItem).NavigationNode);
    }
  }
}

using System.Windows.Controls;

namespace Controls.Breadcrumb
{
  public abstract class NavigationNode : TreeViewItem
  {
    #region Fields and properties

    private readonly ItemsControl parentNode;
    public ItemsControl ParentNode
    {
      get { return parentNode; }
    }

    private string errorMessage;
    public string ErrorMessage
    {
      get { return errorMessage; }
      set { errorMessage = value; }
    }

    public string NodeName
    {
      get { return GetNodeName(); }
    }

    public override string ToString()
    {
      return GetNodeName();
    }

    /// <summary>
    /// return parent TreevieItem Container
    /// </summary>
    public TreeViewItem Container
    {
      get { return (ParentNode.ItemContainerGenerator.ContainerFromItem(this) as TreeViewItem); }   
    }

    #endregion

    #region Constructors

    protected NavigationNode(ItemsControl parentNode)
    {
      this.parentNode = parentNode;
    }
    
    protected NavigationNode(string header, bool expanded, ItemsControl parentNode)
    {
      this.parentNode = parentNode;
      Header = header;
      IsExpanded = expanded;
    }

    #endregion

    #region Virtual members

    virtual public void Remove()
    {
      parentNode.Items.Remove(this); // Remove from tree
    }

    abstract public void Refresh();

    abstract protected string GetNodeName();

    #endregion
  }
}

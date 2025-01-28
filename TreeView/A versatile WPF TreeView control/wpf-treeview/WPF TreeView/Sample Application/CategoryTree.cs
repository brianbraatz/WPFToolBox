using System.Collections.Generic;
using Hardcodet.Wpf.GenericTreeView;
using SampleShop.Products;

namespace Hardcodet.Wpf.Samples
{
  /// <summary>
  /// This is a tree control that contains <see cref="ShopCategory"/>
  /// items. Note that if you need a tree that handles items of
  /// different types, you could define a base class, and interface,
  /// or event define <see cref="object"/> as type parameter.
  /// </summary>
  /// <remarks>You should always declare your deriving class as public,
  /// or VS intellisense might not work correctly.</remarks>
  public class CategoryTree : TreeViewBase<ShopCategory>
  {
    /// <summary>
    /// Generates a unique identifier for a given
    /// item that is represented as a node of the
    /// tree.
    /// </summary>
    /// <param name="item">An item which is represented
    /// by a tree node.</param>
    /// <returns>A unique key that represents the item.</returns>
    public override string GetItemKey(ShopCategory item)
    {
      return item.CategoryName;
    }


    /// <summary>
    /// Gets all child items of a given parent item. The
    /// tree needs this method to properly traverse the
    /// logic tree of a given item.
    /// </summary>
    /// <param name="parent">A currently processed item that
    /// is being represented as a node of the tree.</param>
    /// <returns>All child items to be represented by the
    /// tree.</returns>
    public override ICollection<ShopCategory> GetChildItems(ShopCategory parent)
    {
      return parent.SubCategories;
    }


    /// <summary>
    /// Gets the parent of a given item, if available. If
    /// the item is a top-level element, this method is supposed
    /// to return a null reference.
    /// </summary>
    /// <param name="item">The currently processed item.</param>
    /// <returns>The parent of the item, if available.</returns>
    public override ShopCategory GetParentItem(ShopCategory item)
    {
      return item.ParentCategory;
    }
  }
}
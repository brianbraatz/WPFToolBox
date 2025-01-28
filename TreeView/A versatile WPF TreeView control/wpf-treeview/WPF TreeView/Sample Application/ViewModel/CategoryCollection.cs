using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SampleShop.Products
{
  public class CategoryCollection : KeyedCollection<string, ShopCategory>
  {
    ///<summary>
    ///When implemented in a derived class, extracts the key from the specified element.
    ///</summary>
    ///<returns>
    ///The key for the specified element.
    ///</returns>
    ///<param name="item">The element from which to extract the key.</param>
    protected override string GetKeyForItem(ShopCategory item)
    {
      return item.CategoryName;
    }
  }
}

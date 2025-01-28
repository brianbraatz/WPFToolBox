using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SampleShop;
using SampleShop.Products;

namespace Hardcodet.Wpf.Samples.ViewModel
{
  /// <summary>
  /// Provides a simplified view model for the sample
  /// application.
  /// </summary>
  public class ShopModel : INotifyPropertyChanged
  {
    private CategoryCollection categories;

    public CategoryCollection Categories
    {
      get { return categories; }
      set {
        categories = value;
        RaisePropertyChangedEvent("Categories");
      }
    }


    /// <summary>
    /// Refreshes the data.
    /// </summary>
    public void RefreshData()
    {
      Categories = ShopUtil.CreateShop();
    }


    ///<summary>
    ///Occurs when a property value changes.
    ///</summary>
    public event PropertyChangedEventHandler PropertyChanged;


    /// <summary>
    /// Fires the <see cref="PropertyChanged"/> event for a
    /// given property.
    /// </summary>
    /// <param name="propertyName">The changed property.</param>
    private void RaisePropertyChangedEvent(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }


    public ShopModel()
    {
      RefreshData();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace RD.Controls
{
  public sealed class EllipticSelectionChangedEventArgs : RoutedEventArgs
  {
    private EllipticItem _selectedItem;

    public EllipticSelectionChangedEventArgs(RoutedEvent routedEvent, EllipticItem selectedItem, object source)
      : base(routedEvent, source)
    {
      _selectedItem = selectedItem;
    }
    public EllipticItem SelectedItem
    {
      get { return _selectedItem; }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using RD.Controls;
using System.Xml;


namespace RD.EllipticSample
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>

  public partial class Window1 : Window
  {    
    public Window1()
    {
      InitializeComponent();
    }
    private void _gotoRDWebSite(object sender, RoutedEventArgs e)
    {
      Hyperlink __link = (Hyperlink)sender;
      Process.Start(__link.NavigateUri.AbsoluteUri);
    }
    private void _OnSelected(object sender, RoutedEventArgs e)
    {
      EllipticItem item = (EllipticItem)sender;
      StackPanel __panel = item.GetTemplateContainer<StackPanel>();

      Grid __grid = (Grid)__panel.FindName("PART_Abilities");
      if (__panel.Children.Contains(__grid))
      {
        __panel.Children.Remove(__grid);
      }

      _rdContent.Children.Add(__grid);
      __grid.Visibility = Visibility.Visible;
    }
    private void _OnUnselected(object sender, RoutedEventArgs e)
    {
      _rdContent.Children.RemoveAt(1);
    }
  }
}
#region Using directives
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Input;
#endregion

namespace RD.Controls.ExpanderSample
{
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }
    private void _NavigateTo(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
      e.Handled = true;
    }
    private void _ExpanderEnter(object sender, MouseEventArgs e)
    {
      MatrixTextBlock __header = (MatrixTextBlock)((RDExpander)e.Source).Header;
      __header.TypeWrite();
    }
  }
}
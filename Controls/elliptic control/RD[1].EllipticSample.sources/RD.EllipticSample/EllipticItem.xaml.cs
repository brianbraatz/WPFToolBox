using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WindowsApplication1
{
  public partial class EllipticItem : Control
  {
    private static readonly DependencyProperty TextProperty;
    private static readonly DependencyProperty DescriptionProperty;

    static EllipticItem()
    {
      TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EllipticItem), new FrameworkPropertyMetadata(string.Empty));
      DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(EllipticItem), new FrameworkPropertyMetadata(string.Empty));
    }
    public EllipticItem()
    {
    }
    public string Text
    {
      get { return (string)base.GetValue(TextProperty); }
      set { base.SetValue(TextProperty, value); }
    }
    public string Description
    {
      get { return (string)base.GetValue(DescriptionProperty); }
      set { base.SetValue(DescriptionProperty, value); }
    }
    internal FrameworkElement GetTemplateChild()
    {
      FrameworkElement __visualElement = base.GetTemplateChild(this.Template.VisualTree.Name) as FrameworkElement;
      if (__visualElement == null)
        throw new InvalidCastException();
      
      __visualElement.Name = this.Name;
      return __visualElement;
    }
  }
}

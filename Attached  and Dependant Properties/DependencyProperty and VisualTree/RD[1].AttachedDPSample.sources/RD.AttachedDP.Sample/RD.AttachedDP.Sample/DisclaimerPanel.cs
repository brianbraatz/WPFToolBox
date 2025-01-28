#region Using directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
#endregion

namespace RD.AttachedDP.Sample
{
  public partial class DisclaimerPanel : StackPanel
  {
    #region DependencyProperties
    private static readonly DependencyProperty DisclaimerProperty;
    #endregion

    #region Fields
    private const string RD_URI = "http://blog.rioterdecker.net";
    #endregion

    #region Ctors
    public DisclaimerPanel()
    {
    }
    static DisclaimerPanel()
    {
      DisclaimerProperty = DependencyProperty.RegisterAttached("Disclaimer", typeof(string), typeof(DisclaimerPanel), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(_DisclaimerChangedCallBack)), new ValidateValueCallback(_ValidateValueCallBack));
    }
    #endregion

    #region Methods
    private static void _DisclaimerChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if(d == null) return;

      FrameworkElement __element = (d as FrameworkElement);
      if (__element == null) return;

      Control __disclaimerControl = _BuildDisclaimerVisualTree(e.NewValue.ToString());
      (__element.Parent as DisclaimerPanel).Children.Add(__disclaimerControl);
    }
    private static bool _ValidateValueCallBack(object value)
    {
      return (value is string);
    }
    private static Control _BuildDisclaimerVisualTree(string value)
    {
      Control __control = new Control();

      FrameworkElementFactory __fePanelFactory = new FrameworkElementFactory(typeof(StackPanel));

      FrameworkElementFactory __feFactory = new FrameworkElementFactory(typeof(TextBlock));
      __feFactory = new FrameworkElementFactory(typeof(TextBlock));
      __feFactory.SetValue(TextBlock.TextProperty, value);
      __feFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      __feFactory.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red));
      __feFactory.SetValue(TextBlock.FontSizeProperty, 9.0d);
      __feFactory.SetValue(TextBlock.FontStyleProperty, FontStyles.Italic);

      __fePanelFactory.AppendChild(__feFactory);

      FrameworkElementFactory __feHLTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
      FrameworkElementFactory __feHlinkFactory = new FrameworkElementFactory(typeof(Hyperlink));
      FrameworkElementFactory __feSubTBFactory = new FrameworkElementFactory(typeof(TextBlock));
      __feHLTextBlockFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      __feHlinkFactory.SetValue(Hyperlink.NavigateUriProperty, new Uri(RD_URI));
      __feHlinkFactory.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(_NavigateToRD));
      __feSubTBFactory.SetValue(TextBlock.TextProperty, RD_URI);
      __feSubTBFactory.SetValue(TextBlock.FontSizeProperty, 9.0d);

      __feHlinkFactory.AppendChild(__feSubTBFactory);
      __feHLTextBlockFactory.AppendChild(__feHlinkFactory);
      __fePanelFactory.AppendChild(__feHLTextBlockFactory);

      ControlTemplate __template = new ControlTemplate(typeof(Control));
      __template.VisualTree = __fePanelFactory;

      __control.Template = __template;
      return __control;
    }
    private static void _NavigateToRD(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
    }
    public static string GetDisclaimer(UIElement element)
    {
      return (string)element.GetValue(DisclaimerProperty);
    }
    public static void SetDisclaimer(UIElement element, string value)
    {
      element.SetValue(DisclaimerProperty, value);
    }
    #endregion
  }
}

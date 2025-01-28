using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Data;

namespace RD.Controls
{
  public partial class EllipticItem : Control
  {
    #region Fields
    private EllipticItemDefinition _definition;
    #endregion

    #region Dependency Properties
    public static readonly DependencyProperty TextProperty;
    public static readonly DependencyProperty DescriptionProperty;
    public static readonly DependencyProperty IsSelectedProperty;
    private static readonly DependencyPropertyKey IsSelectedPropertyKey;
    #endregion

    #region Events
    public static readonly RoutedEvent SelectedEvent;
    public static readonly RoutedEvent UnselectedEvent;
    public event RoutedEventHandler Selected
    {
      add { base.AddHandler(SelectedEvent, value); }
      remove { base.RemoveHandler(SelectedEvent, value); }
    }
    public event RoutedEventHandler Unselected
    {
      add { base.AddHandler(UnselectedEvent, value); }
      remove { base.RemoveHandler(UnselectedEvent, value); }
    }
    #endregion

    #region Ctors
    static EllipticItem()
    {
      TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EllipticItem), new FrameworkPropertyMetadata(string.Empty));
      DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(EllipticItem), new FrameworkPropertyMetadata(string.Empty));
      IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(EllipticItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(EllipticItem._OnSelected)));
      IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
      SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EllipticItem));
      UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EllipticItem));
      EventManager.RegisterClassHandler(typeof(EllipticItem), SelectedEvent, new RoutedEventHandler(_OnSelectedEvent));
      EventManager.RegisterClassHandler(typeof(EllipticItem), UnselectedEvent, new RoutedEventHandler(_OnUnselected));
    }
    public EllipticItem()
    {
      this.InitializeComponent();
    }
    #endregion

    #region Properties
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
    public bool IsSelected
    {
      get { return (bool)base.GetValue(IsSelectedProperty); }
    }
    internal EllipticItemDefinition Definition
    {
      get { return _definition; }
      set { _definition = value; }
    }
    #endregion

    #region Methods
    protected virtual void OnSelected(RoutedEventArgs e)
    {
    }
    protected virtual void OnUnselected(RoutedEventArgs e)
    {
    }
    public T GetTemplateContainer<T>() where T:DependencyObject
    {
      return (T)base.GetTemplateChild("PART_Container");
    }
    private void _SetIsSelectedValue(bool selected)
    {
      if (selected)
        base.SetValue(IsSelectedPropertyKey, selected);
      else
        base.ClearValue(IsSelectedPropertyKey);
    }
    private static void _OnUnselected(object sender,  RoutedEventArgs e)
    {
      EllipticItem __item = (EllipticItem)sender;
      __item._SetIsSelectedValue(false);
      __item.OnUnselected(e);
    }
    private static void _OnSelected(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      EllipticItem __item = (EllipticItem)sender;
      __item._SetIsSelectedValue(true);
      RoutedEventArgs __args = new RoutedEventArgs(SelectedEvent);
      __item.OnSelected(__args);
    }
    private static void _OnSelectedEvent(object sender, RoutedEventArgs e)
    {
      EllipticItem __item = (EllipticItem)sender;
      __item._SetIsSelectedValue(true);
      RoutedEventArgs __args = new RoutedEventArgs(SelectedEvent);
      __item.OnSelected(__args);
    }
    #endregion

    internal class EllipticItemDefinition
    {
      #region Fields
      private int _index;
      private Point _position;
      private Storyboard _storyboard;
      #endregion

      #region Ctors
      public EllipticItemDefinition(int index, Point position, Storyboard storyboard, EllipticItem ellipticItem)
      {
        _index = index;
        _position = position;
        _storyboard = storyboard;

        Canvas.SetLeft(ellipticItem, position.X);
        Canvas.SetTop(ellipticItem, position.Y);
      }
      #endregion

      #region Properties
      internal int Index
      {
        get { return _index; }
      }
      internal Point Position
      {
        get { return _position; }
      }
      internal Storyboard AssociatedStoryboard
      {
        get { return _storyboard; }
      }
      #endregion
    }
  }
}

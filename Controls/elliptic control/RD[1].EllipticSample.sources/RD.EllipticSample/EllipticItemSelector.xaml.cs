using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Serialization;
using System.Windows.Media.Animation;

namespace WindowsApplication1
{
  public partial class EllipticItemSelector : Control, IAddChild
  {
    private Canvas _itemsContainer;
    private List<EllipticItem> _children;
    private Dictionary<Storyboard, AnimElement> _transforms;
    private FrameworkElement _selectedItem;
    private EllipticSelectionChangedEventArgs _args;
    private int _selectedIndex;

    private static readonly DependencyProperty EllipticWidthProperty;
    private static readonly DependencyProperty EllipticHeightProperty;
    private static readonly DependencyProperty AngleProperty;
    private static readonly DependencyProperty RotationSpeedProperty;
    private static readonly DependencyProperty ItemSizeProperty;
    private static readonly RoutedEvent SelectionChangedEvent;
    private static readonly RoutedEvent ClickEvent;

    public event EllipticSelectionChangedEventHandler SelectionChanged
    {
      add { base.AddHandler(SelectionChangedEvent, value); }
      remove { base.RemoveHandler(SelectionChangedEvent, value); }
    }
    public event RoutedEventHandler Click
    {
      add { base.AddHandler(ClickEvent, value); }
      remove { base.RemoveHandler(ClickEvent, value); }
    }

    static EllipticItemSelector()
    {
      SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(EllipticSelectionChangedEventHandler), typeof(EllipticItemSelector));
      ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EllipticItemSelector));
      EllipticWidthProperty = DependencyProperty.Register("EllipticWidth", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(100d));
      EllipticHeightProperty = DependencyProperty.Register("EllipticHeight", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(50d));
      AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(0d));
      RotationSpeedProperty = DependencyProperty.Register("RotationSpeed", typeof(Duration), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(new Duration(new TimeSpan(0, 0, 4))));
      ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(int), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(24));
    }
    public EllipticItemSelector()
    {
      this.InitializeComponent();
      _transforms = new Dictionary<Storyboard, AnimElement>();
      _children = new List<EllipticItem>();
    }
    public int ItemSize
    {
      get { return (int)base.GetValue(ItemSizeProperty); }
      set { base.SetValue(ItemSizeProperty, value); }
    }
    public double EllipticWidth
    {
      get { return (double)base.GetValue(EllipticWidthProperty); }
      set { base.SetValue(EllipticWidthProperty, value); }
    }
    public double EllipticHeight
    {
      get { return (double)base.GetValue(EllipticHeightProperty); }
      set { base.SetValue(EllipticHeightProperty, value); }
    }
    public double Angle
    {
      get { return (double)base.GetValue(AngleProperty); }
      set { base.SetValue(AngleProperty, value); }
    }
    public Duration RotationSpeed
    {
      get { return (Duration)base.GetValue(RotationSpeedProperty); }
      set { base.SetValue(RotationSpeedProperty, value); }
    }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public UIElementCollection Children
    {
      get
      {
        if (_itemsContainer != null)
          return _itemsContainer.Children;
        return null;
      }
    }
    protected override System.Collections.IEnumerator LogicalChildren
    {
      get 
      {
        return _children.GetEnumerator();
      }
    }
    private Storyboard _TemplatedStoryboard
    {
      get
      {
        return (Storyboard)this.Resources["SB_Template"];
      }
    }
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      _itemsContainer = (Canvas)(base.GetTemplateChild("_mainContainer") as Grid).Children[0];

      this._CreateLogicalChildren(this);
    }
    private void _InitAnimations()
    {
      int __i = -1;

      double __xBound = 0;
      double __yBound = 0;
      double __ratio = 1.0 / this.Children.Count;

      foreach (EllipticItem element in this.Children)
      {
        ++__i;
        Storyboard __storyboard = (this.Resources["SB_Template"] as Storyboard).Clone();
        DoubleAnimationUsingPath __anim1 = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[0];
        DoubleAnimationUsingPath __anim2 = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[1];
        __anim1.PathGeometry = new PathGeometry();
        __anim2.PathGeometry = new PathGeometry();

        this._InitGeometryTemplate();
        __anim1.PathGeometry.AddGeometry((EllipseGeometry)this.Resources["EllipseGeometry_Template"]);
        __anim2.PathGeometry.AddGeometry((EllipseGeometry)this.Resources["EllipseGeometry_Template"]);

        Viewbox __templatedChild = element.GetTemplateChild() as Viewbox;
        (__templatedChild.Child as FrameworkElement).Name = __templatedChild.Name;
        Storyboard.SetTargetName(__templatedChild.Child, (__templatedChild.Child as FrameworkElement).Name);

        Point __point1 = new Point();
        Point __point2 = new Point();
        ((__storyboard.Children[0] as ParallelTimeline).Children[0] as DoubleAnimationUsingPath).PathGeometry.GetPointAtFractionLength(__ratio * (__i + 1), out __point1, out __point2);

        __xBound = __point1.X;
        __yBound = __point1.Y;

        AnimElement __animElement = new AnimElement(element, __storyboard, Math.Round(__xBound), Math.Round(__yBound), __i, __ratio);
        if (Math.Round(element.Width) == (this.ItemSize * 2) && Math.Round(element.Height) == (this.ItemSize * 2))
        {
          _selectedIndex = __i;
          _selectedItem = element;
        }
        _transforms.Add(__storyboard, __animElement);

        __storyboard.CurrentTimeInvalidated += new EventHandler(_Storyboard_CurrentTimeInvalidated);

        int __seek = (int)(((double)(__animElement.Index + 1) / (double)this.Children.Count * 4) * 1000);
        __storyboard.Begin((element.GetTemplateChild() as Viewbox), true);
        __storyboard.Seek(element, new TimeSpan(0, 0, 0, 0, __seek), TimeSeekOrigin.BeginTime);
      }
    }
    private void _Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
    {
      if (_selectedItem == null)
      {
        AnimElement __element = _transforms[((sender as ClockGroup).Timeline as Storyboard)];
        if (_selectedIndex != __element.Index && Math.Round(__element.ObjectContext.Width) == (this.ItemSize * 2) && Math.Round(__element.ObjectContext.Height) == (this.ItemSize * 2))
        {
          _selectedIndex = __element.Index;

          Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
          while (__enumerator.MoveNext())
            __enumerator.Current.Storyboard.Pause(__enumerator.Current.ObjectContext);
        }
      }
      else if (Math.Round(_selectedItem.Width) == (this.ItemSize * 2) && Math.Round(_selectedItem.Height) == (this.ItemSize * 2))
      {
        Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
        while (__enumerator.MoveNext())
          __enumerator.Current.Storyboard.Pause(__enumerator.Current.ObjectContext);
        this.OnSelectionChanged(_args);
      }
    }
    protected virtual void OnLoaded()
    {
    }
    protected virtual void OnSelectionChanged(EllipticSelectionChangedEventArgs e)
    {
      base.RaiseEvent(e);
      e.Handled = true;
    }
    protected virtual void OnClick()
    {
      RoutedEventArgs __args = new RoutedEventArgs(ClickEvent, this);
      base.RaiseEvent(__args);
    }
    protected virtual void AddChild(object value)
    {
      if (value == null)
        throw new ArgumentNullException();

      EllipticItem __child = value as EllipticItem;
      if (__child == null)
        throw new ArgumentException();

      _children.Add(__child);
    }
    protected virtual void AddText(string text)
    {
      if (text != null)
      {
        for (int i = 0; i < text.Length; i++)
        {
          if (!char.IsWhiteSpace(text[i]))
            throw new ArgumentException();
        }
      }
    }
    private void _CreateLogicalChildren(FrameworkElement logicalParent)
    {
      foreach(EllipticItem item in _children)
      {
        //item.GetTemplateChild().Width = this.ItemSize;
        //item.GetTemplateChild().Height = this.ItemSize;
        //item.InternalImage.Name = item.Name;
        item.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_Children_Click);

        _itemsContainer.Children.Add(item);
        //_consolidateList.Add(item, item);
      }
    }
    private void _Loaded(object sender, RoutedEventArgs e)
    {
      this._InitDurations();
      this._InitAnimations();
      this.OnLoaded();
    }
    private void _InitGeometryTemplate()
    {
      EllipseGeometry __geometry = (EllipseGeometry)this.Resources["EllipseGeometry_Template"];
      __geometry.RadiusX = this.EllipticWidth;
      __geometry.RadiusY = this.EllipticHeight;
      (__geometry.Transform as RotateTransform).Angle = this.Angle;
    }
    private void _InitDurations()
    {
      Storyboard __storyboard = this._TemplatedStoryboard;
      foreach (ParallelTimeline ptl in __storyboard.Children)
      {
        foreach (Timeline timeline in ptl.Children)
        {
          timeline.Duration = this.RotationSpeed;
          if (timeline is DoubleAnimationUsingKeyFrames)
            this._InitKeyFrames(timeline as DoubleAnimationUsingKeyFrames);
        }
      }
    }
    private void _InitKeyFrames(DoubleAnimationUsingKeyFrames animation)
    {
      double __ratio = this.ItemSize / 4;
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize + __ratio, KeyTime.FromPercent(0.0833)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize + (__ratio * 2), KeyTime.FromPercent(0.1666)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize * 2, KeyTime.FromPercent(0.25)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize + (__ratio * 2), KeyTime.FromPercent(0.3332)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize + __ratio, KeyTime.FromPercent(0.4165)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize, KeyTime.FromPercent(0.5)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize - __ratio, KeyTime.FromPercent(0.5831)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize - (__ratio * 2), KeyTime.FromPercent(0.6664)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize / 2, KeyTime.FromPercent(0.75)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize - (__ratio * 2), KeyTime.FromPercent(0.833)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize - __ratio, KeyTime.FromPercent(0.9163)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(this.ItemSize, KeyTime.FromPercent(1.0)));
    }
    void IAddChild.AddChild(object value)
    {
      this.AddChild(value);
    }
    void IAddChild.AddText(string text)
    {
      this.AddText(text);
    }
    private void _Move()
    {
      Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
      while (__enumerator.MoveNext())
        __enumerator.Current.Storyboard.Resume(__enumerator.Current.ObjectContext);
    }
    private void _Children_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      _selectedItem = (FrameworkElement)sender;
      this._Move();
      _args = new EllipticSelectionChangedEventArgs(SelectionChangedEvent, (EllipticItem)sender, this);
      this.OnClick();
    }

    private class AnimElement
    {
      private FrameworkElement _objectContext;
      private Storyboard _storyboard;
      private double _xBound;
      private double _yBound;
      private int _index;
      private double _ratio;

      public AnimElement(FrameworkElement objectContext, Storyboard storyboard, double xBound, double yBound, int index, double ratio)
      {
        _objectContext = objectContext;
        _storyboard = storyboard;
        _xBound = xBound;
        _yBound = yBound;
        _index = index;
        _ratio = ratio;

        Canvas.SetLeft(_objectContext, _xBound);
        Canvas.SetTop(_objectContext, _yBound);
      }
      public FrameworkElement ObjectContext
      {
        get { return _objectContext; }
      }
      public Storyboard Storyboard
      {
        get { return _storyboard; }
      }
      public double XBound
      {
        get { return _xBound; }
      }
      public double YBound
      {
        get { return _yBound; }
      }
      public int Index
      {
        get { return _index; }
      }
    }
  }
}

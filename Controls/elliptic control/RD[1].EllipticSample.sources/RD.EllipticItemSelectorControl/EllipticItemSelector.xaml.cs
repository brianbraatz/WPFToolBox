using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace RD.Controls
{
  public partial class EllipticItemSelector : ContentControl
  {
    #region Fields
    private bool _isInitialized;
    private int _selectedIndex;
    private Viewbox _selectedItem;
    private Storyboard _storyboard;
    private EllipseGeometry _templatedGeometry;
    private Dictionary<Storyboard, Viewbox> _animationInfos;
    private EllipticSelectionChangedEventArgs _args;
    #endregion

    #region Dependency Properties
    public static readonly DependencyProperty EllipticWidthProperty;
    public static readonly DependencyProperty EllipticHeightProperty;
    public static readonly DependencyProperty AngleProperty;
    public static readonly DependencyProperty RotationSpeedProperty;
    public static readonly DependencyProperty ItemWidthProperty;
    public static readonly DependencyProperty ItemHeightProperty;
    #endregion

    #region Events
    public static readonly RoutedEvent SelectionChangedEvent;
    public static readonly RoutedEvent ClickEvent;
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
    #endregion

    #region Ctors
    static EllipticItemSelector()
    {
      SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(EllipticSelectionChangedEventHandler), typeof(EllipticItemSelector));
      ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EllipticItemSelector));
      EllipticWidthProperty = DependencyProperty.Register("EllipticWidth", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(100d));
      EllipticHeightProperty = DependencyProperty.Register("EllipticHeight", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(50d));
      AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(0d));
      RotationSpeedProperty = DependencyProperty.Register("RotationSpeed", typeof(Duration), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(new Duration(new TimeSpan(0, 0, 4))));
      ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(int), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(24));
      ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(int), typeof(EllipticItemSelector), new FrameworkPropertyMetadata(24));
    }
    public EllipticItemSelector()
    {
      this.InitializeComponent();
    }
    #endregion

    #region Properties
    public int ItemWidth
    {
      get { return (int)base.GetValue(ItemWidthProperty); }
      set { base.SetValue(ItemWidthProperty, value); }
    }
    public int ItemHeight
    {
      get { return (int)base.GetValue(ItemHeightProperty); }
      set { base.SetValue(ItemHeightProperty, value); }
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
    public int SelectedIndex
    {
      get { return _selectedIndex; }
    }
    public EllipticItem SelectedItem
    {
      get { return _selectedItem.Child as EllipticItem; }
    }
    private Storyboard TemplatedStoryboard
    {
      get
      {
        if (_storyboard == null)
        {
          _storyboard = (Storyboard)this.Resources["SB_Template"];
          this._InitializeStoryboard();
        }
        return _storyboard;
      }
    }
    private EllipseGeometry TemplatedGeometry
    {
      get
      {
        if (_templatedGeometry == null)
        {
          _templatedGeometry = (EllipseGeometry)this.Resources["EllipseGeometry_Template"];
          _templatedGeometry.RadiusX = this.EllipticWidth;
          _templatedGeometry.RadiusY = this.EllipticHeight;
          (_templatedGeometry.Transform as RotateTransform).Angle = this.Angle;
        }
        return _templatedGeometry;
      }
    }
    #endregion

    #region Methods
    public override void BeginInit()
    {
      base.BeginInit();
      _animationInfos = new Dictionary<Storyboard, Viewbox>(4);
      _isInitialized = false;
    }
    public override void EndInit()
    {
      _isInitialized = true;
      base.EndInit();
    }
    protected override void AddChild(object value)
    {
      if (this.Content == null)
        base.AddChild(value);
      else
      {
        if (_isInitialized) return;
        if (value is EllipticItem)
        {
          EllipticItem __ellipticItem = (EllipticItem)value;
          __ellipticItem.Width = this.ItemWidth;
          __ellipticItem.Height = this.ItemHeight;
          this._AttachToVisualTree(__ellipticItem);
        }
        else
          throw new InvalidOperationException("Incorrect type value");
      }
    }
    protected virtual void OnClick()
    {
      RoutedEventArgs __args = new RoutedEventArgs(ClickEvent, this);
      base.RaiseEvent(__args);
    }
    //protected virtual void OnSelectionChanged(EllipticSelectionChangedEventArgs e)
    //{
    //  base.RaiseEvent(e);
    //  e.Handled = true;
    //}
    private void __ellipticItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (_selectedItem != null)
      {
        EllipticItem __item = _selectedItem.Child as EllipticItem;
        RoutedEventArgs __unselectREA = new RoutedEventArgs(EllipticItem.UnselectedEvent, this);
        __item.RaiseEvent(__unselectREA);
      }

      _selectedItem = (Viewbox)sender;
      this._Move();
      _args = new EllipticSelectionChangedEventArgs(SelectionChangedEvent, _selectedItem.Child as EllipticItem, this);
      this.OnClick();

      RoutedEventArgs __selectREA = new RoutedEventArgs(EllipticItem.SelectedEvent, this);
      (_selectedItem.Child as EllipticItem).RaiseEvent(__selectREA);
    }
    private void _Move()
    {
      foreach (Viewbox viewbox in _itemsContainer.Children)
      {
        (viewbox.Child as EllipticItem).Definition.AssociatedStoryboard.Resume(viewbox);
      }
    }
    private void _AttachToVisualTree(EllipticItem ellipticItem)
    {
      Viewbox __viewbox = new Viewbox();
      __viewbox.Height = ellipticItem.Height;
      __viewbox.Width = ellipticItem.Width;

      __viewbox.Child = ellipticItem;
      __viewbox.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(__ellipticItem_MouseLeftButtonDown);

      (this.Content as Canvas).Children.Add(__viewbox);
    }
    private void _OnLoad(object sender, RoutedEventArgs e)
    {
      this._InitializePositions();
    }
    private void _InitializeStoryboard()
    {
      foreach (ParallelTimeline ptl in _storyboard.Children)
      {
        foreach (Timeline timeline in ptl.Children)
        {
          timeline.Duration = this.RotationSpeed;
          if (timeline is DoubleAnimationUsingKeyFrames)
            this._InitializeKeyFrames(timeline as DoubleAnimationUsingKeyFrames);
        }
      }
    }
    private void _InitializeKeyFrames(DoubleAnimationUsingKeyFrames animation)
    {
      double __max = (double)Math.Max(this.ItemWidth, this.ItemHeight);
      double __ratio = __max / 4;
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max + __ratio, KeyTime.FromPercent(0.0625)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max + (__ratio * 2), KeyTime.FromPercent(0.125)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max * 2, KeyTime.FromPercent(0.25)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max + (__ratio * 2), KeyTime.FromPercent(0.375)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max + __ratio, KeyTime.FromPercent(0.4375)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max, KeyTime.FromPercent(0.5)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max - __ratio, KeyTime.FromPercent(0.5625)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max - (__ratio * 2), KeyTime.FromPercent(0.625)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max / 2, KeyTime.FromPercent(0.75)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max - (__ratio * 2), KeyTime.FromPercent(0.875)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max - __ratio, KeyTime.FromPercent(0.9375)));
      animation.KeyFrames.Add(new LinearDoubleKeyFrame(__max, KeyTime.FromPercent(1.0)));
    }
    private void _InitializePositions()
    {
      int __i = -1;

      double __xBound = 0;
      double __yBound = 0;
      double __ratio = 1.0 / _itemsContainer.Children.Count;

      foreach (Viewbox viewbox in _itemsContainer.Children)
      {
        ++__i;
        Storyboard __storyboard = this._DefineStoryboard(viewbox);

        Point __point1 = new Point();
        Point __point2 = new Point();
        ((__storyboard.Children[0] as ParallelTimeline).Children[0] as DoubleAnimationUsingPath).PathGeometry.GetPointAtFractionLength(__ratio * (__i + 1), out __point1, out __point2);

        __xBound = __point1.X;
        __yBound = __point1.Y;

        EllipticItem __ellipticItem = (EllipticItem)viewbox.Child;
        __ellipticItem.Definition = new EllipticItem.EllipticItemDefinition(__i, new Point(Math.Round(__xBound), Math.Round(__yBound)), __storyboard, __ellipticItem);
        if (Math.Round(viewbox.Width) == (this.ItemWidth * 2) && Math.Round(viewbox.Height) == (this.ItemHeight * 2))
        {
          _selectedIndex = __i;
          _selectedItem = viewbox;
        }
        _animationInfos.Add(__storyboard, viewbox);

        __storyboard.CurrentTimeInvalidated += new EventHandler(_Storyboard_CurrentTimeInvalidated);

        int __seek = (int)(((double)(__ellipticItem.Definition.Index + 1) / (double)_itemsContainer.Children.Count * 4) * 1000);
        __storyboard.Begin(viewbox, true);
        __storyboard.Seek(viewbox, new TimeSpan(0, 0, 0, 0, __seek), TimeSeekOrigin.BeginTime);
      }
    }
    private void _Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
    {
      if (_selectedItem == null)
      {
        Viewbox __viewbox = _animationInfos[((sender as ClockGroup).Timeline as Storyboard)];
        EllipticItem __ellipticItem = __viewbox.Child as EllipticItem;
        if (_selectedIndex != __ellipticItem.Definition.Index && Math.Round(__viewbox.Width) >= ((this.ItemWidth * 2)-1) && Math.Round(__viewbox.Height) >= ((this.ItemHeight * 2)-1))
        {
          _selectedIndex = __ellipticItem.Definition.Index;

          foreach (Viewbox viewbox in _itemsContainer.Children)
          {
            (viewbox.Child as EllipticItem).Definition.AssociatedStoryboard.Pause(viewbox);
          }
        }
      }
      else if (Math.Round(_selectedItem.Width) >= ((this.ItemWidth * 2)-1) && Math.Round(_selectedItem.Height) >= ((this.ItemHeight * 2)-1))
      {
        foreach (Viewbox viewbox in _itemsContainer.Children)
        {
          (viewbox.Child as EllipticItem).Definition.AssociatedStoryboard.Pause(viewbox);
        }
        this.RaiseEvent(_args);
      }
    }
    private Storyboard _DefineStoryboard(Viewbox viewbox)
    {
      Storyboard __storyboard = this.TemplatedStoryboard.Clone();
      DoubleAnimationUsingPath __animX = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[0];
      DoubleAnimationUsingPath __animY = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[1];
      __animX.PathGeometry = new PathGeometry();
      __animY.PathGeometry = new PathGeometry();

      __animX.PathGeometry.AddGeometry(this.TemplatedGeometry);
      __animY.PathGeometry.AddGeometry(this.TemplatedGeometry);

      Storyboard.SetTargetName(viewbox, viewbox.Name);

      return __storyboard;
    }
    #endregion
  }
}

#region Using directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
#endregion

namespace RD.EllipseGeometrySample
{
  public partial class Window1 : Window
  {
    #region Fields
    private Dictionary<Storyboard, AnimElement> _transforms;
    private FrameworkElement _selectedItem;
    private int _selectedIndex;
    #endregion

    #region Ctors
    public Window1()
    {
      InitializeComponent();
      _transforms = new Dictionary<Storyboard, AnimElement>();
    }
    #endregion

    #region Methods
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      foreach (FrameworkElement child in _itemsContainer.Children)
      {
        child.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_Children_Click);
      }
      this._MeasureOverride();
    }
    private void _Children_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      _selectedItem = (FrameworkElement)sender;
      this._Move();
    }
    private void _MeasureOverride()
    {
      int __i = -1;

      double __xBound = 0;
      double __yBound = 0;
      double __ratio = 1.0 / _itemsContainer.Children.Count;

      foreach (FrameworkElement element in _itemsContainer.Children)
      {
        ++__i;
        Storyboard __storyboard = (this.Resources["Move_Storyboard"] as Storyboard).Clone();
        DoubleAnimationUsingPath __anim1 = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[0];
        DoubleAnimationUsingPath __anim2 = (DoubleAnimationUsingPath)(__storyboard.Children[0] as ParallelTimeline).Children[1];
        __anim1.PathGeometry = new PathGeometry();
        __anim2.PathGeometry = new PathGeometry();

        __anim1.PathGeometry.AddGeometry((EllipseGeometry)this.Resources["Ellipse_Template"]);
        __anim2.PathGeometry.AddGeometry((EllipseGeometry)this.Resources["Ellipse_Template"]);

        Storyboard.SetTargetName(element, element.Name);


        Point __point1 = new Point();
        Point __point2 = new Point();
        ((__storyboard.Children[0] as ParallelTimeline).Children[0] as DoubleAnimationUsingPath).PathGeometry.GetPointAtFractionLength(__ratio * (__i + 1), out __point1, out __point2);

        __xBound = __point1.X;
        __yBound = __point1.Y;

        AnimElement __animElement = new AnimElement(element, __storyboard, Math.Round(__xBound), Math.Round(__yBound), __i);
        if (Math.Round(element.Width) == 20 && Math.Round(element.Height) == 20)
        {
          _selectedIndex = __i;
          _selectedItem = element;
        }
        _transforms.Add(__storyboard, __animElement);

        __storyboard.CurrentTimeInvalidated += new EventHandler(_Storyboard_CurrentTimeInvalidated);

        int __seek = (int)(((double)(__animElement.Index + 1) / (double)_itemsContainer.Children.Count * 4) * 1000);
        __storyboard.Begin(element, true);
        __storyboard.Seek(element, new TimeSpan(0, 0, 0, 0, __seek), TimeSeekOrigin.BeginTime);
      }
    }
    private void _Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
    {
      if (_selectedItem == null)
      {
        AnimElement __element = _transforms[((sender as ClockGroup).Timeline as Storyboard)];
        if (_selectedIndex != __element.Index && Math.Round(__element.ObjectContext.Width) == 20 && Math.Round(__element.ObjectContext.Height) == 20)
        {
          _selectedIndex = __element.Index;
          Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
          while (__enumerator.MoveNext())
            __enumerator.Current.Storyboard.Pause(__enumerator.Current.ObjectContext);
        }
      }
      else if (Math.Round(_selectedItem.Width) == 20 && Math.Round(_selectedItem.Height) == 20)
      {
        Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
        while (__enumerator.MoveNext())
          __enumerator.Current.Storyboard.Pause(__enumerator.Current.ObjectContext);
      }
    }
    private void _Move()
    {
      Dictionary<Storyboard, AnimElement>.ValueCollection.Enumerator __enumerator = _transforms.Values.GetEnumerator();
      while (__enumerator.MoveNext())
        __enumerator.Current.Storyboard.Resume(__enumerator.Current.ObjectContext);
    }
    #endregion

    private class AnimElement
    {
      #region Fields
      private FrameworkElement _objectContext;
      private Storyboard _storyboard;
      private double _xBound;
      private double _yBound;
      private int _index;
      #endregion

      #region Ctors
      public AnimElement(FrameworkElement objectContext, Storyboard storyboard, double xBound, double yBound, int index)
      {
        _objectContext = objectContext;
        _storyboard = storyboard;
        _xBound = xBound;
        _yBound = yBound;
        _index = index;

        Canvas.SetLeft(_objectContext, _xBound);
        Canvas.SetTop(_objectContext, _yBound);
      }
      #endregion

      #region Properties
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
      #endregion
    }
  }
}
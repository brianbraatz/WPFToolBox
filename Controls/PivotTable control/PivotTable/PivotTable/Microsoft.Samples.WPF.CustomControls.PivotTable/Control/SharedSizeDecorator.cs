//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: SharedSizeDecorator control.
//  Creator: t-tomkm
//  Date Created: 08/17/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Decorator that allows to share size among many controls.
    /// </summary>
    public class SharedSizeDecorator : Decorator
    {
        #region Static members
        /// <summary>
        /// SharedWidthGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty SharedWidthGroupProperty;
        
        /// <summary>
        /// SharedHeightGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty SharedHeightGroupProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static SharedSizeDecorator()
        {
            SharedWidthGroupProperty = DependencyProperty.Register("SharedWidthGroup",
                typeof(string), typeof(SharedSizeDecorator), new PropertyMetadata(OnSharedWidthGroupChanged));
            SharedHeightGroupProperty = DependencyProperty.Register("SharedHeightGroup",
                typeof(string), typeof(SharedSizeDecorator), new PropertyMetadata(OnSharedHeightGroupChanged));
        }

        private static void OnSharedWidthGroupChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ((SharedSizeDecorator)target).OnSharedWidthGroupChanged(e);
        }

        private static void OnSharedHeightGroupChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ((SharedSizeDecorator)target).OnSharedHeightGroupChanged(e);
        }
        #endregion

        #region Fields
        private SharedSize _sharedWidth;
        private SharedSize _sharedHeight;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SharedSizeDecorator()
        {
            this.Unloaded += OnDecoratorUnloaded;
        }

        #region Properties

        /// <summary>
        /// Gets or sets name of the shared width group.
        /// </summary>
        public string SharedWidthGroup
        {
            get { return (string)GetValue(SharedWidthGroupProperty); }
            set { SetValue(SharedWidthGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets name of the shared height group.
        /// </summary>
        public string SharedHeightGroup
        {
            get { return (string)GetValue(SharedHeightGroupProperty); }
            set { SetValue(SharedHeightGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets the single child of the decorator.
        /// </summary>
        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                FrameworkElement oldElement = Child as FrameworkElement;
                if (oldElement != null)
                {
                    oldElement.Loaded -= OnChildLoaded;
                }

                base.Child = value;
                FrameworkElement newElement = Child as FrameworkElement;
                if (newElement != null)
                {
                    newElement.Loaded += OnChildLoaded;
                }
                else
                {
                    InvalidateDesiredSharedSize();
                }
            }
        }

        private void InvalidateDesiredSharedSize()
        {
            Size desiredChildSize;
            if (Child != null)
            {
                Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                desiredChildSize = Child.DesiredSize;
            }
            else
            {
                desiredChildSize = Size.Empty;
            }
            if (_sharedHeight != null)
            {
                _sharedHeight.UpdateDecoratorSize(this, desiredChildSize.Height);
            }
            if (_sharedWidth != null)
            {
                _sharedWidth.UpdateDecoratorSize(this, desiredChildSize.Width);
            }
        }

        private void OnChildLoaded(object sender, RoutedEventArgs e)
        {
            InvalidateDesiredSharedSize();
        }

        #endregion

        private void OnDecoratorUnloaded(object sender, RoutedEventArgs e)
        {
            if (_sharedHeight != null)
            {
                _sharedHeight.UnregisterDecorator(this);
            }
            if (_sharedWidth != null)
            {
                _sharedWidth.UnregisterDecorator(this);
            }
        }

        #region Handling SharedWidthGroupProperty and SharedHeightGroupProperty changes
        private void OnSharedWidthGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_sharedWidth != null)
            {
                _sharedWidth.UnregisterDecorator(this);
            }
            _sharedWidth = null;

            if (e.NewValue != null)
            {
                SharedSizeScope scope = FindSharedSizeScope();
                if (scope != null)
                {
                    _sharedWidth = scope.GetSharedWidthByName((string)e.NewValue);
                    _sharedWidth.RegisterDecorator(this);
                    InvalidateDesiredSharedSize();
                }
            }
        }

        private void OnSharedHeightGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_sharedHeight != null)
            {
                _sharedHeight.UnregisterDecorator(this);
            }
            _sharedHeight = null;

            if (e.NewValue != null)
            {
                SharedSizeScope scope = FindSharedSizeScope();
                if (scope != null)
                {
                    _sharedHeight = scope.GetSharedHeightByName((string)e.NewValue);
                    _sharedHeight.RegisterDecorator(this);
                    InvalidateDesiredSharedSize();
                }
            }
        }

        private SharedSizeScope FindSharedSizeScope()
        {
            FrameworkElement current = GetParent(this);
            while (current != null)
            {
                SharedSizeScope result = current.GetValue(SharedSizeScope.SharedSizeScopeProperty) as SharedSizeScope;
                if (result != null)
                {
                    return result;
                }
                current = GetParent(current);
            }
            return null;
        }

        private FrameworkElement GetParent(FrameworkElement element)
        {
            FrameworkElement result = element.Parent as FrameworkElement;
            if (result == null)
            {
                result = element.TemplatedParent as FrameworkElement;
            }
            if (result == null)
            {
                result = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return result;
        }
        #endregion

        #region Measuring and arranging
        /// <summary>
        /// Positions child element and determines a size of the decorator.
        /// </summary>
        /// <param name="finalSize">Final size available for the decorator.</param>
        /// <returns>Final size of the decorator.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return Child != null ? ArrangeOverrideWithChild(finalSize) : ArrangeOverrideWithoutChild(finalSize);
        }

        private Size ArrangeOverrideWithoutChild(Size finalSize)
        {
            double actualWidth = _sharedWidth != null ?
                Math.Min(_sharedWidth.MaximumSize, finalSize.Width) :
                finalSize.Width;

            double actualHeight = _sharedHeight != null ?
                Math.Min(_sharedHeight.MaximumSize, finalSize.Height) :
                finalSize.Height;

            return new Size(actualWidth, actualHeight);
        }

        private Size ArrangeOverrideWithChild(Size finalSize)
        {
            double actualWidth = _sharedWidth != null ?
                Math.Min(finalSize.Width, Math.Max(_sharedWidth.MaximumSize, Child.DesiredSize.Width)) :
                finalSize.Width;

            double actualHeight = _sharedHeight != null ?
                Math.Min(finalSize.Height, Math.Max(_sharedHeight.MaximumSize, Child.DesiredSize.Height)) :
                finalSize.Height;

            Size result = new Size(actualWidth, actualHeight);
            Child.Arrange(new Rect(new Point(0, 0), result));

            return result;
        }

        /// <summary>
        /// Measures the size in layout required for child element and determines a size
        /// for this element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to the child.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            if (Child == null)
            {
                return Size.Empty;
            }
            Child.Measure(availableSize);

            double desiredWidth = _sharedWidth != null ? Math.Max(_sharedWidth.MaximumSize, Child.DesiredSize.Width) : Child.DesiredSize.Width;
            double desiredHeight = _sharedHeight != null ? Math.Max(_sharedHeight.MaximumSize, Child.DesiredSize.Height) : Child.DesiredSize.Height;

            return new Size(desiredWidth, desiredHeight);
        }

        #endregion
    }
}

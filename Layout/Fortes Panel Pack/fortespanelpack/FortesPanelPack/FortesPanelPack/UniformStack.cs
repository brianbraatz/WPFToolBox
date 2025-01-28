using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace FortesPanelPack
{
    public class UniformStack : Panel
    {
        #region Dependency Properties
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformStack), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnOrientationPropertyChanged)));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /* Update our virtual properties */
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformStack current = d as UniformStack;
            if (d == null) return; // Shouldn't happen

            bool isVertical = ((Orientation)e.NewValue == Orientation.Vertical);

            current._vWidthProperty = (isVertical) ? WidthProperty : HeightProperty;
            current._vHeightProperty = (isVertical) ? HeightProperty : WidthProperty;
        }

        public static readonly DependencyProperty IsExpanderDetectionEnabledProperty = DependencyProperty.Register("IsExpanderDetectionEnabled", typeof(bool), typeof(UniformStack), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool IsExpanderDetectionEnabled
        {
            get { return (bool)GetValue(IsExpanderDetectionEnabledProperty); }
            set { SetValue(IsExpanderDetectionEnabledProperty, value); }
        }
        #endregion

        #region Layout Overrides
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxVWidth = 0;
            double totalVHeight = 0;
            Size vAvailableSize = VirtualSize(availableSize);
            _fixedVHeight = 0;

            _stretchyChildren.Clear();
            _fixedChildren.Clear();

            foreach (UIElement child in Children)
            {
                if (HasFixedHeight(child))
                {
                    _fixedChildren.Add(child);

                    // Measure the child with the incoming width and infinite height (since it will size to content)
                    child.Measure(VirtualSize(new Size(vAvailableSize.Width, Double.PositiveInfinity)));
                    maxVWidth = Math.Max(maxVWidth, VirtualSize(child.DesiredSize).Width);
                    _fixedVHeight += VirtualSize(child.DesiredSize).Height;
                }
                else
                {
                    _stretchyChildren.Add(child);

                    // Hold off on Measuring until we know how much space is left over
                }
            }

            totalVHeight = _fixedVHeight;

            if (_stretchyChildren.Count > 0)
            {
                // Measure the stretchy children now 
                vAvailableSize.Height = Math.Max(0, vAvailableSize.Height - _fixedVHeight); // Don't want to give negative numbers
                Size UniformMeasureSize = VirtualSize(new Size(vAvailableSize.Width, vAvailableSize.Height / _stretchyChildren.Count));
                foreach (UIElement child in _stretchyChildren)
                {
                    child.Measure(UniformMeasureSize);
                    totalVHeight += VirtualSize(child.DesiredSize).Height;
                    maxVWidth = Math.Max(maxVWidth, VirtualSize(child.DesiredSize).Width);
                }
            }

            return VirtualSize(new Size(maxVWidth, totalVHeight));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {   
            double stretchyVHeight = Math.Max(0, (VirtualSize(finalSize).Height - _fixedVHeight) / _stretchyChildren.Count); // No negative numbers
            Size stretchyArrangeVSize = new Size(VirtualSize(finalSize).Width, stretchyVHeight);

            double currVHeightOffset = 0;

            foreach (UIElement child in Children)
            {
                if (HasFixedHeight(child))
                {
                    double childVHeight = VirtualSize(child.DesiredSize).Height;
                    child.Arrange(VirtualRect(new Rect(new Point(0, currVHeightOffset), new Size(stretchyArrangeVSize.Width, childVHeight))));
                    currVHeightOffset += childVHeight;
                }
                else
                {
                    child.Arrange(VirtualRect(new Rect(new Point(0, currVHeightOffset), stretchyArrangeVSize)));
                    currVHeightOffset += stretchyVHeight;
                }
            }

            return finalSize;
        }
        #endregion

        #region Private Members
        /* In order to share code paths, we use the notion of "virtual" widths and heights */
        DependencyProperty _vWidthProperty = WidthProperty;
        DependencyProperty _vHeightProperty = HeightProperty;
        List<UIElement> _stretchyChildren = new List<UIElement>();
        List<UIElement> _fixedChildren = new List<UIElement>();
        double _fixedVHeight;
        #endregion

        #region Private Helpers
        /* Swaps width and height *only* if we're in virtual mode. Can be used to virtualize and de-virtualize a size */
        private Size VirtualSize(Size incoming)
        {
            if (this.Orientation == Orientation.Vertical)
                return incoming;

            return new Size(incoming.Height, incoming.Width);
        }

        private Rect VirtualRect(Rect incoming)
        {
            if (this.Orientation == Orientation.Vertical)
                return incoming;

            return new Rect(new Point(incoming.Top, incoming.Left), VirtualSize(incoming.Size));
        }

        /* Returns true if the element has a fixed height and should not be equi-sized */
        private bool HasFixedHeight(UIElement child)
        {
            if (!Double.IsNaN((double)child.GetValue(_vHeightProperty)))
                return true;
            else if (IsExpanderDetectionEnabled && !IsExpanded(child))
            {
                return true;
            }

            return false;
        }

        private bool IsExpanded(UIElement child)
        {
            Expander exp = FindExpander(child);

            if (exp == null)
            {
                return true; // No expander == expanded, I guess
            }

            return exp.IsExpanded;
        }

        private Expander FindExpander(UIElement child)
        {
            if (child is Expander) return (Expander) child;

            // There's definitely a better way to do this, it shall wait for the next version
            // (this is why detection is off by default)
            while (VisualTreeHelper.GetChildrenCount(child) == 1)
            {
                // We go more than one level because you can have ContentPresenters, etc in between
                child = VisualTreeHelper.GetChild(child, 0) as UIElement;
                if (child is Expander) return (Expander) child;
            }

            return null;
        }
        #endregion
    }
}

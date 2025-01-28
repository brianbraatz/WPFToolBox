using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;

namespace UpdatedTilePanel
{
    public class TilePanel : Panel
    {
        // Dependency property that controls the size of the child elements
        public static readonly DependencyProperty ChildSizeProperty
           = DependencyProperty.RegisterAttached("ChildSize", typeof(double), typeof(TilePanel),
              new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        // Accessor for the child size dependency property
        public double ChildSize
        {
            get { return (double)GetValue(ChildSizeProperty); }
            set { SetValue(ChildSizeProperty, value); }
        }

        #region TestCode
        protected override void OnInitialized(EventArgs e)
        {
            // If the user clicks on a child object, we want to remove it.
            foreach (FrameworkElement child in Children)
            {
                child.MouseLeftButtonDown += new MouseButtonEventHandler(ChildPressed);
            }
        }

        void ChildPressed(object sender, MouseButtonEventArgs eventArgs)
        {
            Children.Remove((UIElement)sender);
        }
        #endregion

        // Measures the children
        protected override Size MeasureOverride(Size availableSize)
        {
            int childrenPerRow;

            // Figure out how many children fit on each row
            if (availableSize.Width == Double.PositiveInfinity)
                childrenPerRow = this.Children.Count;
            else
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ChildSize));

            // Call measure on all children
            Size childSize = new Size(this.ChildSize, this.ChildSize);
            foreach (UIElement child in this.Children)
            {
                child.Measure(childSize);
            }

            // Calculate the width and height this results in
            double width = childrenPerRow * this.ChildSize;
            double height = this.ChildSize * (Math.Floor((double)this.Children.Count / childrenPerRow) + 1);
            return new Size(width, height);
        }

        // Arrange the children
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Calculate how many children fit on each row
            int childrenPerRow = Math.Max(1, (int)Math.Floor(finalSize.Width / this.ChildSize));

            for (int i = 0; i < this.Children.Count; i++)
            {
                FrameworkElement child = this.Children[i] as FrameworkElement;

                // Figure out where the child goes
                Point newOffset = CalcChildOffset(i, childrenPerRow, this.ChildSize);

                if (_oldChildrenPerRow != -1)
                {
                    // Figure out where the child is now

                    // If the child's index has changed, it's because something has been added
                    // or removed.  We need to use the child's old index to determine its old
                    // offset.  The index is stored (for simplicity) in the child's DataContext.
                    Point oldOffset = CalcChildOffset((int)child.DataContext, _oldChildrenPerRow, _oldChildSize);
                    if (child.RenderTransform != null)
                    {
                        // Note: Updated for Dec CTP - same code otherwise.
                        oldOffset = child.RenderTransform.Transform(oldOffset);
                    }

                    // Transform the child from the new location back to the old position
                    TranslateTransform childTransform = new TranslateTransform();
                    child.RenderTransform = childTransform;

                    // Decay the transformation with an animation
                    childTransform.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(oldOffset.X - newOffset.X));
                    childTransform.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(oldOffset.Y - newOffset.Y));
                }

                // Store the child's new index in the list. This'll normally be the same, unless an
                // item has been added or removed from the list.
                child.DataContext = i;
                
                // Position the child and set its size
                child.Arrange(new Rect(newOffset, new Size(this.ChildSize, this.ChildSize)));
            }

            _oldChildrenPerRow = childrenPerRow;
            _oldChildSize = this.ChildSize;

            return finalSize;
        }

        // Create an animation to decay from start to 0 over .5 seconds
        private static DoubleAnimation MakeAnimation(double start)
        {
            DoubleAnimation animation = new DoubleAnimation(start, 0d, new Duration(TimeSpan.FromMilliseconds(500)));
            animation.AccelerationRatio = 0.2;
            return animation;
        }

        // Given a child index, child size and children per row, figure out where the child goes
        private Point CalcChildOffset(int index, int childrenPerRow, double childSize)
        {
            int row = index / childrenPerRow;
            int column = index % childrenPerRow;
            return new Point(column * childSize, row * childSize);
        }

        private double _oldChildSize = 1d;
        private int _oldChildrenPerRow = -1;
    }
}

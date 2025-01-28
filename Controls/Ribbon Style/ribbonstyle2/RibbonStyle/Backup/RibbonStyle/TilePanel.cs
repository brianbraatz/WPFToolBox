using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace RibbonStyle
{
    public class TilePanel : Panel
    {
        private Int32 arrangedChildrenPerRow;
        public TilePanel() : base() { }

        public static readonly DependencyProperty ChildSizeProperty = DependencyProperty.RegisterAttached("ChildSize", typeof(Size), typeof(TilePanel),
            new FrameworkPropertyMetadata(new Size(1.0d, 1.0d), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public Size ChildSize
        {
            get { return (Size)GetValue(ChildSizeProperty); }
            set { SetValue(ChildSizeProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Int32 childrenPerRow = 0;
            // Figure out how many children fit on each row.
            if (availableSize.Width == Double.PositiveInfinity)
            {
                childrenPerRow = base.InternalChildren.Count;
            }
            else
            {
                childrenPerRow = Math.Max(1, (Int32)Math.Floor(availableSize.Width / this.ChildSize.Width));
            }

            foreach (UIElement child in base.InternalChildren)
            {
                child.Measure(this.ChildSize);
            }

            // Calculate the width and height the above operation results in
            Double width = this.ChildSize.Width * childrenPerRow;
            Double height = 0;
            if (base.InternalChildren.Count % childrenPerRow == 0)
            {
                height = this.ChildSize.Height * (Math.Floor((Double)(base.InternalChildren.Count / childrenPerRow)));
            }
            else
            {
                height = this.ChildSize.Height * (Math.Floor((Double)(base.InternalChildren.Count / childrenPerRow)) + 1);
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Calculate how many children fit on each row.
            Int32 childrenPerRow = Math.Max(1, (Int32)Math.Floor(finalSize.Width / this.ChildSize.Width));
            for (Int32 i = 0; i < base.InternalChildren.Count; i++)
            {
                FrameworkElement child = base.InternalChildren[i] as FrameworkElement;

                // Figure out where the child goes
                Point newOffset = CalculateChildOffset(i, childrenPerRow, this.ChildSize);

                // Position the child and set its size
                child.Arrange(new Rect(newOffset, this.ChildSize));
            }

            arrangedChildrenPerRow = childrenPerRow;
            return finalSize;
        }

        // Given a child index, child size and children per row, figure out where the child goes.
        private Point CalculateChildOffset(Int32 childIndex, Int32 childPerRow, Size childSize)
        {
            Int32 row = childIndex / childPerRow;
            Int32 column = childIndex % childPerRow;

            return new Point(column * childSize.Width, row * childSize.Height);
        }

        protected Int32 GetChildIndexFromPosition(Point position)
        {
            Int32 row = (Int32)(position.Y / this.ChildSize.Width);
            Int32 column = Math.Min(arrangedChildrenPerRow, (Int32)(position.X / this.ChildSize.Height));
            Int32 index = row * arrangedChildrenPerRow + column;
            return Math.Min(index, base.InternalChildren.Count);
        }
    }
}


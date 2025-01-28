using System;
using System.Windows;
using System.Windows.Controls;

namespace ItemsControlExample
{
    public class RadialPanel : Panel
    {
        /// <summary>
        /// Angle is an attached property that tells the RadialPanel where to draw the child. By default
        /// the angle used is 0. Changing this property invalidates the arrange of the Panel.
        /// </summary>
        public static readonly DependencyProperty AngleProperty = DependencyProperty.RegisterAttached(
            "Angle",
            typeof(double),
            typeof(RadialPanel),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static double GetAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(AngleProperty);
        }

        public static void SetAngle(DependencyObject obj, double value)
        {
            obj.SetValue(AngleProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // find out the radius of the children (it is 1/3rd of the minor axis of the RadialPanel)
            double childRadius = Math.Min(availableSize.Width, availableSize.Height) / 3.0;

            // measure each of the children with this size
            foreach (UIElement element in this.InternalChildren)
            {
                element.Measure(new Size(childRadius, childRadius));
            }

            // tell our layout parent that we used all of the space up
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // figure out the size for the children
            double childRadius = Math.Min(finalSize.Width, finalSize.Height) / 3.0;

            foreach (UIElement element in this.InternalChildren)
            {
                double angle = GetAngle(element);

                // figure out where the child goes in cartesian space. We invert the
                // Y value because screen space and normal mathematical cartesian space
                // are reversed in the vertical.
                Point childPoint = new Point(
                    Math.Cos(angle * Math.PI / 180.0) * childRadius,
                    -Math.Sin(angle * Math.PI / 180.0) * childRadius);

                // offset to center of the panel
                childPoint.X += finalSize.Width / 2;
                childPoint.Y += finalSize.Height / 2;

                // arrange so that the center of the child is at the point we calculated,
                // and the child is of the calculated radius.
                element.Arrange(new Rect(
                    childPoint.X - childRadius / 2.0,
                    childPoint.Y - childRadius / 2.0,
                    childRadius,
                    childRadius));
            }

            // we used up all of the space
            return finalSize;
        }
    }
}
using System;
using System.Windows;

namespace ItemsControlExample
{
    public class ItemsRadialPanel : RadialPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double angle = 0.0;
            double spacing = (360.0 / this.InternalChildren.Count);

            // set the angle of the children based on where they are in the list
            foreach (UIElement element in this.InternalChildren)
            {
                SetAngle(element, angle);
                angle += spacing;
            }

            return base.MeasureOverride(availableSize);
        }
    }
}

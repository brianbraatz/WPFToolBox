using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AltLayoutPanelTest
{
    class OverlapStackPanel : StackPanel
    {

        //
        // Summary:
        //     Invoked when the System.Windows.Media.VisualCollection of a visual object
        //     is modified.
        //
        // Parameters:
        //   visualAdded:
        //     The System.Windows.Media.Visual that was added to the collection.
        //
        //   visualRemoved:
        //     The System.Windows.Media.Visual that was removed from the collection.
        protected  override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded,visualRemoved);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {

            UIElementCollection children = this.InternalChildren;

            int childrenResolved = 0;

            foreach (UIElement child in children)
            {

                Rect targetRect = new Rect(40 * childrenResolved, 30, child.DesiredSize.Width, child.DesiredSize.Height);

                child.RenderTransform = new RotateTransform(-20);

                child.Arrange(targetRect);

                childrenResolved++;

            }

            return arrangeSize;

        }

    }

}

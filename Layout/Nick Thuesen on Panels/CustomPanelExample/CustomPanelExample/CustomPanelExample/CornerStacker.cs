using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomPanelExample
{
    /// <summary>
    /// A custom control.
    ///
    /// To use this custom control from a xaml file, do 1a or 1b, and then 2.
    ///
    /// Step 1a)
    /// To use this custom control from a xaml file in this project, add
    /// this mapping PI before the root element in the markup file:
    /// <?Mapping XmlNamespace="controls" ClrNamespace="CustomPanelExample" ?>
    ///
    /// If you want to do this, in this build, you'll also need to set <UICulture>en-US</UICulture> in your project file. (or some other value).
    /// This will start building your application with a satellite assembly, which enables this feature.
    /// 
    /// Step 1b)
    /// To use this custom control from a xaml file in another project, add
    /// this mapping PI before the root element in the markup file:
    /// <?Mapping XmlNamespace="controls"
    ///           ClrNamespace="CustomPanelExample"
    ///           Assembly="CustomPanelExample"?>
    /// From the project where the xaml file lives, you'll need to do add
    /// a project reference to this project:
    ///     [Add Reference/Projects/ThisProject]
    /// Note: You may need to use Rebuild to avoid compilation errors.

    /// Step 2)
    /// In the root tag of the .xaml, define a new namespace prefix.  The uri
    /// inside must match the value for the XmlNamespace attribute in the mapping PI:
    ///     <Window xmlns:cc="controls" ... >
    /// Then go ahead and use your control in the .xaml somewhere.
    ///     <cc:TwistPanel />
    /// Note1: Intellisense in the xml editor won't currently work on your custom control.
    /// </summary>




    /// <summary>
    /// Custom Panel that stacks it's children in the top left hand corner.
    /// -Nick Thuesen
    /// </summary>
    public class CornerStacker : Panel
    {

        //Create a DP so user can set size of dimensions controls are drawn with.
        public static readonly DependencyProperty DesiredElementDimensionProperty = DependencyProperty.Register("DesiredElementDimension", typeof(double), typeof(CornerStacker), new FrameworkPropertyMetadata(75.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        //The width and height controls are drawn at.
        public double DesiredElementDimension
        {
            get { return (double)GetValue(DesiredElementDimensionProperty); }
            set
            {
                //Make sure value is a double we can use else use the default.
                if (Double.IsNaN(value) || Double.IsInfinity(value))
                {
                    value = _defaultDimension;
                }
                SetValue(DesiredElementDimensionProperty, value);
            }
        }


        /// <summary>
        /// Called when Panel needs to arrange it's children.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //Tracks the number of elements allowed in each diagonal row.
            int maxNumElementsInRow = 1;
            //Tracks the current position in each diagonal row.
            int currElementPositionInRow = 0;

            //Cycle through each child of the panel.
            foreach (UIElement currChild in Children)
            {
                //Create a rect for the child and set it's size.
                Rect childRect = new Rect(new Size(DesiredElementDimension, DesiredElementDimension));
                //Calculate current child's position.
                childRect.X = (currElementPositionInRow * DesiredElementDimension);
                childRect.Y = ((maxNumElementsInRow-1) * DesiredElementDimension) - (currElementPositionInRow * DesiredElementDimension);

                //Arrange the child.
                currChild.Arrange(childRect);

                //Check to make sure current size isn't smaller than children's sum.
                if ((childRect.X+DesiredElementDimension) > _currMaxWidth)
                    _currMaxWidth = childRect.X + DesiredElementDimension;
                if ((childRect.Y+DesiredElementDimension) > _currMaxHeight)
                    _currMaxHeight = childRect.Y + DesiredElementDimension;
                
                //Increment to next position.
                currElementPositionInRow++;

                //Check to make sure current position isn't out of bounds for the current diagonal row.
                if (currElementPositionInRow >= maxNumElementsInRow)
                {
                    maxNumElementsInRow++;
                    currElementPositionInRow = 0;
                }
            }

            //Return the size.
            return new Size(_currMaxWidth, _currMaxHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //Just return the size we've been tracking.
            return new Size(_currMaxWidth,_currMaxHeight);
        }

        private double _defaultDimension = 75;  //Default is 75
        private double _currMaxWidth = 0;
        private double _currMaxHeight = 0;
    }
}

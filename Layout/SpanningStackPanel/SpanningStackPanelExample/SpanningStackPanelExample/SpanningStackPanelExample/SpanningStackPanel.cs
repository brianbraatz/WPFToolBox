using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace SpanningStackPanelExample
{
    public class SpanningStackPanel : Grid
    {


        // ------------------------------------------------------------
        //
        // Constructors
        //
        // ------------------------------------------------------------
        #region Constructors

        public SpanningStackPanel()
            : base()
        {
            //Default Orientation Should Be Vertical
            this.Orientation = Orientation.Vertical;

            //Handle the Panel's Loaded Event
            this.Loaded += new RoutedEventHandler(SpanningStackPanel_Loaded);
        }


        #endregion Constructors


        // ------------------------------------------------------------
        //
        // Dependency Properties
        //
        // ------------------------------------------------------------
        #region Dependency Properties

        public static DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SpanningStackPanel), new PropertyMetadata(new PropertyChangedCallback(OnOrientationPropertyChanged)));
        /// <summary>
        /// Orientation of items inside Panel, Vertical or Horizontal.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        #endregion Dependency Properties


        // ------------------------------------------------------------
        //
        // Protected Methods
        //
        // ------------------------------------------------------------
        #region Protected Methods


        /// <summary>
        /// When the visual children change we want to make
        /// sure that our "custom" layout occurs.
        /// </summary>
        /// <param name="visualAdded"></param>
        /// <param name="visualRemoved"></param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            LayoutChildren();
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        #endregion Protected Methods



        // ------------------------------------------------------------
        //
        // Private Methods
        //
        // ------------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Event Handler for the SpanningStackPanel Loaded Event
        /// We want to call LayoutChildren after a load completes even though
        /// it should have been called during the controls initialization phase.
        /// It isn't until the panel is completely loaded that we can know for 
        /// sure the desired layout of the children.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpanningStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutChildren();
        }

        /// <summary>
        /// Event Handler for Orientation Property Change Event
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private static void OnOrientationPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is SpanningStackPanel)
            {
                ((SpanningStackPanel)target).LayoutChildren();
            }
        }

        /// <summary>
        /// The "Main" layout function to be called anytime the children
        /// needs to be placed.
        /// </summary>
        private void LayoutChildren()
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                LayoutChildrenLeftToRight();
            }
            else
            {
                LayoutChildrenRightToLeft();
            }
        }


        /// <summary>
        /// Layout Method for when the panel's FlowDirection is 
        /// RightToLeft
        /// </summary>
        private void LayoutChildrenRightToLeft()
        {
            //Clear the current row and column definitions.
            this.RowDefinitions.Clear();
            this.ColumnDefinitions.Clear();

            int indexTracker = 0;

            //Cycle through the children and generate the proper row or column.
            for(int i = this.Children.Count - 1; i >= 0; i--)
            {
                UIElement child = this.Children[i];

                //Determine if rows or columns should be used.
                if (this.Orientation == Orientation.Vertical)
                {
                    //Orientation is Vertical so use rows.
                    RowDefinition newRowDefinition = CreateRowDefinitionForChild(child);

                    //Add the Row Definition and set the row property on child.
                    this.RowDefinitions.Add(newRowDefinition);
                    SetRow(child, indexTracker);

                }
                else
                {
                    //Orientation is Horizontal so use columns.
                    ColumnDefinition newColumnDefinition = CreateColumnDefinitionForChild(child);
                    //Add the ColumnDefintion and set the row property on the child.
                    this.ColumnDefinitions.Add(newColumnDefinition);
                    SetColumn(child, indexTracker);
                }

                indexTracker++;
            }
        }

        /// <summary>
        /// Layout Method for when the panel's FlowDirection is
        /// LeftToRight
        /// </summary>
        private void LayoutChildrenLeftToRight()
        {
            //Clear the current row and column definitions.
            this.RowDefinitions.Clear();
            this.ColumnDefinitions.Clear();

            int indexTracker = 0;

            //Cycle through the children and generate the proper row or column.
            foreach (UIElement child in this.Children)
            {
                //Determine if rows or columns should be used.
                if (this.Orientation == Orientation.Vertical)
                {
                    //Orientation is Vertical so use rows.
                    RowDefinition newRowDefinition = CreateRowDefinitionForChild(child);

                    //Add the Row Definition and set the row property on child.
                    this.RowDefinitions.Add(newRowDefinition);
                    SetRow(child, indexTracker);

                }
                else
                {
                    //Orientation is Horizontal so use columns.
                    ColumnDefinition newColumnDefinition = CreateColumnDefinitionForChild(child);
                    //Add the ColumnDefintion and set the row property on the child.
                    this.ColumnDefinitions.Add(newColumnDefinition);
                    SetColumn(child, indexTracker);
                }

                indexTracker++;
            }
        }


        /// <summary>
        /// Takes in a UIElement and creates a ColumnDefinition for the passed
        /// child to reside in.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private ColumnDefinition CreateColumnDefinitionForChild(UIElement child)
        {
            //If the child is a FrameworkElement we can sneak a peak at it's height value.  Else we'll ask it its desired size.
            Size sizeOfChild;


            //Come back and rethink how this is done.
            if (child is FrameworkElement)
            {
                FrameworkElement feChild = child as FrameworkElement;
                sizeOfChild = new Size(feChild.Width, feChild.Height);
            }
            else
            {
                //Tell the child he has all the room in the world.
                child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                //Ask the child how much room he'd like to use.
                sizeOfChild = child.DesiredSize;
            }

            //Create the RowDefinition
            ColumnDefinition newColumnDefinition = new ColumnDefinition();
            //Determine Height of the Row
            if (Double.IsNaN(sizeOfChild.Width) || Double.IsPositiveInfinity(sizeOfChild.Width))
            {
                newColumnDefinition.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                newColumnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            }

            return newColumnDefinition;
        }

        /// <summary>
        /// Takes in a UIElement and creates a RowDefinition for the passed child
        /// to reside in.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private RowDefinition CreateRowDefinitionForChild(UIElement child)
        {
            //If the child is a FrameworkElement we can sneak a peak at it's height value.  Else we'll ask it its desired size.
            Size sizeOfChild;


            //Come back and rethink how this is done.
            if (child is FrameworkElement)
            {
                FrameworkElement feChild = child as FrameworkElement;
                sizeOfChild = new Size(feChild.Width, feChild.Height);
            }
            else
            {
                //Tell the child he has all the room in the world.
                child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                //Ask the child how much room he'd like to use.
                sizeOfChild = child.DesiredSize;
            }

            //Create the RowDefinition
            RowDefinition newRowDefinition = new RowDefinition();
            //Determine Height of the Row
            if (Double.IsNaN(sizeOfChild.Height) || Double.IsPositiveInfinity(sizeOfChild.Height))
            {
                newRowDefinition.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                newRowDefinition.Height = new GridLength(1, GridUnitType.Auto);
            }

            return newRowDefinition;
        }

        #endregion Private Methods


    }
}

using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.Samples
{
    public class HyperGrid : Grid
    {
        /*** Private properties ***/

        // Used to determine how many columns should be displayed at the regular width.
        // Valid ranges for Distribution are 0.05 to 1.0.  
        private static readonly DependencyProperty DistributionProperty = DependencyProperty.Register("Distribution", typeof(double), typeof(HyperGrid), new PropertyMetadata(0.25, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));

        // Used to determine the height of the child elements in individual columns.
        // Valid ranges for Zoom are 0.1 to 1.0.  
        private static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(HyperGrid), new PropertyMetadata(1.0, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));

        // Used to make the largest column appear at the position of the mouse cursor, or in the middle of HyperGrid.
        // Valid ranges for Center are 0.0 to 1.0.  
        private static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(double), typeof(HyperGrid), new PropertyMetadata(0.5, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));

        // Used to determine whether to fade out columns on each side of the mouse cursor position.
        private static readonly DependencyProperty UseTransparencyProperty = DependencyProperty.Register("UseTransparency", typeof(bool), typeof(HyperGrid), new PropertyMetadata(true, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));

        // Used to determine whether to reset properties when the mouse is not over HyperGrid.
        private static readonly DependencyProperty MouseLeaveResetCenterProperty = DependencyProperty.Register("MouseLeaveResetCenter", typeof(bool), typeof(HyperGrid), new PropertyMetadata(false, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));
        private static readonly DependencyProperty MouseLeaveResetDistributionProperty = DependencyProperty.Register("MouseLeaveResetDistribution", typeof(bool), typeof(HyperGrid), new PropertyMetadata(false, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));
        private static readonly DependencyProperty MouseLeaveResetZoomProperty = DependencyProperty.Register("MouseLeaveResetZoom", typeof(bool), typeof(HyperGrid), new PropertyMetadata(false, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));
        private static readonly DependencyProperty MouseLeaveResetTransparencyProperty = DependencyProperty.Register("MouseLeaveResetTransparency", typeof(bool), typeof(HyperGrid), new PropertyMetadata(false, new PropertyChangedCallback(HyperGrid.HandlePropertyChange)));

        /*** Accessors ***/

        public double Distribution
        {
            get { return (double)this.GetValue(HyperGrid.DistributionProperty); }
            set { this.SetValue(HyperGrid.DistributionProperty, value); }
        }

        public double Zoom
        {
            get { return (double)this.GetValue(HyperGrid.ZoomProperty); }
            set { this.SetValue(HyperGrid.ZoomProperty, value); }
        }

        public double Center
        {
            get { return (double)this.GetValue(HyperGrid.CenterProperty); }
            set { this.SetValue(HyperGrid.CenterProperty, value); }
        }

        public bool UseTransparency
        {
            get { return (bool)this.GetValue(HyperGrid.UseTransparencyProperty); }
            set { this.SetValue(HyperGrid.UseTransparencyProperty, value); }
        }

        public bool MouseLeaveResetCenter
        {
            get { return (bool)this.GetValue(HyperGrid.MouseLeaveResetCenterProperty); }
            set { this.SetValue(HyperGrid.MouseLeaveResetCenterProperty, value); }
        }

        public bool MouseLeaveResetDistribution
        {
            get { return (bool)this.GetValue(HyperGrid.MouseLeaveResetDistributionProperty); }
            set { this.SetValue(HyperGrid.MouseLeaveResetDistributionProperty, value); }
        }

        public bool MouseLeaveResetZoom
        {
            get { return (bool)this.GetValue(HyperGrid.MouseLeaveResetZoomProperty); }
            set { this.SetValue(HyperGrid.MouseLeaveResetZoomProperty, value); }
        }

        public bool MouseLeaveResetTransparency
        {
            get { return (bool)this.GetValue(HyperGrid.MouseLeaveResetTransparencyProperty); }
            set { this.SetValue(HyperGrid.MouseLeaveResetTransparencyProperty, value); }
        }

        /*** Event handlers ***/

        private static void HandlePropertyChange(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((HyperGrid)source).Update(e);
        }

        protected void Update(DependencyPropertyChangedEventArgs e)
        {
            // For each column, do additional work depending on which properties changed.
            for (int i = 0; i < this.ColumnDefinitions.Count; ++i)
            {
                // Calculate how far this column is from the mouse cursor position.
                double distanceFromDOI =
                    HyperGrid.ColumnDistanceFromDOI(i, this.ColumnDefinitions.Count, this.Center);

                // Plug the distance from zero (the DOI itself is at zero) into the hyperbolic function.
                double hyperbolic =
                    HyperGrid.ValueAtX(distanceFromDOI, this.Zoom, this.Distribution);

                // Get a reference to any child elements if there are any.
                if (i < this.Children.Count)
                {
                    FrameworkElement childElement = this.Children[i] as FrameworkElement;
                    if (childElement != null)
                    {
                        // If the mouse has moved, or UseTransparency has changed, update transparency of column.
                        if (e.Property == HyperGrid.CenterProperty
                            || e.Property == HyperGrid.UseTransparencyProperty)
                        {
                            if (this.UseTransparency)
                            {
                                // Set the transparency based on the hyperbolic function.
                                childElement.Opacity = ValueAtX(
                                        distanceFromDOI, 1, this.Distribution);
                            }
                            else
                            {
                                childElement.Opacity = 1;
                            }
                        }

                        // If the mouse has moved, or Zoom has changed, update the height of child element.
                        if (e.Property == HyperGrid.CenterProperty
                            || e.Property == HyperGrid.ZoomProperty)
                        {
                            // The hyperbolic function returns a value in the range (0,1) which needs to be  
                            // scaled to the actual height of the HyperBar Grid.
                            childElement.Height = hyperbolic * this.ActualHeight;
                        }
                    }
                }

                // If the mouse has moved, or Distribution has changed, update the width of the column.
                if (e.Property == HyperGrid.CenterProperty
                    || e.Property == HyperGrid.DistributionProperty)
                {
                    // Use Grid element 'star-sizing' to set the Width of each column as a weighted  
                    // proportion of available space.
                    // The hyperbolic function returns a value in the range (0,1) which needs to be  
                    // scaled to the actual width of the HyperBar Grid.
                    this.ColumnDefinitions[i].Width =
                        new GridLength(hyperbolic * this.ActualWidth, GridUnitType.Star);
                }
            }
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            // Call the base method.
            base.OnMouseMove(e);

            // Do additional work: Update Center based on the mouse position.
            double mousePosX = Mouse.GetPosition(this).X;
            this.Center = mousePosX / this.ActualWidth;

        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // Call the base method.
            base.OnMouseLeave(e);

            // Do additional work: Reset the HyperGrid.
            if (this.MouseLeaveResetCenter)
                this.Center = 0.5;
            if (this.MouseLeaveResetDistribution)
                this.Distribution = 1.0;
            if (this.MouseLeaveResetZoom)
                this.Zoom = 1.0;
            if (this.MouseLeaveResetTransparency)
                this.UseTransparency = false;
        }

        /*** Helper methods ***/

        private static double ColumnDistanceFromDOI(int columnIndex, int columnCount, double center)
        {
            // This method returns the distance a column is from the degree of interest (DOI),
            // as a proportion of the width of the HyperGrid control. 
            // The position of the mouse pointer within the HyperGrid indicates the DOI.
            // If the mouse is not currently within the HyperGrid then 0.5 is the default value
            // (halfway across the HyperGrid).
            double positionProportion = (double)(columnIndex + 0.5) / columnCount;
            return (center - positionProportion);
        }

        private static double ValueAtX(double x, double zoom, double variance)
        {
            // This method name takes a slight liberty with the word 'hyperbolic' 
            // as the equation used is in fact Gaussian in nature.
            double exponent = (x * x) / (2 * variance * variance);
            return (zoom * Math.Exp(-exponent));
        }
    }
}

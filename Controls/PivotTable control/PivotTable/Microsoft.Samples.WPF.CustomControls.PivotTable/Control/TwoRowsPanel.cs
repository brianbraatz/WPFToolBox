//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: TwoRowsPanel class.
//  Creator: t-tomkm
//  Date Created: 08/24/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Panel that works like a grid with two rows, where the second row controls width of
    /// the whole panel.
    /// </summary>
    public class TwoRowsPanel : Panel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TwoRowsPanel()
        { }

        private UIElement FirstChild
        {
            get { return Children.Count > 0 ? Children[0] : null; }
        }

        private UIElement SecondChild
        {
            get { return Children.Count > 1 ? Children[1] : null; }
        }

        /// <summary>
        /// Positions child elements and determines a size of the panel.
        /// </summary>
        /// <param name="finalSize">Final available size for the panel.</param>
        /// <returns>Final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count < 2)
            {
                return base.ArrangeOverride(finalSize);
            }
            Rect firstChildRectangle = new Rect(0, 0, finalSize.Width, FirstChild.DesiredSize.Height);
            Rect secondChildRectangle = new Rect(0, FirstChild.DesiredSize.Height, finalSize.Width, 
                finalSize.Height - FirstChild.DesiredSize.Height);
            FirstChild.Arrange(firstChildRectangle);
            SecondChild.Arrange(secondChildRectangle);
            return finalSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size
        /// for this element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to children.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count < 2)
            {
                return base.MeasureOverride(availableSize);
            }
            Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            SecondChild.Measure(infiniteSize);
            double width = SecondChild.DesiredSize.Width;
            FirstChild.Measure(new Size(width, double.PositiveInfinity));
            double height = FirstChild.DesiredSize.Height + SecondChild.DesiredSize.Height;
            return new Size(width, height);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeveloperTools.Controls.Graph
{
    /// <summary>
    /// Graph is a simple control used to represent some data on a graph which use bars.
    /// </summary>
    public class Graph : ItemsControl
    {
        #region Constructors

        static Graph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Graph), new FrameworkPropertyMetadata((typeof(Graph))));
        }

        public Graph()
            : base()
        {
        }

        #endregion

        #region Properties

        public Orientation GraphOrientation
        {
            get { return (Orientation)GetValue(GraphOrientationProperty); }
            set { SetValue(GraphOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphOrientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphOrientationProperty =
            DependencyProperty.Register("GraphOrientation", typeof(Orientation), typeof(Graph), new FrameworkPropertyMetadata(Orientation.Horizontal));

        public string Legend
        {
            get { return (string)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Legend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register("Legend", typeof(string), typeof(Graph), new FrameworkPropertyMetadata(string.Empty));

        #endregion
    }
}

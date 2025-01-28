//---------------------------------------
// Curler.cs (c) 2007 by Charles Petzold
//---------------------------------------
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Petzold.CurlAround
{
    [RuntimeNameProperty("Name")]
    public class Curler : Animatable
    {
        // Field used in AngleChangedCallback
        RotateTransform rotate = new RotateTransform();

        // Name dependency property and property
        public static DependencyProperty NameProperty =
            DependencyProperty.Register("Name",
            typeof(string),
            typeof(Curler),
            new PropertyMetadata(null));

        public string Name
        {
            set { SetValue(NameProperty, value); }
            get { return (string)GetValue(NameProperty); }
        }

        // Angle dependency property and property
        public static DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle",
            typeof(double),
            typeof(Curler),
            new PropertyMetadata(0.0, AngleChangedCallback));

        public double Angle
        {
            set { SetValue(AngleProperty, value); }
            get { return (double)GetValue(AngleProperty); }
        }

        // Points dependency property and property
        public static DependencyProperty PointsProperty =
            DependencyProperty.Register("Points",
            typeof(PointCollection),
            typeof(Curler),
            new PropertyMetadata(new PointCollection()));

        public PointCollection Points
        {
            protected set { SetValue(PointsProperty, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        // AngleChangedCallback sets Points property
        static void AngleChangedCallback(DependencyObject obj, 
                                         DependencyPropertyChangedEventArgs args)
        {
            Curler bender = obj as Curler;
            bender.AngleChangedCallback();
        }

        void AngleChangedCallback()
        {
            // Avoid generating notifications on 
            //  every change to the PointCollection
            PointCollection points = Points;
            Points = null;

            // Clear the collection
            points.Clear();

            for (int i = -200; i <= 200; i++)
            {
                Point pt = new Point(i, 0);
                rotate.Angle = i * Angle / 100;
                points.Add(rotate.Transform(pt));
            }

            // Now set that the Points property.
            Points = points;
        }

        // Constructor initializes Points property
        public Curler()
        {
            Points = new PointCollection();
        }

        // CreateInstanceCore override when deriving from Freezable
        protected override Freezable CreateInstanceCore()
        {
            return new Curler();
        }
    }
}

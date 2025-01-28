using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;


namespace ParallaxUiTest
{
    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        public void RotChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            plx.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), e.NewValue);
        }

        public void RotLostMouseCapture(object sender, MouseEventArgs e)
        {
            // Smoothly transition back to no rotation when lost capture on the rotation slider.
            Rotation3D from = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotSlider.Value);
            Rotation3D to = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            Duration dur = new Duration(new TimeSpan(250 * 1000 * 10));

            plx.BeginAnimation(ParallaxUi.ParallaxUi.RotationProperty, new Rotation3DAnimation(from, to, dur, FillBehavior.Stop));
            RotSlider.BeginAnimation(Slider.ValueProperty, new DoubleAnimation(RotSlider.Value, 0, dur, FillBehavior.Stop));

            plx.Rotation = to;
            RotSlider.Value = 0;
        }

        private void ImplodeTransition(object sender, RoutedEventArgs e)
        {
            plx.ImplodeTransition();
        }

        private void Implode2DTransition(object sender, RoutedEventArgs e)
        {
            plx.Implode2DTransition();
        }

    }
}
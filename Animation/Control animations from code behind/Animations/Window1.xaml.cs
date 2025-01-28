using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Animations
{
	public partial class Window1
	{
		public Window1()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        //<Storyboard x:Key="RectangleAnimation">
        //    <DoubleAnimationUsingKeyFrames BeginTime="00:00:00" Storyboard.TargetName="myRectangle" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)">
        //        <SplineDoubleKeyFrame KeyTime="00:00:05" Value="400"/>
        //    </DoubleAnimationUsingKeyFrames>
        //    <DoubleAnimationUsingKeyFrames BeginTime="00:00:00" Storyboard.TargetName="myRectangle" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)">
        //        <SplineDoubleKeyFrame KeyTime="00:00:05" Value="360"/>
        //    </DoubleAnimationUsingKeyFrames>
        //</Storyboard>
        private void btnStartAnimation_Click(object sender, RoutedEventArgs e)
        {
            // address the storyboard by his name
            Storyboard sb = (Storyboard)this.FindResource("RectangleAnimation");

            // search for the two DoubleAnimationUsingKeyFrames elements in the storyboard
            DoubleAnimationUsingKeyFrames translate = (DoubleAnimationUsingKeyFrames)sb.Children[0];
            DoubleAnimationUsingKeyFrames rotate = (DoubleAnimationUsingKeyFrames)sb.Children[1];

            // search for the two SplineDoubleKeyFrame elements in the storyboard
            SplineDoubleKeyFrame translateFrame = (SplineDoubleKeyFrame)translate.KeyFrames[0];
            SplineDoubleKeyFrame rotateFrame = (SplineDoubleKeyFrame)rotate.KeyFrames[0];

            //Duration: adjust the property KeyTime to the value of the slider sldDuration
            translateFrame.KeyTime = new TimeSpan(0, 0, (int)sldDuration.Value);
            rotateFrame.KeyTime = new TimeSpan(0, 0, (int)sldDuration.Value);

            //Rotation: adjust the property Value of the rotateFrame to the value of the slider sldRotation
            rotateFrame.Value = sldRotation.Value;

            //Distance: adjust the property Value of the translateFrame to the value of the slider sldDistance
            translateFrame.Value = sldDistance.Value;
 
            //Finally start the animation
            sb.Begin(this);
        }
	}
}
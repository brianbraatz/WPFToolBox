using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimationCollab
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>

    public partial class Page1 : Page
    {

        public Page1()
        {
            InitializeComponent();
            image.MouseEnter += new System.Windows.Input.MouseEventHandler(image_MouseEnter);
            textBlock.MouseEnter += new System.Windows.Input.MouseEventHandler(textBlock_MouseEnter);
            rectangle.MouseEnter += new System.Windows.Input.MouseEventHandler(rectangle_MouseEnter);
        }

        void rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           //note how we clone it
            Storyboard s = ((Storyboard)this.FindResource("FadeIn2")).Clone();
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1), FillBehavior.HoldEnd);
            Storyboard.SetTargetProperty(da, new PropertyPath("(0).(1).[0].(2)", UIElement.RenderTransformProperty,
                                                                                 TransformGroup.ChildrenProperty,
                                                                                 ScaleTransform.ScaleXProperty));
            s.Children.Add(da);
            DoubleAnimationUsingKeyFrames d = (DoubleAnimationUsingKeyFrames)s.Children[0];
            d.BeginTime = TimeSpan.FromSeconds(.5);
            s.Begin(rectangle);
        }

        void textBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Storyboard s = (Storyboard)this.FindResource("FadeIn2");
            s.Begin(textBlock);
        }

        void image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Storyboard s = (Storyboard)this.FindResource("FadeIn");
            this.BeginStoryboard(s);
        }




    }
}
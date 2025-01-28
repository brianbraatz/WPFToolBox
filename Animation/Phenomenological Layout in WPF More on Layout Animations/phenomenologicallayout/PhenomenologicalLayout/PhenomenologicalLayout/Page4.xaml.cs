using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LayoutToLayout;
namespace PhenomenologicalLayout
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>

    public partial class Page4 : Page
    {
        public Page4()
        {
            InitializeComponent();
        }
        public ArrayList Targets = new ArrayList();
        public ArrayList Hosts = new ArrayList();
        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            //we need to keep a reference to our targets, which are just borders that will occupy the
            //right amount of layout space.  These are currently in the grid in the middle of the canvas
            foreach (FrameworkElement fe in hostPanel.Children)
            {
                Targets.Add((LayoutToLayoutTarget)fe);
            }
            //we also need to keep a reference to our hosts.  These are the actual borders that contain
            //the images on the uppermost canvas, which is what the user acutally sees
            foreach (FrameworkElement fe in trickPanel.Children)
            {
                Hosts.Add((LayoutToLayoutHost)fe);
            }

        }

        void v_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            LayoutToLayoutHost h = (LayoutToLayoutHost)sender;

            for (int i = 0; i < 4; i++)
            {
                //walk the hosts
                if (this.Hosts[i] != h)
                {
                    //enable everyone again
                    (this.Hosts[i] as LayoutToLayoutHost).IsEnabled = true;
                    //remove them from the top stack panel
                    targetPanel.Children.Remove(this.Targets[i] as UIElement);
                    //set up animation
                    (this.Hosts[i] as LayoutToLayoutHost).SetUpAnimation(false);
                    //add our ghost border back to the middle grid
                    LayoutToLayoutTarget target = this.Targets[i] as LayoutToLayoutTarget;
                    //this is a goofy way to maintain which grid cell the image came from
                    Grid.SetColumn(target, Convert.ToInt32(target.Name.Replace("image", "")));
                    hostPanel.Children.Add(target);
                }
                else
                {
                    //scale back the image
                    TransformGroup tg = (this.Hosts[i] as LayoutToLayoutHost).Child.RenderTransform as TransformGroup;
                    ScaleTransform st = tg.Children[0] as ScaleTransform;
                    st.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                    st.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);

                }

            }
        }

        void v_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //get the object that received the mouseover
            LayoutToLayoutHost h = (LayoutToLayoutHost)sender;
            //walk through the hosts
            for (int i = 0; i < 4; i++)
            {
                //different action if the host is the one we are moused over
                if (this.Hosts[i] != h)
                {
                    //disable it so it doesn't pick up hittesting while animating
                    (this.Hosts[i] as LayoutToLayoutHost).IsEnabled = false;
                    //remove its ghost from the middle grid
                    hostPanel.Children.Remove(this.Targets[i] as UIElement);
                    //set up animation the real one
                    (this.Hosts[i] as LayoutToLayoutHost).SetUpAnimation(false);
                    //add the ghost to the top stack panel - the animation will start when this
                    //action invalidates the layout, which the layout class is listening for
                    LayoutToLayoutTarget target = this.Targets[i] as LayoutToLayoutTarget;
                    targetPanel.Children.Add(target);
                }
                else
                {
                    //scale the image that received the mouse over
                    TransformGroup tg = (this.Hosts[i] as LayoutToLayoutHost).Child.RenderTransform as TransformGroup;
                    ScaleTransform st = tg.Children[0] as ScaleTransform;
                    st.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(3, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                    st.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(3, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);

                }
            }
        }


    }
}
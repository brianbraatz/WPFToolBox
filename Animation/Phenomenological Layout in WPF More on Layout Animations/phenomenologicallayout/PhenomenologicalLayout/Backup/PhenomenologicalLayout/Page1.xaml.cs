using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhenomenologicalLayout
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>

    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();

        }
        private void WindowLoaded(object sender, EventArgs e)
        {
            foreach (FrameworkElement fe in hostPanel.Children)
            {
                fe.MouseEnter += new System.Windows.Input.MouseEventHandler(v_MouseEnter);
                fe.MouseLeave += new System.Windows.Input.MouseEventHandler(v_MouseLeave);
            }
        }


        void v_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateImages(0, 0, 1, true, sender);
        }

        void v_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateImages(-100, -300, 3, false, sender);           
        }

        void AnimateImages(double xPos, double yPos, double scaleFactor, bool IsBack, object sender)
        {
            FrameworkElement selected_fe = (FrameworkElement)sender;
            foreach (FrameworkElement fe in hostPanel.Children)
            {
                if (fe != selected_fe)
                {
                    if (IsBack)
                        fe.IsEnabled = true;
                    else
                        fe.IsEnabled = false;
                    TransformGroup tg = fe.RenderTransform as TransformGroup;
                    TranslateTransform tt = tg.Children[1] as TranslateTransform;

                    tt.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(xPos, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                    tt.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(yPos, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                }
                else
                {
                    TransformGroup tg = fe.RenderTransform as TransformGroup;
                    ScaleTransform st = tg.Children[0] as ScaleTransform;
                    st.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(scaleFactor, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                    st.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(scaleFactor, TimeSpan.FromMilliseconds(300)), HandoffBehavior.Compose);
                }
            }

        }

    }
}
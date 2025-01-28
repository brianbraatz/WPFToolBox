
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace SDKSamples
{


    public partial class KeyFramesAnimationPage : Page
    {

        public KeyFramesAnimationPage()
       {

            InitializeComponent();
       }

        public void pageLoaded(object sender, RoutedEventArgs args)
        {
            myVisualBrush.Visual = myImage;
        
        }

        private void exampleCanvasLayoutUpdated(object sender, EventArgs args)
        {


            myVisualBrush.Viewbox =
                            new Rect(
                                Canvas.GetLeft(myRectangle),
                                Canvas.GetTop(myRectangle),
                                myRectangle.ActualWidth,
                                myRectangle.ActualHeight
                            );
        }

    }
}
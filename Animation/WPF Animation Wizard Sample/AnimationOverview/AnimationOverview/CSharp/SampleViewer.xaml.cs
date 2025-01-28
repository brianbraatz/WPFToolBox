
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace SDKSamples
{

    public partial class SampleViewer : Page
    {
       private Page[] examples;
       private UIElementCollection radiobuttons;
       private int sampleIndex;
       
       public SampleViewer()
       {      
            InitializeComponent();
            examples = new Page[16];
            radiobuttons = RadioButtonContainer.Children;
            
            examples[0] = new IntroPage();
            examples[1] = new AnimationUsesPage();
            examples[2] = new ControlAnimationPage();
            examples[3] = new DecorationAnimationPage();
            examples[4] = new AnimationTypesPage();
            examples[5] = new BasicAnimationPage();
            examples[6] = new BasicFromToPage();
            examples[7] = new BasicRepeatBehaviorPage();
            examples[8] = new BasicAutoReversePage();
            examples[9] = new BasicAccelerationDecelerationPage();
            examples[10] = new BasicExamplesPage();
            examples[11] = new KeyFramesAnimationPage();
            examples[12] = new KeyFramesSplinesPage();
            examples[13] = new KeyFramesExamplesPage();
            examples[14] = new PathAnimationPage();
            examples[15] = new ConclusionPage();
       
       }
       
       public void pageLoaded(object sender, RoutedEventArgs args)
       {
   
            Example1RadioButton.IsChecked = true;
            sampleIndex = 0;
       }
       
       
       private void zoomOutStoryboardCompleted(object sender, EventArgs args)
       {
           
            mainFrame.Navigate(examples[sampleIndex]);
        
       }
       
       private void frameContentRendered(object sender, EventArgs args)
       {
            
            Storyboard s = (Storyboard)this.Resources["ZoomInStoryboard"];
            s.Begin(this);
       }
       
       private void zoomInStoryboardCompleted(object sender, EventArgs e)
       {
            scrollViewerBorder.Visibility = Visibility.Visible;
       
       }

       private void GoBack(object sender, RoutedEventArgs args)
       {
           sampleIndex--;
           RadioButton radioBtn = (RadioButton)radiobuttons[sampleIndex];
           radioBtn.IsChecked = true;
       }
       private void GoNext(object sender, RoutedEventArgs args)
       {
               sampleIndex++;
               RadioButton radioBtn = (RadioButton)radiobuttons[sampleIndex];
               radioBtn.IsChecked = true;

       }
       private void sampleSelected(object sender, RoutedEventArgs args)
       {

         
         Point3DCollection points = new Point3DCollection();
         
         double ratio = myScrollViewer.ActualWidth / myScrollViewer.ActualHeight;
         
         points.Add(new Point3D(5, -5 * ratio, 0));
         points.Add(new Point3D(5, 5 * ratio, 0));
         points.Add(new Point3D(-5, 5 * ratio, 0));
         
         points.Add(new Point3D(-5, 5 * ratio, 0));
         points.Add(new Point3D(-5, -5 * ratio, 0));
         points.Add(new Point3D(5, -5 * ratio, 0));

         points.Add(new Point3D(-5, 5 * ratio, 0));
         points.Add(new Point3D(-5, -5 * ratio, 0));
         points.Add(new Point3D(5, -5 * ratio, 0));           
         
         points.Add(new Point3D(5, -5 * ratio, 0));  
         points.Add(new Point3D(5, 5 * ratio, 0));
         points.Add(new Point3D(-5, 5 * ratio, 0)); 
       
         myGeometry.Positions = points;
         myViewport3D.Width = 100;
         myViewport3D.Height = 100 * ratio;
         
         scrollViewerBorder.Visibility = Visibility.Hidden; 
         
         RadioButton button = sender as RadioButton;
         
                  if (button != null)
                  {
                    for(int i = 0; i<radiobuttons.Count; i++)
                    {
                        RadioButton radioBtn = radiobuttons[i] as RadioButton;
                        if (button.Content.ToString() == radioBtn.Content.ToString())
                        {
                            sampleIndex = i;
                        }
                    }
                    if (sampleIndex == (radiobuttons.Count -1))
                    {
                        NextBtn.IsEnabled = false;
                    }
                    else
                    {
                        NextBtn.IsEnabled = true;
                    }
                    if (sampleIndex == 0)
                    {
                        PreviousBtn.IsEnabled = false;
                    }
                    else
                    {
                        PreviousBtn.IsEnabled = true;
                    }
            
                  } 
     }  
    }
}
namespace Video
{
    using System;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Windows.Navigation;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.Threading;
    using System.Windows.Markup;
    using System.Xml;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Security;
    using System.Windows.Interop;
    using HitTestResult2D = System.Windows.Media.HitTestResult;

    
    public partial class Window1
    {
   
        void OnInitialized(object sender, EventArgs e)
        {
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // start video on the meshes
            
            
            VisualBrush vb = new VisualBrush(myMedia0);
            SetMaterial(0, vb);

            vb = new VisualBrush(myMedia1);
            SetMaterial(1, vb);
            vb = new VisualBrush(myMedia2);
            SetMaterial(2, vb);
            vb = new VisualBrush(myMedia3);
            SetMaterial(3, vb);
            vb = new VisualBrush(myMedia4);
            SetMaterial(4, vb);
            vb = new VisualBrush(myMedia5);
            SetMaterial(5, vb);
            
            
            Storyboard s = (Storyboard)this.FindResource("StoryboardPlane0");
            s.Begin(myViewport3D);
            s = (Storyboard)this.FindResource("StoryboardPlane1");
            s.Begin(myViewport3D);
            s = (Storyboard)this.FindResource("StoryboardPlane2");
            s.Begin(myViewport3D);
            s = (Storyboard)this.FindResource("StoryboardPlane3");
            s.Begin(myViewport3D);
            s = (Storyboard)this.FindResource("StoryboardPlane4");
            s.Begin(myViewport3D);
            s = (Storyboard)this.FindResource("StoryboardPlane5");
            s.Begin(myViewport3D);

            // setup trackball for moving the model around
            
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport3D);
            _trackball.Enabled = true;

            MediaTimeline mt = new MediaTimeline(new Uri(@"media\Coral_Sample.wmv", UriKind.Relative));
            MediaClock mc = mt.CreateClock();
            myMedia0.Clock = mc;
        }
        
        #region Events

        #region Buttons

        private void OnStop(object sender, RoutedEventArgs e)
        {
            myMedia0.Clock.Controller.Stop();
        }

        private void OnBegin(object sender, RoutedEventArgs e)
        {
            myMedia0.Clock.Controller.Begin();
        }

        private void OnResume(object sender, RoutedEventArgs e)
        {
            myMedia0.Clock.Controller.Resume();
        }

        private void OnPause(object sender, RoutedEventArgs e)
        {
            myMedia0.Clock.Controller.Pause();
        }

        private void OnReverse(object sender, RoutedEventArgs e)
        {
            // Not Implemented
            //MediaClock mc = myMedia0.Clock;
            //mc.Controller.Reverse();
        }

        private void OnSpeedRatio(object sender, RoutedEventArgs e)
        {
            MediaClock mc = myMedia0.Clock;
            mc.Controller.SpeedRatio = 2;
        }

        private void OnSeek(object sender, RoutedEventArgs e)
        {
            // Didn't work
            MediaClock mc = myMedia0.Clock;
            mc.Controller.Seek(TimeSpan.FromSeconds(15), TimeSeekOrigin.BeginTime);
        }

        #endregion

        #endregion Events

        #region Private

        private void SetMaterial(int position, Brush brush)
        {
            
            ModelVisual3D mv3d = myViewport3D.Children[0] as ModelVisual3D;
            mv3d = mv3d.Children[0] as ModelVisual3D;
            mv3d = mv3d.Children[position] as ModelVisual3D;
            GeometryModel3D gm3d = mv3d.Content as GeometryModel3D;
            DiffuseMaterial bm = new DiffuseMaterial(brush);
            gm3d.Material = bm;
            
        }

        #endregion

        #region Globals
        Trackball _trackball;
        #endregion
         
    }
}

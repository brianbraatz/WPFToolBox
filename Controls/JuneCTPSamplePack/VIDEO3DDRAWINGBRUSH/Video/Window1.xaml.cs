using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using Mesh3DObjects;
using DemoDev;

namespace Ribbon
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport3D);
            _trackball.Enabled = true;

            _sphere = new Sphere();
            _cylinder = new Cylinder();

            _vd1 = new VideoDrawing();
            _vd2 = new VideoDrawing();
            _vd3 = new VideoDrawing();
            _vd4 = new VideoDrawing();

            OnCreate(null, null);
        }

        #region Events

        void OnCreate(object sender, RoutedEventArgs e)
        {
            Transform3DGroup t3dg;
            RotateTransform3D rt3d;

            Model3DGroup m3dg = (myViewport3D.Children[0] as ModelVisual3D).Content as Model3DGroup;

            GeometryModel3D gm3D1 = m3dg.Children[2] as GeometryModel3D;
            GeometryModel3D gm3D2 = m3dg.Children[3] as GeometryModel3D;
            GeometryModel3D gm3D3 = m3dg.Children[4] as GeometryModel3D;
            GeometryModel3D gm3D4 = m3dg.Children[5] as GeometryModel3D;

            // rotate mesh 1 
            t3dg = gm3D1.Transform as Transform3DGroup;
            rt3d = t3dg.Children[1] as RotateTransform3D;
            RotateMesh(rt3d, new TimeSpan(0, 0, 2));

            // rotate mesh 2 
            t3dg = gm3D2.Transform as Transform3DGroup;
            rt3d = t3dg.Children[1] as RotateTransform3D;
            RotateMesh(rt3d, new TimeSpan(0, 0, 1));

            // rotate mesh 3 
            t3dg = gm3D3.Transform as Transform3DGroup;
            rt3d = t3dg.Children[1] as RotateTransform3D;
            RotateMesh(rt3d, new TimeSpan(0, 0, 1));

            // rotate mesh 4 
            t3dg = gm3D4.Transform as Transform3DGroup;
            rt3d = t3dg.Children[1] as RotateTransform3D;
            RotateMesh(rt3d, new TimeSpan(0, 0, 1));
            
            // use the new mesh
            gm3D2.Geometry = _cylinder.Mesh;
            gm3D4.Geometry = _sphere.Mesh;

            DrawVideoOnMesh(_vd1, gm3D1, new Uri(@"media\T2_Sample.wmv", UriKind.Relative));
            DrawVideoOnMesh(_vd2, gm3D2, new Uri(@"http://www.videodetective.net/player.asp?cmd=3&publishedid=862846&CustomerId=67272&videokbrate=100")); // streaming sample
            DrawVideoOnMesh(_vd3, gm3D3, new Uri(@"media\Cave_Sample.wmv", UriKind.Relative));
            DrawVideoOnMesh(_vd4, gm3D4, new Uri(@"media\Coral_Sample.wmv", UriKind.Relative));


        }

        void OnStop(object sender, RoutedEventArgs e)
        {
            myMedia.Stop();

            _vd1.Player.Clock.Controller.Stop();
            
        }

        void OnBegin(object sender, RoutedEventArgs e)
        {
            myMedia.Position = new TimeSpan(0);
            myMedia.Play();

            _vd1.Player.Clock.Controller.Begin();
        }

        void OnPause(object sender, RoutedEventArgs e)
        {
            myMedia.Pause();

            _vd1.Player.Clock.Controller.Pause();
        }

        void OnResume(object sender, RoutedEventArgs e)
        {
            myMedia.Play();

            _vd1.Player.Clock.Controller.Resume();
        }
        #endregion

        #region Private Methods

        // create the video and apply to the mesh
        private void DrawVideoOnMesh(VideoDrawing vd, GeometryModel3D gm3d, Uri uri)
        {
            MediaTimeline mt = new MediaTimeline(uri);
            mt.CurrentTimeInvalidated += new EventHandler(mt_CurrentTimeInvalidated);

            MediaClock mc = mt.CreateClock();
            MediaPlayer mp = new MediaPlayer();
            mp.Clock = mc;
            vd.Player = mp;
            vd.Rect = new Rect(0, 0, 400, 400);
            mc.Controller.Begin();

            DrawingBrush db = new DrawingBrush();
            db.Drawing = vd;

            Brush br = db as Brush;

            MaterialGroup mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(br));
            //mg.Children.Add(new SpecularMaterial(Brushes.White, 40));
            gm3d.Material = mg;
        }

        void mt_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            
        }

        private void RotateMesh (RotateTransform3D rt3d, TimeSpan timespan)
        {
            return;
            // spin the model
            Rotation3D from = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            Rotation3D to = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);

            Rotation3DAnimation animation = new Rotation3DAnimation(from, to, timespan);
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.IsCumulative = true;
            animation.FillBehavior = FillBehavior.HoldEnd;

            rt3d.BeginAnimation(RotateTransform3D.RotationProperty, animation);
        }

        #endregion

        #region Globals

        VisualBrush _vBrush;
        Cylinder _cylinder;
        Sphere _sphere;
        Trackball _trackball;

        VideoDrawing _vd1;
        VideoDrawing _vd2;
        VideoDrawing _vd3;
        VideoDrawing _vd4;

        #endregion
    }

}




using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Windows.Shapes;
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

            _CameraPosition = new Point3D (0,0,5);
            _CameraTarget = new Point3D(0, 0, 0);
            _CameraUp = new Vector3D(0, 1, 0);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // Setup MatrixCamera
            GenerateViewMatrix();
            GeneratePerspectivePprojectionMatrix();

            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport3D);
            _trackball.Enabled = true;

        }

        #region Events

        private void OnMoveForward(object sender, RoutedEventArgs e)
        {
            myTranslateInner.OffsetZ += 1;
            SetLine();
        }

        private void OnMoveBack(object sender, RoutedEventArgs e)
        {
            myTranslateInner.OffsetZ -= 1;
            SetLine();
        }

        private void OnRotateLeft(object sender, RoutedEventArgs e)
        {
            myRotate.Angle += -1;
            SetLine();
        }

        private void OnRotateRight(object sender, RoutedEventArgs e)
        {
            myRotate.Angle += 1;
            SetLine();
        }

        private void OnScaleDown(object sender, RoutedEventArgs e)
        {
            myScale.ScaleX += -0.1;
            myScale.ScaleY += -0.1;
            myScale.ScaleZ += -0.1;
            SetLine();
        }

        private void OnScaleUp(object sender, RoutedEventArgs e)
        {
            myScale.ScaleX += 0.1;
            myScale.ScaleY += 0.1;
            myScale.ScaleZ += 0.1;
            SetLine();

        }

        private void OnSetLine(object sender, RoutedEventArgs e)
        {
            SetLine();
        }
        

        #endregion

        #region Private Methods


        private void GenerateViewMatrix()
        {

            Vector3D backward = _CameraPosition - _CameraTarget;
            backward.Normalize();

            Vector3D right = Vector3D.CrossProduct(_CameraUp, backward);
            right.Normalize();

            Vector3D up2 = Vector3D.CrossProduct(backward, right);
            up2.Normalize();

            // backward, up, right
            view_matrix = new Matrix3D();

            view_matrix.M11 = right.X;
            view_matrix.M21 = right.Y;
            view_matrix.M31 = right.Z;

            view_matrix.M12 = up2.X;
            view_matrix.M22 = up2.Y;
            view_matrix.M32 = up2.Z;

            view_matrix.M13 = backward.X;
            view_matrix.M23 = backward.Y;
            view_matrix.M33 = backward.Z;

            view_matrix.M14 = 0;
            view_matrix.M24 = 0;
            view_matrix.M34 = 0;
            view_matrix.M44 = 1;

            view_matrix = new TranslateTransform3D(-(Vector3D)_CameraPosition).Value * view_matrix;

            ((MatrixCamera)(myViewport3D.Camera)).ViewMatrix = view_matrix;
            
        }

        private void GeneratePerspectivePprojectionMatrix()
        {
            double fov_rad = (fov_degrees / 180.0) * Math.PI;
            double aspect_ratio = (double)myViewport3D.ActualWidth / (double)myViewport3D.ActualHeight;
            double denom = Math.Tan(fov_rad / 2.0);

            double m11 = 1.0 / denom;
            double m22 = aspect_ratio / denom;
            double m33 = far_clip_plane / (near_clip_plane - far_clip_plane);
            double m43 = near_clip_plane * far_clip_plane / (near_clip_plane - far_clip_plane);

            projection_matrix = new Matrix3D(m11, 0, 0, 0,
                                               0, m22, 0, 0,
                                               0, 0, m33, -1,
                                               0, 0, m43, 0);

            ((MatrixCamera)(myViewport3D.Camera)).ProjectionMatrix = projection_matrix;
        }

        private void SetLine()
        {
            // move mesh point to world coordinates
            Transform3DGroup tg = myPlane.Transform as Transform3DGroup;
            Point3D CS = new Point3D(-1, 1, 0); // hardcoded point where the vertex point is
            CS = tg.Transform(CS);

            Point4D WCS = new Point4D(CS.X, CS.Y, CS.Z, 1);

            //Point4D WCS = new Point4D(-1, 1, 0, 1);
            Point4D CCS = WCS * view_matrix * projection_matrix;
            Point3D NDCS = new Point3D();

            NDCS.X = CCS.X / CCS.W;
            NDCS.Y = CCS.Y / CCS.W;

            Point DCS = new Point();  // the window coordinates of the projected point

            DCS.X = ((NDCS.X + 1.0) / 2.0) * myViewport3D.ActualWidth;
            DCS.Y = myViewport3D.ActualHeight * (1.0 - ((NDCS.Y + 1.0) / 2.0));

            myLine.X2 = DCS.X;
            myLine.Y2 = DCS.Y;
        }

        #endregion

        #region Globals

        Trackball _trackball;

        Point3D _CameraPosition;
        Point3D _CameraTarget;
        Vector3D _CameraUp;
        Matrix3D view_matrix = new Matrix3D();


        double fov_degrees = 60;
        double near_clip_plane = 0.1;
        double far_clip_plane = 50;
        Matrix3D projection_matrix;

        #endregion
    }

   
}




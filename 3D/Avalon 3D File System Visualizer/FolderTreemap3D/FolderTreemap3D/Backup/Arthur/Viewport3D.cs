using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Arthur
{
	public delegate void Dispatched();

	public class Viewport3D : System.Windows.Controls.Viewport3D
	{
		public double CameraDistance = 3; // distance from camera to origin
		public double CameraLatitude = Math.PI / 2; // angle from +ve y axis to camera (i.e. latitude)
		public double CameraLongitude = Math.PI; // angle from -ve z axis to camera (i.e. longitude)
		private double _panX = 0;
		private double _panY = 0;
		System.Windows.Controls.Panel _parentPanel = null;
		private double _lastMouseX = 0;
		private double _lastMouseY = 0;

		protected override void OnInitialized(EventArgs args)
		{
			SetCameraPosition();
			_parentPanel = this.Parent as System.Windows.Controls.Panel;
            _parentPanel.MouseLeftButtonDown += new MouseButtonEventHandler(parentWindow_MouseButtonDown);
            _parentPanel.MouseRightButtonDown += new MouseButtonEventHandler(parentWindow_MouseButtonDown);
            _parentPanel.MouseMove += new MouseEventHandler(parentWindow_MouseMove);
		}

        private void parentWindow_MouseButtonDown(object sender, MouseEventArgs e)
		{
			_lastMouseX = Mouse.GetPosition(this).X;
			_lastMouseY = Mouse.GetPosition(this).Y;
		}

		public void SetCameraPosition()
		{
			double x = CameraDistance * -Math.Sin(CameraLongitude) * Math.Sin(CameraLatitude);
			double y = CameraDistance * Math.Cos(CameraLatitude);
			double z = CameraDistance * -Math.Cos(CameraLongitude) * Math.Sin(CameraLatitude);

            ProjectionCamera cam = this.Camera.Clone() as ProjectionCamera;
			cam.Position = new Point3D(x, y, z);
            cam.LookDirection = new Vector3D(-x, -y, -z);
            cam.NearPlaneDistance = 0.01;
			this.Camera = cam;

			TranslateTransform3D pan = new TranslateTransform3D(new Vector3D(_panX, _panY, 0));
            ((this.Children[0] as ModelVisual3D).Content as Model3DGroup).Transform = pan;
		}

		private void parentWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				double currentMouseX = Mouse.GetPosition(this).X;
				double currentMouseY = Mouse.GetPosition(this).Y;
				double dx = _lastMouseX - currentMouseX;
				double dy = _lastMouseY - currentMouseY;

				_lastMouseX = currentMouseX;
				_lastMouseY = currentMouseY;

				CommonMouseMove(dx, dy, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed);

				SetCameraPosition();
				e.Handled = true;
			}
		}


		private void CommonMouseMove(double dx, double dy, bool leftMouseButton, bool rightMouseButton)
		{
			double mouseSensitivity = 5e-3;

            if (leftMouseButton && rightMouseButton) // left+right button activates pan.
			{
				_panX -= dx * mouseSensitivity; _panY += dy * mouseSensitivity;
			}
			else if (rightMouseButton) // right button activates zoom
			{
				CameraDistance += (-dx - dy) * mouseSensitivity;
			}
			else if (leftMouseButton) // left button means rotate
			{
				CameraLatitude += dy * mouseSensitivity;
				if (CameraLatitude < 0)
					CameraLatitude = 1e-10; // not sure yet why setting this to 0 causes a problem (cos(0) is 1 after all).
				if (CameraLatitude > Math.PI)
					CameraLatitude = Math.PI;
				CameraLongitude += dx * mouseSensitivity;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using _3DTools;

namespace Wpf3DGraphingLibrary
{
    /// <summary>
    /// A tooltip that appears in a 3D scene.
    /// </summary>
    public class ToolTip3D : ModelVisual3D
    {
        #region Fields

        private const double DefaultWidthSizeFactor = 150d;
        private const double DefaultHeightSizeFactor = 150d;
        private Matrix3D _visualToScreen;
        private Matrix3D _screenToVisual;
        private GeometryModel3D _model;
        private MeshGeometry3D _mesh;
        private List<Point3D> _Points;
        private double _TextWidth, _TextHeight;
        private Point3D _CenterPoint;
        private Color _TextColor = Colors.Black;
        private Color _BackColor = Colors.BlanchedAlmond;
        private bool _DrawBackground = false;
        private bool _Visible = true;
        private string _Text;
        private double _WidthSizeFactor = DefaultWidthSizeFactor;
        private double _HeightSizeFactor = DefaultHeightSizeFactor;
        private Point3D _Position = new Point3D(0, 0, 0);
        private PerspectiveCamera _Camera;

        #endregion

        #region Properties

        /// <summary>
        /// Sets the color of the text.
        /// </summary>
        public Color TextColor
        {
            get { return _TextColor; }
            set { SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the TextColor property.
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(
                "TextColor",
                typeof(Color),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    Colors.White,
                    OnTextColorChanged));

        private static void OnTextColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._TextColor = (Color)args.NewValue;
            tt.SetText();
        }

        /// <summary>
        /// Background color for the tooltip.
        /// </summary>
        public Color BackColor
        {
            get { return _BackColor; }
            set { SetValue(BackColorProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the BackColor property.
        /// </summary>
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(
                "BackColor",
                typeof(Color),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    Colors.White,
                    OnBackColorChanged));

        private static void OnBackColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._BackColor = (Color)args.NewValue;
            tt.SetText();
        }

        /// <summary>
        /// Gets or sets whether or not to draw the background for the tooltip.
        /// </summary>
        public bool DrawBackground
        {
            get { return _DrawBackground; }
            set 
            { 
                _DrawBackground = value;
                SetText();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the tooltip is visible.
        /// </summary>
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;
                SetText();
            }
        }

        /// <summary>
        /// Gets or sets the text to display in the tooltip.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the Text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    "",
                    OnTextChanged));

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._Text = (string)args.NewValue;
            tt.SetText();
        }

        /// <summary>
        /// The width size factor is used to reduce the size of the tooltip.  The tooltip size could
        /// change depending on the size of the viewport, camera angle, etc.
        /// </summary>
        /// <remarks>
        /// TODO: Calculate factors based on field of view and near plane distance.
        /// </remarks>
        public double WidthSizeFactor
        {
            get { return _WidthSizeFactor; }
            set { SetValue(WidthSizeFactorProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the WidthSizeFactor property.
        /// </summary>
        public static readonly DependencyProperty WidthSizeFactorProperty =
            DependencyProperty.Register(
                "WidthSizeFactor",
                typeof(double),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    DefaultWidthSizeFactor,
                    OnWidthSizeFactorChanged));

        private static void OnWidthSizeFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._WidthSizeFactor = (double)args.NewValue;
            tt.GeometryDirty();
        }

        /// <summary>
        /// The height size factor is used to reduce the size of the tooltip.  The tooltip size could
        /// change depending on the size of the viewport, camera angle, etc.
        /// </summary>
        /// <remarks>
        /// TODO: Calculate factors based on field of view and near plane distance.
        /// </remarks>
        public double HeightSizeFactor
        {
            get { return _HeightSizeFactor; }
            set { SetValue(HeightSizeFactorProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the HeightSizeFactor property.
        /// </summary>
        public static readonly DependencyProperty HeightSizeFactorProperty =
            DependencyProperty.Register(
                "HeightSizeFactor",
                typeof(double),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    DefaultHeightSizeFactor,
                    OnHeightSizeFactorChanged));

        private static void OnHeightSizeFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._HeightSizeFactor = (double)args.NewValue;
            tt.GeometryDirty();
        }

        /// <summary>
        /// Position of the tooltip
        /// </summary>
        public Point3D Position
        {
            get { return _Position; }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Allows for animation on the Position property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position",
                typeof(Point3D),
                typeof(ToolTip3D),
                new PropertyMetadata(
                    new Point3D(0, 0, 0),
                    OnPositionChanged));

        private static void OnPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolTip3D tt = sender as ToolTip3D;
            tt._Position = (Point3D)args.NewValue;
            tt.SetPosition();
            tt.GeometryDirty();
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a tooltip with no background.
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="text"></param>
        public ToolTip3D(PerspectiveCamera cam, string text)
        {
            _DrawBackground = false;
            _Camera = cam;
            _Camera.Changed += new EventHandler(_Camera_Changed);
            SetupModel();
            _Text = text;
            SetText();
        }

        void _Camera_Changed(object sender, EventArgs e)
        {
            SetPosition();
        }

        #endregion

        #region Methods

        private void SetupModel()
        {
            CompositionTarget.Rendering += OnRender;
            SetPosition();

            _mesh = new MeshGeometry3D();
            RebuildGeometry();
            int[] arr = new int[] { 0, 3, 2, 2, 1, 0 };
            foreach (int i in arr)
                _mesh.TriangleIndices.Add(i);
            _mesh.TextureCoordinates.Add(new Point(1, 1));
            _mesh.TextureCoordinates.Add(new Point(0, 1));
            _mesh.TextureCoordinates.Add(new Point(0, 0));
            _mesh.TextureCoordinates.Add(new Point(1, 0));

            _model = new GeometryModel3D();
            _model.Geometry = _mesh;
            this.Content = _model;
        }

        private void OnRender(object sender, EventArgs e)
        {
            if (_mesh.Positions.Count == 0)
            {
                return;
            }

            if (UpdateTransforms())
            {
                SetPosition();
                RebuildGeometry();
            }
        }

        private bool UpdateTransforms()
        {
            Viewport3DVisual viewport;
            bool success;

            Matrix3D visualToScreen = MathUtils.TryTransformTo2DAncestor(this, out viewport, out success);

            if (!success || !visualToScreen.HasInverse)
                return false;

            if (visualToScreen == _visualToScreen)
                return false;

            _visualToScreen = _screenToVisual = visualToScreen;
            _screenToVisual.Invert();

            return true;
        }

        private void SetPosition()
        {
            Vector3D lookAt = _Position - _Camera.Position;
            lookAt.Normalize();
            double d = _Camera.NearPlaneDistance + 1d;
            _CenterPoint = new Point3D(lookAt.X * d + _Camera.Position.X,
                lookAt.Y * d + _Camera.Position.Y, 
                lookAt.Z * d + _Camera.Position.Z);
            Vector3D up = new Vector3D(_Camera.UpDirection.X, _Camera.UpDirection.Y, _Camera.UpDirection.Z);
            up.Normalize();
            Vector3D right = Vector3D.CrossProduct(up, _Camera.LookDirection);
            right.Normalize();
            _Points = new List<Point3D>();
            _Points.Add(_CenterPoint - up - right);  // 0 - lower left-hand corner
            _Points.Add(_CenterPoint - up + right);  // 1 - lower right-hand corner
            _Points.Add(_CenterPoint + up + right);  // 2 - upper right-hand corner
            _Points.Add(_CenterPoint + up - right);  // 3 - upper left-hand corner
        }

        private void RebuildGeometry()
        {
            Point3DCollection points = new Point3DCollection(4);
            foreach (Point3D p3d in _Points)
            {
                Vector3D dir = (p3d - _CenterPoint) * _visualToScreen;
                dir.Z = 0;
                dir.Normalize();
                Vector delta = new Vector(-dir.Y, dir.X);

                Point4D pIn4 = (Point4D)(new Point3D(p3d.X - _CenterPoint.X, p3d.Y - _CenterPoint.Y, p3d.Z - _CenterPoint.Z));
                Point4D pOut4 = pIn4 * _visualToScreen;
                pOut4.X += delta.X * pOut4.W;
                pOut4.Y += delta.Y * pOut4.W;
                pOut4 *= _screenToVisual;
                Point3D pOut = new Point3D(
                    pOut4.X / pOut4.W * (_TextWidth / (2d * WidthSizeFactor)) + _CenterPoint.X,
                    pOut4.Y / pOut4.W * (_TextHeight / (2d * HeightSizeFactor)) + _CenterPoint.Y,
                    pOut4.Z / pOut4.W + _CenterPoint.Z);
                points.Add(pOut);
            }
            points.Freeze();
            _mesh.Positions = points;
        }

        private void SetText()
        {
            FormattedText ft = new FormattedText(_Text,
                    new CultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                    12D,
                    new SolidColorBrush(_TextColor));
            _TextWidth = ft.Width;
            _TextHeight = ft.Height;
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (_Visible)
            {
                if (_DrawBackground)
                {
                    Brush backBrush = new SolidColorBrush(_BackColor);
                    drawingContext.DrawRectangle(backBrush, new Pen(backBrush, 1), new Rect(0, 0, ft.Width, ft.Height));
                }
                drawingContext.DrawText(ft, new Point(0, 0));
            }
            drawingContext.Close();

            VisualBrush vb = new VisualBrush(drawingVisual);
            _model.Material = new DiffuseMaterial(vb);
            GeometryDirty();
        }

        private void GeometryDirty()
        {
            // Force next call to UpdateTransforms() to return true.
            _visualToScreen = MathUtils.ZeroMatrix;
        }

        #endregion
    }
}

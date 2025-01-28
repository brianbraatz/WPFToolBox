using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace ParallaxUi
{
    /// <summary>
    /// Interaction logic for ParallaxUi.xaml
    /// </summary>

    public partial class ParallaxUi : System.Windows.Controls.Decorator
    {
        public ParallaxUi()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        #region Rotation property

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation",
                                        typeof(Rotation3D),
                                        typeof(ParallaxUi),
                                        new FrameworkPropertyMetadata(new AxisAngleRotation3D(ParallaxUi._yAxis, 0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender,
                                                                      new PropertyChangedCallback(RotationChanged)));

        public Rotation3D Rotation
        {
            get { return (Rotation3D)this.GetValue(RotationProperty); }
            set { this.SetValue(RotationProperty, value); }
        }
	
        private static void RotationChanged(DependencyObject changedObject, DependencyPropertyChangedEventArgs e)
        {
            ParallaxUi plx = (ParallaxUi)changedObject;            
            plx.RotChanged((Rotation3D)e.NewValue, plx.Center);
        }

        #endregion

        #region Center property

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center",
                                        typeof(Point3D),
                                        typeof(ParallaxUi),
                                        new FrameworkPropertyMetadata(new Point3D(0.5, 0.5, 0.5),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender,
                                                                      new PropertyChangedCallback(CenterChanged)));

        public Point3D Center
        {
            get { return (Point3D)this.GetValue(CenterProperty); }
            set { this.SetValue(CenterProperty, value); }
        }

        private static void CenterChanged(DependencyObject changedObject, DependencyPropertyChangedEventArgs e)
        {
            ParallaxUi plx = (ParallaxUi)changedObject;
            Point3D center = (Point3D)e.NewValue;
            plx.RotChanged(plx.Rotation, center);
        }

        #endregion

        #region SpeedFactor property

        // For speeding up and slowing down the animations.  Defaults to 1.0.  Higher is faster.

        public static readonly DependencyProperty SpeedFactorProperty =
            DependencyProperty.Register("SpeedFactor",
                                        typeof(double),
                                        typeof(ParallaxUi),
                                        new FrameworkPropertyMetadata(1.0));

        public double SpeedFactor
        {
            get { return (double)this.GetValue(SpeedFactorProperty); }
            set { this.SetValue(SpeedFactorProperty, value); }
        }

        #endregion

        #region RevolutionsFactor property

        // For increasing and decreasing the number of revolutions during an animation.  Defaults to 1.0.  Higher is more.

        public static readonly DependencyProperty RevolutionsFactorProperty =
            DependencyProperty.Register("RevolutionsFactor",
                                        typeof(double),
                                        typeof(ParallaxUi),
                                        new FrameworkPropertyMetadata(1.0));

        public double RevolutionsFactor
        {
            get { return (double)this.GetValue(RevolutionsFactorProperty); }
            set { this.SetValue(RevolutionsFactorProperty, value); }
        }

        #endregion

        #endregion

        /// <summary>
        /// Invoke a 3D transition where the elements are thrown out to random locations and come into place with
        /// random speed, etc.
        /// </summary>
        public void ImplodeTransition()
        {
            Point3D origMidpoint = new Point3D(_originalChild.RenderSize.Width / 2,
                                               _originalChild.RenderSize.Height / 2,
                                               0);

            _longestDurationMsec = 0;
            _longestTimeline = null;

            _inTransition = true;
            ChooseVisibleItem();

            // First create all the timelines, then register a completed handler on the longest, then start them all.
            Queue<AnimationTimeline> q = new Queue<AnimationTimeline>(_eltsInfo.Count * 4);
            foreach (EltInfo ei in _eltsInfo)
            {
                double degreesToRotateThrough = _rand.Next(45, 1440) * RevolutionsFactor;
                q.Enqueue(MyDoubleAnim(degreesToRotateThrough, 0));

                // Move elements in along rays projecting from the overall center through the element's center.
                Vector3D motionRay = ei._midPoint - origMidpoint;

                // use the below for a random incoming motion.
                //Vector3D motionRay = RandomAxis() * _rand.NextDouble() * _initialElementBounds.SizeZ * 2.0;

                q.Enqueue(MyDoubleAnim(motionRay.X, 0));
                q.Enqueue(MyDoubleAnim(-motionRay.Y, 0));     // -y due to coord sys diffs between 2D and 3D
                q.Enqueue(MyDoubleAnim(motionRay.Z * 2, 0));  // exaggerate the z motion a bit
            }

            // Create the overall rotation so that it ends at exactly the moment the longest duration animation ends.
            Vector3D initialRotVector = new Vector3D(1, 1, 0);
            AxisAngleRotation3D initialRot = new AxisAngleRotation3D(initialRotVector, 65);
            AxisAngleRotation3D finalRot = new AxisAngleRotation3D(initialRotVector, 0);
            AnimationTimeline rotAnim = new Rotation3DAnimation(
                                                    initialRot,
                                                    finalRot,
                                                    new Duration(new TimeSpan(_longestDurationMsec * 1000 * 10)),
                                                    FillBehavior.Stop);
            rotAnim.DecelerationRatio = 0.2;
            _longestTimeline.Completed += delegate(object sender, EventArgs e)
                    {
                        _inTransition = false;
                        ChooseVisibleItem();
                    };

            // Now start all the animations.
            this.BeginAnimation(RotationProperty, rotAnim); 
            foreach (EltInfo ei in _eltsInfo)
            {
                ei._rotToApply.Axis = RandomAxis();
                ei._rotToApply.BeginAnimation(AxisAngleRotation3D.AngleProperty, q.Dequeue());
                ei._xltToApply.BeginAnimation(TranslateTransform3D.OffsetXProperty, q.Dequeue());
                ei._xltToApply.BeginAnimation(TranslateTransform3D.OffsetYProperty, q.Dequeue());
                ei._xltToApply.BeginAnimation(TranslateTransform3D.OffsetZProperty, q.Dequeue());
            }
        }

        /// <summary>
        /// Invoke a 2D transition that updates the RenderTransform on each element with rotations and
        /// translations, eventually ending up at the original configuration.
        /// </summary>
        public void Implode2DTransition()
        {
                        
            Point origMidpoint = new Point(_originalChild.RenderSize.Width / 2,
                                           _originalChild.RenderSize.Height / 2);

            _longestDurationMsec = 0;
            _longestTimeline = null;

            foreach (EltInfo ei in _eltsInfo)
            {
                double degreesToRotateThrough = _rand.Next(45, 1440) * RevolutionsFactor;
                RotateTransform rot = new RotateTransform();
                ei._elt.RenderTransformOrigin = new Point(0.5, 0.5);
                //rot.CenterX = ei._midPoint.X;
                //rot.CenterY = ei._midPoint.Y;
                rot.BeginAnimation(RotateTransform.AngleProperty, MyDoubleAnim(degreesToRotateThrough, 0));

                // Move elements in along rays projecting from the overall center through the element's center.
                Vector motionRay = new Point(ei._midPoint.X, ei._midPoint.Y) - origMidpoint;

                TranslateTransform xlt = new TranslateTransform();
                xlt.BeginAnimation(TranslateTransform.XProperty, MyDoubleAnim(motionRay.X, 0)); 
                xlt.BeginAnimation(TranslateTransform.YProperty, MyDoubleAnim(motionRay.Y, 0));

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(rot);
                tg.Children.Add(xlt);
                ei._elt.RenderTransform = tg;
            }      
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            EnsureParallaxElement();                               
            base.OnRender(drawingContext);
        }

        // Private helpers
        private AnimationTimeline MyDoubleAnim(double from, double to)
        {
            int msec = _rand.Next((int)(500.0/SpeedFactor), (int)(5000.0/SpeedFactor));
            return MyDoubleAnim(from, to, msec);
        }

        private AnimationTimeline MyDoubleAnim(double from, double to, int msec)
        {
            DoubleAnimation da = new DoubleAnimation(from, to, new Duration(new TimeSpan(msec * 1000 * 10))); 
            da.DecelerationRatio = 0.2;
            da.FillBehavior = FillBehavior.Stop;

            if (msec > _longestDurationMsec)
            {
                _longestDurationMsec = msec;
                _longestTimeline = da;
            }

            return da;
        }

        private Vector3D RandomAxis()
        {
            Vector3D v = new Vector3D();

            // Pick random value for each component, and random sign too
            v.X = _rand.NextDouble() * (_rand.Next(2) * 2 - 1); 
            v.Y = _rand.NextDouble() * (_rand.Next(2) * 2 - 1); 
            v.Z = _rand.NextDouble() * (_rand.Next(2) * 2 - 1);

            v.Normalize();

            return v;
        }

        private Color RandomColor()
        {
            return Color.FromRgb((byte)_rand.Next(0, 256),
                                 (byte)_rand.Next(0, 256),
                                 (byte)_rand.Next(0, 256));
        }

        /// <summary>
        /// Create a GeometryModel3D of the element by making a mesh that covers the same size, is properly positioned,
        /// and receives a VisualBrush of the element itself as its material.  The key to making this work is careful
        /// positioning of the 3D element stand in in 3-space according to the 2-space dimensions and location.
        /// </summary>
        private void AddElementAs3D(EltInfo ei, double maxZ, List<ModelWithZ> models)
        {
            UIElement elt = ei._elt;
            VisualBrush vb = new VisualBrush(elt);
            vb.TileMode = TileMode.None;

            // Big perf wins from caching the brush!!
            RenderOptions.SetCachingHint(vb, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(vb, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(vb, 2.0);


            Size realRenderSize = TransformSize(elt.TransformToAncestor(_originalChild), elt.RenderSize);

            Point localMidpoint = new Point(realRenderSize.Width / 2, realRenderSize.Height / 2);
            Point parentMidpoint = elt.TranslatePoint(localMidpoint, _originalChild);

            // pick value in [-maxZ, maxZ]
            double zValue = (_rand.NextDouble() * 2 * maxZ) - maxZ;
            ei._midPoint = new Point3D(parentMidpoint.X, parentMidpoint.Y, zValue);

            // Use (0,height) rather than (0,0) since the 3D coord system is y-up, and 2D is y-down.
            Point parentOrigin = elt.TranslatePoint(new Point(0, elt.RenderSize.Height), _originalChild);

            RotateTransform3D rotToApply = new RotateTransform3D();
            rotToApply.CenterX = localMidpoint.X;
            rotToApply.CenterY = localMidpoint.Y; 
            rotToApply.Rotation = new AxisAngleRotation3D();

            TranslateTransform3D xltToApply = new TranslateTransform3D();
            ei._rotToApply = (AxisAngleRotation3D)rotToApply.Rotation;
            ei._xltToApply = xltToApply;

            Transform3DGroup t3dGroup = new Transform3DGroup();
            t3dGroup.Children.Add(new ScaleTransform3D(realRenderSize.Width, realRenderSize.Height, 
                (realRenderSize.Height + realRenderSize.Width) / 5.0));
            t3dGroup.Children.Add(rotToApply);
            
            t3dGroup.Children.Add(new TranslateTransform3D(parentOrigin.X, _vp3d.Height - parentOrigin.Y, zValue));
            t3dGroup.Children.Add(xltToApply);

            GeometryModel3D g3d = new GeometryModel3D();
            g3d.Material = new DiffuseMaterial(vb);
            g3d.Geometry = _planeMesh;
            g3d.Transform = t3dGroup;
            models.Add(new ModelWithZ(zValue, g3d));

            GeometryModel3D bg3d = new GeometryModel3D();
            bg3d.Material = new DiffuseMaterial(new SolidColorBrush(RandomColor())); 
            bg3d.Geometry = _backMesh;
            bg3d.Transform = t3dGroup;
            models.Add(new ModelWithZ(zValue, bg3d)); 
        }

        private Size TransformSize(GeneralTransform xf, Size size)
        {
            // Transform both upper-left and lower-right, and subtract to get the new size.
            return (Size)(xf.Transform((Point)size) - xf.Transform(new Point(0, 0))); 
        }

        private void EnsureParallaxElement()
        {
            if (_originalChild == null)
            {
                _originalChild = this.Child;
            }

            if (_vp3d == null)
            {
                _vp3d = new Viewport3D();
                _vp3d.ClipToBounds = false;
                _vp3d.Camera = CreateCamera(_originalChild, 0.05); 
                _vp3d.Width = _originalChild.RenderSize.Width;
                _vp3d.Height = _originalChild.RenderSize.Height;

                _mv3d = (ModelVisual3D)this.Resources["modelLights"];
                
                Model3DGroup m3dGroup = ((Model3DGroup)_mv3d.Content);
                _planeMesh = (Geometry3D)this.Resources["planeMesh"];
                _backMesh = (Geometry3D)this.Resources["backMesh"];

                _vp3d.Children.Add(_mv3d);

                Canvas c = new Canvas(); 
                this.Child = c;

                // We'll switch off between which of _originalChild and _vp3D is visible.
                c.Children.Add(_originalChild);
                c.Children.Add(_vp3d);

                // Figure out the elements we're interested in.
                Traverse(_originalChild, 0);

                List<ModelWithZ> models = new List<ModelWithZ>(_eltsInfo.Count * 2);

                // Let the max pos to neg z range be the average of the extents of x and y of the _originalChild;
                double maxZ = (_originalChild.RenderSize.Height + _originalChild.RenderSize.Width) / 4.0;
                foreach (EltInfo ei in _eltsInfo)
                {
                    AddElementAs3D(ei, maxZ, models);
                }

                // Sort so models ends up in z order, and insert in that order to avoid transparency issues.
                models.Sort();

                Model3DCollection coll = m3dGroup.Children;
                foreach (ModelWithZ mz in models)
                {
                    coll.Add(mz._model);
                }
                
                _initialElementBounds = m3dGroup.Bounds;
                ChooseVisibleItem();
            }
        }

        private void Traverse(UIElement elt, int level)
        {
            if (elt != null)
            {
                Panel p = elt as Panel;
                if (p != null)
                {
                    foreach (UIElement child in p.Children)
                    {
                        Traverse(child, level + 1);
                    }
                }
                else
                {
                    Decorator d = elt as Decorator;
                    if (d != null)
                    {
                        Traverse(d.Child, level + 1);
                    }
                    else
                    {
                        // Have a leaf node (at least as far as we're concerned).  Save it off.
                        _eltsInfo.Add(new EltInfo(elt));
                    }
                }
            }
        }

        private void ChooseVisibleItem()
        {
            UIElement hide, unhide;
            if (Math.Abs(GetAngle(this.Rotation)) < 0.5 && !_inTransition)
            {
                hide = _vp3d;
                unhide = _originalChild;
            }
            else
            {
                unhide = _vp3d;
                hide = _originalChild;
            }

            hide.Visibility = Visibility.Hidden;
            unhide.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Create a camera that looks down -Z, with up as Y, and positioned right halfway in X and Y on the element, 
        /// and back along Z the right distance based on the field-of-view is the same projected size as the 2D content
        /// that it's looking at.
        /// </summary>
        private Camera CreateCamera(UIElement elt, double fieldOfView)
        {
            Size size = elt.RenderSize;
            double fovInRadians = fieldOfView * (Math.PI / 180);
            double zValue = size.Width / Math.Tan(fovInRadians / 2) / 2;
            return new PerspectiveCamera(new Point3D(size.Width / 2, size.Height / 2, zValue),
                                         new Vector3D(0, 0, -1),
                                         _yAxis,
                                         fieldOfView);
        }

        // UGGH: Can't polymorhically get the angle of a Rotation3D.
        private double GetAngle(Rotation3D rot)
        {
            double angle;
            if (rot is AxisAngleRotation3D)
            {
                angle = ((AxisAngleRotation3D)rot).Angle;
            }
            else if (rot is QuaternionRotation3D)
            {
                angle = ((QuaternionRotation3D)rot).Quaternion.Angle;
            }
            else
            {
                throw new Exception("Unknown Rotation3D");
            }

            return angle;
        }

        /// <summary>
        /// Update the 3D rotation applied.  As the angle approaches 0, bring the Field of View
        /// of the camera towards 0 as well (making it more like an orthographic camera).  This
        /// is required so a face-on view will not show any signs of differences in z-values for
        /// the different elements.
        /// </summary>
        private void RotChanged(Rotation3D rot, Point3D relativeCenter)
        {
            if (_vp3d != null)
            {
                // as the angle approaches 0, bring the Field of View towards 0
                double absAngle = Math.Abs(GetAngle(rot));

                const double defaultFov = 65.0;
                const double angleThreshold = 90.0;
                const double multiplierWithinThreshold = defaultFov / angleThreshold;

                double fov;
                if (absAngle < angleThreshold)
                {
                    // x is the fraction of the angle within the threshold and within [0,1]
                    double x = absAngle / angleThreshold;

                    // makes descent into fov of 0 be non-linear, so less obvious jump.  Requirement for this
                    // calculation is that 0=>0, and 1=>1.
                    double factor = x * x;  
                    fov = factor * angleThreshold * multiplierWithinThreshold;
                    fov = Math.Max(fov, 0.01);
                }
                else
                {
                    fov = defaultFov;
                }

                Point3D absoluteCenter = new Point3D(
                    _initialElementBounds.X + _initialElementBounds.SizeX * relativeCenter.X,
                    _initialElementBounds.Y + _initialElementBounds.SizeY * relativeCenter.Y,
                    _initialElementBounds.Z + _initialElementBounds.SizeZ * relativeCenter.Z);

                _mv3d.Transform = new RotateTransform3D(rot, absoluteCenter);
                _vp3d.Camera = CreateCamera(_originalChild, fov);

                ChooseVisibleItem();
            }
        }


        // Private state data
        private class EltInfo
        {
            public EltInfo(UIElement elt) { _elt = elt; }
            public UIElement _elt;
            public Point3D _midPoint;
            public TranslateTransform3D _xltToApply;
            public AxisAngleRotation3D _rotToApply;
        }

        private class ModelWithZ : IComparable<ModelWithZ>
        {
            public ModelWithZ(double z, Model3D model)
            {
                _z = z;
                _model = model;
            }

            int IComparable<ModelWithZ>.CompareTo(ModelWithZ other)
            {
                if (_z < other._z)
                {
                    return -1;
                }
                else if (_z > other._z)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            public double _z;
            public Model3D _model;
        }

        private List<EltInfo> _eltsInfo = new List<EltInfo>();
        private Viewport3D _vp3d;
        private UIElement _originalChild;
        private Geometry3D _planeMesh;
        private Geometry3D _backMesh;
        private ModelVisual3D _mv3d;
        private Random _rand = new Random();
        private bool _inTransition = false;
        private AnimationTimeline _longestTimeline = null;
        private int _longestDurationMsec = 0;
        private Rect3D _initialElementBounds;
        static private Vector3D _yAxis = new Vector3D(0, 1, 0);
    }
}
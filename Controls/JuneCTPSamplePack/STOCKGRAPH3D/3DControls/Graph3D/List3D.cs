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
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Threading;
using System.Windows.Markup;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Resources;
using Mesh3DObjects;
using DemoDev;

using HitTestResult2D = System.Windows.Media.HitTestResult;

namespace DemoDev
{
    /// <summary>
    /// Interaction logic for List3D.xaml
    /// </summary>

    #region Public Enums

    public enum List3DItemStates
    {
        Unselected,
        Hover,
        Selected,
        OffScreen,
        OnScreen,
        AnimatingUnselected,
        AnimatingHover,
        AnimatingSelected,
        AnimatingOffScreen,
        AnimatingOnScreen,
    }
    #endregion

    public class List3D : ItemsControl
    {
        [DllImport("winmm.dll", EntryPoint = "PlaySound", CharSet = CharSet.Auto)]
        private static extern int PlaySound(String pszSound, int hmod, int flags);

        const int SND_ASYNC = 0x00000001;   /* play asynchronously */
        const int SND_FILENAME = 0x00020000;   /* name is file name */
        const int SND_NOWAIT = 0x00002000;   /* don't wait if the driver is busy */

        // Called Before properties are set
        public List3D()
        {
            ((INotifyCollectionChanged)this.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemsCollectionChanged);
          
            this.Initialized +=new EventHandler(OnInitialized);
            this.Loaded +=new RoutedEventHandler(OnLoaded);
            this.Background = Brushes.Red;
        }

        // Called after properties are set
        private void OnInitialized(object sender, EventArgs e)
        {
            // create the basic models
            InitializeModels();

            _List3DItems = new List3DItemCollection(); // init the items collection
        }

        // called after properties are set
        private void OnLoaded(object sender, EventArgs e)
        {
            InitializeViewport3D();

            // MouseInput Handler
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnPreviewLeftClick);
            //this.PreviewMouseMove += new MouseEventHandler(OnPreviewMouseMove);

            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(_MainViewport3D);
            _trackball.Enabled = true;
            
        }

        #region Overrides
        /*
        protected override Size MeasureOverride(Size sizeConstraint)
        {
            //base.MeasureOverride(sizeConstraint);

            //Size sizeMeasured = new Size(0, 0);
            //sizeMeasured.Width = sizeMeasured.Height = Math.Min(sizeConstraint.Width, sizeConstraint.Height);
            return sizeConstraint;
        }

        protected override Size ArrangeOverride(Size sizeArrange)
        {
            return sizeArrange;
        }
         * */
        
        #endregion

        #region Properties

        public List3DItem SelectedItem
        {
            get { return _Selected; }
        }

        Point3D _cameraPosition;
        public Point3D CameraPosition
        {
            get { return _cameraPosition; }
            set { _cameraPosition = value; }
        }

        Vector3D _cameraLookDirection;
        public Vector3D CameraLookDirection
        {
            get { return _cameraLookDirection; }
            set { _cameraLookDirection = value; }
        }

        private string ClientPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
            }
        }

        public bool CanHover
        {
            get { return _CanHover; }
            set { _CanHover = value; }
        }

        public bool CanSelect
        {
            get { return _CanSelect; }
            set { _CanSelect = value; }
        }

        public int Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        public int Rows
        {
            get { return _Rows; }
            set 
            {
                if (value <= 0)
                    return;

                _Rows = value; 
            }
        }

        double _FieldOfView = 40;
        public double FieldOfView
        {
            get { return _FieldOfView; }
            set { _FieldOfView = value; }
        }

        double _HaloXAxisAngle = 0;
        public double HaloXAxisAngle
        {
            get { return _HaloXAxisAngle; }
            set { _HaloXAxisAngle = value; }
        }

        double _HaloZAxisAngle = 0;
        public double HaloZAxisAngle
        {
            get { return _HaloZAxisAngle; }
            set { _HaloZAxisAngle = value; }
        }

        double _HaloYAxisAngle = 0;
        public double HaloYAxisAngle
        {
            get { return _HaloYAxisAngle; }
            set { _HaloYAxisAngle = value; }
        }

        Vector3D _HaloTranslation;
        public Vector3D HaloTranslation
        {
            get { return _HaloTranslation; }
            set { _HaloTranslation = value; }
        }

        Vector3D _HaloScale;
        public Vector3D HaloScale
        {
            get { return _HaloScale; }
            set { _HaloScale = value; }
        }

        int _SelectedDuration = _SINGLE_STEP_SELECT_DURATION;
        public int SelectedDuration
        {
            get { return _SelectedDuration; }
            set { _SelectedDuration = value; }
        }

        int _UnSelectedDuration = _SINGLE_STEP_UNSELECT_DURATION;
        public int UnSelectedDuration
        {
            get { return _UnSelectedDuration; }
            set { _UnSelectedDuration = value; }
        }

        int _ModelRotationDuration = _SINGLE_STEP_ROTATE_Y_DURATION;
        public int ModelRotationDuration
        {
            get { return _ModelRotationDuration; }
            set { _ModelRotationDuration = value; }
        }

        Brush _LabelXBrush;
        public Brush LabelXBrush
        {
            get { return _LabelXBrush; }
            set 
            {
                _LabelXBrush = value;
                DiffuseMaterial bm = new DiffuseMaterial(_LabelXBrush);

                // set the texture for the mesh
                ((GeometryModel3D)_ModelGridItems.Children[_GRIDLABEL_X]).Material = bm;
            }
        }

        Brush _LabelYBrush;
        public Brush LabelYBrush
        {
            get { return _LabelYBrush; }
            set
            {
                _LabelYBrush = value;

                DiffuseMaterial bm = new DiffuseMaterial(_LabelYBrush);

                // set the texture for the mesh
                ((GeometryModel3D)_ModelGridItems.Children[_GRIDLABEL_Y_LEFT]).Material = bm;
                ((GeometryModel3D)_ModelGridItems.Children[_GRIDLABEL_Y_RIGHT]).Material = bm;
            }
        }

        Brush _LabelZBrush;
        public Brush LabelZBrush
        {
            get { return _LabelZBrush; }
            set
            {
                _LabelZBrush = value;

                DiffuseMaterial bm = new DiffuseMaterial(_LabelZBrush);

                // set the texture for the mesh
                ((GeometryModel3D)_ModelGridItems.Children[_GRIDLABEL_Z]).Material = bm;
            }
        }

        Brush _GridBrush;
        public Brush GridBrush
        {
            get { return _GridBrush; }
            set
            {
                _GridBrush = value;

                DiffuseMaterial bm = new DiffuseMaterial(_GridBrush);

                // set the texture for the mesh
                ((GeometryModel3D)_ModelGridItems.Children[_GRID_X]).Material = bm;
                ((GeometryModel3D)_ModelGridItems.Children[_GRID_Y]).Material = bm;
                ((GeometryModel3D)_ModelGridItems.Children[_GRID_Z]).Material = bm;
            }
        }

        #endregion

        #region Event Handlers

        public delegate void SelectedEventHandler(object sender, EventArgs e);

        public event SelectedEventHandler ItemSelected;
        protected virtual void OnItemSelected(object o, EventArgs e)
        {
            if (ItemSelected != null)
                ItemSelected(o, e);
        }

		public event SelectedEventHandler ItemUnSelected;
		protected virtual void OnItemUnSelected(object o, EventArgs e)
		{
			if (ItemUnSelected != null)
				ItemUnSelected(o, e);
		}

        #endregion Event Handlers

        #region Events

        private void OnPreviewLeftClick(object sender, MouseButtonEventArgs e)
        {
            _MainViewport3D.Focus(); // get keyboard focus

            if (CanSelect == false)
                return;

            Point p = e.GetPosition(this);

            DoHitTest(_MainViewport3D, p);

            if ((_ciHitTest != null) && (_Selected == null))
            {
				_UsingKeyboard = false;
                SelectItem(_ciHitTest);
                OnItemSelected(_ciHitTest, new EventArgs());

                string sound = ClientPath + "select.wav";
                //PlaySound(sound, 0, SND_ASYNC | SND_NOWAIT | SND_FILENAME);
            }
            else if ((_ciHitTest != null) && (_ciHitTest == _Selected))
            {
				_UsingKeyboard = false;
                UnSelectToOnScreen(_ciHitTest);
            }
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (CanHover == false)
                return;

            Point p = e.GetPosition(this);

            DoHitTest(_MainViewport3D, p);

            if ((_ciHitTest != null) && (_ciHitTest != _Hover))
            {
                HoverItem(_ciHitTest);
            }
        }

        private void TranslateComplete(object sender, EventArgs e)
        {
            List3DItem item = sender as List3DItem;

            List3DItemStates state = (List3DItemStates)item.Status;

            if (state == List3DItemStates.AnimatingHover)
            {
                item.Status = (int)List3DItemStates.Hover;
            }

            if (state == List3DItemStates.AnimatingUnselected)
            {
                item.Status = (int)List3DItemStates.Unselected;
            }

            if (state == List3DItemStates.AnimatingSelected)
            {
                item.Status = (int)List3DItemStates.Selected;
            }

            if (state == List3DItemStates.AnimatingOffScreen)
            {
                item.Status = (int)List3DItemStates.OffScreen;
            }

            if (state == List3DItemStates.AnimatingOnScreen)
            {
                item.Status = (int)List3DItemStates.Unselected;
            }


        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(sender, args);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Remove(sender, args);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Reset(sender, args);
                    break;
            }
        }

        #endregion

        #region Public Methods

		public void SelectNextItem()
		{
            if (_List3DItems.Count <= 1)
                return;

            int index = 0;
            List3DItem next;

            if (_Selected != null)
            {
                index = FindItemIndex(_Selected.ID);

                if ((index + 1) >= (_List3DItems.Count))
                    index = 0;
                else
                    index++;

            }

            next = _List3DItems[index] as List3DItem;

            _UsingKeyboard = true;
            UnSelectToOnScreen(_Selected);
            SelectItem(next);
            OnItemSelected(_Selected, new EventArgs());
		}

		public void SelectPreviousItem()
		{
            if (_List3DItems.Count <= 1)
                return;

            int index = 0;
            List3DItem next;

            if (_Selected != null)
            {
                index = FindItemIndex(_Selected.ID);

                if ((index - 1) < 0)
                    index = _List3DItems.Count - 1;
                else
                    index--;
            }

            next = _List3DItems[index] as List3DItem;

            _UsingKeyboard = true;
            UnSelectToOnScreen(_Selected);
            SelectItem(next);
            OnItemSelected(_Selected, new EventArgs());
		}

        public void Select(List3DItem li)
        {
            if (li == _Selected)
                return;

            _UsingKeyboard = true;
            UnSelectToOnScreen(_Selected);
            SelectItem(li);
            OnItemSelected(_Selected, new EventArgs());
        }

        public List3DItem FindList3DItem(string id)
        {
            List3DItem ciItem = null;

            // find the model
            foreach (List3DItem ci in _List3DItems)
            {
                if (ci.ID == id)
                {
                    ciItem = ci;
                    break;
                }
            }

            return ciItem;
        }

        public void FlipSelectedItem()
        {
            Vector3D v = _Selected.Angle;
            Position3D p3D = _Selected.FrontPosition;

            if (_Selected != null)
            {
                if (v.Y == 225)
                    _Selected.AnimateRotate(new Vector3D(p3D.RotationAngle.X, p3D.RotationAngle.Y, p3D.RotationAngle.Z));
                else
                    _Selected.AnimateRotate(new Vector3D(p3D.RotationAngle.X, 225, p3D.RotationAngle.Z));                
            }
        }

        public void RotateLeft()
        {
            if (_TrackModelItemsRotating == true)
                return;

            RotateModelAxisXReset();

            Transform3DGroup t3dg = _ModelGraphItems.Transform as Transform3DGroup;
            RotateTransform3D _GroupRotateTransform = t3dg.Children[1] as RotateTransform3D;

            //double angle = _GroupRotateTransform.Rotation.Angle - (double)(360.0 / ((double)Columns * 2.0525));
            double angle = (_GroupRotateTransform.Rotation as AxisAngleRotation3D).Angle - (double)(360.0 / ((double)Columns));

            _TrackModelItemsRotating = true;

            if (_KeyUpEvent == true)
            {
                ModelRotationDuration = _SINGLE_STEP_ROTATE_Y_DURATION;
                UnSelectedDuration = _SINGLE_STEP_UNSELECT_DURATION;
                SelectedDuration = _SINGLE_STEP_SELECT_DURATION;
                _KeyUpEvent = false;
            }
            else
            {
                ModelRotationDuration = _MULTI_STEP_ROTATE_Y_DURATION;
                UnSelectedDuration = _MULTI_STEP_UNSELECT_DURATION;
                SelectedDuration = _MULTI_STEP_SELECT_DURATION;
            }

            RotateModel(_GroupRotateTransform, angle);

            _AnimatedColumnLeft = _CurrentColumn - _LEFT_COLUMN_EDGE;
            if (_AnimatedColumnLeft < 0)
                _AnimatedColumnLeft = Columns + _AnimatedColumnLeft;

            AnimateColumnFromBackToDefault(_AnimatedColumnLeft);


            if ((_CurrentColumn - 1) < 0)
                _CurrentColumn = (Columns - 1);
            else
                _CurrentColumn--;

            _AnimatedColumnRight = _CurrentColumn + _RIGHT_COLUMN_EDGE;
            if (_AnimatedColumnRight >= Columns)
                _AnimatedColumnRight = _AnimatedColumnRight - Columns;

            AnimateColumnFromDefaultToBack(_AnimatedColumnRight);


            int offset = (_CurrentColumn * Rows) + _CurrentRow;
            List3DItem next = _List3DItems[offset] as List3DItem;

            _UsingKeyboard = true;
            UnSelectToOnScreen(_Selected);
            SelectItem(next);
            _Floater3D.MainImage = next.MainImage;
            OnItemSelected(_Selected, new EventArgs());
        }

        public void RotateRight()
        {
            if (_TrackModelItemsRotating == true)
                return;

            RotateModelAxisXReset();

            Transform3DGroup t3dg = _ModelGraphItems.Transform as Transform3DGroup;
            RotateTransform3D _GroupRotateTransform = t3dg.Children[1] as RotateTransform3D;

            //double angle = _GroupRotateTransform.Rotation.Angle + (double)(360.0 / ((double)Columns * 2.0525)); ;
            double angle = (_GroupRotateTransform.Rotation as AxisAngleRotation3D).Angle + (double)(360.0 / ((double)Columns));

            _TrackModelItemsRotating = true;

            if (_KeyUpEvent == true)
            {
                ModelRotationDuration = _SINGLE_STEP_ROTATE_Y_DURATION;
                UnSelectedDuration = _SINGLE_STEP_UNSELECT_DURATION;
                SelectedDuration = _SINGLE_STEP_SELECT_DURATION;
                _KeyUpEvent = false;
            }
            else
            {
                ModelRotationDuration = _MULTI_STEP_ROTATE_Y_DURATION;
                UnSelectedDuration = _MULTI_STEP_UNSELECT_DURATION;
                SelectedDuration = _MULTI_STEP_SELECT_DURATION;
            }

            RotateModel(_GroupRotateTransform, angle);


            _AnimatedColumnRight = _CurrentColumn + _RIGHT_COLUMN_EDGE;
            if (_AnimatedColumnRight >= Columns)
                _AnimatedColumnRight = _AnimatedColumnRight - Columns;

            AnimateColumnFromBackToDefault(_AnimatedColumnRight);


            if ((_CurrentColumn + 1) > (Columns - 1))
                _CurrentColumn = 0;
            else
                _CurrentColumn++;


            _AnimatedColumnLeft = _CurrentColumn - _LEFT_COLUMN_EDGE;
            if (_AnimatedColumnLeft < 0)
                _AnimatedColumnLeft = Columns + _AnimatedColumnLeft;

            AnimateColumnFromDefaultToBack(_AnimatedColumnLeft);


            int offset = (_CurrentColumn * Rows) + _CurrentRow;
            List3DItem next = _List3DItems[offset] as List3DItem;

            _UsingKeyboard = true;
            UnSelectToOnScreen(_Selected);
            SelectItem(next);
            _Floater3D.MainImage = next.MainImage;
            OnItemSelected(_Selected, new EventArgs());
        }

        public void KeyUpEvent()
        {
            if (_Floater3D.Status == (int)List3DItemStates.Unselected)
            {
                RotateModelAxisX();
            }

            ModelRotationDuration = _SINGLE_STEP_ROTATE_Y_DURATION;
            UnSelectedDuration = _SINGLE_STEP_UNSELECT_DURATION;
            SelectedDuration = _SINGLE_STEP_SELECT_DURATION;
            _KeyUpEvent = true;
            EnsureUnselectedItemState();
        }

        public void CreateHalos()
        {
            Relayout();

            RotateModelAxisX(); // setup animations for next call
            RotateModelAxisX();
        }

        #endregion

        #region Private Methods

        private void DebugTest()
        {
            foreach (List3DItem item in _List3DItems)
            {
                string s = item.DefaultPosition.Translation.ToString();
                DebugHelp3D.Trace.Message (s);
            }
        }

        #region HitTesting

        public bool DoHitTest(Visual target, Point p)
        {
            _ModelHitTest = null;

            //loop through all the hits
            //VisualTreeHelper.HitTest(target, null, new HitTestResultCallback(HTResult), new PointHitTestParameters(p));

            // You can also target a Visual3D rather than the entire viewport3D
            // VisualOperations.HitTest(Visual3D, null, new HitTestResultDelegate(HTResult), new RayHitTestParameters(origin - like the camera, direction - could be directional vector based on camera position and lookat point));

            // returns closest hit
            HitTestResult result = VisualTreeHelper.HitTest(target, p);
            RayHitTestResult ray = result as RayHitTestResult;
            _ModelHitTest = ray.ModelHit;

            if (_ModelHitTest == null)
                return false;

            _ciHitTest = FindList3DItemPositionFromGeometry(_ModelHitTest);

            if (_ciHitTest == null)
                return false;

            return true;
        }

        /*
        public HitTestResultBehavior HTResult(HitTestResult result)
        {
            RayHitTestResult ray = result as RayHitTestResult;

            if (ray == null)
                return HitTestResultBehavior.Continue;

            if (_ModelHitTest == null)
            {
                _HitTestDistance = ray.DistanceToRayOrigin;
                _ModelHitTest = ray.ModelHit;
            }
            else
            {
                if (ray.DistanceToRayOrigin < _HitTestDistance)
                    _ModelHitTest = ray.ModelHit;
            }

            return HitTestResultBehavior.Continue;
        }
        */
        private List3DItem FindList3DItemPositionFromGeometry(Model3D model)
        {
            List3DItem retList3DItem = null;

            if (model == null)
                return retList3DItem;

            foreach (List3DItem li in _List3DItems)
            {
                foreach (GeometryModel3D gm3d in li.ItemGroup.Children)
                {
                    if (model == gm3d)
                    {
                        retList3DItem = li;
                        break;
                    }
                }
            }

            return retList3DItem;
        }

        #endregion

        #region ItemsCollection Methods
     
        private void Reset(object sender, NotifyCollectionChangedEventArgs args)
        {
			if ((_List3DItems != null) && (_ModelGraphItems != null))
			{
                _List3DItems.Clear();
                _ModelGraphItems.Children.Clear();
            }

        }

        private void Remove(object sender, NotifyCollectionChangedEventArgs args)
        {
			if (args == null) 
				return;
            /*
            object o = args.ChangedItems[0] as object;

            if (o == null)
                return;
             */
        }

        private void Add(object sender, NotifyCollectionChangedEventArgs args)
        {

            if ((args.NewItems == null) || (args.NewItems.Count == 0))
                return;

            _List3DItems = args.NewItems[0] as List3DItemCollection;

            if (_List3DItems == null)
                return;

            Relayout();
        }

        #endregion ItemsCollection Methods

        #region Animations

        private void RotateModel(RotateTransform3D model_rt3d, double rotate)
        {

            // setup new animationusing AnimationClockCollection
            RotateTransform3D rt3D = model_rt3d.CloneCurrentValue();
            AxisAngleRotation3D r3d = rt3D.Rotation as AxisAngleRotation3D;

            DoubleAnimation qaCopy = new DoubleAnimation();

            double angle = 0;
            if (rt3D.Rotation != null)
                angle = (rt3D.Rotation as AxisAngleRotation3D).Angle;

            qaCopy.From = angle;
            qaCopy.To = rotate;

            qaCopy.FillBehavior = FillBehavior.HoldEnd;
            //qaCopy.AccelerationRatio = 0.5;
            //qaCopy.DecelerationRatio = 0.5;
            qaCopy.Duration = new TimeSpan(0, 0, 0, 0, ModelRotationDuration);
            qaCopy.Freeze();
            AnimationClock ac = (AnimationClock)qaCopy.CreateClock();
            ac.CurrentStateInvalidated += new EventHandler(OnRotateEnded);
            r3d.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, ac, HandoffBehavior.SnapshotAndReplace);

        }


        private void OnRotateEnded(object sender, EventArgs args)
        {   
            Clock clock = sender as Clock;

            if (clock == null)
                return;

            
            if (clock.CurrentState == ClockState.Filling)
            {
                _TrackModelItemsRotating = false;
            }
   
        }

        private void RotateModelAxisX()
        {

            Transform3DGroup t3dg = _ModelMainItems.Transform as Transform3DGroup;
            RotateTransform3D _GroupRotateTransform = t3dg.Children[2] as RotateTransform3D;

            // setup new animationusing AnimationClockCollection
            RotateTransform3D rt3D = _GroupRotateTransform.CloneCurrentValue();
            AxisAngleRotation3D r3d = rt3D.Rotation as AxisAngleRotation3D;


            DoubleAnimation qaCopy = new DoubleAnimation();

            double angle = 0;
            if (rt3D.Rotation != null)
                angle = (rt3D.Rotation as AxisAngleRotation3D).Angle;

            qaCopy.From = angle;
            qaCopy.To = 10;

            qaCopy.FillBehavior = FillBehavior.HoldEnd;
            qaCopy.AccelerationRatio = 0.5;
            qaCopy.DecelerationRatio = 0.5;
            qaCopy.RepeatBehavior = RepeatBehavior.Forever;
            qaCopy.AutoReverse = true;
            qaCopy.Duration = new TimeSpan(0, 0, 0, 0, 10000);
            qaCopy.Freeze();
            AnimationClock ac = (AnimationClock)qaCopy.CreateClock();
            ac.CurrentStateInvalidated += new EventHandler(OnRotateEndedAxisX);
            r3d.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, ac, HandoffBehavior.SnapshotAndReplace);

        }


        private void RotateModelAxisXReset()
        {

            Transform3DGroup t3dg = _ModelMainItems.Transform as Transform3DGroup;
            RotateTransform3D _GroupRotateTransform = t3dg.Children[2] as RotateTransform3D;

            // setup new animationusing AnimationClockCollection
            RotateTransform3D rt3D = _GroupRotateTransform.CloneCurrentValue();
            AxisAngleRotation3D r3d = rt3D.Rotation as AxisAngleRotation3D;

            DoubleAnimation qaCopy = new DoubleAnimation();

            double angle = 0;
            if (rt3D.Rotation != null)
                angle = (rt3D.Rotation as AxisAngleRotation3D).Angle;

            qaCopy.From = angle;
            qaCopy.To = 0;

            qaCopy.FillBehavior = FillBehavior.HoldEnd;
            qaCopy.AccelerationRatio = 0.0;
            qaCopy.DecelerationRatio = 1.0;
            qaCopy.Duration = new TimeSpan(0, 0, 0, 0, 300);
            qaCopy.Freeze();
            AnimationClock ac = (AnimationClock)qaCopy.CreateClock();
            ac.CurrentStateInvalidated += new EventHandler(OnRotateEndedAxisX);
            r3d.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, ac, HandoffBehavior.SnapshotAndReplace);
        }

        private void OnRotateEndedAxisX(object sender, EventArgs args)
        {
            try
            {
                Clock clock = sender as Clock;

                if (clock == null)
                    return;


                if (clock.CurrentState == ClockState.Filling)
                {

                }
            }
            catch (Exception ex)
            {
                DebugHelp3D.Trace.Message(ex.ToString());
            }

        }


        #endregion

        private void Relayout()
        {
            double maxHeight = 1;
            double maxWidth = 0;
            double maxDepth  = 1;

            if (_List3DItems.Count == 0)
                return;

            _ModelGraphItems.Children.Clear();
            
            // compute the max width, depth and height to fit the graph with a 10 x 10 x 10 space
            foreach (List3DItem li in _List3DItems)
            {
                maxHeight = Math.Max(maxHeight, li.Scale.Y);
                //maxHeight = Math.Max(maxHeight, li.Offset.Y);
                maxWidth = Math.Max(maxWidth, li.Offset.X);
                maxDepth = Math.Max(maxDepth, li.Offset.Z);
            }

            maxWidth++; // start width is zero based
            maxDepth++; // start width is zero based
            //maxHeight++; // start width is zero based
            
            // Computer ratios for raw data to fit inside the Graph
            double ratioHeight = _GRAPH_SIZERATIO / maxHeight;
            double ratioMaxWidth = _GRAPH_SIZERATIO / maxWidth;
            double ratioMaxDepth  = _GRAPH_SIZERATIO / maxDepth;

            Transform3DGroup t3dG = _ModelGraphItems.Transform as Transform3DGroup;

            ScaleTransform3D st3d = t3dG.Children[0] as ScaleTransform3D;
            Vector3D ratioScale = new Vector3D(ratioMaxWidth, ratioHeight, ratioMaxDepth);
            st3d.ScaleX = ratioScale.X;
            st3d.ScaleY = ratioScale.Y;
            st3d.ScaleZ = ratioScale.Z;

            TranslateTransform3D tt3d = t3dG.Children[4] as TranslateTransform3D;
            double xTransform = (ratioMaxWidth / 2); 
            double yTransform = 0;

            Vector3D transform = new Vector3D (tt3d.OffsetX, tt3d.OffsetY, tt3d.OffsetZ) ;
            transform.Y = yTransform;
            transform.X = xTransform;
            tt3d.OffsetX = transform.X;
            tt3d.OffsetY = transform.Y;
            tt3d.OffsetZ = transform.Z;
            
            // Layout the graph
            foreach (List3DItem li in _List3DItems)
            {
                _ModelGraphItems.Children.Add(li.ItemGroup);

                Vector3D vOffsetHeight = new Vector3D(li.Offset.X, li.Offset.Y, li.Offset.Z);
                vOffsetHeight.Y += (li.Scale.Y / 2.0);
                li.AnimateTranslate(vOffsetHeight);
            }

        }

        private void AnimateBetweenColumnsToDefault (int columnLeft, int columnRight)
        {
            int start = columnLeft * Rows;
            int end = columnRight * Rows;
            List3DItem li;

            for (int i=start; i < end; i++)
            {
                li = _List3DItems[i] as List3DItem;
                li.AnimatePostion(li.DefaultPosition);
            }

        }

        private void AnimateColumnFromDefaultToBack(int column)
        {
            int i = column * Rows;
            int end = i + Rows;
            List3DItem li;

            for (; i < end; i++)
            {
                li = _List3DItems[i] as List3DItem;
                li.AnimatePostion(li.BackPosition);
            }
        }

        private void AnimateColumnFromBackToDefault(int column)
        {
            int i = column * Rows;
            int end = i + Rows;
            List3DItem li;

            for (; i < end; i++)
            {
                li = _List3DItems[i] as List3DItem;
                li.AnimatePostion(li.DefaultPosition);
            }
        }

        private void EnsureUnselectedItemState()
        {

            foreach (List3DItem li in _List3DItems)
            {
                if (li.Status == (int)List3DItemStates.Unselected)
                {
                    Vector3D v = li.Offset;
                    if (v == li.FrontPosition.Translation)
                    {
                        li.AnimatePostion(li.DefaultPosition);
                    }
                }
            }

        }

        private void SetBrushToNextLayoutPostion(List3DItem li)
        {
            if ((li == null) || (this.Items.Count == 0))
                return;

            if (_LayoutPosition >= this.Items.Count)
                _LayoutPosition = 0;

            li.MainImage = this.Items[_LayoutPosition] as Brush;

            _LayoutPosition++;
        }

        private FrameworkElement FindViewport3D(Visual parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                Visual visual = VisualTreeHelper.GetChild(parent, i) as Visual;

                if ((visual is FrameworkElement) && (visual is Viewport3D))
                    return (visual as FrameworkElement);
                else
                {
                    FrameworkElement result = FindViewport3D(visual);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }


        private void SelectItem(List3DItem select)
        {
            Vector3D current, next;

            return;
            /*
            // find the model
            foreach (List3DItem item in _List3DItems)
            {
                if (select == item)
                    continue;

                item.Status = (int)List3DItemStates.AnimatingOffScreen;
                item.AnimatePostion(item.BackPosition);
            }

            _Selected = select;
            select.Status = (int)List3DItemStates.AnimatingSelected;
            select.Duration = SelectedDuration;
            //select.VideoModel = true;
            select.AnimatePostion(select.FrontPosition);
             * */
        }

        private void UnSelectToOnScreen(List3DItem select)
        {
            return;
            /*
            // find the model
            foreach (List3DItem item in _List3DItems)
            {
                if (select == item)
                    continue;

                if (_UsingKeyboard == false)
                    item.AnimatePostion(item.DefaultPosition);

                item.Status = (int)List3DItemStates.AnimatingOnScreen;
            }

            if (_Selected == null)
                return;

            _Selected = null;
            select.Duration = UnSelectedDuration;
            select.Status = (int)List3DItemStates.AnimatingUnselected;

            if (_UsingKeyboard == false)
                select.AnimatePostion(select.DefaultPosition);
            else
                select.AnimatePostion(select.BackPosition);

            // trigger event
            OnItemUnSelected(select, new EventArgs());
             */

        }

        private void HoverItem(List3DItem hover)
        {
            Vector3D current, next;

            if ((hover.Status == (int)List3DItemStates.AnimatingOffScreen) || (hover.Status == (int)List3DItemStates.OffScreen) ||
                (hover.Status == (int)List3DItemStates.AnimatingOnScreen))
            {
                return;
            }

            // find the model
            foreach (List3DItem item in _List3DItems)
            {
                if (hover == item)
                    continue;

                List3DItemStates state = (List3DItemStates)item.Status;

                switch (state)
                {
                    case List3DItemStates.AnimatingHover:
                    case List3DItemStates.Hover:
                        current = item.Offset;
                        next = new Vector3D(current.X, current.Y, current.Z);
                        next.Z = 0;

                        item.AnimateTranslate(next);
                        item.Status = (int)List3DItemStates.AnimatingUnselected;
                        break;
                    default:
                        continue;
                        break;

                } // end switch
            } // end foreach

            // hover the item
            if ((hover.Status == (int)List3DItemStates.Selected) || (hover.Status == (int)List3DItemStates.AnimatingSelected))
            {
                _Hover = null;
            }
            else
            {
                current = hover.Offset;
                next = new Vector3D(current.X, current.Y, current.Z);
                next.Z = 1;

                hover.Status = (int)List3DItemStates.AnimatingHover;
                _Hover = hover;
                hover.AnimateTranslate(next);

                string sound = ClientPath + "hover.wav";
              //  PlaySound(sound, 0, SND_ASYNC | SND_NOWAIT | SND_FILENAME);
            }
        }

        private int FindItemIndex(string id)
        {
            int index = -1;

            // find the model
            int i = 0;
            foreach (List3DItem ci in _List3DItems)
            {
                if (ci.ID == id)
                {
                    index = i;
                    break;
                }
                i++;
            }

            return index;
        }

        private void InitializeModels()
        {
            // Create camera
            //
            Vector3D cameraUp = new Vector3D(0, 1, 0);
            _MainCamera = new PerspectiveCamera(CameraPosition, CameraLookDirection, cameraUp, /* fieldOfView (degrees) = */ FieldOfView);
            _MainCamera.NearPlaneDistance = 0.25;
            _MainCamera.FarPlaneDistance = 50;

            // create Main Model group - light and transforms for all sub model groups go here
            //
            _MainGroupVisual3D = new ModelVisual3D();
            _MainGroup = new Model3DGroup();

            // Create default Transform collection

            // Add transform collection to the _MainGroup
            ScaleTransform3D GroupScaleTransform = new ScaleTransform3D(new Vector3D(1, 1, 1));
            RotateTransform3D GroupRotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0), new Point3D(0, 0, 0));
            TranslateTransform3D GroupTranslateTransform = new TranslateTransform3D(new Vector3D(0, 0, 0));

            // setup Transform collection
            Transform3DCollection tcollection = new Transform3DCollection();
            tcollection.Add(GroupScaleTransform);
            tcollection.Add(GroupRotateTransform);
            tcollection.Add(GroupTranslateTransform);

            // setup group transform
            Transform3DGroup tGroupDefault = new Transform3DGroup();
            tGroupDefault.Children = tcollection;

            _MainGroupVisual3D.Transform = tGroupDefault;
            _MainGroup.Transform = tGroupDefault.Clone();

            // Create sub model group [0] for the light
            //
            _ModelLights = new Model3DGroup();

            AmbientLight light1 = new AmbientLight(Colors.White);
            DirectionalLight light2 = new DirectionalLight(Colors.LightGray, new Vector3D(-1, -1, -1));
            _ModelLights.Transform = tGroupDefault.Clone();
            _ModelLights.Children.Add(light1);
            _ModelLights.Children.Add(light2);
            _MainGroup.Children.Add(_ModelLights);

            // Create sub model group [1] for the list items
            //
            _ModelMainItems = new Model3DGroup();

            ScaleTransform3D ModelItemScaleTransform = new ScaleTransform3D(HaloScale);
            RotateTransform3D ModelItemRotateTransformY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HaloYAxisAngle), new Point3D(0, 0, 0));
            RotateTransform3D ModelItemRotateTransformX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), HaloXAxisAngle), new Point3D(0, 0, 0));
            RotateTransform3D ModelItemRotateTransformZ = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), HaloZAxisAngle), new Point3D(0, 0, 0));
            TranslateTransform3D ModelItemTranslateTransform = new TranslateTransform3D(HaloTranslation);

            Transform3DCollection ModelItemtcollection = new Transform3DCollection();
            ModelItemtcollection.Add(ModelItemScaleTransform);
            ModelItemtcollection.Add(ModelItemRotateTransformY);
            ModelItemtcollection.Add(ModelItemRotateTransformX);
            ModelItemtcollection.Add(ModelItemRotateTransformZ);
            ModelItemtcollection.Add(ModelItemTranslateTransform);

            Transform3DGroup ModelItemt3d = new Transform3DGroup();
            ModelItemt3d.Children = ModelItemtcollection;

            _ModelMainItems.Transform = ModelItemt3d;

            // add Grid items
            ComponentResourceKey crk = new ComponentResourceKey(typeof(ControlResource), "StockGraph3DGrid");
            _ModelGridItems = Application.Current.FindResource(crk) as Model3DGroup;
            _ModelGridItems = _ModelGridItems.Clone(); // get unfrozen copy because we are going to manipulate the resource
            _ModelMainItems.Children.Add(_ModelGridItems);

            // add Graph items
            _ModelGraphItems = new Model3DGroup();
            ModelItemScaleTransform = new ScaleTransform3D(new Vector3D(1, 1, 1));
            ModelItemRotateTransformY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0), new Point3D(0, 0, 0));
            ModelItemRotateTransformX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0), new Point3D(0, 0, 0));
            ModelItemRotateTransformZ = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0), new Point3D(0, 0, 0));
            ModelItemTranslateTransform = new TranslateTransform3D(new Vector3D(0, 0, 0));

            Transform3DCollection ModelGraphItemtcollection = new Transform3DCollection();
            ModelGraphItemtcollection.Add(ModelItemScaleTransform);
            ModelGraphItemtcollection.Add(ModelItemRotateTransformY);
            ModelGraphItemtcollection.Add(ModelItemRotateTransformX);
            ModelGraphItemtcollection.Add(ModelItemRotateTransformZ);
            ModelGraphItemtcollection.Add(ModelItemTranslateTransform);

            Transform3DGroup ModelGraphItemt3d = new Transform3DGroup();
            ModelGraphItemt3d.Children = ModelGraphItemtcollection;

            _ModelGraphItems.Transform = ModelGraphItemt3d;


            _ModelMainItems.Children.Add(_ModelGraphItems);

            _MainGroup.Children.Add(_ModelMainItems);
            _MainGroupVisual3D.Content = _MainGroup;

        }

        private void InitializeViewport3D()
        {
            FrameworkElement f = this as FrameworkElement;

            FrameworkElement viewport3DElement = FindViewport3D(f); // since the Viewport3D is in the style, we need to find it.
            _MainViewport3D = viewport3DElement as Viewport3D;

            if (_MainViewport3D != null)
            {
                _MainViewport3D.Camera = _MainCamera;
                _MainViewport3D.Children.Add(_MainGroupVisual3D);

                 ComponentResourceKey crk = new ComponentResourceKey(typeof(ControlResource), "GridGraphBrush");
                GridBrush = Application.Current.FindResource(crk) as Brush;
            }

        }

        #endregion

        #region Globals

	    // 3D elements
        Viewport3D _MainViewport3D;
        RotateTransform3D _ModelsRotateTransform;
        PerspectiveCamera _MainCamera;
        ModelVisual3D _MainGroupVisual3D;
        Model3DGroup _MainGroup;

        Model3DGroup _ModelLights;
        Model3DGroup _ModelMainItems;
        Model3DGroup _ModelGridItems;
        Model3DGroup _ModelGraphItems;

        List3DItem _Floater3D;

        private Trackball _trackball;

        // Properties
        ImageBrush _ModelGridImage;
        ImageBrush _TransparentImage;
        Vector3D _Size;
        ImageBrush _ModelLabelXImage;
        ImageBrush _ModelLabelYImage;
        ImageBrush _ModelLabelZImage;
        bool _RotateModels = false;
        bool _CanHover = true;
        bool _CanSelect = true;
		bool _UsingKeyboard = false;
        int _Columns = 32;
        int _Rows = 1;
        int _AnimatedColumnLeft = 26; //
        int _AnimatedColumnRight = 22; //
        int _CurrentColumn = 0;
        int _CurrentRow = 3;

        // List3D items
        List3DItemCollection _List3DItems;
        int _PreviousList3DItemCount = 0;

        // 3D hittesting
        List3DItem _ciHitTest;
        Model3D _ModelHitTest = null;
        double _HitTestDistance = 0;

        //List Item tracking
        List3DItem _Hover=null;
        List3DItem _Selected = null;
		Vector3D _SelectedSave;

        // Layout tracking
        int _LayoutPosition = 0;
        double _RotationTracking = 0; //BUGBUG - animation is setting angle to abs() value - workingaround
        bool _TrackModelItemsRotating = false;
        bool _KeyUpEvent = true;

        // Const
        const int _RIGHT_COLUMN_EDGE = 11;
        const int _LEFT_COLUMN_EDGE = 7;
        const double _SIZERATIO = 10; // size of the 3D grid in 3D space
        const int _SINGLE_STEP_ROTATE_Y_DURATION = 330;
        const int _SINGLE_STEP_UNSELECT_DURATION = 200;
        const int _SINGLE_STEP_SELECT_DURATION = 100;
        const int _MULTI_STEP_ROTATE_Y_DURATION = 1;
        const int _MULTI_STEP_UNSELECT_DURATION = 300;
        const int _MULTI_STEP_SELECT_DURATION = 1;
        const int _GRIDLABEL_Y_LEFT = 3;
        const int _GRIDLABEL_Y_RIGHT = 2;
        const int _GRIDLABEL_X = 1;
        const int _GRIDLABEL_Z = 0;
        const int _GRID_X = 4;
        const int _GRID_Y = 5;
        const int _GRID_Z = 6;

        // Const
        const double _GRAPH_SIZERATIO = 10; // size of the 3D grid in 3D space

        #endregion

    }

    namespace DebugHelp3D
    {
        using System.Runtime.InteropServices;
        using System.Text;
        using System.IO;
        // Trace Syntax:
        //		DebugHelp.Trace.Message(a string);
        //
        // Launch "start DBMon" to see the output
        public class Trace
        {
            [DllImport("kernel32.dll", EntryPoint = "OutputDebugStringW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern void OutputDebugString(string msg);

            public static void Message(string msg)
            {
                OutputDebugString(msg + "\n");
            }
        }

        public class FileTest
        {
            private static string filePath = System.Environment.GetEnvironmentVariable("TMP").ToString() + @"\log.txt";

            public static void WriteFile(string input)
            {
                FileInfo logFile = new FileInfo(filePath);

                if (logFile.Exists)
                {
                    if (logFile.Length >= 1000000)
                        File.Delete(filePath);
                }

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter w = new StreamWriter(fs);

                w.BaseStream.Seek(0, SeekOrigin.End);
                w.Write(input);
                w.Flush();
                w.Close();
            }

            public static string ReadFile()
            {
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StringBuilder output = new StringBuilder();

                output.Length = 0;

                StreamReader r = new StreamReader(fs);

                r.BaseStream.Seek(0, SeekOrigin.Begin);
                while (r.Peek() > -1)
                {
                    output.Append(r.ReadLine() + "\n");
                }

                r.Close();
                return output.ToString();
            }
        }
    }



}
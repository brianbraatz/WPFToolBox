using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;


namespace Common
{
    public enum CarouselLabelStates
    {
        Right,
        Center,
        Left
    }

    public class CarouselItem
    {
        public CarouselItem(string ID, Brush brush, object Object1)
        {
            _Brush = brush;
            _ID = ID;
            _Object = Object1;
            //_BrushCategory = brushCategory;
            //_CategoryID = categoryID;
        }

        public Brush Brush
        {
            get { return _Brush; }
            set { _Brush = value; }
        }

        public Brush BrushCategory
        {
            get { return _BrushCategory; }
            set { _BrushCategory = value; }
        }
        
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string CategoryID
        {
            get { return _CategoryID; }
            set { _CategoryID = value; }
        }

        public object Object1
        {
            get { return _Object; }
            set { _Object = value; }
        }

        public Point MouseMovePoint
        {
            get { return _MouseMovePoint; }
            set { _MouseMovePoint = value; }
        }

        Brush _Brush;
        Brush _BrushCategory;
        string _ID;
        string _CategoryID;
        object _Object;
        Point _MouseMovePoint;
    }

    public partial class Carousel : Grid
    {
        public Carousel()
        {
            InitializeComponent();

            _CarouselItemCollection = new ObservableCollection<CarouselItem>();
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // MouseInput Handler
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnPreviewLeftClick);
            this.PreviewMouseMove +=new MouseEventHandler(OnPreviewMouseMove);

            // 3D globals
            _carouselVisual3D = myViewport3D.Children[0] as ModelVisual3D;

            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport3D);
            _trackball.Enabled = true;

        }


        #region Properties

        public ObservableCollection<CarouselItem> CarouselItemCollection
        {
            get { return _CarouselItemCollection; }
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

        public event SelectedEventHandler ItemMouseMoveEnter;
        protected virtual void OnItemMouseMoveEnter(object o, EventArgs e)
        {
            if (ItemMouseMoveEnter != null)
                ItemMouseMoveEnter(o, e);
        }

        public event SelectedEventHandler ItemMouseMoveLeave;
        protected virtual void OnItemMouseMoveLeave(object o, EventArgs e)
        {
            if (ItemMouseMoveLeave != null)
                ItemMouseMoveLeave(o, e);
        }


        #endregion

        #region Events

        #region Mouse 

        private void OnPreviewLeftClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);

            DoHitTest(myViewport3D, p);

            if (_SelectedHitTest == -1)
                return;

            AnimateToPosition(_SelectedHitTest);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);

            DoHitTest(myViewport3D, p);

            if (_selectedMouseMovePlane == _SelectedHitTest)
                return;

            _selectedMouseMovePlane = _SelectedHitTest;

            if (_SelectedHitTest == -1)
            {
                string s = "mouse move Leave";
                DebugHelp3D.Trace.Message(s);

                OnItemMouseMoveLeave(null, new EventArgs());
            }
            else
            {
                int offset = FindOffsetFromPosition(_selectedMouseMovePlane);

                int pos = _currentCarouselItem;
                for (int i = 1; i <= Math.Abs(offset); i++)
                {
                    if (offset < 0)
                        pos = NextCarouselItem(pos);
                    else
                        pos = PreviousCarouselItem(pos);
                }

                string s = "mouse move Enter - " + pos.ToString();
                DebugHelp3D.Trace.Message(s);

                _CarouselItemCollection[pos].MouseMovePoint = p;
                OnItemMouseMoveEnter(_CarouselItemCollection[pos], new EventArgs());
            }

        }

        #endregion

        #region Plane Timelines

        private void OnTimelinePlane5(object sender, EventArgs e)
        {

            Clock clock = (Clock)sender;
           
            if (clock.CurrentState != ClockState.Active)  // Ended case
            {
                // starting animation from here, sends a filling state which should be skipped 
                // because it is the begining of the animation not the end
                
                if (_filling == true)
                {
                    _filling = false;
                    return;
                }
                

                if (_AniamtingRotateOffset == 0)
                {
                    OnItemSelected(_CarouselItemCollection[_currentCarouselItem], new EventArgs());
                }

                if (_AniamtingRotateOffset < 0)
                {
                    _AniamtingRotateOffset++;
                    _filling = true;
                    Previous();
                }
                if (_AniamtingRotateOffset > 0)
                {
                    _AniamtingRotateOffset--;
                    _filling = true;
                    Next();
                }

            }

        }

        #endregion

        #endregion

        #region Public 

        public void Draw(string ID)
        {
            // 3D globals
            _carouselVisual3D = myViewport3D.Children[0] as ModelVisual3D;

            if ((ID == null) || (ID.Length == 0) ||  (_CarouselItemCollection.Count < _MAX_OBJECTS) )
                return;

            int pos = FindCarouselPositionFromID(ID);

            if (pos == -1)
                pos = 0;

            _currentCarouselItem = pos;

            for (int i = 1; i <= (_MAX_OBJECTS / 2); i++)
            {
                pos = NextCarouselItem(pos);
            }

            _rightNextCarouselItem = NextCarouselItem(pos);
            _selectedPlane = (_MAX_OBJECTS / 2);

            for (int i=0; i<=_MAX_OBJECTS;i++)
            {
                SetMaterial(i, pos);
                pos = PreviousCarouselItem(pos);
                _leftNextCarouselItem = pos;
            }

            // setup category labels
            //SetLabelMaterial((int)CarouselLabelStates.Center, _currentCarouselItem);
            //SetLabelMaterial((int)CarouselLabelStates.Right, _rightNextCarouselItem);
            //SetLabelMaterial((int)CarouselLabelStates.Left, _leftNextCarouselItem);
            
        }

        public void Next()
        {
            if (_selectedPlane == -1)
                return;

            int begin = Start(_selectedPlane);

            SetMaterial(begin, _leftNextCarouselItem);

            for (int i = 0; i <= _MAX_OBJECTS; i++)
            {
                int next = Previous(i);

                string storyboard = "StoryboardPlane" + begin.ToString();
                string rotate = "RotateTo" + next.ToString();
                string translatex = "TranslateXTo" + next.ToString();
                string translatey = "TranslateYTo" + next.ToString();
                string translatez = "TranslateZTo" + next.ToString();

                BeginStoryboardFromResource(ref myViewport3D, storyboard, rotate, translatex, translatey, translatez);

                begin = Next(begin);
            }

            _selectedPlane = Next(_selectedPlane);
            _rightNextCarouselItem = PreviousCarouselItem(_rightNextCarouselItem);
            _leftNextCarouselItem = PreviousCarouselItem(_leftNextCarouselItem);
            _currentCarouselItem = PreviousCarouselItem(_currentCarouselItem);

        }

        public void Previous()
        {
            if (_selectedPlane == -1)
                return;

            int begin = Start(_selectedPlane);

            SetMaterial(Previous (begin), _rightNextCarouselItem);

            for (int i = 0; i <= _MAX_OBJECTS; i++)
            {
                int next = Next(i);

                string storyboard = "StoryboardPlane" + begin.ToString();
                string rotate = "RotateTo" + next.ToString();
                string translatex = "TranslateXTo" + next.ToString();
                string translatey = "TranslateYTo" + next.ToString();
                string translatez = "TranslateZTo" + next.ToString();

                BeginStoryboardFromResource(ref myViewport3D, storyboard, rotate, translatex, translatey, translatez);

                begin = Next(begin);
            }

            _selectedPlane = Previous(_selectedPlane);
            _rightNextCarouselItem = NextCarouselItem(_rightNextCarouselItem);
            _leftNextCarouselItem = NextCarouselItem(_leftNextCarouselItem);
            _currentCarouselItem = NextCarouselItem(_currentCarouselItem);
        }

        #endregion

        #region Private Methods

        private void BeginStoryboardFromResource(ref Viewport3D vp3D, string storyboardKey, string rotateKey, string translateXKey, string translateYKey, string translateZKey)
        {
            ParallelTimeline pt;

            Storyboard s = (Storyboard)this.FindResource(storyboardKey);
            DoubleAnimation da = (DoubleAnimation)this.FindResource(rotateKey);
            DoubleAnimation tx = (DoubleAnimation)this.FindResource(translateXKey);
            DoubleAnimation ty = (DoubleAnimation)this.FindResource(translateYKey);
            DoubleAnimation tz = (DoubleAnimation)this.FindResource(translateZKey);

            if ((s == null) || (s.Children.Count != 4) || (da == null) || (tx == null) || (ty == null) || (tz == null))
                return;

            // set rotation animation
            pt = s.Children[0] as ParallelTimeline;
            pt.Children.Clear();
            pt.Children.Add(da);

            // set translation animation
            pt = s.Children[1] as ParallelTimeline;
            pt.Children.Clear();
            pt.Children.Add(tx);

            pt = s.Children[2] as ParallelTimeline;
            pt.Children.Clear();
            pt.Children.Add(ty);

            pt = s.Children[3] as ParallelTimeline;
            pt.Children.Clear();
            pt.Children.Add(tz);

            s.Begin(vp3D);
            //ModelVisual3D mv = ModelVisual3D();
            //mv.Content
        }

        private int Start(int selected)
        {
            int begin = selected;

            for (int i = 0; i < (_MAX_OBJECTS/2 ); i++)
            {
                begin = Previous(begin);
            }

            return begin;
        }

        private int End(int selected)
        {
            int begin = selected;

            for (int i = 0; i < (_MAX_OBJECTS / 2); i++)
            {
                begin = Next(begin);
            }

            return begin;
        }

        private int Next(int start)
        {
            int next = start + 1;
            if (next > _MAX_OBJECTS)
                next = 0;

            return next;
        }

        private int Previous(int start)
        {
            int previous = start - 1;
            if (previous < 0)
                previous = _MAX_OBJECTS;

            return previous;
        }

        private int FindOffsetFromPosition(int position)
        {
            int pos = _selectedPlane;
            int found = -1;

            if (position == pos)
                return 0;

            for (int i = 1; i <= (_MAX_OBJECTS/2); i++)
            {
                pos = Previous(pos);

                if (pos == position)
                {
                    found = i;
                    break;
                }

            }

            if (found != -1)
                return found * -1;

            pos = _selectedPlane;
            found = 0;

            for (int i = 1; i <= (_MAX_OBJECTS / 2); i++)
            {
                pos = Next(pos);

                if (pos == position)
                {
                    found = i;
                    break;
                }

            }

            return found;

        }

        private int NextCarouselItem(int start)
        {
            int next = start + 1;
            if (next >= _CarouselItemCollection.Count)
                next = 0;

            return next;
        }

        private int PreviousCarouselItem(int start)
        {
            int previous = start - 1;
            if (previous < 0)
                previous = _CarouselItemCollection.Count-1;

            return previous;
        }

        private int FindCarouselPositionFromID(string ID)
        {
            int pos = -1;
            int i = 0;
            foreach (CarouselItem ci in CarouselItemCollection)
            {
                if (ci.ID == ID)
                {
                    pos = i;
                    break;
                }

                i++;
            }

            return pos;
        }

        private void SetMaterial (int position, int carouselposition)
        {
            if ((position > _MAX_OBJECTS) || (position < 0) || (carouselposition < 0) || (carouselposition > _CarouselItemCollection.Count) || (_carouselVisual3D == null))
                return;

            Brush brush = _CarouselItemCollection[carouselposition].Brush;

            ModelVisual3D mv3dItem = _carouselVisual3D.Children[position] as ModelVisual3D;
            GeometryModel3D gm3d = mv3dItem.Content as GeometryModel3D;
            DiffuseMaterial bm = new DiffuseMaterial(brush);
            gm3d.Material = bm;
        }

        private void AnimateToPosition(int position)
        {
            _AniamtingRotateOffset = FindOffsetFromPosition(position);

            if (_AniamtingRotateOffset < 0)
            {
                _AniamtingRotateOffset++;
                Previous();
            }
            if (_AniamtingRotateOffset > 0)
            {
                _AniamtingRotateOffset--;
                Next();
            }

        }

        #region HitTesting

        public bool DoHitTest(Visual target, Point p)
        {
            _SelectedHitTest = -1; // _HitMesh will be update by HTResult delegate when intersection occurs.

            _ModelHitTest = null;
            VisualTreeHelper.HitTest(target, null, new HitTestResultCallback(HTResult), new PointHitTestParameters(p));

            // You can also target a Visual3D rather than the entire viewport3D
            // VisualOperations.HitTest(Visual3D, null, new HitTestResultDelegate(HTResult), new RayHitTestParameters(origin - like the camera, direction - could be directional vector based on camera position and lookat point));

            _SelectedHitTest = FindCarouselPositionFromGeometry(_ModelHitTest);
           
            if (_SelectedHitTest == -1)
                return false;

            return true;
        }

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

        private int FindCarouselPositionFromGeometry(Model3D model)
        {
            int ret = -1;

            //Model3D model = mesh as GeometryModel3D;

            if (model == null) 
                return ret;

            for (int i = 0; i <= _MAX_OBJECTS; i++)
            {
                ModelVisual3D mv3dItem = _carouselVisual3D.Children[i] as ModelVisual3D;
                Model3D m3d = mv3dItem.Content as Model3D;

                if (m3d == model)
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }

        #endregion

        #endregion

        #region Globals

        int _selectedPlane = -1;
        int _selectedMouseMovePlane = -1;
        int _currentCarouselItem = -1;
        int _rightNextCarouselItem = -1;
        int _leftNextCarouselItem = -1;
        const int _MAX_OBJECTS = 10;
        const int _MAX_LABELS = 2;
        bool _filling = false;

        // hit test
        int _SelectedHitTest;
        int _AniamtingRotateOffset = 0;
        double _HitTestDistance = 0;
        Model3D _ModelHitTest = null;
        

        // 3D 
        ModelVisual3D _carouselVisual3D;

        Trackball _trackball;

        ObservableCollection<CarouselItem> _CarouselItemCollection;
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


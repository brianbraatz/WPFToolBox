using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace Ribbon
{

    public class MorphMesh
    {
        public MorphMesh(Model3DGroup model)
        {
            if (model == null)
                return;

            _Model = model;

            _timeline = new ParallelTimeline(null, Duration.Forever);
            _timeClock = _timeline.CreateClock();
            _timeClock.CurrentTimeInvalidated += new EventHandler(_timeClock_CurrentTimeInvalidated);
            _timeClock.Controller.Begin();
            _lastTime = TimeSpan.FromMilliseconds(0);

            CompositionTarget.Rendering += UpdateDeltaTime;
            CompositionTarget.Rendering += MorphRendering;
        }

        void _timeClock_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            
        }

        #region Event Handlers

        public delegate void SelectedEventHandler(object sender, EventArgs e);

        public event SelectedEventHandler AnimationCompleted;
        protected virtual void OnAnimationCompleted(object o, EventArgs e)
        {
            if (AnimationCompleted != null)
                AnimationCompleted(o, e);
        }

        #endregion Event Handlers

        #region Events

        private void UpdateDeltaTime(object sender, EventArgs e)
        {
            if (_Animating == false)
                return;

            TimeSpan? currentTime = _timeClock.CurrentTime;

            //we're working with nullables, so just to be safe we should check for values
            if (currentTime.HasValue && _lastTime.HasValue)
            {
                //get the difference in time
                TimeSpan diffTime = currentTime.Value - _lastTime.Value;
                _deltaTime += diffTime.TotalMilliseconds;
            }

            //cycle old time
            _lastTime = currentTime;
        }

        private void MorphRendering(object sender, EventArgs e)
        {
            if (_Animating == false)
                return;

            OnRenderEvent();
        }


        #endregion

        #region Public

        public void MorphMeshAnimation(string xamlFromFile, string xamlToFile, double duration)
        {
            if ((!File.Exists(xamlFromFile)) || (!File.Exists(xamlToFile)))
                return;

            _ModelStage1 = (Model3DGroup)XamlReader.Load(new FileStream(xamlFromFile, FileMode.Open, FileAccess.Read));
            _ModelStage2 = (Model3DGroup)XamlReader.Load(new FileStream(xamlToFile, FileMode.Open, FileAccess.Read));

            /*
            if (reader == null)
                return;

            //Canvas canvas = (Canvas)Parser.LoadXml(new FileStream("test.xaml", FileMode.Open, FileAccess.Read));
            _ModelStage1 = MarkupReader.Parser.LoadXml(reader) as Model3DGroup;

            reader = new XmlTextReader(xamlToFile);

            if (reader == null)
                return;

            _ModelStage2 = Parser.LoadXml(reader) as Model3DGroup;
            */
            if ((_ModelStage1 == null) || (_ModelStage2 == null))
                return;

            _Model.Children.Clear();
            _Model.Children.Add(_ModelStage1);

            _FrameDuration = duration;

            _lastTime = _timeClock.CurrentTime; // set timelines for tracking elapsed time
            _deltaTime = 0;
            _Animating = true; // composition rendering is always firing
        }

        #endregion 

        #region Private

        void OnRenderEvent()
        {
            if (_deltaTime >= _FrameDuration)
            {
                _Animating = false;
                _deltaTime = _FrameDuration;
                OnAnimationCompleted(null, new EventArgs());
            }

            CreateGeometry3D();
        }

        private void CreateGeometry3D()
        {
            Model3DGroup m3dgCopy;
            GeometryModel3D gm3d;
            MeshGeometry3D mg3d;
            MeshGeometry3D mg3d1;
            MeshGeometry3D mg3d2;
            
            gm3d = _ModelStage1.Children[0] as GeometryModel3D;
            mg3d1 = gm3d.Geometry as MeshGeometry3D;

            gm3d = _ModelStage2.Children[0] as GeometryModel3D;
            mg3d2 = gm3d.Geometry as MeshGeometry3D;

            if (mg3d1.Positions.Count != mg3d2.Positions.Count)
                return;

            MeshGeometry3D mesh = new MeshGeometry3D();

            // compute new positions
            double distancePercent = _deltaTime / _FrameDuration;
            Point3D pDiff = new Point3D (0,0,0);

            for (int i = 0; i < mg3d1.Positions.Count; i++)
            {
                // normals
                Vector3D v1 = mg3d1.Normals[i];
                Vector3D v2 = mg3d2.Normals[i];
                Vector3D vDiff = v2 - v1;
                vDiff *= distancePercent;
                vDiff += v1;
                mesh.Normals.Add(vDiff);

                // positions
                Point3D p1 = mg3d1.Positions[i];
                Point3D p2 = mg3d2.Positions[i];

                pDiff.X = p2.X - p1.X;
                pDiff.Y = p2.Y - p1.Y;
                pDiff.Z = p2.Z - p1.Z;

                pDiff.X *= distancePercent;
                pDiff.Y *= distancePercent;
                pDiff.Z *= distancePercent;

                pDiff.X += p1.X;
                pDiff.Y += p1.Y;
                pDiff.Z += p1.Z;

                mesh.Positions.Add (pDiff);
            }

            m3dgCopy = _ModelStage1.Clone();
            gm3d = m3dgCopy.Children[0] as GeometryModel3D;
            mg3d = gm3d.Geometry as MeshGeometry3D;
            mg3d.Positions = mesh.Positions;
            mg3d.Normals = mesh.Normals;

            _Model.Children.RemoveAt (0);
            _Model.Children.Add(m3dgCopy);
        }


        #endregion

        #region Globals

        Model3DGroup _Model = null;

        double _FrameDuration = 0;
        bool _Animating = false;
     
        Model3DGroup _ModelStage1;
        Model3DGroup _ModelStage2;

        //timing variables
        private Timeline _timeline;
        private Clock _timeClock;
        private TimeSpan? _lastTime;
        private double _deltaTime = 0;

        #endregion
    }
}

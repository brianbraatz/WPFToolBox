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

namespace Common
{

    public class MorphMesh
    {
        public MorphMesh(GeometryModel3D model)
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
            // Avalon attempts to see if a clock is really being used.
            // By having the eventhandler it forces the clock to be updated
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

        public void Animate(MeshGeometry3D fromMesh, MeshGeometry3D toMesh, double duration, double frameRatio)
        {
            if ((toMesh == null) || (duration == 0))
                return;

            if ((frameRatio <= 0) || (frameRatio > 1))
                return;

            if (fromMesh != null)
                _FromMesh = fromMesh;
            else
                _FromMesh = _Model.Geometry as MeshGeometry3D;

            _ToMesh = toMesh;

            _FrameRatio = frameRatio;
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
            if (_FromMesh.Positions.Count != _ToMesh.Positions.Count)
                return;

            Point3DCollection newPositions = new Point3DCollection();
            Vector3DCollection newNormals = new Vector3DCollection();

            // compute new positions
            double distancePercent = (_deltaTime / _FrameDuration) * _FrameRatio;
            Point3D pDiff = new Point3D (0,0,0);

            for (int i = 0; i < _FromMesh.Positions.Count; i++)
            {
                // normals
                Vector3D v1 = _FromMesh.Normals[i];
                Vector3D v2 = _ToMesh.Normals[i];
                Vector3D vDiff = v2 - v1;
                vDiff *= distancePercent;
                vDiff += v1;
                newNormals.Add(vDiff);

                // positions
                Point3D p1 = _FromMesh.Positions[i];
                Point3D p2 = _ToMesh.Positions[i];

                pDiff.X = p2.X - p1.X;
                pDiff.Y = p2.Y - p1.Y;
                pDiff.Z = p2.Z - p1.Z;

                pDiff.X *= distancePercent;
                pDiff.Y *= distancePercent;
                pDiff.Z *= distancePercent;

                pDiff.X += p1.X;
                pDiff.Y += p1.Y;
                pDiff.Z += p1.Z;

                newPositions.Add(pDiff);
            }

            MeshGeometry3D mesh = _Model.Geometry as MeshGeometry3D;
            mesh.Positions = newPositions;
            mesh.Normals = newNormals;
        }

        #endregion

        #region Globals

        GeometryModel3D _Model = null;

        double _FrameDuration = 0;
        double _FrameRatio = 1;
        bool _Animating = false;

        MeshGeometry3D _FromMesh;
        MeshGeometry3D _ToMesh;

        //timing variables
        private Timeline _timeline;
        private Clock _timeClock;
        private TimeSpan? _lastTime;
        private double _deltaTime = 0;

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

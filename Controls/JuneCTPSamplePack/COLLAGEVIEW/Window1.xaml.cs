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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(myViewport3D);
            _trackball.Enabled = true;

            Storyboard s;

            s = (Storyboard)this.FindResource("Item1Storyboard");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("Item2Storyboard");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("Item3Storyboard");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("Item4Storyboard");
            this.BeginStoryboard(s);

            // Setup Geometry collection
            _MainModel = (myViewport3D.Children[0] as ModelVisual3D).Content as Model3DGroup;
            _ModelImages = _MainModel.Children[0] as Model3DGroup;
            _SwapModels_0 = _ModelImages.Children[0] as Model3DGroup;
            _SwapModels_1 = _ModelImages.Children[1] as Model3DGroup;

            _GeometryCollection = new ArrayList();
            _GeometryComparer = new GeometryComparer();

            for (int i=0; i < _SwapModels_0.Children.Count; i++)
            {
                _GeometryCollection.Add(_SwapModels_0.Children[i] as GeometryModel3D);
            }

            // setup rendering loop handler
            CompositionTarget.Rendering += new EventHandler(ZBuffer_Ordering_Rendering);
        }

        #region Events
       
        private void ZBuffer_Ordering_Rendering(object sender, EventArgs e)
        {
            Model3DGroup m3dgSwap;
            Model3DGroup m3dgClear;
            int i = 0;
            bool swapTo = false;

            _GeometryCollection.Sort(_GeometryComparer);
            
            // BASIC Swapping all the time
            _ModelImages.Children.Clear();
            Model3DGroup m3dg = new Model3DGroup();

            m3dg.Children.Add(_GeometryCollection[0] as GeometryModel3D);
            m3dg.Children.Add(_GeometryCollection[1] as GeometryModel3D);
            m3dg.Children.Add(_GeometryCollection[2] as GeometryModel3D);
            m3dg.Children.Add(_GeometryCollection[3] as GeometryModel3D);

            _ModelImages.Children.Add(m3dg);
            
             

            // PERFORMANCE : NOTICE : clearing the children and re-adding them to the same Model3DGroup causes unmarshal/remarshal cost.
            // To avoid this cost, references on live render targets must not reach zero. The fix is to create a peer Model3DGroup,
            // add the reordered children into the peer (if needed), and then clearing the children. This is a swap of geometries from 
            // one Model3DGroup to another and the visual reference of the geometries is above 1 at all times. The fact that there is a CLR
            // reference because the geometries are in a collection have no effect.
            // BUGBUG : see opacity edges
            /*
            
            if (_SwapTo1 == true)
            {
                swapTo = false;
                m3dgSwap = _SwapModels_1;
                m3dgClear = _SwapModels_0;
            }
            else
            {
                swapTo = true;
                m3dgSwap = _SwapModels_0;
                m3dgClear = _SwapModels_1;
            }
            
            //
            // See if there is a need to swap
            //
            
            for (i = 0; i < m3dgClear.Children.Count; i++)
            {
                if (m3dgClear.Children[i] != _GeometryCollection[i])
                    break;
            }

            if (i == _GeometryCollection.Count)
                return;
            
            
            //
            // swap
            //
            _SwapTo1 = swapTo;

            for (i = 0; i < m3dgClear.Children.Count; i++)
            {
                m3dgSwap.Children.Add(_GeometryCollection[i]);
            }

            m3dgClear.Children.Clear();
            */
            
            
        }

        #endregion

        #region Globals

        private Trackball _trackball;

        private Model3DGroup _MainModel;
        private Model3DGroup _ModelImages;
        private Model3DGroup _SwapModels_0;
        private Model3DGroup _SwapModels_1;
        private bool _SwapTo1 = true;

        private ArrayList _GeometryCollection;
        GeometryComparer _GeometryComparer;

        #endregion
    }

    public class GeometryComparer : IComparer
    {
        public int Compare(object xIn, object yIn)
        {
            GeometryModel3D x = xIn as GeometryModel3D;
            GeometryModel3D y = yIn as GeometryModel3D;

            if ((x == null) && (y == null))
                return 0;

            if ((x != null) && (y == null))
                return 1;

            if ((x == null) && (y != null))
                return -1;

            TranslateTransform3D x_tt = (x.Transform as Transform3DGroup).Children[2] as TranslateTransform3D;
            TranslateTransform3D y_tt = (y.Transform as Transform3DGroup).Children[2] as TranslateTransform3D;

            if (x_tt.OffsetZ > y_tt.OffsetZ)
                return 1;

            if (x_tt.OffsetZ < y_tt.OffsetZ)
                return -1;

            if (x_tt.OffsetZ > y_tt.OffsetZ)
                return 0;

            return 0;
        }
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




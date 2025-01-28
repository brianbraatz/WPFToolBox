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
    public enum Button3DSpinnerStates
    {
        Unselected,
        Selected,
        Hovering,
    }

    public partial class Button3DSpinner : Grid
    {
        public Button3DSpinner()
        {
            InitializeComponent();
        }

        void OnInitialized(object sender, EventArgs e)
        {
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            _OnLoadedCalled = true;

            ChangeScale(new Size (this.ActualWidth, this.ActualHeight));

            // set the text mesh global
            ModelVisual3D m3d = ZAMViewport.Children[0] as ModelVisual3D;
            m3d = m3d.Children[0] as ModelVisual3D;
            Model3DGroup m3dg = m3d.Content as Model3DGroup;
            _ButtonGeometryFront = m3dg.Children[_BUTTON_MESH_FRONT] as GeometryModel3D;
            _ButtonGeometryBack = m3dg.Children[_BUTTON_MESH_BACK] as GeometryModel3D;

            SetText(Text3D);
            SetMaterial(Brush3D);

            // MouseInput Handler
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Button3DSpinner_PreviewMouseLeftButtonUp);
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Button3DSpinner_PreviewMouseLeftButtonDown);
            this.MouseEnter += new MouseEventHandler(Button3DSpinner_MouseEnter);
            this.MouseLeave += new MouseEventHandler(Button3DSpinner_MouseLeave);
            this.SizeChanged += new SizeChangedEventHandler(Button3DSpinner_SizeChanged);
            
            // MorphMesh
            _morphMeshFront = new MorphMesh(_ButtonGeometryFront);
            _morphMeshFront.AnimationCompleted += new MorphMesh.SelectedEventHandler(morphMeshFront_AnimationCompleted);
            _MeshButtonUp = (MeshGeometry3D)this.FindResource("ButtonMeshUp");
            _MeshButtonDown = (MeshGeometry3D)this.FindResource("ButtonMeshDown");
            _BounceMeshButtonUp = (MeshGeometry3D)this.FindResource("BounceButtonUp");
            

        }

       

        #region Properties

        TextBlock _Text3D;
        public TextBlock Text3D
        {
            get { return _Text3D; }
            set 
            {
                _Text3D = value;

                if (_OnLoadedCalled == true)
                {
                    SetText(_Text3D);
                }
            }
        }

        Brush _Brush3D;
        public Brush Brush3D
        {
            get { return _Brush3D; }
            set 
            { 
                _Brush3D = value;

                if (_OnLoadedCalled == true)
                {
                    SetMaterial(_Brush3D); 
                }
            }
        }

        #endregion

        #region Event Handlers

        public delegate void SelectedEventHandler(object sender, EventArgs e);

        public event SelectedEventHandler Selected;
        protected virtual void OnSelected(object o, EventArgs e)
        {
            if (Selected != null)
                Selected(o, e);
        }

        #endregion

        #region Events

        #region Mouse

        void Button3DSpinner_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_CurrentState == Button3DSpinnerStates.Selected)
                return;

            _CurrentState = Button3DSpinnerStates.Unselected;
            
            Storyboard s;
            
            if (_RotateTo180 == true)
            {
                s = (Storyboard)this.FindResource("ClickRotationTo0");
                this.BeginStoryboard(s);
            }
            else
            {
                s = (Storyboard)this.FindResource("ClickRotationTo180");
                this.BeginStoryboard(s);
            }

            if (_morphButtonUp == true)
            {
                _morphButtonUp = false;
                _morphButtonUp = true;
                _morphMeshFront.Animate(null, _MeshButtonUp, 300, 1);
            }
        }

        void Button3DSpinner_MouseEnter(object sender, MouseEventArgs e)
        {
            _CurrentState = Button3DSpinnerStates.Hovering;
            
            Storyboard s;
            
            if (_RotateTo180 == true)
            {
                s = (Storyboard)this.FindResource("MouseEnterRotationTo30");
                this.BeginStoryboard(s);
            }
            else 
            {
                s = (Storyboard)this.FindResource("MouseEnterRotationTo210");
                this.BeginStoryboard(s);
            }
            
        }

        void Button3DSpinner_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            Storyboard s;

            if (_morphButtonUp == true)
            {
                _morphButtonUp = false;
                _morphButtonUp = true;

                _BounceButton = true;
                _BounceUp = true;
                _BounceCurrentAmount = 1;

                _morphMeshFront.Animate(null, _MeshButtonUp, 300, 1);

            }
        }

        void Button3DSpinner_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnSelected(this, new EventArgs());

            _morphButtonUp = true;

            _morphMeshFront.Animate(null, _MeshButtonDown, 100, 1);

            OnSelected(this, new EventArgs());
        }

        #endregion

        void morphMeshFront_AnimationCompleted(object sender, EventArgs e)
        {
            
            if (_BounceButton == false)
                return;

            if (_BounceCurrentAmount >= _BounceMax)
            {
                _BounceButton = false;
                _morphMeshFront.Animate(null, _MeshButtonUp, 200, 1);
                return;
            }

            double ratio = 1.0 - ((double)_BounceCurrentAmount / (double)_BounceMax);

            if (_BounceUp == true)
            {
                _BounceUp = false;
                _morphMeshFront.Animate(null, _BounceMeshButtonUp, 200, ratio);
            }
            else
            {
                _BounceUp = true;
                _morphMeshFront.Animate(null, _MeshButtonDown, 200, ratio);
                _BounceCurrentAmount++;

            }
            
            
        }

        void Button3DSpinner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeScale(e.NewSize);
        }

        private void OnFrontTextDownStoryboard(object sender, EventArgs e)
        {
        }

        #endregion

        #region Private Methods

        private void SetText(TextBlock text)
        {
            if ((text == null) || (text.Text == null) || (text.Text.Length == 0))
                return;

            MaterialGroup m = _ButtonGeometryFront.Material as MaterialGroup;
            DiffuseMaterial dm = m.Children[1] as DiffuseMaterial;
            VisualBrush vb = dm.Brush as VisualBrush;
            Grid g = vb.Visual as Grid;
            TextBlock tb = g.Children[0] as TextBlock;
            tb.Text = text.Text;
            tb.FontFamily = text.FontFamily;
            tb.Foreground = text.Foreground;
        }

        private void SetMaterial (Brush brush)
        {
            MaterialGroup mg = FindResource("ButtonMaterialGroup") as MaterialGroup;
            DiffuseMaterial df = mg.Children[0] as DiffuseMaterial;
            df.Brush = brush;
            _ButtonGeometryFront.Material = mg;
            _ButtonGeometryBack.Material = mg;
        }

        void ChangeScale(Size size)
        {
            ModelVisual3D m3d = ZAMViewport.Children[0] as ModelVisual3D;
            Transform3DGroup t3dg = m3d.Transform as Transform3DGroup;
            ScaleTransform3D st3d = t3dg.Children[1] as ScaleTransform3D;

            double vX = st3d.ScaleX;
            double vY = st3d.ScaleY;
            double vZ = st3d.ScaleZ;

            bool taller = true;

            if (size.Height < size.Width)
                taller = false;


            if (taller == false)
            {
                vX = 1;
                vY = size.Height / size.Width;
            }
            else
            {
                vX = size.Width / size.Height;
                vY = 1;
            }

            st3d.ScaleX = vX;
            st3d.ScaleY = vY;
            st3d.ScaleZ = vZ;
        }

        #endregion

        #region Globals

        private bool _RotateTo180 = true;

        GeometryModel3D _ButtonGeometryFront;
        GeometryModel3D _ButtonGeometryBack;
        const int _BUTTON_MESH_FRONT = 0;
        const int _BUTTON_MESH_BACK = 1;

        Button3DSpinnerStates _CurrentState = Button3DSpinnerStates.Unselected;

        bool _OnLoadedCalled = false;

        // MorphMesh
        MorphMesh _morphMeshFront;
        MeshGeometry3D _MeshButtonUp;
        MeshGeometry3D _MeshButtonDown;
        MeshGeometry3D _BounceMeshButtonUp;
        
        bool _morphButtonUp = false;

        // Jumping Button
        bool _BounceButton = false;
        bool _BounceUp = false;
        int _BounceMax = 4;
        int _BounceCurrentAmount = 1;

        #endregion
    }

}



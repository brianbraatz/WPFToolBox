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

            Model3DGroup group = (myViewport3D.Children[0]as ModelVisual3D).Content as Model3DGroup;
            mm = new MorphMesh(group.Children[0] as Model3DGroup);
            mm.AnimationCompleted += new MorphMesh.SelectedEventHandler(OnMorphMeshAnimationCompleted);
        }

        #region Events

        private void OnStageOneToTwo(object sender, RoutedEventArgs e)
        {
            mm.MorphMeshAnimation("Stage1.xaml", "Stage2.xaml", 1000);
        }

        private void OnStageOneToThree(object sender, RoutedEventArgs e)
        {
            mm.MorphMeshAnimation("Stage1.xaml", "Stage3.xaml", 1000);
        }

        private void OnStageThreeToOne(object sender, RoutedEventArgs e)
        {
            mm.MorphMeshAnimation("Stage3.xaml", "Stage1.xaml", 1000);
        }

        private void OnStageTwoToOne(object sender, RoutedEventArgs e)
        {
            mm.MorphMeshAnimation("Stage2.xaml", "Stage1.xaml", 1000);
        }

        private void OnAnimateStagesToggle(object sender, RoutedEventArgs e)
        {
            if (_AnimateStagesToggle == true)
            {
                _AnimateStagesToggle = false;
            }
            else
            {
                _AnimateStagesToggle = true;
                AnimateMesh();
            }
            
        }

        private void OnMorphMeshAnimationCompleted(object sender, EventArgs e)
        {
            if (_AnimateStagesToggle == true)
                AnimateMesh();
        }

        #endregion

        #region Private Methods

        private void AnimateMesh()
        {
            switch (_MeshPosition)
            {
                case 0:
                    OnStageOneToTwo(null, null);
                    break;
                case 1:
                    OnStageTwoToOne(null, null);
                    break;
                case 2:
                    OnStageOneToThree(null, null);
                    break;
                case 3:
                    OnStageThreeToOne(null, null);
                    break;
            }

            _MeshPosition++;
            if (_MeshPosition >= _MAX_MESHPOSITION)
                _MeshPosition = 0;
        }
       
        #endregion

        #region Globals

        Trackball _trackball;
        MorphMesh mm;

        int _MeshPosition = 0;
        const int _MAX_MESHPOSITION = 4;
        bool _AnimateStagesToggle = false;
        #endregion
    }

   
}




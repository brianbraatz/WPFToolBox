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

        }

        #region Events

        private void OnImage1Animate(object sender, RoutedEventArgs e)
        {
            Storyboard s;

            s = (Storyboard)this.FindResource("ImageBrushOne");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("ImageBrushTwo");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("ImageBrushThree");
            this.BeginStoryboard(s);
            s = (Storyboard)this.FindResource("ImageBrushFour");
            this.BeginStoryboard(s);
        }
        

        #endregion

        #region Globals

        Trackball _trackball;

        #endregion
    }

   
}




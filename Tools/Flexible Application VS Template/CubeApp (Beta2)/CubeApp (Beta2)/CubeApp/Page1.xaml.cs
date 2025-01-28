using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DemoDev;
using System.Windows.Media.Animation;

namespace CubeApp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>

    public partial class Page1 : Page
    {

        public Page1()
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

            if (MyApp.IsXBAP)
            {
                MyGrid.Background = Brushes.Black;
            }
            else
            {
                MyGrid.Background = Brushes.Maroon;

            }
        }

        #region Events
        private void OnImage1Animate(object sender, RoutedEventArgs e)
        {
            Storyboard s;

            s = (Storyboard)this.FindResource("RotateStoryboard");
            this.BeginStoryboard(s);
        }
        #endregion

        #region Globals

        Trackball _trackball;

        #endregion
    }
}
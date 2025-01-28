using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDashboard.widgets
{
    /// <summary>
    /// Interaction logic for VideoReport.xaml
    /// </summary>

    public partial class VideoReport : Page
    {
        public VideoReport()
        {
            InitializeComponent();
        }

        void PageLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
           // Keep manufacturing video paused to start out with
           ManufacturingMediaElement.LoadedBehavior = MediaState.Manual;
           ManufacturingMediaElement.Pause();
        }

        private void PlayVideo(object sender, System.Windows.RoutedEventArgs e)
        {
            ManufacturingMediaElement.Play();
        }

        private void PauseVideo(object sender, System.Windows.RoutedEventArgs e)
        {
            ManufacturingMediaElement.Pause();
        }

        private void StopVideo(object sender, System.Windows.RoutedEventArgs e)
        {
            ManufacturingMediaElement.Stop();
        }
    }
}
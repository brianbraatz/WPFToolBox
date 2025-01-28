using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Input;
using System.IO;
using Microsoft.Samples.KMoore.WPFSamples.Transition;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Controls.xaml
    /// </summary>

    public partial class Controls2 : Page
    {
        public Controls2()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Controls2_Loaded);
        }

           List<object> images = new List<object>();
           List<object> videos = new List<object>();
           void Controls2_Loaded(object sender, RoutedEventArgs e)
        {
            this.images.Add(new Picture("Resources/Images/limecat.jpg"));
            this.images.Add(new Picture("Resources/Images/lemoncat.jpg"));
            this.images.Add(new Picture("Resources/Images/frogcat.jpg"));
            this.videos.Add(new Video("Resources/Video/bear.wmv"));
            this.videos.Add(new Video("Resources/Video/bear.wmv"));
            this.videos.Add(new Video("Resources/Video/bear.wmv"));
            this.videos.Add(new Video("Resources/Video/bear.wmv"));


            _data.ItemsSource = images;
            //_data.ItemsSource = videos;
        }

        public void LoadImages()
        {
           
        }

        private void treeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = sender as TreeViewItem;
            if (tvi == null) return;
            //appears that this is firig before loaded is fired
            if (dp1 == null) return;
            switch (tvi.Header.ToString())
            {
                case "Text":
                    dp1.Visibility = Visibility.Visible;
                    dp3.Visibility = Visibility.Hidden;
                    break;

                case "Images":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Hidden;
                    break;
                case "Melt":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("melt");
                    break;
                case "Cloth":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("cloth");
                    break;
                case "RotateWipe":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("rotateWipe");
                    break;
                case "Diamonds":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("Diamonds");
                    break;
                case "Checkerboard":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("Checkerboard");
                    break;
                case "3D Door":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("3D Door");
                    break;
                case "3D Explosion":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("3D Explosion");
                    break;
                case "Page":
                    dp1.Visibility = Visibility.Hidden;
                    dp3.Visibility = Visibility.Visible;
                    transitionElement.Transition = (Transition)dp3.FindResource("Page");
                    break;

            }
        }

        private void OnMouseLeftDown(object s, MouseEventArgs e)
        {
            _data.SelectedIndex = (_data.SelectedIndex + 1) % _data.Items.Count;
            _data.ScrollIntoView(_data.SelectedItem);
            if (typeof(Video) == _data.SelectedItem)
            {

            }
        }

        private void OnMouseRightDown(object s, MouseEventArgs e)
        {
            _data.SelectedIndex = (_data.SelectedIndex + _data.Items.Count - 1) % _data.Items.Count;
            _data.ScrollIntoView(_data.SelectedItem);
        }


    }

    public class SampleData
    {
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public Brush Icon
        {
            get { return _brush; }
            set { _brush = value; }
        }

        public List<SampleData> Children
        {
            get { return _children; }
        }

        private string _title = String.Empty;
        private List<SampleData> _children = new List<SampleData>();
        private Brush _brush = Brushes.Transparent;
    }
    class UI
    { }
    class Picture
    {
        public Picture(string uri)
        {
            _uri = uri;
        }
        private string _uri;
        public string Uri
        {
            get { return _uri; }
        }
    }
    class Video : Picture
    {
        public Video(string uri) : base(uri) { }
    }
    class Swatch
    {
        public Swatch(string colorName)
        {
            _colorName = colorName;
        }
        private string _colorName;
        public string ColorName
        {
            get { return _colorName; }
        }
    }
}
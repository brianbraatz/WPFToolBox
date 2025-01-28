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
using System.Windows.Shapes;


namespace AsyncBindingTest1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
            
        }

        void onDownloadCompleted(object sender, EventArgs e)
        {
            SetValue(ImagesLoadedPropertyKey, ++loadedCounter);
        }

        

        int loadedCounter = 0;
        int loadedTotalCounter = 0;
        void onImgLoaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                BitmapFrame bimage = img.Source as BitmapFrame;
                if (bimage != null)
                {
                    //why this?
                    //Actually, the images are loaded from cache, so we have to check if it's downloading before, we'll want it to complete an action :)
                    if (bimage.IsDownloading)
                    {
                        bimage.DownloadCompleted += new EventHandler(onDownloadCompleted);
                    }
                    else
                    {
                        SetValue(ImagesLoadedPropertyKey, ++loadedCounter);
                    }
                }
            }
            SetValue(ImagesTotalPropertyKey, ++loadedTotalCounter);
        }

        public static readonly DependencyPropertyKey ImagesLoadedPropertyKey = DependencyProperty.RegisterReadOnly("ImagesLoaded",
            typeof(int),
            typeof(Window1),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ImagesLoadedProperty = ImagesLoadedPropertyKey.DependencyProperty;

        public int ImagesLoaded
        {
            get { return (int)GetValue(ImagesLoadedProperty); }
        }

        public static readonly DependencyPropertyKey ImagesTotalPropertyKey = DependencyProperty.RegisterReadOnly("ImagesTotal",
            typeof(int),
            typeof(Window1),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ImagesTotalProperty = ImagesTotalPropertyKey.DependencyProperty;

        public int ImagesTotal
        {
            get { return (int)GetValue(ImagesTotalProperty); }
        }

    }
}
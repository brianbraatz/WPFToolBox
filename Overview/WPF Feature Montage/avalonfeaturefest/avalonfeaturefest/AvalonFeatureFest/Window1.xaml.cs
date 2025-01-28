using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Input;


namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            Uri iconFile = new Uri(@"pack://application:,,,/Resources/Icons/Icon.ico", UriKind.Absolute);
            IconBitmapDecoder ibd = new IconBitmapDecoder(iconFile, BitmapCreateOptions.None, BitmapCacheOption.Default);
            this.Icon = ibd.Frames[0];
        }

        void HandleRequest(object sender, RequestNavigateEventArgs e)
        {
            //NavigationWindow navWin = (NavigationWindow) MyApp.Current.MainWindow;
            //navWin.Navigate(new Uri("reading.xaml", UriKind.RelativeOrAbsolute));
            ViewFrame.Navigate(e.Uri);
        }
        void UpdateTitle(object sender, NavigationEventArgs e)
        {
            this.Title = ViewFrame.Source.ToString();
        }

        void RootKey(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape) || (e.Key == Key.F11))
            {
                if (this.WindowStyle == WindowStyle.None)
                {
                    this.WindowStyle = WindowStyle.ThreeDBorderWindow;
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                }
                e.Handled = true;
            }

            return;
        }
    }
}
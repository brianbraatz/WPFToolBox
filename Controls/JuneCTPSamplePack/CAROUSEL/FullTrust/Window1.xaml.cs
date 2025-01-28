namespace FullTrust
{
    using System;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Windows.Navigation;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.Threading;
    using System.Windows.Markup;
    using System.Xml;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Security;
    using System.Windows.Interop;
    using HitTestResult2D = System.Windows.Media.HitTestResult;
    using Common;
    
    public partial class Window1
    {
   
        void OnInitialized(object sender, EventArgs e)
        {
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            myCarousel.ItemSelected += new Carousel.SelectedEventHandler(myCarousel_ItemSelected);
            myCarousel.ItemMouseMoveEnter += new Carousel.SelectedEventHandler(myCarousel_ItemMouseMoveEnter);
            myCarousel.ItemMouseMoveLeave += new Carousel.SelectedEventHandler(myCarousel_ItemMouseMoveLeave);

            for (int i = 0; i <= 15; i++)
            {
                string vbres = "VB" + i.ToString();
                string id = "ID" + i.ToString();
                VisualBrush vb = (VisualBrush)this.FindResource(vbres);

                CarouselItem ci = new CarouselItem(id, vb, "some object");
                myCarousel.CarouselItemCollection.Add(ci);
            }

            myCarousel.Draw("ID0");
        }

        #region Events

        #region Carousel

        void myCarousel_ItemMouseMoveLeave(object sender, EventArgs e)
        {
            if (_Popup != null)
                _Popup.IsOpen = false;
        }

        void myCarousel_ItemMouseMoveEnter(object sender, EventArgs e)
        {
            CarouselItem ci = sender as CarouselItem;
            
            if (ci == null)
                return;

            TextBlock tb = new TextBlock();
            tb.Text = ci.ID;
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.Width=200;
            tb.Height =30;

            if (_Popup != null)
                _Popup.IsOpen = false;

            _Popup = new Popup();
            _Popup.Child = tb;
            _Popup.PlacementTarget = myCarousel;
            _Popup.Placement = PlacementMode.Mouse;
            _Popup.IsOpen = true;
            
        }

        void myCarousel_ItemSelected(object sender, EventArgs e)
        {
            CarouselItem ci = sender as CarouselItem;

            if (ci == null)
                return;

        }
        #endregion

        #region Buttons

        private void OnNext(object sender, RoutedEventArgs e)
        {
            myCarousel.Next();
        }

        private void OnPrevious(object sender, RoutedEventArgs e)
        {
            myCarousel.Previous();
        }

        #endregion

        #endregion

        #region Globals
        Popup _Popup;
        #endregion
    }
}
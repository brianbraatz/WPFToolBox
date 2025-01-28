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
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Windows.Resources;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections;
using System.Windows.Media.Media3D;

namespace Casino_t1
{
    /// <summary>
    /// Interaction logic for RollSlot.xaml
    /// </summary>

    public class RollSlot : Viewbox
    {
        Random _rnd;
        static Canvas[] _cnvs;
        static ImageSource[] _imgs;
        static DispatcherTimer _dt;

        static RollSlot()
        {
            loadCanvases();
            loadImages();
            //setAnimation();
            //_waitImage = new SpinImage();
        }
        DoubleAnimation _animation;
        FrameworkElement _element;
        
        public RollSlot()
        {
            _rnd = new Random(); //7
            //_dt = new DispatcherTimer(TimeSpan.FromMilliseconds(10000 / _cnvs.Length), DispatcherPriority.Normal, new EventHandler(tick), this.Dispatcher);
            _element = getSlot(_cnvs[0], _cnvs[1], _cnvs[2]);
            _element.Width = 42;
            _element.Height = 42;
            this.Child = _element;

            RotateTransform rt = new RotateTransform(-90);
            rt.CenterX = _element.Width / 2;
            rt.CenterY = _element.Height / 2;

            ScaleTransform st = new ScaleTransform(-1, 1);
            st.CenterX = _element.Width / 2;
            st.CenterY = _element.Height / 2;

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(st);
            tg.Children.Add(rt);

            _element.RenderTransform = tg;

            //initAnimation();
            

        }

       
        internal void initAnimation()
        {
            if (_animation == null)
            {
                _animation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(_speed)));
               
            }
            else
            {
                _speed = 2000;
                _goesDown = -1;
                _coef = 200;
                _ftime = true;
                _stopRequested = false;
                _animation.From = _animation.To;
                _animation.To = 360;
                _animation.Duration = new Duration(TimeSpan.FromMilliseconds(_speed));
                _animation.Completed -= new EventHandler(_animation_Win);
            }
            
            //_animation.SpeedRatio = 2;
            //_animation.RepeatBehavior = RepeatBehavior.Forever;
            
            _animation.Completed += new EventHandler(_animation_Completed);


            ((RotateTransform3D)((GeometryModel3D)((ModelVisual3D)((Viewport3D)_element).Children[0]).Content).Transform).Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, _animation);


        }

        double _speed = 2000;
        int _goesDown = -1;
        double _coef = 200;
        bool _ftime = true;
        bool _stopRequested = false;
        double _win = 0;

        void reinittick(object sender, EventArgs e)
        { 
                _speed = 2000;
                _goesDown = -1;
                _coef = 200;
                _ftime = true;
                _stopRequested = true;
                
        }
        
        public static readonly DependencyPropertyKey WinningNumberPropertyKey = DependencyProperty.RegisterReadOnly("WinningNumber", typeof(int), typeof(RollSlot), new UIPropertyMetadata(0));
        
        static DependencyProperty WinningNumberProperty = WinningNumberPropertyKey.DependencyProperty;
        
        public int WinningNumber
        {
            get { return (int)GetValue(WinningNumberProperty); }
        }
        
        public static readonly DependencyPropertyKey WinningItemPropertyKey = DependencyProperty.RegisterReadOnly("WinningItem", typeof(FrameworkElement), typeof(RollSlot), new UIPropertyMetadata(null));
        
        static DependencyProperty WinningItemProperty = WinningItemPropertyKey.DependencyProperty;
        
        public FrameworkElement WinningItem
        {
            get { return (FrameworkElement)GetValue(WinningItemProperty); }
        }
        
        void _animation_Win(object sender, EventArgs e)
        {
            
            SetValue(WinningNumberPropertyKey, (int)_win);
            SetValue(WinningItemPropertyKey, GetSlotXAML((Slots)_win));
            
            //debug
            
            //((Window1)Application.Current.MainWindow).myImg.Source = WinningItem;
            
        }
        
        public static FrameworkElement GetSlotXAML(Slots slot)
        {
            switch (slot)
            {
                case Slots.Bar1: return loadResource<Canvas>("Resources/bar1.xaml"); break;
                case Slots.Bar2: return loadResource<Canvas>("Resources/bar2.xaml"); break;
                case Slots.Bar3: return loadResource<Canvas>("Resources/bar3.xaml"); break;
                case Slots.Bell: return loadResource<Canvas>("Resources/bell.xaml"); break;
                case Slots.Limon: return loadResource<Canvas>("Resources/limon.xaml"); break;
                case Slots.Seven: return loadResource<Canvas>("Resources/seven.xaml"); break;
                case Slots.Sherry: return loadResource<Canvas>("Resources/sherry.xaml"); break;
            }
            return null;
        }
        
        internal static FrameworkElement getSlotXAML(int index)
        {
            switch (index)
            {
                case 3: return loadResource<Canvas>("Resources/bar1.xaml"); break;
                case 4: return loadResource<Canvas>("Resources/bar2.xaml"); break;
                case 5: return loadResource<Canvas>("Resources/bar3.xaml"); break;
                case 2: return loadResource<Canvas>("Resources/bell.xaml"); break;
                case 1: return loadResource<Canvas>("Resources/limon.xaml"); break;
                case 6: return loadResource<Canvas>("Resources/seven.xaml"); break;
                case 0: return loadResource<Canvas>("Resources/sherry.xaml"); break;
            }
            return null;
        }
        
        public enum Slots
        { 
            Sherry = 5,
            Limon = 4,
            Bell = 3,
            Bar1 = 2,
            Bar2 = 1,
            Bar3 = 0,
            Seven = 6
        }
        
        void _animation_Completed(object sender, EventArgs e)
        {
            if (_stopRequested)
            {
                _stopRequested = false;
                _animation.Completed -= new EventHandler(_animation_Completed);
                _animation.Completed += new EventHandler(_animation_Win);
                
                _win = _rnd.Next(_cnvs.Length);
               

                _animation.To = (360  / _cnvs.Length * _win) - _animation.From;
                    //180.0 / ((double)_win+2.0);
                    //210 / _win - 10;
                    //(360 / _cnvs.Length * _win) - (360 / _cnvs.Length);
                    //210 / _win;
                ((RotateTransform3D)((GeometryModel3D)((ModelVisual3D)((Viewport3D)_element).Children[0]).Content).Transform).Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, _animation);

                return;
            }

            if (!_ftime && _speed > 2000)
            {

                _speed = 2000;
                _goesDown = -1;
                _coef = 200;
                _ftime = true;
                _stopRequested = true;
                //_dt = new DispatcherTimer(TimeSpan.FromMilliseconds(_rnd.Next(2000)), DispatcherPriority.Normal, new EventHandler(reinittick), this.Dispatcher);

                
            }
            if (_speed > 1000)
            {
                _coef = 500;
                _ftime = false;
            }
            else if (_speed > 500 & _speed <= 1000)
                _coef = 300;
            else if (_speed > 300 & _speed <= 500)
                _coef = 100;
            else if (_speed > 100 & _speed <= 300)
                _coef = 50;
            else if (_speed < 100)
                _coef = 5;

            if (_speed >= 2000)
            {
                _goesDown = -1;
                _speed = 2000;
            }

            if (_speed <= 2000)
            {
                _speed += _goesDown * _coef;
            }

            if (_speed <= 0)
            {
                _speed += _coef;
                _goesDown = 1;
            }

            

            
            
            _animation.Duration = new Duration(TimeSpan.FromMilliseconds(_speed));
            _animation.From = 0;
            //_animation.RepeatBehavior = RepeatBehavior.Forever;
            //_animation.Completed += new EventHandler(_animation_Completed);

            ((RotateTransform3D)((GeometryModel3D)((ModelVisual3D)((Viewport3D)_element).Children[0]).Content).Transform).Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, _animation);

        }

        int i = 1;
        void tick(object sender, EventArgs e)
        {

            if (i == 10)
                i = 1;
            else
                i++;
            _animation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(2/i)));
            _animation.RepeatBehavior = RepeatBehavior.Forever;
            ((RotateTransform3D)((GeometryModel3D)((ModelVisual3D)((Viewport3D)_element).Children[0]).Content).Transform).Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, _animation);
            /*FrameworkElement fe = getSlot(i++);
            this.Child = fe;*/
        }

        StackPanel getSlot(int index)
        {
            Border b = new Border();
            b.Width = _cnvs[index].Width;
            b.Height = _cnvs[index].Height;
            b.Child = _cnvs[index];

            Border b1 = new Border();
            b1.Width = b.Width/1.1;
            b1.Height = b.Height/1.1;
            VisualBrush vb = new VisualBrush(_cnvs[index]);
            
            b1.Background = vb;

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(Colors.Black, 0));
            gsc.Add(new GradientStop(Colors.Transparent, 0.6));
            b1.OpacityMask = new LinearGradientBrush(gsc, new Point(0, 0), new Point(0, 1));

            Border b2 = new Border();
            b2.Width = b.Width/1.1;
            b2.Height = b.Height/1.1;

            b2.Background = vb;

            b2.OpacityMask = new LinearGradientBrush(gsc, new Point(0, 1), new Point(0, 0));

            StackPanel sp = new StackPanel();
            sp.Children.Add(b2);
            sp.Children.Add(b);
            sp.Children.Add(b1);

            return sp;

        }

        public class SpinImage : Image
        {
            static DispatcherTimer dt;
            public SpinImage()
            {
                dt = new DispatcherTimer(TimeSpan.FromMilliseconds(200 / _imgs.Length), DispatcherPriority.Normal, new EventHandler(tick), this.Dispatcher);
            }

            public void Start()
            {
                dt.Start();
            }
            public void Stop()
            {
                dt.Stop();
            }

            int i = 0;
            void tick(object sender, EventArgs e)
            {
                if (i == _imgs.Length)
                    i = 0;

                this.Source = _imgs[i++];
            }
        }


        static void loadImages()
        {
            _imgs = new ImageSource[4];
            _imgs[0] = loadResource<ImageSource>("Resources/r1.png");
            _imgs[1] = loadResource<ImageSource>("Resources/r3.png");
            _imgs[2] = loadResource<ImageSource>("Resources/r2.png");
            _imgs[3] = loadResource<ImageSource>("Resources/r4.png");
            
        }

        static void loadCanvases()
        {
            _cnvs = new Canvas[7];
            _cnvs[3] = loadResource<Canvas>("Resources/bar1.xaml");
            _cnvs[4] = loadResource<Canvas>("Resources/bar2.xaml");
            _cnvs[5] = loadResource<Canvas>("Resources/bar3.xaml");
            _cnvs[2] = loadResource<Canvas>("Resources/bell.xaml");
            _cnvs[1] = loadResource<Canvas>("Resources/limon.xaml");
            _cnvs[6] = loadResource<Canvas>("Resources/seven.xaml");
            _cnvs[0] = loadResource<Canvas>("Resources/sherry.xaml");
        }

        static FrameworkElement getSlot(FrameworkElement center, FrameworkElement up, FrameworkElement down)
        {


            StackPanel p = new StackPanel();
            
            p.Orientation = Orientation.Horizontal;

            for (int i = 0; i < _cnvs.Length; i++)
            {
                FrameworkElement fe = getSlotXAML(i);
                fe.Width = 48;
                fe.Height = 48;
                RotateTransform rt = new RotateTransform(-90);
                rt.CenterX = fe.Width / 2;
                rt.CenterY = fe.Height / 2;
                fe.RenderTransform = rt;

                p.Children.Add(fe/*_cnvs[i]*/);
            }

            
            
            /*p.Children.Add(up);
            p.Children.Add(center);
            p.Children.Add(down);*/
            //p.Background = Brushes.Black;
            //p.Width = 1000;
            //p.Height = 1000;
            GeometryModel3D model = loadResource<GeometryModel3D>("Resources/Cylinder.xaml");
            //Brush brush = new VisualBrush(p as Visual);
            
            
           // DiffuseMaterial material = new DiffuseMaterial();
            //material.Brush = brush;
            //material.Color = Colors.Violet;
            //GeometryModel3D model = new GeometryModel3D(mesh, material);
            ((DiffuseMaterial)model.Material).Brush = new VisualBrush(p);

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            Viewport3D port = new Viewport3D();
            port.Children.Add(visual);
            PerspectiveCamera camera = new PerspectiveCamera(new Point3D(0, 0, 1.7), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
            //camera.NearPlaneDistance = 0.125;
            //camera.FarPlaneDistance = 100;
            port.Camera = camera;

            ModelVisual3D lights = new ModelVisual3D();
            lights.Content = new AmbientLight(Colors.White);
            port.Children.Add(lights);

            return port;

        }

        static BitmapImage GetFrameworkElementAsImageSource(FrameworkElement element)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = CopyFrameworkElementToMemoryStream(element,element.Width,element.Height,new WmpBitmapEncoder());
            bi.EndInit();

            return bi;
        }

        static MemoryStream CopyFrameworkElementToMemoryStream(FrameworkElement copyTarget,
  double width, double height, BitmapEncoder enc)
        {
            // Store the Frameworks current layout transform, as this will be restored later
            Transform storedTransform = copyTarget.LayoutTransform;

            // Set the layout transform to unity to get the nominal width and height
            copyTarget.LayoutTransform = new ScaleTransform(1, 1);
            copyTarget.UpdateLayout();

            double baseHeight = copyTarget.ActualHeight;
            double baseWidth = copyTarget.ActualWidth;

            // Now scale the layout to fit the bitmap
            copyTarget.LayoutTransform =
              new ScaleTransform(baseWidth / width, baseHeight / height);
            copyTarget.UpdateLayout();

            // Render to a Bitmap Source, note that the DPI is changed for the 
            // render target as a way of scaling the FrameworkElement
            RenderTargetBitmap rtb = new RenderTargetBitmap(
              (int)width,
              (int)height,
              96d * width / baseWidth,
              96d * height / baseHeight,
              PixelFormats.Default);

            rtb.Render(copyTarget);

            // Convert from a WPF Bitmap Source to a Win32 Bitamp
            MemoryStream outStream = new MemoryStream();
            enc.Frames.Add(BitmapFrame.Create(rtb));
            enc.Save(outStream);
            // Restore the Framework Element to it's previous state
            copyTarget.LayoutTransform = storedTransform;
            copyTarget.UpdateLayout();

            return outStream;
        }




        static T loadResource<T>(string path)
        {
            T c = default(T);
            StreamResourceInfo sri = Application.GetResourceStream(new Uri(path, UriKind.Relative));

            if (sri.ContentType == "application/xaml+xml")
            {
                c = (T)XamlReader.Load(sri.Stream);
            }
            else if (sri.ContentType.IndexOf("image") >= 0)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = sri.Stream;
                bi.EndInit();
                if (typeof(T) == typeof(ImageSource))
                {
                    c = (T)((object)bi);
                }
                else if (typeof(T) == typeof(Image))
                {

                    Image img = new Image();
                    img.Source = bi;
                    c = (T)((object)img);
                }
            }

            sri.Stream.Close();
            sri.Stream.Dispose();

            return c;
        }

        Canvas getRandom()
        {
            int r = _rnd.Next(0, _cnvs.Length - 1);
            return _cnvs[r];
        }

    }
}
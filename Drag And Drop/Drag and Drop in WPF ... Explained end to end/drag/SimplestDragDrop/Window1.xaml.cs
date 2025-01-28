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
using System.IO;


namespace SimplestDragDrop
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        #region STEP1 

        //STEP 1: WIRE TO DETECT THE MOUSE DRAG ...   
        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            
            this.DragSource.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            this.DragSource.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
        }

        void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // StartDrag(e);
                   //  StartDragCustomCursor(e);
                   // StartDragWindow(e);
                     StartDragInProcAdorner(e); 

                }
            }   
        }

        void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
        #endregion 

        private void StartDrag(MouseEventArgs e)
        {
            IsDragging = true;
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);
            IsDragging = false;
        }

        




        #region STEP2  BASIC Custom Cursor .. 
        Cursor _allOpsCursor = null;

        private void StartDragCustomCursor(MouseEventArgs e)
        {

            GiveFeedbackEventHandler handler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
            this.DragSource.GiveFeedback += handler;
            IsDragging = true;
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);
            this.DragSource.GiveFeedback -= handler;
            IsDragging = false;
        }

        void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragSource_GiveFeedback " + e.Effects.ToString());

            if (this._dragdropWindow == null && this.DragScope == null)
            {
                try
                {
                    //This loads the cursor from a stream .. 
                    if (_allOpsCursor == null)
                    {                        
                        using (Stream cursorStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SimplestDragDrop.DDIcon.cur"))
                        {
                            _allOpsCursor = new Cursor(cursorStream);
                        } 
                    }
                    Mouse.SetCursor(_allOpsCursor);

                    e.UseDefaultCursors = false;
                    e.Handled = true;
                }
                finally { }
            }
            else  // This code is called when we are using a custom cursor  (either a window or adorner ) 
            {
                 
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
        }
        #endregion 


        #region STEP3 ADORNERS
        // 
        DragAdorner _adorner = null;
        AdornerLayer _layer;

        #endregion 




        #region STEP5  -- Use DRAGOVER as a workaround 

        FrameworkElement _dragScope;
        public FrameworkElement DragScope
        {
            get { return _dragScope; }
            set { _dragScope = value; }
        }


        private void StartDragInProcAdorner(MouseEventArgs e)
        {

            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            System.Diagnostics.Debug.Assert(DragScope != null);

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;            

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  

            GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
            this.DragSource.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler; 

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler; 

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler; 

            //Here we create our adorner.. 
            _adorner = new DragAdorner(DragScope, (UIElement)this.dragElement, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false; 
            //Finally lets drag drop 
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);

             // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragSource.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;  

            IsDragging = false;
        }

        private bool _dragHasLeftScope = false; 
        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true; 
            }
            
        }


        void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }

        } 




        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }


        
        #endregion 

      
        #region STEP4  Use an external Window as Cursor 

        void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UpdateWindowLocation(); 

           
        }



        private void StartDragWindow(MouseEventArgs e)
        {

            GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback); ;
            this.DragSource.GiveFeedback += feedbackhandler; 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragSource_QueryContinueDrag);
            this.DragSource.QueryContinueDrag += queryhandler; 
            IsDragging = true;
            CreateDragDropWindow(this.dragElement); 
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            this._dragdropWindow.Show(); 
            DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);
            DestroyDragDropWindow(); 
            IsDragging = false;
            this.DragSource.GiveFeedback -= feedbackhandler;
            this.DragSource.QueryContinueDrag -= queryhandler; 
        }



        private Window _dragdropWindow = null;
        private void CreateDragDropWindow(Visual dragElement)
        {
            System.Diagnostics.Debug.Assert(this._dragdropWindow == null);
            System.Diagnostics.Debug.Assert(dragElement != null);
            // TODO: FE? or UIE??   FE cause I am lazy on size . 
            System.Diagnostics.Debug.Assert(dragElement is FrameworkElement); 

            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            _dragdropWindow.SourceInitialized += new EventHandler(
            delegate(object sender, EventArgs args)
            {

                //TODO assert that we can do this.. 
                PresentationSource windowSource = PresentationSource.FromVisual(this._dragdropWindow);
                IntPtr handle = ((System.Windows.Interop.HwndSource)windowSource).Handle;

                Int32 styles = Win32.GetWindowLong(handle, Win32.GWL_EXSTYLE);
                Win32.SetWindowLong(handle, Win32.GWL_EXSTYLE, styles | Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT);

            });

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            this._dragdropWindow.Content = r;


            // put the window in the right place to start
            UpdateWindowLocation();


        }

        private void DestroyDragDropWindow()
        {
            if (this._dragdropWindow != null)
            {
                this._dragdropWindow.Close();
                this._dragdropWindow = null;
            }
        }
        void UpdateWindowLocation()
        {
            if (this._dragdropWindow != null)
            {
                Win32.POINT p;
                if (!Win32.GetCursorPos(out p))
                {
                    return;
                }
                this._dragdropWindow.Left = (double)p.X;
                this._dragdropWindow.Top = (double)p.Y;
            }
        }
#endregion 
    

        private Point _startPoint;
        private bool _isDragging;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        } 

      
         
    }
}
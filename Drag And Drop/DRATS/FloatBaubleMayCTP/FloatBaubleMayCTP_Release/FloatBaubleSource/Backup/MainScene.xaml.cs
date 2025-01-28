using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;

namespace FloatPanelNS
{
    public partial class MainScene
    {
        bool isResizing = false;
        bool isDragging = false;

        double bumpX;
        double bumpY;

        double gadgetMinWidth = 32;
        double gadgetMinHeight = 32;

        double newGadgetWidth;
        double newGadgetHeight;

        FrameworkElement gadgetParent;

        public MainScene()
        {
            // This assumes that you are navigating to this scene.
            // If you will normally instantiate it via code and display it
            // manually, you either have to call InitializeComponent by hand or
            // uncomment the following line.
            // this.InitializeComponent();

            // Insert code required on object creation below this point.

        }



        // Make changes to next three methods
        // to alter widget content
        private void OnSceneLoaded(object obj, RoutedEventArgs args)
        {
            Grid baubleGridObj = new OrangeBauble(); //Grid based scene works well
            SampleClock.Width = 164; // Take widget frame width into consideration
            SampleClock.Height = 58;  // SampleClock was created in MainScene.xaml
            SampleClock.Margin = new Thickness(256, 256, 0, 0); // Margins are easy to set
            SampleClock.Content = baubleGridObj; // Could have set clock options, etc...
            SampleClock.MouseEnter += new MouseEventHandler(ShowWidgetFrame);
            SampleClock.MouseLeave += new MouseEventHandler(HideWidgetFrame);
        }


        // Make changes here if you want to change SampleClock
        // Or are adding more widgets to this plane
        private void ShowWidgetFrame(object obj, System.Windows.Input.MouseEventArgs args)
        {
            FrameworkElement frameGrid = SampleClock.Template.FindName("Template_FrameGrid", SampleClock) as FrameworkElement;
            frameGrid.Opacity = 0.66;
        }


        private void HideWidgetFrame(object obj, System.Windows.Input.MouseEventArgs args)
        {
            FrameworkElement frameGrid = SampleClock.Template.FindName("Template_FrameGrid", SampleClock) as FrameworkElement;
            frameGrid.Opacity = 0;
        }



        private void CloseButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        // Change WidgetBase_Basic style in MainScene.xaml
        // to alter the frame appearance.
        // Certain Elements, Names, and relationships must exist
        // As seen here and in the template
        private void ResizeOrDrag(object obj, System.Windows.Input.MouseEventArgs args)
        {

            // The background exists to capture mouse events
            // And has to have some opacity to do this.
            // It is only needed during resize and drag
            // I use 10% Opaque, but you can go as low as 1%
            BackgroundGrid.Opacity = 0.1;
            if (!isResizing && !isDragging)
            {
                string senderName = ((FrameworkElement)obj).Name.ToString();
                switch (senderName)
                {
                    case "Template_ResizeGrab":
                        isResizing = true;
                        FrameworkElement gripChild = obj as FrameworkElement;
                        FrameworkElement gripParent = gripChild.Parent as FrameworkElement;
                        gadgetParent = gripParent.TemplatedParent as FrameworkElement;
                        Point posR = args.GetPosition(gadgetParent);
                        bumpX = gadgetParent.Width - posR.X;
                        bumpY = gadgetParent.Height - posR.Y;
                        this.MouseMove += new MouseEventHandler(GadgetResizeHandler);
                        this.MouseLeftButtonUp += new MouseButtonEventHandler(QuitGadgetResize);
                        break;

                    case "Template_BaseGrid":
                        isDragging = true;
                        FrameworkElement dragGrid = obj as FrameworkElement;
                        gadgetParent = dragGrid.TemplatedParent as FrameworkElement;
                        Point posD = args.GetPosition(gadgetParent);
                        bumpX = posD.X;
                        bumpY = posD.Y;
                        this.MouseMove += new MouseEventHandler(GadgetDragHandler);
                        this.MouseLeftButtonUp += new MouseButtonEventHandler(QuitGadgetDrag);
                        break;
                }
            }
        }

        void QuitGadgetResize(object obj, System.Windows.Input.MouseButtonEventArgs args)
        {
            BackgroundGrid.Opacity = 0;
            isResizing = false;
            this.MouseMove -= GadgetResizeHandler;
            this.MouseLeftButtonUp -= QuitGadgetResize;
            this.Cursor = Cursors.Arrow;
        }

        void QuitGadgetDrag(object obj, System.Windows.Input.MouseButtonEventArgs args)
        {
            BackgroundGrid.Opacity = 0;
            isDragging = false;
            this.MouseMove -= GadgetDragHandler;
            this.MouseLeftButtonUp -= QuitGadgetDrag;
            this.Cursor = Cursors.Arrow;
        }

        void GadgetResizeHandler(object obj, System.Windows.Input.MouseEventArgs args)
        {
            if (isResizing)
            {
                Point pos = args.GetPosition(gadgetParent);
                newGadgetWidth = (pos.X + bumpX);
                newGadgetHeight = (pos.Y + bumpY);

                if (newGadgetWidth > gadgetMinWidth)
                {
                    gadgetParent.Width = newGadgetWidth;
                }
                if (newGadgetHeight > gadgetMinHeight)
                {
                    gadgetParent.Height = newGadgetHeight;
                }
            }

        }

        void GadgetDragHandler(object obj, System.Windows.Input.MouseEventArgs args)
        {
            if (isDragging && !isResizing)
            {
                Point pos = args.GetPosition(DocumentRoot);
                // Margins are easy to set
                gadgetParent.Margin = new Thickness(pos.X - bumpX, pos.Y - bumpY, 0, 0);
            }
        }
    }
}




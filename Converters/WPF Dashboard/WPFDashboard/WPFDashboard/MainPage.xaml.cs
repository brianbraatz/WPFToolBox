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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace WPFDashboard
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class MainPage : Window
    {

        public MainPage()
        {
            InitializeComponent();      
        }

        private void ScaleUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UIElement cellContents = (UIElement)sender;

            // Bring the cell forward so it doesn't get occluded by other cells when animating.
            Panel.SetZIndex(cellContents, Panel.GetZIndex(cellContents) + 1);

            // Create a ScaleTransform, leave it at original scale)
            cellContents.RenderTransform = new ScaleTransform(1, 1);

            // Create an animation to animate up the cell's scale from 1X to 1.5X.
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1;
            myDoubleAnimation.To = 1.5;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
         
            // Begin the animation            
            cellContents.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, myDoubleAnimation);
            cellContents.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, myDoubleAnimation);            
        }

        private void ScaleDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UIElement cellContents = (UIElement)sender;

            // Create a ScaleTransform for the cell at its current enlarged scale size
            cellContents.RenderTransform = new ScaleTransform(1.5, 1.5);
                 
            // Create an animation to animate down the cell's scale from 1.5X to 1.0X.
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            Timeline.SetDesiredFrameRate(myDoubleAnimation, 20);
            myDoubleAnimation.From = 1.5;
            myDoubleAnimation.To = 1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            // Begin the animation            
            cellContents.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, myDoubleAnimation);
            cellContents.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, myDoubleAnimation);

            // Reset the Button's Z order
            Panel.SetZIndex(cellContents, 0);            
        }

        private void Maximize(object sender, System.Windows.RoutedEventArgs e)
        {
            // This is what the logical tree looks like:
            //                   DockPanel
            //                  /         \
            //                Button    Viewbox
            //                              \
            //                              Live widget (or bitmap preview)
            // 
            // For the FinancialStatement and SalesReport we have a bitmap preview and
            // not the live widget sine it doesn't make sense to have a live preview of
            // a large document. So for these two, we'll need to hook the live widget up to the
            // root panel of the tree when the maximize button is pressed.
            // For all other widgets, we want to get to the widget 
            // and disconnect it from it's current location in the tree,
            // and then reconnect it at the root so it's maximized.
            // 
            // Read more about logical trees here:
            // http://windowssdk.msdn.microsoft.com/library/default.asp?url=/library/en-us/wpf_conceptual/html/e83f25e5-d66b-4fc7-92d2-50130c9a6649.asp

            // Get to the DockPanel
            Button maximizeButton = (Button)sender;
            DockPanel dockPanel = (DockPanel)maximizeButton.Parent;
            _currentDashboardViewbox = (Viewbox)dockPanel.Children[2];
            UIElement widget;

            if (_currentDashboardViewbox.Name== "FinancialStatementPreview")
            {
                widget = new Frame();
                ((Frame)widget).Source = new Uri("widgets/FinancialStatement.xaml", UriKind.Relative);
            }
            else if (_currentDashboardViewbox.Name == "SalesReportPreview")
            {
                widget = new Frame();
                ((Frame)widget).Source = new Uri("widgets/SalesReport.xaml",UriKind.Relative);
            }
            else
            {
                // Get to the widget and remember where its current place is
                // so that we can put it back when the user hits the minimize button
                widget = _currentDashboardViewbox.Child;

                //Disconnect widget from its current location in the tree
                _currentDashboardViewbox.Child = null;
            }

                // Create a new DockPanel so that we can place the widget in 
                // it and a minimize button.
                DockPanel maximizedDockPanel = new DockPanel();
                maximizedDockPanel.Background = (LinearGradientBrush)Application.Current.FindResource("CellBrush");

                // Create the minimize button
                Button minimizeButton = new Button();
                minimizeButton.Background = Brushes.Transparent;
                
                minimizeButton.SetValue(DockPanel.DockProperty, Dock.Top);
                minimizeButton.Click += new RoutedEventHandler(Minimize);
                minimizeButton.Width = 60;
                minimizeButton.Height = 20;
                minimizeButton.VerticalAlignment = VerticalAlignment.Top;
                minimizeButton.HorizontalAlignment = HorizontalAlignment.Right;

                // Setup the new Panel with our widget and the 
                // minimize button
                maximizedDockPanel.Children.Add(minimizeButton);
                maximizedDockPanel.Children.Add(widget);

                // Finally add the new DockPanel containing our widget
                // to the root so that it's maximized.
                RootGrid.Children.Add(maximizedDockPanel);
                   
                // Create a nice fade in effect when widget is maximized 
                DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 1;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
                maximizedDockPanel.BeginAnimation(DockPanel.OpacityProperty, myDoubleAnimation);
            
        }
        private void Minimize(object sender, System.Windows.RoutedEventArgs e)
        {
            Button minimizeButton = (Button)sender;
            DockPanel maximizedDockPanel = (DockPanel)minimizeButton.Parent;
            
            UIElement widget = maximizedDockPanel.Children[1];

            maximizedDockPanel.Children.RemoveAt(1);
            RootGrid.Children.RemoveAt(1);

            if ((_currentDashboardViewbox.Name != "FinancialStatementPreview") && (_currentDashboardViewbox.Name != "SalesReportPreview"))
            {
                _currentDashboardViewbox.Child = widget;
            }
           
            // Create a nice fade out effect when widget is minimized
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.75));
            RootGrid.BeginAnimation(Grid.OpacityProperty, myDoubleAnimation);
        }

        private Viewbox _currentDashboardViewbox;
    }
    

}
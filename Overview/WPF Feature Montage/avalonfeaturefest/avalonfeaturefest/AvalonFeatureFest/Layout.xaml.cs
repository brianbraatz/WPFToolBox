using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Layout.xaml
    /// </summary>

    public partial class Layout : Page
    {
        public ArrayList Targets = new ArrayList();
        public ArrayList Hosts = new ArrayList();
        int counter = 3;
        int numItems = 12;
        public ArrayList Panels = new ArrayList();
        public ArrayList Buttons = new ArrayList();

        public Layout()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Application myApp = System.Windows.Application.Current;

            Panels.Add(LTLGrid);
            Panels.Add(LTLUniformGrid);
            Panels.Add(LTLStackPanelVertical);
            Panels.Add(LTLStackPanelHorizontal);
            Panels.Add(LTLWrapPanelHorizontal);
            Panels.Add(LTLWrapPanelVertical);
            Panels.Add(LTLDock);

            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = new LayoutToLayoutTarget();
                Targets.Add(target);
                target.Margin = new Thickness(5);
                target.MinWidth = 50; target.MinHeight = 50;
                target.BorderThickness = new Thickness(0);
                // Default
                Grid.SetRow(target, i / 4);
                Grid.SetColumn(target, i % 4);

                switch (i)
                {
                    case 2:
                        Grid.SetColumnSpan(target, 2);
                        break;

                    case 3:
                        Grid.SetRow(target, 2);
                        Grid.SetColumn(target, 1);
                        break;

                    case 4:
                        Grid.SetRowSpan(target, 2);
                        break;

                    case 8:
                        Grid.SetRow(target, 3);
                        Grid.SetColumnSpan(target, 3);
                        break;

                    case 9:
                        Grid.SetRow(target, 3);
                        Grid.SetColumn(target, 3);
                        break;
                }

                switch (i % 4)
                {
                    case 0:
                        DockPanel.SetDock(target, Dock.Left);
                        break;

                    case 1:
                        DockPanel.SetDock(target, Dock.Bottom);
                        break;

                    case 2:
                        DockPanel.SetDock(target, Dock.Top);
                        break;

                    case 3:
                        DockPanel.SetDock(target, Dock.Right);
                        break;
                }

                LTLUniformGrid.Width = 400;
                LTLUniformGrid.Height = 400;
                LTLUniformGrid.Children.Add(target);

                LayoutToLayoutHost host = new LayoutToLayoutHost();
                Hosts.Add(host);
                host.BorderThickness = new Thickness(0);

                Rectangle rect = new Rectangle();
                rect.Fill = createGradient(i);
                rect.RadiusY = 10;
                rect.RadiusX = 10;
                host.Child = rect;

                Canvas.SetLeft(host, 0); Canvas.SetTop(host, 0);
                LTLCanvas.Children.Add(host);

                host.BindToTarget(target);
            }
        }

        void Advance(object sender, EventArgs e)
        {
            switch (counter)
            {
                case 0:
                    ChangePanel(LTLUniformGrid);
                    LTLUniformGrid.Height = Double.NaN;
                    LTLUniformGrid.Width = Double.NaN;
                    counter = 2; // skip ahead
                    PanelName.Text = "UniformGrid";
                    break;

                case 1:
                    //ChangePanel(LTLUniformGrid);
                    break;

                case 2:
                    //ChangePanel(LTLUniformGrid);
                    break;

                case 3:
                    ChangePanel(LTLStackPanelHorizontal);
                    PanelName.Text = "StackPanel (Horizontal)";
                    break;

                case 4:
                    ChangePanel(LTLStackPanelVertical);
                    PanelName.Text = "StackPanel (Vertical)";
                    break;

                case 5:
                    ChangePanel(LTLWrapPanelHorizontal);
                    PanelName.Text = "WrapPanel (Horizontal)";
                    break;

                case 6:
                    ChangePanel(LTLWrapPanelVertical);
                    PanelName.Text = "WrapPanel (Vertical)";
                    break;

                case 7:
                    ChangePanel(LTLDock);
                    PanelName.Text = "DockPanel";
                    break;

                case 8:
                    ChangePanel(LTLGrid);
                    PanelName.Text = "Grid";
                    break;
            }

            counter++;
        }

        void IntroAnimationComplete(object sender, EventArgs e)
        {
            //Container.Visibility = Visibility.Visible;

            ClearPanels();
            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = Targets[i] as LayoutToLayoutTarget;
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);
                LTLGrid.Children.Add(target);
            }

            LTLGrid.Width = Container.Width;
            LTLGrid.Height = Container.Height;
            LTLGrid.UpdateLayout();
        }

        // I know this is ugly and bad, but I'm in a hurry
        private Brush createGradient(int i)
        {
            Color light = Color.FromArgb(0xcc, 0x99, 0x99, 0x99);
            Color dark = Color.FromArgb(0xcc, 0x55, 0x55, 0x55);

            switch (i % 3)
            {
                case 0:
                    light.R += 0x44;
                    dark.R += 0x44;
                    break;

                case 1:
                    light.G += 0x44;
                    dark.G += 0x44;
                    break;

                case 2:
                    light.B += 0x44;
                    dark.B += 0x44;
                    break;
            }

            if ((i > 2) && (i <= 5))
            {
                switch (i % 3)
                {
                    case 0:
                        light.G += 0x44;
                        dark.G += 0x44;
                        break;

                    case 1:
                        light.B += 0x44;
                        dark.B += 0x44;
                        break;

                    case 2:
                        light.R += 0x44;
                        dark.R += 0x44;
                        break;
                }
            }

            if (i > 5)
            {
                switch (i % 3)
                {
                    case 0:
                        light.R -= 0x22;
                        dark.R -= 0x22;
                        break;

                    case 1:
                        light.G -= 0x22;
                        dark.G -= 0x22;
                        break;

                    case 2:
                        light.B -= 0x22;
                        dark.B -= 0x22;
                        break;
                }
            }

            if (i > 8)
            {
                light.B += 0x44;
            }

            return new LinearGradientBrush(light, dark, 45.0);
        }


        /*
         * Set the stackpanel's orientation to vertical
         * */
        void OnStackVertical(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Hosts.Count; i++)
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);

            LTLStackPanelVertical.Orientation = System.Windows.Controls.Orientation.Vertical;
        }

        /*
         * Set the stackpanel's orientation to vertical
         * */
        void OnStackHorizontal(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Hosts.Count; i++)
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);

            LTLStackPanelVertical.Orientation = System.Windows.Controls.Orientation.Horizontal;
        }

        /*
         * Set the wrappanel's orientation to vertical
         * */
        void OnWrapVertical(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Hosts.Count; i++)
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);

            LTLWrapPanelHorizontal.Orientation = System.Windows.Controls.Orientation.Vertical;
        }

        /*
         * Set the wrappanel's orientation to vertical
         * */
        void OnWrapHorizontal(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Hosts.Count; i++)
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);

            LTLWrapPanelHorizontal.Orientation = System.Windows.Controls.Orientation.Horizontal;
        }

        /*
         * Move all buttons into the 5x5 grid
         * */
        void OnGrid5(object sender, RoutedEventArgs e)
        {
            ClearPanels();

            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = Targets[i] as LayoutToLayoutTarget;
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);
                LTLGrid.Children.Add(target);
            }
        }

        /*
         * Move all buttons into the 3x3 grid
         * */
        void OnGrid3(object sender, RoutedEventArgs e)
        {
            ClearPanels();

            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = Targets[i] as LayoutToLayoutTarget;
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);
                LTLUniformGrid.Children.Add(target);
            }

        }

        /*
         * Move all buttons into the stack panel
         * */
        void OnStackPanel(object sender, RoutedEventArgs e)
        {
            ClearPanels();
            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = Targets[i] as LayoutToLayoutTarget;
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);
                LTLStackPanelVertical.Children.Add(target);
            }

        }

        /*
         * Move all buttons into the wrap panel
         * */
        void OnWrapPanel(object sender, RoutedEventArgs e)
        {
            ChangePanel(LTLWrapPanelHorizontal);

        }

        void OnDockPanel(object sender, RoutedEventArgs e)
        {
            ChangePanel(LTLDock);
        }

        void ChangePanel(Panel newPanel)
        {
            ClearPanels();
            for (int i = 0; i < numItems; i++)
            {
                LayoutToLayoutTarget target = Targets[i] as LayoutToLayoutTarget;
                (Hosts[i] as LayoutToLayoutHost).BeginAnimating(false);
                newPanel.Children.Add(target);
            }
        }

        /*
         * Remove all elements from all panels
         * */
        void ClearPanels()
        {
            for (int i = 0; i < Panels.Count; i++)
            {
                (Panels[i] as Panel).Children.Clear();
            }
        }
    }
}
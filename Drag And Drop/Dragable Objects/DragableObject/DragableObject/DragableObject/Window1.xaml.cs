using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace DragableObject
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
                                        e.HorizontalChange);
            Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
                                        e.VerticalChange);
        }
    }
}
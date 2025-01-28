using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Input;


namespace HelloInkCanvas
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

        // Set the EditingMode to ink input.
        private void Ink(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;

            // Set the DefaultDrawingAttributes for a red pen.
            inkCanvas1.DefaultDrawingAttributes.Color = Colors.Red;
            inkCanvas1.DefaultDrawingAttributes.IsHighlighter = false;
            inkCanvas1.DefaultDrawingAttributes.Height = 2;
        }

        // Set the EditingMode to highlighter input.
        private void Highlight(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;

            // Set the DefaultDrawingAttributes for a highlighter pen.
            inkCanvas1.DefaultDrawingAttributes.Color = Colors.Yellow;
            inkCanvas1.DefaultDrawingAttributes.IsHighlighter = true;
            inkCanvas1.DefaultDrawingAttributes.Height = 25;
        }

        // Set the EditingMode to erase by stroke.
        private void EraseStroke(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        // Set the EditingMode to selection.
        private void Select(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
        }
    }
}
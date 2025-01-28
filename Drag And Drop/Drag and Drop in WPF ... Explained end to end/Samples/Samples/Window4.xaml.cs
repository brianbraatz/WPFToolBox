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
 

namespace Samples
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window4 : System.Windows.Window
    {
        DragHelper dragHelper;
        DropHelper dropHelper;
        DragHelper dragHelperWScope;
        DropHelper dropHelperWScope;
        public Window4()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
#if !LEARNING 
            this.DragOver += new DragEventHandler(Window1_DragOver);
#endif
        }

#if !LEARNING 
        void Window1_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            System.Diagnostics.Debug.WriteLine("Window1_DragOver"); 
        }
#endif

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
#if OLD 
            dragHelper = new DragHelper(this.canvas, true, null, null);
            dropHelper = new DropHelper(this.dropTargetPanel1, true, null );

            dragHelperWScope = new DragHelper(this.canvas1, true, null, (UIElement)this.BorderForScope);
            dropHelperWScope = new DropHelper(this.dropTargetPanel2, true, null );
#else
            dragHelper = new DragHelper(this.canvas, null, null);
            dropHelper = new DropHelper(this.dropTargetPanel1 );

            dragHelperWScope = new DragHelper(this.canvas1,  null, (UIElement)this.BorderForScope);
            dropHelperWScope = new DropHelper(this.dropTargetPanel2);
#endif 
        }

    }
}
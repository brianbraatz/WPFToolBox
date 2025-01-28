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
    /// Interaction logic for Window2.xaml
    /// </summary>

    public partial class Window2 : System.Windows.Window
    {

        public Window2()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        DragHelper dragHelper;
        DropHelper dropHelper;
        DragHelper dragHelperWScope;
        DropHelper dropHelperWScope;

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxDragDropDataProvider callback = new ListBoxDragDropDataProvider(this.listSrc);
            dragHelper = new DragHelper(this.listSrc, callback, null);
            dropHelper = new DropHelper(this.listDest);


        }

    }
}
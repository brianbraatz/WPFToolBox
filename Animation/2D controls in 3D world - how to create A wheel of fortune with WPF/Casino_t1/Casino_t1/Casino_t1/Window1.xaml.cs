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


namespace Casino_t1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        void reinit(object sender, MouseButtonEventArgs e)
        {
            //slot.initAnimation();
        }
        void init(object sender, RoutedEventArgs e)
        {
            slot1.initAnimation();
            slot2.initAnimation();
            slot3.initAnimation();
        }

    }
}
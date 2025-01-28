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

namespace RibbonStyle
{
    public partial class MainWindow : OfficeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = CreateIcon("pack://application:,,/Resources/Word2007.ico");
        }

        private static BitmapFrame CreateIcon(String iconLocation)
        {
            System.IO.Stream iconStream = Application.GetResourceStream(new Uri(iconLocation, UriKind.RelativeOrAbsolute)).Stream;
            return BitmapFrame.Create(iconStream);
        }
    }
}
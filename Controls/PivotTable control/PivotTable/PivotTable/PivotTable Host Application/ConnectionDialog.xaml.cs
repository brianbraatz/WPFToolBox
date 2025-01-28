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

namespace PivotTable_Host_Application
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : System.Windows.Window
    {
        public static readonly DependencyProperty CubeNameProperty = DependencyProperty.Register("CubeName", typeof(string), typeof(ConnectionDialog));
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(ConnectionDialog));

        public ConnectionDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            ConnectionString = "Data Source=localhost; Initial Catalog=Adventure Works DW Standard Edition";
            CubeName = "Adventure Works";
        }

        public string CubeName
        {
            get { return (string)GetValue(CubeNameProperty); }
            set { SetValue(CubeNameProperty, value); }
        }

        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
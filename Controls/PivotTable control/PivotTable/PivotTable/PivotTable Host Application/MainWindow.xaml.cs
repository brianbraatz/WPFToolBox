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
using Microsoft.Samples.WPF.CustomControls.DataModel;
using Microsoft.Samples.WPF.CustomControls;
using System.Data;
using System.IO;
using Microsoft.AnalysisServices.AdomdClient;

namespace PivotTable_Host_Application
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void OnCreateDataTableModelButtonClick(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.ReadXmlSchema(GetResourceStream("data.xsd"));
            dataTable.ReadXml(GetResourceStream("data.xml"));
            PivotTable.ItemsSource = dataTable;
        }

        private void OnCreateAdomdModelButtonClick(object sender, RoutedEventArgs e)
        {
            ConnectionDialog dialog = new ConnectionDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    PivotTable.ItemsSource = new AdomdItemsSource(dialog.ConnectionString, dialog.CubeName);
                }
                catch (AdomdException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private Stream GetResourceStream(string fileName)
        {
            Type type = typeof(MainWindow);
            string resourceName = type.Namespace + "." + fileName;
            return type.Assembly.GetManifestResourceStream(resourceName);
        }
    }
}
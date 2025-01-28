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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace WPFDashboard.widgets
{
    /// <summary>
    /// Interaction logic for SalesReport.xaml
    /// </summary>

    public partial class SalesReport : Page
    {
        public SalesReport()
        {
            InitializeComponent();
        }

        // Serializes a snapshot of the FlowDocument in view to XPS (output.xps)
        private void SaveToXPS(object sender, RoutedEventArgs e)
        {
            DocumentPaginator paginator = ((IDocumentPaginatorSource)FDReader.Document).DocumentPaginator;
            
            Package package = Package.Open("output.xps", FileMode.Create);
            XpsDocument xd = new XpsDocument(package);

            // Size takes in pixels so we're doing a quick conversion to set the PageSize to be 8.5 X 11 inches
            paginator.PageSize = new Size(8.5 * 96, 11 * 96); 

            XpsDocumentWriter xdp = XpsDocument.CreateXpsDocumentWriter(xd);
            xdp.Write(paginator);

            xd.Close();
            package.Close();
        }


    }
}
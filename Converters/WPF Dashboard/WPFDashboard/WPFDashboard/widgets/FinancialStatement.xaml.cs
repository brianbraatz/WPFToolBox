using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace WPFDashboard.widgets
{
    /// <summary>
    /// Interaction logic for FinanceStatement.xaml
    /// </summary>

    public partial class FinanceStatement : Page
    {
        public FinanceStatement()
        {
            InitializeComponent();

            FinanceDocumentViewer.Document = LoadXps("resources/FinancialStatement.xps");
        }

        private IDocumentPaginatorSource LoadXps(string filename)
        {
            // Open the Xps file.
            XpsDocument xpsDoc = new XpsDocument(filename, FileAccess.Read);

            //retreive the Document
            IDocumentPaginatorSource document = xpsDoc.GetFixedDocumentSequence();

            //close the document
            xpsDoc.Close();

            return document;
        }


    }
}
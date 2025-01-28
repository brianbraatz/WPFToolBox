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
using System.Windows.Xps.Packaging;
using System.IO.Packaging;

namespace BookDemoApp {
    /// <summary>
    /// Interaction logic for UCFixedDocument.xaml
    /// </summary>

    public partial class UCFixedDocument : System.Windows.Controls.UserControl 
    {
        public UCFixedDocument()
        {
            InitializeComponent();
        }

        public void OnLoaded(object sender, RoutedEventArgs args) 
        {
            XpsDocument document = new XpsDocument(Window1.CombineFileInCurrentDirectory("CSharp 3.0 Specification.xps"), System.IO.FileAccess.Read);

            FixedDocument doc = new FixedDocument();
            FixedDocumentSequence fixedDoc = document.GetFixedDocumentSequence();

            viewer.Document = fixedDoc;
            viewer.FitToHeight();
        }
    }
}

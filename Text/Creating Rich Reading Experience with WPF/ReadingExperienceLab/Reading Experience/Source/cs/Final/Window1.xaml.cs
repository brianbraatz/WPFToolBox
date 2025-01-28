using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace RichReadingLab
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
		// Different FlowDocumentReaders for each article
		// greekFlowDocumentReader already defined in XAML
        private FlowDocumentReader galaxiesFlowDocumentReader = new FlowDocumentReader();
        private FlowDocumentReader carsFlowDocumentReader = new FlowDocumentReader();
		private FlowDocumentReader currentFlowDocumentReader;
		// Used to track the FlowDocumentReader currently being viewed.

		// AnnotationServices for each of the three different FlowDocumentReaders
		private AnnotationService greekAnnotService;
		private AnnotationService galaxiesAnnotService;
		private AnnotationService carsAnnotService;
		private AnnotationService currentAnnotService;
		// Used to track the AnnotationService currently being used.

		// FileStreams to store annotations
		private FileStream greekFileStream = new FileStream("GreekAnnotations.xml", FileMode.OpenOrCreate);
		private FileStream galaxiesFileStream = new FileStream("GalaxiesAnnotations.xml", FileMode.OpenOrCreate);
		private FileStream carsFileStream = new FileStream("CarsAnnotations.xml", FileMode.OpenOrCreate);

        public Window1()
        {
            InitializeComponent();

			// Creates a FlowDocument from the XML files and assigns the FlowDocument to the proper FlowDocumentReader
			galaxiesFlowDocumentReader.Document = ConvertXmlToFlowDocument("Galaxies.xml");
			carsFlowDocumentReader.Document = ConvertXmlToFlowDocument("Cars.xml");

			// Initialize the AnnotationServices and point them to their respective FlowDocumentReaders
			greekAnnotService = new AnnotationService(greekFlowDocumentReader);
			galaxiesAnnotService = new AnnotationService(galaxiesFlowDocumentReader);
			carsAnnotService = new AnnotationService(carsFlowDocumentReader);

			// Enable the AnnotationServices and point them to their respective FileStreams
			greekAnnotService.Enable(new XmlStreamStore(greekFileStream));
			galaxiesAnnotService.Enable(new XmlStreamStore(galaxiesFileStream));
			carsAnnotService.Enable(new XmlStreamStore(carsFileStream));

			// App starts off with the greek article, so set the currentFlowDocumentReader and currentAnnotService to reflect this.
			currentFlowDocumentReader = greekFlowDocumentReader;
			currentAnnotService = greekAnnotService;
        }

		// This function handles the article switching
		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			switch (((Button)sender).Name)
			{
				case "galaxiesButton":
					// Remove the FlowDocumentReader that's in view now and add in the desired one
					dockPanel.Children.Remove(currentFlowDocumentReader);
					dockPanel.Children.Add(galaxiesFlowDocumentReader);
					currentFlowDocumentReader = galaxiesFlowDocumentReader;
					currentAnnotService = galaxiesAnnotService;
					break;

				case "carsButton":
					// Remove the FlowDocumentReader that's in view now and add in the desired one
					dockPanel.Children.Remove(currentFlowDocumentReader);
					dockPanel.Children.Add(carsFlowDocumentReader);
					currentFlowDocumentReader = carsFlowDocumentReader;
					currentAnnotService = carsAnnotService;
					break;

				default:
					// Remove the FlowDocumentReader that's in view now and add in the desired one
					dockPanel.Children.Remove(currentFlowDocumentReader);
					dockPanel.Children.Add(greekFlowDocumentReader);
					currentFlowDocumentReader = greekFlowDocumentReader;
					currentAnnotService = greekAnnotService;
					break;
			}
		}

		// Function takes in a name of a FlowDocument XAML file and converts it into a FlowDocument object
		private FlowDocument ConvertXmlToFlowDocument(String FileName)
		{
			//Create an object representing the xml file
			System.Xml.XPath.XPathDocument originalXML = new System.Xml.XPath.XPathDocument(FileName);

			// Create an object representing the XSLT
			System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
			xslt.Load("GenericToXaml.xslt");

			// Create a memory stream to store the results of the XSLT
			MemoryStream memStream = new MemoryStream();

			// Apply the XSLT transform
			xslt.Transform(originalXML, null, memStream);

			//Use this line for debugging your XSLT:
			//xslt.Transform(originalXML, new XmlTextWriter(Console.Out));

			// Parse the new XAML in the memory stream and convert it into a FlowDocument object
			FlowDocument flowDocument = new FlowDocument();
			memStream.Seek(0, SeekOrigin.Begin);
			flowDocument = (FlowDocument)System.Windows.Markup.XamlReader.Load(memStream);
			memStream.Close();

			return flowDocument;
		}

		// Function to create highlighting
		private void CreateHighlight(object sender, RoutedEventArgs e)
		{
			try
			{
				AnnotationHelper.CreateHighlightForSelection(currentAnnotService,
				  "Vista user", Brushes.Yellow);
			}
			catch (InvalidOperationException)
			{
				// If someone tries to highlight without actually selecting text, do nothing.
			}
		}

		// Function to create a text note
		private void CreateTextNote(object sender, RoutedEventArgs e)
		{
			try
			{
				AnnotationHelper.CreateTextStickyNoteForSelection(currentAnnotService,
			  "Vista user");
			}
			catch (InvalidOperationException)
			{
				// If someone tries to highlight without actually selecting text, do nothing.
			}
		}

		// Function to create an ink note
		private void CreateInkNote(object sender, RoutedEventArgs e)
		{
			try
			{
				AnnotationHelper.CreateInkStickyNoteForSelection(currentAnnotService,
					"Vista user");
			}
			catch (InvalidOperationException)
			{
				// If someone tries to highlight without actually selecting text, do nothing.
			}
		}

		// Serializes the FlowDocument in view to XAML (output.XAML)
		private void XAMLClick(object sender, RoutedEventArgs e)
		{
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.Title = "Serialize to Xaml";
            sfd.DefaultExt = "xaml";
            sfd.Filter = "XAML (*.xaml)|*.xaml";
            
            bool? success = sfd.ShowDialog();
            if (success.HasValue == false || success == false)
            {
                return;
            }
			FileStream fileOut = new FileStream(sfd.FileName, FileMode.Create);
			System.Xml.XmlTextWriter xWriter = new System.Xml.XmlTextWriter(fileOut, null);

			System.Windows.Markup.XamlWriter.Save(currentFlowDocumentReader.Document, xWriter);

			fileOut.Close();
		}

		// Serializes a snapshot of the FlowDocument in view to XPS (output.container)
		private void XPSClick(object sender, RoutedEventArgs e)
		{
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.Title = "Serialize to XPS Document";
            sfd.DefaultExt = "xps";
            sfd.Filter = "XPS Document(*.xps)|*.xps";

            bool? success = sfd.ShowDialog();
            if (success.HasValue == false || success == false)
            {
                return;
            }

            DocumentPaginator paginator = ((IDocumentPaginatorSource)currentFlowDocumentReader.Document).DocumentPaginator;
            Package pckg = Package.Open(sfd.FileName, FileMode.Create);
            XpsDocument xd = new XpsDocument(pckg);
            XpsDocumentWriter xdp = XpsDocument.CreateXpsDocumentWriter(xd);

            xdp.Write(paginator);
            xd.Close();
		}



    }// end:class Window1

}// end:namespace RichReadingLab
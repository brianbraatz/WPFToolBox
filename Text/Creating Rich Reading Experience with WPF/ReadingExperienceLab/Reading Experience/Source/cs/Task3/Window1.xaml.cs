using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Annotations;

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

        public Window1()
        {
            InitializeComponent();

			// Creates a FlowDocument from the XML files and assigns the FlowDocument to the proper FlowDocumentReader
			galaxiesFlowDocumentReader.Document = ConvertXmlToFlowDocument("Galaxies.xml");
			carsFlowDocumentReader.Document = ConvertXmlToFlowDocument("Cars.xml");

			// App starts off with the greek article, so set the currentFlowDocumentReader and currentAnnotService to reflect this.
			currentFlowDocumentReader = greekFlowDocumentReader;
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
					break;

				case "carsButton":
					// Remove the FlowDocumentReader that's in view now and add in the desired one
					dockPanel.Children.Remove(currentFlowDocumentReader);
					dockPanel.Children.Add(carsFlowDocumentReader);
					currentFlowDocumentReader = carsFlowDocumentReader;
					break;

				default:
					// Remove the FlowDocumentReader that's in view now and add in the desired one
					dockPanel.Children.Remove(currentFlowDocumentReader);
					dockPanel.Children.Add(greekFlowDocumentReader);
					currentFlowDocumentReader = greekFlowDocumentReader;
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


    }// end:class Window1

}// end:namespace RichReadingLab
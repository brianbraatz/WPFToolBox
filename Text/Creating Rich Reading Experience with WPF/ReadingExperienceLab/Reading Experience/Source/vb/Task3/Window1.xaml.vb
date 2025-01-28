Imports System.Xml
Imports System.IO
Imports System.Windows
Imports System.Windows.Annotations

' Interaction logic for Window1.xaml
Partial Public Class Window1
    Inherits Window

    ' Different FlowDocumentReaders for each article
    ' greekFlowDocumentReader already defined in XAML
    Private galaxiesFlowDocumentReader As FlowDocumentReader = New FlowDocumentReader
    Private carsFlowDocumentReader As FlowDocumentReader = New FlowDocumentReader
    Private currentFlowDocumentReader As FlowDocumentReader

    Public Sub New()
        InitializeComponent()

        ' Creates a FlowDocument from the XML files and assigns the FlowDocument to the proper FlowDocumentReader
        galaxiesFlowDocumentReader.Document = ConvertXmlToFlowDocument("Galaxies.xml")
        carsFlowDocumentReader.Document = ConvertXmlToFlowDocument("Cars.xml")

        ' App starts off with the greek article, so set the currentFlowDocumentReader and currentAnnotService to reflect this.
        currentFlowDocumentReader = greekFlowDocumentReader

    End Sub
    ' Function takes in a name of a FlowDocument XAML file and converts it into a FlowDocument object
    Private Function ConvertXmlToFlowDocument(ByVal FileName As String) As FlowDocument

        ' Create an object representing the xml file
        Dim originalXML As XPath.XPathDocument = New XPath.XPathDocument(FileName)

        ' Create an object representing the XSLT
        Dim xslt As Xsl.XslCompiledTransform = New Xsl.XslCompiledTransform()
        xslt.Load("GenericToXaml.xslt")

        ' Create a memory stream to store the results of the XSLT
        Dim memStream As MemoryStream = New MemoryStream()

        ' Apply the XSLT transform 
        xslt.Transform(originalXML, New Xsl.XsltArgumentList(), memStream)

        ' Use this line for debugging your XSLT: 
        ' xslt.Transform(originalXML, New XmlTextWriter(Console.Out))

        ' Parse the new XAML in the memory stream and convert it into a FlowDocument object                        
        Dim flowDocument As FlowDocument = New FlowDocument()
        memStream.Seek(0, SeekOrigin.Begin)

        flowDocument = Windows.Markup.XamlReader.Load(memStream)

        memStream.Close()

        Return flowDocument
    End Function
    ' This function handles the article switching
    Private Sub ButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles greekButton.Click, galaxiesButton.Click, carsButton.Click

        Select Case CType(sender, Button).Name
            Case "galaxiesButton"
                ' Remove the FlowDocumentReader that's in view now and add in the desired one
                dockPanel.Children.Remove(currentFlowDocumentReader)
                dockPanel.Children.Add(galaxiesFlowDocumentReader)
                currentFlowDocumentReader = galaxiesFlowDocumentReader

            Case "carsButton"
                ' Remove the FlowDocumentReader that's in view now and add in the desired one
                dockPanel.Children.Remove(currentFlowDocumentReader)
                dockPanel.Children.Add(carsFlowDocumentReader)
                currentFlowDocumentReader = carsFlowDocumentReader

            Case Else
                ' Remove the FlowDocumentReader that's in view now and add in the desired one
                dockPanel.Children.Remove(currentFlowDocumentReader)
                dockPanel.Children.Add(greekFlowDocumentReader)
                currentFlowDocumentReader = greekFlowDocumentReader
        End Select

    End Sub

End Class

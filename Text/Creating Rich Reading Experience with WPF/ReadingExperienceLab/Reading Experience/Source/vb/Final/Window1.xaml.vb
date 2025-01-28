Imports System.Xml
Imports System.IO
Imports System.Windows
Imports System.Windows.Annotations
Imports System.Windows.Annotations.Storage

' Interaction logic for Window1.xaml
Partial Public Class Window1
    Inherits Window

    ' Different FlowDocumentReaders for each article
    ' greekFlowDocumentReader already defined in XAML
    Private galaxiesFlowDocumentReader As FlowDocumentReader = New FlowDocumentReader
    Private carsFlowDocumentReader As FlowDocumentReader = New FlowDocumentReader
    Private currentFlowDocumentReader As FlowDocumentReader

    ' AnnotationServices for each of the three different FlowDocumentReaders		
    Private greekAnnotService As AnnotationService
    Private galaxiesAnnotService As AnnotationService
    Private carsAnnotService As AnnotationService
    Private currentAnnotService As AnnotationService

    ' FileStreams to store annotations
    Private greekFileStream As FileStream = New FileStream("GreekAnnotations.txt", FileMode.OpenOrCreate)
    Private galaxiesFileStream As FileStream = New FileStream("GalaxiesAnnotations.txt", FileMode.OpenOrCreate)
    Private carsFileStream As FileStream = New FileStream("CarsAnnotations.txt", FileMode.OpenOrCreate)

    Public Sub New()
        InitializeComponent()

        ' Creates a FlowDocument from the XML files and assigns the FlowDocument to the proper FlowDocumentReader
        galaxiesFlowDocumentReader.Document = ConvertXmlToFlowDocument("Galaxies.xml")
        carsFlowDocumentReader.Document = ConvertXmlToFlowDocument("Cars.xml")

        ' Initialize the AnnotationServices and point them to their respective FlowDocumentReaders
        greekAnnotService = New AnnotationService(greekFlowDocumentReader)
        galaxiesAnnotService = New AnnotationService(galaxiesFlowDocumentReader)
        carsAnnotService = New AnnotationService(carsFlowDocumentReader)
        
        ' Enable the AnnotationServices and point them to their respective FileStreams
        greekAnnotService.Enable(New XmlStreamStore(greekFileStream))
        galaxiesAnnotService.Enable(New XmlStreamStore(galaxiesFileStream))
        carsAnnotService.Enable(New XmlStreamStore(carsFileStream))

        ' App starts off with the greek article, so set the currentFlowDocumentReader and currentAnnotService to reflect this.
        currentFlowDocumentReader = greekFlowDocumentReader
        currentAnnotService = greekAnnotService

    End Sub
    ' Function to create highlighting
    Private Sub CreateHighlight(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles highlightMenuItem.Click

        Try
            Annotations.AnnotationHelper.CreateHighlightForSelection(currentAnnotService, "Vista user", Brushes.Yellow)
        Catch exception As InvalidOperationException
            ' If someone tries to highlight without actually selecting text, do nothing.    
        End Try

    End Sub
    ' Function to create a text note
    Private Sub CreateTextNote(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles textNoteMenuItem.Click

        Try
            Annotations.AnnotationHelper.CreateTextStickyNoteForSelection(currentAnnotService, "Vista user")
        Catch exception As InvalidOperationException
            ' If someone tries to highlight without actually selecting text, do nothing.    
        End Try

    End Sub
    ' Function to create an ink note
    Private Sub CreateInkNote(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles inkNoteMenuItem.Click

        Try
            Annotations.AnnotationHelper.CreateInkStickyNoteForSelection(currentAnnotService, "Vista user")
        Catch exception As InvalidOperationException
            ' If someone tries to highlight without actually selecting text, do nothing.    
        End Try

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
                DockPanel.Children.Remove(currentFlowDocumentReader)
                DockPanel.Children.Add(galaxiesFlowDocumentReader)
                currentFlowDocumentReader = galaxiesFlowDocumentReader
                currentAnnotService = galaxiesAnnotService

            Case "carsButton"
                ' Remove the FlowDocumentReader that's in view now and add in the desired one
                DockPanel.Children.Remove(currentFlowDocumentReader)
                DockPanel.Children.Add(carsFlowDocumentReader)
                currentFlowDocumentReader = carsFlowDocumentReader
                currentAnnotService = carsAnnotService

            Case Else
                ' Remove the FlowDocumentReader that's in view now and add in the desired one
                DockPanel.Children.Remove(currentFlowDocumentReader)
                DockPanel.Children.Add(greekFlowDocumentReader)
                currentFlowDocumentReader = greekFlowDocumentReader
                currentAnnotService = greekAnnotService
        End Select

    End Sub
    ' Serializes the FlowDocument in view to XAML (output.XAML)
    Private Sub XAMLClick(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles serializeToXAMLButton.Click

        Dim fileOut As FileStream = New FileStream("output.xaml", IO.FileMode.Create)
        Dim xWriter As XmlTextWriter = New XmlTextWriter(fileOut, System.Text.Encoding.UTF8)
        System.Windows.Markup.XamlWriter.Save(currentFlowDocumentReader.Document, xWriter)

        fileOut.Close()

    End Sub
    ' Serializes a snapshot of the FlowDocument in view to XPS (output.container) 
    Private Sub XPSClick(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles serializeToXPSButton.Click

        Dim package As Packaging.Package = Packaging.Package.Open("output.container", FileMode.Create)
        Dim xpsDoc As Xps.Packaging.XpsDocument = New Xps.Packaging.XpsDocument(package)

        Dim packagingPolicy As Xps.Serialization.XpsPackagingPolicy = New Xps.Serialization.XpsPackagingPolicy(xpsDoc)

        Dim manager As Xps.Serialization.XpsSerializationManager = New Xps.Serialization.XpsSerializationManager(packagingPolicy, False)

        manager.SaveAsXaml(currentFlowDocumentReader.Document)

        package.Close()

    End Sub

End Class

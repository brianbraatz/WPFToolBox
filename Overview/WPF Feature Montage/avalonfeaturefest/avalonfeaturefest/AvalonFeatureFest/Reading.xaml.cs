using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Reading.xaml
    /// </summary>

    public partial class Reading : Page
    {
        public Reading()
        {
            InitializeComponent();
        }

        // Turn Annotations On.
        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Make sure that an AnnotationService isn’t already enabled.
            AnnotationService service = AnnotationService.GetService(Viewer);
            if (service == null)
            {
                // (a) Create a Stream for the annotations to be stored in.
                AnnotationStream =
                  new FileStream("annotations.xml", FileMode.OpenOrCreate);
                // (b) Create an AnnotationService on our FlowDocumentPageViewer.
                service = new AnnotationService(Viewer);
                // (c) Create an AnnotationStore and give it the stream we
                // created. (Autoflush == false)
                AnnotationStore store = new XmlStreamStore(AnnotationStream);
                // (d) "Turn on annotations". Annotations will be persisted in
                // the stream created at (a).
                service.Enable(store);
            }
        }

        // Turn Annotations off.
        protected void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // (a) Check that an AnnotationService actually
            // existed and was Enabled.
            AnnotationService service =
            AnnotationService.GetService(Viewer);
            if (service != null && service.IsEnabled)
            {
                // (b) Flush changes to annotations to our stream.
                service.Store.Flush();
                // (c) Turn off annotations.
                service.Disable();

                // (d) Close our stream.
                AnnotationStream.Close();
            }
        }


        // The stream that we will store annotations in.
        Stream AnnotationStream;
    }
}
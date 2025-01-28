using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using WpfBootcampDemo.Model;

namespace WpfBootcampDemo.Controllers
{
    /// <summary>
    /// Handles interaction logic for the XrayWindow.
    /// </summary>
    internal class XrayWindowController
    {
        #region Data

        readonly ICollectionView _xraysView;
        readonly XrayWindow _xrayWindow;

        #endregion // Data

        #region Constructor

        public XrayWindowController(
            XrayWindow xrayWindow, XrayCollection xrays)
        {
            if (xrayWindow == null)
                throw new ArgumentNullException("xrayWindow");

            if (xrays == null)
                throw new ArgumentNullException("xrays");

            _xrayWindow = xrayWindow;

            // NOTE: This assumes that the XrayWindow's View uses 
            // the default view of the xray collection.
            _xraysView = CollectionViewSource.GetDefaultView(xrays);
        }

        #endregion // Constructor

        #region Command Handlers

        public bool CanShowSelectedXray
        {
            // Return true if there is a selected Xray in the UI.
            get { return _xraysView.CurrentItem != null; }
        }

        public void ShowSelectedXray()
        {
            #region Disclaimer

            // This method does not perform any null checks
            // for the sake of clarity and simplicity. In a 
            // real app method like this should more robust.

            #endregion // Disclaimer

            // Get the Xray object selected in the UI.
            Xray selectedXray = _xraysView.CurrentItem as Xray;

            // Find the UI object for displaying x-ray images.
            // If running in a unit test we get a mock object.
            IXrayImageViewer xrayViewer =
                _xrayWindow.FindResource("VIEW_XrayImageViewer")
                as IXrayImageViewer;

            Uri imageLocation = GetXrayImageLocation(selectedXray);
            xrayViewer.ShowImage(imageLocation);

            // The UI detects the new value of HasBeenViewed
            // because Xray implements INotifyPropertyChanged.
            if (!selectedXray.HasBeenViewed)
                selectedXray.HasBeenViewed = true;
        }

        #endregion // Command Handlers

        #region Private Helpers

        static Uri GetXrayImageLocation(Xray xray)
        {
            // For this demo, just grab the x-ray images from the project's Images folder.
            // A real app would have to access a database, Web service, etc.

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = new DirectoryInfo(path).Parent.Parent.FullName;
            path = Path.Combine(path, "Images");
            path = Path.Combine(path, xray.FileName);
            return new Uri(path);
        }

        #endregion // Private Helpers
    }
}
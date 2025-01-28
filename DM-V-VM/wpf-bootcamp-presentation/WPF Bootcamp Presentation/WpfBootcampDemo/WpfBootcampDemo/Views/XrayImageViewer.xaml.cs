using System;
using System.Windows;

namespace WpfBootcampDemo.Views
{
    /// <summary>
    /// Displays the image associated with an Xray in a modal dialog.
    /// </summary>
    public partial class XrayImageViewer : Window, IXrayImageViewer
    {
        public XrayImageViewer()
        {
            InitializeComponent();
        }

        #region IXrayImageViewer Members

        public void ShowImage(Uri imageLocation)
        {
            this.DataContext = imageLocation;
            this.ShowDialog();
        }

        #endregion // IXrayImageViewer Members
    }
}
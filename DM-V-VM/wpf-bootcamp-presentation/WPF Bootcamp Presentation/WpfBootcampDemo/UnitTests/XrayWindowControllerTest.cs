using System;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfBootcampDemo;
using WpfBootcampDemo.Controllers;
using WpfBootcampDemo.Model;

namespace UnitTests
{
    /// <summary>
    /// Unit tests for the XrayWindowController class.
    /// </summary>
    [TestClass]
    public class XrayWindowControllerTest
    {
        #region TestContext

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion // TestContext

        #region CanShowSelectedXrayTest

        /// <summary>
        /// A test for the CanShowSelectedXray property
        ///</summary>
        [TestMethod]
        public void CanShowSelectedXrayTest()
        {
            XrayWindow xrayWindow = new XrayWindow();
            XrayCollection xrays = xrayWindow.DataContext as XrayCollection;                   
            XrayWindowController target = new XrayWindowController(xrayWindow, xrays);

            ICollectionView xraysView = CollectionViewSource.GetDefaultView(xrays);
            xraysView.MoveCurrentToPosition(-1);
            Assert.IsFalse(target.CanShowSelectedXray, "Should not be able to show an image since no Xray is selected.");

            // Setting the position to zero is essentially the
            // same as selecting the first Xray in the ListBox.
            xraysView.MoveCurrentToPosition(0);
            Assert.IsTrue(target.CanShowSelectedXray, "Should be able to show an image since an Xray is selected.");
        }

        #endregion // CanShowSelectedXrayTest

        #region ShowSelectedXrayTest

        /// <summary>
        /// A test for the ShowSelectedXray method
        ///</summary>
        [TestMethod]
        public void ShowSelectedXrayTest()
        {
            XrayWindow xrayWindow = new XrayWindow();

            // Add a fake image viewer to the window's resources
            // so that the controller being tested can use it.
            MockXrayImageViewer mockViewer = new MockXrayImageViewer();
            xrayWindow.Resources.Add(
                "VIEW_XrayImageViewer", 
                mockViewer);

            // Create the collection of x-rays and its default view.
            XrayCollection xrays = xrayWindow.DataContext as XrayCollection;
            ICollectionView xraysView = CollectionViewSource.GetDefaultView(xrays);

            // Select the first Xray object.
            Xray selectedXray = xrays[0];
            xraysView.MoveCurrentTo(selectedXray);

            // Perform the test.
            XrayWindowController target = new XrayWindowController(xrayWindow, xrays);
            target.ShowSelectedXray();

            Assert.IsTrue(mockViewer.ShowImageWasCalled, "ShowImage should have been invoked.");
            Assert.IsTrue(selectedXray.HasBeenViewed, "The HasBeenViewed property should be true.");
        }

        #endregion // ShowSelectedXrayTest

        #region MockXrayImageViewer [nested class]

        /// <summary>
        /// An implementation of IXrayImageViewer
        /// for unit testing purposes.
        /// </summary>
        private class MockXrayImageViewer : IXrayImageViewer
        {
            public bool ShowImageWasCalled = false;

            public void ShowImage(Uri imageLocation)
            {
                this.ShowImageWasCalled = true;
            }
        }

        #endregion // MockXrayImageViewer [nested class]
    }
}
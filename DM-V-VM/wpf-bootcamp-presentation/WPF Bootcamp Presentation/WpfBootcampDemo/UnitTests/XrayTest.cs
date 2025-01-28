using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfBootcampDemo.Model;

namespace UnitTests
{
    /// <summary>
    /// Contains unit tests for the Xray class.
    ///</summary>
    [TestClass]
    public class XrayTest
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

        #region HasBeenViewedTest

        [TestMethod]
        public void HasBeenViewedTest()
        {
            DateTime creationDate = new DateTime();
            string fileName = string.Empty;
            XraySide side = XraySide.Front;
            Xray xray = new Xray(creationDate, fileName, side);

            Assert.IsFalse(xray.HasBeenViewed, "HasBeenViewed should return false now.");

            bool eventIsCorrect = false;
            xray.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e)
                {
                    eventIsCorrect = e.PropertyName == "HasBeenViewed";
                };

            xray.HasBeenViewed = true;

            Assert.IsTrue(eventIsCorrect, "Setting HasBeenViewed to true did not raise the PropertyChanged event correctly.");
            Assert.IsTrue(xray.HasBeenViewed, "HasBeenViewed should return true now.");
        }

        #endregion // HasBeenViewedTest
    }
}
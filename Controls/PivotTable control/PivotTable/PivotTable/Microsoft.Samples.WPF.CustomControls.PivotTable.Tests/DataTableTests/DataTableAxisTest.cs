using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableAxisTest
    {
        private DataTableAxis _axis;

        [TestInitialize]
        public void Initialize()
        {
            _axis = new DataTableAxis();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataTableAxisTestInvalidHierarchy()
        {
            _axis.Hierarchies.Add(new TestHierarchy("TestHierarchy"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataTableAxisTestNullHierarchy()
        {
            _axis.Hierarchies.Add(null);
        }
    }
}

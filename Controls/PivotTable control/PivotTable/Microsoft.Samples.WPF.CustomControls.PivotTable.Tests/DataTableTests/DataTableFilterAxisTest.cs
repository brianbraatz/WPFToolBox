using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableFilterAxisTest : DataTableModelBaseTest
    {
        private DataTableFilterAxis _axis;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _axis = new DataTableFilterAxis(DataTable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataTableFilterAxisTestInvalidHierarchy()
        {
            _axis.Hierarchies.Add(new TestHierarchy("TestHierarchy"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataTableFilterAxisTestNullHierarchy()
        {
            _axis.Hierarchies.Add(null);
        }
    }
}

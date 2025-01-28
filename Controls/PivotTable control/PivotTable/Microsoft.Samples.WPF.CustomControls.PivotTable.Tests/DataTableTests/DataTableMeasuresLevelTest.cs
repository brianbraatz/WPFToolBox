using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableMeasuresLevelTest : DataTableModelBaseTest
    {
        private DataTableMeasuresLevel _level;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            TestHierarchy hierarchy = new TestHierarchy("[TestHierarchy]");
            DataColumn[] columns = new DataColumn[] {
                DataTable.Columns["OrderQuantity"], DataTable.Columns["SalesAmount"] };
            _level = new DataTableMeasuresLevel(hierarchy, columns);
        }

        [TestMethod]
        public void DataTableMeasuresLevelTestConstructor()
        {
            Assert.AreEqual("Measures", _level.Caption);
            Assert.IsInstanceOfType(_level.Hierarchy, typeof(TestHierarchy));
            Assert.AreEqual(0, _level.LevelNumber);
            Assert.AreEqual("[TestHierarchy].[Measures]", _level.UniqueName);
        }

        [TestMethod]
        public void DataTableMeasuresLevelTestMembers()
        {
            Assert.AreEqual(2, _level.Members.Count);
            Assert.AreEqual("OrderQuantity", _level.Members[0].Caption);
            Assert.AreEqual("SalesAmount", _level.Members[1].Caption);
        }
    }
}

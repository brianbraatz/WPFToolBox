using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableMeasuresHierarchyTest : DataTableModelBaseTest
    {
        private DataTableMeasuresHierarchy _hierarchy;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            TestDataModel model = new TestDataModel();
            DataColumn[] columns = new DataColumn[] {
                DataTable.Columns["OrderQuantity"], DataTable.Columns["SalesAmount"] };
            _hierarchy = new DataTableMeasuresHierarchy(model, columns);
        }

        [TestMethod]
        public void DataTableMeasuresHierarchyTestConstructor()
        {
            Assert.AreEqual("Measures", _hierarchy.Caption);
            Assert.AreEqual("Measures", _hierarchy.DisplayFolder);
            Assert.IsInstanceOfType(_hierarchy.Model, typeof(TestDataModel));
            Assert.AreEqual("[Measures]", _hierarchy.UniqueName);
        }

        [TestMethod]
        public void DataTableMeasuresHierarchyTestAxis()
        {
            Assert.IsNull(_hierarchy.Axis);
            TestAxis axis = new TestAxis();
            _hierarchy.Axis = axis;
            Assert.AreSame(axis, _hierarchy.Axis);
        }

        [TestMethod]
        public void DataTableMeasuresHierarchyTestLevels()
        {
            Assert.AreEqual(1, _hierarchy.Levels.Count);
            Assert.IsInstanceOfType(_hierarchy.Levels[0], typeof(DataTableMeasuresLevel));
            Assert.AreEqual(2, _hierarchy.Levels[0].Members.Count);
        }
    }
}

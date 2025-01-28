using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableLevelTest : DataTableModelBaseTest
    {
        private DataTableLevel CreateDataTableLevel(string columnName)
        {
            DataColumn column = DataTable.Columns[columnName];
            return new DataTableLevel(new TestHierarchy("[TestHierarchy]"), column);
        }

        [TestMethod]
        public void DataTableLevelTestConstructor()
        {
            DataTableLevel level = CreateDataTableLevel("MaritalStatus");
            Assert.AreEqual("MaritalStatus", level.Caption);
            Assert.AreEqual("[TestHierarchy].[MaritalStatus]", level.UniqueName);
            Assert.AreEqual("[TestHierarchy]", level.Hierarchy.UniqueName);
            Assert.AreEqual(1, level.LevelNumber);
        }

        [TestMethod]
        public void DataTableLevelTestMembers()
        {
            DataTableLevel level = CreateDataTableLevel("MaritalStatus");
            Assert.AreEqual(2, level.Members.Count);
            Assert.IsTrue(
                (level.Members[0].Caption == "S" && level.Members[1].Caption == "M") ||
                (level.Members[0].Caption == "M" && level.Members[1].Caption == "S"));
        }
    }
}

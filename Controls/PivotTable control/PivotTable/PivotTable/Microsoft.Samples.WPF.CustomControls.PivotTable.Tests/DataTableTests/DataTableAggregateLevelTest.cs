using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableAggregateLevelTest : DataTableModelBaseTest
    {
        private DataTableMember CreateDataTableMember(string numberOfChildren)
        {
            string columnName = "NumberChildrenAtHome";
            DataColumn column = DataTable.Columns[columnName];
            DataRow firstRow = DataTableHelper.FindFirstRow(columnName, numberOfChildren);
            return new DataTableMember(new TestLevel("[SecondLevel]"), column, firstRow);
        }

        [TestMethod]
        public void DataTableAggregateLevelTestConstructor()
        {
            DataTableMember[] nextLevelMembers = new DataTableMember[] { };
            TestHierarchy hierarchy = new TestHierarchy("[TestHierarchy]");
            DataTableAggregateLevel level = new DataTableAggregateLevel(hierarchy, nextLevelMembers);
            Assert.AreEqual("All members", level.Caption);
            Assert.AreSame(hierarchy, level.Hierarchy);
            Assert.AreEqual(0, level.LevelNumber);
            Assert.AreEqual("[TestHierarchy].[All members]", level.UniqueName);
        }

        [TestMethod]
        public void DataTableAggregateLevelTestMembers()
        {
            DataTableMember[] nextLevelMembers = new DataTableMember[] { 
                CreateDataTableMember("0"), CreateDataTableMember("2") };
            TestHierarchy hierarchy = new TestHierarchy("[TestHierarchy]");
            DataTableAggregateLevel level = new DataTableAggregateLevel(hierarchy, nextLevelMembers);
            Assert.AreEqual(1, level.Members.Count);
            Assert.IsInstanceOfType(level.Members[0], typeof(DataTableAggregateMember));
            Assert.AreEqual(2, level.Members[0].Children.Count);
        }
    }
}

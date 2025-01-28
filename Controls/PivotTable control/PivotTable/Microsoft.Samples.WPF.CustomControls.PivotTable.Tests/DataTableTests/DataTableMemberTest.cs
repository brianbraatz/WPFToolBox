using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableMemberTest : DataTableModelBaseTest
    {
        private DataTableMember CreateMember(ILevel level, string columnName, string rowValue)
        {
            DataColumn column = DataTable.Columns[columnName];
            DataRow firstRow = DataTableHelper.FindFirstRow(columnName, rowValue);
            return new DataTableMember(level, column, firstRow);
        }

        [TestMethod]
        public void DataTableMemberTestConstructor()
        {
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMember member = CreateMember(level, "Education", "Bachelors");
            Assert.AreEqual("Bachelors", member.Caption);
            Assert.IsFalse(member.HasChildren);
            Assert.AreEqual("[TestLevel].[Bachelors]", member.UniqueName);
            Assert.AreSame(level, member.Level);
            Assert.AreEqual(0, member.Children.Count);
        }

        [TestMethod]
        public void DataTableMemberTestAddRow()
        {
            TestLevel level = new TestLevel("TestLevel");
            DataTableMember member = CreateMember(level, "MaritalStatus", "S");
            List<DataRow> rows = new List<DataRow>(DataTableHelper.FindRows("MaritalStatus", "S"));
            member.AddRow(rows[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataTableMemberTestAddInvalidRow()
        {
            TestLevel level = new TestLevel("TestLevel");
            DataTableMember member = CreateMember(level, "MaritalStatus", "S");
            DataRow invalidRow = DataTableHelper.FindFirstRow("MaritalStatus", "M");
            member.AddRow(invalidRow);
        }

        [TestMethod]
        public void DataTableMemberTestFilterRow()
        {
            TestLevel level = new TestLevel("[TestLevel]");
            DataColumn column = DataTable.Columns["NumberChildrenAtHome"];
            List<DataRow> zeroChildrenRows = new List<DataRow>(DataTableHelper.FindRows("NumberChildrenAtHome", "0"));
            DataTableMember member = new DataTableMember(level, column, zeroChildrenRows[0]);
            for (int i = 1; i < zeroChildrenRows.Count; i++)
            {
                member.AddRow(zeroChildrenRows[i]);
            }

            Set<DataRow> allRows = new Set<DataRow>(DataTable.Rows);
            member.FilterRows(allRows);
            List<DataRow> allRowsCollection = new List<DataRow>(allRows);
            CollectionAssert.AreEquivalent(zeroChildrenRows, allRowsCollection);
        }
    }
}

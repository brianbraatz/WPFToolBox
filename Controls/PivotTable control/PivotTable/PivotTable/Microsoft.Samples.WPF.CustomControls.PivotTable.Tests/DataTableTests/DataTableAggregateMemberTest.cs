//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableAggregateMemberTest class.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableAggregateMemberTest : DataTableModelBaseTest
    {
        private DataTableMember CreateDataTableMember(string numberOfChildren)
        {
            string columnName = "NumberChildrenAtHome";
            DataColumn column = DataTable.Columns[columnName];
            DataRow firstRow = DataTableHelper.FindFirstRow(columnName, numberOfChildren);
            return new DataTableMember(new TestLevel("[SecondLevel]"), column, firstRow);
        }

        [TestMethod]
        public void DataTableAggregateMemberTestConstructor()
        {
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableAggregateMember member = new DataTableAggregateMember(level, new DataTableMember[] { });
            Assert.AreEqual("All members", member.Caption);
            Assert.AreEqual("[TestLevel].[All members]", member.UniqueName);
            Assert.IsNull(member.ParentMember);
            Assert.AreSame(level, member.Level);
        }

        [TestMethod]
        public void DataTableAggregateMemberTestChildren()
        {
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMember[] childMembers = new DataTableMember[] {
                CreateDataTableMember("0"), CreateDataTableMember("1") };
            DataTableAggregateMember member = new DataTableAggregateMember(level, childMembers);
            Assert.IsTrue(member.HasChildren);
            CollectionAssert.AreEqual(childMembers, member.Children);
        }

        [TestMethod]
        public void DataTableAggregateMemberTestFilterDataRows()
        {
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMember[] childMember = new DataTableMember[] { };
            DataTableAggregateMember member = new DataTableAggregateMember(level, childMember);
            Set<DataRow> rows = new Set<DataRow>(DataTable.Rows);
            Set<DataRow> originalRows = new Set<DataRow>(DataTable.Rows);
            member.FilterRows(rows);
            CollectionAssert.AreEquivalent(new List<DataRow>(originalRows), new List<DataRow>(rows));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableMeasuresMemberTest : DataTableModelBaseTest
    {
        [TestMethod]
        public void DataTableMeasuresMemberTestConstructor()
        {
            DataColumn column = DataTable.Columns["OrderQuantity"];
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMeasuresMember member = new DataTableMeasuresMember(level, column);
            Assert.AreEqual("OrderQuantity", member.Caption);
            Assert.IsFalse(member.HasChildren);
            Assert.AreEqual(0, member.Children.Count);
            Assert.AreEqual("Measures", member.DisplayFolder);
            Assert.IsTrue(member.CanSetAggregateFunction);
            Assert.AreSame(level, member.Level);
            Assert.IsNull(member.ParentMember);
            Assert.AreEqual("[TestLevel].[OrderQuantity]", member.UniqueName);
        }

        [TestMethod]
        public void DataTableMeasureMemberTestAggregateFunction()
        {
            DataColumn column = DataTable.Columns["OrderQuantity"];
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMeasuresMember member = new DataTableMeasuresMember(level, column);
            Assert.IsInstanceOfType(member.AggregateFunction, AggregateFunction.Sum.GetType());
            member.AggregateFunction = AggregateFunction.Average;
            Assert.IsInstanceOfType(member.AggregateFunction, AggregateFunction.Average.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataTableMeasureMemberTestNullAggregateFunction()
        {
            DataColumn column = DataTable.Columns[0];
            TestLevel level = new TestLevel("[TestLevel]");
            DataTableMeasuresMember member = new DataTableMeasuresMember(level, column);
            member.AggregateFunction = null;
        }
    }
}

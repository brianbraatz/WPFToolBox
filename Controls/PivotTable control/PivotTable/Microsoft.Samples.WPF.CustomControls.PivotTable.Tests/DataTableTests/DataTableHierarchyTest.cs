//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableHierarchyTest
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
    public class DataTableHierarchyTest : DataTableModelBaseTest
    {
        [TestMethod]
        public void DataTableHierarchyTestConstructor()
        {
            DataColumn column = DataTable.Columns["NumberChildrenAtHome"];
            TestDataModel testModel = new TestDataModel();
            DataTableHierarchy hierarchy = new DataTableHierarchy(testModel, column);
            Assert.IsNull(hierarchy.Axis);
            Assert.AreEqual("NumberChildrenAtHome", hierarchy.Caption);
            Assert.AreEqual("Dimensions", hierarchy.DisplayFolder);
            Assert.AreEqual("[NumberChildrenAtHome]", hierarchy.UniqueName);
            Assert.AreSame(testModel, hierarchy.Model);
        }

        [TestMethod]
        public void DataTableHierarchyTestAxis()
        {
            DataColumn column = DataTable.Columns["Education"];
            TestDataModel testModel = new TestDataModel();
            DataTableHierarchy hierarchy = new DataTableHierarchy(testModel, column);
            TestAxis axis = new TestAxis();
            hierarchy.Axis = axis;
            Assert.AreSame(axis, hierarchy.Axis);
        }

        [TestMethod]
        public void DataTableHierarchyTestLevels()
        {
            DataColumn column = DataTable.Columns["MaritalStatus"];
            TestDataModel testModel = new TestDataModel();
            DataTableHierarchy hierarchy = new DataTableHierarchy(testModel, column);
            Assert.AreEqual(2, hierarchy.Levels.Count);
            Assert.IsInstanceOfType(hierarchy.Levels[0], typeof(DataTableAggregateLevel));
            Assert.IsInstanceOfType(hierarchy.Levels[1], typeof(DataTableLevel));
        }
    }
}

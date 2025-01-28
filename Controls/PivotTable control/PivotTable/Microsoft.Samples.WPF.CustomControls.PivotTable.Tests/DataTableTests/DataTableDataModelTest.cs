using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    [TestClass]
    public class DataTableDataModelTest : DataTableModelBaseTest
    {
        private DataTableDataModel _dataModel;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _dataModel = new DataTableDataModel(DataTable);
        }

        [TestMethod]
        public void DataTableDataModelTestDimensionsHierarchies()
        {
            string[] expectedNames = new string[] {
                "[MaritalStatus]", "[Education]", "[NumberChildrenAtHome]" };
            AssertHierarchiesNamesEquals(expectedNames);
        }

        private void AssertHierarchiesNamesEquals(string[] expectedNames)
        {
            List<string> actualNames = new List<string>();
            foreach (IHierarchy hierarchy in _dataModel.DimensionsHierarchies)
            {
                actualNames.Add(hierarchy.UniqueName);
            }
            CollectionAssert.AreEquivalent(expectedNames, actualNames);
        }

        [TestMethod]
        public void DataTableDataModelTestMeasuresHierarchy()
        {
            string[] expectedNames = new string[] {
                "[Measures].[Measures].[OrderQuantity]",
                "[Measures].[Measures].[SalesAmount]",
            };
            AssertMeasuresMembersNamesEquals(expectedNames);
        }

        private void AssertMeasuresMembersNamesEquals(string[] expectedNames)
        {
            List<string> actualNames = new List<string>();
            foreach (IMeasuresMember member in _dataModel.MeasuresHierarchy.Levels[0].Members)
            {
                actualNames.Add(member.UniqueName);
            }
            CollectionAssert.AreEquivalent(expectedNames, actualNames);
        }

        [TestMethod]
        public void DataTableDataModelTestFilterAxis()
        {
            Assert.IsInstanceOfType(_dataModel.FilterAxis, typeof(DataTableFilterAxis));
        }

        [TestMethod]
        public void DataTableDataModelTestRegularAxes()
        {
            Assert.IsTrue(_dataModel.RegularAxes.Count >= 2);
            foreach (IAxis regularAxis in _dataModel.RegularAxes)
            {
                Assert.IsInstanceOfType(regularAxis, typeof(DataTableAxis));
            }
        }

        [TestMethod]
        public void DataTableDataModelTestGetCell1()
        {
            _dataModel.RegularAxes[0].Hierarchies.Add(FindHierarchyByName("[MaritalStatus]"));
            _dataModel.RegularAxes[0].ExpandMember(FindHierarchyByName("[MaritalStatus]").Levels[0].Members[0]);
            _dataModel.RegularAxes[1].Hierarchies.Add(FindHierarchyByName("[Education]"));
            _dataModel.RegularAxes[1].ExpandMember(FindHierarchyByName("[Education]").Levels[0].Members[0]);
            _dataModel.RegularAxes[1].Hierarchies.Add(_dataModel.MeasuresHierarchy);
            _dataModel.RegularAxes[1].FilterMembers(null, new IMember[] { FindMeasuresMember("[Measures].[Measures].[SalesAmount]") });
            
            _dataModel.FilterAxis.Hierarchies.Add(FindHierarchyByName("[NumberChildrenAtHome]"));
            _dataModel.FilterAxis.FilterMembers(GetMembersWithoutChildren(), GetMembersWithChildren());
            _dataModel.FilterAxis.FilterMembers(null, new IMember[] { FindHierarchyByName("[NumberChildrenAtHome]").Levels[0].Members[0] });

            Tuple rowTuple1 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[All members].[All members]");
            Tuple rowTuple2 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[MaritalStatus].[M]");
            Tuple rowTuple3 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[MaritalStatus].[S]");

            Tuple columnTuple1 = FindTuple(_dataModel.RegularAxes[1], "[Education].[All members].[All members]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple2 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Bachelors]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple3 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Graduate Degree]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple4 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[High School]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple5 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Partial College]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple6 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Partial High School]", "[Measures].[Measures].[OrderQuantity]");

            Assert.AreEqual(32, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple1 }).Value);
            Assert.AreEqual(12, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple2 }).Value);
            Assert.AreEqual(5, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple3 }).Value);
            Assert.AreEqual(5, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple4 }).Value);
            Assert.AreEqual(7, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple5 }).Value);
            Assert.AreEqual(3, _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple6 }).Value);

            Assert.AreEqual(11, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple1 }).Value);
            Assert.AreEqual(4, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple2 }).Value);
            Assert.AreEqual(2, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple3 }).Value);
            Assert.AreEqual(2, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple4 }).Value);
            Assert.AreEqual(1, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple5 }).Value);
            Assert.AreEqual(2, _dataModel.GetCell(new Tuple[] { rowTuple2, columnTuple6 }).Value);

            Assert.AreEqual(21, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple1 }).Value);
            Assert.AreEqual(8, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple2 }).Value);
            Assert.AreEqual(3, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple3 }).Value);
            Assert.AreEqual(3, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple4 }).Value);
            Assert.AreEqual(6, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple5 }).Value);
            Assert.AreEqual(1, _dataModel.GetCell(new Tuple[] { rowTuple3, columnTuple6 }).Value);
        }

        [TestMethod]
        public void DataTableDataModelTestGetCell2()
        {
            _dataModel.RegularAxes[0].Hierarchies.Add(_dataModel.MeasuresHierarchy);
            _dataModel.RegularAxes[0].FilterMembers(null, new IMember[] { FindMeasuresMember("[Measures].[Measures].[SalesAmount]") });

            _dataModel.FilterAxis.Hierarchies.Add(FindHierarchyByName("[NumberChildrenAtHome]"));
            _dataModel.FilterAxis.FilterMembers(GetMembersWithoutChildren(), GetMembersWithChildren());
            _dataModel.FilterAxis.FilterMembers(null, new IMember[] { FindHierarchyByName("[NumberChildrenAtHome]").Levels[0].Members[0] });

            _dataModel.FilterAxis.Hierarchies.Add(FindHierarchyByName("[Education]"));
            _dataModel.FilterAxis.FilterMembers(GetMembersWithBachelorsOrHighSchool(), GetMembersWithoutBachelorsOrHighSchool());
            _dataModel.FilterAxis.FilterMembers(null, new IMember[] { FindHierarchyByName("[Education]").Levels[0].Members[0] });

            Tuple rowTuple = _dataModel.RegularAxes[0].Tuples[0];
            Assert.AreEqual(17, _dataModel.GetCell(new Tuple[] { rowTuple }).Value);
        }

        [TestMethod]
        public void DataTableDataModelTestGetCell3()
        {
            _dataModel.RegularAxes[0].Hierarchies.Add(FindHierarchyByName("[MaritalStatus]"));
            _dataModel.RegularAxes[0].ExpandMember(FindHierarchyByName("[MaritalStatus]").Levels[0].Members[0]);
            _dataModel.RegularAxes[1].Hierarchies.Add(FindHierarchyByName("[Education]"));
            _dataModel.RegularAxes[1].ExpandMember(FindHierarchyByName("[Education]").Levels[0].Members[0]);
            _dataModel.RegularAxes[1].Hierarchies.Add(_dataModel.MeasuresHierarchy);
            _dataModel.RegularAxes[1].FilterMembers(null, new IMember[] { FindMeasuresMember("[Measures].[Measures].[SalesAmount]") });

            _dataModel.FilterAxis.Hierarchies.Add(FindHierarchyByName("[NumberChildrenAtHome]"));
            _dataModel.FilterAxis.FilterMembers(GetMembersWithoutChildren(), GetMembersWithChildren());
            _dataModel.FilterAxis.FilterMembers(null, new IMember[] { FindHierarchyByName("[NumberChildrenAtHome]").Levels[0].Members[0] });

            Tuple rowTuple1 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[All members].[All members]");
            Tuple rowTuple2 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[MaritalStatus].[M]");
            Tuple rowTuple3 = FindTuple(_dataModel.RegularAxes[0], "[MaritalStatus].[MaritalStatus].[S]");

            Tuple columnTuple1 = FindTuple(_dataModel.RegularAxes[1], "[Education].[All members].[All members]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple2 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Bachelors]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple3 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Graduate Degree]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple4 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[High School]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple5 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Partial College]", "[Measures].[Measures].[OrderQuantity]");
            Tuple columnTuple6 = FindTuple(_dataModel.RegularAxes[1], "[Education].[Education].[Partial High School]", "[Measures].[Measures].[OrderQuantity]");

            Cell firstCell = _dataModel.GetCell(new Tuple[] { rowTuple1, columnTuple1 });
            Assert.AreEqual(rowTuple1, _dataModel.RegularAxes[0].Tuples[firstCell.Coordinates[0]]);
            Assert.AreEqual(columnTuple1, _dataModel.RegularAxes[1].Tuples[firstCell.Coordinates[1]]);
        }

        [TestMethod]
        public void DataTableDataModelTestGetCell4()
        {
            _dataModel.RegularAxes[0].Hierarchies.Add(_dataModel.MeasuresHierarchy);
            Tuple rowTuple1 = FindTuple(_dataModel.RegularAxes[0], "[Measures].[Measures].[OrderQuantity]");

            Assert.AreEqual(60, _dataModel.GetCell(new Tuple[] { rowTuple1 }).Value);
        }

        [TestMethod]
        public void DataTableDataModelTestCellsChanged1()
        {
            bool notified = false;
            _dataModel.CellsChanged += delegate { notified = true; };
            _dataModel.RegularAxes[0].Hierarchies.Add(_dataModel.MeasuresHierarchy);
            Assert.IsTrue(notified);
        }

        [TestMethod]
        public void DataTableDataModelTestCellsChanged2()
        {
            bool notified = false;
            _dataModel.CellsChanged += delegate { notified = true; };
            _dataModel.FilterAxis.Hierarchies.Add(FindHierarchyByName("[NumberChildrenAtHome]"));
            Assert.IsTrue(notified);
        }

        private Tuple FindTuple(IFilterAxis axis, params string[] membersNames)
        {
            foreach (Tuple tuple in axis.Tuples)
            {
                if (DoesTupleHaveMembers(tuple, membersNames))
                {
                    return tuple;
                }
            }
            return null;
        }

        private bool DoesTupleHaveMembers(Tuple tuple, string[] membersNames)
        {
            for (int i = 0; i < tuple.Length; i++)
            {
                if (tuple[i].UniqueName != membersNames[i])
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<IMember> GetMembersWithChildren()
        {
            IHierarchy hierarchy = FindHierarchyByName("[NumberChildrenAtHome]");
            foreach (IMember member in hierarchy.Levels[1].Members)
            {
                if (member.Caption != "0")
                    yield return member;
            }
        }

        private IEnumerable<IMember> GetMembersWithoutChildren()
        {
            IHierarchy hierarchy = FindHierarchyByName("[NumberChildrenAtHome]");
            foreach (IMember member in hierarchy.Levels[1].Members)
            {
                if (member.Caption == "0")
                    yield return member;
            }
        }

        private IEnumerable<IMember> GetMembersWithBachelorsOrHighSchool()
        {
            IHierarchy hierarchy = FindHierarchyByName("[Education]");
            foreach (IMember member in hierarchy.Levels[1].Members)
            {
                if (member.Caption == "Bachelors" || member.Caption == "High School")
                {
                    yield return member;
                }
            }
        }

        private IEnumerable<IMember> GetMembersWithoutBachelorsOrHighSchool()
        {
            IHierarchy hierarchy = FindHierarchyByName("[Education]");
            foreach (IMember member in hierarchy.Levels[1].Members)
            {
                if (member.Caption != "Bachelors" && member.Caption != "High School")
                {
                    yield return member;
                }
            }
        }

        private IMeasuresMember FindMeasuresMember(string uniqueName)
        {
            foreach (IMeasuresMember member in _dataModel.MeasuresHierarchy.Levels[0].Members)
            {
                if (member.UniqueName == uniqueName)
                {
                    return member;
                }
            }
            return null;
        }

        private IHierarchy FindHierarchyByName(string uniqueName)
        {
            foreach (IHierarchy hierarchy in _dataModel.DimensionsHierarchies)
            {
                if (hierarchy.UniqueName == uniqueName)
                {
                    return hierarchy;
                }
            }
            return null;
        }
    }
}

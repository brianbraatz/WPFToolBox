using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    /// <summary>
    /// Tests for Adomd.NET Data Model (AdomdDataModel class).
    /// </summary>
    [TestClass]
    public class AdomdDataModelTest
    {
        private string _connectionString = "Data Source=GBacevicius2; Initial Catalog=Adventure Works DW Standard Edition";
        private Random _random = new Random(44332);

        private AdomdDataModel CreateModel()
        {
            return new AdomdDataModel(new AdomdItemsSource(_connectionString, "Adventure Works"));
        }

        private IHierarchy FindDimensionHierarchy(AdomdDataModel model, string uniqueName)
        {
            foreach (IHierarchy hierarchy in model.DimensionsHierarchies)
            {
                if (hierarchy.UniqueName == uniqueName)
                {
                    return hierarchy;
                }
            }
            return null;
        }

        #region Testing hierarchies, levels and members
        /// <summary>
        /// Tests dimension hierarchies in Adomd.NET model.
        /// </summary>
        /// <remarks>
        /// The ExpectedHierarchies.xml file that used in this test contains subset of hierarchies,
        /// levels and members from the Adventure Works multidimensional database (which is distributed 
        /// with SQL Server 2005 Standard Edition). The file was created by the same code, and later
        /// manually validated.
        /// </remarks>
        [TestMethod]
        public void CompareHierarchiesTestFile()
        {
            AdomdDataModel model = CreateModel();
            XmlDocument actual = CreateDimensionHierarchiesDescription(model);
            actual.Save(@"..\..\ExpectedHierarchies.xml");
            XmlDocument expected = new XmlDocument();
            expected.LoadXml(EmbeddedTestFilesManager.GetFileContent("ExpectedHierarchies.xml"));
            Assert.AreEqual(expected.OuterXml, actual.OuterXml);
        }

        private XmlDocument CreateDimensionHierarchiesDescription(AdomdDataModel model)
        {
            XmlDocument result = new XmlDocument();
            result.AppendChild(result.CreateElement("hierarchies"));
            int maximumNumberOfHierarchies = 15;
            for (int i = 0; i < Math.Min(maximumNumberOfHierarchies, model.DimensionsHierarchies.Count); i++)
            {
                IHierarchy hierarchy = model.DimensionsHierarchies[i];
                result.DocumentElement.AppendChild(CreateDimensionHierarchyElement(hierarchy, result));
            }
            return result;
        }

        private XmlElement CreateDimensionHierarchyElement(IHierarchy hierarchy, XmlDocument elementsFactory)
        {
            XmlElement result = elementsFactory.CreateElement("hierarchy");
            result.SetAttribute("caption", hierarchy.Caption);
            result.SetAttribute("uniqueName", hierarchy.UniqueName);
            foreach (ILevel level in hierarchy.Levels)
            {
                result.AppendChild(CreateDimensionLevelElement(level, elementsFactory));
            }
            return result;
        }

        private XmlElement CreateDimensionLevelElement(ILevel level, XmlDocument elementsFactory)
        {
            XmlElement result = elementsFactory.CreateElement("level");
            result.SetAttribute("caption", level.Caption);
            result.SetAttribute("uniqueName", level.UniqueName);
            if (level.LevelNumber == 0)
            {
                int maximumMembers = 10;
                for (int i = 0; i < Math.Min(maximumMembers, level.Members.Count); i++)
                {
                    IMember member = level.Members[i];
                    result.AppendChild(CreateDimensionMemberElement(member, elementsFactory, i <= 2));
                }
            }
            return result;
        }

        private XmlElement CreateDimensionMemberElement(IMember member, XmlDocument elementsFactory, bool writeChildren)
        {
            XmlElement result = elementsFactory.CreateElement("member");
            result.SetAttribute("caption", member.Caption);
            result.SetAttribute("uniqueName", member.UniqueName);
            result.SetAttribute("levelUniqueName", member.Level.UniqueName);
            result.SetAttribute("parentUniqueName", member.ParentMember == null ? "" : member.ParentMember.UniqueName);
            if (writeChildren)
            {
                int maximumChildren = 10;
                for (int i = 0; i < Math.Min(maximumChildren, member.Children.Count); i++)
                {
                    IMember child = member.Children[i];
                    result.AppendChild(CreateDimensionMemberElement(child, elementsFactory, i <= 2));
                }
            }
            return result;
        }
        #endregion

        #region Testing measures

        [TestMethod]
        public void CreateMeasuresTestFile()
        {
            AdomdDataModel model = CreateModel();
            XmlDocument actual = CreateMeasuresHierarchyDescription(model);
            XmlDocument expected = new XmlDocument();
            expected.LoadXml(EmbeddedTestFilesManager.GetFileContent("ExpectedMeasures.xml"));
            Assert.AreEqual(expected.OuterXml, actual.OuterXml);
        }

        private XmlDocument CreateMeasuresHierarchyDescription(AdomdDataModel model)
        {
            XmlDocument result = new XmlDocument();
            result.AppendChild(result.CreateElement("measuresHierarchies"));
            result.DocumentElement.AppendChild(CreateMeasuresHierarchy(model.MeasuresHierarchy, result));
            return result;
        }

        private XmlElement CreateMeasuresHierarchy(IHierarchy hierarchy, XmlDocument elementsFactory)
        {
            XmlElement result = elementsFactory.CreateElement("hierarchy");
            result.SetAttribute("uniqueName", hierarchy.UniqueName);
            result.SetAttribute("caption", hierarchy.Caption);
            foreach (ILevel level in hierarchy.Levels)
            {
                result.AppendChild(CreateMeasuresLevel(level, elementsFactory));
            }
            return result;
        }

        private XmlElement CreateMeasuresLevel(ILevel level, XmlDocument elementsFactory)
        {
            XmlElement result = elementsFactory.CreateElement("level");
            result.SetAttribute("uniqueName", level.UniqueName);
            result.SetAttribute("caption", level.Caption);
            result.SetAttribute("hierarchy", level.Hierarchy.UniqueName);
            foreach (IMeasuresMember measure in level.Members)
            {
                result.AppendChild(CreateMeasureElement(measure, elementsFactory));
            }
            return result;
        }

        private XmlElement CreateMeasureElement(IMeasuresMember measure, XmlDocument elementsFactory)
        {
            XmlElement result = elementsFactory.CreateElement("measure");
            result.SetAttribute("canSetAggregateFunction", measure.CanSetAggregateFunction.ToString());
            result.SetAttribute("aggregateFunction", measure.AggregateFunction == null ? "null" : measure.AggregateFunction.Name);
            result.SetAttribute("parentMember", measure.ParentMember == null ? "null" : measure.ParentMember.UniqueName);
            result.SetAttribute("childrenCount", measure.Children.Count.ToString());
            result.SetAttribute("uniqueName", measure.UniqueName);
            result.SetAttribute("caption", measure.Caption);
            result.SetAttribute("level", measure.Level.UniqueName);
            return result;
        }

        #endregion

        #region Testing axes

        #region Testing the filter axis

        [TestMethod]
        public void FilterAxisForMeasuresTest()
        {
            AdomdDataModel model = CreateModel();
            model.FilterAxis.Hierarchies.Add(model.MeasuresHierarchy);
            Assert.AreEqual(model.MeasuresHierarchy.Levels[0].Members.Count, model.FilterAxis.Tuples.Count);
            int filteredMemberNumber = _random.Next(model.MeasuresHierarchy.Levels[0].Members.Count);
            IMember memberToFilter = model.MeasuresHierarchy.Levels[0].Members[filteredMemberNumber];

            model.FilterAxis.FilterMembers(null, new IMember[] { memberToFilter });
            Assert.AreEqual(model.MeasuresHierarchy.Levels[0].Members.Count - 1, model.FilterAxis.Tuples.Count);

            int tupleNumber = 0;
            for (int memberNumber = 0; memberNumber < model.MeasuresHierarchy.Levels[0].Members.Count; memberNumber++)
            {
                if (memberNumber == filteredMemberNumber)
                {
                    continue;
                }
                Tuple tuple = model.FilterAxis.Tuples[tupleNumber++];
                IMeasuresMember measure = (IMeasuresMember)model.MeasuresHierarchy.Levels[0].Members[memberNumber];
                Assert.AreEqual(1, tuple.Length);
                Assert.AreEqual(measure, tuple[0]);
            }

            model.FilterAxis.FilterMembers(new IMember[] { memberToFilter }, null);
            Assert.AreEqual(model.MeasuresHierarchy.Levels[0].Members.Count, model.FilterAxis.Tuples.Count);

            tupleNumber = 0;
            for (int memberNumber = 0; memberNumber < model.MeasuresHierarchy.Levels[0].Members.Count; memberNumber++)
            {
                Tuple tuple = model.FilterAxis.Tuples[tupleNumber++];
                IMeasuresMember measure = (IMeasuresMember)model.MeasuresHierarchy.Levels[0].Members[memberNumber];
                Assert.AreEqual(1, tuple.Length);
                Assert.AreEqual(measure, tuple[0]);
            }
        }

        [TestMethod]
        public void FilterAxisForDimensionsTest()
        {
            AdomdDataModel model = CreateModel();
            IFilterAxis axis = model.FilterAxis;

            IHierarchy firstHierarchy = FindDimensionHierarchy(model, "[Customer].[Marital Status]");
            IHierarchy secondHierarchy = FindDimensionHierarchy(model, "[Customer].[Education]");
            IMember firstTopLevelMember = firstHierarchy.Levels[0].Members[0];
            IMember secondTopLevelMember = secondHierarchy.Levels[0].Members[0];

            axis.Hierarchies.Add(firstHierarchy);
            axis.Hierarchies.Add(secondHierarchy);
            Assert.AreEqual(1, axis.Tuples.Count);
            Tuple tuple = axis.Tuples[0];
            Assert.AreEqual(firstTopLevelMember, tuple[0]);
            Assert.AreEqual(secondTopLevelMember, tuple[1]);

            axis.FilterMembers(null, new IMember[] { firstTopLevelMember });
            axis.FilterMembers(new IMember[] { firstTopLevelMember.Children[0] }, null);
            Assert.AreEqual(1, axis.Tuples.Count);
            tuple = axis.Tuples[0];
            Assert.AreEqual(firstTopLevelMember.Children[0], tuple[0]);
            Assert.AreEqual(secondTopLevelMember, tuple[1]);

            axis.FilterMembers(new IMember[] { secondTopLevelMember.Children[0] }, null);
            Assert.AreEqual(2, axis.Tuples.Count);
            Assert.AreEqual(firstTopLevelMember.Children[0], axis.Tuples[0][0]);
            Assert.AreEqual(secondTopLevelMember, axis.Tuples[0][1]);
            Assert.AreEqual(firstTopLevelMember.Children[0], axis.Tuples[1][0]);
            Assert.AreEqual(secondTopLevelMember.Children[0], axis.Tuples[1][1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddInvalidHierarchyToFilterAxisTest()
        {
            AdomdDataModel model = CreateModel();
            AddInvalidHierarchyToAnAxis(model.FilterAxis);
        }

        [TestMethod]
        public void ChangeHierarchieOrderInFilterAxisTest()
        {
            AdomdDataModel model = CreateModel();
            ChangeHierarchiesOrderTest(model, model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckFilterStatusOfNullMemberInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfNullMember(model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithNullLevelInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithNullLevel(model.FilterAxis);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithNullHierarchyInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithNullHierarchy(model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithInvalidHierarchyInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithInvalidHierarchy(model.FilterAxis);
        }

        [TestMethod]
        public void ClearHierarchiesInFilterAxisTest()
        {
            AdomdDataModel model = CreateModel();
            ClearHierarchiesTest(model, model.FilterAxis);
        }

        [TestMethod]
        public void RemoveHierarchyInFilterAxisTest()
        {
            AdomdDataModel model = CreateModel();
            RemoveHierarchyTest(model, model.FilterAxis);
        }

        [TestMethod]
        public void ReplaceHierarchyInFilterAxisTest()
        {
            AdomdDataModel model = CreateModel();
            ReplaceHierarchyTest(model, model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetFilterForNullMemberInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterForNullMember(model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithNullLevelInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithNullLevel(model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithNullHierarchyInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithNullHierarchy(model.FilterAxis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithInvalidHierarchyInFilterAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithInvalidHierarchy(model.FilterAxis);
        }

        #endregion

        #region Testing regular axes
        [TestMethod]
        public void RegularAxisMixedTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis axis = model.RegularAxes[0];

            IHierarchy firstHierarchy = model.MeasuresHierarchy;
            IHierarchy secondHierarchy = FindDimensionHierarchy(model, "[Customer].[Marital Status]");

            axis.Hierarchies.Add(firstHierarchy);
            axis.Hierarchies.Add(secondHierarchy);
            Assert.AreEqual(firstHierarchy.Levels[0].Members.Count, axis.Tuples.Count);

            axis.FilterMembers(null, new IMember[] { firstHierarchy.Levels[0].Members[0] });
            Assert.AreEqual(firstHierarchy.Levels[0].Members.Count - 1, axis.Tuples.Count);
            for (int i = 1; i < axis.Tuples.Count; i++)
            {
                Assert.AreEqual(firstHierarchy.Levels[0].Members[i], axis.Tuples[i - 1][0]);
                Assert.AreEqual(secondHierarchy.Levels[0].Members[0], axis.Tuples[i - 1][1]);
            }

            Assert.AreEqual(MemberState.Collapsed, axis.GetMemberState(secondHierarchy.Levels[0].Members[0]));
            Assert.AreEqual(MemberState.Hidden, axis.GetMemberState(secondHierarchy.Levels[0].Members[0].Children[0]));
            
            axis.ExpandMember(secondHierarchy.Levels[0].Members[0]);
            Assert.AreEqual(MemberState.Expanded, axis.GetMemberState(secondHierarchy.Levels[0].Members[0]));
            Assert.AreEqual(MemberState.Collapsed, axis.GetMemberState(secondHierarchy.Levels[0].Members[0].Children[0]));
            Assert.AreEqual((firstHierarchy.Levels[0].Members.Count - 1) * (secondHierarchy.Levels[0].Members[0].Children.Count + 1),
                axis.Tuples.Count);

            axis.CollapseMember(secondHierarchy.Levels[0].Members[0]);
            Assert.AreEqual(MemberState.Collapsed, axis.GetMemberState(secondHierarchy.Levels[0].Members[0]));
            Assert.AreEqual(MemberState.Hidden, axis.GetMemberState(secondHierarchy.Levels[0].Members[0].Children[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddInvalidHierarchyToRegularAxisTest()
        {
            AdomdDataModel model = CreateModel();
            AddInvalidHierarchyToAnAxis(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckFilterStatusOfNullMemberInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfNullMember(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithNullLevelInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithNullLevel(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithNullHierarchyInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithNullHierarchy(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckFilterStatusOfMemberWithInvalidHierarchyInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            CheckFilterStatusOfMemberWithInvalidHierarchy(model.RegularAxes[0]);
        }

        [TestMethod]
        public void ChangeHierarchiesOrderInRegularAxisTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis axis = model.RegularAxes[1];
            ChangeHierarchiesOrderTest(model, axis);
        }

        [TestMethod]
        public void ClearHierarchiesInRegularAxisTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis axis = model.RegularAxes[0];
            ClearHierarchiesTest(model, axis);
        }

        [TestMethod]
        public void RemoveHierarchyInRegularAxisTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis axis = model.RegularAxes[1];
            RemoveHierarchyTest(model, axis);
        }

        [TestMethod]
        public void ReplaceHierarchyInRegularAxisTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis axis = model.RegularAxes[0];
            ReplaceHierarchyTest(model, axis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetFilterForNullMemberInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterForNullMember(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithNullLevelInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithNullLevel(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithNullHierarchyInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithNullHierarchy(model.RegularAxes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilterStatusForMemberWithInvalidHierarchyInRegularAxis()
        {
            AdomdDataModel model = CreateModel();
            SetFilterStatusForMemberWithInvalidHierarchy(model.RegularAxes[0]);
        }

        #endregion

        private void AddInvalidHierarchyToAnAxis(IFilterAxis axis)
        {
            TestHierarchy hierarchy = new TestHierarchy("Test");
            axis.Hierarchies.Add(hierarchy);
        }

        private void CheckFilterStatusOfNullMember(IFilterAxis axis)
        {
            axis.IsMemberFiltered(null);
        }

        private void CheckFilterStatusOfMemberWithNullLevel(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            axis.IsMemberFiltered(member);
        }

        private void CheckFilterStatusOfMemberWithNullHierarchy(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            member.Level = new TestLevel("Test");
            axis.IsMemberFiltered(member);
        }

        private void CheckFilterStatusOfMemberWithInvalidHierarchy(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            TestLevel level = new TestLevel("Test");
            member.Level = level;
            level.Hierarchy = new TestHierarchy("Test");
            axis.IsMemberFiltered(member);
        }

        private void SetFilterForNullMember(IFilterAxis axis)
        {
            axis.FilterMembers(new IMember[] { null }, null);
        }

        private void SetFilterStatusForMemberWithNullLevel(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            axis.FilterMembers(new IMember[] { member }, null);
        }

        private void SetFilterStatusForMemberWithNullHierarchy(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            member.Level = new TestLevel("Test");
            axis.FilterMembers(null, new IMember[] { member });
        }

        private void SetFilterStatusForMemberWithInvalidHierarchy(IFilterAxis axis)
        {
            TestMember member = new TestMember("Test");
            TestLevel level = new TestLevel("Test");
            member.Level = level;
            level.Hierarchy = new TestHierarchy("Test");
            axis.FilterMembers(new IMember[] { member }, null);
        }

        private void ChangeHierarchiesOrderTest(AdomdDataModel model, IFilterAxis axis)
        {
            IHierarchy firstHierarchy = FindDimensionHierarchy(model, "[Customer].[Education]");
            IHierarchy secondHierarchy = FindDimensionHierarchy(model, "[Customer].[Marital Status]");

            axis.Hierarchies.Add(firstHierarchy);
            axis.Hierarchies.Add(secondHierarchy);
            axis.Hierarchies.Move(0, 1);
            Assert.AreEqual(1, axis.Tuples.Count);

            Tuple tuple = axis.Tuples[0];
            Assert.AreEqual(secondHierarchy.Levels[0].Members[0], tuple[0]);
            Assert.AreEqual(firstHierarchy.Levels[0].Members[0], tuple[1]);
        }

        private void ClearHierarchiesTest(AdomdDataModel model, IFilterAxis axis)
        {
            axis.Hierarchies.Add(model.MeasuresHierarchy);
            axis.Hierarchies.Clear();
            Assert.AreEqual(0, axis.Hierarchies.Count);
            Assert.AreEqual(0, axis.Tuples.Count);
        }

        private void RemoveHierarchyTest(AdomdDataModel model, IFilterAxis axis)
        {
            IHierarchy firstHierarchy = FindDimensionHierarchy(model, "[Customer].[Education]");
            IHierarchy secondHierarchy = FindDimensionHierarchy(model, "[Customer].[Marital Status]");

            axis.Hierarchies.Add(firstHierarchy);
            axis.Hierarchies.Add(secondHierarchy);
            axis.Hierarchies.Remove(firstHierarchy);

            Assert.AreEqual(1, axis.Hierarchies.Count);
            Assert.AreEqual(secondHierarchy, axis.Hierarchies[0]);
            Assert.AreEqual(1, axis.Tuples.Count);
            Assert.AreEqual(secondHierarchy.Levels[0].Members[0], axis.Tuples[0][0]);
        }

        private void ReplaceHierarchyTest(AdomdDataModel model, IFilterAxis axis)
        {
            IHierarchy dimensionHierarchy = FindDimensionHierarchy(model, "[Customer].[Education]");

            axis.Hierarchies.Add(model.MeasuresHierarchy);
            axis.Hierarchies[0] = dimensionHierarchy;
            Assert.AreEqual(dimensionHierarchy, axis.Hierarchies[0]);
            Assert.AreEqual(1, axis.Tuples.Count);
            Assert.AreEqual(dimensionHierarchy.Levels[0].Members[0], axis.Tuples[0][0]);
        }

        #endregion

        #region Testing access to the cells

        /// <summary>
        /// Testing result for model that should execute the following query:
        /// SELECT
        ///    {[Measures].[Customer Count]} ON AXIS(0),
        ///    {[Customer].[Marital Status].[All Customers],
        ///     [Customer].[Marital Status].&[M],
        ///     [Customer].[Marital Status].&[S]} ON AXIS(1)
        ///FROM
        ///    [Adventure Works]
        ///WHERE
        ///    {[Customer].[Education].&[Bachelors],
        ///     [Customer].[Education].&[Graduate Degree]}
        /// </summary>
        [TestMethod]
        public void GetCellTest()
        {
            AdomdDataModel model = CreateModel();
            IAxis firstAxis = model.RegularAxes[0];
            IAxis secondAxis = model.RegularAxes[2];
            IFilterAxis filterAxis = model.FilterAxis;

            IHierarchy firstHierarchy = model.MeasuresHierarchy;
            IHierarchy secondHierarchy = FindDimensionHierarchy(model, "[Customer].[Marital Status]");
            IHierarchy filterHierarchy = FindDimensionHierarchy(model, "[Customer].[Education]");

            firstAxis.Hierarchies.Add(firstHierarchy);            
            foreach (IMember member in firstHierarchy.Levels[0].Members)
            {
                if (member.UniqueName == "[Measures].[Customer Count]")
                {
                    firstAxis.FilterMembers(new IMember[] { member }, null);
                }
                else
                {
                    firstAxis.FilterMembers(null, new IMember[] { member });
                }
                Assert.AreEqual(member.UniqueName != "[Measures].[Customer Count]", firstAxis.IsMemberFiltered(member));
            }

            secondAxis.Hierarchies.Add(secondHierarchy);
            secondAxis.ExpandMember(secondHierarchy.Levels[0].Members[0]);
            
            filterAxis.Hierarchies.Add(filterHierarchy);
            filterAxis.FilterMembers(null, new IMember[] { filterHierarchy.Levels[0].Members[0] });
            Assert.IsTrue(filterAxis.IsMemberFiltered(filterHierarchy.Levels[0].Members[0]));

            IMember firstChildMember = filterHierarchy.Levels[0].Members[0].Children[0];
            IMember secondChildMember = filterHierarchy.Levels[0].Members[0].Children[1];
            Assert.AreEqual("[Customer].[Education].&[Bachelors]", firstChildMember.UniqueName);
            Assert.AreEqual("[Customer].[Education].&[Graduate Degree]", secondChildMember.UniqueName);

            filterAxis.FilterMembers(new IMember[] { firstChildMember }, null);
            filterAxis.FilterMembers(new IMember[] { secondChildMember }, null);
            Assert.IsFalse(filterAxis.IsMemberFiltered(firstChildMember));
            Assert.IsFalse(filterAxis.IsMemberFiltered(secondChildMember));

            int[] expectedValues = new int[] { 8545, 5033, 3512 };
            int nextValueIndex = 0;
            Tuple[] cellPosition = new Tuple[2];
            foreach (Tuple columnTuple in firstAxis.Tuples)
            {
                cellPosition[0] = columnTuple;
                foreach (Tuple rowTuple in secondAxis.Tuples)
                {
                    cellPosition[1] = rowTuple;
                    Cell cell = model.GetCell(cellPosition);
                    Assert.AreEqual(expectedValues[nextValueIndex++], cell.Value);
                }
            }
            Assert.AreEqual(expectedValues.Length, nextValueIndex);
        }

        #endregion
    }
}

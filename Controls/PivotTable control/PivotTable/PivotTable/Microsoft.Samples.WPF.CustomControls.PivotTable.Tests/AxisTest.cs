using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass]
    public class AxisTest : AxisBaseTest
    {
        private Axis _axis;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _axis = new InternalTestAxis();
            _axis.Hierarchies.Add(_firstHierarchy);
            _axis.Hierarchies.Add(_secondHierarchy);
        }

        protected override IFilterAxis FilterAxis
        {
            get { return _axis; }
        }

        [TestMethod]
        public void AxisTestCollapseAndExpandMember()
        {
            _axis.CollapseMember(_member111);
            Assert.AreEqual(MemberState.Collapsed, _axis.GetMemberState(_member111));
            Assert.AreEqual(MemberState.Hidden, _axis.GetMemberState(_member121));
            _axis.ExpandMember(_member111);
            Assert.AreEqual(MemberState.Expanded, _axis.GetMemberState(_member111));
            Assert.AreNotEqual(MemberState.Hidden, _axis.GetMemberState(_member121));
        }

        [TestMethod]
        public void AxisTestFilterMembers()
        {
            Assert.IsFalse(FilterAxis.IsMemberFiltered(_member211));
            FilterAxis.FilterMembers(null, new IMember[] { _member211 });
            Assert.IsTrue(FilterAxis.IsMemberFiltered(_member211));
            FilterAxis.FilterMembers(new IMember[] { _member211 }, null);
            Assert.IsFalse(FilterAxis.IsMemberFiltered(_member211));
        }

        [TestMethod]
        public void AxisTestTuples()
        {
            _axis.CollapseMember(_member111);
            Assert.AreEqual(2, _axis.Tuples.Count);
            AssertTupleNamesEqual(new string[] { "(Member1.1.1, Member2.1.1)", "(Member1.1.1, Member2.1.2)" });
            _axis.ExpandMember(_member111);
            _axis.FilterMembers(null, new IMember[] { _member212 });
            AssertTupleNamesEqual(new string[] {
                "(Member1.1.1, Member2.1.1)",
                "(Member1.2.1, Member2.1.1)",
                "(Member1.2.2, Member2.1.1)"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AxisTestCollapseInvalidMember()
        {
            _axis.CollapseMember(new TestMember("TestMember"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AxisTestExpandNullMember()
        {
            _axis.ExpandMember(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AxisTestFilterInvalidMember()
        {
            FilterAxis.FilterMembers(null, new IMember[] { new TestMember("TestMember") });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AxisTestFilterNullMemeber()
        {
            FilterAxis.FilterMembers(new IMember[] { null }, null);
        }

        private class InternalTestAxis : Axis
        {
            private ObservableCollection<IHierarchy> _hierarchies;

            public InternalTestAxis()
            {
                _hierarchies = new ObservableCollection<IHierarchy>();
                ((INotifyCollectionChanged)_hierarchies).CollectionChanged += OnHierarchiesChanged;
            }

            public override ObservableCollection<IHierarchy> Hierarchies
            {
                get { return _hierarchies; }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass]
    public class FilterAxisTest : AxisBaseTest
    {
        private InternalTestFilterAxis _axis;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _axis = new InternalTestFilterAxis();
            _axis.Hierarchies.Add(_firstHierarchy);
            _axis.Hierarchies.Add(_secondHierarchy);
        }

        protected override IFilterAxis FilterAxis
        {
            get { return _axis; }
        }

        [TestMethod]
        public void FilterAxisTestFilterMembers()
        {
            Assert.IsFalse(FilterAxis.IsMemberFiltered(_member211));
            FilterAxis.FilterMembers(null, new IMember[] { _member211 });
            Assert.IsTrue(FilterAxis.IsMemberFiltered(_member211));
            FilterAxis.FilterMembers(new IMember[] { _member211 }, null);
            Assert.IsFalse(FilterAxis.IsMemberFiltered(_member211));
        }

        [TestMethod]
        public void FilterAxisTestTuples()
        {
            _axis.FilterMembers(null, new IMember[] { _member121, _member122 });
            Assert.AreEqual(2, _axis.Tuples.Count);
            AssertTupleNamesEqual(new string[] { "(Member1.1.1, Member2.1.1)", "(Member1.1.1, Member2.1.2)" });
            _axis.FilterMembers(new IMember[] { _member121, _member122 }, null);
            _axis.FilterMembers(null, new IMember[] { _member212 });
            AssertTupleNamesEqual(new string[] {
                "(Member1.1.1, Member2.1.1)",
                "(Member1.2.1, Member2.1.1)",
                "(Member1.2.2, Member2.1.1)"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FilterAxisTestFilterInvalidMember()
        {
            FilterAxis.FilterMembers(null, new IMember[] { new TestMember("TestMember") });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterAxisTestFilterNullMemeber()
        {
            FilterAxis.FilterMembers(new IMember[] { null }, null);
        }

        private class InternalTestFilterAxis : FilterAxis
        {
            private ObservableCollection<IHierarchy> _hierarchies;

            public InternalTestFilterAxis()
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

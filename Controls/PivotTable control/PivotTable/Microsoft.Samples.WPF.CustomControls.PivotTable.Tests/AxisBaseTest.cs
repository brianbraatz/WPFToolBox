using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public abstract class AxisBaseTest
    {
        protected TestHierarchy _firstHierarchy;
        protected TestHierarchy _secondHierarchy;
        protected TestMember _member111;
        protected TestMember _member121;
        protected TestMember _member122;
        protected TestMember _member211;
        protected TestMember _member212;

        protected AxisBaseTest()
        { }

        [TestInitialize]
        public virtual void Initialize()
        {
            _firstHierarchy = new TestHierarchy("[Hierarchy1]");
            _secondHierarchy = new TestHierarchy("[Hierarchy2]");
            TestLevel firstLevel = new TestLevel("[Level1]");
            TestLevel secondLevel = new TestLevel("[Level2]");
            TestLevel thirdLevel = new TestLevel("[Level3]");
            _member111 = new TestMember("Member1.1.1");
            _member121 = new TestMember("Member1.2.1");
            _member122 = new TestMember("Member1.2.2");
            _member211 = new TestMember("Member2.1.1");
            _member212 = new TestMember("Member2.1.2");
            firstLevel.AddMember(_member111);
            _member111.Level = firstLevel;
            _member111.AddChild(_member121);
            _member121.ParentMember = _member111;
            _member111.AddChild(_member122);
            _member122.ParentMember = _member111;
            secondLevel.AddMember(_member121);
            _member121.Level = secondLevel;
            secondLevel.AddMember(_member122);
            _member122.Level = secondLevel;
            thirdLevel.AddMember(_member211);
            _member211.Level = thirdLevel;
            _member212.Level = thirdLevel;
            thirdLevel.AddMember(_member212);
            _firstHierarchy.AddLevel(firstLevel);
            firstLevel.Hierarchy = _firstHierarchy;
            _firstHierarchy.AddLevel(secondLevel);
            secondLevel.Hierarchy = _firstHierarchy;
            _secondHierarchy.AddLevel(thirdLevel);
            thirdLevel.Hierarchy = _secondHierarchy;
        }

        protected abstract IFilterAxis FilterAxis { get; }
        
        protected void AssertTupleNamesEqual(IList expectedNames)
        {
            List<string> actualNames = new List<string>();
            foreach (Tuple tuple in FilterAxis.Tuples)
            {
                actualNames.Add(tuple.ToString());
            }
            CollectionAssert.AreEquivalent(expectedNames, actualNames);
        }
    }
}

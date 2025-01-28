using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass]
    public class SetTest
    {
        [TestMethod]
        public void SetTestItemsConstructor()
        {
            Set<string> set = new Set<string>(new string[] { "a", "b", "c", "a" });
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.Contains("a"));
            Assert.IsTrue(set.Contains("b"));
            Assert.IsTrue(set.Contains("c"));
            Assert.IsFalse(set.Contains("d"));
        }

        [TestMethod]
        public void SetTestAdd()
        {
            Set<int> set = new Set<int>();
            Assert.IsFalse(set.Contains(4));
            Assert.IsFalse(set.Contains(3));
            set.Add(4);
            set.Add(3);
            set.Add(3);
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(3));
        }

        [TestMethod]
        public void SetTestAddItems()
        {
            Set<int> set = new Set<int>();
            set.Add(new int[] { 3, 4, 5, 3 });
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(5));
        }

        [TestMethod]
        public void SetTestRemove()
        {
            Set<int> set = new Set<int>();
            set.Add(3);
            set.Add(3);
            set.Add(4);
            set.Remove(3);
            Assert.AreEqual(1, set.Count);
            Assert.IsFalse(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
            set.Remove(4);
            Assert.IsFalse(set.Contains(4));
        }

        [TestMethod]
        public void SetTestSubtract()
        {
            Set<int> set1 = new Set<int>();
            Set<int> set2 = new Set<int>();
            set1.Add(new int[] { 1, 2, 3, 4, 5 });
            set2.Add(new int[] { 3, 4, 5, 6, 7 });
            set1.Subtract(set2);
            Assert.AreEqual(2, set1.Count);
            Assert.IsTrue(set1.Contains(1));
            Assert.IsTrue(set1.Contains(2));
            Assert.IsFalse(set1.Contains(3));
            Assert.IsFalse(set1.Contains(4));
            Assert.IsFalse(set1.Contains(5));
        }

        [TestMethod]
        public void SetTestUnion()
        {
            Set<int> set1 = new Set<int>();
            Set<int> set2 = new Set<int>();
            set1.Add(new int[] { 1, 2, 3, 4, 5 });
            set2.Add(new int[] { 3, 4, 5, 6, 7 });
            set1.Union(set2);
            Assert.AreEqual(7, set1.Count);
            Assert.IsTrue(set1.Contains(1));
            Assert.IsTrue(set1.Contains(2));
            Assert.IsTrue(set1.Contains(3));
            Assert.IsTrue(set1.Contains(4));
            Assert.IsTrue(set1.Contains(5));
            Assert.IsTrue(set1.Contains(6));
            Assert.IsTrue(set1.Contains(7));
        }

        [TestMethod]
        public void SetTestIntersect()
        {
            Set<int> set1 = new Set<int>();
            Set<int> set2 = new Set<int>();
            set1.Add(new int[] { 1, 2, 3, 4, 5 });
            set2.Add(new int[] { 3, 4, 5, 6, 7 });
            set1.Intersect(set2);
            Assert.AreEqual(3, set1.Count);
            Assert.IsTrue(set1.Contains(3));
            Assert.IsTrue(set1.Contains(4));
            Assert.IsTrue(set1.Contains(5));
            Assert.IsFalse(set1.Contains(1));
            Assert.IsFalse(set1.Contains(2));
            Assert.IsFalse(set1.Contains(6));
            Assert.IsFalse(set1.Contains(7));
        }

        [TestMethod]
        public void SetTestEnumeration()
        {
            Set<int> set1 = new Set<int>();
            set1.Add(new int[] { 4, 6, 1, 2 });
            List<int> allItems = new List<int>(set1);
            Assert.AreEqual(4, allItems.Count);
            Assert.IsTrue(allItems.Contains(1));
            Assert.IsTrue(allItems.Contains(2));
            Assert.IsTrue(allItems.Contains(4));
            Assert.IsTrue(allItems.Contains(6));
        }

        [TestMethod]
        public void SetTestClear()
        {
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            set.Clear();
            Assert.AreEqual(0, set.Count);
            Assert.IsFalse(set.Contains(1));
            Assert.IsFalse(set.Contains(2));
            Assert.IsFalse(set.Contains(3));
        }

        [TestMethod]
        public void SetTestCopyTo()
        {
            Set<int> set = new Set<int>(new int[] { 11, 12, 13 });
            int[] output = new int[3];
            set.CopyTo(output, 0);
            List<int> outputItems = new List<int>(output);
            Assert.IsTrue(outputItems.Contains(11));
            Assert.IsTrue(outputItems.Contains(12));
            Assert.IsTrue(outputItems.Contains(13));
        }
    }
}

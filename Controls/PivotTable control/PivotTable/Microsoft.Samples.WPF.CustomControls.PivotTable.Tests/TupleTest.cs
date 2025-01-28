using Microsoft.Samples.WPF.CustomControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass()]
    public class TupleTest
    {
        private Tuple CreateTuple(params string[] memberNames)
        {
            List<IMember> members = new List<IMember>();
            foreach (string memberName in memberNames)
            {
                members.Add(new TestMember(memberName));
            }
            return new Tuple(members);
        }

        /// <summary>
        /// Tests if overriden Object.Equals(object obj) method works correctly.
        /// </summary>
        [TestMethod()]
        public void OverridenEqualsTest()
        {
            Tuple tuple1 = CreateTuple("a", "b", "c");
            Tuple tuple2 = CreateTuple("a", "b", "c");
            Tuple tuple3 = CreateTuple("a", "a", "a");
            Assert.IsTrue(tuple1.Equals(tuple2));
            Assert.IsTrue(tuple2.Equals(tuple1));
            Assert.IsFalse(tuple1.Equals(tuple3));
            Assert.IsFalse(tuple2.Equals(null));
            Assert.IsFalse(tuple2.Equals("abc"));
        }

        /// <summary>
        /// Tests if the IEquatable<Tuple>.Equals(Tuple other) method works correctly.
        /// </summary>
        [TestMethod()]
        public void EquatableEqualsTest()
        {
            Tuple tuple1 = CreateTuple("a", "b", "c");
            Tuple tuple2 = CreateTuple("a", "b", "c");
            Tuple tuple3 = CreateTuple("a");
            Assert.IsTrue(tuple1.Equals(tuple2));
            Assert.IsTrue(tuple2.Equals(tuple1));
            Assert.IsFalse(tuple1.Equals(tuple3));
            Assert.IsFalse(tuple2.Equals(tuple3));
            Assert.IsFalse(tuple3.Equals(tuple1));
            Assert.IsFalse(tuple3.Equals(tuple2));
            Assert.IsFalse(tuple1.Equals(null));
            Assert.IsFalse(tuple2.Equals(null));
            Assert.IsFalse(tuple3.Equals(null));
        }

        /// <summary>
        /// Tests whether the Tuple correctly implements the IEnumerable<IMember> interface.
        /// </summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            string[] expected = new string[] { "a", "b", "c", "a" };
            int i = 0;
            Tuple tuple = CreateTuple(expected);
            foreach (IMember member in tuple)
            {
                Assert.IsTrue(i < expected.Length);
                Assert.AreEqual(member.UniqueName, expected[i]);
                i++;
            }
            Assert.AreEqual(expected.Length, i, "Tuple enumerator didn't return all members.");
        }

        /// <summary>
        /// Tests whether the Tuple correctly implements the IEnumerable interface.
        /// </summary>
        [TestMethod]
        public void GetEnumeratorTest1()
        {
            string[] expected = new string[] { "a", "b", "c", "a" };
            int i = 0;
            Tuple tuple = CreateTuple(expected);
            foreach (IMember member in (IEnumerable)tuple)
            {
                Assert.IsTrue(i < expected.Length);
                Assert.AreEqual(member.UniqueName, expected[i]);
                i++;
            }
            Assert.AreEqual(expected.Length, i, "Tuple enumerator didn't return all members.");
        }

        /// <summary>
        /// Tests if two tuple that are equal have the same hash code.
        /// </summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            Tuple tuple1 = CreateTuple("a", "b", "c");
            Tuple tuple2 = new Tuple(tuple1);
            Assert.AreEqual(tuple1.GetHashCode(), tuple2.GetHashCode(), "Hash codes for equal objects should be equal.");
        }

        /// <summary>
        /// Tests the Tuple.Length property.
        /// </summary>
        [TestMethod()]
        public void LengthTest()
        {
            Tuple tuple1 = CreateTuple("a", "b", "c", "a");
            Assert.AreEqual(4, tuple1.Length);
        }

        /// <summary>
        /// Tests the indexer property for Tuple.
        /// </summary>
        [TestMethod()]
        public void ItemTest()
        {
            string[] memberNames = new string[] { "a", "e", "g", "1", "5" };
            Tuple tuple = CreateTuple(memberNames);

            for (int i = 0; i < memberNames.Length; i++)
            {
                Assert.AreEqual(memberNames[i], tuple[i].UniqueName);
            }
        }

        /// <summary>
        /// Tests the Tuple.ToString() method.
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            string[] memberNames = new string[] { "a", "l", "a", " ", "m", "a", " ", "k", "o", "t", "a" };
            string expected = "(a, l, a,  , m, a,  , k, o, t, a)";
            Tuple tuple = CreateTuple(memberNames);
            Assert.AreEqual(expected, tuple.ToString());
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IMember head) constructor with valid argument.
        /// </summary>
        [TestMethod()]
        public void ConstructorTest1()
        {
            TestMember member = new TestMember("member");
            Tuple tuple = new Tuple(member);
            Assert.AreEqual(1, tuple.Length);
            Assert.AreEqual(member, tuple[0]);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(Tuple tail) copy constructor with valid argument.
        /// </summary>
        [TestMethod()]
        public void ConstructorTest2()
        {
            Tuple source = CreateTuple("a", "b", "c");
            Tuple copy = new Tuple(source);
            Assert.AreEqual(source, copy);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IMember head, Tuple tail) constructor with valid arguments.
        /// </summary>
        [TestMethod()]
        public void ConstructorTest3()
        {
            Tuple tail = CreateTuple("1", "2", "3");
            TestMember head = new TestMember("0");
            Tuple tuple = new Tuple(head, tail);
            Assert.AreEqual(tail.Length + 1, tuple.Length);
            Assert.AreEqual(head, tuple[0]);
            for (int i = 0; i < tail.Length; i++)
            {
                Assert.AreEqual(tail[i], tuple[i + 1]);
            }
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IEnumerable&lt;IMember&gt; members) constructor with valid arguments.
        /// </summary>
        [TestMethod]
        public void ConstructorTest4()
        {
            string[] memberNames = new string[] { "q", "w", "e", "r", "" };
            Tuple tuple = CreateTuple(memberNames);
            Assert.AreEqual(memberNames.Length, tuple.Length);
            for (int i = 0; i < memberNames.Length; i++)
            {
                Assert.AreEqual(memberNames[i], tuple[i].UniqueName);
            }
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IMember head) constructor with null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorInvokationTest1()
        {
            Tuple tuple = new Tuple((IMember)null);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(Tuple tuple) constructor with null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorInvokationTest2()
        {
            Tuple tuple = new Tuple((Tuple)null);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IMember head, Tuple tail) with null head.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorInvokationTest3()
        {
            Tuple tuple = new Tuple(null, CreateTuple("a"));
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IMember head, Tuple tail) with null tail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorInvokationTest4()
        {
            Tuple tuple = new Tuple(new TestMember("a"), null);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IEnumerable&lt;IMember&gt;) with null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorInvokationTest5()
        {
            Tuple tuple = new Tuple((IEnumerable<IMember>)null);
        }

        /// <summary>
        /// Tests the Tuple.Tuple(IEnumerable&lt;IMember&gt;) with collection
        /// that contains null members.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidConstructorInvokationTest6()
        {
            IMember[] members = new IMember[] { new TestMember("a"), null };
            Tuple tuple = new Tuple(members);
        }
    }
}

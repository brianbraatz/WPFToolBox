using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Samples.WPF.CustomControls;
using System.Collections;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass()]
    public class AverageAggregateFunctionTest
    {
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ConstructorTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor.CreatePrivate();
            Assert.AreEqual("Average", target.Name);
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForDoubleArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor(target);
            IEnumerable<double?> arguments = new double?[] {
                null, 4.0, 5.0, 1.2, null, 3.4, 5.6, -1.0, 0.0, null, null
            };

            object expected = 2.6;
            object actual = accessor.ComputeValueForDoubleArguments(arguments);
            Assert.AreEqual(expected, actual, "AverageAggregateFunction.ComputeValueForDoubleArguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForInt32ArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor(target);
            IEnumerable<int?> arguments = new int?[] { 
                4, null, 3, 2, 5, 6, 3, null
            };

            object expected = 23.0 / 6.0;
            object actual = accessor.ComputeValueForInt32Arguments(arguments);
            Assert.AreEqual(expected, actual, "AverageAggregateFunction.ComputeValueForInt32Arguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForSingleArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor(target);
            IEnumerable<float?> arguments = new float?[] {
                3.43f, null, 2.4f, 3.14f, 9.04f, null
            };

            object expected = 4.5025f;
            object actual = accessor.ComputeValueForSingleArguments(arguments);
            Assert.AreEqual(expected, actual, "AverageAggregateFunction.ComputeValueForSingleArguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForNullArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_AverageAggregateFunctionAccessor.CreatePrivate();
            IEnumerable arguments = new object[] { null, null, null };
            object actual = target.ComputeValue(arguments);
            Assert.IsNull(actual);
        }
    }

    [TestClass()]
    public class SumAggregateFunctionTest
    {
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForDoubleArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor(target);
            IEnumerable<double?> arguments = new double?[] {
                4.0, 3.0, 2.4, null, 1.2
            };

            object expected = 10.6;
            object actual = accessor.ComputeValueForDoubleArguments(arguments);

            Assert.AreEqual(expected, actual, "SumAggregateFunction.ComputeValueForDoubleArguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForInt32ArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor(target);
            IEnumerable<int?> arguments = new int?[] {
                null, null, 1, 2, 5, null
            };

            object expected = 8;
            object actual = accessor.ComputeValueForInt32Arguments(arguments);

            Assert.AreEqual(expected, actual, "SumAggregateFunction.ComputeValueForInt32Arguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForSingleArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor accessor = new Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor(target);
            IEnumerable<float?> arguments = new float?[] {
                null, 2f, 3f, 5f, 6f, 1f, null, 0f, null
            };

            object expected = 17.0f;
            object actual = accessor.ComputeValueForSingleArguments(arguments);
            Assert.AreEqual(expected, actual, "SumAggregateFunction.ComputeValueForSingleArguments did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForNullArgumentsTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor.CreatePrivate();
            IEnumerable arguments = new object[] { null, null, null, null };
            object actual = target.ComputeValue(arguments);
            Assert.IsNull(actual);
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ConstructorTest()
        {
            NumericAggregateFunction target = Microsoft_Samples_WPF_CustomControls_SumAggregateFunctionAccessor.CreatePrivate();
            Assert.AreEqual("Sum", target.Name, "Name of the SumAggregateFunction is incorrect.");
        }
    }

    [TestClass()]
    public class CountAggregateFunctionTest
    {
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueTest()
        {
            AggregateFunction target = Microsoft_Samples_WPF_CustomControls_CountAggregateFunctionAccessor.CreatePrivate();
            IEnumerable arguments = new object[] {
                null, 1, 2.0f, null, "Ala ma kota.", null, 4M
            };

            object expected = 4;
            object actual = target.ComputeValue(arguments);

            Assert.AreEqual(expected, actual, "CountAggregateFunction.ComputeValue did not return the expected value.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ComputeValueForNullArgumentsTest()
        {
            AggregateFunction target = Microsoft_Samples_WPF_CustomControls_CountAggregateFunctionAccessor.CreatePrivate();
            IEnumerable arguments = new object[] { null, null, null };
            object expected = 0;
            object actual = target.ComputeValue(arguments);
            Assert.AreEqual(expected, actual);
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ConstructorTest()
        {
            AggregateFunction target = Microsoft_Samples_WPF_CustomControls_CountAggregateFunctionAccessor.CreatePrivate();
            Assert.AreEqual("Count", target.Name, "The CountAggregateFunction has incorrect name.");
        }
    }

    [TestClass]
    public class NumericAggregateFunctionTest
    {
        private TestAggregateFunction _aggregateFunction;

        [TestInitialize]
        public void SetUp()
        {
            _aggregateFunction = new TestAggregateFunction();
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod]
        public void TestTypeForInt32()
        {
            List<object[]> testData = new List<object[]>();
            testData.Add(new object[] { 1, 2, 3, 4 });
            testData.Add(new object[] { (byte)1, (short)4, null, null, (uint)455, -1 });
            testData.Add(new object[] { null, null, null });

            foreach (object[] arguments in testData)
            {
                string expected = "Int32_" + CountNotNullItems(arguments).ToString();
                Assert.AreEqual(expected, _aggregateFunction.ComputeValue(arguments), "Arguments should be converted to Int32 type.");
            }
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod]
        public void TestTypeForSingle()
        {
            object[] arguments = new object[] { 1.0f, null, 2.0f };
            string expected = "Single_" + CountNotNullItems(arguments).ToString();
            Assert.AreEqual(expected, _aggregateFunction.ComputeValue(arguments), "Arguments should be converted to Single type.");
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod]
        public void TestTypeForDouble()
        {
            List<object[]> testData = new List<object[]>();
            testData.Add(new object[] { null, 1, 2.0f, 4.0 });
            testData.Add(new object[] { 3.0M, 3.0f, (long)1, (ulong)2, 1 });
            testData.Add(new object[] { 3.2M, null, null, null });

            foreach (object[] arguments in testData)
            {
                string expected = "Double_" + CountNotNullItems(arguments).ToString();
                Assert.AreEqual(expected, _aggregateFunction.ComputeValue(arguments), "Arguments should be converted to Double type.");
            }
        }

        private int CountNotNullItems(object[] items)
        {
            int result = 0;
            foreach (object item in items)
            {
                if (item != null)
                {
                    result++;
                }
            }
            return result;
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidType()
        {
            object[] arguments = new object[] { 1, 2, 3, 4, null, "Ala ma kota" };
            _aggregateFunction.ComputeValue(arguments);
        }

        private class TestAggregateFunction : NumericAggregateFunction
        {
            public TestAggregateFunction()
                : base("Test")
            { }

            protected override object ComputeValueForInt32Arguments(IEnumerable<int?> arguments)
            {
                int count = 0;
                foreach (int? argument in arguments)
                {
                    if (argument.HasValue)
                    {
                        count++;
                    }
                }
                return "Int32_" + count.ToString();
            }

            protected override object ComputeValueForSingleArguments(IEnumerable<float?> arguments)
            {
                int count = 0;
                foreach (float? argument in arguments)
                {
                    if (argument.HasValue)
                    {
                        count++;
                    }
                }
                return "Single_" + count.ToString();
            }

            protected override object ComputeValueForDoubleArguments(IEnumerable<double?> arguments)
            {
                int count = 0;
                foreach (double? argument in arguments)
                {
                    if (argument.HasValue)
                    {
                        count++;
                    }
                }
                return "Double_" + count.ToString();
            }
        }
    }

    [TestClass]
    public class AggregateFunctionTest
    {
        [TestMethod]
        public void TestPredefinedAggregateFunctions()
        {
            Assert.AreEqual("Average", AggregateFunction.Average.Name);
            Assert.AreEqual("Count", AggregateFunction.Count.Name);
            Assert.AreEqual("Sum", AggregateFunction.Sum.Name);
        }
    }
}

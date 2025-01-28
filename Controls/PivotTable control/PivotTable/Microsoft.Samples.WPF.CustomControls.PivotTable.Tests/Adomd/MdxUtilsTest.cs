using Microsoft.Samples.WPF.CustomControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Samples.WPF.CustomControls.DataModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{    
    [TestClass()]
    public class MdxUtilsTest
    {        
        /// <summary>
        /// Tests the MdxUtils.CreateTuplesSetExpression(StringBuilder result, IEnumerable&lt;Tuple&gt;) method.
        /// </summary>
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void GenerateTuplesSetExpressionTest()
        {
            StringBuilder output = new StringBuilder();
            List<Tuple> tuples = new List<Tuple>();
            tuples.Add(CreateTuple("a1", "a2", "a3"));
            tuples.Add(CreateTuple("b1", "b2", "b3"));
            tuples.Add(CreateTuple("c1", "c2", "c3"));
            string expected = "{(a1, a2, a3), \r\n(b1, b2, b3), \r\n(c1, c2, c3)}";
            Microsoft_Samples_WPF_CustomControls_DataModel_MdxUtilsAccessor.GenerateTuplesSetExpression(output, tuples);
            Assert.AreEqual(expected, output.ToString());
        }

        private Tuple CreateTuple(params string[] membersNames)
        {
            List<IMember> members = new List<IMember>();
            foreach (string memberName in membersNames)
            {
                members.Add(new TestMember(memberName));
            }
            return new Tuple(members);
        }
    }
}

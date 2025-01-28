//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: MdxUtils class.
//  Creator: t-tomkm
//  Date Created: 7/27/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Helper class for generating MDX queries.
    /// </summary>
    internal static class MdxUtils
    {
        /// <summary>
        /// Generates MDX expression for tuples set.
        /// </summary>
        /// <param name="result">String builder where the result should be written to.</param>
        /// <param name="tuples">Set of tuples.</param>
        public static void GenerateTuplesSetExpression(StringBuilder result, IEnumerable<Tuple> tuples)
        {
            bool firstTupleWritten = false;
            result.Append("{");
            foreach (Tuple tuple in tuples)
            {
                if (firstTupleWritten)
                {
                    result.AppendLine(", ");
                }
                else
                {
                    firstTupleWritten = true;
                }                
                GenerateTupleExpression(result, tuple);
            }
            result.Append("}");
        }

        private static void GenerateTupleExpression(StringBuilder result, Tuple tuple)
        {
            bool firstMemberWritten = false;
            result.Append("(");
            foreach (IMember member in tuple)
            {
                if (firstMemberWritten)
                {
                    result.Append(", ");
                }
                else
                {
                    firstMemberWritten = true;
                }
                result.Append(member.UniqueName);
            }
            result.Append(")");
        }
    }
}

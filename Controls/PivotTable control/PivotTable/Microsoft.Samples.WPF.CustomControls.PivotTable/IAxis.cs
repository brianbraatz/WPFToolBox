//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IAxis interface.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents an axis within a multidimensional data model.
    /// </summary>
    /// <remarks>
    /// <para>
    /// From SQL Server 2005 Books Online:
    /// An axis is essentially a set of ordered, congruent tuples used to organize the cells.
    /// </para>
    /// <para>
    /// Each regular axis is also a filter axis.
    /// </para>
    /// </remarks>
    public interface IAxis : IFilterAxis
    {
        /// <summary>
        /// Collapses member.
        /// </summary>
        /// <param name="member">Member to collapse.</param>
        void CollapseMember(IMember member);

        /// <summary>
        /// Expands member.
        /// </summary>
        /// <param name="member">Member to expand.</param>
        void ExpandMember(IMember member);

        /// <summary>
        /// Checks state of member.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>
        /// Returns MemberInTupleState.Collapsed if the member is collapsed, 
        /// MemberInTupleState.Expanded if the member is expanded, and 
        /// MemberInTupleState.Hidden if one of the member's ancestors within the 
        /// same tuple is collapsed.
        /// </returns>
        MemberState GetMemberState(IMember member);
    }
}

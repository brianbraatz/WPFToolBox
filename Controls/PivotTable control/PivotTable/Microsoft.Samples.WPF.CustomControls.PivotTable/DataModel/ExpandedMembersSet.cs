//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: ExpandedMembersSet class.
//  Creator: t-tomkm
//  Date Created: 7/28/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Helper class that stores state of members. It can be used by any IAxis implementation.
    /// </summary>
    /// <remarks>
    /// By default, all members are collapsed.
    /// </remarks>
    internal class ExpandedMembersSet
    {
        private Dictionary<IMember, bool> _expandedMembers;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExpandedMembersSet()
        {
            _expandedMembers = new Dictionary<IMember, bool>();
        }

        /// <summary>
        /// Collapses member.
        /// </summary>
        /// <param name="member">Member to collapse.</param>
        public void CollapseMember(IMember member)
        {
            Assert.ArgumentNotNull(member, "member", "Cannot collapse null member.");
            if (_expandedMembers.ContainsKey(member))
            {
                _expandedMembers.Remove(member);
            }
        }

        /// <summary>
        /// Expands member.
        /// </summary>
        /// <param name="member">Member to expand.</param>
        public void ExpandMember(IMember member)
        {
            Assert.ArgumentNotNull(member, "member", "Cannot expand null member.");
            if (!_expandedMembers.ContainsKey(member))
            {
                _expandedMembers.Add(member, true);
            }
        }

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
        public MemberState GetMemberState(IMember member)
        {
            Assert.ArgumentNotNull(member, "member", "Cannot return state for null member.");
            IMember parent = member.ParentMember;
            while (parent != null)
            {
                if (!_expandedMembers.ContainsKey(parent))
                {
                    return MemberState.Hidden;
                }
                parent = parent.ParentMember;
            }
            if (_expandedMembers.ContainsKey(member))
            {
                return MemberState.Expanded;
            }
            else
            {
                return MemberState.Collapsed;
            }
        }

        /// <summary>
        /// Notifies the set about hierarchy removal.
        /// </summary>
        /// <param name="hierarchy">Removed hierarchy.</param>
        public void OnHierarchyRemoved(IHierarchy hierarchy)
        {
            Assert.ArgumentNotNull(hierarchy, "hierarchy", "Hierarchy cannot be null.");
            List<IMember> membersToRemove = new List<IMember>();
            foreach (IMember member in _expandedMembers.Keys)
            {
                if (member.Level.Hierarchy == hierarchy)
                {
                    membersToRemove.Add(member);
                }
            }
            foreach (IMember member in membersToRemove)
            {
                _expandedMembers.Remove(member);
            }
        }

        /// <summary>
        /// Notifies the set about removal of all hierarchies.
        /// </summary>
        public void OnHierarchiesCleared()
        {
            _expandedMembers.Clear();
        }
    }
}

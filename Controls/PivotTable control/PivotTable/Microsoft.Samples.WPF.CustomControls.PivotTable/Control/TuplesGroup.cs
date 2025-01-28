//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: The TuplesGroup class.
//  Creator: t-tomkm
//  Date Created: 08/16/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// A group that describes member in a tuple. It is used as a data context
    /// for a tree view node.
    /// </summary>
    internal class TuplesGroup
    {
        #region Fields
        private IMember _member;
        private ILevel _level;
        private bool _isAggregateMember;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">Level of the tree view node.</param>
        /// <param name="member">Member to display.</param>
        public TuplesGroup(ILevel level, IMember member)
        {
            _member = member;
            _level = level;
            _isAggregateMember = _member.Level != _level;
        }

        #region Properties
        /// <summary>
        /// Gets caption of the node.
        /// </summary>
        public string Caption
        {
            get
            {
                return _isAggregateMember ? "" : _member.Caption;
            }
        }

        /// <summary>
        /// Gets member of the node.
        /// </summary>
        public IMember Member
        {
            get { return _member; }
        }

        /// <summary>
        /// Gets level of the node.
        /// </summary>
        /// <remarks>
        /// If the IsAggregateGroup property is true, member returned by the member property doesn't 
        /// belong to this level. The Level property rather indicates the level number into the tree 
        /// and helps to synchronize size of nodes.
        /// </remarks>
        public ILevel Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Checks whether this node is expandable.
        /// </summary>
        public bool IsExpandable
        {
            get { return (!_isAggregateMember) && _member.HasChildren; }
        }

        /// <summary>
        /// Checks whether this node is aggregate node.
        /// </summary>
        public bool IsAggregateGroup
        {
            get { return _isAggregateMember; }
        }
        #endregion

        #region Overriden Object methods
        /// <summary>
        /// Determines whether the specified Object is equal to the current node.
        /// </summary>
        /// <param name="obj">The Object to compare with the current node.</param>
        /// <returns>true if the specified Object is a TuplesGroup and has the same
        /// member as the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            TuplesGroup other = obj as TuplesGroup;
            if (other == null)
            {
                return false;
            }
            return _member == other._member;
        }

        /// <summary>
        /// Returns hash code for the current object.
        /// </summary>
        /// <returns>Hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return _member.GetHashCode();
        }
        #endregion
    }
}

//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: Tuple class.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents an ordered collection of members from different hierarchies within one axis.
    /// </summary>
    public class Tuple : IEnumerable<IMember>, IEquatable<Tuple>
    {
        private IMember[] _members;

        #region Constructors
        /// <summary>
        /// Constructor that creates tuple with only one member.
        /// </summary>
        /// <param name="head">The member to add to the tuple.</param>
        public Tuple(IMember head)
        {
            Assert.ArgumentNotNull(head, "head", "Head cannot be null.");
            _members = new IMember[1];
            _members[0] = head;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="tail">The tuple that should be copied.</param>
        public Tuple(Tuple tail)
        {
            Assert.ArgumentNotNull(tail, "tail", "Tail cannot be null.");
            _members = new IMember[tail._members.Length];
            for (int i = 0; i < _members.Length; i++)
            {
                _members[i] = tail._members[i];
            }
        }

        /// <summary>
        /// Constructor that creates tuple with given head (i.e. first member) and
        /// tail (i.e. all members except the first one).
        /// </summary>
        /// <param name="head">First element of the new tuple.</param>
        /// <param name="tail">Tuple that stores tail (all members except the first one)
        /// of the new tuple.</param>
        public Tuple(IMember head, Tuple tail)
        {
            Assert.ArgumentNotNull(head, "head", "Head cannot be null.");
            Assert.ArgumentNotNull(tail, "tail", "Tail cannot be null.");

            _members = new IMember[tail._members.Length + 1];
            _members[0] = head;
            for (int i = 0; i < tail._members.Length; i++)
            {
                _members[i + 1] = tail._members[i];
            }
        }

        /// <summary>
        /// Constructor that creates tuple with given members.
        /// </summary>
        /// <param name="members">
        /// Member of the new tuple.
        /// </param>
        public Tuple(IEnumerable<IMember> members)
        {
            Assert.ArgumentNotNull(members, "members", "Members cannot be null.");

            List<IMember> listOfMembers = new List<IMember>();
            foreach (IMember member in members)
            {
                Assert.ArgumentValid(member != null, "members", "Members returned form an enumerator cannot be null.");

                listOfMembers.Add(member);
            }
            _members = listOfMembers.ToArray();
        }

        /// <summary>
        /// Private constructor, used only for internal factory methods.
        /// </summary>
        private Tuple()
        {}

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Creates new tuple by replacing member at given position in this tuple.
        /// </summary>
        /// <remarks>
        /// This operation doesn't destroy this tuple.
        /// </remarks>
        /// <param name="index">Zero-based index at which member should be replaced.</param>
        /// <param name="newMember">The new member.</param>
        /// <returns>Tuple with replaced member at the given position.</returns>
        public Tuple Replace(int index, IMember newMember)
        {
            Assert.ArgumentNotNull(newMember, "newMember", "New member cannot be null.");
            Assert.ArgumentValid((index >= 0) && (index < _members.Length), "index", "Invalid index.");
            
            Tuple result = new Tuple();
            result._members = new IMember[_members.Length];
            for (int i = 0; i < index; i++)
            {
                result._members[i] = _members[i];
            }
            result._members[index] = newMember;
            for (int i = index + 1; i < _members.Length; i++)
            {
                result._members[i] = _members[i];
            }
            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets number of members within this tuple.
        /// </summary>
        public int Length
        {
            get { return _members.Length; }
        }

        /// <summary>
        /// Indexer. Gets member at the given position within the tuple.
        /// </summary>
        /// <param name="index">Position of the member.</param>
        /// <returns>
        /// Member at given position.
        /// </returns>
        public IMember this[int index]
        {
            get { return _members[index]; }
        }

        #endregion

        #region Overriden Object methods
        /// <summary>
        /// Determines whether the specified Object is equal to the current Tuple. 
        /// </summary>
        /// <param name="obj">The Object to compare with the current Tuple.</param>
        /// <returns>true if the specified Object is equal to the current Tuple; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Tuple);
        }

        /// <summary>
        /// Returns hash code for a tuple.
        /// </summary>
        /// <returns>A hash code for the current Tuple.</returns>
        public override int GetHashCode()
        {
            int result = 0;
            foreach (IMember member in _members)
            {
                result = result ^ member.GetHashCode();
            }
            return result;
        }

        /// <summary>
        /// Returns a string that represents the current Tuple.
        /// </summary>
        /// <returns>A string that represents the current Tuple.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            bool firstEntryWritten = false;
            builder.Append("(");
            foreach (IMember member in _members)
            {
                if (firstEntryWritten)
                {
                    builder.Append(", ");
                }
                builder.Append(member.UniqueName);
                firstEntryWritten = true;
            }
            builder.Append(")");
            return builder.ToString();
        }
        #endregion

        #region IEquatable<Tuple> Members

        /// <summary>
        /// Determines whether the specified Tuple is equal to the current Tuple. 
        /// </summary>
        /// <param name="other">The Tuple to compare with the current Tuple.</param>
        /// <returns>true if the specified Tuple is equal to the current Tuple; otherwise, false.</returns>
        public bool Equals(Tuple other)
        {
            if ((other == null) || (_members.Length != other._members.Length))
            {
                return false;
            }

            for (int i = 0; i < _members.Length; i++)
            {
                if (!_members[i].Equals(other._members[i]))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IEnumerable<IMember> Members

        /// <summary>
        /// Returns an enumerator that iterates through all members.
        /// </summary>
        /// <returns>An IEnumerator&lt;IMember&gt; object that can be used to iterate 
        /// through the collection. </returns>
        public IEnumerator<IMember> GetEnumerator()
        {
            foreach (IMember member in _members)
            {
                yield return member;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through all members.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate 
        /// through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IMember member in _members)
            {
                yield return member;
            }
        }

        #endregion
    }
}

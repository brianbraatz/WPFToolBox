//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: Axis class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Base IAxis implementation.
    /// </summary>
    public abstract class Axis : IAxis
    {
        #region Fields
        private ExtendedObservableCollection<Tuple> _tuples;
        private ReadOnlyObservableCollection<Tuple> _readOnlyTuplesView;
        private Dictionary<IHierarchy, Dictionary<IMember, bool>> _notVisibleMembers;
        private ExpandedMembersSet _expandedMembersSet;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Axis()
        {
            _tuples = new ExtendedObservableCollection<Tuple>();
            _readOnlyTuplesView = new ReadOnlyObservableCollection<Tuple>(_tuples);
            _expandedMembersSet = new ExpandedMembersSet();
            _notVisibleMembers = new Dictionary<IHierarchy, Dictionary<IMember, bool>>();
        }

        #region Validating members
        /// <summary>
        /// Validates a member passed as an argument.
        /// </summary>
        /// <param name="member">Member to validate.</param>
        private void ValidateMember(IMember member)
        {
            Assert.ArgumentNotNull(member, "member", "Member cannot be null");
            Assert.ArgumentValid((member.Level != null) && Hierarchies.Contains(member.Level.Hierarchy),
                "member", "Member doesn't belong to this axis.");
        }

        /// <summary>
        /// Validates collection of members passed as an argument.
        /// </summary>
        /// <param name="members">Collection of members to validate.</param>
        private void ValidateMembers(IEnumerable<IMember> members)
        {
            if (members == null)
            {
                return;
            }
            foreach (IMember member in members)
            {
                ValidateMember(member);
            }
        }
        #endregion

        #region Handling changes in hierarchies
        /// <summary>
        /// Event handler for Hierarchies.CollectionChanged event.
        /// </summary>
        protected void OnHierarchiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnHierarchyAdded((IHierarchy)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Move:
                    OnHierarchyMoved();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnHierarchyRemoved((IHierarchy)e.OldItems[0]);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnHierarchyReplaced((IHierarchy)e.OldItems[0], (IHierarchy)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnHierarchiesCleared();
                    break;
            }
        }

        private void OnHierarchyAdded(IHierarchy hierarchy)
        {
            hierarchy.Axis = this;
            _notVisibleMembers.Add(hierarchy, new Dictionary<IMember, bool>());
            RecreateTuples();
        }

        private void OnHierarchyMoved()
        {
            RecreateTuples();
        }

        private void OnHierarchyRemoved(IHierarchy hierarchy)
        {
            hierarchy.Axis = null;
            _notVisibleMembers.Remove(hierarchy);
            _expandedMembersSet.OnHierarchyRemoved(hierarchy);
            RecreateTuples();
        }

        private void OnHierarchyReplaced(IHierarchy oldHierarchy, IHierarchy newHierarchy)
        {
            oldHierarchy.Axis = null;
            newHierarchy.Axis = this;
            _notVisibleMembers.Remove(oldHierarchy);
            _notVisibleMembers.Add(newHierarchy, new Dictionary<IMember, bool>());
            RecreateTuples();
        }

        private void OnHierarchiesCleared()
        {
            _notVisibleMembers.Clear();
            _expandedMembersSet.OnHierarchiesCleared();
            RecreateTuples();
        }
        #endregion

        #region Creating tuples
        private void RecreateTuples()
        {
            List<Tuple> newTuples = new List<Tuple>();
            foreach (Tuple tuple in GetInitialTuples())
            {
                GenerateChildTuples(newTuples, tuple, 0);
            }
            _tuples.ReplaceAllItems(newTuples);
        }

        private IEnumerable<Tuple> GetInitialTuples()
        {
            return Hierarchies.Count > 0 ? CreateInitialTuplesStartingFromHierarchy(0) : new List<Tuple>();
        }

        private IList<Tuple> CreateInitialTuplesStartingFromHierarchy(int hierarchyNumber)
        {
            List<Tuple> result = new List<Tuple>();
            int nextHierarchyNumber = hierarchyNumber + 1;
            if (nextHierarchyNumber < Hierarchies.Count)
            {
                IList<Tuple> nextHierarchyTuples = CreateInitialTuplesStartingFromHierarchy(nextHierarchyNumber);
                foreach (IMember head in GetInitialMembersFromHierarchy(Hierarchies[hierarchyNumber]))
                {
                    foreach (Tuple tail in nextHierarchyTuples)
                    {
                        result.Add(new Tuple(head, tail));
                    }
                }
            }
            else
            {
                foreach (IMember head in GetInitialMembersFromHierarchy(Hierarchies[hierarchyNumber]))
                {
                    result.Add(new Tuple(head));
                }
            }
            return result;
        }

        private IEnumerable<IMember> GetInitialMembersFromHierarchy(IHierarchy hierarchy)
        {
            foreach (IMember member in hierarchy.Levels[0].Members)
            {
                if (!IsMemberFiltered(member))
                {
                    yield return member;
                }
            }
        }

        private void GenerateChildTuples(IList<Tuple> result, Tuple tuple, int startPosition)
        {
            result.Add(tuple);
            //int expandablePosition = FindFirstExpandablePosition(tuple);
            foreach (int expandablePosition in FindExpandablePositions(tuple, startPosition))
            {
                foreach (IMember child in tuple[expandablePosition].Children)
                {
                    if (!IsMemberFiltered(child))
                    {
                        GenerateChildTuples(result, tuple.Replace(expandablePosition, child), expandablePosition);
                    }
                }
            }
        }

        private IEnumerable<int> FindExpandablePositions(Tuple tuple, int startPosition)
        {
            for (int i = startPosition; i < tuple.Length; i++)
            {
                IMember member = tuple[i];
                if (GetMemberState(member) == MemberState.Expanded)
                {
                    if (member.Children.Count > 0)
                    {
                        yield return i;
                    }
                }
            }
        }
        #endregion

        #region IAxis Members

        /// <summary>
        /// Collapses member.
        /// </summary>
        /// <param name="member">Member to collapse.</param>
        public void CollapseMember(IMember member)
        {
            ValidateMember(member);
            _expandedMembersSet.CollapseMember(member);
            RecreateTuples();
        }

        /// <summary>
        /// Expands member.
        /// </summary>
        /// <param name="member">Member to expand.</param>
        public void ExpandMember(IMember member)
        {
            ValidateMember(member);
            _expandedMembersSet.ExpandMember(member);
            RecreateTuples();
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
            return _expandedMembersSet.GetMemberState(member);
        }

        #endregion

        #region IFilterAxis Members

        /// <summary>
        /// Gets all hierarchies within the axis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be used to add new hierarchies to the axis.
        /// </para>
        /// <para>
        /// A hierarchy can belong only to one axis. When user tries to add
        /// hierarchy that already belongs to different axis, the Add method
        /// should throw an InvalidOperationException.
        /// </para>
        /// </remarks>
        public abstract ObservableCollection<IHierarchy> Hierarchies { get; }

        /// <summary>
        /// Gets all tuples within the axis.
        /// </summary>
        public ReadOnlyObservableCollection<Tuple> Tuples
        {
            get { return _readOnlyTuplesView; }
        }

        /// <summary>
        /// Shows or hides (filters) members.
        /// </summary>
        /// <param name="visibleMember">Members that should be visible. This parameter can be null.</param>
        /// <param name="notVisibleMembers">Members that should be not visible. This parameter can be null.</param>
        /// /// <remarks>
        /// If the same member is in both visibleMembers and notVisibleMembers collections, it is treated as if
        /// it was only in visibleMembers collection.
        /// </remarks>
        public void FilterMembers(IEnumerable<IMember> visibleMembers, IEnumerable<IMember> notVisibleMembers)
        {
            ValidateMembers(visibleMembers);
            ValidateMembers(notVisibleMembers);

            bool tuplesChanged = false;

            if (notVisibleMembers != null)
            {
                foreach (IMember member in notVisibleMembers)
                {
                    IHierarchy hierarchy = member.Level.Hierarchy;
                    if (!_notVisibleMembers[hierarchy].ContainsKey(member))
                    {
                        _notVisibleMembers[hierarchy].Add(member, true);
                        tuplesChanged = true;
                    }
                }
            }

            if (visibleMembers != null)
            {
                foreach (IMember member in visibleMembers)
                {
                    IHierarchy hierarchy = member.Level.Hierarchy;
                    if (_notVisibleMembers[hierarchy].ContainsKey(member))
                    {
                        _notVisibleMembers[hierarchy].Remove(member);
                        tuplesChanged = true;
                    }
                }
            }

            if (tuplesChanged)
            {
                RecreateTuples();
            }
        }

        /// <summary>
        /// Checks whether the member is visible or hidden (filtered).
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>
        /// Returns true if the member is visible. If the member is hidden (filtered), returns
        /// false.
        /// </returns>
        public bool IsMemberFiltered(IMember member)
        {
            ValidateMember(member);
            return _notVisibleMembers[member.Level.Hierarchy].ContainsKey(member);
        }

        #endregion
    }
}

//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: FilterAxis class.
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
    /// Base, abstract class for filter axes.
    /// </summary>
    public abstract class FilterAxis : IFilterAxis
    {
        #region Fields
        private Dictionary<IHierarchy, Dictionary<IMember, bool>> _visibleMembers;
        private ExtendedObservableCollection<Tuple> _tuples;
        private ReadOnlyObservableCollection<Tuple> _readOnlyTuplesView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public FilterAxis()
        {
            _tuples = new ExtendedObservableCollection<Tuple>();
            _readOnlyTuplesView = new ReadOnlyObservableCollection<Tuple>(_tuples);
            _visibleMembers = new Dictionary<IHierarchy, Dictionary<IMember, bool>>();
        }

        #region Validating arguments
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

        private void ValidateMember(IMember member)
        {
            Assert.ArgumentNotNull(member, "member", "Member cannot be null.");
            Assert.ArgumentValid((member.Level != null) && Hierarchies.Contains(member.Level.Hierarchy),
                "member", "Member doesn't belong to this axis.");
        }
        #endregion

        #region Handling changes in hierarchies
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
            _visibleMembers.Add(hierarchy, CreateVisibleMembersForNewHierarchy(hierarchy));
            RecreateTuples();
        }

        private void OnHierarchyMoved()
        {
            RecreateTuples();
        }

        private void OnHierarchyRemoved(IHierarchy hierarchy)
        {
            hierarchy.Axis = null;
            _visibleMembers.Remove(hierarchy);
            RecreateTuples();
        }

        private void OnHierarchyReplaced(IHierarchy oldHierarchy, IHierarchy newHierarchy)
        {
            oldHierarchy.Axis = null;
            newHierarchy.Axis = this;
            _visibleMembers.Remove(oldHierarchy);
            _visibleMembers.Add(newHierarchy, CreateVisibleMembersForNewHierarchy(newHierarchy));
            RecreateTuples();
        }

        private void OnHierarchiesCleared()
        {
            _visibleMembers.Clear();
            RecreateTuples();
        }

        private Dictionary<IMember, bool> CreateVisibleMembersForNewHierarchy(IHierarchy hierarchy)
        {
            Dictionary<IMember, bool> result = new Dictionary<IMember, bool>();
            foreach (IMember firstLevelMember in hierarchy.Levels[0].Members)
            {
                result.Add(firstLevelMember, true);
            }
            return result;
        }
        #endregion

        #region Creating tuples
        private void RecreateTuples()
        {
            if (Hierarchies.Count > 0)
            {
                _tuples.ReplaceAllItems(CreateTuplesStartingFromHierarchy(0));
            }
            else
            {
                _tuples.Clear();
            }
        }

        private IList<Tuple> CreateTuplesStartingFromHierarchy(int hierarchyNumber)
        {
            List<Tuple> result = new List<Tuple>();
            int nextHierarchyNumber = hierarchyNumber + 1;
            if (nextHierarchyNumber < Hierarchies.Count)
            {
                foreach (IMember head in GetAllMembersFromHierarchy(Hierarchies[hierarchyNumber]))
                {
                    foreach (Tuple tail in CreateTuplesStartingFromHierarchy(nextHierarchyNumber))
                    {
                        result.Add(new Tuple(head, tail));
                    }
                }
            }
            else
            {
                foreach (IMember member in GetAllMembersFromHierarchy(Hierarchies[hierarchyNumber]))
                {
                    result.Add(new Tuple(member));
                }
            }
            return result;
        }

        private IEnumerable<IMember> GetAllMembersFromHierarchy(IHierarchy hierarchy)
        {
            return _visibleMembers[hierarchy].Keys;
        }
        #endregion

        #region IFilterAxis Members

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
        /// <remarks>
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
                    if (_visibleMembers[hierarchy].ContainsKey(member))
                    {
                        _visibleMembers[hierarchy].Remove(member);
                        tuplesChanged = true;
                    }
                }
            }
            if (visibleMembers != null)
            {
                foreach (IMember member in visibleMembers)
                {
                    IHierarchy hierarchy = member.Level.Hierarchy;
                    if (!_visibleMembers[hierarchy].ContainsKey(member))
                    {
                        _visibleMembers[hierarchy].Add(member, true);
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
            return !_visibleMembers[member.Level.Hierarchy].ContainsKey(member);
        }
        #endregion
    }
}

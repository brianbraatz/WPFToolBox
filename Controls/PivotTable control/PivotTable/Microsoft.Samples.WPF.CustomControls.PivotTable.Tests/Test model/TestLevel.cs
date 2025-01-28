using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public class TestLevel : ILevel
    {
        private string _uniqueName;
        private IHierarchy _hierarchy;
        private int _levelNumber;
        private ObservableCollection<IMember> _members;
        private ReadOnlyObservableCollection<IMember> _readOnlyMembersView;

        public TestLevel(string uniqueName)
            : this(uniqueName, null, -1)
        { }

        public TestLevel(string uniqueName, IHierarchy hierarchy, int levelNumber)
        {
            _uniqueName = uniqueName;
            _hierarchy = hierarchy;
            _levelNumber = levelNumber;
            _members = new ObservableCollection<IMember>();
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
        }

        #region ILevel Members

        public string Caption
        {
            get { return _uniqueName; }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public int LevelNumber
        {
            get { return _levelNumber; }
            set { _levelNumber = value; }
        }

        public IHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set { _hierarchy = value; }
        }

        public ReadOnlyObservableCollection<IMember> Members
        {
            get { return _readOnlyMembersView; }
        }

        #endregion

        public void AddMember(IMember member)
        {
            _members.Add(member);
        }

        public void RemoveMember(IMember member)
        {
            _members.Remove(member);
        }
    }
}

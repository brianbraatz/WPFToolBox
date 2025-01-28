using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public class TestMember : IMember, IEquatable<TestMember>
    {
        private string _name;
        private IMember _parent;
        private ILevel _level;
        private ObservableCollection<IMember> _children;
        private ReadOnlyObservableCollection<IMember> _childrenView;

        public TestMember(string name)
        {
            _name = name;
            _children = new ObservableCollection<IMember>();
            _childrenView = new ReadOnlyObservableCollection<IMember>(_children);
        }

        #region Overriden Object methods.
        public override bool Equals(object obj)
        {
            return Equals(obj as TestMember);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }
        #endregion

        #region IMember Members

        public string Caption
        {
            get { return _name; }
        }

        public string UniqueName
        {
            get { return _name; }
        }

        public ILevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public IMember ParentMember
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ReadOnlyObservableCollection<IMember> Children
        {
            get { return _childrenView; }
        }

        public bool HasChildren
        {
            get { return _childrenView.Count > 0; }
        }

        public void AddChild(IMember child)
        {
            _children.Add(child);
        }

        #endregion

        #region IEquatable<TestMember> Members

        public bool Equals(TestMember other)
        {
            if (other == null)
            {
                return false;
            }
            return _name == other._name;
        }

        #endregion
    }
}

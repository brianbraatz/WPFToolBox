using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public class TestAxis : IFilterAxis
    {
        #region IFilterAxis Members

        public ObservableCollection<IHierarchy> Hierarchies
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ReadOnlyObservableCollection<Tuple> Tuples
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void FilterMembers(IEnumerable<IMember> visibleMembers, IEnumerable<IMember> notVisibleMembers)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsMemberFiltered(IMember member)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

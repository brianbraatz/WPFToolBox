using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public class TestDataModel : IDataModel
    {
        #region IDataModel Members

        public ReadOnlyObservableCollection<IAxis> RegularAxes
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IFilterAxis FilterAxis
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ReadOnlyObservableCollection<IHierarchy> DimensionsHierarchies
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IHierarchy MeasuresHierarchy
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public Cell GetCell(IEnumerable<Tuple> cellPosition)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler CellsChanged;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    public class DataTableModelBaseTest
    {
        private DataTable _dataTable;
        private DataTableHelper _dataTableHelper;

        protected DataTable DataTable
        {
            get { return _dataTable; }
        }

        protected DataTableHelper DataTableHelper
        {
            get { return _dataTableHelper; }
        }

        [TestInitialize]
        public virtual void Initialize()
        {
            TestDataTableFactory factory = new TestDataTableFactory();
            _dataTable = factory.CreateTestDataTable("DataForDataTable.xsd", "DataForDataTable.xml");
            _dataTableHelper = new DataTableHelper(_dataTable);
        }
    }
}

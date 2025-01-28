using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    public class DataTableHelper
    {
        private DataTable _dataTable;

        public DataTableHelper(DataTable dataTable)
        {
            _dataTable = dataTable;
        }

        public IEnumerable<DataRow> FindRows(string columnName, object rowValue)
        {
            foreach (DataRow row in _dataTable.Rows)
            {
                if (Object.Equals(row[columnName], rowValue))
                {
                    yield return row;
                }
            }
        }

        public DataRow FindFirstRow(string columnName, object rowValue)
        {
            foreach (DataRow row in _dataTable.Rows)
            {
                if (Object.Equals(row[columnName], rowValue))
                {
                    return row;
                }
            }
            return null;
        }

        public Dictionary<DataRow, object> GetAllRowsDictionary()
        {
            Dictionary<DataRow, object> allRows = new Dictionary<DataRow, object>();
            foreach (DataRow row in _dataTable.Rows)
            {
                allRows.Add(row, null);
            }
            return allRows;
        }
    }
}

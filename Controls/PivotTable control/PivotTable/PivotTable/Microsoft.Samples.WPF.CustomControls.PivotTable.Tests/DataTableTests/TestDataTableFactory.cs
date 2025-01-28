using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Data;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.DataTableTests
{
    internal class TestDataTableFactory
    {
        public DataTable CreateTestDataTable(string schemaFileName, string dataFileName)
        {
            DataTable result = new DataTable();
            result.ReadXmlSchema(EmbeddedTestFilesManager.GetFileStream(schemaFileName));
            result.ReadXml(EmbeddedTestFilesManager.GetFileStream(dataFileName));
            return result;
        }
    }
}

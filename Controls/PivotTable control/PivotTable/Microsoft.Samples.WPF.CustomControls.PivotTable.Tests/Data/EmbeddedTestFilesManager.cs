using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Data
{
    public static class EmbeddedTestFilesManager
    {
        public static string GetFileContent(string fileName)
        {
            using (Stream stream = GetFileStream(fileName))
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public static Stream GetFileStream(string fileName)
        {
            Type type = typeof(EmbeddedTestFilesManager);
            string resourceName = type.Namespace + "." + fileName;
            return type.Assembly.GetManifestResourceStream(resourceName);
        }
    }
}

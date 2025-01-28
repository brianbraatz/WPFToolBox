//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdItemsSource class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Adomd = Microsoft.AnalysisServices.AdomdClient;
using System;

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// An Adomd.NET items source for PivotTable control. It consists
    /// of two elements - a connection to the Analysis Services server,
    /// and a cube name.
    /// </summary>
    public class AdomdItemsSource : IDisposable
    {
        private Adomd.AdomdConnection _connection;
        private string _cubeName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="cubeName">Name of the cube.</param>
        public AdomdItemsSource(string connectionString, string cubeName)
        {
            Assert.ArgumentNotNull(connectionString, "connectionString", "Connection string cannot be null.");
            Assert.ArgumentNotNull(cubeName, "cubeName", "Cube name cannot be null.");
            _connection = new Adomd.AdomdConnection(connectionString);
            _cubeName = cubeName;
        }

        /// <summary>
        /// Connection to the Analysis Services.
        /// </summary>
        public Adomd.AdomdConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Name of the cube.
        /// </summary>
        public string CubeName
        {
            get { return _cubeName; }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indicates whether this method was invoked by IDisposable.Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Dispose();
            }
        }

        #endregion
    }
}

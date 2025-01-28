//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableMeasuresHierarchy class.
//  Creator: t-tomkm
//  Date Created: 09/06/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IHierarchy implementation for DataTable model. It contains only levels
    /// that stores measures.
    /// </summary>
    public class DataTableMeasuresHierarchy : IHierarchy
    {
        #region Fields
        private IDataModel _model;
        private IFilterAxis _axis;
        private ObservableCollection<ILevel> _levels;
        private ReadOnlyObservableCollection<ILevel> _readOnlyLevelsView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Data model that contains this hierarchy.</param>
        /// <param name="columns">Columns that define measures. DataType for each column
        /// must be numeric.</param>
        public DataTableMeasuresHierarchy(IDataModel model, IEnumerable<DataColumn> columns)
        {
            Assert.ArgumentNotNull(model, "model", "Model cannot be null.");
            Assert.ArgumentNotNull(columns, "columns", "Columns collection cannot be null.");
            _model = model;
            _levels = new ObservableCollection<ILevel>();
            _levels.Add(new DataTableMeasuresLevel(this, columns));
            _readOnlyLevelsView = new ReadOnlyObservableCollection<ILevel>(_levels);
        }

        #region IHierarchy Members

        /// <summary>
        /// Gets caption of the hierarchy.
        /// </summary>
        public string Caption
        {
            get { return "Measures"; }
        }

        /// <summary>
        /// Gets unique name (key) of the hierarchy.
        /// </summary>
        public string UniqueName
        {
            get { return string.Format("[{0}]", Caption); }
        }

        /// <summary>
        /// Gets display folder used to group hierarchies.
        /// </summary>
        public string DisplayFolder
        {
            get { return "Measures"; }
        }

        /// <summary>
        /// Gets or sets axis that contains the hierarchy.
        /// </summary>
        public IFilterAxis Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }

        /// <summary>
        /// Gets all levels within the hierarchy.
        /// </summary>
        public ReadOnlyObservableCollection<ILevel> Levels
        {
            get { return _readOnlyLevelsView; }
        }

        /// <summary>
        /// Gets model that contains this hierarchy.
        /// </summary>
        public IDataModel Model
        {
            get { return _model; }
        }

        #endregion
    }
}

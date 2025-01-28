//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DataTableHierarchy class. 
//  Creator: t-tomkm
//  Date Created: 09/01/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// IHierarchy implementation for DataTable model. It stores dimension members.
    /// </summary>
    public class DataTableHierarchy : IHierarchy
    {
        #region Fields
        private IFilterAxis _axis;
        private IDataModel _model;
        private DataColumn _column; 
        private ObservableCollection<ILevel> _levels;
        private ReadOnlyObservableCollection<ILevel> _readOnlyLevelsView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">Data model that contains this hierarchy.</param>
        /// <param name="column">Column that defines this hierarchy.</param>
        public DataTableHierarchy(IDataModel model, DataColumn column)
        {
            _model = model;
            _column = column;
            _levels = new ObservableCollection<ILevel>();
            FillLevels();
            _readOnlyLevelsView = new ReadOnlyObservableCollection<ILevel>(_levels);
        }

        private void FillLevels()
        {
            DataTableLevel secondLevel = new DataTableLevel(this, _column);
            DataTableAggregateLevel firstLevel = new DataTableAggregateLevel(this, GetDataTableMembers(secondLevel));
            _levels.Add(firstLevel);
            _levels.Add(secondLevel);
        }

        private IEnumerable<DataTableMember> GetDataTableMembers(DataTableLevel level)
        {
            foreach (DataTableMember member in level.Members)
            {
                yield return member;
            }
        }

        #region IHierarchy Members
        /// <summary>
        /// Gets caption of the hierarchy.
        /// </summary>
        public string Caption
        {
            get { return _column.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the hierarchy.
        /// </summary>
        public string UniqueName
        {
            get { return string.Format("[{0}]", Caption.Replace('.', '_')); }
        }

        /// <summary>
        /// Gets display folder used to group hierarchies.
        /// </summary>
        public string DisplayFolder
        {
            get { return "Dimensions"; }
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

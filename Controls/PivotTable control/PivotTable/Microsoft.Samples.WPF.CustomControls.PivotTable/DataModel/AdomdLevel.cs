//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdLevel class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// The ILevel implementation for Adomd.NET model.
    /// </summary>
    internal class AdomdLevel : ILevel
    {
        #region Fields
        private Level _sourceLevel;
        private AdomdObjectsRepository _repository;
        private AdomdHierarchy _hierarchy;
        private ObservableCollection<IMember> _members;
        private ReadOnlyObservableCollection<IMember> _readOnlyMembersView;
        
        private bool _initializedMembers = false;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceLevel">The Adomd.NET level object.</param>
        /// <param name="hierarchy">The hierarchy that contains this level.</param>
        /// <param name="repository">Repository used to associate Adomd.NET objects to 
        /// model objects.</param>
        public AdomdLevel(Level sourceLevel, AdomdHierarchy hierarchy, AdomdObjectsRepository repository)
        {
            _sourceLevel = sourceLevel;
            _hierarchy = hierarchy;
            _repository = repository;
            _repository.Register(sourceLevel, this);
        }

        private void InitializeAllMembers()
        {
            Assert.OperationValid(!_initializedMembers, "Members of this level are already initialized.");
            _members = new ObservableCollection<IMember>();
            foreach (Member sourceMember in _sourceLevel.GetMembers())
            {                
                AdomdMember modelMember = _repository.GetMember(sourceMember);
                _members.Add(modelMember);
            }
            _readOnlyMembersView = new ReadOnlyObservableCollection<IMember>(_members);
            _initializedMembers = true;
        }

        #region ILevel Members

        /// <summary>
        /// Gets caption of the level.
        /// </summary>
        public string Caption
        {
            get { return _sourceLevel.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the level.
        /// </summary>
        public string UniqueName
        {
            get { return _sourceLevel.UniqueName; }
        }

        /// <summary>
        /// Gets the ordinal position of the level within the parent hierarchy.
        /// </summary>
        public int LevelNumber
        {
            get { return _sourceLevel.LevelNumber; }
        }

        /// <summary>
        /// Gets the hierarchy that contains this level.
        /// </summary>
        public IHierarchy Hierarchy
        {
            get { return _hierarchy; }
        }

        /// <summary>
        /// Gets all members within this level.
        /// </summary>
        public ReadOnlyObservableCollection<IMember> Members
        {
            get 
            {
                if (!_initializedMembers)
                {
                    InitializeAllMembers();
                }
                return _readOnlyMembersView; 
            }
        }

        #endregion
    }
}

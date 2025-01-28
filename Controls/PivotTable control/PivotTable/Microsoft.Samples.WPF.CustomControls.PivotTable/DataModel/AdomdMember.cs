//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdMember class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// The IMember implementation for Adomd.NET model.
    /// </summary>
    internal class AdomdMember : IMember
    {
        #region Fields
        private AdomdObjectsRepository _repository;
        
        private Member _sourceMember;
        private AdomdLevel _level;
        private AdomdMember _parentMember;
        
        private ObservableCollection<IMember> _children;
        private ReadOnlyObservableCollection<IMember> _readOnlyChildrenView;

        private bool _initializedChildren = false;
        private bool _initializedParent = false;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceMember">An Adomd.NET source member.</param>
        /// <param name="level">Level that contains this member.</param>
        /// <param name="repository">Repository used to associate Adomd.NET and 
        /// model objects.</param>
        public AdomdMember(Member sourceMember, AdomdLevel level, AdomdObjectsRepository repository)
        {
            _sourceMember = sourceMember;
            _level = level;
            _repository = repository;
            _repository.Register(sourceMember, this);
        }

        #region Lazy initialization
        private void InitializeChildren()
        {
            Assert.OperationValid(!_initializedChildren, "Children of this member are already initialized.");

            _children = new ObservableCollection<IMember>();
            foreach (Member sourceChild in _sourceMember.GetChildren())
            {
                AdomdMember modelChild = _repository.GetMember(sourceChild);
                _children.Add(modelChild);
            }
            _readOnlyChildrenView = new ReadOnlyObservableCollection<IMember>(_children);
            _initializedChildren = true;
        }

        private void InitializeParent()
        {
            Assert.OperationValid(!_initializedParent, "Parent of this member is already initialized.");

            _parentMember = _repository.GetMember(_sourceMember.Parent);
            _initializedParent = true;
        }
        #endregion

        #region Overriden Object methods

        /// <summary>
        /// Determines whether the specified object is equal to the current AdomdMember. 
        /// </summary>
        /// <param name="obj">The object to compare with the current member.</param>
        /// <returns>true, if the specified object is equal to the current member; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            AdomdMember other = obj as AdomdMember;
            if (other == null)
            {
                return false;
            }
            return UniqueName.Equals(other.UniqueName);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current member.</returns>
        public override int GetHashCode()
        {
            return UniqueName.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current member.
        /// </summary>
        /// <returns>A string that represents the current member.</returns>
        public override string ToString()
        {
            return _sourceMember.UniqueName;
        }

        #endregion

        #region IMember Members

        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        public string Caption
        {
            get { return _sourceMember.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the member.
        /// </summary>
        public string UniqueName
        {
            get { return _sourceMember.UniqueName; }
        }

        /// <summary>
        /// Gets the level that contains this member.
        /// </summary>
        public ILevel Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Gets the parent member of this member.
        /// </summary>
        /// <remarks>
        /// This property is initialized on demand (lazy load).
        /// </remarks>
        public virtual IMember ParentMember
        {
            get 
            { 
                if (!_initializedParent)
                {
                    InitializeParent();               
                }
                return _parentMember;
            }
        }

        /// <summary>
        /// Gets all members that are children of this member (i.e. this member
        /// is their parent member).
        /// </summary>
        /// <remarks>
        /// Children are initialized on demand (lazy load).
        /// </remarks>
        public ReadOnlyObservableCollection<IMember> Children
        {
            get 
            {
                if (!_initializedChildren)
                {                    
                    InitializeChildren();
                }
                return _readOnlyChildrenView; 
            }
        }

        /// <summary>
        /// Gets number of children. It allows to check if the member is not leaf without
        /// retreiving all children.
        /// </summary>
        public bool HasChildren
        {
            get { return _sourceMember.ChildCount > 0; }
        }

        #endregion
    }
}

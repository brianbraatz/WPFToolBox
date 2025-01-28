//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdMeasuresMember class.
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
    /// The IMeasuresMember implementation for Adomd.NET model.
    /// </summary>
    internal class AdomdMeasuresMember : IMeasuresMember
    {
        #region Fields
        private Measure _sourceMeasure;
        private AdomdMeasuresLevel _level;
        private ObservableCollection<IMember> _children;
        private ReadOnlyObservableCollection<IMember> _readOnlyChildrenView;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceMeasure">Source Adomd.NET measure.</param>
        /// <param name="level">Level that contains this measure member.</param>
        public AdomdMeasuresMember(Measure sourceMeasure, AdomdMeasuresLevel level, AdomdObjectsRepository repository)
        {
            _sourceMeasure = sourceMeasure;
            _level = level;
            _children = new ObservableCollection<IMember>();
            _readOnlyChildrenView = new ReadOnlyObservableCollection<IMember>(_children);
            repository.Register(this);
        }

        #region Overriden Object methods

        /// <summary>
        /// Determines whether the specified object is equal to the current AdomdMember. 
        /// </summary>
        /// <param name="obj">The object to compare with the current member.</param>
        /// <returns>true, if the specified object is equal to the current member; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            AdomdMeasuresMember other = obj as AdomdMeasuresMember;
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
            return _sourceMeasure.UniqueName;
        }

        #endregion

        #region IMeasuresMember Members

        /// <summary>
        /// Gets folder that is used to group measures.
        /// </summary>
        public string DisplayFolder
        {
            get 
            {
                if (_sourceMeasure.Properties["MEASUREGROUP_NAME"] != null)
                {
                    return _sourceMeasure.Properties["MEASUREGROUP_NAME"].Value.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this member supports custom aggregate function.
        /// </summary>
        /// <remarks>
        /// The AdomdMeasuresMember doesn't support custom aggregate functions.
        /// </remarks>
        public bool CanSetAggregateFunction
        {
            get { return false; }
        }

        /// <summary>
        /// Gets and sets aggregate function for this member.
        /// </summary>
        /// <remarks>
        /// The AdomdMeasuresMember doesn't support custom aggregate functions.
        /// </remarks>
        public AggregateFunction AggregateFunction
        {
            get { return null; }
            set
            {
                throw new InvalidOperationException("Measures member for Adomd.NET doesn't support custom aggregate functions.");
            }
        }
        #endregion

        #region IMember Members
        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        public string Caption
        {
            get { return _sourceMeasure.Caption; }
        }

        /// <summary>
        /// Gets unique name (key) of the member.
        /// </summary>
        public string UniqueName
        {
            get { return _sourceMeasure.UniqueName; }
        }

        /// <summary>
        /// Gets the level that contains this member.
        /// </summary>
        public ILevel Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Gets the parent member of this member. It is always null for an AdomdMeasuresMember.
        /// </summary>
        public IMember ParentMember
        {
            get { return null; }
        }

        /// <summary>
        /// Gets all members that are children of this member (i.e. this member
        /// is their parent member). The collection is always empty for AdomdMeasuresMember.
        /// </summary>
        public ReadOnlyObservableCollection<IMember> Children
        {
            get { return _readOnlyChildrenView; }
        }

        /// <summary>
        /// Gets number of children. It allows to check if the member is not leaf without
        /// retreiving all children.
        /// </summary>
        public bool HasChildren
        {
            get { return false; }
        }

        #endregion
    }
}

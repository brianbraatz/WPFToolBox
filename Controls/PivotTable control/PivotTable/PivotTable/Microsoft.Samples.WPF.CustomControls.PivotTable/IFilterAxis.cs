//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IFilterAxis interface.
//  Creator: t-tomkm
//  Date Created: 7/27/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents a filter axis within a multidimensional data model.
    /// </summary>
    public interface IFilterAxis
    {
        /// <summary>
        /// Gets all hierarchies within the axis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be used to add new hierarchies to the axis.
        /// </para>
        /// <para>
        /// A hierarchy can belong only to one axis. When user tries to add
        /// hierarchy that already belongs to different axis, the Add method
        /// should throw an InvalidOperationException.
        /// </para>
        /// </remarks>
        ObservableCollection<IHierarchy> Hierarchies { get; }

        /// <summary>
        /// Gets all tuples within the axis.
        /// </summary>
        ReadOnlyObservableCollection<Tuple> Tuples { get; }

        /// <summary>
        /// Shows or hides (filters) members.
        /// </summary>
        /// <param name="visibleMember">Members that should be visible. This parameter can be null.</param>
        /// <param name="notVisibleMembers">Members that should be not visible. This parameter can be null.</param>
        /// <remarks>
        /// If the same member is in both visibleMembers and notVisibleMembers collections, it is treated as if
        /// it was only in visibleMembers collection.
        /// </remarks>
        void FilterMembers(IEnumerable<IMember> visibleMembers, IEnumerable<IMember> notVisibleMembers);

        /// <summary>
        /// Checks whether the member is visible or hidden (filtered).
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>
        /// Returns true if the member is visible. If the member is hidden (filtered), returns
        /// false.
        /// </returns>
        bool IsMemberFiltered(IMember member);
    }
}

//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IMember interface.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents a single member within a level.
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// Gets caption of the member.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Gets unique name (key) of the member.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Gets the level that contains this member.
        /// </summary>
        ILevel Level { get; }

        /// <summary>
        /// Gets the parent member of this member.
        /// </summary>
        IMember ParentMember { get; }

        /// <summary>
        /// Gets all members that are children of this member (i.e. this member
        /// is their parent member).
        /// </summary>
        ReadOnlyObservableCollection<IMember> Children { get; }

        /// <summary>
        /// Gets number of children. It allows to check if the member is not leaf without
        /// retreiving all children.
        /// </summary>
        bool HasChildren { get; }
    }
}

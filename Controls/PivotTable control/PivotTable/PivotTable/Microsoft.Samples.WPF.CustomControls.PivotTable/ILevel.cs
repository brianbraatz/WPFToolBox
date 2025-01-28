//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: 
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents a level within a hierarchy.
    /// </summary>
    public interface ILevel
    {
        /// <summary>
        /// Gets caption of this level.
        /// </summary>
        string Caption { get; }
        
        /// <summary>
        /// Gets unique name (key) of this level.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Gets the ordinal position of this level within the parent hierarchy.
        /// </summary>
        int LevelNumber { get; }

        /// <summary>
        /// Gets the hierarchy that contains this level.
        /// </summary>
        IHierarchy Hierarchy { get; }

        /// <summary>
        /// Gets all members within this level.
        /// </summary>
        ReadOnlyObservableCollection<IMember> Members { get; }
    }
}

//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IHierarchy interface.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents dimensions or measures hierarchy.
    /// </summary>
    public interface IHierarchy
    {
        /// <summary>
        /// Gets caption of the hierarchy.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Gets unique name (key) of the hierarchy.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Gets display folder used to group hierarchies.
        /// </summary>
        string DisplayFolder { get; }
        
        /// <summary>
        /// Gets or sets axis that contains the hierarchy.
        /// </summary>
        IFilterAxis Axis { get; set; }

        /// <summary>
        /// Gets all levels within the hierarchy.
        /// </summary>
        ReadOnlyObservableCollection<ILevel> Levels { get; }

        /// <summary>
        /// Gets model that contains this hierarchy.
        /// </summary>
        IDataModel Model { get; }
    }
}

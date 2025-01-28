//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IMeasuresMember interface.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Represents a member within measures hierarchy.
    /// </summary>
    /// <remarks>
    /// This interface hides the IMeasuresLevel, IMeasuresMember and Children properties
    /// from the IMember interface. When you implement this interface, implement the 
    /// IMember explicitly.
    /// </remarks>
    public interface IMeasuresMember : IMember
    {
        /// <summary>
        /// Gets folder that is used to group measures.
        /// </summary>
        string DisplayFolder { get; }

        /// <summary>
        /// Gets a value indicating whether this member supports custom aggregate function.
        /// </summary>
        bool CanSetAggregateFunction { get; }

        /// <summary>
        /// Gets and sets aggregate function for this member.
        /// </summary>
        AggregateFunction AggregateFunction { get; set; }
    }
}

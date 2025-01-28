//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: IDataTableMember interface.
//  Creator: t-tomkm
//  Date Created: 09/05/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Interface for DataTable model dimnesion members.
    /// </summary>
    public interface IDataTableDimensionMember : IMember
    {
        /// <summary>
        /// Removes all rows that don't belong to this members.
        /// </summary>
        /// <param name="rows">Rows set to filter.</param>
        void FilterRows(ISet<DataRow> rows);
    }
}

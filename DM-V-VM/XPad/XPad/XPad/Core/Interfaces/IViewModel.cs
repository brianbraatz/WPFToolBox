using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
namespace XPad
{
    public interface IViewModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the current cursor.
        /// </summary>
        /// <value>The current cursor.</value>
        Cursor CurrentCursor { get;set;}
        #endregion
    }
}

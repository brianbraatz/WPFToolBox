using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
namespace XPad
{
    static class ViewModel
    {
        #region Fields
        private static Cursor mCurrentCursor;
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets the current cursor
        /// </summary>
        public static Cursor CurrentCursor
        {
            get { return mCurrentCursor; }
            set
            {
                mCurrentCursor = value;
                Mouse.OverrideCursor = value;
                //RaiseOnCursorChange(value);
            }
        }
        #endregion

      
    }
}

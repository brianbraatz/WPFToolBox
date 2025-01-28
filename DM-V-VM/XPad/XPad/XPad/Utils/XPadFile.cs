using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    /// <summary>
    /// Just a little struct to pass the data between the model and the file on disk.
    /// In a more elaborate structure the metadata should be separate from the actual text content,
    /// but I kept it simple here.
    /// </summary>
    public struct XPadFile
    {
        #region Fields
        private string mPath;
        private string mTitle;
        private string mContent;
        #endregion

        #region Properties
        /// <summary>
        /// Get the path where the file resides.
        /// </summary>
        public string Path
        {
            get
            {
                return mPath;
            }
            set
            {
                mPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the content of the file
        /// </summary>
        public string Content
        {
            get
            {
                return mContent;
            }
            set
            {
                mContent = value;
            }
        }
        #endregion


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public XPadFile(string title, string path, string content)
        {
            mTitle = title;
            mPath = path;
            mContent = content;
        }
        #endregion
  
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XPad
{
    /// <summary>
    /// Implements the IWorkItem interface and serves as a template for all work items.
    /// </summary>
    public abstract class AbstractWorkItem : IWorkItem
    {
        #region Fields
        /// <summary>
        /// the Enabled field
        /// </summary>
        private bool mEnabled = true;
        /// <summary>
        /// keeps a reference to the name of the item
        /// </summary>
        private string mName;
        /// <summary>
        /// keeps a reference to the previous cursor
        /// </summary>
        protected Cursor prevCursor;
        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the Enabled
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return mEnabled;
            }

            set
            {               
                mEnabled = value;
            }
        }

        /// <summary>
        /// Gets the name of the action/command
        /// </summary>
        /// <value>The name.</value>
        public string Name { 
            get {
                return mName;
            }
        }

        #endregion


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public AbstractWorkItem(string name)
        {
            this.mName = name;
        }
        #endregion
  

        #region Methods
        /// <summary>
        /// Restores the cursor.
        /// </summary>
        protected void RestoreCursor()
        {
            if (prevCursor != null)
            {
                
                ViewModel.CurrentCursor = prevCursor;                
                prevCursor = null;
            }
        }
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            return null;
        }
        #endregion

        

       

    }
}

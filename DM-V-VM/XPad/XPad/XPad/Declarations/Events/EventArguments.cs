using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    #region ToolEventArgs
    /// <summary>
    /// Tool event argument
    /// </summary>
    public sealed class ToolEventArgs : EventArgs
    {

        /// <summary>
        /// the Tool field
        /// </summary>
        private ITool mTool;
        /// <summary>
        /// Gets or sets the Properties
        /// </summary>
        public ITool Properties
        {
            get
            {
                return mTool;
            }
            set
            {
                mTool = value;
            }
        }
        /// <summary>
        /// The empty argument.
        /// </summary>
        public static readonly new ToolEventArgs Empty = new ToolEventArgs();

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ToolEventArgs(ITool tool)
        {
            this.mTool = tool;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ToolEventArgs"/> class.
        /// </summary>
        public ToolEventArgs()
        {
        }
        #endregion

    }
    #endregion

    #region CollectionEventArgs
    /// <summary>
    /// Event argument to pass <see cref="CollectionBase"/> information via events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionEventArgs<T> : EventArgs
    {
        private T item;

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public T Item
        {
            get { return item; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">A parameter of the generics Type T</param>
        public CollectionEventArgs(T item)
        {
            this.item = item;
        }
    }
    #endregion
}

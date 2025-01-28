using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    /// <summary>
    /// Describes an element of work in the diagram engine which can be accessed through user interactions or via inversion of control.
    /// </summary>
    public interface IWorkItem : IServiceProvider
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:ITool"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the tool.
        /// </summary>
        /// <value>The name.</value>
        string Name { get;  }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
/*    
    http://www.theserverside.net/blogs/showblog.tss?id=pluginArchitectures
    http://www.martinfowler.com/articles/injection.html
    http://www.codeproject.com/csharp/components.asp
 */
    /// <summary>
    /// Describes a tool's members. <seealso cref="AbstractTool"/>. A tool, contrary to a <seealso cref="IService"/> does need the user to perform a set of actions in order to be complete.
    /// </summary>
    public interface ITool : IWorkItem
    {

        /// <summary>
        /// Gets or sets a value indicating whether this tool is active. If true the tool is actually performing work via the various mouse or keyboard event handlers.
        /// If <see cref="Enabled"/> is false the tool can never be active. Furthermore, if the tool <see cref="IsSuspended"/> it means another tool has suspended the activity of this tool.
        /// 
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether this tool can activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can activate; otherwise, <c>false</c>.
        /// </value>
        bool CanActivate { get; }

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <returns></returns>
        bool DeactivateTool();

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <returns></returns>
        bool ActivateTool();

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        //IController Controller { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspended. A tool enters in a suspended mode when another tool has been activated and disallows another to continue its normal activity. For example, the <see cref="MoveTool"/> and <see cref="SelectionTool"/> are 
        /// mutually exclusive and similarly for the drawing tools and the selection tool. 
        /// <para>This suspended state is independent of the <see cref="IsActive"/> and the <see cref="Enabled"/> states.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        bool IsSuspended { get; set;}
    }

    
}

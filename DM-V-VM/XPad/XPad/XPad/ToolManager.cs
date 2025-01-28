using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    static class ToolManager 
    {
        #region Events
        /// <summary>
        /// Occurs when a tool is activated
        /// </summary>
        static event EventHandler<ToolEventArgs> OnToolActivate;
        /// <summary>
        /// Occurs when a tool is deactivated
        /// </summary>
        static event EventHandler<ToolEventArgs> OnToolDeactivate;
        #endregion
        #region Fields
        private static CollectionBase<ITool> registeredTools;
        #endregion
        #region Properties
        /// <summary>
        /// Gets the registered tools.
        /// </summary>
        /// <value>The tools.</value>
        public static CollectionBase<ITool> Tools
        {
            get { return registeredTools; }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>An instance of the requested service, if registered.</returns>
        /// <remarks>
        /// Note that contrary to the services we have to keep an instance of a tool alive because
        /// of the connection between the UICommands and the methods of the tool, e.g. the OnQueryEnabled method that tells
        /// the UI element whether the tool can be used or not.
        /// </remarks>
        public static object GetService(Type serviceType)
        {
           return  FindTool(serviceType);
            

            //if (serviceType.Equals(typeof(OpenFileTool)))
            //{
            //    OpenFileTool tool = FindTool(typeof(OpenFileTool)) as OpenFileTool;
            //    if (tool == null)
            //    {
            //        tool = new OpenFileTool();
            //    }
            //    return tool;
            //}
            //return null;
        }

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        static ToolManager()
        {
            registeredTools = new CollectionBase<ITool>();
            

            //the ObjectBuilder somewhere around here...?
            //http://www.sayedhashimi.com/CategoryView,category,ObjectBuilder.aspx

            //add the file tools
            registeredTools.Add(new OpenFileTool());
            registeredTools.Add(new SaveFileTool());
            registeredTools.Add(new ExitTool());
        }
        #endregion

        #region Methods
        private static object FindTool(Type type)
        {
            //using anonymous method here
            Predicate<ITool> predicate = delegate(ITool tool)
            {
                if (tool.GetType().Equals(type))
                    return true;
                else
                    return false;
            };
            return registeredTools.Find(predicate);
        }
        #endregion

    }
}

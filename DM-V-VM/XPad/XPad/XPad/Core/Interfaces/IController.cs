using System;

namespace XPad
{
    /// <summary>
    /// Interface of a controller
    /// </summary>
    public interface IController 
    {
        #region Events
       
      
       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the model
        /// </summary>
        ITextModel Model { get; set;}

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        IViewModel View { get;set;}

        /// <summary>
        /// Gets the tools.
        /// </summary>
        /// <value>The tools.</value>
        CollectionBase<ITool> Tools { get;}

        /// <summary>
        /// Gets the undo manager.
        /// </summary>
        /// <value>The undo manager.</value>
        UndoManager UndoManager { get;}

       

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IController"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled
        {
            get;
            set;
        }
        #endregion

        #region Methods

      

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <param name="toolName">Name of the tool.</param>
        void ActivateTool(string toolName);

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        void AddTool(ITool tool);

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns></returns>
        bool DeactivateTool(ITool tool);

       
        /// <summary>
        /// Suspends all tools
        /// </summary>
        void SuspendAllTools();

        /// <summary>
        /// Unsuspends all tools.
        /// </summary>
        void UnsuspendAllTools();
        

        void RunActivity(string name);
        #endregion

        
    }
}

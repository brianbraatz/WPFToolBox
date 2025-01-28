using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XPad
{
    /// <summary>
    /// Abstract base class for all tools.
    /// </summary>
    public abstract class AbstractTool : AbstractWorkItem, ITool
    {
        #region Fields
        private RoutedCommand mRoutedCommand;
        
        
      
        /// <summary>
        /// the tool's cursor
        /// </summary>
        private Cursor mCursor = Cursors.Arrow;
        /// <summary>
        /// whether the tool is currently active
        /// </summary>
        private bool mIsActive;
        
        /// <summary>
        /// the suspend state of the tool
        /// </summary>
        private bool mIsSuspended;
        #endregion

        #region Properties
        /// <summary>
        /// Routed command associated with the model.
        /// </summary>
        public RoutedCommand Command
        {
            get { return mRoutedCommand; }
            protected set { mRoutedCommand = value; }
        }
       
        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspended. A tool enters in a suspended mode when another tool has been activated and disallows another to continue its normal activity. For example, the <see cref="MoveTool"/> and <see cref="SelectionTool"/> are
        /// mutually exclusive and similarly for the drawing tools and the selection tool.
        /// <para>This suspended state is independent of the <see cref="IsActive"/> and the <see cref="Enabled"/> states.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuspended
        {
            get { return mIsSuspended; }
            set { mIsSuspended = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tool is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return mIsActive; }
            set { mIsActive = value; }
        }
     
       
        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>The cursor.</value>
        public Cursor Cursor
        {
            get { return mCursor; }
            set { mCursor = value; }
        }

        /// <summary>
        /// Gets or sets the Enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }

            set
            {
                //disable the tool first if it is active
                if (!value && IsActive)
                {
                    DeactivateTool();
                }
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tool excludes other tools.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is exclusive; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsExclusive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tool can activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can activate; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanActivate
        {
            get
            {
                if (Enabled)
                {
                    return !IsActive;
                }
                else
                {
                    return false;
                }
            }

        }
        #endregion

          #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public AbstractTool(string name) : base(name)
        {
            mRoutedCommand = new RoutedCommand();
        }
        #endregion
			
        #region Methods
        /// <summary>
        /// Determines if a command is enabled. Override to provide custom behavior. Do not call the
        /// base version when overriding.
        /// </summary>
        public virtual void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Function to execute the command.
        /// </summary>
        public abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            return null;
        }
        #endregion

     

    
     

        #region Methods
        
        #region Activation & deactivation

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <returns></returns>
        public bool DeactivateTool()
        {

            if (IsActive)
            {
                OnDeactivateTool();
                IsActive = false;
                RestoreCursor();
                UnsuspendTools();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <returns></returns>
        public bool ActivateTool()
        {
            //halt other actions
            SuspendOtherTools();

            if (Enabled && !IsActive)
            {
                prevCursor = ViewModel.CurrentCursor;
                IsActive = true;
                OnActivateTool();
            }
            return IsActive;
        }
        /// <summary>
        /// Suspends the other tools.
        /// </summary>
        public void SuspendOtherTools()
        {
            foreach (ITool tool in ToolManager.Tools)
            {
                if (tool != this)
                    tool.IsSuspended = true;
            }
        }
        /// <summary>
        /// Releases the previously suspeneded tools <see cref="SuspendOtherTools"/>
        /// </summary>
        public void UnsuspendTools()
        {
            foreach (ITool tool in ToolManager.Tools)
            {
                tool.IsSuspended = false;
            }
        }
        #endregion
        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected virtual void OnActivateTool() { }

        /// <summary>
        /// Called when the tool is deactivated.
        /// </summary>
        protected virtual void OnDeactivateTool() { }

     
        #endregion

    }
}

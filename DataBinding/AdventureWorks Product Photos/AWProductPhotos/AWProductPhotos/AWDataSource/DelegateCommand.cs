namespace AWDataSource
{
    using System;
    using System.Windows.Input;

    ///
    /// DelegateCommand is a simplified version of WPF's ICommand. You can wrap one of these around any method,
    /// and thus bind any Command on any WPF object to your method.
    ///
    /// DelegateCommand also supports an IsEnabled property you can use to turn the command on and off.
    ///
    public sealed class DelegateCommand : ICommand
    {
        // Type signature of the method that DelegateCommand works with - returns void, no arguments.
        public delegate void SimpleEventHandler();

        // Remember the method so we can call it when it's time.
        private SimpleEventHandler handler;

        // Maintain our enabled state.
        private bool isEnabled = true;

        // Simple constructor: Just pass in the method that needs to be called when the command executes.
        public DelegateCommand(SimpleEventHandler handler)
        {
            this.handler = handler;
        }

        #region ICommand implementation

        // Executing the command is as simple as calling that method we were handed on creation.
        void ICommand.Execute(object arg)
        {
            this.handler();
        }

        // Saying whether we can execute the command.
        bool ICommand.CanExecute(object arg)
        {
            return this.IsEnabled;
        }

        // This is the event that WPF's command architecture listens to so it knows when to update the UI
        // on command enable/disable.
        public event EventHandler CanExecuteChanged;
        #endregion

        // Public visibility of our isEnabled flag - note that when it is set, we need to raise the event
        // so that WPF knows to update any UI that uses this command.
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.OnCanExecuteChanged();
            }
        }

        // Simple event propagation that makes sure someone is listening to the event before raising it.
        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
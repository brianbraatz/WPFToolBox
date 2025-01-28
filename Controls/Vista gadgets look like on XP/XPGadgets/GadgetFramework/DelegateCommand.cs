namespace GadgetFramework
{
    using System;
    using System.Windows.Input;

    public sealed class DelegateCommand : ICommand
    {
        public delegate void SimpleEventHandler(object arg);
        private SimpleEventHandler handler;
        private bool isEnabled = true;

        public DelegateCommand(SimpleEventHandler handler)
        {
            this.handler = handler;
        }

        #region ICommand implementation
        void ICommand.Execute(object arg)
        {
            this.handler(arg);
        }

        bool ICommand.CanExecute(object arg)
        {
            return this.IsEnabled;
        }

        public event EventHandler CanExecuteChanged;
        #endregion

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.OnCanExecuteChanged();
            }
        }

        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
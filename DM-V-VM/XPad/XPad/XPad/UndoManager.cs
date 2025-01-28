using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
namespace XPad
{
    /// <summary>
    /// Manages the undo/redo stack
    /// </summary>
    public static class UndoManager
    {
        #region Fields
        /// <summary>
        /// the undo stack
        /// </summary>
        private static Stack<ICommand> _undoStack = new Stack<ICommand>();

        /// <summary>
        /// the redo stack
        /// </summary>
        private static Stack<ICommand> _redoStack = new Stack<ICommand>();
        /// <summary>
        /// the <c>ObservableCollection</c> of undo's you can bind to some UIElement
        /// </summary>
        private static ObservableCollection<string> _undoStackNames = new ObservableCollection<string>();
        /// <summary>
        /// the <c>ObservableCollection</c> of redo's you can bind to some UIElement
        /// </summary>
        private static ObservableCollection<string> _redoStackNames = new ObservableCollection<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the undo stack.
        /// </summary>
        /// <value>The undo stack.</value>
        internal static Stack<ICommand> UndoStack
        {
            get { return UndoManager._undoStack; }
            //set { UndoManager._undoStack = value; }
        }
        /// <summary>
        /// Gets the redo stack.
        /// </summary>
        /// <value>The redo stack.</value>
        internal static Stack<ICommand> RedoStack
        {
            get { return UndoManager._redoStack; }
            //set { UndoManager._redoStack = value; }
        }
        /// <summary>
        /// Gets a value indicating whether there is anything that can be undone.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        public static bool CanUndo
        {
            get { return _undoStack.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether there is anything that can be rolled forward
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public static bool CanRedo
        {
            get { return _redoStack.Count > 0; }
        }

        /// <summary>
        /// Gets the undo stack info.
        /// </summary>
        /// <value>The undo stack info.</value>
        public static ObservableCollection<string> UndoStackInfo
        {
            get
            {
                return _undoStackNames;
            }
        }

        /// <summary>
        /// Gets the redo stack info.
        /// </summary>
        /// <value>The redo stack info.</value>
        public static ObservableCollection<string> RedoStackInfo
        {
            get
            {
                return _redoStackNames;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the specified command record.
        /// </summary>
        /// <param name="commandRecord">The command record.</param>
        internal static void Execute(ICommand commandRecord)
        {
            commandRecord.Redo();
            if (commandRecord is ICommand)
            {
                _undoStack.Push((ICommand)commandRecord);
                _undoStackNames.Insert(0, commandRecord.Text);
            }
        }

        /// <summary>
        /// If there is anything in the undo stack the top is popped and undone.
        /// </summary>
        internal static void Undo()
        {
            if (_undoStack.Count > 0)
            {
                ICommand commandRecord = _undoStack.Pop();
                commandRecord.Undo();
                _undoStackNames.RemoveAt(0);
                _redoStack.Push(commandRecord);
                _redoStackNames.Insert(0, commandRecord.Text);
            }
        }

        /// <summary>
        /// If there is anything in the redo stack the top is popped and executed
        /// </summary>
        internal static void Redo()
        {
            if (_redoStack.Count > 0)
            {
                ICommand commandrecord = _redoStack.Pop();
                Execute(commandrecord);
                _redoStackNames.RemoveAt(0);
            }
        }
        #endregion
        

      
    }

}

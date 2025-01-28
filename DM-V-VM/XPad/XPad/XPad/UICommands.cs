using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
namespace XPad
{
    /// <summary>
    /// Bundling of UI commands.
    /// </summary>
    public static class UICommands
    {

        #region Fields
        ///// <summary>
        ///// the OpenCommand field
        ///// </summary>
        //private static OpenCommand mOpenCommand;
        ///// <summary>
        ///// the SaveCommand field
        ///// </summary>
        //private static SaveCommand mSaveCommand;
        ///// <summary>
        ///// the NewCommand field
        ///// </summary>
        //private static NewCommand mNewCommand;
        /// <summary>
        /// the TextModel field
        /// </summary>
        private static TextModel mTextModel;
        ///// <summary>
        ///// the ExitCommand field
        ///// </summary>
        //private static ExitCommand mExitCommand;
        #endregion

        #region Properties
        ///// <summary>
        ///// Gets or sets the OpenCommand
        ///// </summary>
        //public static OpenCommand OpenCommand
        //{
        //    get { return mOpenCommand; }
        //    set { mOpenCommand = value; }
        //}
        ///// <summary>
        ///// Gets or sets the SaveCommand
        ///// </summary>
        //public static SaveCommand SaveCommand
        //{
        //    get { return mSaveCommand; }
        //    set { mSaveCommand = value; }
        //}
        ///// <summary>
        ///// Gets or sets the NewCommand
        ///// </summary>
        //public static NewCommand NewCommand
        //{
        //    get { return mNewCommand; }
        //    set { mNewCommand = value; }
        //}

        ///// <summary>
        ///// Gets or sets the exit command.
        ///// </summary>
        ///// <value>The exit command.</value>
        //public static ExitCommand ExitCommand
        //{
        //    get { return mExitCommand; }
        //    set { mExitCommand = value; }
        //}
        #endregion

        #region Constructor

        #endregion

        #region OpenFileCommand

        public static Control GetOpenFileCommand(DependencyObject sender)
        {
            return (Control)sender.GetValue(OpenFileCommandProperty);
        }

        public static void SetOpenFileCommand(DependencyObject sender, Control command)
        {
            sender.SetValue(OpenFileCommandProperty, command);
        }


        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenFileCommandProperty =
            DependencyProperty.RegisterAttached("OpenFileCommand", typeof(Control), typeof(UICommands),
                new PropertyMetadata(new PropertyChangedCallback(OnOpenFileCommandInvalidated)));

        private static void OnOpenFileCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();
            //inversion of control here, we try to bind to whatever implements the interface in the 
            //service manager
            AbstractTool tool = ToolManager.GetService(typeof(OpenFileTool)) as AbstractTool;
            if (tool != null)
            {
                element.CommandBindings.Add(new CommandBinding(tool.Command, tool.OnExecute, tool.OnQueryEnabled));

                if (element is ICommandSource)
                {
                    element.SetValue(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, tool.Command);
                }
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region TextCommand

        /// <summary>
        /// Gets the text command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        public static string GetTextCommand(DependencyObject sender)
        {
            return (string)sender.GetValue(TextCommandProperty);
        }

        public static void SetTextCommand(DependencyObject sender, string command)
        {
            sender.SetValue(TextCommandProperty, command);
        }

        public static readonly DependencyProperty TextCommandProperty =
            DependencyProperty.RegisterAttached("TextCommand", typeof(String), typeof(UICommands),
                new PropertyMetadata(new PropertyChangedCallback(OnAttach)));

        /// <summary>
        /// Callback when the TextCommand property is set or changed.
        /// </summary>
        private static void OnAttach(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TextBoxBase element = (TextBoxBase)dependencyObject;
            Binding binding = new Binding("Text");
            binding.Source = Controller.Model;
            binding.NotifyOnTargetUpdated = true;
            binding.Mode = BindingMode.TwoWay;
            binding.NotifyOnSourceUpdated = true;
            //the following line is crucial to update the source...but one would think that the TwoWay setting is enough, no???
            //see http://blogs.msdn.com/wpfsdk/archive/2006/10/19/wpf-basic-data-binding-faq.aspx
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(element, TextBox.TextProperty, binding);
        }

        #endregion

        #region SaveCommand

        public static Control GetSaveFileCommand(DependencyObject sender)
        {
            return (Control)sender.GetValue(SaveFileCommandProperty);
        }

        public static void SetSaveFileCommand(DependencyObject sender, Control command)
        {
            sender.SetValue(SaveFileCommandProperty, command);
        }


        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveFileCommandProperty =
            DependencyProperty.RegisterAttached("SaveFileCommand", typeof(Control), typeof(UICommands),
                new PropertyMetadata(new PropertyChangedCallback(OnSaveFileCommandInvalidated)));

        private static void OnSaveFileCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            AbstractTool tool = ToolManager.GetService(typeof(SaveFileTool)) as AbstractTool;
            if (tool != null)
            {
                element.CommandBindings.Add(new CommandBinding(tool.Command, tool.OnExecute, tool.OnQueryEnabled));

                if (element is ICommandSource)
                {
                    element.SetValue(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, tool.Command);
                }
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region NewFileCommand

        public static Control GetNewFileCommand(DependencyObject sender)
        {
            return (Control)sender.GetValue(NewFileCommandProperty);
        }

        public static void SetNewFileCommand(DependencyObject sender, Control command)
        {
            sender.SetValue(NewFileCommandProperty, command);
        }


        // Using a DependencyProperty as the backing store for NewCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewFileCommandProperty =
            DependencyProperty.RegisterAttached("NewFileCommand", typeof(Control), typeof(UICommands),
                new PropertyMetadata(new PropertyChangedCallback(OnNewFileCommandInvalidated)));

        private static void OnNewFileCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            AbstractTool tool = ToolManager.GetService(typeof(NewFileTool)) as AbstractTool;
            if (tool != null)
            {
                element.CommandBindings.Add(new CommandBinding(tool.Command, tool.OnExecute, tool.OnQueryEnabled));

                if (element is ICommandSource)
                {
                    element.SetValue(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, tool.Command);
                }
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region ExitCommand

        public static Control GetExitCommand(DependencyObject sender)
        {
            return (Control)sender.GetValue(ExitCommandProperty);
        }

        public static void SetExitCommand(DependencyObject sender, Control command)
        {
            sender.SetValue(ExitCommandProperty, command);
        }


        // Using a DependencyProperty as the backing store for ExitCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExitCommandProperty =
            DependencyProperty.RegisterAttached("ExitCommand", typeof(Control), typeof(UICommands),
                new PropertyMetadata(new PropertyChangedCallback(OnExitCommandInvalidated)));

        private static void OnExitCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            AbstractTool tool = ToolManager.GetService(typeof(ExitTool)) as AbstractTool;
            if (tool != null)
            {
                element.CommandBindings.Add(new CommandBinding(tool.Command, tool.OnExecute, tool.OnQueryEnabled));

                if (element is ICommandSource)
                {
                    element.SetValue(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, tool.Command);
                }
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion


    }
}

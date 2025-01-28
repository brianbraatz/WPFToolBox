using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
namespace XPad
{
    /// <summary>
    /// Embodies the exit action.
    /// </summary>
    public class ExitTool : AbstractTool
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommand"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ExitTool()
            : base("Exit application")
        {
            //note that WPF defines a lot of standard commands in the ApplicationCommands class 
            //You can in the constructor override the base definition and use these if you prefer,
            //something like;
            //this.Command = ApplicationCommands.Close;
            //Note that the Command setter has the 'protected' modified.
        }

        /// <summary>
        /// Determines if a command is enabled. Override to provide custom behavior. Do not call the
        /// base version when overriding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //the following would enable the save action only if the file has been set before
            //e.CanExecute = !string.IsNullOrEmpty(model.CurrentFileName);
            e.CanExecute = true;

        }

        /// <summary>
        /// Function to execute the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            //UICommands.NewCommand.OnExecute(this, null);
            Application.Current.Shutdown(0);
        }



    }
}

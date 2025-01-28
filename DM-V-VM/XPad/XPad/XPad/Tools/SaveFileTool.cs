using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.IO;
using System.Windows;
namespace XPad
{
    /// <summary>
    /// Embodies the save action.
    /// </summary>
    public class SaveFileTool : AbstractTool
    {
       
        #region Properties

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveCommand"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public SaveFileTool()
            : base("Save document")
        {
           
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
            XPadFile xfile = new XPadFile(string.Empty, Controller.Model.CurrentFileName, Controller.Model.Text);
            ITextFileService service = ServiceManager.GetService(typeof(ITextFileService)) as ITextFileService;
            service.SaveFile(xfile);
            
        }

    }
}

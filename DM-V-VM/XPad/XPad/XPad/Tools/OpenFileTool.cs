using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
namespace XPad
{
    /// <summary>
    /// Instrument to open a file from disk.
    /// </summary>
    public class OpenFileTool : AbstractTool
    {
      
        #region Properties

        #endregion
        public OpenFileTool()
            : base("Open document")
        {
           
        }

        public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            //XPadFile xfile = new XPadFile(string.Empty, Controller.Model.CurrentFileName, Controller.Model.Text);
            ITextFileService service = ServiceManager.GetService(typeof(ITextFileService)) as ITextFileService;
            XPadFile file= service.OpenFile(string.Empty);
            Controller.Model.Text = file.Content;
        }

    }
}

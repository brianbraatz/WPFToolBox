using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
namespace XPad
{
    /// <summary>
    /// 
    /// </summary>
    public class NewFileTool : AbstractTool
    {
        TextModel model;
        #region Properties

        #endregion
        public NewFileTool(TextModel model)
            : base("New document")
        {
            this.model = model;
        }

        public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //string symbol = e.Parameter as string;
            //e.CanExecute = (!string.IsNullOrEmpty(symbol));
            e.CanExecute = true;
            //e.Handled = true;
        }

        public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!model.IsSaved)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Unsaved content", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (res)
                {
                    case MessageBoxResult.Yes:

                        XPadFile xfile = new XPadFile(string.Empty, Controller.Model.CurrentFileName, Controller.Model.Text);
                        ITextFileService service = ServiceManager.GetService(typeof(ITextFileService)) as ITextFileService;
                        service.SaveFile(xfile);
                        break;
                    case MessageBoxResult.Cancel:
                        e.Handled = true;
                        return;

                }
            }
            model.CurrentFileName = string.Empty;
            model.Text = string.Empty;
        }


    }

}

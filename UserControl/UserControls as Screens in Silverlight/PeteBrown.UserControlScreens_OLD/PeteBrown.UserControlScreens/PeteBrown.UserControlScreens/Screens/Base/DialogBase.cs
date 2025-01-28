using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PeteBrown.UserControlScreens.Screens.Base
{
    public class DialogBase : ScreenBase
    {
        private DialogResult _dialogResult = DialogResult.None;

        public DialogResult DialogResult
        {
            get { return _dialogResult; }
            protected set { _dialogResult = value; }
        }


        // We override and set the dialog result here to clear any values
        protected override void PrepareForShow()
        {
            _dialogResult = DialogResult.None;

            base.PrepareForShow();
        }
    }
}

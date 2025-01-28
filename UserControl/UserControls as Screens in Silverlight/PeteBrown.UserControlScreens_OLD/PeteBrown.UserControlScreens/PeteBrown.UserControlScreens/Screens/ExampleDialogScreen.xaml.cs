using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using PeteBrown.UserControlScreens.Controls;
using PeteBrown.UserControlScreens.Screens.Base;

namespace PeteBrown.UserControlScreens.Screens
{
    public class ExampleDialogScreen : DialogBase
    {
        private SimpleButton _okButton;
        private SimpleButton _cancelButton;
        private TextBlock _title;
        private TextBlock _text;

        public ExampleDialogScreen()
        {
            LoadXaml("PeteBrown.UserControlScreens.Screens.ExampleDialogScreen.xaml");
            //System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream("PeteBrown.UserControlScreens.ExampleDialogScreen.xaml");
            //this.InitializeFromXaml(new System.IO.StreamReader(s).ReadToEnd());

            _okButton = FindByName<SimpleButton>("OkButton");
            _cancelButton = FindByName<SimpleButton>("CancelButton");
            _title = FindByName<TextBlock>("Title");
            _text = FindByName<TextBlock>("Text");

            SetUpEventHandlers();
        }

        private void SetUpEventHandlers()
        {
            _okButton.Click += new EventHandler(_okButton_Click);
            _cancelButton.Click += new EventHandler(_cancelButton_Click);
        }

        void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            Hide();
        }

        void _okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            Hide();
        }


        public string Title
        {
            get { return _title.Text; }
            set { _title.Text = value; }
        }

        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; }
        }
    
    }
}

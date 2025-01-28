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
    public class MainScreen : ScreenBase
    {
        private ExampleDialogScreen _exampleDialogScreen;
        private WizardControllerScreen _wizardControllerScreen;
        private SimpleButton _showDialog;
        private SimpleButton _showWizard;
        private TextBlock _messages;

        private bool _wizardDisplayed = false;
        

        public MainScreen()
        {
            LoadXaml("PeteBrown.UserControlScreens.Screens.MainScreen.xaml");
            //System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream("PeteBrown.UserControlScreens.MainScreen.xaml");
            //this.InitializeFromXaml(new System.IO.StreamReader(s).ReadToEnd());

            _exampleDialogScreen = FindByName<ExampleDialogScreen>("ExampleDialogScreen");
            _wizardControllerScreen = FindByName<WizardControllerScreen>("WizardControllerScreen");
            _showDialog = FindByName<SimpleButton>("ShowDialog");
            _showWizard = FindByName<SimpleButton>("ShowWizard");
            _messages = FindByName<TextBlock>("Messages");

            SetUpEventHandlers();
        }


        private void SetUpEventHandlers()
        {
            _showDialog.Click += new EventHandler(_showDialog_Click);
            _showWizard.Click += new EventHandler(_showWizard_Click);
            _exampleDialogScreen.Closed += new EventHandler(_exampleDialogScreen_Closed);
        }

        /// <summary>
        /// Toggles display of the wizard screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _showWizard_Click(object sender, EventArgs e)
        {
            if (_wizardDisplayed)
            {
                _showWizard.Text = "Show Wizard";
                _wizardControllerScreen.Hide();
            }
            else
            {
                _showWizard.Text = "Hide Wizard";
                _wizardControllerScreen.Show();
            }

            _wizardDisplayed = !_wizardDisplayed;
        }

        // Raised when the dialog hide animation has completed
        void _exampleDialogScreen_Closed(object sender, EventArgs e)
        {
            // Operate on the selection made in that window
            if (_exampleDialogScreen.DialogResult == DialogResult.OK)
            {
                _messages.Text = "You clicked \"OK\"";
            }
            else
            {
                _messages.Text = "You cancelled the dialog.";
            }
        }

        // Display the dialog
        void _showDialog_Click(object sender, EventArgs e)
        {
            _exampleDialogScreen.Show();
        }

        protected override void OnShowCompleted(object sender, EventArgs e)
        {
            base.OnShowCompleted(sender, e);

            FindByName<Storyboard>("BackgroundAnimationStoryboard").Begin();
        }
    }
}

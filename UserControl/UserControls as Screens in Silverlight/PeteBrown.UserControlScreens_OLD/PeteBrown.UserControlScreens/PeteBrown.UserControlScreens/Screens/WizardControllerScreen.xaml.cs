using System;
using System.Collections.Generic;
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
    public class WizardControllerScreen : ScreenBase
    {

        private List<WizardPageBase> _pages = new List<WizardPageBase>();
        private SimpleButton _previousButton;
        private SimpleButton _nextButton;

        private WizardPageBase _currentPage = null;

        public WizardControllerScreen()
        {
            LoadXaml("PeteBrown.UserControlScreens.Screens.WizardControllerScreen.xaml");

            _pages.Add(FindByName<WizardPageBase>("WizardPage01"));
            _pages.Add(FindByName<WizardPageBase>("WizardPage02"));
            _pages.Add(FindByName<WizardPageBase>("WizardPage03"));

            _previousButton = FindByName<SimpleButton>("Previous");
            _nextButton = FindByName<SimpleButton>("Next");

            SetUpEventHandlers();
        }

        private void SetUpEventHandlers()
        {
            _previousButton.Click += new EventHandler(_previousButton_Click);
            _nextButton.Click +=new EventHandler(_nextButton_Click);
        }

        void _previousButton_Click(object sender, EventArgs e)
        {
            if (_currentPage.CanMoveBackward)
            {
                ShowPage(_pages[_pages.IndexOf(_currentPage)-1]);
            }
        }

        void _nextButton_Click(object sender, EventArgs e)
        {
            if (_currentPage.CanMoveForward)
            {
                ShowPage(_pages[_pages.IndexOf(_currentPage) + 1]);
            }
        }


        public override void Show()
        {
            base.Show();

            ShowPage(_pages[0]);
        }

        private void ShowPage(WizardPageBase page)
        {
            // hide old page

            if (_currentPage != null)
            {
                _currentPage.Hide();
            }

            // show new page

            _currentPage = page;
            _currentPage.NavigationChanged += new EventHandler(_currentPage_NavigationChanged);
            _currentPage.Show();
            

        }

        void _currentPage_NavigationChanged(object sender, EventArgs e)
        {
            _nextButton.Enabled = _currentPage.CanMoveForward;
            _previousButton.Enabled = _currentPage.CanMoveBackward;
        }

    }
}

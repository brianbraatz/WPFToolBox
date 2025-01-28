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
    public class WizardPage03 : WizardPageBase
    {
        public WizardPage03()
        {
            LoadXaml("PeteBrown.UserControlScreens.Screens.WizardPage03.xaml");
        }


        public override bool CanMoveBackward
        {
            get { return true; }
        }

        public override bool CanMoveForward
        {
            get { return false; }
        }

    }
}

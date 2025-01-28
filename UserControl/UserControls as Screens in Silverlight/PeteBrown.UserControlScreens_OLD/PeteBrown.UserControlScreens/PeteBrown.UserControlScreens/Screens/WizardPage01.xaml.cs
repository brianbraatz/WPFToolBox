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
    public class WizardPage01 : WizardPageBase
    {
        public WizardPage01()
        {
            LoadXaml("PeteBrown.UserControlScreens.Screens.WizardPage01.xaml");
        }

        public override bool CanMoveBackward
        {
            get { return false; }
        }

        public override bool CanMoveForward
        {
            get { return true; }
        }

    }
}

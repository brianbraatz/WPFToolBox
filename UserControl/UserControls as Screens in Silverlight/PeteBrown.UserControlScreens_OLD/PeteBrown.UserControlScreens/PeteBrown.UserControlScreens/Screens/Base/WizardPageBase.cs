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
    public abstract class WizardPageBase : ScreenBase
    {
        public event EventHandler NavigationChanged;
        
        public abstract bool CanMoveForward { get; }
        public abstract bool CanMoveBackward { get; }
        //public abstract bool CanFinish { get; }

        /// <summary>
        /// The derived wizard page should call this function when it 
        /// does something that changes the values returned by 
        /// CanMoveForward and CanMoveBackward. For example, if the
        /// user completes a required amount of information on the
        /// page.
        /// </summary>
        protected void RaiseNavigationChanged()
        {
            if (NavigationChanged != null)
                NavigationChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raise the navigation changed here just to notify
        /// the container about our current state.
        /// </summary>
        public override void Show()
        {
            RaiseNavigationChanged();

            base.Show();
        }
    }
}

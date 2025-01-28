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
using PeteBrown.UserControlScreens.Controls.Base;

namespace PeteBrown.UserControlScreens.Screens.Base
{
    public abstract class ScreenBase : ControlBase
    {
        private const string _screenShowStoryboardName = "ShowScreenStoryboard";
        private const string _screenHideStoryboardName = "HideScreenStoryboard";

        public event EventHandler Closed;


        /// <summary>
        /// Since we reuse instances of windows in this pattern, we
        /// need a way to reset values. Override this function to do that
        /// </summary>
        protected virtual void PrepareForShow()
        {
        }

        /// <summary>
        /// The Show() method uses a known animation, if present, to display 
        /// the screen. Typically the animation will animate the opacity or
        /// it will animate position/rotation to bring the screen into view
        /// </summary>
        public virtual void Show()
        {
            PrepareForShow();

            // force visibility since you can't animate the Visible property
            this.Visibility = Visibility.Visible;

            if (FindByName<Storyboard>(_screenShowStoryboardName) == null)
            {
                // I often animate the opacity to show an item. In this case, since 
                // we're already visible, I just ensure Opacity is 1
                this.Opacity = 1;
                OnShowCompleted(this, new EventArgs());
            }
            else
            {
                FindByName<Storyboard>(_screenShowStoryboardName).Begin();
                FindByName<Storyboard>(_screenShowStoryboardName).Completed += new EventHandler(OnShowCompleted);
            }

            LogShowToHitLogger();
        }


        /// <summary>
        /// The Hide() method does roughly the opposite of the Show() method
        /// Once you understand the pattern, feel free to alter either one 
        /// of these to suit your needs.
        /// </summary>
        public virtual void Hide()
        {
            if (FindByName<Storyboard>(_screenHideStoryboardName) == null)
            {
                // hide just by collapsing this event
                this.Visibility = Visibility.Collapsed;

                OnHideCompleted(this, new EventArgs());
            }
            else
            {
                FindByName<Storyboard>(_screenHideStoryboardName).Begin();
                FindByName<Storyboard>(_screenHideStoryboardName).Completed +=new EventHandler(OnHideCompleted);
            }

            LogHideToHitLogger();        
        }

        // Since we're not actual blocking when showing a modal window, we need a
        // notification event. This can also be useful for regular windows, so it
        // is in this class.
        protected void RaiseClosed()
        {
            if (Closed != null)
                Closed(this, new EventArgs());
        }

        protected virtual void OnShowCompleted(object sender, EventArgs e)
        {
            // TODO: Anything special you may want to have happen when the Show storyboard completes
        }

        protected virtual void OnHideCompleted(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

            RaiseClosed();

            // TODO: Anything special you may want to have happen when the Hide storyboard completes
        }



        protected void LogShowToHitLogger()
        {
            // TODO: Add hit logger code (see LogHideToHitLogger() for comments)
        }

        protected void LogHideToHitLogger()
        {
            // TODO: Add hit logger code
            // If you use an analytics package, put the call here. This often
            // requires calling out to JavaScript, but can sometimes be done
            // directly in the code. Either way, you'll want to encapsulate
            // the calls in another class
        }


    }
}

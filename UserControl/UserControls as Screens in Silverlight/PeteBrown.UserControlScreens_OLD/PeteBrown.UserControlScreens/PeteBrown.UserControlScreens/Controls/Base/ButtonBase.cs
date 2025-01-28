using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PeteBrown.UserControlScreens.Controls.Base
{
    /// <summary>
    /// ButtonBase encapsulates the functionality common to most buttons. This
    /// is an example only. Feel free to use it if it works well for you.
    /// </summary>
    public abstract class ButtonBase : ControlBase
    {
        private const string _mouseEnterStoryboardName = "MouseEnterStoryboard";
        private const string _mouseLeaveStoryboardName = "MouseLeaveStoryboard";
        private const string _mouseLeftButtonDownStoryboardName = "MouseLeftButtonDownStoryboard";
        private const string _mouseLeftButtonUpStoryboardName = "MouseLeftButtonUpStoryboard";

        protected bool _mouseOver;
        protected bool _mouseLeftButtonDown;
        protected bool _enabled = true;

        public event EventHandler Click;

        public ButtonBase()
        {
            this.MouseEnter += new MouseEventHandler(OnMouseEnter);
            this.MouseLeave += new EventHandler(OnMouseLeave);
            this.MouseLeftButtonDown += new MouseEventHandler(OnMouseLeftButtonDown);
            this.MouseLeftButtonUp +=new MouseEventHandler(OnMouseLeftButtonUp);
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; UpdateLayout();  }
        }

        protected virtual void UpdateLayout()
        {
            // This is cheesy. it would be better to either animate the
            // state or hard-code some layers in here to show different 
            // states of the button
            if (_enabled)
            {
                this.Opacity = 1;
            }
            else
            {
                this.Opacity = 0.25;
            }
        }


        protected void RaiseClick()
        {
            if (Click != null)
            {
                Click(this, new EventArgs());
            }
        }

        protected virtual void PlayMouseEnterAnimation()
        {
            if (FindByName<Storyboard>(_mouseEnterStoryboardName) != null)
                FindByName<Storyboard>(_mouseEnterStoryboardName).Begin();
        }

        protected virtual void PlayMouseLeaveAnimation()
        {
            if (FindByName<Storyboard>(_mouseLeaveStoryboardName) != null)
                FindByName<Storyboard>(_mouseLeaveStoryboardName).Begin();
        }

        protected virtual void PlayMouseLeftButtonDownAnimation()
        {
            if (FindByName<Storyboard>(_mouseLeftButtonDownStoryboardName) != null)
                FindByName<Storyboard>(_mouseLeftButtonDownStoryboardName).Begin();
        }

        protected virtual void PlayMouseLeftButtonUpAnimation()
        {
            if (FindByName<Storyboard>(_mouseLeftButtonUpStoryboardName) != null)
                FindByName<Storyboard>(_mouseLeftButtonUpStoryboardName).Begin();
        }



        private void OnMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            ReleaseMouseCapture();

            if (_enabled)
            {
                if (_mouseLeftButtonDown && _mouseOver)
                {
                    PlayMouseLeftButtonUpAnimation();
                    RaiseClick();
                }
            }

           _mouseLeftButtonDown = false;
        }

        private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (_enabled)
            {
                CaptureMouse();

                PlayMouseLeftButtonDownAnimation();
                _mouseLeftButtonDown = true;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_enabled)
            {

                if (_mouseLeftButtonDown)
                {
                    PlayMouseLeftButtonUpAnimation();
                }

                _mouseOver = false;
                PlayMouseLeaveAnimation();
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_enabled)
            {
                if (_mouseLeftButtonDown)
                {
                    PlayMouseLeftButtonDownAnimation();
                }

                _mouseOver = true;
                PlayMouseEnterAnimation();
            }
        }

    }
}

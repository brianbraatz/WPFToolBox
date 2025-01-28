using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;

namespace FollowTheKeyboard
{
	public partial class Window1
	{
        // Keep a convenient reference to the movable control's translation.
        // This property gets set when the app starts.
        private TranslateTransform translation = new TranslateTransform();

        public Window1()
        {
            this.InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            // Always call the base method when implementing an override of the OnInitialized method.
            base.OnInitialized(e);

            // When the app starts, set our private property to the current transform.
            this.MovableControl.RenderTransform = this.translation;

            // The movable control has keyboard focus from the moment the app starts.
            MovableControl.Focus();
        }

        private void MovableControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // When the mouse button is clicked over the movable control, give it keyboard focus.
            MovableControl.Focus();
        }

        private void MovableControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Arrow keys will only move the moveable control if it has keyboard focus already.
            // This means that arrow keys will move the slider bars if they have focus.

            // Get the current speed selected by the user.
            double speed = this.SpeedSlider.Value;

            // Tell Windows that you have handled the key down event so that it is removed from the message queue.
            e.Handled = true;

            // Move the movable control based on the arrow keys.
            switch (e.Key)
            {
                // The Key class is defined in System.Windows.Input.
                case Key.Up:
                    // Translate the moveable control a distance based on the speed value.
                    this.translation.Y -= speed;
                    break;
                case Key.Down:
                    this.translation.Y += speed;
                    break;
                case Key.Left:
                    this.translation.X -= speed;
                    break;
                case Key.Right:
                    this.translation.X += speed;
                    break;
                case Key.Tab:
                    // If the tab key is pressed, change focus to the next control.
                    SpeedSlider.Focus();
                    break;
                default:
                    // For all other key presses, let Windows handle this key down event.
                    e.Handled = false;
                    break;
            }

        }

	}
}
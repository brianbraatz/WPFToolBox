using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Input;

namespace FollowThePointer
{
	public partial class Window1
	{
        // Keep a convenient reference to the movable control's translation.
        // This property gets set when the app starts.
        private TranslateTransform translation = new TranslateTransform();

        // A resonable minimum value for velocity for the sake of efficiency.
        private const double epsilon = 1;

        // The Stopwatch class is defined in the System.Diagnostics namespace.
        private Stopwatch stopwatch = new Stopwatch();

        private Vector velocity = new Vector();
        private TimeSpan prevTime = TimeSpan.Zero;

        public Window1()
        {
            this.InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            // Always call the base method when implementing an override of the OnInitialized method.
            base.OnInitialized(e);

            // Hook up a handler to be called each time the scene is rendered.
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;

            // Get the current translation of the movable control.
            this.MovableControl.RenderTransform = this.translation;

            // Start the stopwatch.
            this.stopwatch.Start();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Current position of mouse pointer relative to the movable control.
            // The Mouse class is defined in the System.Windows.Input namespace.
            Point mousePos = Mouse.GetPosition(this.MovableControl); 

            TimeSpan currentTime = this.stopwatch.Elapsed;
            double elapsedTime = (currentTime - this.prevTime).TotalSeconds;
            this.prevTime = currentTime;

            // The vector to the mouse pointer represents the force currently acting on the movable control.
            Vector force = new Vector(mousePos.X, mousePos.Y);

            // The smaller the value of damping, the more iterations it takes to approach the mouse pointer, thus allowing velocity to grow larger.
            force = (force * this.SpringSlider.Value - this.velocity * this.DampingSlider.Value) * elapsedTime;

            // The current force causes an acceleration (a change in velocity).
            this.velocity += force;

            // If the eye won't notice any further motion then don't animate on this iteration.
            if (velocity.Length < epsilon) 
                return;

            // Distance equals speed times time.
            this.translation.X += this.velocity.X * elapsedTime; 
            this.translation.Y += this.velocity.Y * elapsedTime;
        }
    }
}
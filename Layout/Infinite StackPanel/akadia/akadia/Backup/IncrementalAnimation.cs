namespace Akadia {

	using System;
	using System.Windows;
	using System.Windows.Media.Animation;
	

	public class IncrementalAnimation : DoubleAnimationBase {
		public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate", typeof(double), typeof(IncrementalAnimation));
		TimeSpan lastTime = TimeSpan.Zero;
		double lastValue = 0;

		public double Rate {
			get { return (double)this.GetValue(IncrementalAnimation.RateProperty); }
			set { this.SetValue(IncrementalAnimation.RateProperty, value); }
		}

		protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock) {

			if (animationClock.CurrentTime.HasValue) {
				double elapsed = (animationClock.CurrentTime.Value - lastTime).TotalSeconds;
				this.lastTime = animationClock.CurrentTime.Value;

				this.lastValue += elapsed * this.Rate;
			}
			return this.lastValue;
		}

		protected override Freezable CreateInstanceCore() {
			return new IncrementalAnimation();
		}
	}
}

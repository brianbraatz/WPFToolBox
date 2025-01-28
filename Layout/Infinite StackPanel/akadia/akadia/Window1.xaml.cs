namespace Akadia
{
	using System;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Media.Animation;

	public partial class Window1
	{
		IncrementalAnimation incrementor = new IncrementalAnimation();
		public Window1()
		{
			this.InitializeComponent();
		}

		protected  void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			this.incrementor.Duration = Duration.Forever;
			this.InfinitePanel.BeginAnimation(InfinitePanel.OffsetProperty, this.incrementor);
		}

		protected void OnMouseMove(MouseEventArgs e) {
			this.incrementor.Rate = (e.GetPosition(this).X - this.Width / 2) * .5;
		}
	}
}

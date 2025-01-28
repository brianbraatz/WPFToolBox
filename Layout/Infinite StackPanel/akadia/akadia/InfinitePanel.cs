namespace Akadia {

	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System;

	public class InfinitePanel: StackPanel {

		public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(InfinitePanel), new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

		public InfinitePanel() {
		}

		public double Offset {
			get { return (double)this.GetValue(InfinitePanel.OffsetProperty); }
			set { this.SetValue(InfinitePanel.OffsetProperty, value); }
		}

		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}

		protected override Size ArrangeOverride(Size arrangeSize) {
			Rect availableRect = new Rect(arrangeSize);
			bool isHorizontal = this.Orientation == Orientation.Horizontal;

			double offset = this.Offset;
			if (isHorizontal)
				offset = offset % arrangeSize.Width;
			else
				offset = offset % arrangeSize.Height;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this); ++i) {
				FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(this, i);

				if (isHorizontal) {
					if (offset > this.DesiredSize.Width - child.Margin.Left)
						offset -= arrangeSize.Width;
					else if (offset + child.DesiredSize.Width < 0)
						offset += arrangeSize.Width;


					availableRect.X = offset;
					availableRect.Width = child.DesiredSize.Width;
					availableRect.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);

					offset += child.DesiredSize.Width;
				}
				else {
					if (offset > this.DesiredSize.Height - child.Margin.Top)
						offset -= arrangeSize.Height;
					else if (offset + child.DesiredSize.Height < 0)
						offset += arrangeSize.Height;


					availableRect.X = offset;
					availableRect.Width = child.DesiredSize.Height;
					availableRect.Height = Math.Max(arrangeSize.Width, child.DesiredSize.Width);

					offset += child.DesiredSize.Height;
				}
				child.Arrange(availableRect);
			}

			return arrangeSize;
		}
	}
}

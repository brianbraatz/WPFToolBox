namespace Snoop
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows.Documents;
	using System.Windows;
	using System.Windows.Media;
	using System.Windows.Controls;

	/// <summary>
	/// Simple helper class to allow any UIElements to be used as an Adorner.
	/// </summary>
	public class AdornerContainer: Adorner {
		private UIElement child;

		public AdornerContainer(UIElement adornedElement): base(adornedElement) {
		}

		protected override Size ArrangeOverride(Size finalSize) {
			if (this.child != null)
				this.child.Arrange(new Rect(finalSize));
			return finalSize;
		}

		public UIElement Child {
			get { return this.child; }
			set  {
				this.AddVisualChild(value);
				this.child = value; 
			}
		}

		protected override int VisualChildrenCount {
			get { return this.child == null ? 0 : 1; }
		}

		protected override Visual GetVisualChild(int index) {
			if (index == 0 && this.child != null)
				return this.child;
			return base.GetVisualChild(index);
		}
	}
}

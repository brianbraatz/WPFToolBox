namespace Disorder
{
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows;
	using System.Windows.Data;

	public class StackPanelZ: StackPanel
	{
		public static DependencyProperty TopIndexProperty = DependencyProperty.Register("TopIndex", typeof(int), typeof(StackPanelZ), new PropertyMetadata(-1, new PropertyChangedCallback(StackPanelZ.TopIndexChanged)));

		public StackPanelZ() {
		}

		protected override void OnInitialized(System.EventArgs e) {
			base.OnInitialized(e);

			// Default to binding that works for placing this as the ItemPanel in a ListBox, if unset.
			if (this.ReadLocalValue(StackPanelZ.TopIndexProperty) == DependencyProperty.UnsetValue) {
				Binding binding = new Binding("SelectedIndex");
				// binding.RelativeSource = "/TemplatedParent/TemplatedParent";
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                
			// HACK 	this.SetValue(StackPanelZ.TopIndexProperty, binding.ProvideValue(this, StackPanelZ.TopIndexProperty));
			}
		}

		protected override Visual GetVisualChild(int index) {
			int topIndex = this.TopIndex;

			if (index >= topIndex && topIndex != -1) {
				if (index == this.VisualChildrenCount - 1)
					return base.GetVisualChild(topIndex);
				return base.GetVisualChild(index + 1);
			}
			return base.GetVisualChild(index);
		}

		public int TopIndex {
			get { return (int)this.GetValue(StackPanelZ.TopIndexProperty); }
			set {
				this.SetValue(StackPanelZ.TopIndexProperty, value);
			}
		}

		private static void TopIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StackPanelZ panel = (StackPanelZ)d;

			int index = panel.TopIndex;
			if (index != -1) {
				Visual visual = panel.GetVisualChild(panel.VisualChildrenCount - 1);
				panel.RemoveVisualChild(visual);
				panel.AddVisualChild(visual);
			}
		}
	}
}

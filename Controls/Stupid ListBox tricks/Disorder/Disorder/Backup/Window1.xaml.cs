using System.Collections.ObjectModel;
namespace Disorder
{
	public partial class Window1
	{
		private ObservableCollection<int> items = new ObservableCollection<int>();

		public Window1() {
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			this.InitializeComponent();

			for (int i = 0; i < 10; ++i)
				this.items.Add(i);
		}

		public ObservableCollection<int> Items {
			get { return this.items; }
		}
	}
}

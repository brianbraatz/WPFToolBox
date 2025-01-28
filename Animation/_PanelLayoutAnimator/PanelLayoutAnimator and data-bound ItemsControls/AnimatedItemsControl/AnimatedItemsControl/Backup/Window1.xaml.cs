using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WindowsApplication9
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window, INotifyPropertyChanged
	{

		public Window1()
		{
			this.DataCollection.Add(new Data("Item 1"));
			this.DataCollection.Add(new Data("Item 2"));
			this.DataCollection.Add(new Data("Item 3"));
			this.DataCollection.Add(new Data("Item 4"));
			this.DataCollection.Add(new Data("Item 5"));
			this.DataCollection.Add(new Data("Item 6"));
			this.DataCollection.Add(new Data("Item 7"));
			this.DataCollection.Add(new Data("Item 8"));
			this.DataCollection.Add(new Data("Item 9"));

			this.Loaded += new RoutedEventHandler(Window1_Loaded);
			InitializeComponent();
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			CollectionViewSource cvs = FindResource("cvs") as CollectionViewSource;
			cvs.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
		}

		private ObservableCollection<Data> _dataCollection = new ObservableCollection<Data>();

		public ObservableCollection<Data> DataCollection
		{
			get { return _dataCollection; }
		}

		void OnClick(object sender, RoutedEventArgs e)
		{
			CollectionViewSource cvs = FindResource("cvs") as CollectionViewSource;
			using (cvs.DeferRefresh())
			{
				ListSortDirection lsd = cvs.SortDescriptions[0].Direction;
				cvs.SortDescriptions.Clear();
				cvs.SortDescriptions.Add(new SortDescription("Name", (lsd == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}
	}

	public class Data : DependencyObject, INotifyPropertyChanged
	{
		public Data(string name)
		{
			this.Name = name;
		}

		private string _name = "";

		public string Name
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged("Name"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}
	}
}
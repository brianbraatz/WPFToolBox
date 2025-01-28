using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace SimpleDataTemplateSample {
  public partial class MainWindow : Window {

    public MainWindow() {
      InitializeComponent();
    }


    protected override void OnClosing(CancelEventArgs e) {
      base.OnClosing(e);

      // Get the selected named color
      ObjectDataProvider provider = this.mainGrid.FindResource("BooksODP") as ObjectDataProvider;
      ICollectionView view = CollectionViewSource.GetDefaultView(provider.ObjectInstance);
      Book book = (Book)view.CurrentItem;
      MessageBox.Show("Selected book = " + book.ToString());
    }
  }
}
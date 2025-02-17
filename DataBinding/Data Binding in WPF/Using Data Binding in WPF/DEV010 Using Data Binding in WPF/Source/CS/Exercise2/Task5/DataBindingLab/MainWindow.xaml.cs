using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DataBindingLab
{
    public partial class MainWindow : Window
    {
        CollectionViewSource listingDataView;

        public MainWindow()
        {
            InitializeComponent();
            listingDataView = (CollectionViewSource)(this.Resources["listingDataView"]);
        }

        private void ShowOnlyBargainsFilter(object sender, FilterEventArgs e)
        {
            AuctionItem product = e.Item as AuctionItem;
            if (product != null)
            {
                // Filter out products with price 25 or above
                if (product.CurrentPrice < 25)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void AddGrouping(object sender, RoutedEventArgs args)
        {
            // This groups by property "Category"
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = "Category";
            listingDataView.GroupDescriptions.Add(groupDescription);
        }

        private void RemoveGrouping(object sender, RoutedEventArgs args)
        {
            listingDataView.GroupDescriptions.Clear();
        }

        private void AddSorting(object sender, RoutedEventArgs args)
        {
            // This sorts the items first by Category and within each Category, by StartDate
            // Notice that because Category is an enumeration, the order of the items is the same as in the 
            // enumeration declaration
            listingDataView.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
            listingDataView.SortDescriptions.Add(new SortDescription("StartDate", ListSortDirection.Ascending));
        }

        private void RemoveSorting(object sender, RoutedEventArgs args)
        {
            listingDataView.SortDescriptions.Clear();
        }

    }
}
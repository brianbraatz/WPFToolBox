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
    }
}
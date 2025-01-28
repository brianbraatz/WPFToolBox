using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DemoDev;

namespace StockGraph3D
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            _myApp = (Application)System.Windows.Application.Current;

            // Get the product Data
            _StockData = (StocksData)_myApp.FindResource("Stocks");
            
            // add MSFT as a starter stock
            StockItem si = new StockItem();
            si.ID = "msft1";
            si.Title = "msft";
            si.Description = "Microsft";
            GraphComboBox.SelectedIndex = 3;
            ComponentResourceKey crk = new ComponentResourceKey(typeof(ControlResource), "BlueGraphBrush");
            si.GraphBrush = Application.Current.FindResource(crk) as Brush;
            _StockData.StocksItems.Add(si);

            // add label brushes
            VisualBrush vb = new VisualBrush(StocksListBoxLabelX);
            GraphList3D.LabelXBrush = vb;
            vb = new VisualBrush(StocksListBoxLabelY);
            GraphList3D.LabelYBrush = vb;
            vb = new VisualBrush(StocksListBoxLabelZ);
            GraphList3D.LabelZBrush = vb;

        }

        #region Events

        private void OnList3DItemSelected(object o, EventArgs e)
        {
            List3DItem li = o as List3DItem;

            if (li == null)
                return;

            string s = li.ID + " " + li.StockDate + " " + li.StockPrice;
            StockItemHitTestText.Text = s;

        }

        private void OnAddStock(object sender, RoutedEventArgs e)
        {
            string stock = StockTextBox.Text;

            if ((stock == null) || (stock.Length == 0))
                return;
           
            StockItem si = new StockItem();
            si.ID = stock;
            si.Title = stock;
            si.Description = stock;
            ComponentResourceKey crk = new ComponentResourceKey(typeof(ControlResource), (GraphComboBox.SelectedValue as ComboBoxItem).Content as string);
            si.GraphBrush = Application.Current.FindResource(crk) as Brush;
            _StockData.StocksItems.Add(si);
         

        }

        private void OnGraphStocks(object sender, RoutedEventArgs e)
        {
            StockTime st1 = new StockTime(StockFromDateYear.Text, StockFromDateMonth.Text, StockFromDateDay.Text, StockFromInterval.Text);
            StockTime st2 = new StockTime(StockToDateYear.Text, StockToDateMonth.Text, StockToDateDay.Text, StockFromInterval.Text);

            _StockData.Build3DGraph(st1, st2);
        }

        private void OnAnimateGraphStocks(object sender, RoutedEventArgs e)
        {
            StockTime st1 = new StockTime(StockFromDateYear.Text, StockFromDateMonth.Text, StockFromDateDay.Text, StockFromInterval.Text);
            StockTime st2 = new StockTime(StockToDateYear.Text, StockToDateMonth.Text, StockToDateDay.Text, StockFromInterval.Text);

            _StockData.BuildAnimate3DGraph(st1, st2);
        }

        private void OnAnimateTo(object sender, RoutedEventArgs e)
        {
            _StockData.ReplayToAnimate3DGraph();
        }

        private void OnAnimateFrom(object sender, RoutedEventArgs e)
        {
            _StockData.ReplayFromAnimate3DGraph();
        }

        private void OnDeleteStock(object sender, RoutedEventArgs e)
        {
            _StockData.StocksItems.RemoveAt(StocksListBox.SelectedIndex);
        }

        private void OnClearStocks(object sender, RoutedEventArgs e)
        {
            _StockData.StocksItems.Clear();
            _StockData.Graph3DItems.Clear();
        }
       
        #endregion

        #region Private Methods

        #endregion

        #region Globals

        StocksData _StockData;
        Application _myApp;

        #endregion
    }

   
}




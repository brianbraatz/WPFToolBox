using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace StockWatch
{
	public partial class Scene1
	{
        private const string unknownSymbol = "";
        private const string enterSymbol = "Enter symbol...";
        private StockWatchModel stockWatchModel;

		public Scene1()
		{
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			// this.InitializeComponent();

			// Insert code required on object creation below this point.
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

            ObjectDataProvider odp = this.FindResource("StockWatchModelDS") as ObjectDataProvider;
            this.stockWatchModel = odp.Data as StockWatchModel;
		}

		private void CloseButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			// TODO: Add event handler implementation here.
            if (this.SearchTextBox.Text != enterSymbol && this.SearchTextBox.Text.Trim() != "")
            {
                Thread thread = new Thread(new ParameterizedThreadStart(this.fetchQuote)); 
                thread.Start(this.SearchTextBox.Text.Trim());
            }
		}
		private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			this.SearchTextBox.SelectAll();
		}
		private void OnGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			// TODO: Add event handler implementation here.
			this.SearchTextBox.SelectAll();
		}
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (this.SearchTextBox.Text != enterSymbol && this.SearchTextBox.Text.Trim() != "")
                {
                    Quote quote = StockWatchModel.GetQuote(this.SearchTextBox.Text);
                    if (quote != null)
                    {
                        stockWatchModel.QuoteList.Add(quote);
                    }
                }
            }
        }

        private void fetchQuote(object data)
        {
            Quote quote = StockWatchModel.GetQuote(data as String);
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new DispatcherOperationCallback(UpdateAfterFetchQuote), quote);
        }

        private object UpdateAfterFetchQuote(object data)
        {
            Quote quote = data as Quote;
            if (quote == null)
            {
                this.SearchTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                this.CompanyHintTextBlock.Text = "";
            }
            else
            {
                this.SearchTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                this.CompanyHintTextBlock.Text = quote.Name;
            }

            return null;
        }

        private void OnClickOnStockQuote(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        	Grid senderGrid = sender as Grid;
        	Quote quote = senderGrid.DataContext as Quote;
        	Process.Start(quote.Link);
        }

	}
}

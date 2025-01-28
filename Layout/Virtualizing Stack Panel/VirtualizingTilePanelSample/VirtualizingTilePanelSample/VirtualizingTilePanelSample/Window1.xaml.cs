using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace VirtualizingTilePanelSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();

            for (int i = 0; i < 1000; i++)
            {
                _data.Add(GetNextValue());
            }
        }

        protected override void  OnInitialized(EventArgs e)
        {
 	        base.OnInitialized(e);
            _itemsControl.ItemsSource = _data;

            _insertButton.Click += new RoutedEventHandler(InsertButton_Click);
            _removeButton.Click += new RoutedEventHandler(RemoveButton_Click);
            _clearButton.Click += new RoutedEventHandler(ClearButton_Click);
        }

        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _data.Clear();
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int removePos = int.Parse(_removeIndex.Text);
            int removeCount = int.Parse(_removeCount.Text);

            for (int i = 0; i < removeCount; i++)
            {
                _data.RemoveAt(removePos);
            }
        }

        void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            int insertPos = int.Parse(_insertIndex.Text);
            int insertCount = int.Parse(_insertCount.Text);

            for (int i = 0; i < insertCount; i++)
            {
                _data.Insert(insertPos++, GetNextValue());
            }
        }

        private string GetNextValue()
        {
            _nextValue++;
            return _nextValue.ToString();
        }

        ObservableCollection<string> _data = new ObservableCollection<string>();
        int _nextValue = 0;
    }
}
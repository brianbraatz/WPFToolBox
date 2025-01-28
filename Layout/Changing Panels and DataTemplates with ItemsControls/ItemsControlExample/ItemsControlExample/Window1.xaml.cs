using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace ItemsControlExample
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // wire up handlers for the buttons
            addButton.Click += new RoutedEventHandler(OnClickAdd);
            radialButton.Click += new RoutedEventHandler(OnClickRadial);
            scrollButton.Click += new RoutedEventHandler(OnClickScroll);
            textButton.Click += new RoutedEventHandler(OnClickText);
            hashButton.Click += new RoutedEventHandler(OnClickHash);

            // add some strings to start with
            _strings.Add("string 1");
            _strings.Add("string 2");
            
            // wire up the string collection to the ListBox
            listBox.ItemsSource = _strings;
        }

        /// <summary>
        /// When we click on the hash button, we show the hash data template
        /// </summary>
        private void OnClickHash(object sender, RoutedEventArgs e)
        {
            listBox.ItemTemplate = (DataTemplate)FindResource("hashTemplate");
        }

        /// <summary>
        /// When we click on the text button, we show the text data template
        /// </summary>
        private void OnClickText(object sender, RoutedEventArgs e)
        {
            listBox.ItemTemplate = (DataTemplate)FindResource("textTemplate");
        }

        /// <summary>
        /// When we click on the scroll button, we want to see a scrolling ListBox.
        /// </summary>
        private void OnClickScroll(object sender, RoutedEventArgs e)
        {
            listBox.Template = (ControlTemplate)FindResource("scrollTemplate");
        }

        /// <summary>
        /// When we click on the radial button, we want to see a radial ListBox.
        /// </summary>
        private void OnClickRadial(object sender, RoutedEventArgs e)
        {
            listBox.Template = (ControlTemplate)FindResource("radialTemplate");
        }

        /// <summary>
        /// When we click add, we want to add an item to the end of the list.
        /// </summary>
        private void OnClickAdd(object sender, RoutedEventArgs e)
        {
            int stringNumber = _strings.Count + 1;

            _strings.Add("string " + stringNumber.ToString());
        }

        /// <summary>
        /// We use ObservableCollection because it will fire events to let the ListBox keep things in sync.
        /// </summary>
        ObservableCollection<string> _strings = new ObservableCollection<string>();
    }
}
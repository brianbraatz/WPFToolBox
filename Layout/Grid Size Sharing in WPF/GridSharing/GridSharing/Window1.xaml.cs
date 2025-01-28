using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace GridSharing
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            DataContext = this;

            List<DataObject> list = new List<DataObject>();
            list.Add(new DataObject("John Smith", "NY", "Accounting"));
            list.Add(new DataObject("Guy Smiley", "NY", "News", DateTime.Today.AddDays(5), DateTime.Today.AddDays(30)));
            list.Add(new DataObject("Jane Doe", "LA", "Accounting", DateTime.Today.AddDays(-100), DateTime.Today.AddDays(-50)));
            list.Add(new DataObject("Nobody", "Chicago", "Sales", DateTime.Today.AddDays(100), DateTime.Today.AddDays(125)));

            DataList = new ObservableCollection<DataObject>(list);
        }

        public ObservableCollection<DataObject> DataList
        {
            get
            {
                return (ObservableCollection<DataObject>)GetValue(DataListProperty);
            }
            set
            {
                SetValue(DataListProperty, value);
            }
        }

        public static DependencyProperty DataListProperty = DependencyProperty.Register(
            "DataList",
            typeof(ObservableCollection<DataObject>),
            typeof(Window1),
            new PropertyMetadata(null, new PropertyChangedCallback(OnDataListChanged)));

        private static void OnDataListChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            
        }
        
    }
}
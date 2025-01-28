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
using Microsoft.Win32;
using System.Xml;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;


namespace WPFLogger
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        

        internal void do1(object sender, RoutedEventArgs e)
        {
            WriteLog(sender, "Something done!");
        }

        internal void do2(object sender, RoutedEventArgs e)
        {
            new RoutedEventHandler(do2Handler).Invoke(sender, e);
        }

        internal void do2Handler(object sender, RoutedEventArgs e)
        {
            WriteLog(sender, "Something else done!");
        }

        internal void do3(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler h = new RoutedEventHandler(do3Handler);
            h.BeginInvoke(sender, e, callBack, debug);
        }

        AsyncCallback callBack = new AsyncCallback(do3HandlerDone);

        internal void do3Handler(object sender, RoutedEventArgs e)
        {
            WriteLog(null, "Beginning to do something long");
            System.Threading.Thread.Sleep(10000);
        }

        internal static void do3HandlerDone(IAsyncResult result)
        {
            WriteLog(result, "Something long done");
        }
        protected void OnLoad(object sender, RoutedEventArgs e)
        {
            debug = this.Resources["myDebugData"] as XmlDataProvider;
            debug.IsAsynchronous = true;
            WriteLog(this, "Initialized...");            
        }

        static XmlDataProvider debug;
        public static bool m_isDebug = true;
        GridViewColumnHeader m_lastHeaderClicked = null;
        ListSortDirection m_lastDirection = ListSortDirection.Ascending;
       

        void onSort(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != m_lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (m_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    m_lastHeaderClicked = headerClicked;
                    m_lastDirection = direction;
                }
            }
        }



        void ClearAll(object sender, RoutedEventArgs e)
        {
            if (debug != null)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save trace values?", "WPF Logger", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        SaveFileDialog std = new SaveFileDialog();
                        std.DefaultExt = ".vlog";
                        std.RestoreDirectory = true;
                        std.Filter = "VC Log files|*.vlog|All Files|*.*";
                        if (std.ShowDialog() == true)
                        {
                            debug.Document.Save(std.FileName);
                        }
                        break;

                    case MessageBoxResult.Cancel:
                        e.Handled = true;
                        return;
                        break;
                }
                debug.Document.SelectSingleNode(debug.XPath).RemoveAll();

            }
        }
        void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(debugLV.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        internal delegate void WriteLogDelegate(object sender, string data);

        internal static void WriteLog(object sender, string data)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new WriteLogDelegate(WriteLog), sender, data);
            }
            else
            {
                if (debug != null && m_isDebug)
                {
                    System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(false);


                    XmlElement item = debug.Document.CreateElement("Item");

                    item.SetAttribute("Date", DateTime.Now.ToString("dd-MMM-yy HH:mm:ss.ffff"));
                    item.SetAttribute("Source", sender!=null?sender.ToString() + ".":Thread.CurrentThread.Name + stack.GetFrame(1).GetMethod().Name);
                    item.SetAttribute("Message", data);

                    debug.Document.SelectSingleNode(debug.XPath).AppendChild(item);
                }
            }
        }
    }
}
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


namespace Ricciolo.Controls.Test
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            MonthItem[] list = new MonthItem[12];
            for (int x = 1; x <= 12; x++)
                list[x - 1] = new MonthItem(x);

            this.DataContext = list;

            //tab.SelectionChanged += new SelectionChangedEventHandler(tab_SelectionChanged);
        }

        void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }

    public class MonthItem
    {
        private int _month;
        private string _name;

        public MonthItem(int month)
        {
            this._month = month;
            this._name = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        public int Month
        {
            get { return this._month; }
        }

        public string Name
        {
            get { return this._name; }
        }
    }
}
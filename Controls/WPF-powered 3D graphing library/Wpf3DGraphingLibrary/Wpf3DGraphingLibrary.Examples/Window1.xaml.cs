using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Wpf3DGraphingLibrary;

namespace Wpf3DGraphingLibrary.Examples
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            TryABarGraph();
            TryAPieGraph();
            Grid.SetColumn(label1, 1);
            Grid.SetRow(label1, 0);
            label1.Content = "Mouse over a bar to read its information.";
            Grid.SetColumn(label2, 0);
            Grid.SetRow(label2, 1);
            label2.Content = "\n\n\nClick on a piece of the pie to read its information.";
        }

        private void TryABarGraph()
        {
        	//List<double> lst = new List<double>();
        	/*Random r = new Random();
        	int c = r.Next(4, 30);
        	for (int i = 0; i < c; i++)
        	{
        		double d = r.NextDouble();
        		d = (d - 0.5d) * 93d;
        		lst.Add(d);
        	}*/
        	//lst.AddRange(new double[] { 8, 7, 8, 7, 5, 4, 4, 5, 9, 8, 10, 11 });

            Dictionary<string, double> dict = new Dictionary<string, double>();
            dict["January"] = 8;
            dict["February"] = 7;
            dict["March"] = 8;
            dict["April"] = 7;
            dict["May"] = 5;
            dict["June"] = 4;
            dict["July"] = 4;
            dict["August"] = 5;
            dict["September"] = 9;
            dict["October"] = 8;
            dict["November"] = 10;
            dict["December"] = 11;
        	
        	BarGraph bg = new BarGraph(dict, Colors.CadetBlue, Colors.BurlyWood, new TimeSpan(0, 0, 0, 0, 500));
            bg.MouseOver += new GraphActionDelegate(BarGraphMouseOver);
            bg.MouseOverAction = BarGraphUserActions.Tooltip;
            Viewport3D v = bg.Initialize();
        	v.Name = "BarGraph1";
            Grid.SetColumn(v, 0);
            Grid.SetRow(v, 0);
            myGrid.Children.Add(v);
        }

        void BarGraphMouseOver(BaseGraph sender, object key, object value)
        {
            label1.Content = "Mouse over a bar to read its information.\n\n\nKey: " + key.ToString() + "\nValue: " + value.ToString();
        }
        
        private void TryAPieGraph()
        {
        	/*List<double> lst = new List<double>();
        	//lst.AddRange(new double[] { 50, 3, 2, 10, 20, 15 });
            for (int i = 0; i < 5; i++)
            {
                lst.Add(5);
                lst.Add(2);
                lst.Add(10);
                lst.Add(8);
            }*/
        	
        	DataTable dt = new DataTable();
        	dt.Columns.Add("James", typeof(double));
        	dt.Columns.Add("Jennifer", typeof(double));
        	dt.Columns.Add("David", typeof(double));
        	dt.Columns.Add("Brian", typeof(double));
        	dt.Columns.Add("Paul", typeof(double));
        	dt.Columns.Add("Neil", typeof(double));
        	dt.Columns.Add("Doug", typeof(double));
        	DataRow dr = dt.NewRow();
        	dr.ItemArray = new object[] { 5, 2, 10, 8, 17, 4, 12 };
        	
        	PieGraph pg = new PieGraph(dr, Colors.BurlyWood, new TimeSpan(0, 0, 0, 0, 500));
            pg.LeftClicked += new GraphActionDelegate(PieGraphLeftClick);
            pg.LeftClickAction = PieGraphUserActions.Focus;
            pg.MouseOverAction = PieGraphUserActions.Highlight;
            //pg.Outlining = false;
            Viewport3D v = pg.Initialize();
        	v.Name = "PieGraph1";
        	Grid.SetColumn(v, 1);
        	Grid.SetRow(v, 1);
        	myGrid.Children.Add(v);
        }

        void PieGraphLeftClick(BaseGraph sender, object key, object value)
        {
            label2.Content = "\n\n\nClick on a piece of the pie to read its information.\n\n\nKey: " + key.ToString() + "\nValue: " + value.ToString();
        }
    }
}

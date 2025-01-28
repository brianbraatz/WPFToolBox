// **************************************************
//  WPF 2-D Chart class (demo window)
//  Created by Li Gao  - July, 2007
//  http://ligao101.wordpress.com/                                              
//                           
// This chart supports panning/zomming and track-ball feature similar
//  to what you see on Google Finance.
//  
//  Note: 
//    1. This code is NOT optimized for performance. 
//    2. This chart is created for demo purposes only.
//    3. You can double-click on the chart to revert the zoom to its original state 
//        the same as the "Home" button
//                              
// This code is created AS-IS. No warranty or support what so ever.
//***************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WPF2DChartCode.WpfControls;
using WPF2DChartCode.WPFControls.TimeSeriesDataLib;

namespace WPF2DChartCode
{
	public partial class Window1
	{
		public Window1()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void DemoButton_Click(object sender, RoutedEventArgs e)
        {

            RandomDataSeries ranData = new RandomDataSeries();

            FillSampleData(MyDemoChart, ranData);

            // set static labels
            MyDemoChart.SetStaticYLabel("My Values");
            MyDemoChart.SetTitle("Li Gao's WPF Chart");
        }

        private void FillSampleData(WpfChart chart, RandomDataSeries RandomData)
        {
            if (chart == null)
                return;

            if (chart.DataSeries == null)
                return;

            TimeSeriesData data = chart.DataSeries;
            data.Clear();

            RandomData.ValueScale = 400;
            RandomData.ReSeed();

            List<TimeSeriesDataPoint> points = new List<TimeSeriesDataPoint>();

            for (int i = 0; i <= 40; i++)
            {
                points.Add(RandomData.NextDataPoint);
            }

            data.AddPointsRange(points.ToArray());         

        }
	}
}
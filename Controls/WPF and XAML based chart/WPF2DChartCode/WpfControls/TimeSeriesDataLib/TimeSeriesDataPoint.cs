// **************************************************
//  Time Series data point
//  Created by Li Gao  - July, 2007
// http://ligao101.wordpress.com/                                              
//                           
//                                                
// This code is created AS-IS. No warranty or support what so ever.
//***************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace WPF2DChartCode.WPFControls.TimeSeriesDataLib
{
    public class TimeSeriesDataPoint
    {
        private double pointValue;
        private DateTime timeStamp;

        public bool hasStepFromValue;
        public double stepFromValue;        
   
        public TimeSeriesDataPoint()
        {
            pointValue = 0.0;
            timeStamp = DateTime.Now;
            hasStepFromValue = false;
            stepFromValue = pointValue;
        }
       
        public TimeSeriesDataPoint(DateTime time,  double value)
        {
            pointValue = value;
            timeStamp = time;
        }

        public double Value
        {
            get { return pointValue; }
            set { pointValue = value; }
        }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        public TimeSeriesDataPoint Clone()
        {
            TimeSeriesDataPoint newPoint = new TimeSeriesDataPoint();
            newPoint.Value = this.Value;
            newPoint.TimeStamp = this.TimeStamp;
            newPoint.hasStepFromValue = this.hasStepFromValue;
            newPoint.stepFromValue = this.stepFromValue;

            return newPoint;
        }

    }
}

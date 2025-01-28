// **************************************************
//  Time Series data collection
//  Created by Li Gao  - July, 2007
// http://ligao101.wordpress.com/                                              
//                           
//                                                
// This code is created AS-IS. No warranty or support what so ever.
//***************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WPF2DChartCode.WPFControls.TimeSeriesDataLib
{  
    public delegate void PlotSizeChangedDelegate();

    public class TimeSeriesData
    {
        private SortedList<DateTime, TimeSeriesDataPoint> dataPoints;
        private object syncLock = new object();
        private PathFigure paths;

        private double minX=0;
        private double minY=0;
        private double maxX=0;
        private double maxY=0;

        private bool alwaysRemoveOldPointWhenAdd = true;

        private bool isCustomTimeRange ;
        private DateTime customTimeOrigin = DateTime.Now;
        private double customMinX=0;
        private double customMaxX=0;

        public PlotSizeChangedDelegate PlotSizeChanged;
        
        /// <summary>
        ///  Generate the plot region that defines the chart rectangle region size and region zero point
        /// </summary>
        public Rect GetPlotRectangle()
        {
            double tempMinX, tempMaxX, tempMinY, tempMaxY;

            if (isCustomTimeRange)
            {
                tempMinX = customMinX ;
                tempMaxX = customMaxX;    
            }
            else
            {
                tempMinX = minX;
                tempMaxX = maxX;               
            }

            double oversize = 0.05f;

            if (dataPoints.Count == 0)
            {
                tempMinY = 0.0;
                tempMaxY = 1.0;
            }
            else
            {
                tempMinY = minY;
                tempMaxY = maxY;
            }

            Vector minPoint = new Vector(tempMinX, tempMinY);
            Vector maxPoint = new Vector(tempMaxX, tempMaxY);
          
            // Make sure that the plot size is greater than zero
            if (maxPoint.Y == minPoint.Y)
            {
                if (maxPoint.Y != 0)
                {
                    maxPoint.Y *= 1.05;
                    minPoint.Y *= 0.95;
                }
                else
                {
                    maxPoint.Y = 1;
                    minPoint.Y = 0;
                }
            }

            if (maxPoint.X == minPoint.X)
            {
                if (maxPoint.X != 0.0)
                {
                    // default to 15 minutes span
                    double xVal = maxPoint.X  ;
                    maxPoint.X = xVal + 450;
                    minPoint.X = xVal - 450;
                }
                else
                {
                    // default to 15 minutes span
                    maxPoint.X =900;
                    minPoint.X =0;
                }
            }

            // Add a bit of a border around the plot
            maxPoint.X = maxPoint.X + (maxPoint.X - minPoint.X) * oversize * .5;
            minPoint.X = minPoint.X - (maxPoint.X - minPoint.X) * oversize * .5;
           
            maxPoint.Y = maxPoint.Y + (maxPoint.Y - minPoint.Y) * oversize * .5;
            //minPoint.Y = minPoint.Y - (maxPoint.Y - minPoint.Y) * oversize * .5;
            minPoint.Y = minPoint.Y - 2;

            // no negative plot region
            if (minPoint.Y < 0)
                minPoint.Y = 0;

            return new Rect(minPoint.X, minPoint.Y, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);
        }//GetPlotRectangle

        /// <summary>
        ///  Gets whether the chart is using custom chart region time range
        /// </summary>
        public bool IsCustomTimeRange
        {
            get { return isCustomTimeRange; }
        }      

        /// <summary>
        ///  Gets or sets the custom chart region start time. Set the value will cause the chart to redraw itself
        /// </summary>
        public DateTime CustomStartTime
        {
            get
            {
                DateTime startTime = customTimeOrigin;
               startTime= startTime.AddSeconds(customMinX);

                return startTime;
            }

            set
            {
                isCustomTimeRange = true;
                customTimeOrigin = value;
                customMinX = 0;

                // regenerate the path
                GeneratePaths();
            }
        }

        /// <summary>
        ///  Gets or sets the custom chart region end time. Set the value will cause the chart to redraw itself
        /// </summary>
        public DateTime CustomEndTime
        {
            get
            {
                DateTime endTime = customTimeOrigin;
                endTime = endTime.AddSeconds(customMaxX);

                return endTime;
            }

            set
            {
                isCustomTimeRange = true;
                customMaxX = GetPointSeconds(value);

                // regenerate the path
                GeneratePaths();
            }
        }

        public TimeSeriesData()
        {
            dataPoints = new SortedList<DateTime, TimeSeriesDataPoint>();
            isCustomTimeRange = false;
            paths = new PathFigure();        
        }

        public void AddPoint(string time, double value)
        {
            AddPoint(DateTime.Parse(time), value);
        }

        public void AddPoint(DateTime time, double value)
        {
            AddPoint(new TimeSeriesDataPoint(time, value));
        }

        public void AddPoint(TimeSeriesDataPoint point)
        {          
            // assuming the new point is always newer data point
            if (dataPoints.Count > 0)
            {
                if (point.TimeStamp <= dataPoints.Keys[dataPoints.Count - 1])
                {
                    //TODO: log some message here
                    return;
                }
            }
                        
            lock (syncLock)
            {              
                //assuming point is newer data
                if (isCustomTimeRange)
                {
                    DateTime maxTime = this.CustomEndTime ;
                    if (point.TimeStamp > maxTime)
                    {
                        TimeSpan span = point.TimeStamp - maxTime;
                        
                        customTimeOrigin = customTimeOrigin.AddSeconds(span.TotalSeconds);
                        customMinX = 0;
                        customMaxX = GetPointSeconds(point.TimeStamp);

                        List<DateTime> obsoleteDates = new List<DateTime>();

                        foreach (TimeSeriesDataPoint tempPoint in dataPoints.Values)
                        {
                            if (tempPoint.TimeStamp < customTimeOrigin)
                            {
                                obsoleteDates.Add(point.TimeStamp);
                            }
                            else
                                break;
                        }

                        foreach (DateTime temptime in obsoleteDates)
                        {
                            dataPoints.Remove(temptime);
                        }
                
                    }
                }
                else
                {
                    // remove the oldest data point
                    if (alwaysRemoveOldPointWhenAdd)
                    {
                        if (dataPoints.Count > 0)                        
                            dataPoints.RemoveAt(0);                        
                    }                   
    
                }
                 
                dataPoints.Add(point.TimeStamp, point);
            }

            // Raise the plotsize changed event                  
            if (PlotSizeChanged != null)
                    PlotSizeChanged();            

        }

        public void AddPointsRange(TimeSeriesDataPoint[] points)
        {
            foreach (TimeSeriesDataPoint dp in points)
            {
                dataPoints.Add(dp.TimeStamp, dp);
            }

            //TODO: calculate custom range change?

            if (PlotSizeChanged != null)
                PlotSizeChanged();            

        }

        public double GetClosedInterpolatedValue(DateTime TimeStamp)
        {
            DateTime prevTime = DateTime.MaxValue;
            DateTime nextTime = DateTime.MaxValue ;
            bool hasFoundValue = false;
            double returnValue = double.NaN;

            foreach (DateTime tempTime in dataPoints.Keys)
            {
                if (TimeStamp >= prevTime && TimeStamp < tempTime)
                {
                    nextTime = tempTime ;
                    hasFoundValue = true;
                    break;
                }
                else
                {
                    prevTime = tempTime;
                }
            }

            if (TimeStamp == prevTime)
                hasFoundValue = true;

            if (hasFoundValue)
            {
                if (TimeStamp == prevTime)
                {
                    returnValue = dataPoints[prevTime].Value;
                }
                else
                {
                    if (nextTime <  DateTime.MaxValue )
                    {
                        double val1 = dataPoints[prevTime].Value;
                        double val2 = dataPoints[nextTime].Value;

                        double time1 = this.GetPointSeconds(prevTime);
                        double time2 = this.GetPointSeconds(nextTime);
                        double x = this.GetPointSeconds(TimeStamp);
                        double y = val1 + (val2 - val1) * (x - time1) / (time2 - time1);
                        returnValue = y;                       
                    }                  
                }               
            }

            return returnValue;
        }

        public double GetClosedPrevValue(DateTime TimeStamp)
        {
            DateTime prevTime = DateTime.MaxValue ;
            bool hasFoundValue = false;
            double returnValue = double.NaN;
            
            foreach (DateTime tempTime in dataPoints.Keys)
            {
                if (TimeStamp >= prevTime && TimeStamp < tempTime)
                {
                    hasFoundValue = true;
                    break;
                }
                else
                {
                    prevTime = tempTime;
                }
            }

            if (hasFoundValue)
            {
                returnValue = dataPoints[prevTime].Value;
            }

            return returnValue;
        }
               
        public PathGeometry GetPathGeometry ()
        {           
            PathGeometry geometry = new PathGeometry();

            PathFigure fig = paths.Clone();       
            
            geometry.Figures.Add(paths);
            return geometry;            
        }

        public SortedList<DateTime, TimeSeriesDataPoint> GetData()
        {
            return dataPoints;
        }

        public void Clear()
        {
            lock (syncLock)
            {
                dataPoints.Clear();

                if (paths.Segments != null)
                    paths.Segments.Clear();
            }            
        }

        public void SetTimeRange(DateTime StartTime, DateTime EndTime)
        {
            List<DateTime> obsoleteDates = new List<DateTime>();

            lock (syncLock)
            {
                foreach (TimeSeriesDataPoint point in dataPoints.Values)
                {
                    if (point.TimeStamp < StartTime || point.TimeStamp > EndTime)
                    {
                        obsoleteDates.Add(point.TimeStamp);
                    }                    
                }

                foreach (DateTime temptime in obsoleteDates)
                {
                    dataPoints.Remove(temptime);
                }
            }

            isCustomTimeRange = true;
            customTimeOrigin = StartTime;
            customMinX = GetPointSeconds(StartTime);
            customMaxX = GetPointSeconds(EndTime);

            // raise the plot size changed event
            if (PlotSizeChanged != null)
                PlotSizeChanged();
            
        }

        public void GeneratePaths()
        {
            bool isFirstPoint = true;

            minX = double.MaxValue;
            minY = double.MaxValue;
            maxX = double.MinValue;
            maxY = double.MinValue;

            foreach (TimeSeriesDataPoint pt in dataPoints.Values)
            {
                double x =GetPointSeconds(pt.TimeStamp );
                                
                if (isFirstPoint)
                {
                    paths.StartPoint = new Point(x, pt.Value);                    
                    isFirstPoint = false;
                }
                else
                {
                    // create step line if avail.
                    if (pt.hasStepFromValue)
                        paths.Segments.Add(new LineSegment(new Point(x, pt.stepFromValue), true));

                    paths.Segments.Add(new LineSegment(new Point(x, pt.Value), true));
                }

                if (minX > x)
                    minX = x;
                if (maxX < x)
                    maxX = x;

                if (minY > pt.Value)
                    minY = pt.Value;

                if (maxY < pt.Value)
                    maxY = pt.Value;
            }

            if (! isFirstPoint )
            {
                paths.Segments.Add(new LineSegment(new Point(maxX, 0), true));
                paths.Segments.Add(new LineSegment(new Point(minX, 0), true));
                paths.Segments.Add(new LineSegment(paths.StartPoint, true));

            }
            
        }

        public DateTime FromPointSeconds(double secs)
        {
            DateTime origin = DateTime.Now;

            if (isCustomTimeRange)
            {
                origin = customTimeOrigin;
            }
            else
            {
                foreach (DateTime tempTime in dataPoints.Keys)
                {
                    origin = tempTime;
                    break;
                }
            }         

            //origin = new DateTime(origin.Year, origin.Month, origin.Day, 0, 0, 0);
            return origin.AddSeconds(secs);
        }

        public double GetPointSeconds(DateTime value)
        {
            DateTime origin =DateTime.Now;

            if (isCustomTimeRange)
            {
                origin = customTimeOrigin;
            }
            else
            {
                foreach (DateTime tempTime in dataPoints.Keys)
                {
                    origin = tempTime;
                    break;
                }
            }          

            //origin = new DateTime(origin.Year, origin.Month, origin.Day, 0, 0, 0);

            TimeSpan span = value - origin;
            return span.TotalSeconds ;
        }


    }
}

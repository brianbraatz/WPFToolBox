// **************************************************
// Random Time Series data generator 
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
    public class RandomDataSeries
    {
        private int seed;
        private DateTime dateOrigin;
        private int valScale;
        private int samplePerMinute;

        private int counter =0;
        private Random random;

        public RandomDataSeries()
        {
            seed = DateTime.Now.Millisecond;
            dateOrigin = DateTime.Now;
            valScale = 100;
            samplePerMinute = 4;
            random = new Random(seed);
        }

        public RandomDataSeries(int SeedValue, DateTime Origin, int ValueScale, int SampleFrequencyPerMin)
        {
            seed =SeedValue;
            dateOrigin = Origin;
            valScale = ValueScale;
            samplePerMinute = SampleFrequencyPerMin;
            random = new Random(seed);
        }

        /// <summary>
        ///  Generating and returning the next available data point as a time series data point
        /// </summary>
        public TimeSeriesDataPoint NextDataPoint
        {
            get
            {
                counter++;
                if (counter <= 0)
                    counter = 1;

                DateTime tempDate = dateOrigin.AddSeconds(counter * 60.0 / samplePerMinute);
                double val = Math.Abs(Math.Sin(counter * 2.001 * 
                    Math.PI/ (20*samplePerMinute))) * random.Next(valScale);

                return new TimeSeriesDataPoint(tempDate, val);

            }
        }

        /// <summary>
        ///  Generating and returning the next available data point (only the y value returned)
        /// </summary>
        public double NextDataValue
        {
            get
            {
                TimeSeriesDataPoint point = this.NextDataPoint;
                return point.Value;
            }
        }

        /// <summary>
        ///  Re-generate a new random seed
        /// </summary>
        public void ReSeed()
        {
            int limit = random.Next(400);
            int newSeed =0;

            for (int i = 0; i < limit; i++)
                newSeed = random.Next(979);

            if (newSeed <= 0)            
            {
                newSeed = DateTime.Now.Millisecond +11;
            }

            Seed = newSeed;            
        }

        #region Public Properties

        public int Seed
        {
            get { return seed; }
            set 
            { 
                seed = value;
                random = new Random(seed);
                // reset the random counter
                counter = 0;
            }
        }

        public DateTime DateOrigin
        {
            get { return dateOrigin; }
            set { dateOrigin = value; }
        }

        public int ValueScale
        {
            get { return valScale; }
            set { valScale = value; }
        }

        public int SampleFrequencyPerMin
        {
            get { return samplePerMinute; }
            set { samplePerMinute = value; }
        }

        #endregion
    }
}

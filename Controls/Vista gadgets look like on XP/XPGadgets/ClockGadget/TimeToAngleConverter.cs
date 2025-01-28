using System.ComponentModel;
using System;
using System.Windows.Data;

namespace ClockGadget
{
    public class TimeToAngleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime time = (DateTime)value;
            if ((string)parameter == "Hour")
            {
                return (((time.Hour % 12) * 30) + ((time.Minute % 60) * 0.5));
            }
            else if ((string)parameter == "Minute")
            {
                return (time.Minute * 6);
            }
            else
            {
                return (time.Second * 6);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
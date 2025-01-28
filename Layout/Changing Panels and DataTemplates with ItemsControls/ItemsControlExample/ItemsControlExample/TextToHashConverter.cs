using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ItemsControlExample
{
    public class TextToHashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetHashCode();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

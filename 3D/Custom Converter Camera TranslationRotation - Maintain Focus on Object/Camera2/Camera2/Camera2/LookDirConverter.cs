using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Web;
using System.ComponentModel;

using System.Windows.Media.Media3D;

namespace ConvertXaml
{
    public class LookDirConverter : IValueConverter
    {
        public LookDirConverter()
        {
            // look at the origin by default
            lookAtPoint = new Point3D(0,0,0);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // value should be a Vector3D
            Point3D origValue = (Point3D)value;

            // subtracting two points gives us a vector
            return (lookAtPoint - origValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			throw new NotImplementedException("The ConvertBack method is not implemented because this Converter should only be used in a one-way Binding.");
		}

        private Point3D lookAtPoint;

        /// <summary>
        /// Property to set/get the point that we want the camera to always
        /// be looking at.
        /// </summary>
        public Point3D LookAtPoint
        {
            get
            {
                return lookAtPoint;
            }
            set
            {
                lookAtPoint = value;
            }
        }
    }
}

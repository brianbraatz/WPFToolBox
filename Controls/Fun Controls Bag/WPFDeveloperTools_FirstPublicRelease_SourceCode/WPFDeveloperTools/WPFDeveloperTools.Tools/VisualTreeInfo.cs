using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace WPFDeveloperTools.Tools
{
    /// <summary>
    /// Class use to browse and return the Visual Tree of a Visual element.
    /// </summary>
    public class VisualTreeInfo
    {
        private static StringBuilder sbListControls;

        /// <summary>
        /// Method to call to get the Visual Tree of a Visual element.
        /// </summary>
        /// <param name="element">The Visual element that yo want to browse to have its Visual Tree.</param>
        /// <returns>A StringBuilder object that contains the Visual Tree of the specified Visual element.</returns>
        public static StringBuilder GetVisualTreeInfo(Visual element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(String.Format("Element {0} is null !", element.ToString()));
            }

            sbListControls = new StringBuilder();

            GetControlsList(element, 0);

            return sbListControls;
        }

        private static void GetControlsList(Visual control, int level)
        {
            const int indent = 4;
            int ChildNumber = VisualTreeHelper.GetChildrenCount(control);

            for (int i = 0; i <= ChildNumber - 1; i++)
            {
                Visual v = VisualTreeHelper.GetChild(control, i) as Visual;

                if (v != null)
                {
                    sbListControls.Append(new string(' ', level * indent));
                    sbListControls.Append(v.GetType());
                    sbListControls.Append(Environment.NewLine);

                    if (VisualTreeHelper.GetChildrenCount(v) > 0)
                    {
                        GetControlsList(v, level + 1);
                    }
                }
                else
                {
                    throw new ArgumentNullException(String.Format("Element {0} is null !", v.ToString()));
                }
            }
        }
    }
}

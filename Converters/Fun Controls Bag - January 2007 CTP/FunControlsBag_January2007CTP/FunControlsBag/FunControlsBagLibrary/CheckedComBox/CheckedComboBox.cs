using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FunControlsBagLibrary.CheckedComboBox
{
    /// <summary>
    /// CheckedComboBox is a simple ComboBox where Items are represented by CheckBox
    /// </summary>
    public class CheckedComboBox : ComboBox
    {
        #region Constructors

        static CheckedComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckedComboBox), new FrameworkPropertyMetadata((typeof(CheckedComboBox))));
        }

        public CheckedComboBox()
            : base()
        {
            this.Loaded += new RoutedEventHandler(CheckedComboBox_Loaded);
        }

        #endregion

        private void CheckedComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

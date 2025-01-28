using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace FunControlsBagLibrary.CheckedListBox
{
    /// <summary>
    /// The CheckedListBox control provides the same functionality that the CheckedListBox on WindowsForms.
    /// In other words, you have a ListBox where Items are, in fact, composed of Checkbox.
    /// </summary>
    public class CheckedListBox : ListBox
    {
        static CheckedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckedListBox), new FrameworkPropertyMetadata((typeof(CheckedListBox))));
        }

        public CheckedListBox()
            : base()
        {
        }
    }
}

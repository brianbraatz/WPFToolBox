using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace FunControlsBagLibrary.ComboBoxFontSelection
{
    /// <summary>
    /// ComboBoxFontSelection is a mimic of the ComboBox used in Office 2007 to select a Font.
    /// It will display a list of all the posible fonts to use.
    /// </summary>
    public class ComboBoxFontSelection : ComboBox
    {
        #region Constructors

        static ComboBoxFontSelection()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxFontSelection), new FrameworkPropertyMetadata((typeof(ComboBoxFontSelection))));
        }

        public ComboBoxFontSelection()
            : base()
        {
            ICollection<FontFamily> FontList = Fonts.SystemFontFamilies;

            foreach (FontFamily fontFamily in FontList)
            {
                ComboBoxItem font = new ComboBoxItem();
                font.FontFamily = fontFamily;
                font.Content = fontFamily.Source;

                this.Items.Add(font);
            }

            this.SelectedIndex = 0;
        }

        #endregion
    }
}

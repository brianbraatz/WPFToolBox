using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace WPFDeveloperTools.Controls.ComboBoxFontSelection
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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxFontSelection), new FrameworkPropertyMetadata((typeof(ComboBoxFontSelection))));
        }

        public ComboBoxFontSelection()
            : base()
        {
            // TODO: This part takes too long to load. Need to optimize it !
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

        #region Events

        public delegate void HandleEscapeKeyPressHandler(ComboBoxFontSelection sender);

        /// <summary>
        /// Event used to handle the press on the Escape Key
        /// </summary>
        public event HandleEscapeKeyPressHandler HandleEscapeKeyPress;

        #endregion

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                if (HandleEscapeKeyPress != null)
                {
                    HandleEscapeKeyPress(this);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFDeveloperTools.Controls.CheckedComboBox
{
    /// <summary>
    /// CheckedComboBox is a simple ComboBox where Items are represented by CheckBox.
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
            this.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(chkSelect_Click));
        }

        #endregion

        #region Properties

        private ComboBoxItem ItemToDisplay;

        #endregion

        private void chkSelect_Click(object sender, RoutedEventArgs e)
        {
            string TextToDisplay = string.Empty;

            foreach (ComboBoxItem item in this.Items)
            {
                Border borderSelect = item.Template.FindName("borderSelect", item) as Border;

                if (borderSelect != null)
                {
                    CheckBox chkSelect = borderSelect.FindName("chkSelect") as CheckBox;

                    if (chkSelect != null)
                    {
                        if (chkSelect.IsChecked.GetValueOrDefault())
                        {
                            if (!string.IsNullOrEmpty(TextToDisplay))
                            {
                                TextToDisplay += ", ";
                            }

                            TextToDisplay += chkSelect.Content;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(TextToDisplay))
            {
                // Remove item from the list
                if (ItemToDisplay != null)
                {
                    if (this.Items.Contains(ItemToDisplay))
                    {
                        this.Items.Remove(ItemToDisplay);
                    }
                }

                // Add item to the list for displaying the text
                ItemToDisplay = new ComboBoxItem();
                ItemToDisplay.Content = TextToDisplay;
                ItemToDisplay.Visibility = Visibility.Collapsed;

                this.Items.Add(ItemToDisplay);

                this.SelectedItem = ItemToDisplay;
            }
        }
    }
}

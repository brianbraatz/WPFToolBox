using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace FunControlsBagSampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        public void ClickOnConfirmButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"Drive C:\ is formated.");
        }

        public void OnFontSelectionMouseMove(object sender, MouseEventArgs e)
        {
            if (this.richTb.Selection != null && !this.richTb.Selection.IsEmpty)
            {
                TextSelection SelectedText = this.richTb.Selection;

                ComboBoxItem ComboItemFont = e.Source as ComboBoxItem;

                if (ComboItemFont != null)
                {
                    string SelectedFont = ComboItemFont.Content.ToString();

                    SelectedText.ApplyPropertyValue(RichTextBox.FontFamilyProperty, SelectedFont);
                }
            }
        }
    }
}
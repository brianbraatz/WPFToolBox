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



namespace WPFDeveloperTools.Controls.SampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();

            MyPresenceControl.UserStatusChanged += new RoutedPropertyChangedEventHandler<string>(MyPresenceControl_UserStatusChanged);

            this.cbSelectFont.HandleEscapeKeyPress += new WPFDeveloperTools.Controls.ComboBoxFontSelection.ComboBoxFontSelection.HandleEscapeKeyPressHandler(cbSelectFont_HandleEscapeKeyPress);
        }

        public void ClickOnConfirmButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"Drive C:\ is formated.");
        }

        public void OnFontSelectionMouseMove(object sender, MouseEventArgs e)
        {
            ApplySelectedFontFromComboBoxItem(e.Source as ComboBoxItem);
        }

        private void cbSelectFont_HandleEscapeKeyPress(WPFDeveloperTools.Controls.ComboBoxFontSelection.ComboBoxFontSelection sender)
        {
            ApplySelectedFontFromComboBoxItem(sender.SelectedItem as ComboBoxItem);
        }

        private void ApplySelectedFontFromComboBoxItem(ComboBoxItem ComboItemFont)
        {
            if (ComboItemFont != null)
            {
                if (this.richTb.Selection != null && !this.richTb.Selection.IsEmpty)
                {
                    TextSelection SelectedText = this.richTb.Selection;

                    string SelectedFont = ComboItemFont.Content.ToString();

                    SelectedText.ApplyPropertyValue(RichTextBox.FontFamilyProperty, SelectedFont);
                }
            }
        }

        public void OnBindButtonClick(object sender, RoutedEventArgs e)
        {
            string s = "Here is some text which is binded :)";

            Binding b = new Binding();
            b.Source = s;

            this.m_RunBindable.SetBinding(WPFDeveloperTools.Controls.BindableRunControl.BindableRunControl.BindableTextProperty, b);
        }

        public void ChangeStatusClick(object sender, RoutedEventArgs e)
        {
            this.MyPresenceControl.UserStatus = WPFDeveloperTools.Controls.PresenceControl.PresenceControl.Status.Away;
        }

        void MyPresenceControl_UserStatusChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            this.m_tbPreviousStatus.Text = e.OldValue;

            this.m_tbNewStatus.Text = e.NewValue;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace WPFDeveloperTools.Controls.BindableRunControl
{
    /// <summary>
    /// BindableRunControl is a control that inherit from the Run control but which have a bindable Text Property.
    /// </summary>
    public class BindableRunControl : Run
    {
        #region Constructors

        public BindableRunControl()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to use to specify the text that need to be binded.
        /// </summary>
        public string BindableText
        {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }

        public static readonly DependencyProperty BindableTextProperty =
            DependencyProperty.Register("BindableText", typeof(string), typeof(BindableRunControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindableTextChanged)));

        #endregion

        private static void OnBindableTextChanged(DependencyObject element, DependencyPropertyChangedEventArgs arg)
        {
            string TextToBind = arg.NewValue as string;

            if (!string.IsNullOrEmpty(TextToBind))
            {
                BindableRunControl control = element as BindableRunControl;

                if (control != null)
                {
                    control.Text = TextToBind;
                }
            }
        }
    }
}

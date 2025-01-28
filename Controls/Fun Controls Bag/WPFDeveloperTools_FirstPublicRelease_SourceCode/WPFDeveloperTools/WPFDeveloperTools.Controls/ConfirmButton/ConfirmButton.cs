using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFDeveloperTools.Controls.ConfirmButton
{
    /// <summary>
    /// This class provides a Confirm Button. In other words, this means that you will be able to give your users
    /// the choice to confirm the click on a button before doing any actions.
    /// </summary>
    public class ConfirmButton : Button
    {
        #region Constructors

        static ConfirmButton()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmButton), new FrameworkPropertyMetadata((typeof(ConfirmButton))));
        }

        public ConfirmButton()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set the question to confirm or not.
        /// </summary>
        public string ConfirmSentence
        {
            get { return (string)GetValue(ConfirmSentenceProperty); }
            set { SetValue(ConfirmSentenceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfirmSentence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfirmSentenceProperty =
            DependencyProperty.Register("ConfirmSentence", typeof(string), typeof(ConfirmButton), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Set the title of the MessageBox that appears to confirm (or not) the question.
        /// </summary>
        public string ConfirmButtonTitle
        {
            get { return (string)GetValue(ConfirmButtonTitleProperty); }
            set { SetValue(ConfirmButtonTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfirmButtonTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfirmButtonTitleProperty =
            DependencyProperty.Register("ConfirmButtonTitle", typeof(string), typeof(ConfirmButton), new FrameworkPropertyMetadata(string.Empty));

        #endregion

        protected override void OnClick()
        {
            MessageBoxResult result = MessageBox.Show(ConfirmSentence, ConfirmButtonTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.None);

            switch (result)
            {
                case MessageBoxResult.Cancel:
                case MessageBoxResult.No:
                case MessageBoxResult.None:
                    break;

                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:
                    base.OnClick();
                    break;

                default:
                    break;
            }
        }
    }
}

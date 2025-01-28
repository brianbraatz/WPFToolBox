using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFDeveloperTools.Controls.CollapsiblePanel
{
    /// <summary>
    /// CollapsiblePanel is a control that represent a Panel that can be collapsed.
    /// It's composed of an Extander and a Panel.
    /// </summary>
    public class CollapsiblePanel : Expander
    {
        #region Constructors

        static CollapsiblePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsiblePanel), new FrameworkPropertyMetadata((typeof(CollapsiblePanel))));
        }

        public CollapsiblePanel()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Use to indicate the text to show in the Panel.
        /// </summary>
        public string InfoToShow
        {
            get { return (string)GetValue(InfoToShowProperty); }
            set { SetValue(InfoToShowProperty, value); }
        }

        public static readonly DependencyProperty InfoToShowProperty =
            DependencyProperty.Register("InfoToShow", typeof(string), typeof(CollapsiblePanel), new FrameworkPropertyMetadata(string.Empty));

        #endregion
    }
}

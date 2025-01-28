using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WPFDeveloperTools.Controls.RichTextBoxWithToolBar
{
    /// <summary>
    /// RichTextBoxWithToolBar is a simple RichTextBox which provides a toolbar for the most wanted options (Bold, Italic, Underline, etc...).
    /// </summary>
    public class RichTextBoxWithToolBar : RichTextBox
    {
        #region Constructors

        static RichTextBoxWithToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextBoxWithToolBar), new FrameworkPropertyMetadata((typeof(RichTextBoxWithToolBar))));
        }

        public RichTextBoxWithToolBar()
            : base()
        {
        }

        #endregion
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using PeteBrown.UserControlScreens.Controls.Base;

namespace PeteBrown.UserControlScreens.Controls
{
    /// <summary>
    /// A simple text button with some basic animations
    /// </summary>
    public class SimpleButton : ButtonBase
    {
        private TextBlock _text;

        public SimpleButton()
        {
            LoadXaml("PeteBrown.UserControlScreens.Controls.SimpleButton.xaml");

            // the above line replaces this code, which comes automatically with the template
            //System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream("PeteBrown.UserControlScreens.SimpleButton.xaml");
            //this.InitializeFromXaml(new System.IO.StreamReader(s).ReadToEnd());

            _text = FindByName<TextBlock>("Text");
        }


        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; UpdateLayout(); }
        }


        protected override void UpdateLayout()
        {
            base.UpdateLayout();

            // Center the text in the canvas. This does not handle overflow or clipping.
            // Note that Width and Height will only be valid if they are set in the xaml
            // that instantiates (not defines) this control.
            _text.SetValue(Canvas.LeftProperty, (Width - _text.ActualWidth) / 2);
            _text.SetValue(Canvas.TopProperty, (Height - _text.ActualHeight) / 2);
        }
    }
}

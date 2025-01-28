//---------------------------------------------------
// DumpButtonTemplate.cs (c) 2006 by Charles Petzold
//---------------------------------------------------
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Serialization;
using System.Xml;

namespace Petzold.DumpButtonTemplate
{
    public class DumpButtonTemplate : Window
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new DumpButtonTemplate());
        }
        public DumpButtonTemplate()
        {
            Title = "Dump Button Template";

            // Create Button
            Button btn = new Button();
            btn.Content = "Button";

            // Make content of window so Template property is non-null
            Content = btn;

            // Create TextBox to display XAML
            TextBox txtbox = new TextBox();
            txtbox.FontFamily = new FontFamily("Lucida Console");
            txtbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            txtbox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = txtbox;

            if (btn.Template != null)
            {
                // Dump the XAML into the TextBox
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = new string(' ', 4);
                settings.NewLineOnAttributes = true;

                StringBuilder strbuild = new StringBuilder();
                XmlWriter xmlwrite = XmlWriter.Create(strbuild, settings);
                XamlWriter.Save(btn.Template, xmlwrite);

                txtbox.Text = strbuild.ToString();
            }
        }
    }
}

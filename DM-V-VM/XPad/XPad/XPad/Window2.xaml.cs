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
using System.Diagnostics;
namespace XPad
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>

    public partial class Window2 : System.Windows.Window
    {
     

        public Window2()
        {
            //this initializes the model and wil cascade other static stuff as well
            Controller.Model = new TextModel();            
            InitializeComponent();
            //Assign the well-know notepad icon to the window, 
            //watch out for the fancy resources notation in WPF
            //and the the difference between 'resource' and 'Embedded resource'!
            Uri iconUri = new Uri("pack://application:,,,/Resources/notepad.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            UndoManager.UndoStackInfo.Add("One");
            UndoManager.UndoStackInfo.Add("Two");
            
        }
        private void TestIt(object sender, RoutedEventArgs e)
        {
            UndoManager.UndoStackInfo.Add("Undo" + new Random().Next(0, 1000).ToString());
        }
        private void OnDown(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("Down at " + e.GetPosition(testCanvas));
        
        }
    }
}
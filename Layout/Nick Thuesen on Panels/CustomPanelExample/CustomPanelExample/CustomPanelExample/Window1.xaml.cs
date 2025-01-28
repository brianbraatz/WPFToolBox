using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;


namespace CustomPanelExample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        private int NumButtonCounter = 1;
        public Window1()
        {
            InitializeComponent();
        }

        // To use Loaded event put Loaded="WindowLoaded" attribute in root element of .xaml file.
        // private void WindowLoaded(object sender, RoutedEventArgs e) {}


        private void AddButton_clicked(object sender, RoutedEventArgs e) 
        {
            //Increment Counter
            NumButtonCounter++;
            //Create new Button
            Button newButton = new Button();
            newButton.Content = NumButtonCounter;

            //Add Button To Panel
            MyCS.Children.Add(newButton);
        }

        private void RemoveButton_clicked(object sender, RoutedEventArgs e)
        {
            if (NumButtonCounter > 0)
            {
                //RemoveButton
                MyCS.Children.RemoveAt(NumButtonCounter-1);

                //Update counter
                NumButtonCounter--;
            }
        }

        private void IncreaseDim_clicked(object sender, RoutedEventArgs e)
        {
            MyCS.DesiredElementDimension = MyCS.DesiredElementDimension + 10;
        }

        private void DecreaseDim_clicked(object sender, RoutedEventArgs e)
        {
            if (MyCS.DesiredElementDimension > 10)
            {
                MyCS.DesiredElementDimension = MyCS.DesiredElementDimension - 10;
            }
        }


    }
}
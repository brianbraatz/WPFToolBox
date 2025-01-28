using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace RabidCalc
{
    public partial class DisplayCalcScene : Window
    {

        CalcClass MyCalcObject = new CalcClass();

        public DisplayCalcScene()
        {
            InitializeComponent();
            MyCalcObject.OnDisplayChange += new DisplayChangeHandler(UpdateDisplay);
            DisplayTextBox.Text = MyCalcObject.Display;
        }

        private void DragAttempt(object sender, RoutedEventArgs e)
        {
            RootWindow.DragMove();
            e.Handled = true;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            // MyApplication.Shutdown();
            Application.Current.Shutdown();
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            // MyApplication.Shutdown();
            //DisplayCalcScene.
        }

        private void ClearEntryClick(object sender, RoutedEventArgs e)
        {
            //Display = String.Empty;
            //UpdateDisplay();
            MessageBox.Show("You clicked CE.");
        }

        private void DigitBtnClick(object sender, RoutedEventArgs e)
        {
            //Numbers and decimal btns can use their content
            //as the relevant info. 
            string strDigit = ((Button)sender).Content.ToString();
            MyCalcObject.CalcInput(strDigit);
        }

        private void FunctionBtnClick(object sender, RoutedEventArgs e)
        {
            string strBtnName = ((Button)sender).Name.ToString();
            switch (strBtnName)
            {
                case "EqualsBtn":
                    MyCalcObject.CalcInput("Equals");
                    break;
                case "AddBtn":
                    MyCalcObject.CalcInput("Add");
                    break;
                case "SubtractBtn":
                    MyCalcObject.CalcInput("Subtract");
                    break;
                case "MultiplyBtn":
                    MyCalcObject.CalcInput("Multiply");
                    break;
                case "DivideBtn":
                    MyCalcObject.CalcInput("Divide");
                    break;
                case "ClearBtn":
                    MyCalcObject.CalcInput("Clear");
                    break;
                case "ClearEntryBtn":
                    MyCalcObject.CalcInput("ClearEntry");
                    break;
                case "MemoryRecallBtn":
                    MyCalcObject.CalcInput("MemoryRecall");
                    break;
                case "MemoryStoreBtn":
                    MyCalcObject.CalcInput("MemoryStore");
                    break;
                case "MemoryPlusBtn":
                    MyCalcObject.CalcInput("MemoryPlus");
                    break;
                case "MemoryClearBtn":
                    MyCalcObject.CalcInput("MemoryClear");
                    break;
            }
        }


        private void UpdateDisplay(DisplayChangeEventArgs e)
        {
            //string tempDisplay = e.UpdatedDisplay;
            //if (tempDisplay.Length < 16)
            DisplayTextBox.Text = e.UpdatedDisplay;
            //MessageBox.Show("DisplayUpdated");
        }




    }
}

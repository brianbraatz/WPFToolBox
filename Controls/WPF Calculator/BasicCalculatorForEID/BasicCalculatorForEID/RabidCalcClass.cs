using System;
using System.Collections.Generic;
using System.Text;

namespace RabidCalc
{
    public delegate void DisplayChangeHandler(DisplayChangeEventArgs e);

    //public delegate void DisplayChangeHandler();


    public class DisplayChangeEventArgs : EventArgs
    {
        private string display;
        public string UpdatedDisplay
        {
            get
            {
                return display;
            }
        }
        public DisplayChangeEventArgs()
        {
            display = "EventConstructErr";
        }
        public DisplayChangeEventArgs(string newDisplay)
        {
            display = newDisplay;
        }
    }

    public class CalcClass
    {
        //Enumerate the operations this calculator can do
        private enum Operation
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }

        //Create Display Change event
        public event DisplayChangeHandler OnDisplayChange;

        //Create fields
        private string _memory;
        private string _display;
        private string _lastResult;
        private bool _newDisplay;

        private Operation PendingOp;

        //Create Properties
        private Double Memory
        {
            get
            {
                if (_memory == String.Empty)
                    return 0.0;
                else
                    return Convert.ToDouble(_memory);
            }
            set
            {
                _memory = value.ToString();
            }
        }

        public string Display
        {
            get
            {
                return _display;
            }
            set
            {
                _display = value;
                //OnDisplayChange(new DisplayChangeEventArgs(Display));//No work here
            }
        }

        private Double LastResult
        {
            get
            {
                if (_lastResult == String.Empty)
                    return 0.0;
                else
                    return Convert.ToDouble(_lastResult);
            }
            set
            {
                _lastResult = value.ToString();
            }
        }

        private bool NewDisplay
        {
            get
            {
                return _newDisplay;
            }
            set
            {
                _newDisplay = value;
            }
        }

        //Default class constructor
        public CalcClass()
        {
            PendingOp = Operation.None;
            Display = "0";
            NewDisplay = false;
            LastResult = 0;
        }

        public void CalcInput(string calcInput)
        {
            //If input is a char for display
            //Run AddDigit
            //char c = (calcInput.ToCharArray())[0];
            if (calcInput.Length == 1)
            {
                char[] tempCharArr = calcInput.ToCharArray(0, 1);
                char c = tempCharArr[0];
                //Double check input--will need to modify for HEX, etc.
                if ((c >= '0' && c <= '9') || c == '.')
                {
                    AddDigit(c);
                    return;
                }
            }

            switch (calcInput)
            {
                case "Equals":
                    if (PendingOp != Operation.None)
                    {
                        Calculate();
                        PendingOp = Operation.None;
                        NewDisplay = true;
                        LastResult = Convert.ToDouble(Display);
                        break;
                    }
                    else
                    {
                        break;
                    }

                case "Add":
                    //If NewDisplay = True Need to wait for more input Just update PendingOp
                    if (NewDisplay)
                    {
                        PendingOp = Operation.Add;
                        break;
                    }
                    //If PendingOp != None Calculate PendingOp
                    //Update Display to new result--do in Calculate()
                    //Update PendingOp to new op
                    if (PendingOp != Operation.None)
                    {
                        Calculate();
                        PendingOp = Operation.Add;
                        NewDisplay = true;
                        break;
                    }

                    //If NewDisplay = False & If PendingOp = None
                    //Update PendingOp Set NewDisplay = True
                    //Store Old Display in LastResult
                    else
                    {
                        PendingOp = Operation.Add;
                        NewDisplay = true;
                        LastResult = Convert.ToDouble(Display);
                        break;
                    }

                case "Subtract":
                    if (NewDisplay)
                    {
                        PendingOp = Operation.Subtract;
                        break;
                    }
                    if (PendingOp != Operation.None)
                    {
                        Calculate();
                        PendingOp = Operation.Subtract;
                        NewDisplay = true;
                        break;
                    }
                    else
                    {
                        PendingOp = Operation.Subtract;
                        NewDisplay = true;
                        LastResult = Convert.ToDouble(Display);
                        break;
                    }

                case "Multiply":
                    if (NewDisplay)
                    {
                        PendingOp = Operation.Multiply;
                        break;
                    }
                    if (PendingOp != Operation.None)
                    {
                        Calculate();
                        PendingOp = Operation.Multiply;
                        NewDisplay = true;
                        break;
                    }
                    else
                    {
                        PendingOp = Operation.Multiply;
                        NewDisplay = true;
                        LastResult = Convert.ToDouble(Display);
                        break;
                    }
                case "Divide":
                    if (NewDisplay)
                    {
                        PendingOp = Operation.Divide;
                        break;
                    }
                    if (PendingOp != Operation.None)
                    {
                        Calculate();
                        PendingOp = Operation.Divide;
                        NewDisplay = true;
                        break;
                    }
                    else
                    {
                        PendingOp = Operation.Divide;
                        NewDisplay = true;
                        LastResult = Convert.ToDouble(Display);
                        break;
                    }
                case "ClearEntry":
                    Display = "0";
                    OnDisplayChange(new DisplayChangeEventArgs(Display)); //Fire event
                    break;
                case "Clear":
                    PendingOp = Operation.None;
                    LastResult = 0;
                    Display = "0";
                    OnDisplayChange(new DisplayChangeEventArgs(Display)); //Fire event
                    break;
                case "MemoryStore":
                    Memory = Convert.ToDouble(Display);
                    break;
                case "MemoryClear":
                    Memory = 0;
                    break;
                case "MemoryPlus":
                    Memory = Memory + Convert.ToDouble(Display);
                    break;
                case "MemoryRecall":
                    Display = Convert.ToString(Memory);
                    OnDisplayChange(new DisplayChangeEventArgs(Display)); //Fire event
                    NewDisplay = true;
                    break;

            }
        }

        private void Calculate()
        {
            double operandX = LastResult;
            double operandY = Convert.ToDouble(Display);
            double resultZ = 0;

            switch (PendingOp)
            {
                case Operation.None:
                    break;
                case Operation.Add:
                    resultZ = operandX + operandY;
                    Display = Convert.ToString(resultZ);//resultZ.ToString();
                    break;
                case Operation.Subtract:
                    resultZ = operandX - operandY;
                    Display = Convert.ToString(resultZ);
                    break;
                case Operation.Multiply:
                    resultZ = operandX * operandY;
                    Display = Convert.ToString(resultZ);
                    break;
                case Operation.Divide:
                    if (operandY != 0)
                    {
                        resultZ = operandX / operandY;
                        Display = Convert.ToString(resultZ);
                    }
                    break;

            }
            OnDisplayChange(new DisplayChangeEventArgs(Display)); //Fire event
            LastResult = Convert.ToDouble(Display);
            PendingOp = Operation.None;
        }

        private void AddDigit(char c)
        {
            if (NewDisplay)
            {
                Display = string.Empty;
                NewDisplay = false;
            }
            if (Display == "0")
                Display = String.Empty;
            if (c == '.') //Decimal point
            {
                if (Display.IndexOf('.', 0) >= 0)  //Allow only one decimal per number
                    return;
                if (Display.Length == 0)  //Format if decimal first entry
                    Display = "0.";
                else
                    Display = Display + c;
            }
            else  //If not decimal, then a digit of some sort
            {
                Display = Display + c; //Add digit to end of Display
            }
            //TO DO: Need to make sure display stays under 16 chars
            OnDisplayChange(new DisplayChangeEventArgs(Display)); //Fire event
        }

    } //End class CalcClass

} //End namespace RabidCalc
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
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;


namespace MTADependencyProperties
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {
        public static readonly DependencyProperty MyTextProperty = DependencyProperty.Register("MyText", typeof(string), typeof(Window1));
        public string MyText
        {
            get { return GetValue(MyTextProperty).ToString(); }
            //set { SetValue(MyTextProperty, value); }
            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (SendOrPostCallback)delegate { SetValue(MyTextProperty, value); },
                    value);
            }
        }

        public static readonly DependencyProperty ButtonTitleProperty = DependencyProperty.Register("ButtonTitle", typeof(ButtonStates), typeof(Window1), new UIPropertyMetadata(ButtonStates.Start));


        public ButtonStates ButtonTitle
        {
            get
            {

                try
                {
                    return (ButtonStates)this.Dispatcher.Invoke(
                       System.Windows.Threading.DispatcherPriority.Background,
                       (DispatcherOperationCallback)delegate { return GetValue(ButtonTitleProperty); },
                       ButtonTitleProperty);
                }
                catch
                {
                    
                    return (ButtonStates)ButtonTitleProperty.DefaultMetadata.DefaultValue;
                }
            }
            //get { return (ButtonStates)GetValue(ButtonTitleProperty); }
            set { SetValue(ButtonTitleProperty, value); }
        }


        public Window1()
        {
            InitializeComponent();
        }
        
        void onClick(object sender, RoutedEventArgs e)
        {
            if (ButtonTitle == ButtonStates.Start)
            {
                //start
                new Thread(new ThreadStart(updateText)).Start();
                ButtonTitle = ButtonStates.Stop;
            }
            else
            { 
                //end
                ButtonTitle = ButtonStates.Start;
            }
        }

        Random r;
        void updateText()
        {
            r = new Random();
            while (ButtonTitle == ButtonStates.Stop)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 10; i++)
                {
                    sb.Append((char)r.Next(0x41, 0x51));
                }

                MyText = sb.ToString();

            }
        }

        public enum ButtonStates
        { 
        
            Start,
            Stop
        }

    }
}
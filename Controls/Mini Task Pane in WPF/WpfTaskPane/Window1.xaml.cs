using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTaskPane
{
    internal class ExpanderState : INotifyPropertyChanged
    {
        // Event required by INotifyPropertyChanged interface.
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isExpanded=true;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                bool isChanged = false;
                if (isExpanded != value)
                    isChanged = true;

                isExpanded = value;

                if (isChanged)
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }
    }

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ExpanderState expanderState = new ExpanderState();      
        public Window1()
        {
            InitializeComponent();
            ExpanderButton.DataContext = expanderState;
  
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            DoubleAnimation da = new DoubleAnimation();
            da.To =expanderState.IsExpanded? 0:1;
            da.From = expanderState.IsExpanded ? 1 : 0;
            da.Duration = TimeSpan.FromSeconds(0.4);
            da.Completed += new EventHandler(da_Completed);

            DoubleAnimation rotateAni = new DoubleAnimation();
            rotateAni.From = 0;
            rotateAni.To = -20.0;
            rotateAni.Duration = TimeSpan.FromMilliseconds(200);
            
            mainPanelScale.BeginAnimation(ScaleTransform.ScaleYProperty, da);
            ImageIconRotateTrans.BeginAnimation(RotateTransform.AngleProperty, rotateAni);
        }

        private void da_Completed(object sender, EventArgs e)
        {
            expanderState.IsExpanded = !expanderState.IsExpanded;
        }
    }
}

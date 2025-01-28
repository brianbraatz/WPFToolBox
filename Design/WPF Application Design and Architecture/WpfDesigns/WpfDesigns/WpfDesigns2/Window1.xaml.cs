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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDesigns2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            //when an operation completes from the Operations user control, update the data in the ReflectionDisplay user contrl
            this.OperationsControl.ReflectionOperationComplete += new EventHandler<ReflectionOperationEventArgs>(OperationsControl_ReflectionOperationComplete);
        }

        void OperationsControl_ReflectionOperationComplete(object sender, ReflectionOperationEventArgs e)
        {
            //updating the property will update the listbox which is bound to the ReflectionItems dependency property
            this.ReflectionResultsControl.ReflectionResults = e.Results;
        }
    }
}

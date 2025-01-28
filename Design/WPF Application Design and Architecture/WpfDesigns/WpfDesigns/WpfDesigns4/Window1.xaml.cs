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
using WpfDesigns4.Views;

namespace WpfDesigns4
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Window1_Loaded);

            CommandBinding swapViewsCommandBinding = new CommandBinding(ViewCommands.SwapViews);
            swapViewsCommandBinding.Executed += new ExecutedRoutedEventHandler(swapViewsCommandBinding_Executed);
            this.CommandBindings.Add(swapViewsCommandBinding);
        }

        void swapViewsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ApplicationManager.ViewState.SwapViews();
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
//            this.ApplicationManager.InitializeViewState();
        }

        private ApplicationManager ApplicationManager
        {
            get
            {
                ObjectDataProvider data = Resources["ApplicationManager"] as ObjectDataProvider;
                return data.Data as ApplicationManager;
            }
        }
    }
}

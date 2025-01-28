using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using WpfDesigns4.ViewModels;

namespace WpfDesigns4.Views
{
    public class OperationsView : Control
    {
        static OperationsView()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(OperationsView), new FrameworkPropertyMetadata(typeof(OperationsView)));
        }

        public OperationsView()
        {
            CommandBinding findAssembliesCommandBinding = new CommandBinding(ReflectionCommands.FindAssemblies);
            findAssembliesCommandBinding.Executed += new ExecutedRoutedEventHandler(findAssembliesCommandBinding_Executed);
            this.CommandBindings.Add(findAssembliesCommandBinding);

            CommandBinding findTypesCommandBinding = new CommandBinding(ReflectionCommands.FindTypes);
            findTypesCommandBinding.Executed += new ExecutedRoutedEventHandler(findTypesCommandBinding_Executed);
            this.CommandBindings.Add(findTypesCommandBinding);
        }

        void findAssembliesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.FindAssemblies();
        }

        void findTypesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.FindTypes();
        }

        private OperationsViewModel ViewModel
        {
            get
            {
                return this.DataContext as OperationsViewModel;
            }
        }
    }
}

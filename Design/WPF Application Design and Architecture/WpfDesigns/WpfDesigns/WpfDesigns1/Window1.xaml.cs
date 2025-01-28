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
using System.Reflection;

namespace WpfDesigns1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string _matchText;

        public Window1()
        {
            InitializeComponent();
        }

        public void ShowAssemblies_Click(object sender, RoutedEventArgs e)
        {
            //example of not using data binding and directly updating GUI
            this.ResultsListBox.ItemsSource = FindAssemblyNames();
        }

        public void ShowTypes_Click(object sender, RoutedEventArgs e)
        {
            this.ResultsListBox.ItemsSource = FindTypeNames();
        }

        public void MatchTextChanged(object sender, RoutedEventArgs e)
        {
            this._matchText = this.MatchTextBox.Text;
        }

        private List<string> FindAssemblyNames()
        {
            List<string> assemblyNames = new List<string>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (this._matchText == "*" || assembly.FullName.Contains(this._matchText))
                {
                    assemblyNames.Add(assembly.FullName);
                }
            }

            return assemblyNames;
        }

        private List<string> FindTypeNames()
        {
            List<string> typeNames = new List<string>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (this._matchText == "*" || type.FullName.Contains(this._matchText))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            return typeNames;
        }
    }
}

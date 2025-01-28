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
using System.Windows.Controls.Primitives;
using System.Diagnostics;


namespace MouseOverMenuItemSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem parentMenu = e.OriginalSource as MenuItem;
            if (parentMenu.HasItems)
            {
                if (parentMenu.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    EventHandler itemsGenerated = null;
                    itemsGenerated = (EventHandler)delegate(object o, EventArgs args)
                    {
                        if (parentMenu.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                        {
                            parentMenu.ItemContainerGenerator.StatusChanged -= itemsGenerated;
                            EnableSubmenuItems(parentMenu);
                        }
                    };
                    parentMenu.ItemContainerGenerator.StatusChanged += itemsGenerated;
                }
                else
                {
                    EnableSubmenuItems(parentMenu);
                }
            }
        }

        private void EnableSubmenuItems(MenuItem parentMenu)
        {
            foreach (object item in parentMenu.Items)
            {
                MenuItem childMenuItem = parentMenu.ItemContainerGenerator
                        .ContainerFromItem(item) as MenuItem;
                if (childMenuItem != null)
                {
                    int rand = ((new Random((int)DateTime.Now.Ticks)).Next(100));
                    Debug.WriteLine(rand);
                    childMenuItem.IsEnabled = (rand % 2 == 0);
                }
            }
        }

    }
}
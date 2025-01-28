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
using System.Collections.ObjectModel;


namespace ListBoxofLists
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

        void onSelection(object sender, SelectionChangedEventArgs e)
        {
            Employees emps = this.Resources["employees"] as Employees;

            if (emps != null)
            {
                for (int i = 0; i < e.RemovedItems.Count; i++)
                {
                    Project p = e.RemovedItems[i] as Project;
                    for (int j = 0; j < p.Employees.Count; j++)
                    {
                        if (emps.Contains(p.Employees[j]))
                        {
                            if(emps[emps.IndexOf(p.Employees[j])].Width == 0)
                            {
                                emps.Remove(p.Employees[j]);
                            }
                            else
                            {
                                emps[emps.IndexOf(p.Employees[j])].Width--;
                            }
                        }
                    }
                }

                for (int i = 0; i < e.AddedItems.Count; i++)
                {
                    Project p = e.AddedItems[i] as Project;
                    for (int j = 0; j < p.Employees.Count; j++)
                    {
                        if (!emps.Contains(p.Employees[j]))
                        {
                            emps.Add(p.Employees[j]);
                        }
                        else
                        {
                            emps[emps.IndexOf(p.Employees[j])].Width++;
                        }
                    }
                }
            }
        }

    }
}
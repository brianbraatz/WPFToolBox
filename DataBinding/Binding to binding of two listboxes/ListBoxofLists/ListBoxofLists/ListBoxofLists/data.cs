using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections;
using System.Windows;

namespace ListBoxofLists
{
    #region dummy data
    public class Employee
    {
        public Employee(string name)
        {
            m_name = name;
        }

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public override string ToString()
        {
            return m_name;
        }

        private int m_width;

        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }
	
	
    }

    public class Project
    {

        public Project(string name)
        {
            m_name = name;
        }

        public Project(string name, params Employee[] employees)
        {
            m_name = name;

            m_employees = new Employees();
            
            Array.ForEach(employees, m_employees.Add);
            
        }
        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }


        private Employees m_employees;

        public Employees Employees
        {
            get {
                if (m_employees == null)
                    m_employees = new Employees();
                return m_employees; }
        }

        public override string ToString()
        {
            return m_name;
        }
	
    }

    public class Projects : ObservableCollection<Project>
    {
        public Projects()
        {
            Random _r = new Random();
            for (int i = 0; i < 10; i++)
            {
                Project p = new Project("Project "+i);
                
                for (int j = 0; j < 15; j++)
                {
                    p.Employees.Add(AllEmployees[_r.Next(AllEmployees.Count - 1)]);
                }


                base.Add(p);
            }
            
        }

        private List<Employee> _allEmployees;

        internal List<Employee> AllEmployees
        {
            get {
                if (_allEmployees == null)
                {
                    _allEmployees = new List<Employee>();
                    for (int i = 0; i < 20; i++)
                    {
                        _allEmployees.Add(new Employee("Employee " + (i + 1)));
                    }
                }

                return _allEmployees; }
        }

    }

    public class Employees : ObservableCollection<Employee> { }
    #endregion

    

    #region converter
    public class WidthToSizeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double size = double.Parse(value.ToString());

            if(size == 0.0)
                size = Application.Current.MainWindow.FontSize;
            else
                size *= Application.Current.MainWindow.FontSize;

            return size ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
    #endregion
}

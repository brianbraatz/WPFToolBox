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

namespace StyleSelector
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
            this.DataContext = new Source();
            ContainerStyle cs = new ContainerStyle();
            cs.StyleClassA = this.Resources["StyleClassA"] as Style;
            cs.StyleClassB = this.Resources["StyleClassB"] as Style;
            tv.ItemContainerStyleSelector = cs;
        }

    }

    public class ContainerStyle : System.Windows.Controls.StyleSelector
    {
        private Style styleClassA;

        public Style StyleClassA
        {
            get { return styleClassA; }
            set { styleClassA = value; }
        }

        private Style styleClassB;

        public Style StyleClassB
        {
            get { return styleClassB; }
            set { styleClassB = value; }
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if(item is ClassA)
            {
                return styleClassA;
            }
            else if(item is ClassB)
            {
                return styleClassB;
            }
            return null;
        }
    }

    public interface IBase
    {
    }

    public class ClassA : IBase
    {
        private int myInt;

        public int MyInt
        {
            get { return myInt; }
            set { myInt = value; }
        }

        public ClassA(int myItem)
        {
            this.myInt = myInt;
        }
    }

    public class ClassB : IBase
    {
        private string myString;

        public string MyString
        {
            get { return myString; }
            set { myString = value; }
        }

        public ClassB(string myString)
        {
            this.myString = myString;
        }
    }

    public class Source
    {
        private ObservableCollection<IBase> myCollection;

        public ObservableCollection<IBase> MyCollection
        {
            get { return myCollection; }
            set { myCollection = value; }
        }

        public Source()
        {
            this.myCollection = new ObservableCollection<IBase>();
            this.myCollection.Add(new ClassA(1));
            this.myCollection.Add(new ClassB("A"));
            this.myCollection.Add(new ClassA(2));
            this.myCollection.Add(new ClassB("B"));
            this.myCollection.Add(new ClassA(3));
            this.myCollection.Add(new ClassB("C"));
        }
    }
}
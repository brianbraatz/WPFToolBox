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
using System.Xml;


namespace TriggersAndEvents
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {

        public Window1()
        {
            InitializeComponent();
            initInter();
        }

        private void initInter()
        {
            XmlDataProvider data = new XmlDataProvider();
            data.Document = new System.Xml.XmlDocument();
            data.Document.InnerXml = getXMLstring();
            data.XPath = "/root/node";
            
            Binding itemsBinding = new Binding();
            itemsBinding.XPath = "val";

            FrameworkElementFactory comboFactory = new FrameworkElementFactory(typeof(ComboBox));
            comboFactory.Name = "myComboFactory";
            comboFactory.SetBinding(ComboBox.ItemsSourceProperty, itemsBinding);
            comboFactory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(popup));
            
            DataTemplate itemsTemplate = new DataTemplate();
            itemsTemplate.VisualTree = comboFactory;

            Binding nodeBinding = new Binding();
            nodeBinding.Source = data;

            ListView lv = new ListView();
            lv.ItemTemplate = itemsTemplate;
            lv.SetBinding(ListView.ItemsSourceProperty, nodeBinding);

            myGrid.Children.Add(lv);

        }

        void popup(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                XmlElement el = e.AddedItems[0] as XmlElement;
                MessageBox.Show("Got selection changed from item number "+el.ParentNode.SelectSingleNode("@id").InnerText+Environment.NewLine+"The selection is: "+el.InnerText);
            }
            
        }

        string getXMLstring()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            for (int i = 0; i < 10; i++)
            {
                sb.Append("<node id='"+i+"'>");
                for (int j = 0; j < 20; j++)
                {
                    sb.Append("<val>val" + i + "-"+ j +"</val>");
                }
                sb.Append("</node>");
            }
            sb.Append("</root>");

            return sb.ToString();
        }

    }
}
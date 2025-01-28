
using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Navigation;

namespace SDKSamples
{


    public partial class app : Application
    {
    
        public app()
        {
            InitializeComponent();
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
        
            Window myWindow = new Window();
            myWindow.Content = new SampleViewer();
            myWindow.Show();
            this.MainWindow = myWindow;
            base.OnStartup(e);       
        }
       
        
    }
}
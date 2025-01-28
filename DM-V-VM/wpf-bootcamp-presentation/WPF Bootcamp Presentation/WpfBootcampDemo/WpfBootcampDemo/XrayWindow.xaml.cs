using System.Windows;
using System.Windows.Input;
using WpfBootcampDemo.Controllers;
using WpfBootcampDemo.Model;

namespace WpfBootcampDemo
{
    /// <summary>
    /// The main Window of the demo app.
    /// </summary>
    public partial class XrayWindow : Window
    {
        #region Data

        readonly XrayWindowController _controller;

        #endregion // Data

        #region Constructor

        public XrayWindow()
        { 
            InitializeComponent();

            XrayCollection xrays = XrayCollection.Load();

            // Create the controller that  
            // handles user interaction.
            _controller = new XrayWindowController(this, xrays);

            // Use the list of Xray objects as 
            // this Window's data source.
            base.DataContext = xrays;
        }

        #endregion // Constructor

        #region Command Sinks

        // These methods handle events of  
        // the ShowSelectedXray command.

        void ShowSelectedXray_CanExecute(
            object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _controller.CanShowSelectedXray;
        }

        void ShowSelectedXray_Executed(
            object sender, ExecutedRoutedEventArgs e)
        {
            _controller.ShowSelectedXray();
        }

        #endregion // Command Sinks
    }
}
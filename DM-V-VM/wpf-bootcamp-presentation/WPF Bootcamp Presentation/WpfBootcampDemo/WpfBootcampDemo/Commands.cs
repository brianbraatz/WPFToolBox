using System.Windows.Input;
using WpfBootcampDemo.Properties;

namespace WpfBootcampDemo
{
    /// <summary>
    /// Contains application-specific routed commands.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Executed when the selected Xray's 
        /// image should be displayed.
        /// </summary>
        public static readonly RoutedUICommand ShowSelectedXray;

        static Commands()
        {
            ShowSelectedXray = new RoutedUICommand(
                Resources.ShowSelectedXrayCommandText,
                "ShowSelectedXray",
                typeof(Commands));
        }
    }
}
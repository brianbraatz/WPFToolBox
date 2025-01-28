using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfDesigns4.Views
{
    public static class ViewCommands
    {
        public static readonly RoutedCommand SwapViews = new RoutedCommand("SwapViews", typeof(ReflectionCommands));
    }
}

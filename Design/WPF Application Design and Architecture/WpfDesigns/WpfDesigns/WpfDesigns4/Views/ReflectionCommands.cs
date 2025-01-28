using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfDesigns4.Views
{
    public static class ReflectionCommands
    {
        public static readonly RoutedCommand FindAssemblies = new RoutedCommand("FindAssemblies", typeof(ReflectionCommands));
        public static readonly RoutedCommand FindTypes = new RoutedCommand("FindTypes", typeof(ReflectionCommands));
    }
}

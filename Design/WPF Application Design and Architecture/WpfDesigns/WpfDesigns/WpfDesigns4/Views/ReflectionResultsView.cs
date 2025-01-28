using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WpfDesigns4.Views
{
    public class ReflectionResultsView : Control
    {
        static ReflectionResultsView()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ReflectionResultsView), new FrameworkPropertyMetadata(typeof(ReflectionResultsView)));
        }
    }
}

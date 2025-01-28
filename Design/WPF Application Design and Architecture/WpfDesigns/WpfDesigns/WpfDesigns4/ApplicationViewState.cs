using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace WpfDesigns4
{
    public class ApplicationViewState : DependencyObject
    {
        public static readonly DependencyProperty LeftSideViewProperty =
            DependencyProperty.Register("LeftSideView", typeof(object), typeof(ApplicationViewState), new UIPropertyMetadata(null));

        public static readonly DependencyProperty RightSideViewProperty =
            DependencyProperty.Register("RightSideView", typeof(object), typeof(ApplicationViewState), new UIPropertyMetadata(null));

        public ApplicationViewState()
        {
        }

        public object LeftSideView
        {
            get { return (object)GetValue(LeftSideViewProperty); }
            set 
            { 
                SetValue(LeftSideViewProperty, value); }
        }
        public object RightSideView
        {
            get { return (object)GetValue(RightSideViewProperty); }
            set { SetValue(RightSideViewProperty, value); }
        }

        public void SwapViews()
        {
            object left = this.LeftSideView;
            this.LeftSideView = this.RightSideView;
            this.RightSideView = left;
        }
    }
}

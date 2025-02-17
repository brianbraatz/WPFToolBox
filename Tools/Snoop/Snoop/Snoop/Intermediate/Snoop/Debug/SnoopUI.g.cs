//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Snoop;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Snoop {
    
    
    /// <summary>
    /// SnoopUI
    /// </summary>
    public partial class SnoopUI : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        internal System.Windows.Controls.ComboBox TreeFilter;
        
        internal Snoop.ProperTreeView Tree;
        
        internal System.Windows.Controls.GridSplitter GridSplitter;
        
        internal Snoop.PropertyGrid2 PropertyGrid;
        
        internal Snoop.Previewer PreviewArea;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Snoop;component/snoopui.xaml", System.UriKind.Relative);
            System.Windows.Application.LoadComponent(this, resourceLocater);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TreeFilter = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.Tree = ((Snoop.ProperTreeView)(target));
            return;
            case 3:
            this.GridSplitter = ((System.Windows.Controls.GridSplitter)(target));
            return;
            case 4:
            this.PropertyGrid = ((Snoop.PropertyGrid2)(target));
            return;
            case 5:
            this.PreviewArea = ((Snoop.Previewer)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

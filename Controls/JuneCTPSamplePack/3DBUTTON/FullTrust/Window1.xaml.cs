namespace FullTrust
{
    using System;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Windows.Navigation;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.Threading;
    using System.Windows.Markup;
    using System.Xml;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Security;
    using System.Windows.Interop;
    using HitTestResult2D = System.Windows.Media.HitTestResult;
    using Common;
    
    public partial class Window1
    {
   
        void OnInitialized(object sender, EventArgs e)
        {
            
        }

        private void OnLoaded(object sender, EventArgs e)
        {

        }

        #region Events

        void OnDarkBlue(object sender, EventArgs e)
        {
            Brush brush = this.FindResource ("DarkBlueBrush") as Brush;
            TextBlock textblock = this.FindResource("DarkBlueText") as TextBlock;
            Big3DButton.Brush3D = brush;
            Big3DButton.Text3D = textblock;
        }

        void OnDarkGreen(object sender, EventArgs e)
        {
            Brush brush = this.FindResource("DarkGreenBrush") as Brush;
            TextBlock textblock = this.FindResource("DarkGreenText") as TextBlock;
            Big3DButton.Brush3D = brush;
            Big3DButton.Text3D = textblock;
        }

        void OnDarkGray(object sender, EventArgs e)
        {
            Brush brush = this.FindResource("DarkGrayBrush") as Brush;
            TextBlock textblock = this.FindResource("DarkGrayText") as TextBlock;
            Big3DButton.Brush3D = brush;
            Big3DButton.Text3D = textblock;
        }

        void OnDarkRed(object sender, EventArgs e)
        {
            Brush brush = this.FindResource("DarkRedBrush") as Brush;
            TextBlock textblock = this.FindResource("DarkRedText") as TextBlock;
            Big3DButton.Brush3D = brush;
            Big3DButton.Text3D = textblock;
        }

        #endregion
    }
}
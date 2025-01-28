using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interop;

namespace FloatPanelNS
{
    public partial class FloatPanelApp : System.Windows.Application
    {
        HwndSource testHwnd;

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);

            //Rect workArea = SystemParameters.WorkArea;

            HwndSourceParameters parameters = new HwndSourceParameters("TestFloatWindowTitle");
            parameters.PositionX = 0;
            parameters.PositionY = 0;
            parameters.Width = (int)(SystemParameters.VirtualScreenWidth);
            parameters.Height = (int)(SystemParameters.VirtualScreenHeight);

            parameters.UsesPerPixelOpacity = true;
            testHwnd = new HwndSource(parameters);

            object o = Application.LoadComponent(new Uri("MainScene.baml", UriKind.Relative));
            FrameworkElement child = o as FrameworkElement;

            testHwnd.RootVisual = child;
            testHwnd.Disposed += new EventHandler(testHwnd_Disposed);

        }

        void testHwnd_Disposed(object sender, EventArgs e)
        {
            this.Shutdown();
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace RabidCalc
{
    public partial class CalcApp : System.Windows.Application
    {
        void StartupCalc(object sender, StartupEventArgs e)
        {
            Window CalcWindow = new RabidCalc.DisplayCalcScene();
            CalcWindow.Title = "RabidCalc";
            CalcWindow.Height = 196;
            CalcWindow.Width = 174;
            CalcWindow.Show();
        }
    }
}
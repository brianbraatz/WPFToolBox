using System;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using DemoDev;

namespace Ribbon
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Background3D : UserControl
    {
        public Background3D()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            //ControlTemplate ct = (ControlTemplate)this.FindResource("UserControlTemplateOverride");
            //this.Template = ct;

            
            Storyboard s;

            ControlTemplate ct = this.Template as ControlTemplate;
            s = (Storyboard)ct.Resources["ImageBrushOne"];
            s.Begin(this,ct);
            s = (Storyboard)ct.Resources["ImageBrushTwo"];
            s.Begin(this, ct);
            s = (Storyboard)ct.Resources["ImageBrushThree"];
            s.Begin(this, ct);
            s = (Storyboard)ct.Resources["ImageBrushFour"];
            s.Begin(this, ct);

            /*
            s = (Storyboard)this.Template.Resources["ImageBrushTwo"];
            this.BeginStoryboard(s);
            s = (Storyboard)this.Template.Resources["ImageBrushThree"];
            this.BeginStoryboard(s);
            s = (Storyboard)this.Template.Resources["ImageBrushFour"];
            this.BeginStoryboard(s);
             */
 
        }

        #region Events

        #endregion

        #region Globals


        #endregion
    }

   
}




using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PathGeo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        public void OnLoad(Object sender, EventArgs e)
        {
            //Create the text
            FormattedText formattedText = new FormattedText(
                        "TEXT",
                        new System.Globalization.CultureInfo("en-US"),
                        FlowDirection.LeftToRight,
                        new Typeface(
                            new FontFamily("Segoe UI"),
                            FontStyles.Normal,
                            FontWeights.UltraBlack,
                            FontStretches.Normal
                            ),
                            90,
                            Brushes.Red);
            formattedText.SetFontWeight(FontWeights.Bold, 0, 4);

            //Build a geometry out of the text
            Geometry geo = new PathGeometry();
            geo = formattedText.BuildGeometry(new Point(0, 0));

            // Flatten the geometry and create a PathGeometry out of it
            PathGeometry pathGeo = new PathGeometry();
            pathGeo = geo.GetFlattenedPathGeometry();

            //Use the PathGeometry for the empty placeholder Path element in XAML 
            path.Data = pathGeo;

            //Use the PathGeometry for the matrix animation
            matrixAnim.PathGeometry = pathGeo;  

        }

    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Input;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Controls.xaml
    /// </summary>

    public partial class Controls : Page
    {
        public Controls()
        {
            InitializeComponent();
        }

        void ChangeFishies(object sender, RoutedEventArgs e)
        {
            if (e.Source != BigFish)
                return;

            if (BigFish.Style != null)
            {
                _oldBigFishStyle = BigFish.Style;
                _oldSmallFishStyle = SmallFish.Style;
                _oldBigFishTop = Canvas.GetTop(BigFish);
                _oldBigFishLeft = Canvas.GetLeft(BigFish);
                _oldBigFishPadding = BigFish.Padding;

                BigFish.Padding = new Thickness(10);
                BigFish.Style = null;
                SmallFish.Style = null;
                //Canvas.SetTop(BigFish, 150);
                //Canvas.SetLeft(BigFish, 200);
            }
            else
            {
                BigFish.Padding = _oldBigFishPadding;
                BigFish.Style = _oldBigFishStyle;
                SmallFish.Style = _oldSmallFishStyle;
                //Canvas.SetTop(BigFish, _oldBigFishTop);
                //Canvas.SetLeft(BigFish, _oldBigFishLeft);
            }
        }

        Style _oldBigFishStyle;
        Style _oldSmallFishStyle;
        double _oldBigFishTop;
        double _oldBigFishLeft;
        Thickness _oldBigFishPadding;
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Text.xaml
    /// </summary>

    public partial class Text : Page
    {
        int counter = -1;

        public Text()
        {
            InitializeComponent();
        }

        void GoForward(object sender, RoutedEventArgs e)
        {
            if (counter >= 4) return;

            counter++;

            Storyboard s = getStoryboard();

            if (s != null)
                BeginStoryboard(s);
        }

        private Storyboard getStoryboard()
        {
            Storyboard s = null;

            switch (counter)
            {
                case 0:
                    s = this.TryFindResource("GrowText") as Storyboard;
                    break;

                case 1:
                    s = this.TryFindResource("WipeText") as Storyboard;
                    break;

                case 2:
                    s = this.TryFindResource("ShowEmbeddedFonts") as Storyboard;
                    break;

                case 3:
                    s = this.TryFindResource("HideEmbeddedFonts") as Storyboard;
                    break;

                default:
                    s = null;
                    break;
            }
            return s;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Vectors.xaml
    /// </summary>

    public partial class Vectors : Page
    {
        int counter = -1;

        public Vectors()
        {
            InitializeComponent();
        }

        void GoBack(object sender, RoutedEventArgs e)
        {
            if (counter <= 0) return;

            counter--;

            Storyboard s = getStoryboard();

            if (s != null)
                BeginStoryboard(s);
        }

        void GoForward(object sender, RoutedEventArgs e)
        {
            if (counter >= 5) return;

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
                    s = this.TryFindResource("GrowFace") as Storyboard;
                    break;

                case 1:
                    s = this.TryFindResource("ShrinkFace") as Storyboard;
                    break;

                case 2:
                    s = this.TryFindResource("RotateAndSkew") as Storyboard;
                    break;

                case 3:
                    s = this.TryFindResource("RemoveLines") as Storyboard;
                    break;

                case 4:
                    s = this.TryFindResource("ChangeHair") as Storyboard;
                    break;

                default:
                    s = null;
                    break;
            }
            return s;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Collections.Generic;

namespace FeatureMontage
{
    /// <summary>
    /// Interaction logic for Effects.xaml
    /// </summary>

    public partial class Effects : Page
    {
        public Effects()
        {
            InitializeComponent();

            _blur = new BlurBitmapEffect();
            Binding blurBind = new Binding("Value");
            blurBind.ElementName = "BlurRadius";
            BindingOperations.SetBinding(_blur, BlurBitmapEffect.RadiusProperty, blurBind);


            // <DropShadowBitmapEffect Color="{Binding ElementName=DropShadowColor, Path=SelectedItem.Content}"
            //                         Direction="{Binding ElementName=DropShadowDirection, Path=Value}"
            //                         Opacity="{Binding ElementName=DropShadowOpacity, Path=Value}"
            //                         ShadowDepth="{Binding ElementName=DropShadowShadowDepth, Path=Value}"
            //                         Softness="{Binding ElementName=DropShadowSoftness, Path=Value}" />

            _dropShadow = new DropShadowBitmapEffect();
            Binding dropBind = new Binding("SelectedItem.Content");
            dropBind.ElementName = "DropShadowColor";
            BindingOperations.SetBinding(_dropShadow, DropShadowBitmapEffect.ColorProperty, dropBind);

            dropBind = new Binding("Value");
            dropBind.ElementName = "DropShadowDirection";
            BindingOperations.SetBinding(_dropShadow, DropShadowBitmapEffect.DirectionProperty, dropBind);

            dropBind = new Binding("Value");
            dropBind.ElementName = "DropShadowOpacity";
            BindingOperations.SetBinding(_dropShadow, DropShadowBitmapEffect.OpacityProperty, dropBind);

            dropBind = new Binding("Value");
            dropBind.ElementName = "DropShadowShadowDepth";
            BindingOperations.SetBinding(_dropShadow, DropShadowBitmapEffect.ShadowDepthProperty, dropBind);

            dropBind = new Binding("Value");
            dropBind.ElementName = "DropShadowSoftness";
            BindingOperations.SetBinding(_dropShadow, DropShadowBitmapEffect.SoftnessProperty, dropBind);
        }


        void ChangeEffectContent(object sender, EventArgs e)
        {
            string SelectedValue = ((ComboBoxItem)EffectContent.SelectedValue).Content as string;
            if ((SelectedValue != null) && (GuineaPig != null))
            {
                if (SelectedValue == "Text")
                {
                    UIElement text = GridDude.TryFindResource("EffectsText") as UIElement;
                    if (text != null)
                        GuineaPig.Content = text;
                }
                else if (SelectedValue == "Image")
                {
                    UIElement limeCat = GridDude.TryFindResource("LimeCat") as UIElement;
                    if (limeCat != null)
                        GuineaPig.Content = limeCat;
                }
                else
                {
                    UIElement fishy = GridDude.TryFindResource("SmallFish") as UIElement;
                    if (fishy != null)
                        GuineaPig.Content = fishy;
                }
            }
        }

        void EnableBlur(object sender, EventArgs e)
        {
            GuineaPigBitmapEffectGroup.Children.Add(_blur);
        }

        void DisableBlur(object sender, EventArgs e)
        {
            GuineaPigBitmapEffectGroup.Children.Remove(_blur);
        }

        void EnableDropShadow(object sender, EventArgs e)
        {
            GuineaPigBitmapEffectGroup.Children.Add(_dropShadow);
        }

        void DisableDropShadow(object sender, EventArgs e)
        {
            GuineaPigBitmapEffectGroup.Children.Remove(_dropShadow);
        }


        BlurBitmapEffect _blur;
        DropShadowBitmapEffect _dropShadow;
    }
}
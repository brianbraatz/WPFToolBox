//--------------------------------------------------
// TryPrinting.cs (c) 2006 by Charles Petzold
//--------------------------------------------------
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Printing;

namespace Petzold.TryPrinting
{
    public class TryPrinting : Window
    {
        // Private fields to pass information between Page Setup and Print.                
        PrintTicket prntkt;
        Thickness marginPage = new Thickness(96);

        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new TryPrinting());
        }
        public TryPrinting()
        {
            Title = "Try Printing";
            FontSize = 24;

            StackPanel stack = new StackPanel();
            Content = stack;

            Button btn = new Button();
            btn.Content = "Page Set_up...";
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.Margin = new Thickness(24);
            btn.Click += SetupOnClick;
            stack.Children.Add(btn);

            btn = new Button();
            btn.Content = "_Print...";
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.Margin = new Thickness(24);
            btn.Click += PrintOnClick;
            stack.Children.Add(btn);
        }
        void SetupOnClick(object sender, RoutedEventArgs args)
        {
            PageSetupDialog dlg = new PageSetupDialog();

            // Initialize PrintTicket if available.
            if (prntkt != null)
                dlg.SetPrintTicket(prntkt);

            // Initialize Margin properties (convert to 1/1000 mm).
            dlg.LeftMargin = DiuToMicrometer(marginPage.Left);
            dlg.TopMargin = DiuToMicrometer(marginPage.Top);
            dlg.RightMargin = DiuToMicrometer(marginPage.Right);
            dlg.BottomMargin = DiuToMicrometer(marginPage.Bottom);

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                // Save PrintTicket and margins from dialog box.
                prntkt = dlg.GetPrintTicket();

                marginPage = new Thickness(MicrometerToDiu(dlg.LeftMargin), 
                                           MicrometerToDiu(dlg.TopMargin), 
                                           MicrometerToDiu(dlg.RightMargin), 
                                           MicrometerToDiu(dlg.BottomMargin));
            }
        }
        void PrintOnClick(object sender, RoutedEventArgs args)
        {
            PrintDialog dlg = new PrintDialog();

            // Set PrintTicket from field.
            if (prntkt != null)
                dlg.SetPrintTicket(prntkt);

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                // Create DrawingVisual and open DrawingContext.
                DrawingVisual vis = new DrawingVisual();
                DrawingContext dc = vis.RenderOpen();
                Pen pn = new Pen(Brushes.Black, 1);

                // Rectangle describes page less margins.
                Rect rectPage = new Rect(marginPage.Left, marginPage.Top,
                                    dlg.PrintableAreaWidth - 
                                        (marginPage.Left + marginPage.Right),
                                    dlg.PrintableAreaHeight - 
                                        (marginPage.Top + marginPage.Bottom));

                // Draw rectangle to reflect user's margins.
                dc.DrawRectangle(null, pn, rectPage);

                // Create formatted text object showing PrintableArea properties.
                FormattedText formtxt = new FormattedText(
                    String.Format("Hello, Printer! {0} x {1}", 
                                  dlg.PrintableAreaWidth / 96, 
                                  dlg.PrintableAreaHeight / 96),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Times New Roman"), 
                                 FontStyles.Italic, FontWeights.Normal, 
                                 FontStretches.Normal), 
                    48, Brushes.Black);

                // Get size of text.
                Size sizeText = new Size(formtxt.Width, formtxt.Height);

                // Calculate point to center text within margins.
                Point ptText = 
                    new Point(rectPage.Left + 
                                    (rectPage.Width - formtxt.Width) / 2,
                              rectPage.Top + 
                                    (rectPage.Height - formtxt.Height) / 2);

                // Draw text and surrounding rectangle.
                dc.DrawText(formtxt, ptText);
                dc.DrawRectangle(null, pn, new Rect(ptText, sizeText));

                // Close DrawingContext.
                dc.Close();

                // Rotate graphics based on PageOrientation property.
                switch (dlg.GetPrintTicket().PageOrientation)
                {
                    case PageOrientation.Landscape:
                        vis.Transform = 
                            new RotateTransform(-90, dlg.PrintableAreaWidth / 2,
                                                     dlg.PrintableAreaWidth / 2);
                        break;

                    case PageOrientation.ReversePortrait:
                        vis.Transform = 
                            new RotateTransform(180, dlg.PrintableAreaWidth / 2,
                                                     dlg.PrintableAreaHeight / 2);
                        break;

                    case PageOrientation.ReverseLandscape:
                        vis.Transform = 
                            new RotateTransform(90, dlg.PrintableAreaHeight / 2,
                                                    dlg.PrintableAreaHeight / 2);
                        break;
                }

                // Finally, print the page.
                dlg.PrintVisual(vis, "My first print job");
            }
        }            
        // Methods to convert between device-independent units and 
        //      thousandths of millimeters.
        double MicrometerToDiu(double d)
        {
            return 96 * d / 25400;
        }
        double DiuToMicrometer(double d)
        {
            return 25400 * d / 96;
        }
    }
}

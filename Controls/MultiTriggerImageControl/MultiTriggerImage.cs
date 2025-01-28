#region using directives
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
#endregion

namespace BrunoSilvaUi.Decorators
{
    public class MultiTriggerImage : Decorator
    {
        #region Initialization
        public MultiTriggerImage() { }
        #endregion

        #region private members
        private Int32Rect[] viewBoxes;
        private ImageBrush normalBrush;
        private ImageBrush overBrush;
        private ImageBrush pressedBrush;
        private ImageBrush disabledBrush;
        private ImageBrush defaultBrush;
        private int previousState = 3;
        private Rect[] sourceRects;
        private Rect[] destRects;
        #endregion

        #region Properties
        #region Image
        public BitmapImage Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapImage), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(default(BitmapImage), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTriggersCountChanged)));
        #endregion

        #region IsDefault
        public bool IsDefault
        {
            get { return (bool)GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
        }
        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region TriggersCount
        public int TriggersCount
        {
            get { return (int)GetValue(TriggersCountProperty); }
            set { SetValue(TriggersCountProperty, value); }
        }
        public static readonly DependencyProperty TriggersCountProperty =
            DependencyProperty.Register("TriggersCount", typeof(int), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTriggersCountChanged)));

        private static void OnTriggersCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MultiTriggerImage control = sender as MultiTriggerImage;
            if (control != null && control.Image != null && control.TriggersCount > 0)
            {
                control.viewBoxes = new Int32Rect[control.TriggersCount];

                for (int i = 0; i < control.TriggersCount; i++)
                {
                    control.viewBoxes[i].X = 0;
                    control.viewBoxes[i].Width = (int)(control.Image.PixelWidth);
                    control.viewBoxes[i].Height = (int)(control.Image.PixelHeight / control.TriggersCount);
                    control.viewBoxes[i].Y = (int)((control.Image.PixelHeight / control.TriggersCount) * i);
                }

                CroppedBitmap cpbNormal = new CroppedBitmap(
                    control.Image,
                    new Int32Rect(
                        control.viewBoxes[1].X,
                        control.viewBoxes[1].Y,
                        control.viewBoxes[1].Width,
                        control.viewBoxes[1].Height));

                control.normalBrush = new ImageBrush(cpbNormal);

                CroppedBitmap cpbOver = new CroppedBitmap(
                    control.Image,
                    new Int32Rect(
                        control.viewBoxes[3].X,
                        control.viewBoxes[3].Y,
                        control.viewBoxes[3].Width,
                        control.viewBoxes[3].Height));

                control.overBrush = new ImageBrush(cpbOver);

                CroppedBitmap cpbPressed = new CroppedBitmap(
                    control.Image,
                    new Int32Rect(
                        control.viewBoxes[4].X,
                        control.viewBoxes[4].Y,
                        control.viewBoxes[4].Width,
                        control.viewBoxes[4].Height));

                control.pressedBrush = new ImageBrush(cpbPressed);

                CroppedBitmap cpbDisabled = new CroppedBitmap(
                    control.Image,
                    new Int32Rect(
                        control.viewBoxes[2].X,
                        control.viewBoxes[2].Y,
                        control.viewBoxes[2].Width,
                        control.viewBoxes[2].Height));

                control.disabledBrush = new ImageBrush(cpbDisabled);

                CroppedBitmap cpbDefault = new CroppedBitmap(
                    control.Image,
                    new Int32Rect(
                        control.viewBoxes[0].X,
                        control.viewBoxes[0].Y,
                        control.viewBoxes[0].Width,
                        control.viewBoxes[0].Height));

                control.defaultBrush = new ImageBrush(cpbDefault);
            }
        }
        #endregion

        #region DrawingMargin
        public Thickness DrawingMargin
        {
            get { return (Thickness)GetValue(DrawingMarginProperty); }
            set { SetValue(DrawingMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawingMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawingMarginProperty =
            DependencyProperty.Register("DrawingMargin", typeof(Thickness), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDrawingMarginChanged)));

        private static void OnDrawingMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MultiTriggerImage control = sender as MultiTriggerImage;
            if (control != null)
                control.DrawingMarginChanged();
        }

        private void DrawingMarginChanged()
        {
            sourceRects = GenerateSourceRects();
            destRects = GenerateDestinationRects();
        }
        #endregion

        #region RenderPressed
        public bool RenderPressed
        {
            get { return (bool)GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }
        public static readonly DependencyProperty RenderPressedProperty =
            DependencyProperty.Register("RenderPressed", typeof(bool), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region RenderDefaulted
        public bool RenderDefaulted
        {
            get { return (bool)GetValue(RenderDefaultedProperty); }
            set { SetValue(RenderDefaultedProperty, value); }
        }
        public static readonly DependencyProperty RenderDefaultedProperty =
            DependencyProperty.Register("RenderDefaulted", typeof(bool), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region RenderOver
        public bool RenderOver
        {
            get { return (bool)GetValue(RenderOverProperty); }
            set { SetValue(RenderOverProperty, value); }
        }
        public static readonly DependencyProperty RenderOverProperty =
            DependencyProperty.Register("RenderOver", typeof(bool), typeof(MultiTriggerImage), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRenderOverChanged)));

        public static void OnRenderOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MultiTriggerImage control = sender as MultiTriggerImage;
            if (control != null)
            {
                if (!control.RenderPressed)
                    control.InvalidateVisual();
            }
        }
        #endregion
        #endregion

        #region Render
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.ActualWidth != 0 || this.ActualHeight != 0)
            {
                if (Image != null && TriggersCount > 0)
                {
                    ImageBrush img1 = new ImageBrush();
                    ImageBrush img2 = new ImageBrush();

                    DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
                    DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 200)));

                    fadeOut.BeginTime = new TimeSpan(0, 0, 0, 0, 120);

                    if (previousState == 0)
                        img1.ImageSource = defaultBrush.ImageSource;
                    else if (previousState == 2)
                        img1.ImageSource = disabledBrush.ImageSource;
                    else if (previousState == 3)
                        img1.ImageSource = overBrush.ImageSource;
                    else if (previousState == 4)
                        img1.ImageSource = pressedBrush.ImageSource;
                    else
                        img1.ImageSource = normalBrush.ImageSource;

                    if (RenderOver && !RenderPressed && IsEnabled)
                    {
                        img2 = overBrush;
                        previousState = 3;
                    }
                    else if (RenderPressed && IsEnabled)
                    {
                        img2 = pressedBrush;
                        previousState = 4;
                    }
                    else if (!IsEnabled)
                    {
                        img2 = disabledBrush;
                        previousState = 2;
                    }
                    else if (RenderDefaulted && IsEnabled)
                    {
                        img2 = defaultBrush;
                        previousState = 0;
                    }
                    else
                    {
                        img2 = normalBrush;
                        previousState = 1;
                    }

                    if (DrawingMargin == null
                        || (DrawingMargin.Top == 0
                        && DrawingMargin.Left == 0
                        && DrawingMargin.Right == 0
                        && DrawingMargin.Bottom == 0))
                    {
                        drawingContext.DrawRectangle(img1, null, new Rect(new Size(this.ActualWidth, this.ActualHeight)));
                        drawingContext.DrawRectangle(img2, null, new Rect(new Size(this.ActualWidth, this.ActualHeight)));

                        img1.BeginAnimation(ImageBrush.OpacityProperty, fadeOut);
                        img2.BeginAnimation(ImageBrush.OpacityProperty, fadeIn);
                    }
                    else
                    {

                        if (sourceRects == null)
                            sourceRects = GenerateSourceRects();
                        if (destRects == null)
                            destRects = GenerateDestinationRects();

                        for (int i = 0; i < sourceRects.Length; i++)
                        {
                            ImageBrush brush1 = new ImageBrush(img1.ImageSource);
                            brush1.ViewboxUnits = BrushMappingMode.Absolute;
                            brush1.Viewbox = sourceRects[i];
                            drawingContext.DrawRectangle(brush1, null, destRects[i]);

                            ImageBrush brush2 = new ImageBrush(img2.ImageSource);
                            brush2.ViewboxUnits = BrushMappingMode.Absolute;
                            brush2.Viewbox = sourceRects[i];
                            drawingContext.DrawRectangle(brush2, null, destRects[i]);

                            brush1.BeginAnimation(ImageBrush.OpacityProperty, fadeOut);
                            brush2.BeginAnimation(ImageBrush.OpacityProperty, fadeIn);
                        }
                    }
                }
            }
        }

        protected Rect[] GenerateSourceRects()
        {
            Size imageSize = new Size(normalBrush.ImageSource.Width, normalBrush.ImageSource.Height);
            double middleHeight = imageSize.Height - (DrawingMargin.Top + DrawingMargin.Bottom);
            double centerWidth = imageSize.Width - (DrawingMargin.Left + DrawingMargin.Right);
            double bottomTop = imageSize.Height - DrawingMargin.Bottom;
            double rightLeft = imageSize.Width - DrawingMargin.Right;

            Rect[] rects = new Rect[]
            {
                // TopLeft
                new Rect(0,0, DrawingMargin.Left, DrawingMargin.Top),
                // MiddleLeft
                new Rect(0, DrawingMargin.Top, DrawingMargin.Left, middleHeight),
                // BottomLeft
                new Rect(0, bottomTop, DrawingMargin.Left, DrawingMargin.Bottom),
                // TopCenter
                new Rect(DrawingMargin.Left, 0, centerWidth, DrawingMargin.Top),
                // MiddleCenter
                new Rect(DrawingMargin.Left, DrawingMargin.Top, centerWidth, middleHeight),
                // BottomCenter
                new Rect(DrawingMargin.Left, bottomTop, centerWidth, DrawingMargin.Bottom),
                // TopRight
                new Rect(rightLeft, 0, DrawingMargin.Right, DrawingMargin.Top),
                // MiddleRight
                new Rect(rightLeft, DrawingMargin.Top, DrawingMargin.Right, middleHeight),
                // BottomRight
                new Rect(rightLeft, bottomTop, DrawingMargin.Right, DrawingMargin.Bottom)
            };

            return rects;
        }

        protected Rect[] GenerateDestinationRects()
        {
            if (this.ActualHeight == 0 || this.ActualWidth == 0)
                return null;
            Size destSize = new Size(this.ActualWidth, this.ActualHeight);
            double middleHeight = destSize.Height - (DrawingMargin.Top + DrawingMargin.Bottom);
            double centerWidth = destSize.Width - (DrawingMargin.Left + DrawingMargin.Right);
            double bottomTop = destSize.Height - DrawingMargin.Bottom;
            double rightLeft = destSize.Width - DrawingMargin.Right;

            Rect[] rects = new Rect[]
            {
                // TopLeft
                new Rect(0,0, DrawingMargin.Left, DrawingMargin.Top),
                // MiddleLeft
                new Rect(0, DrawingMargin.Top, DrawingMargin.Left, middleHeight),
                // BottomLeft
                new Rect(0, bottomTop, DrawingMargin.Left, DrawingMargin.Bottom),
                // TopCenter
                new Rect(DrawingMargin.Left, 0, centerWidth, DrawingMargin.Top),
                // MiddleCenter
                new Rect(DrawingMargin.Left, DrawingMargin.Top, centerWidth, middleHeight),
                // BottomCenter
                new Rect(DrawingMargin.Left, bottomTop, centerWidth, DrawingMargin.Bottom),
                // TopRight
                new Rect(rightLeft, 0, DrawingMargin.Right, DrawingMargin.Top),
                // MiddleRight
                new Rect(rightLeft, DrawingMargin.Top, DrawingMargin.Right, middleHeight),
                // BottomRight
                new Rect(rightLeft, bottomTop, DrawingMargin.Right, DrawingMargin.Bottom)
            };

            return rects;
        }
        #endregion
    }
}
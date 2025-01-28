// **************************************************
//  WPF 2-D Chart class (WPF User Control)
//  Created by Li Gao  - July, 2007
//  http://ligao101.wordpress.com/                                              
//                           
// This chart supports panning/zomming and track-ball feature similar
//  to what you see on Google Finance.
//  
//  Note: 
//    1. This code is NOT optimized for performance. 
//    2. This chart is created for demo purposes only.
//    3. You can double-click on the chart to revert the zoom to its original state 
//        the same as the "Home" button
//
//                                                
// This code is created AS-IS. No warranty or support what so ever.
//***************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF2DChartCode.WPFControls.TimeSeriesDataLib;

namespace WPF2DChartCode.WpfControls
{
     public enum ChartLineTypeEnum
    {
        StepLine,
        StraightLine
    }

    /// <summary>
    /// Interaction logic for WpfChart.xaml
    /// </summary>

    public partial class WpfChart : System.Windows.Controls.UserControl
    {
        private TimeSeriesData dataSeries;
        private TimeSeriesData plotData;
        private MatrixTransform shapeTransform;
        private PathGeometry chartClip;
        private Point minPoint;
        private Point maxPoint;
        private Point optimalGridLineSpacing;
        private PanZoomCalculator panZoomCalculator;
        private AdornerCursor adorner;
        private AdornerLayer adornerLayer = null;
        private bool isInPanMode = false;

        public ChartLineTypeEnum chartLineType;

        public WpfChart()
        {
            InitializeComponent();

            LoadImages();
            chartLineType = ChartLineTypeEnum.StraightLine;

            dataSeries = new TimeSeriesData();
            dataSeries.PlotSizeChanged += new PlotSizeChangedDelegate(dataSeries_PlotSizeChanged);
            plotData = new TimeSeriesData();
            shapeTransform = new MatrixTransform();
            chartClip = new PathGeometry();
            optimalGridLineSpacing = new Point(100, 50);
            adorner = new AdornerCursor(ChartInteractiveCanvas, shapeTransform);
            adorner.CanvasSize = new Size(ChartInteractiveCanvas.ActualWidth, ChartInteractiveCanvas.ActualHeight);

            adorner.PanCursorImage = PanCommandImage.Source;

            panZoomCalculator = new PanZoomCalculator();

            ChartCanvas.SizeChanged += new SizeChangedEventHandler(ChartCanvas_SizeChanged);
            ChartCanvas.IsVisibleChanged += new DependencyPropertyChangedEventHandler(ChartCanvas_IsVisibleChanged);

            ChartToolBar.MouseEnter += new MouseEventHandler(ChartToolBar_MouseEnter);
            ChartToolBar.MouseLeave += new MouseEventHandler(ChartToolBar_MouseLeave);
            ChartToolBar.MouseMove += new MouseEventHandler(ChartToolBar_MouseMove);
            ChartToolBarPanel.MouseMove += new MouseEventHandler(ChartToolBar_MouseMove);
            ChartToolBarPanel.MouseDown += new MouseButtonEventHandler(ChartToolBarPanel_MouseDown);
            ChartToolBarPanel.MouseUp += new MouseButtonEventHandler(ChartToolBarPanel_MouseUp);
            ChartToolBar.PreviewMouseMove += new MouseEventHandler(ChartToolBar_MouseMove);
            ZoomResetCommand.Click += new RoutedEventHandler(ZoomResetCommand_Click);
            ZoomCommand.Click += new RoutedEventHandler(ZoomCommand_Click);
            PanCommand.Click += new RoutedEventHandler(PanCommand_Click);

            AttachEventsToCanvas(ChartInteractiveCanvas);

            ResizeChart();

        }

        public void ShowNoDataLabel(bool ShowLabel)
        {
            if (ShowLabel)
                NoDataLabel.Opacity = 0.7;
            else
                NoDataLabel.Opacity = 0.0;
        }       

        private void LoadImages()
        {
            string bmpHomeName = "WPF2DChartCode.WpfControls.Resources.Home.bmp";
            string bmpZoomName = "WPF2DChartCode.WpfControls.Resources.ZoomIn.bmp";
            string bmpPanName = "WPF2DChartCode.WpfControls.Resources.Breakpoint.bmp";

            System.IO.Stream strm = null;
            try
            {
                strm = this.GetType().Assembly.GetManifestResourceStream(bmpHomeName);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(strm);
                bitmap.MakeTransparent();
                IntPtr h_bm = bitmap.GetHbitmap();
                BitmapSource bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ZoomResetCommandImage.Source = bms;

                strm = this.GetType().Assembly.GetManifestResourceStream(bmpZoomName);
                bitmap = new System.Drawing.Bitmap(strm);
                bitmap.MakeTransparent();
                h_bm = bitmap.GetHbitmap();
                bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ZoomCommandImage.Source = bms;

                strm = this.GetType().Assembly.GetManifestResourceStream(bmpPanName);
                bitmap = new System.Drawing.Bitmap(strm);
                bitmap.MakeTransparent();
                h_bm = bitmap.GetHbitmap();
                bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                PanCommandImage.Source = bms;

            }
            catch (Exception)
            {
                //Do nothing               
            }

        }

        public void SetTitle(string TitleString)
        {
            titleBox.Text = TitleString;
        }

        protected void AttachEventsToCanvas(UIElement eventCanvas)
        {
            eventCanvas.LostMouseCapture += new MouseEventHandler(eventCanvas_LostMouseCapture);
            eventCanvas.MouseMove += new MouseEventHandler(eventCanvas_MouseMove);
            eventCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(eventCanvas_MouseDown);
            eventCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(eventCanvas_MouseUp);
            eventCanvas.MouseEnter += new MouseEventHandler(eventCanvas_MouseEnter);
            eventCanvas.MouseLeave += new MouseEventHandler(eventCanvas_MouseLeave);
        }

        private void dataSeries_PlotSizeChanged()
        {
            DrawChart();
        }

        #region Canvas Mouse Event Handlers

        protected void ResizePlot()
        {
            SetChartTransform(ChartCanvas.ActualWidth, ChartCanvas.ActualHeight);

            // Still need to redraw the grid lines
            InvalidateMeasure();
        }

        private void eventCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.None;
        }


        private void eventCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            adorner.Visibility = Visibility.Visible;
            this.Cursor = Cursors.None;

            Point mousePos = e.GetPosition(ChartInteractiveCanvas);

            TimeSeriesDataPoint dataPoint;
            if (chartLineType == ChartLineTypeEnum.StepLine)
                dataPoint = GetPrevDataPointFromMouseCoordinates(mousePos);
            else
                dataPoint = GetInterpolatedDataPointFromMouseCoordinates(mousePos);

            if (dataPoint.Value != double.NaN)
            {
                if ((e.LeftButton != MouseButtonState.Pressed) &&
                    e.RightButton != MouseButtonState.Pressed)
                {
                    adorner.HasLockPoint = true;
                    Point lockPoint = new Point(mousePos.X, dataPoint.Value);
                    lockPoint = shapeTransform.Transform(lockPoint);

                    // Re-calc the lock point;
                    lockPoint.X = mousePos.X;
                    lockPoint.Y = ChartInteractiveCanvas.ActualHeight - lockPoint.Y;

                    adorner.LockPoint = lockPoint;
                }
                else
                    adorner.HasLockPoint = false;
            }
            else
            {
                adorner.HasLockPoint = false;
            }

            adorner.MousePoint = mousePos;
            if (adorner.IsDrawingPanningVisual)
            {
                PanChart();
            }

            ShowCurrentCoValue(dataPoint);
        }

        private void ChartToolBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (adorner != null)
            {
                adorner.Visibility = Visibility.Hidden;
                TickerLabel.Inlines.Clear();
            }

            this.Cursor = Cursors.Arrow;

            ChartToolBar.Opacity = 0.9;
            e.Handled = true;
        }

        private void ChartToolBar_MouseLeave(object sender, MouseEventArgs e)
        {
            ChartToolBar.Opacity = 0.15;
        }

        private void ChartToolBar_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            e.Handled = true;
        }

        private void ChartToolBarPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ChartToolBarPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void eventCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            adorner.Visibility = Visibility.Visible;
            adorner.IsDrawingZoomVisual = false;
            adorner.IsDrawingPanningVisual = false;
        }

        private void eventCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            adorner.Visibility = Visibility.Hidden;
            TickerLabel.Inlines.Clear();

            this.Cursor = Cursors.Arrow;
            adorner.IsDrawingZoomVisual = false;
            adorner.IsDrawingPanningVisual = false;
        }

        private void eventCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                Point mousePos = e.GetPosition(ChartInteractiveCanvas);
                adorner.MouseDownPoint = mousePos;

                adorner.IsDrawingPanningVisual = false;
                adorner.IsDrawingZoomVisual = false;

                adorner.IsDrawingPanningVisual = isInPanMode;
                adorner.IsDrawingZoomVisual = !isInPanMode;
            }
            else
            {
                ResetZoom();
            }
        }

        private void eventCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(ChartInteractiveCanvas);
            adorner.MouseUpPoint = mousePos;

            bool isZoom = adorner.IsDrawingZoomVisual;
            adorner.IsDrawingZoomVisual = false;
            adorner.IsDrawingPanningVisual = false;

            if (isZoom)
            {
                ZoomIn();
            }
        }

        private void ZoomResetCommand_Click(object sender, RoutedEventArgs e)
        {
            ResetZoom();
        }

        private void ZoomCommand_Click(object sender, RoutedEventArgs e)
        {
            isInPanMode = false;
            adorner.IsInPanMode = isInPanMode;
            UpdateChartToolbarVisuals();

        }

        private void PanCommand_Click(object sender, RoutedEventArgs e)
        {
            isInPanMode = true;
            adorner.IsInPanMode = isInPanMode;
            UpdateChartToolbarVisuals();
        }

        private void UpdateChartToolbarVisuals()
        {
            //Do nothing
        }

        private void ShowCurrentCoValue(TimeSeriesDataPoint dataPoint)
        {
            if (adorner == null)
                return;

            if (adorner.Visibility == Visibility.Visible)
            {
                if (dataPoint.Value != double.NaN)
                {
                    TickerLabel.Text = dataPoint.TimeStamp.ToLongTimeString() + "  " + dataPoint.Value.ToString("#0.00");
                }
                else
                {
                    TickerLabel.Text = dataPoint.TimeStamp.ToLongTimeString() + " " + @"N/A";
                }

            }
        }

        private TimeSeriesDataPoint GetPrevDataPointFromMouseCoordinates(Point mousePos)
        {
            TimeSeriesDataPoint dataPoint = new TimeSeriesDataPoint();
            dataPoint.TimeStamp = DateTime.Now;
            dataPoint.Value = double.NaN;

            Point pos = mousePos;
            pos.Y = ChartInteractiveCanvas.ActualHeight - pos.Y;

            GeneralTransform inverse = shapeTransform.Inverse;
            if (inverse != null)
            {
                pos = inverse.Transform(pos);
                DateTime xVal = dataSeries.FromPointSeconds(pos.X);
                double yVal = dataSeries.GetClosedPrevValue(xVal);

                dataPoint.TimeStamp = xVal;
                dataPoint.Value = yVal;

            }

            return dataPoint;
        }

        private TimeSeriesDataPoint GetInterpolatedDataPointFromMouseCoordinates(Point mousePos)
        {
            TimeSeriesDataPoint dataPoint = new TimeSeriesDataPoint();
            dataPoint.TimeStamp = DateTime.Now;
            dataPoint.Value = double.NaN;

            Point pos = mousePos;
            pos.Y = ChartInteractiveCanvas.ActualHeight - pos.Y;

            GeneralTransform inverse = shapeTransform.Inverse;
            if (inverse != null)
            {
                pos = inverse.Transform(pos);
                DateTime xVal = dataSeries.FromPointSeconds(pos.X);
                double yVal = dataSeries.GetClosedInterpolatedValue(xVal);

                dataPoint.TimeStamp = xVal;
                dataPoint.Value = yVal;

            }

            return dataPoint;
        }

        private void PanChart()
        {

            double offsetX = adorner.MouseDownPoint.X - adorner.MousePoint.X;
            double offsetY = adorner.MousePoint.Y - adorner.MouseDownPoint.Y;

            // we only pan either horizontally or vertically
            // but not both at the same time
            if (Math.Abs(offsetX) > Math.Abs(offsetY))
                offsetY = 0;

            if (Math.Abs(offsetY) > Math.Abs(offsetX))
                offsetX = 0;

            offsetX = offsetX / ChartInteractiveCanvas.ActualWidth;
            offsetY = offsetY / ChartInteractiveCanvas.ActualHeight;

            double speedFactor = 0.1;

            // calculate the accumulative offsets
            offsetX = panZoomCalculator.Pan.X + offsetX * speedFactor / panZoomCalculator.Zoom.X;
            offsetY = panZoomCalculator.Pan.Y + offsetY * speedFactor / panZoomCalculator.Zoom.Y;

            panZoomCalculator.Pan = new Point(offsetX, offsetY);
            ResizePlot();
        }

        private void ZoomIn()
        {

            Rect rect = new Rect();
            rect.X = Math.Min(adorner.MouseDownPoint.X, adorner.MousePoint.X);
            rect.Y = Math.Min(adorner.MouseDownPoint.Y, adorner.MousePoint.Y);
            rect.Width = Math.Abs(adorner.MouseDownPoint.X - adorner.MousePoint.X);
            rect.Height = Math.Abs(adorner.MouseDownPoint.Y - adorner.MousePoint.Y);

            double offsetX = rect.X;
            offsetX = offsetX / ChartInteractiveCanvas.ActualWidth;

            double offsetY = rect.Y + rect.Height;
            offsetY = (ChartInteractiveCanvas.ActualHeight - offsetY) / ChartInteractiveCanvas.ActualHeight;

            double scaleX = rect.Width;
            scaleX = ChartInteractiveCanvas.ActualWidth / scaleX;
            scaleX = panZoomCalculator.Zoom.X * scaleX;

            double scaleY = rect.Height;
            scaleY = ChartInteractiveCanvas.ActualHeight / scaleY;
            scaleY = panZoomCalculator.Zoom.Y * scaleY;

            // calculate the accumulative offsets
            offsetX = panZoomCalculator.Pan.X + offsetX / panZoomCalculator.Zoom.X;
            offsetY = panZoomCalculator.Pan.Y + offsetY / panZoomCalculator.Zoom.Y;

            // limiting the zoom capability to 50000.00 times
            // this should be enough for most of charts!
            if (scaleX <= 50000.00 && scaleY <= 50000.00)
            {
                panZoomCalculator.Pan = new Point(offsetX, offsetY);
                panZoomCalculator.Zoom = new Point(scaleX, scaleY);
                ResizePlot();
            }

        }

        private void ResetZoom()
        {
            panZoomCalculator.Pan = new Point(0, 0);
            panZoomCalculator.Zoom = new Point(1, 1);

            isInPanMode = false;
            adorner.IsInPanMode = isInPanMode;

            UpdateChartToolbarVisuals();

            ResizePlot();
        }

        #endregion

        void ChartCanvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && adornerLayer == null)
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(ChartInteractiveCanvas);
                adornerLayer.Add(adorner);
                adorner.Visibility = this.IsMouseOver ? Visibility.Visible : Visibility.Hidden;
            }
            else if (adornerLayer != null)
            {
                adornerLayer.Remove(adorner);
                adornerLayer = null;
            }
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (adorner != null)
                adorner.CanvasSize = new Size(ChartInteractiveCanvas.ActualWidth, ChartInteractiveCanvas.ActualHeight);

            ResizeChart();
        }

        private void ResizeChart()
        {
            chartClip.Clear();
            chartClip.AddGeometry(new RectangleGeometry(
                new Rect(0, 0, ChartCanvas.ActualWidth, ChartCanvas.ActualHeight)));

            InvalidateMeasure();
            SetChartTransform(ChartCanvas.ActualWidth, ChartCanvas.ActualHeight);
        }

        public TimeSeriesData DataSeries
        {
            get { return dataSeries; }
            set { dataSeries = value; }
        }

        public void DrawChart()
        {

            InvalidateMeasure();

            InitPlotData();
            SetChartTransform(ChartCanvas.ActualWidth, ChartCanvas.ActualHeight);

            // ChartCanvas.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
            ChartCanvas.Children.Clear();

            if (plotData.GetData().Count > 0)
            {
                ShowNoDataLabel(false);
                Path path = new Path();

                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Transform = shapeTransform;
                pathGeometry.AddGeometry(plotData.GetPathGeometry());

                path.Stroke = new SolidColorBrush(System.Windows.Media.Colors.DarkBlue);
                path.StrokeThickness = 1.0;
                //path.Fill = new SolidColorBrush( System.Windows.Media.Color.FromRgb(196,230,230));
                path.Fill = new SolidColorBrush(System.Windows.Media.Colors.SkyBlue);
                path.Opacity = 0.8;

                path.Data = pathGeometry;
                path.Clip = chartClip;

                ChartCanvas.Children.Add(path);

                DrawGridlinesAndLabels();
            }
            else
            {
                ShowNoDataLabel(true);
            }

        }

        public void SetStaticLabels()
        {
            //titleBox.Text = "Li Gao's Sample WPF-based Step Chart";
            //titleBox.FontSize = 14;

            yAxisLabel.Inlines.Clear();
            yAxisLabel.Inlines.Add(new Bold(new Run("BlackBerry ")));
            yAxisLabel.Inlines.Add(new Run("Latency ("));
            yAxisLabel.Inlines.Add(new Bold(new Italic(new Run("Seconds"))));
            yAxisLabel.Inlines.Add(new Run(")"));

            xAxisLabel.Text = "Time";
        }

        public void SetStaticYLabel(string YLabel)
        {
            yAxisLabel.Text = YLabel;
            xAxisLabel.Text = "Time";
        }

        private void SetChartTransform(double width, double height)
        {

            Rect plotArea = plotData.GetPlotRectangle();

            minPoint = plotArea.Location;
            minPoint.Offset(plotArea.Width * panZoomCalculator.Pan.X, plotArea.Height * panZoomCalculator.Pan.Y);

            maxPoint = minPoint;
            maxPoint.Offset(plotArea.Width / panZoomCalculator.Zoom.X, plotArea.Height / panZoomCalculator.Zoom.Y);

            Point plotScale = new Point();
            plotScale.X = (width / plotArea.Width) * panZoomCalculator.Zoom.X;
            plotScale.Y = (height / plotArea.Height) * panZoomCalculator.Zoom.Y;

            Matrix shapeMatrix = Matrix.Identity;
            shapeMatrix.Translate(-minPoint.X, -minPoint.Y);
            shapeMatrix.Scale(plotScale.X, plotScale.Y);
            shapeTransform.Matrix = shapeMatrix;

        }

        private void InitPlotData()
        {
            plotData.Clear();

            bool hasPreviousValue = false;
            double previousValue = 0.0;

            List<TimeSeriesDataPoint> plotPoints = new List<TimeSeriesDataPoint>();

            foreach (TimeSeriesDataPoint pt in dataSeries.GetData().Values)
            {
                TimeSeriesDataPoint tempPoint = pt.Clone();

                if (chartLineType == ChartLineTypeEnum.StepLine)
                {
                    if (hasPreviousValue)
                    {
                        if (previousValue != tempPoint.Value)
                        {
                            tempPoint.hasStepFromValue = true;
                            tempPoint.stepFromValue = previousValue;
                        }
                    }
                }

                plotPoints.Add(tempPoint);

                hasPreviousValue = true;
                previousValue = pt.Value;

            }

            plotData.AddPointsRange(plotPoints.ToArray());

            if (dataSeries.IsCustomTimeRange)
            {
                DateTime startTime = dataSeries.CustomStartTime;
                DateTime endTime = dataSeries.CustomEndTime;

                plotData.SetTimeRange(startTime, endTime);
            }

            plotData.GeneratePaths();

        }

        /// <summary>
        /// Add grid lines here
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            DrawGridlinesAndLabels();
            return base.MeasureOverride(constraint);
        }

        private void DrawGridlinesAndLabels()
        {
            DrawGridlinesAndLabels(TextCanvas, new Size(TextCanvas.ActualWidth, TextCanvas.ActualHeight), minPoint, maxPoint);
        }

        /// <summary>
        /// Draw all the gridlines and labels for the gridlines
        /// </summary>
        protected void DrawGridlinesAndLabels(Canvas textCanvas, Size size, Point minXY, Point maxXY)
        {
            // Clear the text canvas
            textCanvas.Children.Clear();

            // Create brush for writing text
            Brush axisBrush = new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue);
            Brush axisScaleBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);
            Brush axisTickBrush = new SolidColorBrush(System.Windows.Media.Colors.Black);

            // Need to pick appropriate scale increment.
            // Go for a 2Exx, 5Exx, or 1Exx type scale
            double scaleX = 0.0;
            double scaleY = 0.0;

            // Work out all the limits

            if (maxXY.X != minXY.X)
                scaleX = (double)size.Width / (double)(maxXY.X - minXY.X);
            if (maxXY.Y != minXY.Y)
                scaleY = (double)size.Height / (double)(maxXY.Y - minXY.Y);
            double optimalSpacingX = optimalGridLineSpacing.X / scaleX;

            double spacingX = ChartUtilities.ClosestValueInListTimesBaseToInteger(optimalSpacingX, new double[] { 1, 3, 6 }, 12.0);

            if (spacingX < 2.0)
                spacingX = ChartUtilities.Closest_1_2_5_Pow10(optimalSpacingX);

            double optimalSpacingY = optimalGridLineSpacing.Y / scaleY;
            double spacingY = ChartUtilities.Closest_1_2_5_Pow10(optimalSpacingY);

            int startXmult = (int)Math.Ceiling(minXY.X / spacingX);
            int endXmult = (int)Math.Floor(maxXY.X / spacingX);
            int startYmult = (int)Math.Ceiling(minXY.Y / spacingY);
            int endYmult = (int)Math.Floor(maxXY.Y / spacingY);

            double maxXLabelHeight = 0;

            PathFigure pathFigure = new PathFigure();
            PathFigure tickPathFigure = new PathFigure();

            // Draw all the vertical gridlines
            double prevPos = double.MinValue;

            for (int lineNo = startXmult; lineNo <= endXmult; ++lineNo)
            {
                double xValue = lineNo * spacingX;
                double xPos = (xValue - minXY.X) * scaleX;

                TextBlock text = new TextBlock();

                if (spacingX < 60)
                {
                    text.Text = plotData.FromPointSeconds(xValue).ToLongTimeString();
                }
                else
                    text.Text = plotData.FromPointSeconds(xValue).ToShortTimeString();

                text.Foreground = axisScaleBrush;
                text.Measure(size);

                // check overlapping Time Axis labels
                double xPos1 = xPos - text.DesiredSize.Width * .5;

                if (xPos1 > prevPos + 5)
                {
                    Point startPoint = new Point(xPos, size.Height - 4);
                    Point endPoint = new Point(xPos, size.Height);

                    // create X Axis ticks
                    tickPathFigure.Segments.Add(new LineSegment(startPoint, false));
                    tickPathFigure.Segments.Add(new LineSegment(endPoint, true));

                    text.SetValue(Canvas.LeftProperty, xPos1);
                    text.SetValue(Canvas.TopProperty, size.Height + 5);
                    textCanvas.Children.Add(text);
                    maxXLabelHeight = Math.Max(maxXLabelHeight, text.DesiredSize.Height);

                    prevPos = xPos1 + text.DesiredSize.Width;
                }
            }

            xGridlineLabels.Height = maxXLabelHeight + 7;

            // Set string format for vertical text
            double maxYLabelHeight = 0;

            // Draw all the horizontal gridlines

            for (int lineNo = startYmult; lineNo <= endYmult; ++lineNo)
            {
                double yValue = lineNo * spacingY;
                double yPos = (-yValue + minXY.Y) * scaleY + size.Height;

                Point startPoint = new Point(0, yPos);
                Point endPoint = new Point(size.Width, yPos);

                pathFigure.Segments.Add(new LineSegment(startPoint, false));
                pathFigure.Segments.Add(new LineSegment(endPoint, true));

                Point startPoint1 = new Point(-2, yPos);
                Point endPoint1 = new Point(2, yPos);

                // create Y Axis ticks
                tickPathFigure.Segments.Add(new LineSegment(startPoint1, false));
                tickPathFigure.Segments.Add(new LineSegment(endPoint1, true));

                TextBlock text = new TextBlock();
                text.Text = yValue.ToString();
                //text.LayoutTransform = new RotateTransform(-90);
                text.Measure(size);

                text.SetValue(Canvas.LeftProperty, -text.DesiredSize.Width - 5);
                text.SetValue(Canvas.TopProperty, yPos - text.DesiredSize.Height * .5);
                textCanvas.Children.Add(text);
                maxYLabelHeight = Math.Max(maxYLabelHeight, text.DesiredSize.Width);
            }
            yGridLineLabels.Height = maxYLabelHeight + 2;

            Path path = new Path();
            path.Stroke = axisBrush;
            path.StrokeThickness = 0.5;
            PathGeometry pathGeometry = new PathGeometry(new PathFigure[] { pathFigure });

            pathGeometry.Transform = (Transform)textCanvas.RenderTransform.Inverse;
            path.Data = pathGeometry;

            textCanvas.Children.Add(path);

            // adding ticks
            Path tickPath = new Path();
            tickPath.Stroke = axisTickBrush;
            tickPath.StrokeThickness = 1;
            PathGeometry tickPathGeometry = new PathGeometry(new PathFigure[] { tickPathFigure });

            tickPathGeometry.Transform = (Transform)textCanvas.RenderTransform.Inverse;
            tickPath.Data = tickPathGeometry;

            ChartTicksCanvas.Children.Clear();
            ChartTicksCanvas.Children.Add(tickPath);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;

namespace Ricciolo.Controls
{
    public class CalendarGrid : Grid
    {

        private static DependencyProperty MonthProperty = DependencyProperty.Register("Month", typeof(Int32), typeof(CalendarGrid), new PropertyMetadata(OnMonthChanged));

        public CalendarGrid()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());

            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition());
        }

        private static void OnMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CalendarGrid)d).Recreate();
        }

        public Int32 Month
        {
            get { return (Int32)this.GetValue(MonthProperty); }
            set { this.SetValue(MonthProperty, value); }
        }

        private void Recreate()
        {
            this.Children.Clear();
            DateTimeFormatInfo format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            int days = DateTime.DaysInMonth(DateTime.Today.Year, this.Month);
            int day = 0;
            int startx = (int)new DateTime(DateTime.Today.Year, this.Month, 1).DayOfWeek - (int)format.FirstDayOfWeek;
            if (startx < 0) startx = 6;
            for (int y = 0; y < this.RowDefinitions.Count; y++)
            {
                for (int x = 0; x < this.ColumnDefinitions.Count; x++)
                {
                    TextBlock tb;
                    // Days
                    if (y == 0)
                    {
                        tb = new TextBlock();
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.FontWeight = FontWeights.Bold;
                        int d = (int)format.FirstDayOfWeek + x;
                        if (d > 6) d = 0;
                        tb.Text = format.GetAbbreviatedDayName((DayOfWeek)Enum.ToObject(typeof(DayOfWeek), d));
                    }
                    else
                    {
                        // Skip if I didn't reach first day
                        if (y == 1 && x < startx) continue;
                        day++;
                        if (day > days) return;

                        // Create text with number
                        tb = new TextBlock();
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.Text = day.ToString();
                    }
                    SetColumn(tb, x);
                    SetRow(tb, y);
                    this.Children.Add(tb);
                }
            }
        }
    }
}

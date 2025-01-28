using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;

namespace System.Windows.Controls
{
    public partial class Book : ItemsControl
    {
        public Book() 
        {
            Width = 300;
            Height = 200;
            InitializeComponent();
        }

        static Book() 
        {
        }

        private PageStatus _status = PageStatus.None;
        private DataTemplate defaultDataTemplate;

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            BookPage bp0 = GetTemplateChild("sheet0") as BookPage;
            BookPage bp1 = GetTemplateChild("sheet1") as BookPage;
            defaultDataTemplate = (DataTemplate) Resources["defaultDataTemplate"];
            Read<PageStatus> GetStatus = delegate() { return _status; };
            Action<PageStatus> SetStatus = delegate(PageStatus ps) { _status = ps; };
            bp0.GetStatus += GetStatus;
            bp0.SetStatus += SetStatus;
            bp1.GetStatus += GetStatus;
            bp1.SetStatus += SetStatus;

            RefreshSheetsContent();
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (CheckCurrentSheetIndex())
            {
                CurrentSheetIndex = GetItemsCount() / 2;
            }
            else
                RefreshSheetsContent();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (CheckCurrentSheetIndex())
            {
                CurrentSheetIndex = GetItemsCount() / 2;
            }
            else
                RefreshSheetsContent();
        }

        internal object GetPage(int index)
        {
            if ((index >= 0) && (index < Items.Count))
                return Items[index];
            
            Canvas c = new Canvas();
            //c.Background = Brushes.White;
            
            return c;
        }

        private int _currentSheetIndex = 0;

        public int CurrentSheetIndex
        {
            get { return _currentSheetIndex; }
            set 
            {
                if (_status != PageStatus.None) return;

                if (_currentSheetIndex != value)
                {
                    if ((value >= 0) && (value <= GetItemsCount() / 2))
                    {
                        _currentSheetIndex = value;
                        RefreshSheetsContent();
                    }
                    else
                        throw new Exception("Index out of bounds");
                }
            }
        }

        protected virtual bool CheckCurrentSheetIndex()
        {
            return CurrentSheetIndex > (GetItemsCount() / 2);
        }

        //public void GoToNextPage(bool withAnimation) 
        //{
        //    BookPage bp1 = GetTemplateChild("sheet1") as BookPage;
        //    bp1.origin = CornerOrigin.BottomRight;
        //    bp1.TurnPage();
        //}
        
        //public void GoToPreviousPage(bool withAnimation) 
        //{
        //    BookPage bp0 = GetTemplateChild("sheet0") as BookPage;
        //    bp0.origin = CornerOrigin.BottomLeft;
        //    bp0.TurnPage();
        //}
        
        public int GetItemsCount() 
        {
            if (ItemsSource != null) {
                if (ItemsSource is ICollection)
                    return (ItemsSource as ICollection).Count;
                int count = 0;
                foreach (object o in ItemsSource) count++;
                return count;
            }
            return Items.Count;
        }

        private void RefreshSheetsContent() 
        {
            BookPage bp0 = GetTemplateChild("sheet0") as BookPage;
            if (bp0 == null)
                return;

            BookPage bp1 = GetTemplateChild("sheet1") as BookPage;

            ContentPresenter sheet0Page0Content = bp0.FindName("page0") as ContentPresenter;
            ContentPresenter sheet0Page1Content = bp0.FindName("page1") as ContentPresenter;
            ContentPresenter sheet0Page2Content = bp0.FindName("page2") as ContentPresenter;

            ContentPresenter sheet1Page0Content = bp1.FindName("page0") as ContentPresenter;
            ContentPresenter sheet1Page1Content = bp1.FindName("page1") as ContentPresenter;
            ContentPresenter sheet1Page2Content = bp1.FindName("page2") as ContentPresenter;

            Visibility bp0Visibility = Visibility.Visible;
            Visibility bp1Visibility = Visibility.Visible;
            
            bp1.IsTopRightCornerEnabled = true;
            bp1.IsBottomRightCornerEnabled = true;
            
            Visibility sheet0Page0ContentVisibility = Visibility.Visible;
            Visibility sheet0Page1ContentVisibility = Visibility.Visible;
            Visibility sheet0Page2ContentVisibility = Visibility.Visible;
            Visibility sheet1Page0ContentVisibility = Visibility.Visible;
            Visibility sheet1Page1ContentVisibility = Visibility.Visible;
            Visibility sheet1Page2ContentVisibility = Visibility.Visible;

            DataTemplate dt = ItemTemplate;
            if (dt == null)
                dt = defaultDataTemplate;

            sheet0Page0Content.ContentTemplate = dt;
            sheet0Page1Content.ContentTemplate = dt;
            sheet0Page2Content.ContentTemplate = dt;
            sheet1Page0Content.ContentTemplate = dt;
            sheet1Page1Content.ContentTemplate = dt;
            sheet1Page2Content.ContentTemplate = dt;
                
            sheet0Page2ContentVisibility = _currentSheetIndex == 1 ? Visibility.Hidden : Visibility.Visible;
            int count = GetItemsCount();
            int sheetCount = count / 2;
            bool isOdd = (count % 2) == 1;

            if (_currentSheetIndex == sheetCount)
            {
                if (isOdd)
                {
                    bp1.IsTopRightCornerEnabled = false;
                    bp1.IsBottomRightCornerEnabled = false;
                }
                else
                    bp1Visibility = Visibility.Hidden;
            }
            
            if (_currentSheetIndex == sheetCount - 1) 
            {
                if (!isOdd)
                    sheet1Page2ContentVisibility = Visibility.Hidden;
            }

            if (_currentSheetIndex == 0)
            {
                sheet0Page0Content.Content = null;
                sheet0Page1Content.Content = null;
                sheet0Page2Content.Content = null;
                bp0.IsEnabled = false;
                bp0Visibility = Visibility.Hidden;
            }
            else
            {
                sheet0Page0Content.Content = GetPage(2 * (CurrentSheetIndex - 1) + 1);
                sheet0Page1Content.Content = GetPage(2 * (CurrentSheetIndex - 1));
                sheet0Page2Content.Content = GetPage(2 * (CurrentSheetIndex - 1) - 1);
                bp0.IsEnabled = true;
            }

            sheet1Page0Content.Content = GetPage(2 * CurrentSheetIndex);
            sheet1Page1Content.Content = GetPage(2 * CurrentSheetIndex + 1);
            sheet1Page2Content.Content = GetPage(2 * CurrentSheetIndex + 2);

            bp0.Visibility = bp0Visibility;
            bp1.Visibility = bp1Visibility;
            
            sheet0Page0Content.Visibility = sheet0Page0ContentVisibility;
            sheet0Page1Content.Visibility = sheet0Page1ContentVisibility;
            sheet0Page2Content.Visibility = sheet0Page2ContentVisibility;
            sheet1Page0Content.Visibility = sheet1Page0ContentVisibility;
            sheet1Page1Content.Visibility = sheet1Page1ContentVisibility;
            sheet1Page2Content.Visibility = sheet1Page2ContentVisibility;

        }

        private void OnLeftMouseDown(object sender, MouseButtonEventArgs args) 
        {
            BookPage bp0 = GetTemplateChild("sheet0") as BookPage;
            BookPage bp1 = GetTemplateChild("sheet1") as BookPage;
            Canvas.SetZIndex((bp0 as BookPage), 1);
            Canvas.SetZIndex((bp1 as BookPage), 0);
        }
        private void OnRightMouseDown(object sender, MouseButtonEventArgs args) 
        {
            BookPage bp0 = GetTemplateChild("sheet0") as BookPage;
            BookPage bp1 = GetTemplateChild("sheet1") as BookPage;
            Canvas.SetZIndex((bp0 as BookPage), 0);
            Canvas.SetZIndex((bp1 as BookPage), 1);
        }
        private void OnLeftPageTurned(object sender, RoutedEventArgs args) 
        {
            CurrentSheetIndex--;
        }
        private void OnRightPageTurned(object sender, RoutedEventArgs args) 
        {
            CurrentSheetIndex++;
        }
        private void OnLeftMouseDoubleClick(object sender, MouseButtonEventArgs args) 
        {
            
        }
        private void OnRightMouseDoubleClick(object sender, MouseButtonEventArgs args) 
        {
            
        }
    }
}

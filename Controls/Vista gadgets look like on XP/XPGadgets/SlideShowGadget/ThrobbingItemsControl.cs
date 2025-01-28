namespace SlideShowGadget
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows.Data;

    public class ThrobbingItemsControl : ItemsControl
    {
        private Timer timer;
        private int currentPageIndex = 0;
        private delegate void DispatcherCallback();
	
        public int RefreshRate
        {
            get { return (int)base.GetValue(RefreshRateProperty); }
            set 
            { 
                base.SetValue(RefreshRateProperty, value);
                this.reInitialize();
            }
        }

        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
        }
	
        public static readonly DependencyProperty RefreshRateProperty = DependencyProperty.Register("RefreshRate", typeof(int), typeof(ThrobbingItemsControl), new PropertyMetadata(5000));

        public int ItemsPerRefresh
        {
            get { return (int)base.GetValue(ItemsPerRefreshProperty); }
            set
            { 
                base.SetValue(ItemsPerRefreshProperty, value);
                this.reInitialize();
            }
        }  

        public static readonly DependencyProperty ItemsPerRefreshProperty = DependencyProperty.Register("ItemsPerRefresh", typeof(int), typeof(ThrobbingItemsControl), new PropertyMetadata(4));

        public bool IsThrobbing
        {
			get { return (bool)base.GetValue(IsThrobbingProperty); }
            set 
            {
				base.SetValue(IsThrobbingProperty, value);
            }
        }

		public static readonly DependencyProperty IsThrobbingProperty = DependencyProperty.Register("IsThrobbing", typeof(bool), typeof(ThrobbingItemsControl));

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

			this.reInitialize();
        }

        private void onTimerTick(object o)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (DispatcherCallback)this.onTimerTickCallBack);
        }

        private void onTimerTickCallBack()
        {
            if (this.IsThrobbing)
            {
                CollectionView view = CollectionViewSource.GetDefaultView(this.Items) as CollectionView;
                view.Filter = null;

                if ((this.currentPageIndex + this.ItemsPerRefresh) >= this.Items.Count)
                {
                    this.currentPageIndex = 0;
                }
                else
                {
                    this.currentPageIndex = this.currentPageIndex + this.ItemsPerRefresh;
                }

                view.Filter = new Predicate<object>(this.contains);
            }
        }

        private void reInitialize()
        {
            timer = null;
            this.currentPageIndex = 0;
			CollectionView view = CollectionViewSource.GetDefaultView(this.Items) as CollectionView;

			view.Filter = new Predicate<object>(this.contains);

			if (this.Items.Count != 0)
			{
				timer = new Timer(new TimerCallback(this.onTimerTick), null, 0, this.RefreshRate);
			}
        }

        private bool contains(object o)
        {
            int indexOfItem = this.Items.IndexOf(o);

            if ((indexOfItem >= this.currentPageIndex) && (indexOfItem < (this.currentPageIndex + this.ItemsPerRefresh)))
            {
                return true;
            }

            return false;
        }
    }
}
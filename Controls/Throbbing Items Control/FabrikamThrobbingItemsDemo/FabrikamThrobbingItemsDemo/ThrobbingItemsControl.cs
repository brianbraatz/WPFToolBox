namespace FabrikamThrobbingItemsDemo
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
        private int currentIndex = 0;
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
				base.SetValue(IsThrobbingProperty, true);
                this.reInitialize();
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
			CollectionView view = CollectionViewSource.GetDefaultView(this.Items) as CollectionView;
			view.Filter = null;

            if ((this.currentIndex + this.ItemsPerRefresh) >= this.Items.Count)
            {
                this.currentIndex = 0;
            }
            else
            {
                this.currentIndex = this.currentIndex + this.ItemsPerRefresh;
            }

			view.Filter = new Predicate<object>(this.contains);
        }

        private void reInitialize()
        {
            timer = null;
            this.currentIndex = 0;
			CollectionView view = CollectionViewSource.GetDefaultView(this.Items) as CollectionView;

			view.Filter = new Predicate<object>(this.contains);

			if (this.Items.Count != 0 && this.IsThrobbing)
			{
				timer = new Timer(new TimerCallback(this.onTimerTick), null, 0, this.RefreshRate);
			}
        }

        private bool contains(object o)
        {
            int indexOfItem = this.Items.IndexOf(o);

            if ((indexOfItem >= this.currentIndex) && (indexOfItem < (this.currentIndex + this.ItemsPerRefresh)))
            {
                return true;
            }

            return false;
        }
    }
}
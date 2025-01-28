using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Net;
using System.Text;
using System.Collections.ObjectModel;

using DemoDev;

namespace StockGraph3D
{
    public class StockTime
    {
        public StockTime()
        {
        }

        public StockTime(string year, string month, string day, string interval)
        {
            Year = year;
            Month = month;
            Day = day;
            Interval = interval;
        }

        #region Properties

        public string Year
        {
            get { return _Year; }
            set { _Year = value; }
        }

        public string Month
        {
            get { return _Month; }
            set { _Month = value; }
        }

        public string Day
        {
            get { return _Day; }
            set { _Day = value; }
        }

        public string Interval
        {
            get { return _Interval; }
            set { _Interval = value; }
        }

        #endregion

        #region Globals

        private string _Year;
        private string _Month;
        private string _Day;
        private string _Interval;

        #endregion
    }

    public class StocksData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StocksData()
        {			
            _StocksItems = new StocksItemCollection();
            _Graph3DItems = new Graph3DItemCollection();
            _Graph3DLabelXItems = new StocksLabelItemXCollection();
            _Graph3DLabelYItems = new StocksLabelItemYCollection();
            _Graph3DLabelZItems = new StocksLabelItemZCollection();
            _StocksItems.StocksItemCostChanged += new EventHandler(OnStocksItemCostChanged);

            _wc = new WebClient();
        }

        #region Properties

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int TotalExpenses
        {
            // calculated property, no setter
            get { return _totalExpenses; }
        }

        public StocksItemCollection StocksItems
        {
            get { return _StocksItems; }
        }

        public Graph3DItemCollection Graph3DItems
        {
            get { return _Graph3DItems; }
        }

        public StocksLabelItemXCollection Graph3DLabelXItems
        {
            get { return _Graph3DLabelXItems; }
        }

        public StocksLabelItemYCollection Graph3DLabelYItems
        {
            get { return _Graph3DLabelYItems; }
        }
        public StocksLabelItemZCollection Graph3DLabelZItems
        {
            get { return _Graph3DLabelZItems; }
        }


        #endregion

        #region Public

        public StockItem FindStocksFromID(string id)
        {
            StockItem piRet = null;

            // find Stocks
            foreach (StockItem pi in _StocksItems)
            {
                if (pi.ID == id)
                {
                    piRet = pi;
                    break;
                }
            }

            return piRet;
        }

        public List3DItem FindList3DItemFromID(string id)
        {
            return null;
            /*
            List3DItem liRet = null;

            // find Stocks
            foreach (List3DItem li in _List3DItems)
            {
                if (li.ID == id)
                {
                    liRet = li;
                    break;
                }
            }

            return liRet;
          */
        }

        public void Build3DGraph(StockTime from, StockTime to)
        {
            _Graph3DItems.Clear();
            _Graph3DLabelXItems.Clear();
            _Graph3DLabelYItems.Clear();
            _Graph3DLabelZItems.Clear();

            List3DItemCollection l3dic = new List3DItemCollection();

            int row = 0;

            foreach (StockItem si in StocksItems)
            {
                AddGraph3DItem(ref l3dic, si, from, to, row);
                row++;
            }

            _Graph3DItems.Add (l3dic);

            BuildLabels();
        }

        public void BuildAnimate3DGraph(StockTime from, StockTime to)
        {
            if ((Graph3DItems == null) || (Graph3DItems.Count == 0))
                return;

            List3DItemCollection currentl3dic = Graph3DItems[0] as List3DItemCollection;
            _List3DFromAnimationItems = CopyList3DItemCollection(currentl3dic);

            _List3DToAnimationItems = new List3DItemCollection();

            int row = 0;

            foreach (StockItem si in StocksItems)
            {
                AddGraph3DItem(ref _List3DToAnimationItems, si, from, to, row);
                row++;
            }

            ReplayToAnimate3DGraph();
        }

        public void ReplayToAnimate3DGraph()
        {
            ReplayAnimate3DGraph(_List3DToAnimationItems);
        }

        public void ReplayFromAnimate3DGraph()
        {
            ReplayAnimate3DGraph(_List3DFromAnimationItems);
        }

        #endregion

        #region Private

        private void ReplayAnimate3DGraph(List3DItemCollection to)
        {
            List3DItemCollection currentl3dic = Graph3DItems[0] as List3DItemCollection;

            if ((to == null) || (to.Count == 0) || (currentl3dic == null) || (currentl3dic.Count == 0))
                return;

            int i = 0;
            foreach (List3DItem li in currentl3dic)
            {
                if (i >= to.Count)
                    continue;

                double heightFrom = li.Scale.Y;
                double heightTo = (to[i] as List3DItem).Scale.Y;
                double heightOffset = (heightTo - heightFrom) / 2.0;

                Vector3D scale = new Vector3D(li.Scale.X, (to[i] as List3DItem).Scale.Y, li.Scale.Z);
                li.AnimateScale(scale);

                Vector3D height = new Vector3D(li.Offset.X, li.Offset.Y + heightOffset, li.Offset.Z);
                li.AnimateTranslate(height);

                i++;
            }

        }

        private List3DItemCollection CopyList3DItemCollection(List3DItemCollection source)
        {
            List3DItemCollection l3dic = null;

            if ((source == null) || (source.Count == 0))
                return null;

            l3dic = new List3DItemCollection();

            foreach (List3DItem li in source)
            {
                List3DItem liNew = new List3DItem(li.Scale, li.Offset, new Vector3D(li.RotationAxisX.X, li.RotationAxisY.Y,li.RotationAxisZ.Z));
                l3dic.Add(liNew);
            }

            return l3dic;
        }

        private void OnStocksItemCostChanged(object sender, EventArgs e)
        {
            RecalculateTotalExpense();
        }

        private void RecalculateTotalExpense()
        {
            _totalExpenses = 0;
            /*
            foreach (StocksItem item in StocksItems)
            {
                _totalExpenses += item.Price;
            }
            */
            NotifyPropertyChanged("TotalExpenses");
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string BuildStockURL(string stock, StockTime from, StockTime to)
        {
            //string url = "http://finance.yahoo.com/d/quotes.csv?s=" + symbols + "&f=sl1d1t1c1baohgv&e=.csv";
            // url = "http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=sl1d11c1ohgv&e=.csv";
            //url = "http://moneycentral.msn.com/investor/research/freport.asp?Symbol=INTC&&Type=Fund";
            string url = "http://ichart.yahoo.com/table.csv?s=" + stock + "&a=" + from.Month + "&b=" + from.Day + "&c=" + from.Year + "&d=" + to.Month + "&e=" + to.Month + "&f=" + to.Year + "&g=d&ignore=.csv";

            return url;
        }

        private void GetStockData(string url, ref string[] quotes)
        {
            byte[] data = null;

            // Get byte data
            try
            {
                data = _wc.DownloadData(url);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception:({0})", ex.Message);
                string s = ex.ToString();
                return;
            }

            if (data == null)
                return;

            // Convert to string
            char[] cData = Encoding.ASCII.GetChars(data);
            string stockQuotes = new string(cData);

            quotes = stockQuotes.Split(new char[] { '\n' });
        }

        private void AddGraph3DItem(ref List3DItemCollection list3DItems, StockItem si, StockTime from, StockTime to, int row)
        {
            string[] quotes = null;
            string url, date, close;
            string[] dayquote;
            int i = 1;
            string stockdata;

            if (list3DItems == null)
                return;

            url = BuildStockURL(si.Title, from, to);
            GetStockData(url, ref quotes);

            if (quotes == null)
                return;

            int total = quotes.Length-1;
            int step = Convert.ToInt32 (from.Interval);
            int itemcount = 0;
            while (i < total)
            {
                stockdata = quotes[i];

                // Date, Open, High, Low, Close, Vol, Adj. Close
                dayquote = stockdata.Split(new char[] { ',' });

                date = dayquote[0];
                close = dayquote[4];
                double stockDayClose = Convert.ToDouble(close);

                // setup 3D elements
                List3DItem li3d = new List3DItem(new Vector3D(1, stockDayClose, 1), new Vector3D(row, 0, (double)itemcount), new Vector3D(0, 0, 0));
                //List3DItem li3d = new List3DItem(new Vector3D(1, 4, 1), new Vector3D(row, stockDayClose, (double)itemcount), new Vector3D(0, 0, 0));
                li3d.ID = si.ID;
                li3d.StockDate = date;
                li3d.StockPrice = stockDayClose;
                li3d.MainImage = si.GraphBrush;
                list3DItems.Add(li3d);

                i += step;
                itemcount++;
            }
        }

        private void BuildLabels()
        {
            // add labelX items
            foreach (StockItem si in StocksItems)
            {
                LabelItemX lix = new LabelItemX();
                lix.Title = si.Title;
                Graph3DLabelXItems.Add(lix);
            }

            //add labelY items
            double high = 0;
            List3DItemCollection l3dic = Graph3DItems[0] as List3DItemCollection;

            foreach (List3DItem li in l3dic)
            {
                if (high < li.StockPrice)
                    high = li.StockPrice;
            }

            double step = high / 10;
            double total = high;
            for (int i = 0; i < 10; i++)
            {
                LabelItemY liy = new LabelItemY();
                liy.Title = ((int)total).ToString(); ;
                Graph3DLabelYItems.Add(liy);

                total -= step;
            }

            // add labelZ items
            total = (l3dic.Count / StocksItems.Count);
            step = total / 10;
            step = Math.Round(step, MidpointRounding.AwayFromZero);
            if (step < 1)
                step = 1;
            for (int i = 0; i < total; i+=(int)step)
            {
                List3DItem li = l3dic[i];
                LabelItemZ liz = new LabelItemZ();
                liz.Title = li.StockDate;
                Graph3DLabelZItems.Add(liz);

            }
        }


        #endregion Private

        #region Globals

        private string _Name;
        private StocksItemCollection _StocksItems;
        private Graph3DItemCollection _Graph3DItems;
        private List3DItemCollection _List3DToAnimationItems;
        private List3DItemCollection _List3DFromAnimationItems;
		private int _totalExpenses;
        private WebClient _wc;
        private StocksLabelItemXCollection _Graph3DLabelXItems;
        private StocksLabelItemYCollection _Graph3DLabelYItems;
        private StocksLabelItemZCollection _Graph3DLabelZItems;

        #endregion Globals
    }

    public class StockItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        public Brush GraphBrush
        {
            get { return _GraphBrush; }
            set
            {
                _GraphBrush = value;
                NotifyPropertyChanged("GraphBrush");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                NotifyPropertyChanged("Price");
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string StocksImageFileName
        {
            get { return _StocksImageFileName; }
            set
            {
                _StocksImageFileName = value;
                NotifyPropertyChanged("StocksImageFileName");
            }
        }

        public Brush StocksImage
        {
            get { return _StocksImage; }
            set
            {
                _StocksImage = value;
                NotifyPropertyChanged("StocksImage");
            }
        }

        public ObservableCollection<TrackItem> Tracks
		{
			get { return _Tracks; }
			set
			{
				_Tracks = value;
				NotifyPropertyChanged("Tracks");
			}
		}

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string _ID = "Default ID";
        private string _Title = "Default Title";
        private string _description = "Default Description";
        private Brush _GraphBrush;
        private string _Price = "$0.0";
        private string _StocksImageFileName = string.Empty;
        private Brush _StocksImage = null;
        private ObservableCollection<TrackItem> _Tracks = new ObservableCollection<TrackItem>();
    }
	
    public class StocksItemCollection : ObservableCollection<StockItem>
    {
        public event EventHandler StocksItemCostChanged;

        public new void Add(StockItem item)
        {
            if (item != null)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(StocksItemPropertyChanged);
            }
            base.Add(item);
        }

        private void StocksItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cost")
            {
                RaiseStocksItemCostChanged(this, new EventArgs());
            }
        }

        void RaiseStocksItemCostChanged(object sender, EventArgs args)
        {
            if (StocksItemCostChanged != null)
            {
                StocksItemCostChanged(sender, args);
            }
        }
    }


    public class LabelItemX : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string _Title = "Default Title";
    }

    public class StocksLabelItemXCollection : ObservableCollection<LabelItemX>
    {
    }

    public class LabelItemY : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string _Title = "Default Title";
    }

    public class StocksLabelItemYCollection : ObservableCollection<LabelItemY>
    {
    }


    public class LabelItemZ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string _Title = "Default Title";
    }

    public class StocksLabelItemZCollection : ObservableCollection<LabelItemZ>
    {
    }



	public class TrackItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string Number
		{
			get { return _Number; }
			set
			{
				_Number = value;
				NotifyPropertyChanged("Number");
			}
		}

		public string Title
		{
			get { return _Title; }
			set
			{
				_Title = value;
				NotifyPropertyChanged("Title");
			}
		}

		private void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}

		public TrackItem(string Number, string Title)
		{
			_Number = Number;
			_Title = Title;
		}
		private string _Number = "Default Number";
		private string _Title = "Default Title";		
	}
}





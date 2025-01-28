using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;

namespace StockWatch
{
    public class StockWatchModel
    {
        private ObservableCollection<Quote> quoteList = new ObservableCollection<Quote>();

        public ObservableCollection<Quote> QuoteList
        {
            get { return quoteList; }
        }

        private void RefreshQuotes()
        {
        }

        public static Quote GetQuote(string symbol)
        {
            try
            {

                XmlDocument document = new XmlDocument();
                document.Load("http://www.live.com/cfw/quote.aspx?symbols=" + symbol);

                XmlNodeList list = document.GetElementsByTagName("item");
                if (list.Count != 0)
                {
                    XmlNode node = list[0];
                    Quote quote = new Quote();
                    quote.Symbol = node.Attributes["symbol"].InnerText;
                    quote.Change = Double.Parse(node.Attributes["change"].InnerText);
                    quote.CurrentValue = Double.Parse(node.Attributes["last"].InnerText);
                    quote.Name = node.SelectSingleNode("title").InnerText;
                    quote.Link = node.SelectSingleNode("link").InnerText;

                    return quote;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}

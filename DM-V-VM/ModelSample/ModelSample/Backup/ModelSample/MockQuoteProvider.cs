using System;
using System.Collections.Generic;
using System.Text;

namespace ModelSample
{
    /// <summary>
    /// Mock IStockQuoteProvider that alwyas returns 100.
    /// </summary>
    public class MockQuoteProvider : IStockQuoteProvider
    {
        public bool TryGetQuote(string symbol, out double quote)
        {
            quote = 100.0;

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDesigns2
{
    public class ReflectionOperationEventArgs : EventArgs
    {
        private readonly List<string> _results;

        public ReflectionOperationEventArgs(List<string> results)
        {
            this._results = results;
        }

        public List<string> Results
        {
            get
            {
                return this._results;
            }
        }
    }
}

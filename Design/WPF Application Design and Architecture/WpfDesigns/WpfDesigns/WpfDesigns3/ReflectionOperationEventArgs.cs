using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDesigns3
{
    public class ReflectionOperationEventArgs<T> : EventArgs
    {
        private List<T> _results;

        public ReflectionOperationEventArgs(List<T> results)
        {
            this._results = results;
        }

        public List<T> Results
        {
            get
            {
                return this._results;
            }
        }
    }
}

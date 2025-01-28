using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDesigns4.DataModels
{
    //Not really a DataModel but it is part of the data/model layer
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

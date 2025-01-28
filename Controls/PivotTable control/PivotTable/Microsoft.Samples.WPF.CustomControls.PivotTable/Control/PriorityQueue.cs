//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: PriorityQueue class.
//  Creator: t-tomkm
//  Date Created: 08/17/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Priority queue based on heap and hash table.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    internal class PriorityQueue<TKey, TValue>
        where TValue: IComparable<TValue>
    {
        #region Fields
        private Dictionary<TKey, int> _indexes;
        private List<KeyValuePair> _items;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PriorityQueue()
        {
            _indexes = new Dictionary<TKey, int>();
            _items = new List<KeyValuePair>();
        }

        #region Properties
        /// <summary>
        /// Gets number of items in the priority queue.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Returns collection of all keys.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get { return _indexes.Keys; }
        }

        /// <summary>
        /// Returns maximum value. If there are no elements, returns default value.
        /// </summary>
        public TValue MaximumValue
        {
            get { return _items.Count > 0 ? _items[0].Value : default(TValue); }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds new element to the queue. This operation has complexity O(log n).
        /// </summary>
        /// <param name="key">Key of the element.</param>
        /// <param name="value">Value of the element.</param>
        public void Add(TKey key, TValue value)
        {
            _items.Add(new KeyValuePair(key, value));
            _indexes[key] = _items.Count - 1;
            UpHeap(_items.Count - 1);
        }

        /// <summary>
        /// Checks whether the queue contains the given key. It approaches an O(1) operation.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>true if the queue contains an element with the specified key; otherwise, false.</returns>
        public bool Contains(TKey key)
        {
            return _indexes.ContainsKey(key);
        }

        /// <summary>
        /// Removes element with given key from the queue. This operation has complexity
        /// O(log n).
        /// </summary>
        /// <param name="key">Key of element to remove.</param>
        public void Remove(TKey key)
        {
            int index = _indexes[key];
            
            int lastItemIndex = _items.Count - 1;
            if (index < lastItemIndex)
            {
                SwapItems(index, lastItemIndex);
                _items.RemoveAt(lastItemIndex);
                DownHeap(index);
            }
            else
            {
                _items.RemoveAt(lastItemIndex);
            }
            _indexes.Remove(key);
        }

        /// <summary>
        /// Updated value of element with the given key. This operation has complexity O(log n).
        /// </summary>
        /// <param name="key">Key of the element to update.</param>
        /// <param name="value">New value of the element.</param>
        public void UpdateValue(TKey key, TValue value)
        {
            Remove(key);
            Add(key, value);
        }
        #endregion

        #region Private methods
        private int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private int GetLeftChildIndex(int index)
        {
            return index * 2 + 1;
        }

        private int GetRightChildIndex(int index)
        {
            return index * 2 + 2;
        }

        private void DownHeap(int index)
        {
            int current = index;
            int leftChild = GetLeftChildIndex(current);

            while (leftChild < _items.Count)
            {
                int rightChild = GetRightChildIndex(current);
                int maximumIndex = current;
                if (_items[leftChild].CompareTo(_items[maximumIndex]) > 0)
                {
                    maximumIndex = leftChild;
                }
                if ((rightChild < _items.Count) && (_items[rightChild].CompareTo(_items[maximumIndex]) > 0))
                {
                    maximumIndex = rightChild;
                }
                if (maximumIndex == current)
                {
                    break;
                }
                SwapItems(current, maximumIndex);
                current = maximumIndex;
                leftChild = GetLeftChildIndex(current);
            }
        }

        private void UpHeap(int index)
        {
            int current = index;

            while (current != 0)
            {
                int parent = GetParentIndex(current);
                if (_items[parent].CompareTo(_items[current]) >= 0)
                {
                    return;
                }
                SwapItems(current, parent);
                current = parent;
            }
        }

        private void SwapItems(int firstIndex, int secondIndex)
        {
            _indexes[_items[firstIndex].Key] = secondIndex;
            _indexes[_items[secondIndex].Key] = firstIndex;
            KeyValuePair temp = _items[firstIndex];
            _items[firstIndex] = _items[secondIndex];
            _items[secondIndex] = temp;
        }
        #endregion

        /// <summary>
        /// Structure that stores key and value and implements IComparable interface.
        /// </summary>
        private struct KeyValuePair : IComparable<KeyValuePair>
        {
            private TKey _key;
            private TValue _value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="value">Value.</param>
            public KeyValuePair(TKey key, TValue value)
            {
                _key = key;
                _value = value;
            }

            /// <summary>
            /// Gets key.
            /// </summary>
            public TKey Key
            {
                get { return _key; }
            }

            /// <summary>
            /// Gets value.
            /// </summary>
            public TValue Value
            {
                get { return _value; }
            }

            #region IComparable<KeyValuePair> Members
            /// <summary>
            /// Compares values of the current object with another KeyValuePair's values.
            /// </summary>
            /// <param name="other">An KeyValuePair to compare with this object.</param>
            /// <returns>A value that indicates the relative order of the objects being compared.
            /// See documentation for IComparable&lt;T&gt; for more details.</returns>
            public int CompareTo(KeyValuePair other)
            {
                return _value.CompareTo(other._value);
            }
            #endregion
        }
    }
}

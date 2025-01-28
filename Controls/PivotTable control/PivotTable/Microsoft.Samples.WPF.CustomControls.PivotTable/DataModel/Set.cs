//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: Set class.
//  Creator: t-tomkm
//  Date Created: 09/07/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// ISet&lt;T&gt; implementation.
    /// </summary>
    /// <typeparam name="T">Type of items.</typeparam>
    public class Set<T> : ISet<T>
    {
        private Dictionary<T, object> _items;

        /// <summary>
        /// Constructor. Creates an empty set.
        /// </summary>
        public Set()
        {
            _items = new Dictionary<T, object>();
        }

        /// <summary>
        /// Constructor. Creates a set that contains all given items.
        /// </summary>
        /// <param name="items">Items that should be added to the set. Only one instance
        /// of each item will be added to the set.</param>
        public Set(IEnumerable<T> items)
            : this()
        {
            Add(items);
        }

        /// <summary>
        /// Constructor. Creates a set that contains all given items.
        /// </summary>
        /// <param name="items">Items that should be added to the set. Only one instance
        /// of each item will be added to the set. Each item in the collection must be
        /// of type T.</param>
        public Set(IEnumerable items) : this()
        {
            Add(items);
        }

        /// <summary>
        /// Gets number of items in the set.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Adds new item to the set. If the set already contains the given item,
        /// it is not added second time.
        /// </summary>
        /// <param name="item">New item to add.</param>
        public void Add(T item)
        {
            if (!_items.ContainsKey(item))
            {
                _items.Add(item, null);
            }
        }

        /// <summary>
        /// Adds new items to the set.
        /// </summary>
        /// <param name="items">Collection of items to add to the set.</param>
        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds new items to the set.
        /// </summary>
        /// <param name="items">Collection of items to add to the set. Each item
        /// must be of type T.</param>
        public void Add(IEnumerable items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes all items from the set.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Removes item from the set.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Returns true, if the given item existed in the set and was removed; otherwise,
        /// returns false.</returns>
        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Checks whether the set contains the given item.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Returns true if the set contains the given item; otherwise, returns false.</returns>
        public bool Contains(T item)
        {
            return _items.ContainsKey(item);
        }

        /// <summary>
        /// Removes from the current set all items that exist in the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains elements to remove.</param>
        public void Subtract(ISet<T> otherSet)
        {
            foreach (T item in otherSet)
            {
                if (Contains(item))
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// Adds to the current set all items from the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains elements to add.</param>
        public void Union(ISet<T> otherSet)
        {
            Add(otherSet);
        }

        /// <summary>
        /// Removes from the current set all items that are not in the given set.
        /// </summary>
        /// <param name="otherSet">Set that contains element that shouldn't be removed.</param>
        public void Intersect(ISet<T> otherSet)
        {
            Set<T> itemsToRemove = new Set<T>();
            foreach (T item in this)
            {
                if (!otherSet.Contains(item))
                {
                    itemsToRemove.Add(item);
                }
            }
            Subtract(itemsToRemove);
        }

        /// <summary>
        /// Copies the elements of the Set to an Array, starting at a particular Array index. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements
        /// copied from Set. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Set is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in _items.Keys)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (T item in _items.Keys)
            {
                yield return item;
            }
        }

        #endregion
    }
}

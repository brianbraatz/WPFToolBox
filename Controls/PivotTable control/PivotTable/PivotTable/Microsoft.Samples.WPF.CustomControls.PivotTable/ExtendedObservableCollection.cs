//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: ExtendedObservableCollection<T> class.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// ObservableCollection&lt;T&gt; that allows to add multiple items with single
    /// change notification.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    internal class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        private bool _notifyChanges;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtendedObservableCollection()
        {
            _notifyChanges = true;
        }

        /// <summary>
        /// Adds new items to the collection. It raises the CollectionChanged event only once
        /// (with NotifyCollectionChangedAction.Reset action).
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            try
            {
                _notifyChanges = false;
                CheckReentrancy();
                foreach (T item in items)
                {
                    Add(item);
                }
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            finally
            {
                _notifyChanges = true;
            }
        }

        /// <summary>
        /// Removes all old items from the collection and inserts new ones. It raises
        /// the CollectionChanged event only once (with NotifyCollectionChangedAction.Reset
        /// action).
        /// </summary>
        /// <param name="newItems">New set of items.</param>
        public void ReplaceAllItems(IEnumerable<T> newItems)
        {
            try
            {
                _notifyChanges = false;
                CheckReentrancy();
                Clear();
                foreach (T item in newItems)
                {
                    Add(item);
                }
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            finally
            {
                _notifyChanges = true;
            }
        }

        /// <summary>
        /// Clears the internal collection. If the _notifyChanges filed is set to
        /// false, this method will not notify about the change.
        /// </summary>
        protected override void ClearItems()
        {
            if (_notifyChanges)
            {
                CheckReentrancy();
            }
            Items.Clear();
            if (_notifyChanges)
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Inserts new item to the internal collection. If the _notifyChanges field
        /// is set to false, this method will not notify about the change.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="item"/> 
        /// should be inserted.</param>
        /// <param name="item">The tuple to insert into the collection.</param>
        protected override void InsertItem(int index, T item)
        {
            if (_notifyChanges)
            {
                CheckReentrancy();
            }
            Items.Insert(index, item);
            if (_notifyChanges)
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, index));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}

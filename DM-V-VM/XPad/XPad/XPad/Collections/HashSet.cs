using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{


    /// <summary>
    /// A set collection class based on linear hashing
    /// </summary>
    [Serializable]
    public class HashSet<T> : CollectionBase<T>, ICollection<T>
    {
        #region Feature
        /// <summary>
        /// Enum class to assist printing of compilation alternatives.
        /// </summary>
        [Flags]
        public enum Feature : short
        {
            /// <summary>
            /// Nothing
            /// </summary>
            Dummy = 0,
            /// <summary>
            /// Buckets are of reference type
            /// </summary>
            RefTypeBucket = 1,
            /// <summary>
            /// Primary buckets are of value type
            /// </summary>
            ValueTypeBucket = 2,
            /// <summary>
            /// Using linear probing to resolve index clashes
            /// </summary>
            LinearProbing = 4,
            /// <summary>
            /// Shrink table when very sparsely filled
            /// </summary>
            ShrinkTable = 8,
            /// <summary>
            /// Use chaining to resolve index clashes
            /// </summary>
            Chaining = 16,
            /// <summary>
            /// Use hash function on item hash code
            /// </summary>
            InterHashing = 32,
            /// <summary>
            /// Use a universal family of hash functions on item hash code
            /// </summary>
            RandomInterHashing = 64
        }



        static Feature features = Feature.Dummy | Feature.ValueTypeBucket;


        /// <summary>
        /// Show which implementation features was chosen at compilation time
        /// </summary>
        public static Feature Features { get { return features; } }

        #endregion

        #region Fields

        int indexmask, bits, bitsc, origbits, lastchosen; //bitsc==32-bits; indexmask==(1<<bits)-1;

        Bucket[] table;


        bool defaultvalid = false;

        T defaultitem;

        double fillfactor = 0.66;

        int resizethreshhold;



        #endregion

        #region Events

        

        #endregion

        #region Bucket nested class(es)

        struct Bucket
        {
            internal T item;

            internal int hashval; //Cache!


            internal OverflowBucket overflow;


            internal Bucket(T item, int hashval)
            {
                this.item = item;
                this.hashval = hashval;
                this.overflow = default(OverflowBucket);
            }

        }



        class OverflowBucket
        {
            internal T item;

            internal int hashval; //Cache!

            internal OverflowBucket overflow;


            internal OverflowBucket(T item, int hashval, OverflowBucket overflow)
            {
                this.item = item;
                this.hashval = hashval;
                this.overflow = overflow;
            }
        }

        #endregion

        #region Basic Util

        bool equals(T i1, T i2) { return itemequalityComparer.Equals(i1, i2); }

#if !REFBUCKET
        bool isnull(T item) { return itemequalityComparer.Equals(item, default(T)); }
#endif

        int gethashcode(T item) { return itemequalityComparer.GetHashCode(item); }


        int hv2i(int hashval)
        {
#if INTERHASHING
			//Note: *inverse  mod 2^32 is -1503427877
			return (int)(((uint)hashval * 1529784659) >>bitsc); 
#elif RANDOMINTERHASHING
      return (int)(((uint)hashval * randomhashfactor) >> bitsc);
#else
            return indexmask & hashval;
#endif
        }


        void expand()
        {
            //Console.WriteLine(String.Format("Expand to {0} bits", bits+1));
            resize(bits + 1);
        }


        void shrink()
        {
            if (bits > 3)
            {
                //Console.WriteLine(String.Format("Shrink to {0} bits", bits - 1));
                resize(bits - 1);
            }
        }


        void resize(int bits)
        {
            //Console.WriteLine(String.Format("Resize to {0} bits", bits));
            this.bits = bits;
            bitsc = 32 - bits;
            indexmask = (1 << bits) - 1;

            Bucket[] newtable = new Bucket[indexmask + 1];

            for (int i = 0, s = table.Length; i < s; i++)
            {
                Bucket b = table[i];

#if LINEARPROBING
#if REFBUCKET
        if (b != null)
        {
          int j = hv2i(b.hashval);

          while (newtable[j] != null) { j = indexmask & (j + 1); }

          newtable[j] = b;
        }
#else
        if (!isnull(b.item))
        {
          int j = hv2i(b.hashval);

          while (!isnull(newtable[j].item)) { j = indexmask & (j + 1); }

          newtable[j] = b;
        }
#endif
#else
#if REFBUCKET
        while (b != null)
        {
          int j = hv2i(b.hashval);

          newtable[j] = new Bucket(b.item, b.hashval, newtable[j]);
          b = b.overflow;
        }
#else
                if (!isnull(b.item))
                {
                    insert(b.item, b.hashval, newtable);

                    OverflowBucket ob = b.overflow;

                    while (ob != null)
                    {
                        insert(ob.item, ob.hashval, newtable);
                        ob = ob.overflow;
                    }
                }
#endif
#endif
            }

            table = newtable;
            resizethreshhold = (int)(table.Length * fillfactor);
            //Console.WriteLine(String.Format("Resize to {0} bits done", bits));
        }

#if REFBUCKET
#else
#if LINEARPROBING
#else
        //Only for resize!!!
        private void insert(T item, int hashval, Bucket[] t)
        {
            int i = hv2i(hashval);
            Bucket b = t[i];

            if (!isnull(b.item))
            {
                t[i].overflow = new OverflowBucket(item, hashval, b.overflow);
            }
            else
                t[i] = new Bucket(item, hashval);
        }
#endif
#endif

        /// <summary>
        /// Search for an item equal (according to itemequalityComparer) to the supplied item.  
        /// </summary>
        /// <param name="item"></param>
        /// <param name="add">If true, add item to table if not found.</param>
        /// <param name="update">If true, update table entry if item found.</param>
        /// <param name="raise">If true raise events</param>
        /// <returns>True if found</returns>
        private bool searchoradd(ref T item, bool add, bool update, bool raise)
        {

#if LINEARPROBING
#if REFBUCKET
      int hashval = gethashcode(item);
      int i = hv2i(hashval);
      Bucket b = table[i];

      while (b != null)
      {
        T olditem = b.item;
        if (equals(olditem, item))
        {
          if (update)
            b.item = item;
          else
            item = olditem;

          if (raise && update)
            raiseForUpdate(item, olditem);
          return true;
        }

        b = table[i = indexmask & (i + 1)];
      }

      if (!add) goto notfound;

      table[i] = new Bucket(item, hashval);

#else
      if (isnull(item))
      {
        if (defaultvalid)
        {
          T olditem = defaultitem;
          if (update)
            defaultitem = item;
          else
            item = defaultitem;

          if (raise && update)
            raiseForUpdate(item, olditem);
          return true;
        }

        if (!add) goto notfound;

        defaultvalid = true;
        defaultitem = item;
      }
      else
      {
        int hashval = gethashcode(item);
        int i = hv2i(hashval);
        T t = table[i].item;

        while (!isnull(t))
        {
          if (equals(t, item))
          {
            if (update)
              table[i].item = item;
            else
              item = t;

            if (raise && update)
              raiseForUpdate(item, t);
            return true;
          }

          t = table[i = indexmask & (i + 1)].item;
        }

        if (!add) goto notfound;

        table[i] = new Bucket(item, hashval);
      }
#endif
#else
#if REFBUCKET
      int hashval = gethashcode(item);
      int i = hv2i(hashval);
      Bucket b = table[i], bold = null;

      if (b != null)
      {
        while (b != null)
        {
          T olditem = b.item;
          if (equals(olditem, item))
          {
            if (update)
            {
              b.item = item;
            }
            item = b.item;

            if (raise && update)
              raiseForUpdate(item, olditem);
            return true;
          }

          bold = b;
          b = b.overflow;
        }

        if (!add) goto notfound;

        bold.overflow = new Bucket(item, hashval, null);
      }
      else
      {
        if (!add) goto notfound;

        table[i] = new Bucket(item, hashval, null);
      }
#else
            if (isnull(item))
            {
                if (defaultvalid)
                {
                    T olditem = defaultitem;
                    if (update)
                        defaultitem = item;
                    else
                        item = defaultitem;

                    if (raise && update)
                        raiseForUpdate(item, olditem);
                    return true;
                }

                if (!add) goto notfound;

                defaultvalid = true;
                defaultitem = item;
            }
            else
            {
                int hashval = gethashcode(item);
                int i = hv2i(hashval);
                Bucket b = table[i];

                if (!isnull(b.item))
                {
                    if (equals(b.item, item))
                    {
                        if (update)
                            table[i].item = item;
                        else
                            item = b.item;

                        if (raise && update)
                            raiseForUpdate(item, b.item);
                        return true;
                    }

                    OverflowBucket ob = table[i].overflow;

                    if (ob == null)
                    {
                        if (!add) goto notfound;

                        table[i].overflow = new OverflowBucket(item, hashval, null);
                    }
                    else
                    {
                        T olditem = ob.item;
                        while (ob.overflow != null)
                        {
                            if (equals(item, olditem))
                            {
                                if (update)
                                    ob.item = item;
                                else
                                    item = olditem;

                                if (raise && update)
                                    raiseForUpdate(item, olditem);
                                return true;
                            }

                            ob = ob.overflow;
                            olditem = ob.item;
                        }

                        if (equals(item, olditem))
                        {
                            if (update)
                                ob.item = item;
                            else
                                item = olditem;

                            if (raise && update)
                                raiseForUpdate(item, olditem);
                            return true;
                        }

                        if (!add) goto notfound;

                        ob.overflow = new OverflowBucket(item, hashval, null);
                    }
                }
                else
                {
                    if (!add) goto notfound;

                    table[i] = new Bucket(item, hashval);
                }
            }
#endif
#endif
            size++;
            if (size > resizethreshhold)
                expand();
        notfound:
            if (raise && add)
                raiseForAdd(item);
            return false;
        }


        private bool remove(ref T item)
        {

            if (size == 0)
                return false;
#if LINEARPROBING
#if REFBUCKET
      int hashval = gethashcode(item);
      int index = hv2i(hashval);
      Bucket b = table[index];

      while (b != null)
      {
        if (equals(item, b.item))
        {
          //ref
          item = table[index].item;
          table[index] = null;

          //Algorithm R
          int j = (index + 1) & indexmask;

          b = table[j];
          while (b != null)
          {
            int k = hv2i(b.hashval);

            if ((k <= index && index < j) || (index < j && j < k) || (j < k && k <= index))
            //if (index > j ? (j < k && k <= index): (k <= index || j < k) )
            {
              table[index] = b;
              table[j] = null;
              index = j;
            }

            j = (j + 1) & indexmask;
            b = table[j];
          }

          goto found;
        }

        b = table[index = indexmask & (index + 1)];
      }
      return false;
#else
      if (isnull(item))
      {
        if (!defaultvalid)
          return false;

        //ref
        item = defaultitem;
        defaultvalid = false;
        defaultitem = default(T); //No spaceleaks!
      }
      else
      {
        int hashval = gethashcode(item);
        int index = hv2i(hashval);
        T t = table[index].item;

        while (!isnull(t))
        {
          if (equals(item, t))
          {
            //ref
            item = table[index].item;
            table[index].item = default(T);

            //algorithm R
            int j = (index + 1) & indexmask;
            Bucket b = table[j];

            while (!isnull(b.item))
            {
              int k = hv2i(b.hashval);

              if ((k <= index && index < j) || (index < j && j < k) || (j < k && k <= index))
              {
                table[index] = b;
                table[j].item = default(T);
                index = j;
              }

              j = (j + 1) & indexmask;
              b = table[j];
            }

            goto found;
          }

          t = table[index = indexmask & (index + 1)].item;
        }

        return false;
      }
#endif
    found:
#else
#if REFBUCKET
      int hashval = gethashcode(item);
      int index = hv2i(hashval);
      Bucket b = table[index], bold;

      if (b == null)
        return false;

      if (equals(item, b.item))
      {
        //ref
        item = b.item;
        table[index] = b.overflow;
      }
      else
      {
        bold = b;
        b = b.overflow;
        while (b != null && !equals(item, b.item))
        {
          bold = b;
          b = b.overflow;
        }

        if (b == null)
          return false;

        //ref
        item = b.item;
        bold.overflow = b.overflow;
      }

#else
            if (isnull(item))
            {
                if (!defaultvalid)
                    return false;

                //ref
                item = defaultitem;
                defaultvalid = false;
                defaultitem = default(T); //No spaceleaks!
            }
            else
            {
                int hashval = gethashcode(item);
                int index = hv2i(hashval);
                Bucket b = table[index];
                OverflowBucket ob = b.overflow;

                if (equals(item, b.item))
                {
                    //ref
                    item = b.item;
                    if (ob == null)
                    {
                        table[index] = new Bucket();
                    }
                    else
                    {
                        b = new Bucket(ob.item, ob.hashval);
                        b.overflow = ob.overflow;
                        table[index] = b;
                    }
                }
                else
                {
                    if (ob == null)
                        return false;

                    if (equals(item, ob.item))
                    {
                        //ref
                        item = ob.item;
                        table[index].overflow = ob.overflow;
                    }
                    else
                    {
                        while (ob.overflow != null)
                            if (equals(item, ob.overflow.item))
                            {
                                //ref
                                item = ob.overflow.item;
                                break;
                            }
                            else
                                ob = ob.overflow;

                        if (ob.overflow == null)
                            return false;

                        ob.overflow = ob.overflow.overflow;
                    }
                }
            }
#endif
#endif
            size--;

            return true;
        }


        private void clear()
        {
            bits = origbits;
            bitsc = 32 - bits;
            indexmask = (1 << bits) - 1;
            size = 0;
            table = new Bucket[indexmask + 1];
            resizethreshhold = (int)(table.Length * fillfactor);
#if !REFBUCKET
            defaultitem = default(T);
            defaultvalid = false;
#endif
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Create a hash set with natural item equalityComparer and default fill threshold (66%)
        /// and initial table size (16).
        /// </summary>
        public HashSet()
            : this(EqualityComparer<T>.Default) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer and default fill threshold (66%)
        /// and initial table size (16).
        /// </summary>
        /// <param name="itemequalityComparer">The external item equalityComparer</param>
        public HashSet(IEqualityComparer<T> itemequalityComparer)
            : this(16, itemequalityComparer) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer and default fill threshold (66%)
        /// </summary>
        /// <param name="capacity">Initial table size (rounded to power of 2, at least 16)</param>
        /// <param name="itemequalityComparer">The external item equalityComparer</param>
        public HashSet(int capacity, IEqualityComparer<T> itemequalityComparer)
            : this(capacity, 0.66, itemequalityComparer) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer.
        /// </summary>
        /// <param name="capacity">Initial table size (rounded to power of 2, at least 16)</param>
        /// <param name="fill">Fill threshold (in range 10% to 90%)</param>
        /// <param name="itemequalityComparer">The external item equalityComparer</param>
        public HashSet(int capacity, double fill, IEqualityComparer<T> itemequalityComparer)
            : base(itemequalityComparer)
        {
            if (fill < 0.1 || fill > 0.9)
                throw new ArgumentException("Fill outside valid range [0.1, 0.9]");
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be non-negative");
            //this.itemequalityComparer = itemequalityComparer;
            origbits = 4;
            while (capacity - 1 >> origbits > 0) origbits++;
            clear();
        }



        #endregion

        #region IEditableCollection<T> Members



        /// <summary>
        /// Check if an item is in the set 
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>True if set contains item</returns>
        
        public virtual bool Contains(T item) { return searchoradd(ref item, false, false, false); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so report the actual item object found.
        /// </summary>
        /// <param name="item">On entry, the item to look for.
        /// On exit the item found, if any</param>
        /// <returns>True if set contains item</returns>
        
        public virtual bool Find(ref T item) { return searchoradd(ref item, false, false, false); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so replace the item object in the set with the supplied one.
        /// </summary>
        /// <param name="item">The item object to update with</param>
        /// <returns>True if item was found (and updated)</returns>
        
        public virtual bool Update(T item)
        { updatecheck(); return searchoradd(ref item, false, true, true); }

        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so replace the item object in the set with the supplied one.
        /// </summary>
        /// <param name="item">The item object to update with</param>
        /// <param name="olditem"></param>
        /// <returns>True if item was found (and updated)</returns>
        public virtual bool Update(T item, out T olditem)
        { updatecheck(); olditem = item; return searchoradd(ref olditem, false, true, true); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set.
        /// If found, report the actual item object in the set,
        /// else add the supplied one.
        /// </summary>
        /// <param name="item">On entry, the item to look for or add.
        /// On exit the actual object found, if any.</param>
        /// <returns>True if item was found</returns>
        
        public virtual bool FindOrAdd(ref T item)
        { updatecheck(); return searchoradd(ref item, true, false, true); }


        /// <summary>
        /// Check if an item (collection equal to a supplied one) is in the set and
        /// if so replace the item object in the set with the supplied one; else
        /// add the supplied one.
        /// </summary>
        /// <param name="item">The item to look for and update or add</param>
        /// <returns>True if item was updated</returns>
        
        public virtual bool UpdateOrAdd(T item)
        { updatecheck(); return searchoradd(ref item, true, true, true); }


        /// <summary>
        /// Check if an item (collection equal to a supplied one) is in the set and
        /// if so replace the item object in the set with the supplied one; else
        /// add the supplied one.
        /// </summary>
        /// <param name="item">The item to look for and update or add</param>
        /// <param name="olditem"></param>
        /// <returns>True if item was updated</returns>
        public virtual bool UpdateOrAdd(T item, out T olditem)
        { updatecheck(); olditem = item; return searchoradd(ref olditem, true, true, true); }


        /// <summary>
        /// Remove an item from the set
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if item was (found and) removed </returns>
        
        public virtual bool Remove(T item)
        {
            updatecheck();
            if (remove(ref item))
            {
#if SHRINK
				if (size<resizethreshhold/2 && resizethreshhold>8)
					shrink();
#endif
                raiseForRemove(item);
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Remove an item from the set, reporting the actual matching item object.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <param name="removeditem">The removed value.</param>
        /// <returns>True if item was found.</returns>
        
        public virtual bool Remove(T item, out T removeditem)
        {
            updatecheck();
            removeditem = item;
            if (remove(ref removeditem))
            {
#if SHRINK
				if (size<resizethreshhold/2 && resizethreshhold>8)
					shrink();
#endif
                raiseForRemove(removeditem);
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Remove all items in a supplied collection from this set.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="items">The items to remove.</param>
        
        public virtual void RemoveAll<U>(IEnumerable<U> items) where U : T
        {
            updatecheck();
            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(this);
            bool raise = raiseHandler.MustFire;
            T jtem;
            foreach (U item in items)
            { jtem = item; if (remove(ref jtem) && raise) raiseHandler.Remove(jtem); }
#if SHRINK
			if (size < resizethreshhold / 2 && resizethreshhold > 16)
			{
				int newlength = table.Length;

				while (newlength >= 32 && newlength * fillfactor / 2 > size)
					newlength /= 2;

				resize(newlength - 1);
			}
#endif
            if (raise) raiseHandler.Raise();
        }

        /// <summary>
        /// Remove all items from the set, resetting internal table to initial size.
        /// </summary>
        
        public virtual void Clear()
        {
            updatecheck();
            int oldsize = size;
            clear();
            if (ActiveEvents != 0 && oldsize > 0)
            {
                raiseCollectionCleared(true, oldsize);
                raiseCollectionChanged();
            }
        }


        /// <summary>
        /// Remove all items *not* in a supplied collection from this set.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="items">The items to retain</param>
        
        public virtual void RetainAll<U>(IEnumerable<U> items) where U : T
        {
            updatecheck();

            HashSet<T> aux = new HashSet<T>(EqualityComparer);

            //This only works for sets:
            foreach (U item in items)
                if (Contains(item))
                {
                    T jtem = item;

                    aux.searchoradd(ref jtem, true, false, false);
                }

            if (size == aux.size)
                return;

            CircularQueue<T> wasRemoved = null;
            if ((ActiveEvents & EventTypeEnum.Removed) != 0)
            {
                wasRemoved = new CircularQueue<T>();
                foreach (T item in this)
                    if (!aux.Contains(item))
                        wasRemoved.Enqueue(item);
            }

            table = aux.table;
            size = aux.size;
#if !REFBUCKET
            defaultvalid = aux.defaultvalid;
            defaultitem = aux.defaultitem;
#endif
            indexmask = aux.indexmask;
            resizethreshhold = aux.resizethreshhold;


            if ((ActiveEvents & EventTypeEnum.Removed) != 0)
                raiseForRemoveAll(wasRemoved);
            else if ((ActiveEvents & EventTypeEnum.Changed) != 0)
                raiseCollectionChanged();
        }

        /// <summary>
        /// Check if all items in a supplied collection is in this set
        /// (ignoring multiplicities). 
        /// </summary>
        /// <param name="items">The items to look for.</param>
        /// <typeparam name="U"></typeparam>
        /// <returns>True if all items are found.</returns>
        
        public virtual bool ContainsAll<U>(IEnumerable<U> items) where U : T
        {
            foreach (U item in items)
                if (!Contains(item))
                    return false;
            return true;
        }


        /// <summary>
        /// Create an array containing all items in this set (in enumeration order).
        /// </summary>
        /// <returns>The array</returns>
        
        public override T[] ToArray()
        {
            T[] res = new T[size];
            int index = 0;

#if !REFBUCKET
            if (defaultvalid)
                res[index++] = defaultitem;
#endif
            for (int i = 0; i < table.Length; i++)
            {
                Bucket b = table[i];
#if LINEARPROBING
#if REFBUCKET
        if (b != null)
          res[index++] = b.item;
#else
        if (!isnull(b.item))
          res[index++] = b.item;
#endif
#else
#if REFBUCKET
        while (b != null)
        {
          res[index++] = b.item;
          b = b.overflow;
        }
#else
                if (!isnull(b.item))
                {
                    res[index++] = b.item;

                    OverflowBucket ob = b.overflow;

                    while (ob != null)
                    {
                        res[index++] = ob.item;
                        ob = ob.overflow;
                    }
                }
#endif
#endif
            }

            Debug.Assert(size == index);
            return res;
        }


        /// <summary>
        /// Count the number of times an item is in this set (either 0 or 1).
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>1 if item is in set, 0 else</returns>
        
        public virtual int ContainsCount(T item) { return Contains(item) ? 1 : 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionBase<T> UniqueItems() { return this; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionBase<KeyValuePair<T, int>> ItemMultiplicities()
        {
            return new MultiplicityOne<T>(this);
        }

        /// <summary>
        /// Remove all (at most 1) copies of item from this set.
        /// </summary>
        /// <param name="item">The item to remove</param>
        
        public virtual void RemoveAllCopies(T item) { Remove(item); }

        #endregion

        #region IEnumerable<T> Members


        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        
       

        /// <summary>
        /// Create an enumerator for this set.
        /// </summary>
        /// <returns>The enumerator</returns>
        
        public override IEnumerator<T> GetEnumerator()
        {
            int index = -1;
            int mystamp = stamp;
            int len = table.Length;

#if LINEARPROBING
#if REFBUCKET
      while (++index < len)
      {
        if (mystamp != stamp) throw new CollectionModifiedException();

        if (table[index] != null) yield return table[index].item;
      }
#else
      if (defaultvalid)
        yield return defaultitem;

      while (++index < len)
      {
        if (mystamp != stamp) throw new CollectionModifiedException();

        T item = table[index].item;

        if (!isnull(item)) yield return item;
      }
#endif
#else
#if REFBUCKET
      Bucket b = null;
#else
            OverflowBucket ob = null;

            if (defaultvalid)
                yield return defaultitem;
#endif
            while (true)
            {
                if (mystamp != stamp)
                    throw new CollectionModifiedException();

#if REFBUCKET
        if (b == null || b.overflow == null)
        {
          do
          {
            if (++index >= len) yield break;
          } while (table[index] == null);

          b = table[index];
          yield return b.item;
        }
        else
        {
          b = b.overflow;
          yield return b.item;
        }
#else
                if (ob != null && ob.overflow != null)
                {
                    ob = ob.overflow;
                    yield return ob.item;
                }
                else if (index >= 0 && ob == null && (ob = table[index].overflow) != null)
                {
                    yield return ob.item;
                }
                else
                {
                    do
                    {
                        if (++index >= len) yield break;
                    } while (isnull(table[index].item));

                    yield return table[index].item;
                    ob = null;
                }
#endif
            }
#endif
        }

        #endregion

        #region ISink<T> Members
        /// <summary>
        /// Report if this is a set collection.
        /// </summary>
        /// <value>Always false</value>
        
        public virtual bool AllowsDuplicates { get { return false; } }

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting { get { return true; } }

        /// <summary>
        /// Add an item to this set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if item was added (i.e. not found)</returns>
        
        public virtual bool Add(T item)
        {
            updatecheck();
            return !searchoradd(ref item, true, false, true);
        }

        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. Since this
        /// collection has set semantics, only items not already in the collection
        /// will be added.
        /// </summary>
        /// <typeparam name="U">The type of items to add</typeparam>
        /// <param name="items">The items to add</param>
        
        public virtual void AddAll<U>(IEnumerable<U> items) where U : T
        {
            updatecheck();
            bool wasChanged = false;
            bool raiseAdded = (ActiveEvents & EventTypeEnum.Added) != 0;
            CircularQueue<T> wasAdded = raiseAdded ? new CircularQueue<T>() : null;
            foreach (T item in items)
            {
                T jtem = item;

                if (!searchoradd(ref jtem, true, false, false))
                {
                    wasChanged = true;
                    if (raiseAdded)
                        wasAdded.Enqueue(item);
                }
            }
            //TODO: implement a RaiseForAddAll() method
            if (raiseAdded & wasChanged)
                foreach (T item in wasAdded)
                    raiseItemsAdded(item, 1);
            if (((ActiveEvents & EventTypeEnum.Changed) != 0 && wasChanged))
                raiseCollectionChanged();
        }


        #endregion

        #region Diagnostics

        /// <summary>
        /// Test internal structure of data (invariants)
        /// </summary>
        /// <returns>True if pass</returns>
        
        public virtual bool Check()
        {
            int count = 0;
#if LINEARPROBING
      int lasthole = table.Length - 1;

#if REFBUCKET
      while (lasthole >= 0 && table[lasthole] != null)
#else
      while (lasthole >= 0 && !isnull(table[lasthole].item))
#endif
      {
        lasthole--;
        count++;
      }

      if (lasthole < 0)
      {
        Console.WriteLine("Table is completely filled!");
        return false;
      }

      for (int cellindex = lasthole + 1, s = table.Length; cellindex < s; cellindex++)
      {
        Bucket b = table[cellindex];
        int hashindex = hv2i(b.hashval);

        if (hashindex <= lasthole || hashindex > cellindex)
        {
          Console.WriteLine("Bad cell item={0}, hashval={1}, hashindex={2}, cellindex={3}, lasthole={4}", b.item, b.hashval, hashindex, cellindex, lasthole);
          return false;
        }
      }

      int latesthole = -1;

      for (int cellindex = 0; cellindex < lasthole; cellindex++)
      {
        Bucket b = table[cellindex];

#if REFBUCKET
        if (b != null)
#else
        if (!isnull(b.item))
#endif
        {
          count++;

          int hashindex = hv2i(b.hashval);

          if (cellindex < hashindex && hashindex <= lasthole)
          {
            Console.WriteLine("Bad cell item={0}, hashval={1}, hashindex={2}, cellindex={3}, latesthole={4}", b.item, b.hashval, hashindex, cellindex, latesthole);
            return false;
          }
        }
        else
        {
          latesthole = cellindex;
          break;
        }
      }

      for (int cellindex = latesthole + 1; cellindex < lasthole; cellindex++)
      {
        Bucket b = table[cellindex];

#if REFBUCKET
        if (b != null)
#else
        if (!isnull(b.item))
#endif
        {
          count++;

          int hashindex = hv2i(b.hashval);

          if (hashindex <= latesthole || cellindex < hashindex)
          {
            Console.WriteLine("Bad cell item={0}, hashval={1}, hashindex={2}, cellindex={3}, latesthole={4}", b.item, b.hashval, hashindex, cellindex, latesthole);
            return false;
          }
        }
        else
        {
          latesthole = cellindex;
        }
      }

      return true;
#else
            bool retval = true;
            for (int i = 0, s = table.Length; i < s; i++)
            {
                int level = 0;
                Bucket b = table[i];
#if REFBUCKET
        while (b != null)
        {
          if (i != hv2i(b.hashval))
          {
            Console.WriteLine("Bad cell item={0}, hashval={1}, index={2}, level={3}", b.item, b.hashval, i, level);
            retval = false;
          }

          count++;
          level++;
          b = b.overflow;
        }
#else
                if (!isnull(b.item))
                {
                    count++;
                    if (i != hv2i(b.hashval))
                    {
                        Console.WriteLine("Bad cell item={0}, hashval={1}, index={2}, level={3}", b.item, b.hashval, i, level);
                        retval = false;
                    }

                    OverflowBucket ob = b.overflow;

                    while (ob != null)
                    {
                        level++;
                        count++;
                        if (i != hv2i(ob.hashval))
                        {
                            Console.WriteLine("Bad cell item={0}, hashval={1}, index={2}, level={3}", b.item, b.hashval, i, level);
                            retval = false;
                        }

                        ob = ob.overflow;
                    }
                }
#endif
            }

            if (count != size)
            {
                Console.WriteLine("size({0}) != count({1})", size, count);
                retval = false;
            }

            return retval;
#endif
        }


      
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Make a shallow copy of this HashSet.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            HashSet<T> clone = new HashSet<T>(size > 0 ? size : 1, itemequalityComparer);
            //TODO: make sure this really adds in the counting bag way!
            clone.AddAll(this);
            return clone;
        }

        #endregion

    }
}



using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    /// <summary>
    /// A base class for implementing a dictionary based on a set collection implementation.
    /// <i>See the source code for <see cref="T:C5.HashDictionary`2"/> for an example</i>
    /// 
    /// </summary>
    [Serializable]
    public abstract  class DictionaryBase<K, V> : CollectionBase<KeyValuePair<K, V>>, IDictionary<K, V>
    {
        /// <summary>
        /// The set collection of entries underlying this dictionary implementation
        /// </summary>
        protected ICollection<KeyValuePair<K, V>> pairs;

        IEqualityComparer<K> keyequalityComparer;

        #region Events
       
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyequalityComparer"></param>
        public DictionaryBase(IEqualityComparer<K> keyequalityComparer)
        {
            if (keyequalityComparer == null)
                throw new NullReferenceException("Key equality comparer cannot be null");
            this.keyequalityComparer = keyequalityComparer;
        }
      

        #region IDictionary<K,V> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual IEqualityComparer<K> EqualityComparer { get { return keyequalityComparer; } }


        /// <summary>
        /// Add a new (key, value) pair (a mapping) to the dictionary.
        /// </summary>
        /// <exception cref="InconsistencyException"> if there already is an entry with the same key. </exception>
        /// <param name="key">Key to add</param>
        /// <param name="value">Value to add</param>
     
        public virtual void Add(K key, V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);
            if(pairs.Contains(p))           
                throw new InconsistencyException("Key being added: '" + key + "'");
        }

        /// <summary>
        /// Add the entries from a collection of <see cref="T:C5.KeyValuePair`2"/> pairs to this dictionary.
        /// <para><b>TODO: add restrictions L:K and W:V when the .Net SDK allows it </b></para>
        /// </summary>
        /// <exception cref="InconsistencyException"> 
        /// If the input contains duplicate keys or a key already present in this dictionary.</exception>
        /// <param name="entries"></param>
        public virtual void AddAll<L, W>(IEnumerable<KeyValuePair<L, W>> entries)
            where L : K
            where W : V
        {
            foreach (KeyValuePair<L, W> pair in entries)
            {
                KeyValuePair<K, V> p = new KeyValuePair<K, V>(pair.Key, pair.Value);
                if (pairs.Contains(p))
                    throw new InconsistencyException("Key being added: '" + pair.Key + "'");
            }
        }

        /// <summary>
        /// Remove an entry with a given key from the dictionary
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <returns>True if an entry was found (and removed)</returns>
  
        public virtual bool Remove(K key)
        {
            V v;

              
            if (this.Find(key, out v))
            {
                pairs.Remove(new KeyValuePair<K, V>(key, v));                
                return true;
            }
            return false;
        }

        public virtual bool Find(K key, out V value)
        {
            Predicate<KeyValuePair<K, V>> predicate = delegate(KeyValuePair<K, V> pair)
            {
                if (pair.Key.Equals(key))
                    return true;
                else
                    return false;
            };
             
            KeyValuePair<K,V> p=   base.Find(predicate);
            if (!p.Equals(default(KeyValuePair<K,V>)))
            {
                value = p.Value;
                return true;
            }
            else
            {
                value = default(V);
                return false;
            }
        }


        /// <summary>
        /// Remove an entry with a given key from the dictionary and report its value.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <param name="value">On exit, the value of the removed entry</param>
        /// <returns>True if an entry was found (and removed)</returns>
  
        public virtual bool Remove(K key, out V value)
        {
            //KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);
            V v;
            

            if (this.Find(key, out v))
            {
                pairs.Remove(new KeyValuePair<K, V>(key, v));
                value = v;
                return true;
            }
            else
            {
                value = default(V);
                return false;
            }
        }


        /// <summary>
        /// Remove all entries from the dictionary
        /// </summary>

        public virtual void Clear() { pairs.Clear(); }



        /// <summary>
        /// Check if there is an entry with a specified key
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>True if key was found</returns>
        
        public virtual bool Contains(K key)
        {

            return Keys.Contains(key);
        }

     

     

      


        /// <summary>
        /// Look for a specific key in the dictionary and if found replace the value with a new one.
        /// This can be seen as a non-adding version of "this[key] = val".
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="value">The new value</param>
        /// <returns>True if key was found</returns>
        
        public virtual bool Update(K key, V value)
        {
            if (Keys.Contains(key))
            {
                this[key] = value;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="oldvalue"></param>
        /// <returns></returns>
        public virtual bool Update(K key, V value, out V oldvalue)
        {
            if (Keys.Contains(key))
            {

                oldvalue = this[key];
                this[key] = value;
                return true;
            }
            oldvalue = default(V);
            return false;
        }


        /// <summary>
        /// Look for a specific key in the dictionary. If found, report the corresponding value,
        /// else add an entry with the key and the supplied value.
        /// </summary>
        /// <param name="key">On entry the key to look for</param>
        /// <param name="value">On entry the value to add if the key is not found.
        /// On exit the value found if any.</param>
        /// <returns>True if key was found</returns>        
        public virtual bool FindOrAdd(K key, ref V value)
        {

            if (Keys.Contains(key))
            {
                value = this[key] ;
                return true;
            }
            else
            {
                this.Add(key, value);                                
                return false;
            }
        }


        /// <summary>
        /// Update value in dictionary corresponding to key if found, else add new entry.
        /// More general than "this[key] = val;" by reporting if key was found.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="value">The value to add or replace with.</param>
        /// <returns>True if entry was updated.</returns>
        
        public virtual bool UpdateOrAdd(K key, V value)
        {
            if (Keys.Contains(key))
            {
                this[key] = value;
                return true;
            }
            else
            {
                this.Add(key, value);
                return false;
            }
        }


        



        #region Keys,Values support classes

        internal class ValuesCollection : CollectionBase<V>, ICollectionBase<V>
        {
            ICollection<KeyValuePair<K, V>> pairs;


            internal ValuesCollection(ICollection<KeyValuePair<K, V>> pairs)
            { this.pairs = pairs; }


         

            
            public override IEnumerator<V> GetEnumerator()
            {
                //Updatecheck is performed by the pairs enumerator
                foreach (KeyValuePair<K, V> p in pairs)
                    yield return p.Value;
            }

            public override bool IsEmpty { get { return pairs.Count==0; } }

            
            public override int Count { get { return pairs.Count; } }

        }



        internal class KeysCollection : CollectionBase<K>, ICollectionBase<K>
        {
            ICollection<KeyValuePair<K, V>> pairs;


            internal KeysCollection(ICollection<KeyValuePair<K, V>> pairs)
            { this.pairs = pairs; }

        

            
            public override IEnumerator<K> GetEnumerator()
            {
                foreach (KeyValuePair<K, V> p in pairs)
                    yield return p.Key;
            }

            public override bool IsEmpty { get { return pairs.Count==0; } }

            
            public override int Count { get { return pairs.Count; } }

           
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containg the all the keys of the dictionary</value>
        
        public virtual ICollection<K> Keys { get { return new KeysCollection(pairs); } }


        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containing all the values of the dictionary</value>
        
        public virtual ICollection<V> Values { get { return new ValuesCollection(pairs); } }

        

        /// <summary>
        /// Indexer by key for dictionary. 
        /// <para>The get method will throw an exception if no entry is found. </para>
        /// <para>The set method behaves like <see cref="M:C5.DictionaryBase`2.UpdateOrAdd(`0,`1)"/>.</para>
        /// </summary>
        /// <exception cref="NoSuchItemException"> On get if no entry is found. </exception>
        /// <value>The value corresponding to the key</value>
        
        public virtual V this[K key]
        {
            
            get
            {

                V v;
                if (Find(key, out v))
                    return v;
                else
                    throw new InconsistencyException("Key '" + key.ToString() + "' not present in Dictionary");
            }
            
            set
            {
                UpdateOrAdd(key, value);
            
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value>True if dictionary is read  only</value>
        
        public virtual bool IsReadOnly { get { return pairs.IsReadOnly; } }


        

        #endregion

        #region ICollectionBase<KeyValuePair<K,V>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is empty.</value>
        public override bool IsEmpty { get { return Keys.Count==0; } }


        /// <summary>
        /// 
        /// </summary>
        /// <value>The number of entrues in the dictionary</value>
        
        public override int Count { get { return pairs.Count; } }

        /// <summary>
        /// 
        /// </summary>
        /// <value>The number of entrues in the dictionary</value>
        


        /// <summary>
        /// Create an enumerator for the collection of entries of the dictionary
        /// </summary>
        /// <returns>The enumerator</returns>
        
        public override IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return pairs.GetEnumerator(); ;
        }



        #endregion

        public bool TryGetValue(K key, out V value)
        {
            throw new  NotImplementedException("Is this really a useful method for you?");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public override bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            return Showing.ShowDictionary<K, V>(this, stringbuilder, ref rest, formatProvider);
        }


        public abstract object Clone();


        public bool ContainsKey(K key)
        {
            return Keys.Contains(key);
        }

      
    }
}

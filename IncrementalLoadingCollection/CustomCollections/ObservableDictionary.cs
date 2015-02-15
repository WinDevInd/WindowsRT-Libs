//----------------------------------------------------------------------------------------------
// <copyright file="ObservableList.cs" company="TJInnoation" Owner="Jaykumar K Daftary">
// Copyright (c) TJInnoation.  All rights reserved.
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace TJI.IL.Collection.CustomCollections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;


    /// <summary>
    /// List with INotifyCollectionChanged implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableDictionary<K,T> : Dictionary<K,T>, INotifyCollectionChanged
    {
        private List<K> keysAdded;

        public ObservableDictionary()
        {
            this.keysAdded = new List<K>();
        }

        ~ObservableDictionary()
        {
            if(this.keysAdded!=null)
            {
                keysAdded.Clear();
                keysAdded = null;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Observable Dictionary destroyed >>>>>>>>>>>>>>>>>>>>");
#endif
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Add item to dictionary
        /// </summary>
        /// <param name="item">K key ,T Item to add</param>
        public new void Add(K key, T item)
        {
            base.Add(key,item);
            object newItem = this[key];
            keysAdded.Add(key);
            RaiseCollectionChanged(CollectionChangedAction.Added, newItem, this.Count - 1);
        }       

        /// <summary>
        /// Remove item from dictionary
        /// </summary>
        /// <param name="item">K Key to remove</param>
        public new bool Remove(K key)
        {
            var deletedItem = this[key];
            int index = keysAdded.IndexOf(key);
            if (base.Remove(key))
            {
                keysAdded.Remove(key);
                RaiseCollectionChanged(CollectionChangedAction.Removed, deletedItem, index);
                return true;
            }
            return false;
        }       

        /// <summary>
        /// Replace the item at spacified index which raise the Collection change event
        /// </summary>
        /// <param name="key">key value of item</param>
        /// <param name="item">New item</param>
        public void Replace(K key, T item)
        {
            object oldItem = this[key];
            int index = keysAdded.IndexOf(key);
            this[key] = item;            
            RaiseCollectionChanged(CollectionChangedAction.Replaced, oldItem, item, index);
        }

        /// <summary>
        /// Clear the dictionary
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            keysAdded.Clear();
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }


        /// <summary>
        /// Common Method which raise dictionary chaged
        /// </summary>
        /// <param name="action">CollectionChangedAction</param>
        private void RaiseCollectionChanged(CollectionChangedAction action)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            switch (action)
            {
                case CollectionChangedAction.Changed:
                case CollectionChangedAction.Reset:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;                
            }
        }

        private void RaiseCollectionChanged(CollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            switch (action)
            {
                case CollectionChangedAction.Added:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                    break;
                case CollectionChangedAction.Removed:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                    break;                
            }
        }

        private void RaiseCollectionChanged(CollectionChangedAction action, object oldItem, object newItem, int index)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            switch (action)
            {
                case CollectionChangedAction.Replaced:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
                    break;
            }

        }
    }
}

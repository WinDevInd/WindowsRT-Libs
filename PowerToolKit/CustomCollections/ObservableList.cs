//----------------------------------------------------------------------------------------------
// <copyright file="ObservableList.cs" company="JISoft" Owner="Jaykumar K Daftary">
// Copyright (c) TJInnoation.  All rights reserved.
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Collections.CustomCollections
{    
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;


    /// <summary>
    /// List with INotifyCollectionChanged implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableList<T> : List<T>, INotifyCollectionChanged
    {      
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Add item to List
        /// </summary>
        /// <param name="item">T Item to add</param>
        public new void Add(T item)
        {
            base.Add(item);
            RaiseCollectionChanged(CollectionChangedAction.Added, item, this.Count - 1);
        }

        /// <summary>
        /// Add item to List
        /// </summary>
        /// <param name="item">Collection of T Item</param>
        public new void AddRange(IEnumerable<T> item)
        {
            base.AddRange(item);
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }

        /// <summary>
        /// Remove item from collection
        /// </summary>
        /// <param name="item">T item to remove</param>
        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                RaiseCollectionChanged(CollectionChangedAction.Removed, item, IndexOf(item));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove item at given index
        /// </summary>
        /// <param name="index">index of item to remove</param>
        public new void RemoveAt(int index)
        {
            object item = this[index];
            base.RemoveAt(index);
            RaiseCollectionChanged(CollectionChangedAction.Removed, item, index);
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified
        /// predicate.
        /// </summary>
        /// <param name="matches">The number of elements removed from the System.Collections.Generic.List<T></param>
        public new int RemoveAll(Predicate<T> items)
        {
            int removedItem = base.RemoveAll(items);
            RaiseCollectionChanged(CollectionChangedAction.Reset);
            return removedItem;
        }

        /// <summary>
        ///Insert item at specific index 
        /// </summary>
        /// <param name="index">index of item</param>
        /// <param name="item"> item </param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            RaiseCollectionChanged(CollectionChangedAction.Added, item, this.Count - 1);
        }

        /// <summary>
        /// Insert collection from spacifid index
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="items">collection</param>
        public new void InsertRange(int index, IEnumerable<T> items)
        {
            base.InsertRange(index, items);
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }

        /// <summary>
        /// Revese the collection
        /// </summary>
        public new void Reverse()
        {
            base.Reverse();
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }

        /// <summary>
        /// Reverse the collection with specified range
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="count">count</param>
        public new void Reverse(int start, int count)
        {
            base.Reverse(start, count);
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }
        
        /// <summary>
        /// Replace the item at spacified index which raise the Collection change event
        /// </summary>
        /// <param name="index">index of item</param>
        /// <param name="item">New item</param>
        public void Replace(int index, T item)
        {
            object oldItem = this[index];
            this[index] = item;
            RaiseCollectionChanged(CollectionChangedAction.Replaced, oldItem, item, index);
        }

        /// <summary>
        /// Clear the collection
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            RaiseCollectionChanged(CollectionChangedAction.Reset);
        }


        /// <summary>
        /// Common Method which raise collection chaged
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

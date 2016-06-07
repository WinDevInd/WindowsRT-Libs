//----------------------------------------------------------------------------------------------
// <copyright file="IncrementalLoadingCollection.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Collections.ILCollections
{
    using PowerToolKit.CustomCollections;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// ObservableCollection with Incremental loading support
    /// </summary>
    /// <typeparam name="T">Expecting type</typeparam>
    public abstract class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoadingExtended
    {
        /// <summary>
        /// Indicating whether thread is busy operation.
        /// </summary>
        private bool isBusy;

        #region ISupportIncrementalLoading

        /// <summary>
        /// Gets a value indicating whether Collection has more item
        /// </summary>
        public bool HasMoreItems
        {
            get { return this.CanLoadMoreItems(); }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        /// <summary>
        /// Wrapper method - Load more item asynchronously
        /// </summary>
        /// <param name="count">No of item to load at a time</param>
        /// <returns>The wrapped results of the load operation</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (this.isBusy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }
            else
            {
                this.isBusy = true;
                return AsyncInfo.Run((c) => this.LoadMoreItemsAsync((int)count));
            }
        }

        #endregion       

        #region Overridables method

        /// <summary>
        ///     Overridable method - Load more item asynchronously
        /// </summary>
        /// <param name="count">No of item to load at a time</param>
        /// <returns>The wrapped results of the load operation</returns>        
        protected abstract Task<int> LoadNextItemsAsync(int count);

        /// <summary>
        /// Overridable Method - Gets a value indicating whether Collection can load more item
        /// </summary>
        /// <returns>Returns the value indicating whether collection can load more item</returns>
        protected abstract bool CanLoadMoreItems();        

        #endregion

        #region private Methods

        /// <summary>
        /// Load more item asynchronously
        /// </summary>
        /// <param name="count">No of item to load at a time</param>
        /// <returns>The wrapped results of the load operation</returns>
        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(int count)
        {
            try
            {
                var items = await this.LoadNextItemsAsync(count);                              
                return new LoadMoreItemsResult { Count = (uint)items };
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.isBusy = false;
            }
        }

        #endregion
    }
}

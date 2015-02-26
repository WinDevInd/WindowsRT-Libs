//----------------------------------------------------------------------------------------------
// <copyright file="JFlipView.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.FlipView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation.Metadata;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    [Version(1)]
    [WebHostHidden]
    public class JFlipView : Windows.UI.Xaml.Controls.FlipView, IDisposable
    {
        private bool _disposed;
        public event EventHandler onItemPropertyChanged;

        /// <summary>
        /// Represents a control that enables a user to select an item from a collection of items.
        ///  Note :
        ///      Use Dispose method during unloading of Element when Setting IncrementalLoadingTrigger value "Edge" 
        ///      default value is Edge - to support IncrementalLoading of data
        /// </summary>
        public JFlipView()
        {
            this.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            this.DataFetchSize = 3;
            this.IncrementalLoadingThreshold = 1;
            IsBusy = false;
        }

        ~JFlipView()
        {
            if (!_disposed)
            {
                Dispose(false);
            }

            System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>> JFlipView Destoyed By GC <<<<<<<<<<<<<<<<<<<<<<<<<<");
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (this.Items.Count == 0)
            {
                this.LoadNextItemAsync(false);
            }

            if (this.onItemPropertyChanged != null)
            {
                this.onItemPropertyChanged(this, EventArgs.Empty);
            }
        }

        private bool IsBusy
        {
            get;
            set;
        }

        private IncrementalLoadingTrigger _IncrementalLoadingTrigger;
        /// <summary>
        /// Gets or sets a value that indicates the conditions for prefetch operations
        ///     by the ListViewBase class. Use Dispose method during unloading of Element
        ///     when Setting IncrementalLoadingTrigger value "Edge"
        ///
        /// Returns:
        ///     An enumeration value that indicates the conditions that trigger prefetch
        ///     operations. The default is Edge.
        /// </summary>
        public IncrementalLoadingTrigger IncrementalLoadingTrigger
        {
            get
            {
                return _IncrementalLoadingTrigger;
            }
            set
            {
                if (value != _IncrementalLoadingTrigger)
                {
                    _IncrementalLoadingTrigger = value;
                    switch (value)
                    {
                        case IncrementalLoadingTrigger.Edge:
                            this.SelectionChanged += JFlipView_SelectionChanged;
                            break;
                        case IncrementalLoadingTrigger.None:
                            this.SelectionChanged -= JFlipView_SelectionChanged;
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the amount of data to fetch for virtualizing/prefetch operations.
        ///
        ///  The default value is  3.
        /// </summary>
        public int DataFetchSize
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the threshold range that governs when the ListViewBase class
        ///     will begin to prefetch more items.
        ///
        /// The default value is 1
        /// </summary>
        public int IncrementalLoadingThreshold
        {
            get;
            set;
        }

        private async void JFlipView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            await LoadNextItemAsync();
        }

        // Summary:
        //      Handle the LazyLoading of items
        async private Task LoadNextItemAsync(bool isInitialized = true)
        {
            try
            {
                if (this.ItemsSource != null && CanIncrementalLoadigTrigger())
                {
                    IsBusy = true;
                    ISupportIncrementalLoading incrementalLoadingInterface = this.ItemsSource as ISupportIncrementalLoading;
                    if (incrementalLoadingInterface != null)
                    {
                        if (incrementalLoadingInterface.HasMoreItems)
                        {
                            if (!isInitialized)
                            {
                                await incrementalLoadingInterface.LoadMoreItemsAsync(1);
                                return;
                            }

                            for (int i = 0; i <= this.IncrementalLoadingThreshold; i++)
                            {
                                await incrementalLoadingInterface.LoadMoreItemsAsync((uint)this.DataFetchSize);
                            }
                        }
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Summary:
        //      Identifies when to trigger IncrementalLoading
        private bool CanIncrementalLoadigTrigger()
        {
            if (!IsBusy && IncrementalLoadingTrigger == IncrementalLoadingTrigger.Edge && (this.Items.Count - this.SelectedIndex) <= (DataFetchSize * IncrementalLoadingThreshold))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Dispose managed resource and Event unsubscription 
        ///        
        ///CanThrowException:        
        ///                     True if You need to handle disposing exception othewise False
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception e)
            {
                string Message = "Error occured during dispose " + e.Message;
                throw new Exception(Message, e);

            }
        }

        private void Dispose(bool disposeManagedResource)
        {
            if (disposeManagedResource)
            {
                this.SelectionChanged -= JFlipView_SelectionChanged;
            }
            //Release other resources if availble
            _disposed = true;
        }
    }
}

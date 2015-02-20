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

        // Summary:
        //      Represents a control that enables a user to select an item from a collection
        //      of items.
        //  Note :
        //      Use Dispose method during unloading of Element when Setting IncrementalLoadingTrigger value "Edge" 
        //      default value is Edge - to support IncrementalLoading of data
        //
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
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (this.Items.Count == 0)
            {
                this.LoadNextItemAsync(false);
            }
        }

        private bool IsBusy
        {
            get;
            set;
        }
        //
        // Summary:
        //     Gets or sets a value that indicates the conditions for prefetch operations
        //     by the ListViewBase class. Use Dispose method during unloading of Element
        //     when Setting IncrementalLoadingTrigger value "Edge"
        //
        // Returns:
        //     An enumeration value that indicates the conditions that trigger prefetch
        //     operations. The default is Edge.
        private IncrementalLoadingTrigger _IncrementalLoadingTrigger;
        public IncrementalLoadingTrigger IncrementalLoadingTrigger
        {
            get
            {
                return _IncrementalLoadingTrigger;
            }
            set
            {
                if(value!=_IncrementalLoadingTrigger)
                {
                    _IncrementalLoadingTrigger = value;
                    switch(value)
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

        //
        // Summary:
        //     Gets or sets the amount of data to fetch for virtualizing/prefetch operations.
        //
        // Returns:
        //     The amount of data to fetch per interval, in pages. The default is 3.
        public double DataFetchSize
        {
            get;
            set;
        }

        // Summary:
        //     Gets or sets the threshold range that governs when the ListViewBase class
        //     will begin to prefetch more items.
        //
        // Returns:
        //     The loading threshold, in terms of pages. The default is 1
        public double IncrementalLoadingThreshold
        {
            get;
            set;
        }

        // Summary:
        //     Occurs when the currently selected item changes.
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

        // Summary:
        //     Dispose managed resource and Event unsubscription 
        //
        // Parameters:
        //      canThrowException:        
        //                     True if You need to handle disposing exception othewise False
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

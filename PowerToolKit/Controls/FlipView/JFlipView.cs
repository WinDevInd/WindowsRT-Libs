//----------------------------------------------------------------------------------------------
// <copyright file="JFlipView.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Controls
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
    public class JFlipView : Windows.UI.Xaml.Controls.FlipView
    {
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

        /// <summary>
        /// Gets or set if Flipview is busy in adding more item to item source
        /// Can be identified if flipview is busy with incremental loading of data
        /// </summary>
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

        /// <summary>
        /// Selection changed - to identify if flipview reached to end of collection - to trigger Incremental loading
        /// </summary>
        /// <param name="sender">Flipview</param>
        /// <param name="e">Flipview selection changed event arguments</param>
        private async void JFlipView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            await LoadNextItemAsync();
        }

        /// <summary>
        ///  Handle the LazyLoading of items
        /// </summary>
        /// <param name="isInitialized">Is called with Flipview's Initialization</param>
        /// <returns>Task (void)</returns>

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

                            for (int i = 1; i <= this.IncrementalLoadingThreshold; i++)
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
            if (!IsBusy && IncrementalLoadingTrigger == IncrementalLoadingTrigger.Edge
                && (this.Items.Count - this.SelectedIndex) <= (DataFetchSize * IncrementalLoadingThreshold))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            try
            {
                this.SelectionChanged -= JFlipView_SelectionChanged;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception occurend in Flipview Dispose " + e.Message + " Stack trace : " + e.StackTrace + " - Please report this to developer on nuget");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

//----------------------------------------------------------------------------------------------
// <copyright file="JFlipView.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Controls
{
    using PowerToolKit.CustomCollections;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation.Metadata;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    public class JFlipView : Windows.UI.Xaml.Controls.FlipView
    {
        public event EventHandler onItemPropertyChanged;
        private DispatcherTimer timer;
        private bool shouldResumeSlideShow;
        private bool isSlideShowPlayingInitid = false;

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
            this.Loaded += JFlipView_Loaded;
            this.Unloaded += JFlipView_Unloaded;

            this.timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            this.SlideShowTimeSpan = TimeSpan.FromSeconds(30.00);
            this.shouldResumeSlideShow = false;
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
                this.IsSlideShowPlaying = shouldResumeSlideShow;

            }

            if (this.onItemPropertyChanged != null)
            {
                this.onItemPropertyChanged(this, EventArgs.Empty);
            }
        }



        private TimeSpan _SlideShowTimeSpan;
        /// <summary>
        /// Gets or set the Slideshow play duration
        /// </summary>
        public TimeSpan SlideShowTimeSpan
        {
            get
            {
                return _SlideShowTimeSpan;
            }
            set
            {
                _SlideShowTimeSpan = value;
                timer.Interval = value;
            }
        }

        private bool _IsSlideShowPlaying;
        /// <summary>
        /// Gets or set the SlideshowPlaying true/false to control automatic-slideshow
        /// </summary>
        public bool IsSlideShowPlaying
        {
            get
            {
                return (_IsSlideShowPlaying || isSlideShowPlayingInitid);
            }
            set
            {
                OnSlideShowPlayingChanged(value);
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
                    ISupportIncrementalLoadingExtended incrementalLoadingInterface = this.ItemsSource as ISupportIncrementalLoadingExtended;
                    if (incrementalLoadingInterface != null && !incrementalLoadingInterface.IsBusy)
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
            if (!IsBusy && this.Items != null && IncrementalLoadingTrigger == IncrementalLoadingTrigger.Edge
                && (this.Items.Count - this.SelectedIndex) <= (DataFetchSize * IncrementalLoadingThreshold))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Slideshow playing property changed callback
        /// </summary>
        /// <param name="value">true or false - IsSlideshowRunning</param>
        private void OnSlideShowPlayingChanged(bool value)
        {
            _IsSlideShowPlaying = value;
            if (value)
            {
                if (this.Items != null)
                {
                    this.Play();
                }
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        ///  Start the slideshow
        ///     Slideshow interval will be referd from SlideShowTimeSpan property
        ///     Default value of SlideShowTimeSpan is 30 seconds
        /// </summary>
        private void Play()
        {
            if (isSlideShowPlayingInitid)
            {
                return;
            }
            timer.Interval = this.SlideShowTimeSpan;
            timer.Start();
            isSlideShowPlayingInitid = true;
        }

        /// <summary>
        ///  Pause the slideshow
        ///  This method will remove the timer for slideshow
        /// </summary>
        private void Stop()
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, object e)
        {
            if (this.Items != null && Items.Count > 1)
            {
                if (this.SelectedIndex == this.Items.Count - 1)
                {
                    this.SelectedIndex = 0;
                }
                else
                {
                    this.SelectedIndex += 1;
                }
            }
        }

        void JFlipView_Unloaded(object sender, RoutedEventArgs e)
        {
            shouldResumeSlideShow = IsSlideShowPlaying;
            isSlideShowPlayingInitid = false;
            timer.Stop();
        }

        void JFlipView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!shouldResumeSlideShow)
            {
                return;
            }
            isSlideShowPlayingInitid = shouldResumeSlideShow;
            IsSlideShowPlaying = shouldResumeSlideShow;
        }

        public void Dispose()
        {
            try
            {
                this.SelectionChanged -= JFlipView_SelectionChanged;
                this.Loaded -= JFlipView_Loaded;
                this.Unloaded -= JFlipView_Unloaded;
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

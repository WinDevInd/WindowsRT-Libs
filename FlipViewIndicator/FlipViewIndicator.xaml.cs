//----------------------------------------------------------------------------------------------
// <copyright file="PaginationIndicator.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Pagination
{
    using JISoft.Collections.CustomCollections;
    using JISoft.FlipView;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class FlipViewIndicator : UserControl, IDisposable
    {
        private JFlipView IndicatorProvider;
        private int previousSelectedIndex;        
        private DispatcherTimer timer;
        private bool shouldResumeSlideShow;


        public FlipViewIndicator()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        ~FlipViewIndicator()
        {
            System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>> FlipView Indicator disposed <<<<<<<<<<<<<<<<<");
        }

        /// <summary>
        /// Initialize default values and new refrences for properties
        /// </summary>
        private void Initialize()
        {
            this.IndicatorStyle = ListViewIndicator.Style;
            this.previousSelectedIndex = 0;
            this.IndicatorSelector = new IndicatorSelector();
            this.IndicatorItemSource = new ObservableList<TemplateChooser>();            
            this.timer = new DispatcherTimer();
            this.SlideShowTimeSpan = TimeSpan.FromSeconds(30.00);
            shouldResumeSlideShow = false;
        }

        #region Dependency and Public Property

        /// <summary>
        ///  Gets or set Margin for Indicators
        /// </summary>
        public Thickness IndicatorMargin
        {
            get { return (Thickness)GetValue(IndicatorMarginProperty); }
            set { SetValue(IndicatorMarginProperty, value); }
        }

        /// <summary>
        /// Gets or set Style for Horizontal ListView (Indicator List)
        /// </summary>
        public Style IndicatorStyle
        {
            get { return (Style)GetValue(IndicatorStyleProperty); }
            set { SetValue(IndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Gets or set Padding for ItemTemplate in Indicator
        /// </summary>
        public Thickness IndicatorPadding
        {
            get { return (Thickness)GetValue(IndicatorPaddingProperty); }
            set { SetValue(IndicatorPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or set JISoft.FlipView.JFlipView as a Provider for Itemsource and other things to handle complete working        
        /// </summary>
        public object PaginationProvider
        {
            get { return (object)GetValue(PaginationProviderProperty); }
            set { SetValue(PaginationProviderProperty, value); }
        }

        /// <summary>
        /// Gets or set yhe IndicatorList background
        /// </summary>
        public Brush IndicatorBackground
        {
            get { return (Brush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or set the Indicator panel backgrund
        /// </summary>
        public Brush ItemsPanelBackground
        {
            get { return (Brush)GetValue(ItemsPanelBackgroundProperty); }
            set { SetValue(ItemsPanelBackgroundProperty, value); }
        }

        #endregion

        #region Private Properties

        /// <summary> 
        ///     Gets the ItemTemplateSelector use for Indicator
        ///<summary> 
        public IndicatorSelector IndicatorSelector
        {
            get;
            private set;
        }

        /// <summary> 
        ///     Gets the ItemSource as Collection which is used for Indicator
        public ObservableList<TemplateChooser> IndicatorItemSource
        {
            get;
            private set;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or set the SelectedItemTemplate - Used as selected item's data template
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get
            {
                return this.IndicatorSelector.SelectedItemTemplate;
            }
            set
            {
                this.IndicatorSelector.SelectedItemTemplate = value;
            }
        }

        /// <summary>
        /// Gets or set the UnSelectedItemTemplate - Used as non-selected item's data template
        /// </summary>
        public DataTemplate UnSelectedItemTemplate
        {
            get
            {
                return this.IndicatorSelector.UnSelectedItemTemplate;
            }
            set
            {
                this.IndicatorSelector.UnSelectedItemTemplate = value;
            }
        }

        /// <summary>
        /// Gets or set the Slideshow play duration
        /// </summary>
        public TimeSpan SlideShowTimeSpan
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets the Status of Slideshow running
        /// </summary>
        public bool IsSlideShowRunning
        {
            get;
            private set;
        }

        #endregion

        #region DependencyProperty Declaration

        public static readonly DependencyProperty IndicatorMarginProperty =
            DependencyProperty.Register("IndicatorMargin", typeof(Thickness), typeof(FlipViewIndicator), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(Style), typeof(FlipViewIndicator), new PropertyMetadata(null, onIndicatorStyleChanged));

        public static readonly DependencyProperty IndicatorPaddingProperty =
            DependencyProperty.Register("IndicatorPadding", typeof(Thickness), typeof(FlipViewIndicator), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty PaginationProviderProperty =
            DependencyProperty.Register("PaginationProvider", typeof(object), typeof(FlipViewIndicator), new PropertyMetadata(null, onIndicatorProviderChanged));

        public static readonly DependencyProperty IndicatorBackgroundProperty =
           DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(FlipViewIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register("ItemsPanelBackground", typeof(Brush), typeof(FlipViewIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        #endregion

        #region PropertyChange Call backs
        /// <summary> 
        ///     Call back -  when PagnationProvider changed
        /// <summary> 
        private static void onIndicatorProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlipViewIndicator indicatorControl = d as FlipViewIndicator;
            if (indicatorControl != null)
            {
                if (indicatorControl.IndicatorProvider == null)
                {
                    JFlipView flipView = e.NewValue as JFlipView;
                    indicatorControl.SubscribeToFlipViewEvents(ref flipView);
                }
            }
        }

        /// <summary> 
        ///     Call back -  when Listview style changed
        private static void onIndicatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Not thought about it yet :P
        }

        #endregion

        #region PageinationProvider Flipview related code

        private void SubscribeToFlipViewEvents(ref JFlipView flipView)
        {
            if (this.IndicatorProvider == null)
            {
                flipView.onItemPropertyChanged += IndicatorProviderControl_onItemPropertyChanged;
                flipView.SelectionChanged += flipView_SelectionChanged;
                IndicatorProvider = flipView;
            }

            SetIndicatorListItemSource(flipView);
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            JFlipView provider = sender as JFlipView;
            int selectedIndex = provider.SelectedIndex;
            this.ListViewIndicator.SelectedIndex = selectedIndex;
            this.ListViewIndicator.ScrollIntoView(ListViewIndicator.Items[selectedIndex]);
        }

        private void IndicatorProviderControl_onItemPropertyChanged(object sender, EventArgs e)
        {
            JFlipView flipView = sender as JFlipView;
            SetIndicatorListItemSource(flipView);
        }

        #endregion

        # region Events for ListView

        private void ListViewIndicator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewIndicator.SelectedIndex == -1)
            {
                return;
            }
            if (this.IndicatorProvider.SelectedIndex != ListViewIndicator.SelectedIndex)
            {
                IndicatorProvider.SelectedIndex = ListViewIndicator.SelectedIndex;
            }
            IndicatorItemSource.Replace(previousSelectedIndex, new TemplateChooser() { IsSelected = false });
            IndicatorItemSource.Replace(IndicatorProvider.SelectedIndex, new TemplateChooser() { IsSelected = true });
            previousSelectedIndex = IndicatorProvider.SelectedIndex;
        }

        #endregion

        #region private Methods and Incontrol events

        private void SetIndicatorListItemSource(JFlipView flipView)
        {
            for (int i = IndicatorItemSource.Count; i <= flipView.Items.Count - 1; i++)
            {
                IndicatorItemSource.Insert(IndicatorItemSource.Count, new TemplateChooser() { IsSelected = false });
            }

            //// Make first items as selected - manually
            if (flipView.Items.Count > 0 && (flipView.SelectedIndex == -1 || flipView.SelectedIndex == 0))
            {
                IndicatorItemSource.Replace(0, new TemplateChooser() { IsSelected = true });
            }
        }

        private void IndicatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IndicatorProvider != null)
            {                
                if (shouldResumeSlideShow)
                {
                    this.Play();
                }
            }
        }

        private void IndicatorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Pause();
            shouldResumeSlideShow = true;
        }
        #endregion

        #region SlideShow methods

        /// <summary>
        ///  Start the slideshow
        ///     Slideshow interval will be referd from SlideShowTimeSpan property
        ///     Default value of SlideShowTimeSpan is 30 seconds
        /// </summary>
        public void Play()
        {
            if (IsSlideShowRunning || this.ListViewIndicator.Items.Count <= 1)
            {
                return;
            }            
            timer.Interval = this.SlideShowTimeSpan;            
            IsSlideShowRunning = true;
            timer.Start();
        }

        /// <summary>
        ///  Pause the slideshow
        ///  This method will remove the timer for slideshow
        /// </summary>
        public void Pause()
        {            
            timer.Stop();
            IsSlideShowRunning = false;
        }

        /// <summary>        
        ///     Stop and reset the slideshow
        ///     This method will reset the flipview index to 0.        
        /// </summary>
        public void Stop()
        {
            
            timer.Stop();
            IsSlideShowRunning = false;
            this.ListViewIndicator.SelectedIndex = 0;
        }

        private void timer_Tick(object sender, object e)
        {
            if (this.IndicatorProvider != null && this.IndicatorProvider.Items.Count > 1)
            {
                if (this.IndicatorProvider.SelectedIndex == this.IndicatorProvider.Items.Count - 1)
                {
                    IndicatorProvider.SelectedIndex = 0;
                    return;
                }
                IndicatorProvider.SelectedIndex += 1;
            }
        }
        #endregion

        /// <summary>
        /// Clear all the resources here - better for Avaoid memory leaks
        /// As we are using the JISoft.FlipView.JFlipView refrence.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool removeAll = false)
        {
            if (this.IndicatorProvider != null)
            {
                this.IndicatorProvider.SelectionChanged -= flipView_SelectionChanged;
                this.IndicatorProvider.onItemPropertyChanged -= IndicatorProviderControl_onItemPropertyChanged;
                this.IndicatorProvider = null;
                this.IndicatorItemSource.Clear();
                timer.Tick -= timer_Tick;
                timer.Stop();
                IsSlideShowRunning = false;
                if (removeAll)
                {
                    this.IndicatorItemSource = null;
                }
            }
        }
    }
}

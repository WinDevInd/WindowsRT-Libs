//----------------------------------------------------------------------------------------------
// <copyright file="JFlipViewIndicator.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace JISoft.Controls
{
    using JISoft.Collections.CustomCollections;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.ApplicationModel;
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

    public sealed partial class JFlipViewIndicator : UserControl, IDisposable
    {
        private DispatcherTimer timer;
        private bool shouldResumeSlideShow;
        private bool isSlideShowPlayingInitid = false;

        public JFlipViewIndicator()
        {
            this.InitializeComponent();
            this.ListViewIndicator.DataContext = this;
            this.Initialize();
        }

        ~JFlipViewIndicator()
        {
            System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>> FlipView Indicator disposed <<<<<<<<<<<<<<<<<");
        }

        protected override void OnApplyTemplate()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Initialize default values and new refrences for properties
        /// </summary>
        private void Initialize()
        {
            // this.IndicatorSelector = new IndicatorSelector();
            this.timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            this.SlideShowTimeSpan = TimeSpan.FromSeconds(30.00);
            this.shouldResumeSlideShow = false;
            this.isSlideShowPlayingInitid = true;
        }

        /// <summary>
        /// Gets the JISoft.Flipview's Instance which is passed in PaginationProvider
        /// </summary>
        public JFlipView IndicatorProvider
        {
            get;
            private set;
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

        /// <summary>
        /// Indicator control's Itemsource for list of slides/bullets
        /// </summary>
        public object IndicatorSource
        {
            get { return (object)GetValue(IndicatorSourceProperty); }
            set { SetValue(IndicatorSourceProperty, value); }
        }


        #endregion

        #region Public Property

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
        /// Gets or set the SelectedItemTemplate - Used as selected item's data template
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set the UnSelectedItemTemplate - Used as non-selected item's data template
        /// </summary>
        public DataTemplate UnSelectedItemTemplate
        {
            get;
            set;
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
            DependencyProperty.Register("IndicatorMargin", typeof(Thickness), typeof(JFlipViewIndicator),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(Style), typeof(JFlipViewIndicator),
            new PropertyMetadata(0));

        public static readonly DependencyProperty IndicatorPaddingProperty =
            DependencyProperty.Register("IndicatorPadding", typeof(Thickness), typeof(JFlipViewIndicator),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty PaginationProviderProperty =
            DependencyProperty.Register("PaginationProvider", typeof(object), typeof(JFlipViewIndicator),
            new PropertyMetadata(null, OnIndicatorProviderChanged));

        public static readonly DependencyProperty IndicatorBackgroundProperty =
           DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(JFlipViewIndicator),
           new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register("ItemsPanelBackground", typeof(Brush), typeof(JFlipViewIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty IndicatorSourceProperty =
           DependencyProperty.Register("IndicatorSource", typeof(object), typeof(JFlipViewIndicator),
           new PropertyMetadata(null, IndicatorItemSourceChanged));


        #endregion

        #region PropertyChange Call backs

        /// <summary>
        /// Callback For Indicator listview item source changed
        /// </summary>
        private static void IndicatorItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JFlipViewIndicator indicatorControl = d as JFlipViewIndicator;
            if (indicatorControl != null)
            {

                if (e.NewValue != null && indicatorControl.IsSlideShowPlaying)
                {
                    indicatorControl.IsSlideShowPlaying = true;
                }
            }
        }

        /// <summary> 
        ///     Call back -  when PagnationProvider changed
        /// <summary> 
        private static void OnIndicatorProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JFlipViewIndicator indicatorControl = d as JFlipViewIndicator;
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
        /// Slideshow playing property changed callback
        /// </summary>
        /// <param name="value">true or false - IsSlideshowRunning</param>
        private void OnSlideShowPlayingChanged(bool value)
        {
            _IsSlideShowPlaying = value;
            if (value)
            {
                if (this.IndicatorSource != null)
                {
                    this.Play();
                }
            }
            else
            {
                this.Stop();
            }
        }

        #endregion

        #region PageinationProvider Flipview related code

        private void SubscribeToFlipViewEvents(ref JFlipView flipView)
        {
            if (flipView == null)
            {
                throw new ArgumentException("Invalid Refrence passed in PaginatinProvider - It must be JISoft.Flipview Element");
            }
            IndicatorProvider = flipView;
            Binding selectedIndexBinding = new Binding();
            selectedIndexBinding.Path = new PropertyPath("SelectedIndex");
            selectedIndexBinding.ElementName = "IndicatorProvider";
            selectedIndexBinding.Mode = BindingMode.TwoWay;
            selectedIndexBinding.Source = IndicatorProvider;
            ListViewIndicator.SetBinding(ListView.SelectedIndexProperty, selectedIndexBinding);
        }

        #endregion

        #region private Methods and Incontrol events

        private void IndicatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IndicatorProvider != null)
            {
                if (shouldResumeSlideShow)
                {
                    this.IsSlideShowPlaying = true;
                }
            }
        }

        private void IndicatorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Stop();
            shouldResumeSlideShow = true;
        }
        #endregion

        #region SlideShow methods

        /// <summary>
        ///  Start the slideshow
        ///     Slideshow interval will be referd from SlideShowTimeSpan property
        ///     Default value of SlideShowTimeSpan is 30 seconds
        /// </summary>
        private void Play()
        {
            if (IsSlideShowRunning)
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
        private void Stop()
        {
            timer.Stop();
            IsSlideShowRunning = false;
        }

        private void timer_Tick(object sender, object e)
        {
            if (this.IndicatorProvider != null && this.IndicatorProvider.Items != null && this.IndicatorProvider.Items.Count > 1)
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
            shouldResumeSlideShow = false;
            IsSlideShowRunning = false;
            timer.Tick -= timer_Tick;
            timer.Stop();
        }

        private void ListViewIndicator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListViewIndicator.Items != null && this.ListViewIndicator.Items.Count > 0)
            {
                int selectedIndex = this.ListViewIndicator.SelectedIndex;
                this.ListViewIndicator.ScrollIntoView(this.ListViewIndicator.Items[selectedIndex]);
            }
        }
    }
}

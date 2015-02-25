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


        public FlipViewIndicator()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        /// Summary:
        ///     Initialize default values and new refrences for properties
        private void Initialize()
        {
            this.IndicatorStyle = ListViewIndicator.Style;
            this.previousSelectedIndex = 0;
            this.IndicatorSelector = new IndicatorSelector();
            IndicatorItemSource = new ObservableList<TemplateChooser>();
        }

        #region Dependency and Public Property

        /// Summary:
        ///     Gets or set Margin for Indicators
        public Thickness IndicatorMargin
        {
            get { return (Thickness)GetValue(IndicatorMarginProperty); }
            set { SetValue(IndicatorMarginProperty, value); }
        }

        /// Summary:
        ///     Gets or set Style for Horizontal ListView (Indicator List)
        public Style IndicatorStyle
        {
            get { return (Style)GetValue(IndicatorStyleProperty); }
            set { SetValue(IndicatorStyleProperty, value); }
        }

        /// Summary:
        ///    Gets or set Padding for ItemTemplate in Indicator
        public Thickness IndicatorPadding
        {
            get { return (Thickness)GetValue(IndicatorPaddingProperty); }
            set { SetValue(IndicatorPaddingProperty, value); }
        }

        /// Summary:
        ///     Gets or set JISoft.FlipView.JFlipView as a Provider for Itemsource and other things to handle complete working
        public object PaginationProvider
        {
            get { return (object)GetValue(PaginationProviderProperty); }
            set { SetValue(PaginationProviderProperty, value); }
        }

        public Brush IndicatorBackground
        {
            get { return (Brush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }

        public static readonly DependencyProperty IndicatorBackgroundProperty =
            DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(FlipViewIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        public Brush ItemsPanelBackground
        {
            get { return (Brush)GetValue(ItemsPanelBackgroundProperty); }
            set { SetValue(ItemsPanelBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPanelBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register("ItemsPanelBackground", typeof(Brush), typeof(FlipViewIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        #endregion

        #region Private Properties

        /// Summary:
        ///     Gets the ItemTemplateSelector use for Indicator
        public IndicatorSelector IndicatorSelector
        {
            get;
            private set;
        }

        /// Summary:
        ///     Gets the ItemSource as Collection which is used for Indicator
        public ObservableList<TemplateChooser> IndicatorItemSource
        {
            get;
            private set;
        }

        #endregion

        #region Public Property

        /// Summary:
        ///     Gets or set the SelectedItemTemplate - Used as selected item's data template
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

        /// Summary:
        ///     Gets or set the UnSelectedItemTemplate - Used as non-selected item's data template
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

        #endregion

        #region DependencyProperty Declaration

        public static readonly DependencyProperty IndicatorMarginProperty =
            DependencyProperty.Register("IndicatorMargin", typeof(Thickness), typeof(FlipViewIndicator), new PropertyMetadata(null));

        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(Style), typeof(FlipViewIndicator), new PropertyMetadata(null, onIndicatorStyleChanged));

        public static readonly DependencyProperty IndicatorPaddingProperty =
            DependencyProperty.Register("IndicatorPadding", typeof(Thickness), typeof(FlipViewIndicator), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty PaginationProviderProperty =
            DependencyProperty.Register("PaginationProvider", typeof(object), typeof(FlipViewIndicator), new PropertyMetadata(null, onIndicatorProviderChanged));

        #endregion

        #region PropertyChange Call backs
        /// Summary:
        ///     Call back -  when PagnationProvider changed
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

        /// Summary:
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

        #region private Methods
        private void SetIndicatorListItemSource(JFlipView flipView)
        {
            for (int i = IndicatorItemSource.Count; i <= flipView.Items.Count - 1; i++)
            {
                IndicatorItemSource.Insert(IndicatorItemSource.Count, new TemplateChooser() { IsSelected = false });
            }

            if (flipView.SelectedIndex == -1 || flipView.SelectedIndex == 0)
            {
                IndicatorItemSource.Replace(0, new TemplateChooser() { IsSelected = true });
            }
        }
        #endregion


        /// Summary:
        ///         Clear all the resources here - better for Avaoid memory leaks
        ///         As we are using the JISoft.FlipView.JFlipView refrence.
        public void Dispose()
        {
            if (this.IndicatorProvider != null)
            {
                this.IndicatorProvider.SelectionChanged -= flipView_SelectionChanged;
                this.IndicatorProvider.onItemPropertyChanged -= IndicatorProviderControl_onItemPropertyChanged;
                this.IndicatorProvider = null;
                this.IndicatorItemSource.Clear();
                this.IndicatorItemSource = null;
            }
        }
    }
}

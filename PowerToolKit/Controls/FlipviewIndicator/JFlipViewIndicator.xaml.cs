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

    public enum ItemsSourceProvider
    {
        PaginationProvider,
        ManualProvider
    }

    public sealed partial class JFlipViewIndicator : UserControl
    {
        public JFlipViewIndicator()
        {
            this.InitializeComponent();
            this.ListViewIndicator.DataContext = this;
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
        /// Gets the JISoft.Flipview's Instance which is passed in PaginationProvider
        /// </summary>
        public object IndicatorProvider
        {
            get;
            private set;
        }

        private ItemsSourceProvider _ItmesSourceProvider;
        public ItemsSourceProvider ItemsSourceProvider
        {
            get
            {
                return _ItmesSourceProvider;
            }
            set
            {
                if (value != _ItmesSourceProvider)
                {
                    _ItmesSourceProvider = value;
                    OnItemSourceProviderChanged(value);
                }
            }

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
        public object IndicatorItemsSource
        {
            get { return (object)GetValue(IndicatorSourceProperty); }
            set { SetValue(IndicatorSourceProperty, value); }
        }


        #endregion

        #region Public Property

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

        public static readonly DependencyProperty IndicatorBackgroundProperty =
           DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(JFlipViewIndicator),
           new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register("ItemsPanelBackground", typeof(Brush), typeof(JFlipViewIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty IndicatorSourceProperty =
           DependencyProperty.Register("IndicatorItemsSource", typeof(object), typeof(JFlipViewIndicator),
           new PropertyMetadata(null,onPropertyChanged));



        public object FlipViewElement
        {
            get { return (object)GetValue(FlipViewElementProperty); }
            set { SetValue(FlipViewElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Flip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlipViewElementProperty =
            DependencyProperty.Register("FlipViewElement", typeof(object), typeof(JFlipViewIndicator), new PropertyMetadata(null, onPropertyChanged));

        private static void onPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JFlipViewIndicator indicator = d as JFlipViewIndicator;
            if (indicator != null)
            {
                if (indicator.FlipViewElement != null)
                {
                    indicator.SubscribeToFlipViewEvents();
                }
            }
        }
        #endregion


        private void OnItemSourceProviderChanged(ItemsSourceProvider provider)
        {
            this.ListViewIndicator.ClearValue(ListView.ItemsSourceProperty);
            Binding itemsSourceBinding = new Binding();

            if (provider == ItemsSourceProvider.PaginationProvider)
            {
                if (this.FlipViewElement != null)
                {
                    itemsSourceBinding.Path = new PropertyPath("ItemsSource");
                    itemsSourceBinding.ElementName = "PaginationProvider";
                    itemsSourceBinding.Source = FlipViewElement;
                }
            }
            else
            {
                itemsSourceBinding.Path = new PropertyPath("IndicatorItemsSource");
                itemsSourceBinding.Source = this;
            }

            ListViewIndicator.SetBinding(ListView.ItemsSourceProperty, itemsSourceBinding);

        }

        private void SubscribeToFlipViewEvents()
        {
            if (FlipViewElement == null)
            {
                throw new ArgumentException("Invalid Refrence passed in PaginatinProvider - It must be JISoft.Flipview Element");
            }
            if (this.ListViewIndicator.Items != null && this.ListViewIndicator.Items.Count > 0)
            {
                Binding selectedIndexBinding = new Binding();
                selectedIndexBinding.Path = new PropertyPath("SelectedIndex");
                selectedIndexBinding.ElementName = "PaginationProvider";
                selectedIndexBinding.Mode = BindingMode.TwoWay;
                selectedIndexBinding.Source = FlipViewElement;
                ListViewIndicator.SetBinding(ListView.SelectedIndexProperty, selectedIndexBinding);
            }
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

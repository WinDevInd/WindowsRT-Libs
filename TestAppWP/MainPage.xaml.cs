using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TestAppWP
{

    public class FKPivotItem : PivotItem
    {
        public FKPivotItem()
        {
            base.DefaultStyleKey = typeof(FKPivotItem);
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public class ImageModel
        {
            public string ImagePath
            {
                get;
                set;
            }
        }
        public List<ImageModel> Images
        {
            get;
            set;
        }

        public MainPage()
        {
            //Images = new List<ImageModel>();
            //for (int i = 2; i <= 9; i++)
            //{
            //    Images.Add(new ImageModel() { ImagePath = "/Assets/WinLogos/" + i + ".jpg" });
            //}
            InitializeComponent();
            //this.flipvw.ItemsSource = Images;

            //this.FlipViewInd.PaginationProvider = flipvw;
            //this.FlipViewInd.IndicatorSource = Images;

            //this.FlipViewInd.SlideShowTimeSpan = TimeSpan.FromSeconds(5);

            //this.FlipViewInd.IndicatorSource = null;
            //this.FlipViewInd.IndicatorSource = Images;

            this.HTMLTextbox.Html = "Standard Delivery by Wednesday, 22nd April | <b><font color=\'#5FA700\'>FREE</font></b>\n";
            String s = "<h1>My First Heading</h1>\n" + "<p>My first paragraph.</p>";
            this.HTMLTextbox.Html += s;
        }

        private void FlipViewIndicator_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(BlankPage1));
            this.Frame.GoBack();
        }

        private void Play_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void Pause_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void Stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }


    }
}

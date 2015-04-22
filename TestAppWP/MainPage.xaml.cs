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
        public ObservableCollection<ImageModel> Images
        {
            get;
            set;
        }

        public MainPage()
        {
           
            Images = new ObservableCollection<ImageModel>();
            this.DataContext = this;
            InitializeComponent();
            for (int i = 2; i <= 9; i++)
            {
                Images.Add(new ImageModel() { ImagePath = "/Assets/WinLogos/" + i + ".jpg" });
            }

            this.flipvw.SlideShowTimeSpan = TimeSpan.FromSeconds(3);
            //this.flipvw.ItemsSource = Images;

            //this.FlipViewInd.PaginationProvider = flipvw;
            //this.FlipViewInd.IndicatorSource = Images;

            //this.FlipViewInd.SlideShowTimeSpan = TimeSpan.FromSeconds(5);

            //this.FlipViewInd.IndicatorSource = null;
            //this.FlipViewInd.IndicatorSource = Images;

//            this.HTMLTextbox.Html = "Standard Delivery by Wednesday, 22nd April | <b><font color=\'#5FA700\'>FREE</font></b>\n";
//            String s = "<div><span style=\\\"font-weight:bold\\\">Validity</span> : <span>30 days from delivery</span></div>\\r\\n<div>"+
//                "<span style=\\\"font-weight:bold\\\">Covers</span> : <span>Damaged, Defective, Item not as described</span></div>\\r\\n<div>"+
//                "<span style=\\\"font-weight:bold\\\">Type Accepted</span> : <span>Replacement</span></div>\\r\\n\\r\\n <div>\\r\\n If you have received a damaged or defective product or it is not as described, you can get a replacement within 30 days of delivery at no extra cost.\\r\\n </div>\\r\\n <div>\\r\\n <strong>\\r\\n <b> When does this policy not apply?</b>\\r\\n </strong>\\r\\n <div>\\r\\n The guarantee does not extend to: <br/>\\r\\n <ul>\\r\\n <li>Defective products which are covered under the manufacturer\'s warranty.</li>\\r\\n <li>Digital products and services (Flipkart eBooks & music downloads).</li>\\r\\n <li>Innerwear, Lingerie, socks and clothing freebies</li>\\r\\n <li>Damages due to misuse of product or Incidental damage due to malfunctioning of product.</li>\\r\\n <li>Any consumable item which has been used or installed.</li>\\r\\n <li>Products with tampered or missing serial numbers.</li>\\r\\n <li>Items that are returned without original packaging, freebies or accessories.</li>\\r\\n </ul>\\r\\n </div>\\r\\n </div>\\r\\n\\r\\n</div>\n" +
//"";

//            this.HTMLTextbox.Html += s;
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

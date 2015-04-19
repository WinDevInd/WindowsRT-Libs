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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestingLibApp
{
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
            this.DataContext = this;
            this.InitializeComponent();
            Images = new List<ImageModel>();
            this.FlipViewInd.PaginationProvider = flipvw;
            this.FlipViewInd.IndicatorSource = Images;
            this.FlipViewInd.SlideShowTimeSpan = TimeSpan.FromSeconds(5);
            for (int i = 2; i <= 9; i++)
            {
                Images.Add(new ImageModel() { ImagePath = "/Assets/WinLogos/" + i + ".jpg" });
            }
            this.FlipViewInd.IndicatorSource = null;
            this.FlipViewInd.IndicatorSource = Images;
            
        }

        private void FlipViewIndicator_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1));
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("UnSelected Tapped");
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Selected Tapped");
        }        
    }
}

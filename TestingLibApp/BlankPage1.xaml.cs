using System;
using System.Collections.Generic;
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
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
            this.Loaded += BlankPage1_Loaded;
        }

        private void BlankPage1_Loaded(object sender, RoutedEventArgs e)
        {
            String s = "<dl>\n <dt>Coffee</dt>\n <dd>- black hot drink</dd>\n <dt>Milk</dt>\n <dd>- white cold drink</dd>\n</dl>\n";
            this.HtmlView.Html = s;
        }

        private void RichTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}

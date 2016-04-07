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
            String s = "<p>\r\n\tIf you&#39;re looking for a stylish, high-performing smartphone that is economical, then the Le 1s is a worthy bet.</p>\r\n<p>\r\n\t<strong>Performance</strong><br />\r\n\tWith an Octacore Helio X10 Turbo processor (64-bit) which clocks at 2.2 GHz and 3 GB of RAM, the Le 1s opens web pages in an instant, switches between apps within seconds and streams high-quality videos without any glitches. The PowerVR G6200 GPU of the device makes it perfect for hardcore gaming.</p>\r\n<p>\r\n\t<strong>Display</strong><br />\r\n\tWatch your favourite episode of Friends on the brilliant 13.97 cm (5.5) FHD display which boasts a PPI of 403. The In-Cell display reduces the screen thickness and delivers sharper pictures, giving you a visual treat.</p>\r\n<p>\r\n\t<strong>Rear Camera</strong><br />\r\n\tThis smartphone is accompanied by a high-definition 13 MP rear camera that features Samsung ISOCELL technology and a blue glass infrared filter. This camera has an F2.0 aperture and is capable of 0.09 seconds fast focus. It achieves 6x digital zoom and records 4K/1080p videos.</p>\r\n<p>\r\n\t<strong>Front Camera</strong><br />\r\n\tClick a selfie every day with the 5 MP front camera of the Le 1s, share it on Facebook and let the compliments keep coming. The front camera features a wide-angle (85 degrees) lens with an F2.0 aperture.</p>\r\n<p>\r\n\t<strong>Fingerprint Scanner</strong><br />\r\n\tA perfect combination of aesthetics and functionality, the Le 1s boasts the world&#39;s first mirror surfaced fingerprint scanner with 6H hardness. Apart from unlocking the screen, you can use this fingerprint scanner to take photos too.</p>\r\n<p>\r\n\t<strong>Fast Charging</strong><br />\r\n\tThis device comes with a high-density fast-charging battery and a Type-C 24-watt charger, so you&#39;ll never run out of charge.</p>\r\n<p>\r\n\t<strong>Design</strong><br />\r\n\tThe Le 1s has been designed to perfection. It features a full-metal body made from aircraft-grade aluminium, a bezel-less ID and a screwless design. The device is 7.5 mm thin and weighs 169 grams.</p>\r\n";
            this.HtmlView.Html = s;
        }

        private void RichTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}

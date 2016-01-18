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
using BandLib;
using BandLibs.Manager;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Band_Companian
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, ICallbackReceiverInterface
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
           

        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await BandManager.Instance.Connect(this);
            BandManager.Instance.RequestAndSubscribeSensor(SensorType.Heart);
        }

        public void Callback(CallbackSubscriptionType callbackSubscriptionType, object callbackObject)
        {

        }
    }
}

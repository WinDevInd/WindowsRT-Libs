using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media;

namespace JISoft.Controls
{
    public class ImageClickEventArgs : EventArgs
    {
        public ImageClickEventArgs(ImageSource source)
        {
            this.ImageSource = source;
        }

        public ImageSource ImageSource { get; set; }
    }
}

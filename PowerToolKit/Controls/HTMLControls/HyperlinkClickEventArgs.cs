using System;

namespace JISoft.Controls.HTMLControls
{
    public class HyperlinkClickEventArgs : EventArgs
    {
        public HyperlinkClickEventArgs(Uri uri)
        {
            this.NavigationUri = uri;
        }

        public Uri NavigationUri { get; set; }
    }
}

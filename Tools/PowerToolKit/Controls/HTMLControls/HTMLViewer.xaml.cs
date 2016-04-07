using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using JISoft.HTMLTools;
using System.Threading.Tasks;

namespace JISoft.Controls
{
    public partial class HTMLViewer : UserControl
    {
        private object lockObj = new object();
        public HTMLViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty;
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty;

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.Register("MaxLines", typeof(int), typeof(HTMLViewer), new PropertyMetadata(0));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(HTMLViewer), new PropertyMetadata(TextWrapping.Wrap));


        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TextTrimming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(HTMLViewer), new PropertyMetadata(TextTrimming.None));


        [System.ComponentModel.DefaultValue("")]
        public string Html
        {
            get
            {
                return (string)GetValue(HtmlProperty);
            }
            set
            {
                SetValue(HtmlProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for HTML.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(HTMLViewer), new PropertyMetadata("", (d, e) =>
            {
                HTMLViewer tb = d as HTMLViewer;
                tb.OnHtmlPropertyChanged(e);
            }));

        private async void OnHtmlPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                string newVal = "";
                if (string.IsNullOrWhiteSpace((string)e.NewValue))
                {
                    return;
                }
                newVal = e.NewValue as string;
                await Task.Run(() =>
                 {
                     lock (lockObj)
                     {
                         
                         var xamlXml = HtmlToXamlConverter.ConvertHtmlToXamlInternal(newVal, false);
                         devideParagraphs(xamlXml);
                         foreach (var hyperlink in xamlXml.Descendants(XName.Get(HtmlToXamlConverter.Xaml_Hyperlink, HtmlToXamlConverter._xamlNamespace)))
                         {
                             if (hyperlink.Attribute(HtmlToXamlConverter.Xaml_Foreground) == null)
                             {
                                 hyperlink.SetAttributeValue(XName.Get(HtmlToXamlConverter.Xaml_Foreground, HtmlToXamlConverter._xamlNamespace), @"Transparent");
                             }
                         }

                         Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                         {
                             try
                             {
                                 String s = "<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\n" + xamlXml
                                     + "</RichTextBlock>\n";

                                 //var section = XamlReader.Load(xamlXml.ToString()) as Windows.UI.Xaml.Documents.BlockCollection;
                                 RichTextBlock rtb = XamlReader.Load(s) as RichTextBlock;
                                 rtb.HorizontalAlignment = this.HorizontalContentAlignment;
                                 rtb.VerticalAlignment = this.VerticalContentAlignment;
                                 SetBindings(ref rtb);
                                 Scroll.Children.Clear();
                                 Scroll.Children.Add(rtb);
                                 if (rtb.Blocks != null)
                                 {
                                     foreach (var block in rtb.Blocks)
                                     {
                                         var p = block as Paragraph;
                                         if (p != null)
                                         {
                                             postParseInlinesSettings(p.Inlines);
                                         }
                                     }
                                 }
                             }
                             catch
                             {

                             }
                         });
                     }
                 });
            }
            catch
            {

            }
        }

        // divide paragraphs by lineBreaks. It lets to show long paragraph in different rtb'es withot any visual issues
        private void devideParagraphs(XElement element)
        {
            var breaks = element.Descendants(XName.Get(HtmlToXamlConverter.Xaml_LineBreak, HtmlToXamlConverter._xamlNamespace)).ToArray();
            foreach (var br in breaks)
            {
                if (br != null && br.ElementsAfterSelf().Count() > 0)
                {
                    if (br.NextNode.GetType() == typeof(XElement))
                    {
                        if ((br.NextNode as XElement).Name.LocalName == HtmlToXamlConverter.Xaml_LineBreak)
                        {
                            continue;
                        }
                    }
                    XElement par = new XElement(br.Parent);
                    par.SetValue("");
                    var nodes = br.NodesAfterSelf().ToArray();
                    foreach (var node in nodes)
                    {
                        node.Remove();
                    }
                    par.Add(nodes);
                    br.Parent.AddAfterSelf(par);
                    br.Remove();
                }
            }
            var sections = element.Descendants(XName.Get(HtmlToXamlConverter.Xaml_Section, HtmlToXamlConverter._xamlNamespace)).ToArray();
            foreach (var section in sections)
            {
                section.AddAfterSelf(section.Nodes());
                section.Remove();
            }
        }

        // no event can by added in xamlParser
        void postParseInlinesSettings(InlineCollection collection)
        {
            foreach (var il in collection)
            {
                var h = il as Hyperlink;
                if (h != null)
                {
                    // h.CommandParameter = h.NavigateUri;

                    if (h.Foreground.GetType() == typeof(SolidColorBrush))
                    {
                        h.Foreground = this.Foreground;
                    }
                    h.Foreground = new SolidColorBrush(Colors.Blue);
                    h.Click += hyperlink_Click;
                    postParseInlinesSettings(h.Inlines);
                    continue;
                }
                var ui = il as InlineUIContainer;
                if (ui != null)
                {
                    (ui.Child as Button).Template = null;
                    (ui.Child as Button).Click += new RoutedEventHandler(HTMLViewer_Click);
                    var im = (ui.Child as Button).Content as Image;
                    im.ImageOpened += HTMLTextBox_ImageOpened;
                    var source = im.Source as BitmapImage;
                    // support for base64 encoded images
                    if (source.UriSource.Scheme == "data" && (source.UriSource.AbsolutePath.StartsWith("image/png;base64,") || source.UriSource.AbsolutePath.StartsWith("image/jpg;base64,")))
                    {
                        var base64string = source.UriSource.AbsolutePath.Substring(17);
                        byte[] fileBytes = Convert.FromBase64String(base64string);

                        using (IRandomAccessStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length).AsRandomAccessStream())
                        {
                            source.CreateOptions = BitmapCreateOptions.None;
                            source.SetSource(ms);
                        }
                    }
                    if (source.PixelWidth > 0 && ActualWidth > 24 && double.IsNaN(im.Width))
                    {
                        var width = (double)ActualWidth - 24;
                        if (source.PixelWidth < width)
                            width = source.PixelWidth;
                        if (im.Width < width && im.Width != 0)
                            width = im.Width;

                        im.Width = width;
                        im.Height = source.PixelHeight * width / source.PixelWidth;
                    }

                    continue;
                }
                var span = il as Span;
                if (span != null)
                {
                    postParseInlinesSettings(span.Inlines);
                }
            }
        }

        void HTMLViewer_Click(object sender, RoutedEventArgs e)
        {
            if (ImageClick != null)
            {
                var im = (sender as Button).Content as Image;
                ImageClick(im, new ImageClickEventArgs(im.Source));
            }
        }

        void HTMLTextBox_ImageOpened(object sender, RoutedEventArgs e)
        {
            var im = sender as Image;
            var source = im.Source as BitmapImage;
            if (source.PixelWidth > 0 && ActualWidth > 24 && double.IsNaN(im.Width))
            {
                var width = (double)ActualWidth - 24;
                if (source.PixelWidth < width)
                    width = source.PixelWidth;
                if (im.Width < width && im.Width != 0)
                    width = im.Width;

                im.Width = width;
                im.Height = source.PixelHeight * width / source.PixelWidth;
            }
        }

        void hyperlink_Click(object sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs e)
        {
            if (HyperlinkClick != null)
            {
                HyperlinkClick(sender, new HyperlinkClickEventArgs((sender as Hyperlink).NavigateUri));
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is clicked on a some Hyperlink in text.
        /// </summary>
        public event EventHandler<HyperlinkClickEventArgs> HyperlinkClick;

        /// <summary>
        /// Determine how 
        /// </summary>
        //public HTMLTextBoxHyperlynkNavigaionPolitic NavigaionPolitic { get; set; }

        /// <summary>
        /// Occurs when the left mouse button is clicked on a some Image.
        /// </summary>
        public event EventHandler<ImageClickEventArgs> ImageClick;

        /// <summary>
        /// !!! Not Used
        /// </summary>
        public Brush HyperlinksForeground
        {
            get { return (Brush)GetValue(HyperlinksForegroundProperty); }
            set { SetValue(HyperlinksForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HyperlinksForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HyperlinksForegroundProperty =
            DependencyProperty.Register("HyperlinksForeground", typeof(Brush), typeof(HTMLViewer), new PropertyMetadata(new SolidColorBrush()));


        private void SetBindings(ref RichTextBlock rtb)
        {
            Binding textWrapBinding = new Binding();
            textWrapBinding.Path = new PropertyPath("TextWrapping");
            textWrapBinding.Source = this;
            rtb.SetBinding(RichTextBlock.TextWrappingProperty, textWrapBinding);

            Binding textTrimmingBinding = new Binding();
            textTrimmingBinding.Path = new PropertyPath("TextTrimming");
            textTrimmingBinding.Source = this;
            rtb.SetBinding(RichTextBlock.TextTrimmingProperty, textTrimmingBinding);

            Binding maxLineBinding = new Binding();
            maxLineBinding.Path = new PropertyPath("MaxLines");
            maxLineBinding.Source = this;
            rtb.SetBinding(RichTextBlock.MaxLinesProperty, maxLineBinding);

            Binding b = new Binding();
            b.Path = new PropertyPath("FontSize");
            b.Source = this;
            rtb.SetBinding(RichTextBlock.FontSizeProperty, b);
            b = new Binding();
            b.Path = new PropertyPath("Foreground");
            b.Source = this;
            rtb.SetBinding(RichTextBlock.ForegroundProperty, b);
        }
    }
}

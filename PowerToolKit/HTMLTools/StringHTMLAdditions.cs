using System.Net;
using System.Text.RegularExpressions;


namespace JISoft.HTMLTools
{
    public static class StringHTMLAdditions 
    {
        /// <summary>
        /// removes HTML tags and decodes HTMLEncoded symbols
        /// </summary>
        /// <example>
        /// this code:
        /// <code>
        /// "&lt;b&gt;a&amp;lt;b&lt;/b&gt;".PlainStringFromHtml();
        /// </code>
        /// returns "a&lt;b"
        /// </example>
        public static string PlainStringFromHtml(this string str)
        {
            Regex opentag = new Regex("<\\w+(:\\w+)*(\\s+([\\w^\"']+)=(\"|')([^\"']*)(\"|'))*\\s*/?>");
            Regex closetag = new Regex("</\\w+(:\\w+)*(\\s+([\\w^\"']+)=(\"|')([^\"']*)(\"|'))*\\s*>");
            Regex whitespaseAndNewLine = new Regex("(\\u000A|\\u000D|\\s)+"); // \n | \r | whitespase
            return WebUtility.HtmlDecode(whitespaseAndNewLine.Replace(closetag.Replace(opentag.Replace(str, ""), ""), ""));
        }


        /// <summary>
        /// Converts Html string to Xaml string. Section or Span elements will be used as a root element
        /// </summary>
        public static string XamlStringFromHtml(this string str)
        {
            return HtmlToXamlConverter.ConvertHtmlToXaml(str, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asFlowDocument">
        /// true indicates that we need a FlowDocument as a root element;
        /// false means that Section or Span elements will be used
        /// dependeing on StartFragment/EndFragment comments locations.
        /// </param>
        public static string XamlStringFromHtml(this string str, bool asFlowDocument)
        {
            return HtmlToXamlConverter.ConvertHtmlToXaml(str, asFlowDocument);
        }
    }
}

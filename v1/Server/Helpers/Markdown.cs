
namespace Server.Helpers
{
    using System.Web.Mvc;

    public static class MarkdownHelper
    {
        public static MvcHtmlString Markdown(this HtmlHelper helper, string mdText)
        {
            MarkdownDeep.Markdown md = new MarkdownDeep.Markdown() { 
                ExtraMode = true, 
                SafeMode = true
            };
            return new MvcHtmlString(md.Transform(mdText));
        }

    }
}
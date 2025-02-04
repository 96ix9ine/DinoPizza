using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Text.Encodings.Web;

namespace DinoPizza.Helpers
{
    public static class MyHtmlHelper
    {
        public static HtmlString Pagination(this IHtmlHelper html, int pageCount)
        {
            var tagDiv = new TagBuilder("div");
            for (int i = 1; i <= pageCount; i++) 
            {
                // создаём тэги
                var tagA = new TagBuilder("a"); // <a></a>
                tagA.Attributes.Add("href", $"List?page={i}");
                tagA.InnerHtml.SetContent(i.ToString());

                tagDiv.InnerHtml.AppendHtml(tagA);
            }

            var writer = new System.IO.StringWriter();
            tagDiv.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}

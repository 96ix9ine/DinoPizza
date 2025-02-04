using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace DinoPizza.Helpers
{
    [HtmlTargetElement("Pagination")]
    public class TagPagination : TagHelper
    {
        [HtmlAttributeName]
        public int Count { get; set; }

        [HtmlAttributeName]
        public int Active { get; set; }

        [HtmlAttributeName]
        public string? SubCategory { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            // bootstrap pagination
            output.TagName = "nav";
            var tagNav = new TagBuilder("nav");

            var tagUl = new TagBuilder("ul");
            tagUl.AddCssClass("pagination");

            for (int i = 1; i <= Count; i++)
            {
                var tagLi = new TagBuilder("li");
                tagLi.AddCssClass("page-item");

                var tagA = new TagBuilder("a");
                tagA.AddCssClass("page-link");

                if (String.IsNullOrEmpty(SubCategory))
                {
                    tagA.Attributes.Add("href", $"List?page={i}");
                }
                else
                {
                    tagA.Attributes.Add("href", 
                        $"List?page={i}&subCategory={SubCategory}");
                }

                tagA.InnerHtml.SetContent(i.ToString());

                if (i == Active)
                    tagLi.AddCssClass("active");

                tagLi.InnerHtml.SetHtmlContent(tagA); // <a> - всталяем внутрь <li> 
                tagUl.InnerHtml.AppendHtml(tagLi); // <li> - добавляем внутрь ul
            }
            tagNav.InnerHtml.SetHtmlContent(tagUl);

            var writer = new System.IO.StringWriter();
            tagNav.InnerHtml.WriteTo(writer, HtmlEncoder.Default);
            output.Content.SetHtmlContent(writer.ToString());
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}

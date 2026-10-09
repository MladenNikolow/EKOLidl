namespace EKOLidl.Web.TagHelpers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// Dark page header for inner pages: <c>&lt;page-hero eyebrow="..." title="..."&gt;lead and actions&lt;/page-hero&gt;</c>.
    /// Pair it with <c>ViewData["HeaderOverlay"] = true</c> so the site header floats over it.
    /// </summary>
    [HtmlTargetElement("page-hero")]
    public class PageHeroTagHelper : TagHelper
    {
        private const string Wave =
            "<div class=\"pointer-events-none absolute inset-x-0 bottom-0 h-10 overflow-hidden md:h-14\" aria-hidden=\"true\">"
            + "<svg class=\"absolute bottom-1 h-full w-[200%] animate-wave text-lagoon/20 [animation-duration:26s]\" viewBox=\"0 0 2880 120\" preserveAspectRatio=\"none\">"
            + "<path fill=\"currentColor\" d=\"M0 60 Q180 0 360 60 T720 60 T1080 60 T1440 60 T1800 60 T2160 60 T2520 60 T2880 60 V120 H0 Z\"/></svg>"
            + "<svg class=\"absolute bottom-0 h-full w-[200%] animate-wave text-[var(--page-surface)]\" viewBox=\"0 0 2880 120\" preserveAspectRatio=\"none\">"
            + "<path fill=\"currentColor\" d=\"M0 70 Q180 20 360 70 T720 70 T1080 70 T1440 70 T1800 70 T2160 70 T2520 70 T2880 70 V120 H0 Z\"/></svg>"
            + "</div>";

        /// <summary>
        /// Gets or sets the small label above the title.
        /// </summary>
        public string Eyebrow { get; set; }

        /// <summary>
        /// Gets or sets the page title (the page's only h1).
        /// </summary>
        public string Title { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperContent childContent = await output.GetChildContentAsync();

            output.TagName = "section";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "sea overflow-hidden");
            output.Attributes.SetAttribute("aria-labelledby", "page-title");

            output.Content.AppendHtml("<div class=\"container-page relative pb-20 pt-32 md:pb-28 md:pt-40\">");

            if (!string.IsNullOrWhiteSpace(this.Eyebrow))
            {
                output.Content.AppendHtml("<p class=\"eyebrow\" data-reveal>");
                output.Content.Append(this.Eyebrow);
                output.Content.AppendHtml("</p>");
            }

            output.Content.AppendHtml("<h1 id=\"page-title\" class=\"mt-5 max-w-4xl text-h1\" data-reveal=\"1\">");
            output.Content.Append(this.Title);
            output.Content.AppendHtml("</h1>");
            output.Content.AppendHtml(childContent);
            output.Content.AppendHtml("</div>");
            output.Content.AppendHtml(Wave);
        }
    }
}

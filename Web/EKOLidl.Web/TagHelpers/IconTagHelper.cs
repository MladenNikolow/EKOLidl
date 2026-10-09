namespace EKOLidl.Web.TagHelpers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// Renders an inline Lucide icon (ISC licence, https://lucide.dev): <c>&lt;icon name="map-pin" class="size-5" /&gt;</c>.
    /// Icons inherit the text colour and are hidden from screen readers unless a <c>label</c> is given.
    /// </summary>
    [HtmlTargetElement("icon", TagStructure = TagStructure.WithoutEndTag)]
    public class IconTagHelper : TagHelper
    {
        private static readonly Dictionary<string, string> Icons = new()
        {
            ["anchor"] = "<path d=\"M12 6v16\"/><path d=\"m19 13 2-1a9 9 0 0 1-18 0l2 1\"/><path d=\"M9 11h6\"/><circle cx=\"12\" cy=\"4\" r=\"2\"/>",
            ["arrow-left"] = "<path d=\"m12 19-7-7 7-7\"/><path d=\"M19 12H5\"/>",
            ["arrow-right"] = "<path d=\"M5 12h14\"/><path d=\"m12 5 7 7-7 7\"/>",
            ["arrow-up-right"] = "<path d=\"M7 7h10v10\"/><path d=\"M7 17 17 7\"/>",
            ["calendar-days"] = "<path d=\"M8 2v3\"/><path d=\"M16 2v3\"/><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M3 9h18\"/><path d=\"M8 13h.01\"/><path d=\"M12 13h.01\"/><path d=\"M16 13h.01\"/><path d=\"M8 17h.01\"/><path d=\"M12 17h.01\"/><path d=\"M16 17h.01\"/>",
            ["camera"] = "<path d=\"M13.997 4a2 2 0 0 1 1.76 1.05l.486.9A2 2 0 0 0 18.003 7H20a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h1.997a2 2 0 0 0 1.759-1.048l.489-.904A2 2 0 0 1 10.004 4z\"/><circle cx=\"12\" cy=\"13\" r=\"3\"/>",
            ["check"] = "<path d=\"M20 6 9 17l-5-5\"/>",
            ["chevron-left"] = "<path d=\"m15 18-6-6 6-6\"/>",
            ["chevron-right"] = "<path d=\"m9 18 6-6-6-6\"/>",
            ["circle-alert"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"12\" x2=\"12\" y1=\"8\" y2=\"12\"/><line x1=\"12\" x2=\"12.01\" y1=\"16\" y2=\"16\"/>",
            ["clock"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 6v6l4 2\"/>",
            ["copy"] = "<rect width=\"14\" height=\"14\" x=\"8\" y=\"8\" rx=\"2\" ry=\"2\"/><path d=\"M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2\"/>",
            ["external-link"] = "<path d=\"M15 3h6v6\"/><path d=\"M10 14 21 3\"/><path d=\"M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6\"/>",
            ["file-text"] = "<path d=\"M6 22a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8a2.4 2.4 0 0 1 1.704.706l3.588 3.588A2.4 2.4 0 0 1 20 8v12a2 2 0 0 1-2 2z\"/><path d=\"M14 2v5a1 1 0 0 0 1 1h5\"/><path d=\"M10 9H8\"/><path d=\"M16 13H8\"/><path d=\"M16 17H8\"/>",
            ["fish"] = "<path d=\"M6.5 12c.94-3.46 4.94-6 8.5-6 3.56 0 6.06 2.54 7 6-.94 3.47-3.44 6-7 6s-7.56-2.53-8.5-6Z\"/><path d=\"M18 12v.5\"/><path d=\"M16 17.93a9.77 9.77 0 0 1 0-11.86\"/><path d=\"M7 10.67C7 8 5.58 5.97 2.73 5.5c-1 1.5-1 5 .23 6.5-1.24 1.5-1.24 5-.23 6.5C5.58 18.03 7 16 7 13.33\"/><path d=\"M10.46 7.26C10.2 5.88 9.17 4.24 8 3h5.8a2 2 0 0 1 1.98 1.67l.23 1.4\"/><path d=\"m16.01 17.93-.23 1.4A2 2 0 0 1 13.8 21H9.5a5.96 5.96 0 0 0 1.49-3.98\"/>",
            ["flag"] = "<path d=\"M4 22V4a1 1 0 0 1 .4-.8A6 6 0 0 1 8 2c3 0 5 2 7.333 2q2 0 3.067-.8A1 1 0 0 1 20 4v10a1 1 0 0 1-.4.8A6 6 0 0 1 16 16c-3 0-5-2-8-2a6 6 0 0 0-4 1.528\"/>",
            ["graduation-cap"] = "<path d=\"M21.42 10.922a1 1 0 0 0-.019-1.838L12.83 5.18a2 2 0 0 0-1.66 0L2.6 9.08a1 1 0 0 0 0 1.832l8.57 3.908a2 2 0 0 0 1.66 0z\"/><path d=\"M22 10v6\"/><path d=\"M6 12.5V16a6 3 0 0 0 12 0v-3.5\"/>",
            ["hand-heart"] = "<path d=\"M11 14h2a2 2 0 0 0 0-4h-3c-.6 0-1.1.2-1.4.6L3 16\"/><path d=\"m14.45 13.39 5.05-4.694C20.196 8 21 6.85 21 5.75a2.75 2.75 0 0 0-4.797-1.837.276.276 0 0 1-.406 0A2.75 2.75 0 0 0 11 5.75c0 1.2.802 2.248 1.5 2.946L16 11.95\"/><path d=\"m2 15 6 6\"/><path d=\"m7 20 1.6-1.4c.3-.4.8-.6 1.4-.6h4c1.1 0 2.1-.4 2.8-1.2l4.6-4.4a1 1 0 0 0-2.75-2.91\"/>",
            ["hourglass"] = "<path d=\"M5 22h14\"/><path d=\"M5 2h14\"/><path d=\"M17 22v-4.172a2 2 0 0 0-.586-1.414L12 12l-4.414 4.414A2 2 0 0 0 7 17.828V22\"/><path d=\"M7 2v4.172a2 2 0 0 0 .586 1.414L12 12l4.414-4.414A2 2 0 0 0 17 6.172V2\"/>",
            ["image-up"] = "<path d=\"M10.3 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2v10l-3.1-3.1a2 2 0 0 0-2.814.014L6 21\"/><path d=\"m14 19.5 3-3 3 3\"/><path d=\"M17 22v-5.5\"/><circle cx=\"9\" cy=\"9\" r=\"2\"/>",
            ["images"] = "<path d=\"m22 11-1.296-1.296a2.4 2.4 0 0 0-3.408 0L11 16\"/><path d=\"M4 8a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2\"/><circle cx=\"13\" cy=\"7\" r=\"1\" fill=\"currentColor\"/><rect x=\"8\" y=\"2\" width=\"14\" height=\"14\" rx=\"2\"/>",
            ["info"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 16v-4\"/><path d=\"M12 8h.01\"/>",
            ["mail"] = "<path d=\"m22 7-8.991 5.727a2 2 0 0 1-2.009 0L2 7\"/><rect x=\"2\" y=\"4\" width=\"20\" height=\"16\" rx=\"2\"/>",
            ["map-pin"] = "<path d=\"M20 10c0 4.993-5.539 10.193-7.399 11.799a1 1 0 0 1-1.202 0C9.539 20.193 4 14.993 4 10a8 8 0 0 1 16 0\"/><circle cx=\"12\" cy=\"10\" r=\"3\"/>",
            ["megaphone"] = "<path d=\"M11 6a13 13 0 0 0 8.4-2.8A1 1 0 0 1 21 4v12a1 1 0 0 1-1.6.8A13 13 0 0 0 11 14H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2z\"/><path d=\"M6 14a12 12 0 0 0 2.4 7.2 2 2 0 0 0 3.2-2.4A8 8 0 0 1 10 14\"/><path d=\"M8 6v8\"/>",
            ["menu"] = "<path d=\"M4 5h16\"/><path d=\"M4 12h16\"/><path d=\"M4 19h16\"/>",
            ["phone"] = "<path d=\"M13.832 16.568a1 1 0 0 0 1.213-.303l.355-.465A2 2 0 0 1 17 15h3a2 2 0 0 1 2 2v3a2 2 0 0 1-2 2A18 18 0 0 1 2 4a2 2 0 0 1 2-2h3a2 2 0 0 1 2 2v3a2 2 0 0 1-.8 1.6l-.468.351a1 1 0 0 0-.292 1.233 14 14 0 0 0 6.392 6.384\"/>",
            ["recycle"] = "<path d=\"M7 19H4.815a1.83 1.83 0 0 1-1.57-.881 1.785 1.785 0 0 1-.004-1.784L7.196 9.5\"/><path d=\"M11 19h8.203a1.83 1.83 0 0 0 1.556-.89 1.784 1.784 0 0 0 0-1.775l-1.226-2.12\"/><path d=\"m14 16-3 3 3 3\"/><path d=\"M8.293 13.596 7.196 9.5 3.1 10.598\"/><path d=\"m9.344 5.811 1.093-1.892A1.83 1.83 0 0 1 11.985 3a1.784 1.784 0 0 1 1.546.888l3.943 6.843\"/><path d=\"m13.378 9.633 4.096 1.098 1.097-4.096\"/>",
            ["shell"] = "<path d=\"M14 11a2 2 0 1 1-4 0 4 4 0 0 1 8 0 6 6 0 0 1-12 0 8 8 0 0 1 16 0 10 10 0 1 1-20 0 11.93 11.93 0 0 1 2.42-7.22 2 2 0 1 1 3.16 2.44\"/>",
            ["shield-check"] = "<path d=\"M20 13c0 5-3.5 7.5-7.66 8.95a1 1 0 0 1-.67-.01C7.5 20.5 4 18 4 13V6a1 1 0 0 1 1-1c2 0 4.5-1.2 6.24-2.72a1.17 1.17 0 0 1 1.52 0C14.51 3.81 17 5 19 5a1 1 0 0 1 1 1z\"/><path d=\"m9 12 2 2 4-4\"/>",
            ["sparkles"] = "<path d=\"M11.017 2.814a1 1 0 0 1 1.966 0l1.051 5.558a2 2 0 0 0 1.594 1.594l5.558 1.051a1 1 0 0 1 0 1.966l-5.558 1.051a2 2 0 0 0-1.594 1.594l-1.051 5.558a1 1 0 0 1-1.966 0l-1.051-5.558a2 2 0 0 0-1.594-1.594l-5.558-1.051a1 1 0 0 1 0-1.966l5.558-1.051a2 2 0 0 0 1.594-1.594z\"/><path d=\"M20 2v4\"/><path d=\"M22 4h-4\"/><circle cx=\"4\" cy=\"20\" r=\"2\"/>",
            ["sun"] = "<circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 2v2\"/><path d=\"M12 20v2\"/><path d=\"m4.93 4.93 1.41 1.41\"/><path d=\"m17.66 17.66 1.41 1.41\"/><path d=\"M2 12h2\"/><path d=\"M20 12h2\"/><path d=\"m6.34 17.66-1.41 1.41\"/><path d=\"m19.07 4.93-1.41 1.41\"/>",
            ["trash-2"] = "<path d=\"M10 11v6\"/><path d=\"M14 11v6\"/><path d=\"M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6\"/><path d=\"M3 6h18\"/><path d=\"M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2\"/>",
            ["users"] = "<path d=\"M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2\"/><path d=\"M16 3.128a4 4 0 0 1 0 7.744\"/><path d=\"M22 21v-2a4 4 0 0 0-3-3.87\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/>",
            ["waves"] = "<path d=\"M2 12q2.5 2 5 0t5 0 5 0 5 0\"/><path d=\"M2 19q2.5 2 5 0t5 0 5 0 5 0\"/><path d=\"M2 5q2.5 2 5 0t5 0 5 0 5 0\"/>",
            ["x"] = "<path d=\"M18 6 6 18\"/><path d=\"m6 6 12 12\"/>",
        };

        /// <summary>
        /// Gets or sets the Lucide icon name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an accessible label; when empty the icon is decorative.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the stroke width (Lucide default is 2).
        /// </summary>
        public double Stroke { get; set; } = 2;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Icons.TryGetValue(this.Name ?? string.Empty, out string paths))
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "svg";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            output.Attributes.SetAttribute("viewBox", "0 0 24 24");
            output.Attributes.SetAttribute("fill", "none");
            output.Attributes.SetAttribute("stroke", "currentColor");
            output.Attributes.SetAttribute("stroke-width", this.Stroke.ToString(System.Globalization.CultureInfo.InvariantCulture));
            output.Attributes.SetAttribute("stroke-linecap", "round");
            output.Attributes.SetAttribute("stroke-linejoin", "round");

            // Default to 20px unless the caller sets a size; keep any other classes (colour, margins).
            string classes = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;
            if (!classes.Contains("size-", System.StringComparison.Ordinal))
            {
                output.Attributes.SetAttribute("class", $"size-5 shrink-0 {classes}".Trim());
            }

            if (string.IsNullOrWhiteSpace(this.Label))
            {
                output.Attributes.SetAttribute("aria-hidden", "true");
                output.Attributes.SetAttribute("focusable", "false");
            }
            else
            {
                output.Attributes.SetAttribute("role", "img");
                output.Attributes.SetAttribute("aria-label", this.Label);
            }

            output.Content.SetHtmlContent(paths);
        }
    }
}

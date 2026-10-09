namespace EKOLidl.Web.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    ///     Model for the <c>_Gallery</c> partial.
    /// </summary>
    /// <param name="Images">Image URLs, in display order.</param>
    /// <param name="AltPrefix">Text used to describe each photo, e.g. the event title.</param>
    public sealed record Gallery(IReadOnlyList<string> Images, string AltPrefix);
}

namespace EKOLidl.Web.Infrastructure
{
    /// <summary>
    ///     Model for the <c>_EmptyState</c> partial, shown when a list has nothing to display.
    /// </summary>
    /// <param name="Icon">Lucide icon name.</param>
    /// <param name="Title">Short heading.</param>
    /// <param name="Text">One-sentence explanation.</param>
    public sealed record EmptyState(string Icon, string Title, string Text);
}

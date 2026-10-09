namespace EKOLidl.Web.Infrastructure
{
    /// <summary>
    ///     Model for the <c>_Pagination</c> partial.
    /// </summary>
    /// <param name="Controller">Controller of the paged list.</param>
    /// <param name="Action">Action of the paged list.</param>
    /// <param name="Page">Current 1-based page.</param>
    /// <param name="PagesCount">Total number of pages.</param>
    public sealed record Pagination(string Controller, string Action, int Page, int PagesCount);
}

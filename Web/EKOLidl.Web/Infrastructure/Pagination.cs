namespace EKOLidl.Web.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    ///     Model for the <c>_Pagination</c> partial.
    /// </summary>
    /// <param name="Controller">Controller of the paged list.</param>
    /// <param name="Action">Action of the paged list.</param>
    /// <param name="Page">Current 1-based page.</param>
    /// <param name="PagesCount">Total number of pages.</param>
    /// <param name="RouteValues">Extra query values to keep on every page link, e.g. a filter.</param>
    public sealed record Pagination(string Controller, string Action, int Page, int PagesCount, IDictionary<string, string> RouteValues = null);
}

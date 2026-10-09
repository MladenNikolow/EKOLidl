namespace EKOLidl.Data.Models.Signals
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Common.Repositories;

    public interface ISignalRepository : IRepository<Signal>
    {
        /// <summary>
        /// Gets one page of approved (public) or pending signals, newest first.
        /// </summary>
        /// <param name="isApproved">Whether to list approved or pending signals.</param>
        /// <param name="page">1-based page number.</param>
        /// <param name="itemsPerPage">Items per page.</param>
        /// <returns>The signals on the page.</returns>
        Task<IEnumerable<Signal>> GetSignalsByPageAsync(bool isApproved, int page, int itemsPerPage);

        /// <summary>
        /// Counts approved (public) or pending signals.
        /// </summary>
        /// <param name="isApproved">Whether to count approved or pending signals.</param>
        /// <returns>The count.</returns>
        Task<int> GetSignalsCountAsync(bool isApproved);

        Task<IEnumerable<Signal>> GetNewestApprovedSignalsAsync(int count);
    }
}

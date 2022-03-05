namespace EKOLidl.Data.Models.Signals
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Common.Repositories;

    public interface ISignalRepository : IRepository<Signal>
    {
        Task<IEnumerable<Signal>> GetApprovedSignalsByPageAsync(int page, int itemsPerPage);
    }
}

namespace EKOLidl.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;

    public interface ISignalService
    {
        Task<IEnumerable<Signal>> GetSignalsAsync(string query);

        Task<Signal> GetSignalAsync(string id);

        Task AddSignalAsync(Signal signal);

        Task UpdateSignalAsync(string id, Signal signal);

        Task DeleteSignalAsync(string id);

        Task<int> GetCountAsync();

        Task<IEnumerable<Signal>> GetSignalsByPageAsync(int page, int eventsPerPage = 12);

        Task<IEnumerable<Signal>> GetNewestSignals(int count = 4);
    }
}

namespace EKOLidl.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;

    public interface ISignalService
    {
        Task<IEnumerable<Signal>> GetSignalsAsync(string query);

        Task<Event> GetSignalAsync(string id);

        Task AddSignalAsync(Signal ev);

        Task UpdateSignalAsync(string id, Signal ev);

        Task DeleteSignalAsync(string id);
    }
}

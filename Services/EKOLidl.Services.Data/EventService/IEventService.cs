namespace LidlProject.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;

    public interface IEventService
    {
        Task<IEnumerable<Event>> GetEventsAsync(string query);

        Task<Event> GetEventAsync(string id);

        Task AddEventAsync(Event ev);

        Task UpdateEventAsync(string id, Event ev);

        Task DeleteEventAsync(string id);

        Task<int> GetCountAsync();

        Task<IEnumerable<Event>> GetEventsByPageAsync(int page, int eventsPerPage = 9);

        Task<IEnumerable<Event>> GetNewestEvents(int count = 4);
    }
}

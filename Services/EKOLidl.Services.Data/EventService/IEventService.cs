
namespace EKOLidl.Services.Data.EventService
{
    using System.Collections.Generic;

    using EKOLidl.Data.Models;

    public interface IEventService
    {
        public ICollection<Event> GetAllEvents();

        public Event AddEventAsync(Event eventToAdd);

        public Event UpdateEventAsync(Event eventToAdd);

        public bool RemoveEventAsync(Event eventToRemove);
    }
}

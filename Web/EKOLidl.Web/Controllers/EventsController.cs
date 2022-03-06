namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using EKOLidl.Services.Mapping;
    using EKOLidl.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        private readonly IEventRepository eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<IActionResult> Events(int page = 1)
        {
            const int eventsPerPage = 9;
            IEnumerable<Event> events = await this.eventRepository.GetItemsByPageAsync(page, eventsPerPage);
            int eventsCount = await this.eventRepository.GetItemsCountAsync();

            EventsViewModel eventsViewModel = new EventsViewModel
            {
                Events = new List<Event>(events),
                Page = page,
                Count = eventsCount,
                EventsPerPage = eventsPerPage,
            };

            return this.View(eventsViewModel);
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var result = await this.eventRepository.GetItemAsync(id);
            EventDetailsViewModel signalDetailsViewModel = AutoMapperConfig.MapperInstance.Map<EventDetailsViewModel>(result);

            return this.View(signalDetailsViewModel);
        }
    }
}

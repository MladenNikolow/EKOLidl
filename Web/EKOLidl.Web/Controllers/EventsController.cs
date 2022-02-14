namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Web.ViewModels.Events;
    using LidlProject.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        private readonly IEventService eventService;

        public EventsController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public async Task<IActionResult> Events(int page = 1)
        {
            const int eventsPerPage = 9;
            List<Event> events = new List<Event>(this.eventService.GetEventsByPageAsync(page, eventsPerPage).GetAwaiter().GetResult());
            int eventsCount = await this.eventService.GetCountAsync();

            EventsViewModel eventsViewModel = new EventsViewModel
            {
                Events = events,
                Page = page,
                Count = eventsCount,
                EventsPerPage = eventsPerPage,
            };

            return this.View(eventsViewModel);
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            return this.View(await this.eventService.GetEventAsync(id));
        }
    }
}

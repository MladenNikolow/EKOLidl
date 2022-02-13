namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using LidlProject.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        private readonly IEventService eventService;

        public EventsController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        [Route("Events/{page}")]
        public async Task<IActionResult> Events(int page = 0)
        {
            return this.View(await this.eventService.GetEventsByPage(page));
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            return this.View(await this.eventService.GetEventAsync(id));
        }
    }
}

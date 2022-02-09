namespace EKOLidl.Web.Controllers
{
    using System.Threading.Tasks;

    using LidlProject.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        private readonly IEventService eventService;

        public EventsController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            return this.View(await this.eventService.GetEventsAsync("SELECT * FROM c"));
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return this.View(await this.eventService.GetEventAsync(id));
        }
    }
}

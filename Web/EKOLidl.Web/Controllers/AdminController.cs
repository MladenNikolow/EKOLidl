namespace EKOLidl.Web.Controllers
{
    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using EKOLidl.Services.Mapping;
    using EKOLidl.Web.ViewModels.Administration;
    using EKOLidl.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AdminController : Controller
    {
        private readonly IEventRepository eventRepository;

        public AdminController(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1")]
        public IActionResult Login()
        {
            CredentialsInputModel input = new CredentialsInputModel()
            {
                Username = string.Empty,
                Password = string.Empty,
            };
            return this.View(input);
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1")]
        [HttpPost]
        public IActionResult Login(CredentialsInputModel credentials)
        {
            string username = "admin";
            string password = "password";

            this.ModelState.Clear();

            if (credentials.Username != username)
            {
                this.ModelState.AddModelError("password", "Грешно потребителско име или парола!");
                return this.View();
            }

            if (credentials.Password != password)
            {
                this.ModelState.AddModelError("password", "Грешно потребителско име или парола!");
                return this.View();
            }

            return this.Redirect("3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/AdminHome");
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/AdminHome")]
        public IActionResult AdminHome()
        {
            return this.View();
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/EditEvents")]
        public async Task<IActionResult> EditEvents(int page = 1)
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

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/EditEvent")]
        public async Task<IActionResult> EditEvent(string id)
        {
            var result = await this.eventRepository.GetItemAsync(id);
            EditEventInputModel editEventInputModel = AutoMapperConfig.MapperInstance.Map<EditEventInputModel>(result);

            return this.View(editEventInputModel);
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/EditEvent")]
        [HttpPost]
        public async Task<IActionResult> EditEvent(EditEventInputModel editEventInputModel)
        {
            this.ModelState.Clear();

            // Model State Validation
            if (!this.ModelState.IsValid)
            {
                return this.View(editEventInputModel);
            }

            return this.View(editEventInputModel);
        }

        [Route("Admin/3912cd5e-8b27-405a-9e93-da59d97db6343d1dbb05-7d9a-4b28-a945-f738489a59d1/EditSignals")]
        public IActionResult EditSignals()
        {
            return this.View();
        }
    }
}

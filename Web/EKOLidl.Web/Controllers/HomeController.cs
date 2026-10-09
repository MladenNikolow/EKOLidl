namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Web.ViewModels;
    using EKOLidl.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly ISignalRepository signalRepository;

        public HomeController(IEventRepository eventRepository, ISignalRepository signalRepository)
        {
            this.eventRepository = eventRepository;
            this.signalRepository = signalRepository;
        }

        public async Task<IActionResult> Index()
        {
            var events = await this.eventRepository.GetNewestItems(3);
            var signals = await this.signalRepository.GetNewestApprovedSignalsAsync(3);

            HomeViewModel viewModel = new HomeViewModel()
            {
                Events = new List<Event>(events),
                Signals = new List<Signal>(signals),
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult About()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}

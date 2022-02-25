namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Services.Data;
    using EKOLidl.Web.ViewModels;
    using EKOLidl.Web.ViewModels.Home;
    using LidlProject.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IEventService eventService;
        private readonly ISignalService signalService;

        public HomeController(IEventService eventService, ISignalService signalService)
        {
            this.eventService = eventService;
            this.signalService = signalService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await this.eventService.GetNewestEvents();
            var signals = await this.signalService.GetNewestSignals();

            HomeViewModel viewModel = new HomeViewModel()
            {
                Events = events,
                Signals = signals,

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

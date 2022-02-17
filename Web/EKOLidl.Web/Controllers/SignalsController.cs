namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EKOLidl.Data.Models;
    using EKOLidl.Services.Data;
    using EKOLidl.Web.ViewModels.Signals;
    using Microsoft.AspNetCore.Mvc;

    public class SignalsController : BaseController
    {
        private readonly ISignalService signalService;

        public SignalsController(ISignalService signalService)
        {
            this.signalService = signalService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int signalsPerPage = 12;
            IEnumerable<Signal> signals = await this.signalService.GetSignalsByPageAsync(page, signalsPerPage);
            int signalsCount = await this.signalService.GetCountAsync();

            SignalsViewModel signalsViewModel = new SignalsViewModel
            {
                Signals = signals,
                Page = page,
                Count = signalsCount,
                SignalsPerPage = signalsPerPage,
            };

            return this.View(signalsViewModel);
        }

        [Route("Signals/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            return this.View(await this.signalService.GetSignalAsync(id));
        }

    }
}

namespace EKOLidl.Web.Controllers
{
    using System.Threading.Tasks;

    using EKOLidl.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class SignalsController : BaseController
    {
        private readonly ISignalService signalService;

        public SignalsController(ISignalService signalService)
        {
            this.signalService = signalService;
        }

        [Route("Signals/{page}")]
        public async Task<IActionResult> Signals(int page = 1)
        {
            return this.View(await this.signalService.GetSignalsByPage(page));
        }

        [Route("Signals/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            return this.View(await this.signalService.GetSignalAsync(id));
        }

    }
}

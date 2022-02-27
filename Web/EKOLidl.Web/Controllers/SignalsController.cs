namespace EKOLidl.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data;
    using EKOLidl.Services.Data.BlobContainerService;
    using EKOLidl.Services.Mapping;
    using EKOLidl.Web.ViewModels.Signals;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class SignalsController : BaseController
    {
        private readonly ISignalService signalService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IBlobContainerService blobContainerService;

        public SignalsController(ISignalService signalService, IWebHostEnvironment webHostEnvironment, IBlobContainerService blobContainerService)
        {
            this.signalService = signalService;
            this.webHostEnvironment = webHostEnvironment;
            this.blobContainerService = blobContainerService;
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
            var result = await this.signalService.GetSignalAsync(id);
            SignalDetailsViewModel signalDetailsViewModel = AutoMapperConfig.MapperInstance.Map<SignalDetailsViewModel>(result);

            return this.View(signalDetailsViewModel);
        }

        [Route("Signals/Create")]
        public IActionResult Create()
        {
            var viewModel = new CreateSignalInputModel();
            viewModel.TrashType = new List<TrashType>();
            viewModel.TrashTypeItems = ((TrashType[])Enum.GetValues(typeof(TrashType))).ToList().Select(x => new
            {
                id = (int)x,
                description = ((DescriptionAttribute[])x.GetType().GetField(x.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false))[0].Description,
            }).ToList().Select(x => new KeyValuePair<int, string>(x.id, x.description)).ToList();

            return this.View(viewModel);
        }

        [Route("Signals/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSignalInputModel input)
        {

            // Image Validation
            foreach (var image in input.Images)
            {
                if (!this.blobContainerService.IsValidImage(image))
                {
                    this.ModelState.AddModelError("Images", "Моля, добавете валидна снимка във формат: .png, .jpg или .jpeg и с размер до 10 MB.");
                }
            }

            // General Validation
            if (!this.ModelState.IsValid)
            {
                input.TrashTypeItems = new List<KeyValuePair<int, string>>();
                return this.View(input);
            }

            // Save Data
            try
            {
                input.ImageUrls = await this.blobContainerService.UploadFilesAsync(input.Images, this.webHostEnvironment.WebRootPath);

                var signal = AutoMapperConfig.MapperInstance.Map<Signal>(input);
                await this.signalService.AddSignalAsync(signal);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(input);
            }

            this.TempData["Message"] = "Сигналът е добавен успешно.";

            // TODO: Redirect to recipe info page
            return this.RedirectToAction("Signals");
        }
    }
}

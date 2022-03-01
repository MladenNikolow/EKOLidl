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

        public async Task<IActionResult> Index(int page = 1, bool isCreateSingalContext = false)
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
                IsCreateSignalContext = isCreateSingalContext,
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
            viewModel.Album = new Album()
            {
                Links = new List<string>(),
            };

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
            this.ModelState.Clear();

            input.TrashTypeItems = ((TrashType[])Enum.GetValues(typeof(TrashType))).ToList().Select(x => new
            {
                id = (int)x,
                description = ((DescriptionAttribute[])x.GetType().GetField(x.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false))[0].Description,
            }).ToList().Select(x => new KeyValuePair<int, string>(x.id, x.description)).ToList();

            if (input.TrashType == null)
            {
                this.ModelState.AddModelError("TrashType", "Моля изберете тип на отпадъка");
            }

            foreach (var image in input.Images)
            {
                // Image Validation
                if (!this.blobContainerService.IsValidImage(image))
                {
                    this.ModelState.AddModelError("Images", "Разрешени формат за изображение: .png, .jpg или .jpeg. Максимален размер до 5 MB.");
                    break;
                }
            }

            // Model State Validation
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            // Save Data
            try
            {
                // Upload Images
                var imagesResult = await this.blobContainerService.UploadFilesAsync(input.Images, this.webHostEnvironment.WebRootPath);

                if (imagesResult == null)
                {
                    this.ModelState.AddModelError("Images", "Възникна проблем при обработка на вашия сигнал. Моля, опитайте онтово!");
                }
                else
                {
                    input.Album = new Album();
                    input.Album.Links = (List<string>)imagesResult;

                    var signalResult = await this.signalService.AddSignalAsync(input);
                    if (!signalResult)
                    {
                        this.ModelState.AddModelError("Images", "Възникна проблем при обработка на вашия сигнал. Моля, опитайте онтово!");
                    }
                }
            }
            catch
            {
                this.ModelState.AddModelError("Images", "Възникна проблем при обработка на вашия сигнал. Моля, опитайте онтово!");
            }

            // Model State Validation
            if (!this.ModelState.IsValid)
            {
                // TODO: Да се изтрият снимките, ако съшествуват
                return this.View(input);
            }

            this.TempData["Message"] = "Благодарим Ви! Вашият сигнал е записан успешно и ще бъде видим на страницата възможно най-скоро.";

            return this.RedirectToAction("Index", new { page = 1, isCreateSingalContext = true });
        }
    }
}

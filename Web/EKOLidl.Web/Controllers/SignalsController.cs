namespace EKOLidl.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.Services.Interfaces;
    using EKOLidl.Services.Mapping;
    using EKOLidl.Web.ViewModels.Signals;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class SignalsController : BaseController
    {
        private readonly ISignalRepository signalRepository;
        private readonly IFileValidationService fileValidationService;
        private readonly IStorageService storageService;

        public SignalsController(ISignalRepository signalRepository, IFileValidationService fileValidationService, IStorageService storageService)
        {
            this.signalRepository = signalRepository;
            this.fileValidationService = fileValidationService;
            this.storageService = storageService;
        }

        public async Task<IActionResult> Index(int page = 1, bool isCreateSingalContext = false)
        {
            const int signalsPerPage = 12;
            IEnumerable<Signal> signals = await this.signalRepository.GetApprovedSignalsByPageAsync(page, signalsPerPage);
            int signalsCount = await this.signalRepository.GetItemsCountAsync();

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
            var result = await this.signalRepository.GetItemAsync(id);
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

            if (input.CheckedRules == false)
            {
                this.ModelState.AddModelError("CheckedRules", "Моля съгласете се с общите условия на сайта");
            }

            foreach (var image in input.Images)
            {
                // Image Validation
                var validationResult = await this.fileValidationService.CheckAsync(image);
                if (!validationResult)
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
                var imagesResult = await this.storageService.UploadFilesAsync(input.Images);

                input.Album = new Album();
                input.Album.Links = (List<string>)imagesResult;

                // TODO: Make mapping from input to signal
                Signal signal = new Signal
                {
                    Title = input.Title,
                    Description = input.Description,
                    TrashType = input.TrashType,
                    Location = input.Location,
                    Album = input.Album,
                    Reporter = new Reporter()
                    {
                        Names = input.ReporterNames,
                        Gsm = input.ReporterGsm,
                        Email = input.ReporterEmail,
                    },
                    Result = new Result()
                    {
                        Description = "Още няма добавени резултати!",
                        Album = new Album()
                        {
                            Links = new List<string>(),
                        },
                    },
                };

                if (input.TrashType.Contains(EKOLidl.Data.Models.Signals.TrashType.Other))
                {
                    input.TrashType = new List<EKOLidl.Data.Models.Signals.TrashType>();
                    input.TrashType.Add(EKOLidl.Data.Models.Signals.TrashType.Other);

                    signal.OtherTrashType = input.OtherTrashType;
                }

                await this.signalRepository.AddItemAsync(signal);
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

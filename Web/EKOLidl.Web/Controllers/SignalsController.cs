namespace EKOLidl.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.Services.Interfaces;
    using EKOLidl.Web.ViewModels.Signals;
    using Microsoft.AspNetCore.Http;
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
            int signalsCount = await this.signalRepository.GetApprovedSignalsCountAsync();

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
            Signal signal = await this.signalRepository.GetItemAsync(id);

            // Signals are public only after a moderator approves them.
            if (signal == null || !signal.IsApproved)
            {
                return this.NotFound();
            }

            return this.View(SignalDetailsViewModel.FromEntity(signal));
        }

        [Route("Signals/Create")]
        public IActionResult Create()
        {
            var viewModel = new CreateSignalInputModel
            {
                TrashType = new List<TrashType>(),
                Album = new Album()
                {
                    Links = new List<string>(),
                },
                TrashTypeItems = TrashTypeExtensions.GetSelectableItems(),
            };

            return this.View(viewModel);
        }

        [Route("Signals/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSignalInputModel input)
        {
            this.ModelState.Clear();

            input.TrashType ??= new List<TrashType>();
            input.TrashTypeItems = TrashTypeExtensions.GetSelectableItems();
            List<IFormFile> images = input.Images?.ToList() ?? new List<IFormFile>();

            if (input.TrashType.Count == 0)
            {
                this.ModelState.AddModelError("TrashType", "Моля, изберете тип на отпадъка");
            }

            if (input.CheckedRules == false)
            {
                this.ModelState.AddModelError("CheckedRules", "Моля, съгласете се с общите условия на сайта");
            }

            if (images.Count == 0)
            {
                this.ModelState.AddModelError("Images", "Моля, прикачете снимки към сигнала");
            }

            foreach (var image in images)
            {
                if (!await this.fileValidationService.CheckAsync(image))
                {
                    this.ModelState.AddModelError("Images", "Разрешени формати за изображение: .jpg, .jpeg, .png или .webp. Максимален размер до 5 MB.");
                    break;
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            List<string> uploadedLinks = new List<string>();

            try
            {
                uploadedLinks = (await this.storageService.UploadFilesAsync(images)).ToList();

                Signal signal = new Signal
                {
                    Title = input.Title,
                    Description = input.Description,
                    TrashType = input.TrashType,
                    Location = input.Location,
                    Album = new Album { Links = uploadedLinks },
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

                if (input.TrashType.Contains(TrashType.Other))
                {
                    signal.OtherTrashType = input.OtherTrashType;
                }

                await this.signalRepository.AddItemAsync(signal);
            }
            catch
            {
                await this.storageService.DeleteFilesAsync(uploadedLinks);
                this.ModelState.AddModelError("Images", "Възникна проблем при обработка на вашия сигнал. Моля, опитайте отново!");

                return this.View(input);
            }

            this.TempData["message"] = "Благодарим Ви! Вашият сигнал е записан успешно и ще бъде видим на страницата възможно най-скоро.";

            return this.RedirectToAction("Index", new { page = 1, isCreateSingalContext = true });
        }
    }
}

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
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class SignalsController : BaseController
    {
        // Five photos of up to 5 MB plus the form fields.
        private const int MaxRequestBytes = 28 * 1024 * 1024;

        private readonly ISignalRepository signalRepository;
        private readonly IFileValidationService fileValidationService;
        private readonly IStorageService storageService;

        public SignalsController(ISignalRepository signalRepository, IFileValidationService fileValidationService, IStorageService storageService)
        {
            this.signalRepository = signalRepository;
            this.fileValidationService = fileValidationService;
            this.storageService = storageService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int signalsPerPage = 12;
            IEnumerable<Signal> signals = await this.signalRepository.GetSignalsByPageAsync(true, page, signalsPerPage);
            int signalsCount = await this.signalRepository.GetSignalsCountAsync(true);

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
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> Create(CreateSignalInputModel input)
        {
            input.TrashType ??= new List<TrashType>();
            input.TrashTypeItems = TrashTypeExtensions.GetSelectableItems();
            List<IFormFile> images = input.Images?.ToList() ?? new List<IFormFile>();

            if (input.TrashType.Contains(TrashType.Other) && string.IsNullOrWhiteSpace(input.OtherTrashType))
            {
                this.ModelState.AddModelError(nameof(input.OtherTrashType), "Моля, опишете какви са другите отпадъци.");
            }

            if (!input.CheckedRules)
            {
                this.ModelState.AddModelError(nameof(input.CheckedRules), "Моля, съгласете се с общите условия на сайта.");
            }

            if (images.Count > CreateSignalInputModel.MaxImages)
            {
                this.ModelState.AddModelError(nameof(input.Images), $"Можете да прикачите до {CreateSignalInputModel.MaxImages} снимки.");
            }

            foreach (var image in images)
            {
                if (!await this.fileValidationService.CheckAsync(image))
                {
                    this.ModelState.AddModelError(nameof(input.Images), "Снимките трябва да са JPG, PNG или WebP и до 5 MB всяка.");
                    break;
                }
            }

            if (!this.ModelState.IsValid)
            {
                // Browsers clear file inputs when the form comes back, so ask for the photos again.
                if (images.Count > 0 && this.ModelState.GetFieldValidationState(nameof(input.Images)) != ModelValidationState.Invalid)
                {
                    this.ModelState.AddModelError(nameof(input.Images), "Моля, изберете снимките отново - браузърът не ги запазва след проверка.");
                }

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
                        Description = Result.NoResultsYet,
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

            return this.RedirectToAction("Index");
        }
    }
}

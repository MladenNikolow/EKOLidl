namespace EKOLidl.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.Services.Interfaces;
    using EKOLidl.Web.Infrastructure;
    using EKOLidl.Web.ViewModels.Administration;
    using EKOLidl.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.Extensions.Logging;

    [Authorize]
    [Route("admin")]
    public class AdminController : Controller
    {
        public const string LoginRateLimit = "admin-login";

        private const int PageSize = 12;
        private const int MaxPhotosPerUpload = 10;
        private const int MaxRequestBytes = 60 * 1024 * 1024;
        private const string MessageKey = "admin-message";

        private readonly IEventRepository eventRepository;
        private readonly ISignalRepository signalRepository;
        private readonly IStorageService storageService;
        private readonly IFileValidationService fileValidationService;
        private readonly AdminAuthenticator authenticator;
        private readonly ILogger<AdminController> logger;

        public AdminController(
            IEventRepository eventRepository,
            ISignalRepository signalRepository,
            IStorageService storageService,
            IFileValidationService fileValidationService,
            AdminAuthenticator authenticator,
            ILogger<AdminController> logger)
        {
            this.eventRepository = eventRepository;
            this.signalRepository = signalRepository;
            this.storageService = storageService;
            this.fileValidationService = fileValidationService;
            this.authenticator = authenticator;
            this.logger = logger;
        }

        // ── Sign in / out ──────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            if (this.User.Identity?.IsAuthenticated == true)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View(new CredentialsInputModel());
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting(LoginRateLimit)]
        public async Task<IActionResult> Login(CredentialsInputModel input, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            if (!this.authenticator.Verify(input.Username, input.Password))
            {
                this.logger.LogWarning("Failed admin sign-in from {RemoteIp}.", this.HttpContext.Connection.RemoteIpAddress);
                this.ModelState.AddModelError(string.Empty, "Грешно потребителско име или парола.");
                input.Password = null;
                return this.View(input);
            }

            Claim[] claims =
            {
                new Claim(ClaimTypes.Name, input.Username.Trim()),
                new Claim(AdminAuthenticator.SecurityStampClaim, this.authenticator.SecurityStamp),
            };
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            this.logger.LogInformation("Admin signed in from {RemoteIp}.", this.HttpContext.Connection.RemoteIpAddress);

            return this.Url.IsLocalUrl(returnUrl) ? this.LocalRedirect(returnUrl) : this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction(nameof(this.Login));
        }

        // ── Dashboard ──────────────────────────────────────────────────────
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel
            {
                EventsCount = await this.eventRepository.GetItemsCountAsync(),
                PendingSignalsCount = await this.signalRepository.GetSignalsCountAsync(false),
                ApprovedSignalsCount = await this.signalRepository.GetSignalsCountAsync(true),
                LatestPending = await this.signalRepository.GetSignalsByPageAsync(false, 1, 5),
            };

            return this.View(model);
        }

        // ── Signals ────────────────────────────────────────────────────────
        [HttpGet("signals")]
        public async Task<IActionResult> Signals(bool approved = false, int page = 1)
        {
            AdminSignalsViewModel model = new AdminSignalsViewModel
            {
                Signals = await this.signalRepository.GetSignalsByPageAsync(approved, page, PageSize),
                ShowsApproved = approved,
                PendingCount = await this.signalRepository.GetSignalsCountAsync(false),
                ApprovedCount = await this.signalRepository.GetSignalsCountAsync(true),
                Page = page,
                SignalsPerPage = PageSize,
            };

            return this.View(model);
        }

        [HttpGet("signals/{id}")]
        public async Task<IActionResult> ModerateSignal(string id)
        {
            Signal signal = await this.signalRepository.GetItemAsync(id);
            if (signal == null)
            {
                return this.NotFound();
            }

            return this.View(new AdminSignalViewModel { Signal = signal, Input = ModerateSignalInputModel.FromEntity(signal) });
        }

        [HttpPost("signals/{id}")]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> ModerateSignal(string id, [Bind(Prefix = "Input")] ModerateSignalInputModel input)
        {
            Signal signal = await this.signalRepository.GetItemAsync(id);
            if (signal == null || input.Id != id)
            {
                return this.NotFound();
            }

            List<IFormFile> newResultPhotos = await this.ValidatePhotosAsync(input.NewResultPhotos, "Input.NewResultPhotos");
            if (!this.ModelState.IsValid)
            {
                return this.View(new AdminSignalViewModel { Signal = signal, Input = input });
            }

            signal.Album ??= new Album { Links = new List<string>() };
            signal.Result ??= new Result();
            signal.Result.Album ??= new Album { Links = new List<string>() };

            List<string> removed = RemovePhotos(signal.Album, input.RemovePhotos)
                .Concat(RemovePhotos(signal.Result.Album, input.RemovePhotos))
                .ToList();
            signal.Result.Album.Links.AddRange(await this.storageService.UploadFilesAsync(newResultPhotos));
            signal.Result.Description = string.IsNullOrWhiteSpace(input.ResultDescription) ? Result.NoResultsYet : input.ResultDescription.Trim();
            signal.IsApproved = input.IsApproved;

            await this.signalRepository.UpdateItemAsync(signal.Id, signal);
            await this.storageService.DeleteFilesAsync(removed);
            this.logger.LogInformation("Signal {SignalId} moderated: approved={Approved}.", signal.Id, signal.IsApproved);

            this.TempData[MessageKey] = "Промените по сигнала са запазени.";
            return this.RedirectToAction(nameof(this.ModerateSignal), new { id });
        }

        [HttpPost("signals/{id}/approval")]
        public async Task<IActionResult> SetApproval(string id, bool approved)
        {
            Signal signal = await this.signalRepository.GetItemAsync(id);
            if (signal == null)
            {
                return this.NotFound();
            }

            signal.IsApproved = approved;
            await this.signalRepository.UpdateItemAsync(signal.Id, signal);
            this.logger.LogInformation("Signal {SignalId} approval set to {Approved}.", signal.Id, approved);

            this.TempData[MessageKey] = approved ? $"Сигналът „{signal.Title}“ е публикуван." : $"Сигналът „{signal.Title}“ е скрит от сайта.";
            return this.RedirectToAction(nameof(this.Signals), new { approved = !approved });
        }

        [HttpPost("signals/{id}/delete")]
        public async Task<IActionResult> DeleteSignal(string id)
        {
            Signal signal = await this.signalRepository.GetItemAsync(id);
            if (signal == null)
            {
                return this.NotFound();
            }

            await this.signalRepository.DeleteItemAsync(signal.Id);
            await this.storageService.DeleteFilesAsync(
                (signal.Album?.Links ?? new List<string>()).Concat(signal.Result?.Album?.Links ?? new List<string>()));
            this.logger.LogInformation("Signal {SignalId} deleted.", signal.Id);

            this.TempData[MessageKey] = $"Сигналът „{signal.Title}“ е изтрит.";
            return this.RedirectToAction(nameof(this.Signals), new { approved = signal.IsApproved });
        }

        // ── Events ─────────────────────────────────────────────────────────
        [HttpGet("events")]
        public async Task<IActionResult> Events(int page = 1)
        {
            EventsViewModel model = new EventsViewModel
            {
                Events = (await this.eventRepository.GetItemsByPageAsync(page, PageSize)).ToList(),
                Count = await this.eventRepository.GetItemsCountAsync(),
                Page = page,
                EventsPerPage = PageSize,
            };

            return this.View(model);
        }

        [HttpGet("events/{id}")]
        public async Task<IActionResult> EditEvent(string id)
        {
            Event @event = await this.eventRepository.GetItemAsync(id);
            if (@event == null)
            {
                return this.NotFound();
            }

            return this.View(EditEventInputModel.FromEntity(@event, SiteInfo.ToSofiaTime(@event.EventDate)));
        }

        [HttpPost("events/{id}")]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<IActionResult> EditEvent(string id, EditEventInputModel input)
        {
            Event @event = await this.eventRepository.GetItemAsync(id);
            if (@event == null || input.Id != id)
            {
                return this.NotFound();
            }

            List<IFormFile> newPhotos = await this.ValidatePhotosAsync(input.NewPhotos, nameof(input.NewPhotos));
            List<IFormFile> newResultPhotos = await this.ValidatePhotosAsync(input.NewResultPhotos, nameof(input.NewResultPhotos));
            if (!this.ModelState.IsValid)
            {
                input.Album = @event.Album;
                input.ResultAlbum = @event.Result?.Album;
                return this.View(input);
            }

            @event.Album ??= new Album { Links = new List<string>() };
            @event.Result ??= new Result();
            @event.Result.Album ??= new Album { Links = new List<string>() };

            List<string> removed = RemovePhotos(@event.Album, input.RemovePhotos)
                .Concat(RemovePhotos(@event.Result.Album, input.RemovePhotos))
                .ToList();
            @event.Album.Links.AddRange(await this.storageService.UploadFilesAsync(newPhotos));
            @event.Result.Album.Links.AddRange(await this.storageService.UploadFilesAsync(newResultPhotos));

            @event.Title = input.Title.Trim();
            @event.ShortDescription = input.ShortDescription.Trim();
            @event.LongDescription = input.LongDescription.Trim();
            @event.EventDate = DateTime.SpecifyKind(input.EventDate!.Value, DateTimeKind.Unspecified); // Sofia time, as entered.
            @event.Duration = input.Duration;
            @event.Result.Description = string.IsNullOrWhiteSpace(input.ResultDescription) ? Result.NoResultsYet : input.ResultDescription.Trim();

            await this.eventRepository.UpdateItemAsync(@event.Id, @event);
            await this.storageService.DeleteFilesAsync(removed);
            this.logger.LogInformation("Event {EventId} updated.", @event.Id);

            this.TempData[MessageKey] = "Събитието е запазено.";
            return this.RedirectToAction(nameof(this.EditEvent), new { id });
        }

        // ── Helpers ────────────────────────────────────────────────────────
        private static List<string> RemovePhotos(Album album, List<string> toRemove)
        {
            if (album?.Links == null || toRemove == null || toRemove.Count == 0)
            {
                return new List<string>();
            }

            List<string> removed = album.Links.Where(toRemove.Contains).ToList();
            album.Links.RemoveAll(toRemove.Contains);
            return removed;
        }

        private async Task<List<IFormFile>> ValidatePhotosAsync(IEnumerable<IFormFile> files, string key)
        {
            List<IFormFile> photos = files?.ToList() ?? new List<IFormFile>();

            if (photos.Count > MaxPhotosPerUpload)
            {
                this.ModelState.AddModelError(key, $"Можете да качите до {MaxPhotosPerUpload} снимки наведнъж.");
            }

            foreach (IFormFile photo in photos)
            {
                if (!await this.fileValidationService.CheckAsync(photo))
                {
                    this.ModelState.AddModelError(key, "Снимките трябва да са JPG, PNG или WebP и до 5 MB всяка.");
                    break;
                }
            }

            return photos;
        }
    }
}

namespace EKOLidl.Web.ViewModels.Administration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EKOLidl.Data.Models;
    using Microsoft.AspNetCore.Http;

    public class ModerateSignalInputModel
    {
        [Required]
        public string Id { get; set; }

        [Display(Name = "Публикуван на сайта")]
        public bool IsApproved { get; set; }

        [StringLength(2000, ErrorMessage = "Резултатът може да е до 2000 символа.")]
        [Display(Name = "Резултат")]
        public string ResultDescription { get; set; }

        [Display(Name = "Снимки на резултата")]
        public IEnumerable<IFormFile> NewResultPhotos { get; set; }

        /// <summary>
        /// Gets or sets URLs of photos the moderator ticked for removal.
        /// </summary>
        public List<string> RemovePhotos { get; set; } = new();

        public static ModerateSignalInputModel FromEntity(Signal signal)
        {
            return new ModerateSignalInputModel
            {
                Id = signal.Id,
                IsApproved = signal.IsApproved,
                ResultDescription = signal.Result?.Description == Result.NoResultsYet ? null : signal.Result?.Description,
            };
        }
    }
}

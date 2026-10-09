namespace EKOLidl.Web.ViewModels.Administration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EKOLidl.Data.Models;
    using Microsoft.AspNetCore.Http;

    public class EditEventInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Моля, въведете заглавие.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "Заглавието трябва да е между 3 и 120 символа.")]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Моля, въведете кратко описание.")]
        [StringLength(300, ErrorMessage = "Краткото описание може да е до 300 символа.")]
        [Display(Name = "Кратко описание")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Моля, въведете описание.")]
        [StringLength(5000, ErrorMessage = "Описанието може да е до 5000 символа.")]
        [Display(Name = "Описание")]
        public string LongDescription { get; set; }

        [Required(ErrorMessage = "Моля, въведете дата и час.")]
        [Display(Name = "Дата и час (българско време)")]
        public DateTime? EventDate { get; set; }

        [Range(0, 72, ErrorMessage = "Продължителността трябва да е между 0 и 72 часа.")]
        [Display(Name = "Продължителност (часове)")]
        public int Duration { get; set; }

        [StringLength(2000, ErrorMessage = "Резултатите може да са до 2000 символа.")]
        [Display(Name = "Резултати")]
        public string ResultDescription { get; set; }

        [Display(Name = "Нови снимки на събитието")]
        public IEnumerable<IFormFile> NewPhotos { get; set; }

        [Display(Name = "Нови снимки на резултатите")]
        public IEnumerable<IFormFile> NewResultPhotos { get; set; }

        /// <summary>
        /// Gets or sets URLs of photos the administrator ticked for removal.
        /// </summary>
        public List<string> RemovePhotos { get; set; } = new();

        /// <summary>
        /// Gets or sets the current photos (display only, refilled from the entity).
        /// </summary>
        public Album Album { get; set; }

        /// <summary>
        /// Gets or sets the current result photos (display only, refilled from the entity).
        /// </summary>
        public Album ResultAlbum { get; set; }

        public static EditEventInputModel FromEntity(Event @event, DateTime eventDateLocal)
        {
            return new EditEventInputModel
            {
                Id = @event.Id,
                Title = @event.Title,
                ShortDescription = @event.ShortDescription,
                LongDescription = @event.LongDescription,
                EventDate = eventDateLocal,
                Duration = @event.Duration,
                ResultDescription = @event.Result?.Description == Result.NoResultsYet ? null : @event.Result?.Description,
                Album = @event.Album,
                ResultAlbum = @event.Result?.Album,
            };
        }
    }
}

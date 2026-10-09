namespace EKOLidl.Web.ViewModels.Signals
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using Microsoft.AspNetCore.Http;

    public class CreateSignalInputModel
    {
        public const int MaxImages = 5;

        /// <summary>
        /// Bulgarian mobile number, with optional spaces or dashes: 0888 123 456, +359 888 123 456.
        /// Shared with the form's HTML pattern attribute, which browsers compile in Unicode-sets ("v") mode:
        /// keep it JavaScript-compatible and escape "-" inside character classes.
        /// </summary>
        public const string PhonePattern = @"^(\+359|0)[\s\-]?8[7-9]([\s\-]?\d){7}$";

        /// <summary>
        /// Letters of any script, spaces, apostrophes, dots and hyphens (Анна-Мария, O'Neil).
        /// </summary>
        public const string NamePattern = @"^[\p{L}\s'.\-]+$";

        [Required(ErrorMessage = "Моля, добавете заглавие на сигнала.")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Заглавието трябва да е между 5 и 40 символа.")]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Моля, опишете какво видяхте.")]
        [StringLength(300, MinimumLength = 20, ErrorMessage = "Описанието трябва да е между 20 и 300 символа.")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Моля, изберете поне един вид отпадъци.")]
        [Display(Name = "Вид отпадъци")]
        public List<TrashType> TrashType { get; set; }

        [StringLength(40, ErrorMessage = "Моля, опишете вида отпадъци с до 40 символа.")]
        [Display(Name = "Какви други отпадъци")]
        public string OtherTrashType { get; set; }

        [Required(ErrorMessage = "Моля, опишете къде се намира мястото.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "Местоположението трябва да е между 3 и 120 символа.")]
        [Display(Name = "Местоположение")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Моля, прикачете поне една снимка.")]
        [Display(Name = "Снимки")]
        public IEnumerable<IFormFile> Images { get; set; }

        public Album Album { get; set; }

        [Required(ErrorMessage = "Моля, въведете името си.")]
        [StringLength(80, ErrorMessage = "Името може да е до 80 символа.")]
        [RegularExpression(NamePattern, ErrorMessage = "Името може да съдържа само букви, интервали и тире.")]
        [Display(Name = "Име и фамилия")]
        public string ReporterNames { get; set; }

        [Required(ErrorMessage = "Моля, въведете телефон за връзка.")]
        [RegularExpression(PhonePattern, ErrorMessage = "Въведете мобилен номер, например 0888 123 456 или +359 888 123 456.")]
        [Display(Name = "Телефон")]
        public string ReporterGsm { get; set; }

        [Required(ErrorMessage = "Моля, въведете имейл за връзка.")]
        [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл адрес.")]
        [StringLength(120, ErrorMessage = "Имейлът може да е до 120 символа.")]
        [Display(Name = "Имейл")]
        public string ReporterEmail { get; set; }

        public bool CheckedRules { get; set; }

        public List<KeyValuePair<int, string>> TrashTypeItems { get; set; }
    }
}

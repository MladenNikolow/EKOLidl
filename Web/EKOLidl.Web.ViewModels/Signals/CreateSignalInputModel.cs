namespace EKOLidl.Web.ViewModels.Signals
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using Microsoft.AspNetCore.Http;

    public class CreateSignalInputModel
    {
        [Required(ErrorMessage = "Моля, добавете заглавие на сигнала")]
        [MinLength(5, ErrorMessage = "Дължината на заглавието трябва да е между 5 и 40 символа")]
        [MaxLength(40, ErrorMessage = "Дължината на заглавието трябва да е между 5 и 40 символа")]
        [Display(Name = "Заглавие на сигнала")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Моля, добавете описание на сигнала")]
        [MinLength(20, ErrorMessage = "Дължината на заглавието трябва да е между 20 и 300 символа")]
        [MaxLength(300, ErrorMessage = "Дължината на заглавието трябва да е между 20 и 300 символа")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Моля, посочете тип на отпадъка")]
        [Display(Name = "Тип на отпадъка")]
        public List<TrashType> TrashType { get; set; }

        public string OtherTrashType { get; set; }

        [Required(ErrorMessage = "Моля, въведете местоположението на сигнала")]
        [Display(Name = "Местоположение на замърсеният район")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Моля, прикачете снимки към сигнала")]
        [Display(Name = "Снимки на замърсеният район")]
        public IEnumerable<IFormFile> Images { get; set; }

        public Album Album { get; set; }

        [Required(ErrorMessage = "Моля добавете имена на подател")]
        [Display(Name = "Име на подател")]
        public string ReporterNames { get; set; }

        [Required(ErrorMessage = "Моля, добавете телефон за връзка")]
        [Display(Name = "Телефон за връзка")]
        [RegularExpression(@"(\+359|0)\s?8(\d{2}\s\d{3}\d{3}|[789]\d{7})$", ErrorMessage = "Моля, въведете валиден телефонен номер в един от следните формати: +359XXXXXXXXX / 0XXXXXXXXX")]
        public string ReporterGsm { get; set; }

        [Required(ErrorMessage = "Моля, добавете имейл за връзка")]
        [Display(Name = "Имейл за връзка")]
        public string ReporterEmail { get; set; }

        public List<KeyValuePair<int, string>> TrashTypeItems { get; set; }
    }
}

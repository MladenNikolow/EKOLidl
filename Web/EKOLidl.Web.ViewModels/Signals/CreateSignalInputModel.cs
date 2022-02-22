namespace EKOLidl.Web.ViewModels.Signals
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EKOLidl.Data.Models.Signals;
    using Microsoft.AspNetCore.Http;

    public class CreateSignalInputModel
    {
        [Required]
        [MinLength(5, ErrorMessage = "Дължината на заглавието трябва да е между 5 и 40 символа")]
        [MaxLength(40, ErrorMessage = "Дължината на заглавието трябва да е между 5 и 40 символа")]
        [Display(Name = "Заглавие на сигнала")]
        public string Title { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "Дължината на заглавието трябва да е между 20 и 300 символа")]
        [MaxLength(300, ErrorMessage = "Дължината на заглавието трябва да е между 20 и 300 символа")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Тип на отпадъка")]
        public List<TrashType> TrashType { get; set; }

        public string OtherTrashType { get; set; }

        [Required]
        [Display(Name = "Локация")]
        public string Location { get; set; }

        [Required]
        public IEnumerable<IFormFile> Images { get; set; }

        public List<KeyValuePair<int, string>> TrashTypeItems { get; set; }
    }
}

namespace EKOLidl.Web.ViewModels.Administration
{
    using System.ComponentModel.DataAnnotations;

    public class CredentialsInputModel
    {
        [Required(ErrorMessage = "Моля, въведете име!")]
        [MinLength(3, ErrorMessage = "Невалидно име или парола!")]
        [MaxLength(40, ErrorMessage = "Невалидно име или парола!")]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Моля, въведете парола!")]
        [MinLength(3, ErrorMessage = "Невалидно име или парола!")]
        [MaxLength(40, ErrorMessage = "Невалидно име или парола!")]
        [Display(Name = "Парола")]
        public string Password { get; set; }
    }
}

namespace EKOLidl.Web.ViewModels.Administration
{
    using System.ComponentModel.DataAnnotations;

    public class CredentialsInputModel
    {
        [Required(ErrorMessage = "Моля, въведете потребителско име.")]
        [StringLength(64, ErrorMessage = "Грешно потребителско име или парола.")]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [StringLength(256, ErrorMessage = "Грешно потребителско име или парола.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }
    }
}

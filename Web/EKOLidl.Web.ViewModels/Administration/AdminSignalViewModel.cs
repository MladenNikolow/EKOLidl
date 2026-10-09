namespace EKOLidl.Web.ViewModels.Administration
{
    using EKOLidl.Data.Models;

    /// <summary>
    /// A signal with its moderation form.
    /// </summary>
    public class AdminSignalViewModel
    {
        public Signal Signal { get; set; }

        public ModerateSignalInputModel Input { get; set; }
    }
}

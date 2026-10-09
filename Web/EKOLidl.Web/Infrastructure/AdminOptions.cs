namespace EKOLidl.Web.Infrastructure
{
    /// <summary>
    ///     The single administrator account, bound from the "Admin" configuration section
    ///     (App Service settings <c>Admin__Username</c> and <c>Admin__PasswordHash</c>).
    /// </summary>
    public class AdminOptions
    {
        public const string Section = "Admin";

        /// <summary>
        ///     Gets or sets the login name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password hash produced by <c>dotnet run --project Web/EKOLidl.Web -- --hash-password</c>.
        /// </summary>
        public string PasswordHash { get; set; }
    }
}

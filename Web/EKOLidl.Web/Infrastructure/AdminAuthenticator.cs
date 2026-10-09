namespace EKOLidl.Web.Infrastructure
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///     Checks administrator credentials against configuration. The password is only ever stored as a
    ///     <see cref="PasswordHasher{TUser}"/> hash (PBKDF2 with a per-hash salt).
    /// </summary>
    public class AdminAuthenticator
    {
        public const string SecurityStampClaim = "admin_stamp";

        private static readonly PasswordHasher<AdminOptions> Hasher = new();

        private readonly IOptionsMonitor<AdminOptions> options;

        public AdminAuthenticator(IOptionsMonitor<AdminOptions> options)
        {
            this.options = options;
        }

        /// <summary>
        ///     Gets a value indicating whether an administrator account is configured.
        /// </summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(this.options.CurrentValue.Username)
            && !string.IsNullOrWhiteSpace(this.options.CurrentValue.PasswordHash);

        /// <summary>
        ///     Gets a value that changes whenever the password hash changes. It is stored in the sign-in cookie,
        ///     so changing the password signs out every existing session.
        /// </summary>
        public string SecurityStamp
        {
            get
            {
                byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(this.options.CurrentValue.PasswordHash ?? string.Empty));
                return Convert.ToHexString(hash, 0, 16);
            }
        }

        public static string HashPassword(string password) => Hasher.HashPassword(null, password);

        public bool Verify(string username, string password)
        {
            if (!this.IsConfigured || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            AdminOptions admin = this.options.CurrentValue;
            bool usernameMatches = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(username.Trim().ToLowerInvariant()),
                Encoding.UTF8.GetBytes(admin.Username.Trim().ToLowerInvariant()));

            // Always run the hash check, so a wrong username takes as long as a wrong password.
            PasswordVerificationResult result;
            try
            {
                result = Hasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
            }
            catch (FormatException)
            {
                // A malformed hash in configuration must never let anyone in.
                return false;
            }

            return usernameMatches && result != PasswordVerificationResult.Failed;
        }
    }
}

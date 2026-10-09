namespace EKOLidl.Web.Infrastructure
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    ///     Adds security headers to every response, including a Content-Security-Policy that only allows this
    ///     site's own scripts and styles, images from Blob Storage, and the one inline script by its hash.
    /// </summary>
    public static class SecurityHeaders
    {
        /// <summary>
        ///     The only inline script on the site (rendered by _Layout): it flags that JavaScript runs.
        /// </summary>
        public const string JsFlagScript = "document.documentElement.classList.add('js');";

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, string blobOrigin, bool reportOnly)
        {
            string scriptHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(JsFlagScript)));
            string policy = string.Join(
                "; ",
                "default-src 'self'",
                $"script-src 'self' 'sha256-{scriptHash}'",
                "style-src 'self'",
                $"img-src 'self' data: blob: https://*.blob.core.windows.net {blobOrigin}".TrimEnd(),
                "font-src 'self'",
                "connect-src 'self'",
                "object-src 'none'",
                "base-uri 'self'",
                "form-action 'self'",
                "frame-ancestors 'self'");
            string policyHeader = reportOnly ? "Content-Security-Policy-Report-Only" : "Content-Security-Policy";

            return app.Use(async (context, next) =>
            {
                IHeaderDictionary headers = context.Response.Headers;
                headers[policyHeader] = policy;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["X-Frame-Options"] = "SAMEORIGIN";
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
                headers["Cross-Origin-Opener-Policy"] = "same-origin";

                await next();
            });
        }
    }
}

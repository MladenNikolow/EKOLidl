namespace EKOLidl.Web
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Unicode;
    using System.Threading.RateLimiting;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.Repositories;
    using EKOLidl.Services.Data.ServiceExtensions;
    using EKOLidl.Services.Data.Services;
    using EKOLidl.Services.Data.Services.AzureBlobStorage;
    using EKOLidl.Services.Data.Services.Interfaces;
    using EKOLidl.Web.Controllers;
    using EKOLidl.Web.Infrastructure;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.WebEncoders;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // dotnet run --project Web/EKOLidl.Web -- --hash-password
            if (args.Length > 0 && args[0] == "--hash-password")
            {
                PrintPasswordHash();
                return;
            }

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            CosmosDbSettings cosmosDbSettings = GetRequiredSection<CosmosDbSettings>(builder.Configuration, "AzureConnections:CosmosDB");
            AzureBlobStorageSettings blobStorageSettings = GetRequiredSection<AzureBlobStorageSettings>(builder.Configuration, "AzureConnections:BlobStorage");

            ConfigureServices(builder.Services, builder.Configuration, cosmosDbSettings, blobStorageSettings);

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                await DevelopmentData.InitializeAsync(app.Services, cosmosDbSettings);
            }

            Configure(app);

            await app.RunAsync();
        }

        private static void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            CosmosDbSettings cosmosDbSettings,
            AzureBlobStorageSettings blobStorageSettings)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            // Emit Cyrillic as-is instead of &#x...; entities (Razor encodes only Basic Latin by default).
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // Azure App Service terminates TLS in front of the app; trust its X-Forwarded-For (the last hop only).
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = 1;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Administrator sign-in
            services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.Section));
            services.AddSingleton<AdminAuthenticator>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/admin/login";
                    options.LogoutPath = "/admin/logout";
                    options.AccessDeniedPath = "/admin/login";
                    options.Cookie.Name = "domoreto.admin";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.IsEssential = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;

                    // Changing the password (or removing the account) signs out every existing session.
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        AdminAuthenticator authenticator = context.HttpContext.RequestServices.GetRequiredService<AdminAuthenticator>();
                        string stamp = context.Principal?.FindFirst(AdminAuthenticator.SecurityStampClaim)?.Value;
                        if (!authenticator.IsConfigured || stamp != authenticator.SecurityStamp)
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    };
                });
            services.AddAuthorization();

            // At most 5 sign-in attempts per IP address every 5 minutes.
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(AdminController.LoginRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(5), QueueLimit = 0 }));
            });

            // Azure Cosmos Db
            services.AddAzureCosmosDb(cosmosDbSettings.EndpointUrl, cosmosDbSettings.PrimaryKey, cosmosDbSettings.DatabaseName, cosmosDbSettings.Containers);

            // Azure Blob Storage
            services.AddAzureBlobStorage(blobStorageSettings);

            // Repositories
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISignalRepository, SignalRepository>();

            // Custom Services
            services.AddScoped<IFileValidationService, ImageValidationService>();
        }

        private static void Configure(WebApplication app)
        {
            app.UseForwardedHeaders();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enforced in production; report-only in development so the developer error page keeps working.
            string blobOrigin = app.Services.GetRequiredService<Azure.Storage.Blobs.BlobContainerClient>().Uri.GetLeftPart(UriPartial.Authority);
            app.UseSecurityHeaders(blobOrigin, reportOnly: app.Environment.IsDevelopment());

            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseHttpsRedirection();

            // The site is Bulgarian only: dates and numbers always use bg-BG.
            CultureInfo bulgarian = new CultureInfo("bg-BG");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(bulgarian),
                SupportedCultures = new[] { bulgarian },
                SupportedUICultures = new[] { bulgarian },
            });

            app.UseRouting();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }

        private static T GetRequiredSection<T>(IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Get<T>()
                ?? throw new InvalidOperationException(
                    $"Missing configuration section '{key}'. Set it in App Service settings, or locally in appsettings.Development.json / user-secrets (see README).");
        }

        /// <summary>
        ///     Reads a password without echoing it and prints its hash for the Admin__PasswordHash setting.
        /// </summary>
        private static void PrintPasswordHash()
        {
            Console.Error.Write("Парола за администратора: ");
            string password;

            if (Console.IsInputRedirected)
            {
                password = Console.In.ReadLine();
            }
            else
            {
                StringBuilder buffer = new StringBuilder();
                for (ConsoleKeyInfo key = Console.ReadKey(intercept: true); key.Key != ConsoleKey.Enter; key = Console.ReadKey(intercept: true))
                {
                    if (key.Key == ConsoleKey.Backspace && buffer.Length > 0)
                    {
                        buffer.Length--;
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        buffer.Append(key.KeyChar);
                    }
                }

                password = buffer.ToString();
                Console.Error.WriteLine();
            }

            if (string.IsNullOrEmpty(password) || password.Length < 12)
            {
                Console.Error.WriteLine("Паролата трябва да е поне 12 символа.");
                Environment.ExitCode = 1;
                return;
            }

            Console.WriteLine(AdminAuthenticator.HashPassword(password));
        }
    }
}

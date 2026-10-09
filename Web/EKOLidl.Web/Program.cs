namespace EKOLidl.Web
{
    using System;
    using System.Globalization;
    using System.Text.Encodings.Web;
    using System.Text.Unicode;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.Repositories;
    using EKOLidl.Services.Data.ServiceExtensions;
    using EKOLidl.Services.Data.Services;
    using EKOLidl.Services.Data.Services.AzureBlobStorage;
    using EKOLidl.Services.Data.Services.Interfaces;
    using EKOLidl.Web.Infrastructure;
    using Microsoft.AspNetCore.Builder;
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
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            CosmosDbSettings cosmosDbSettings = GetRequiredSection<CosmosDbSettings>(builder.Configuration, "AzureConnections:CosmosDB");
            AzureBlobStorageSettings blobStorageSettings = GetRequiredSection<AzureBlobStorageSettings>(builder.Configuration, "AzureConnections:BlobStorage");

            ConfigureServices(builder.Services, cosmosDbSettings, blobStorageSettings);

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
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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
    }
}

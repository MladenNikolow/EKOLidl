namespace EKOLidl.Web
{
    using System.Reflection;

    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.BlobContainerService;
    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.Repositories;
    using EKOLidl.Services.Data.ServiceExtensions;
    using EKOLidl.Services.Mapping;
    using EKOLidl.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CosmosDbSettings cosmosDbSettings = this.configuration.GetSection("Connections:CosmosDB").Get<CosmosDbSettings>();
            services = IServiceCollectionExtensions.AddCosmosDb(services, cosmosDbSettings.EndpointUrl, cosmosDbSettings.PrimaryKey, cosmosDbSettings.DatabaseName, cosmosDbSettings.Containers);

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISignalRepository, SignalRepository>();

            services.AddSingleton<IBlobContainerService>(InitializeBlobContainerService(this.configuration.GetSection("AzureStorage")));

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews(
                options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }).AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(this.configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }

        private static BlobContainerService InitializeBlobContainerService(IConfigurationSection configurationSection)
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            string containerName = configurationSection.GetSection("Container").Value;

            return new BlobContainerService(connectionString, containerName);
        }
    }
}

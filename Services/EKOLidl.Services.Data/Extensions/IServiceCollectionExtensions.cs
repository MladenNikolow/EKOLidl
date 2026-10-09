namespace EKOLidl.Services.Data.ServiceExtensions
{
    using System.Collections.Generic;

    using Azure.Storage.Blobs;
    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.CosmosDbContainerFactory;
    using EKOLidl.Services.Data.Services;
    using EKOLidl.Services.Data.Services.AzureBlobStorage;
    using EKOLidl.Services.Data.Services.Interfaces;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceCollectionExtensions
    {
        /// <summary>
        ///     Register a singleton Cosmos client and the container factory that wraps it.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="endpointUrl"></param>
        /// <param name="primaryKey"></param>
        /// <param name="databaseName"></param>
        /// <param name="containers"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureCosmosDb(
            this IServiceCollection services,
            string endpointUrl,
            string primaryKey,
            string databaseName,
            List<ContainerInfo> containers)
        {
            CosmosClient client = new CosmosClient(endpointUrl, primaryKey);
            CosmosDbContainerFactory cosmosDbClientFactory = new CosmosDbContainerFactory(client, databaseName, containers);
            services.AddSingleton(client);
            services.AddSingleton<ICosmosDbContainerFactory>(cosmosDbClientFactory);
            return services;
        }

        /// <summary>
        ///     Setup Azure Blob Storage Service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="azureBlobStorageSettings"></param>
        public static void AddAzureBlobStorage(this IServiceCollection services, AzureBlobStorageSettings azureBlobStorageSettings)
        {
            services.AddSingleton(new BlobContainerClient(azureBlobStorageSettings.ConnectionString, azureBlobStorageSettings.Container));
            services.AddScoped<IStorageService, AzureBlobStorageService>();
        }
    }
}

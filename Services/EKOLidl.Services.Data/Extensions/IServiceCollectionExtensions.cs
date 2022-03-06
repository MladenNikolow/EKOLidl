namespace EKOLidl.Services.Data.ServiceExtensions
{
    using System.Collections.Generic;

    using Azure.Storage.Blobs;
    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.CosmosDbContainerFactory;
    using EKOLidl.Services.Data.Services;
    using EKOLidl.Services.Data.Services.AzureBlobStorage;
    using EKOLidl.Services.Data.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceCollectionExtensions
    {
        /// <summary>
        ///     Register a singleton instance of Cosmos Db Container Factory, which is a wrapper for the CosmosClient.
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
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(endpointUrl, primaryKey);
            CosmosDbContainerFactory cosmosDbClientFactory = new CosmosDbContainerFactory(client, databaseName, containers);
            services.AddSingleton<ICosmosDbContainerFactory>(cosmosDbClientFactory);
            return services;
        }

        /// <summary>
        ///     Setup Azure Blob Storage Service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAzureBlobStorage(this IServiceCollection services, AzureBlobStorageSettings azureBlobStorageSettings)
        {
            services.AddScoped<IStorageService, AzureBlobStorageService>(factory =>
            {
                BlobContainerClient blobContainerClient = new BlobContainerClient(azureBlobStorageSettings.ConnectionString, azureBlobStorageSettings.Container);
                return new AzureBlobStorageService(blobContainerClient);
            });
        }
    }
}

namespace EKOLidl.Services.Data.ServiceExtensions
{
    using System.Collections.Generic;

    using EKOLidl.Services.Data.CosmosDb;
    using EKOLidl.Services.Data.CosmosDb.CosmosDbContainerFactory;
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
        public static IServiceCollection AddCosmosDb(
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
    }
}

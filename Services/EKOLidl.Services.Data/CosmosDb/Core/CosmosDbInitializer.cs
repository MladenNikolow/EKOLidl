namespace EKOLidl.Services.Data.CosmosDb
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Azure.Cosmos;

    /// <summary>
    ///     Creates the database and containers when they are missing. Meant for local development
    ///     against the Cosmos DB emulator; production containers are provisioned in Azure.
    /// </summary>
    public static class CosmosDbInitializer
    {
        public static async Task EnsureCreatedAsync(CosmosClient client, CosmosDbSettings settings)
        {
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName);

            foreach (ContainerInfo container in settings.Containers ?? new List<ContainerInfo>())
            {
                await database.Database.CreateContainerIfNotExistsAsync(container.Name, container.PartitionKey);
            }
        }
    }
}

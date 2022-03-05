using Microsoft.Azure.Cosmos;

namespace EKOLidl.Services.Data.CosmosDb
{
    public class CosmosDbContainer : ICosmosDbContainer
    {
        public CosmosDbContainer(
            CosmosClient cosmosClient,
            string databaseName,
            string containerName)
        {
            this.Container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public Container Container { get; }
    }
}

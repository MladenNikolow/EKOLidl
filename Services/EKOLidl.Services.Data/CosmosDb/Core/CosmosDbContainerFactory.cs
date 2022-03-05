namespace EKOLidl.Services.Data.CosmosDb.CosmosDbContainerFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Azure.Cosmos;

    public class CosmosDbContainerFactory : ICosmosDbContainerFactory
    {
        /// <summary> Azure Cosmos DB Client. </summary>
        private readonly CosmosClient cosmosClient;

        /// <summary> The name of the database. </summary>
        private readonly string databaseName;

        /// <summary> List with info about containers </summary>
        private readonly List<ContainerInfo> containers;

        /// <summary> Constructor. </summary>
        /// <param name="cosmosClient"></param>
        /// <param name="databaseName"></param>
        /// <param name="containers"></param>
        public CosmosDbContainerFactory(
            CosmosClient cosmosClient,
            string databaseName,
            List<ContainerInfo> containers)
        {
            this.databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            this.containers = containers ?? throw new ArgumentNullException(nameof(containers));
            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public ICosmosDbContainer GetContainer(string containerName)
        {
            if (this.containers.Where(x => x.Name == containerName) == null)
            {
                throw new ArgumentException($"Unable to find container: {containerName}");
            }

            return new CosmosDbContainer(this.cosmosClient, this.databaseName, containerName);
        }
    }
}

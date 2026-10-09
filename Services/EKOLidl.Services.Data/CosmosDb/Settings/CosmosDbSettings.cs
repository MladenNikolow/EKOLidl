namespace EKOLidl.Services.Data.CosmosDb
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using EKOLidl.Services.Data.CosmosDb.CosmosDbContainerFactory;

    public class CosmosDbSettings
    {
        /// <summary>
        ///     Gets or sets cosmosDb Account - The Azure Cosmos DB endpoint.
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        ///     Gets or sets key - The primary key for the Azure DocumentDB account.
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        ///     Gets or sets database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     Gets or sets list of containers in the database.
        /// </summary>
        public List<ContainerInfo> Containers { get; set; }
    }
}

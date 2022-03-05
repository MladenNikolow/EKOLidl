namespace EKOLidl.Services.Data.CosmosDb
{
    using Microsoft.Azure.Cosmos;

    public interface ICosmosDbContainer
    {
        /// <summary> Gets Azure Cosmos DB Container. </summary>
        Container Container { get; }
    }
}

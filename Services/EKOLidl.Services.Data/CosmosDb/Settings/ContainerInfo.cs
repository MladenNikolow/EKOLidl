namespace EKOLidl.Services.Data.CosmosDb
{
    public class ContainerInfo
    {
        /// <summary>
        ///     Gets or sets container Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets container partition Key.
        /// </summary>
        public string PartitionKey { get; set; }
    }
}

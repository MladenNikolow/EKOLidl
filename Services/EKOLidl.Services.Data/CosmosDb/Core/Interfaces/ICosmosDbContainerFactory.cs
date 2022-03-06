namespace EKOLidl.Services.Data.CosmosDb
{
    public interface ICosmosDbContainerFactory
    {
        /// <summary> Returns a CosmosDbContainer wrapper. </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        ICosmosDbContainer GetContainer(string containerName);
    }
}

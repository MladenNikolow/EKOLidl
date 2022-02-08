namespace EKOLidl.Services.Data.SignalService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using Microsoft.Azure.Cosmos;

    public class SignalService : ISignalService
    {
        private Microsoft.Azure.Cosmos.Container container;

        public SignalService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this.container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Signal>> GetSignalsAsync(string queryString)
        {
            var query = this.container.GetItemQueryIterator<Signal>(new QueryDefinition(queryString));
            List<Signal> results = new List<Signal>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Signal> GetSignalAsync(string id)
        {
            try
            {
                ItemResponse<Signal> response = await this.container.ReadItemAsync<Signal>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddSignalAsync(Signal signal)
        {
            await this.container.CreateItemAsync<Signal>(signal, new PartitionKey(signal.Id));
        }

        public async Task DeleteSignalAsync(string id)
        {
            await this.container.DeleteItemAsync<Signal>(id, new PartitionKey(id));
        }

        public async Task UpdateSignalAsync(string id, Signal signal)
        {
            await this.container.UpsertItemAsync<Signal>(signal, new PartitionKey(id));
        }
    }
}

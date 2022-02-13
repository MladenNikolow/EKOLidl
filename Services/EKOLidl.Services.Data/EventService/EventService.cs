namespace LidlProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using Microsoft.Azure.Cosmos;

    public class EventService : IEventService
    {
        private readonly Microsoft.Azure.Cosmos.Container container;

        public EventService(
          CosmosClient dbClient,
          string databaseName,
          string containerName)
        {
            this.container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddEventAsync(Event ev)
        {
            await this.container.CreateItemAsync<Event>(ev, new PartitionKey(ev.Id));
        }

        public async Task DeleteEventAsync(string id)
        {
            await this.container.DeleteItemAsync<Event>(id, new PartitionKey(id));
        }

        public async Task<Event> GetEventAsync(string id)
        {
            try
            {
                ItemResponse<Event> response = await this.container.ReadItemAsync<Event>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(string queryString)
        {
            var query = this.container.GetItemQueryIterator<Event>(new QueryDefinition(queryString));
            List<Event> results = new();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateEventAsync(string id, Event ev)
        {
            await this.container.UpsertItemAsync<Event>(ev, new PartitionKey(id));
        }
    }
}

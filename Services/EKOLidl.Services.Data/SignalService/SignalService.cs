namespace EKOLidl.Services.Data.SignalService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Web.ViewModels.Signals;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;

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

        public async Task AddSignalAsync(CreateSignalInputModel input)
        {
            var signal = new Signal
            {
                Title = input.Title,
                Description = input.Description,
                TrashType = input.TrashType,
                Location = input.Location,
                Album = input.Album,
                Reporter = new Reporter()
                {
                    Names = input.ReporterNames,
                    Gsm = input.ReporterGsm,
                    Email = input.ReporterEmail
                },
                Result = new Result()
                {
                    Description = "Още няма добавени резултати!",
                    Album = new Album()
                    {
                        Links = new List<string>(),
                    },
                },
            };

            if (input.TrashType.Contains(EKOLidl.Data.Models.Signals.TrashType.Other))
            {
                input.TrashType = new List<EKOLidl.Data.Models.Signals.TrashType>();
                input.TrashType.Add(EKOLidl.Data.Models.Signals.TrashType.Other);

                signal.OtherTrashType = input.OtherTrashType;
            }

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

        public async Task<int> GetCountAsync()
        {
            IOrderedQueryable<Signal> linqQueryable = this.container.GetItemLinqQueryable<Signal>();
            int count = await linqQueryable.CountAsync();

            return count;
        }

        public async Task<IEnumerable<Signal>> GetSignalsByPageAsync(int page, int signalsPerPage = 12)
        {
            int offset = signalsPerPage * (page - 1);

            string query = @$"select * from events e
                            order by e.created_on
                            offset {offset} limit {signalsPerPage}";

            return await this.GetSignalsAsync(query);
        }

        public async Task<IEnumerable<Signal>> GetNewestSignals(int count = 4)
        {
            string query = $@"SELECT * FROM EVENTS E
                              ORDER BY E.created_on
                              OFFSET 0 LIMIT {count}";

            return await this.GetSignalsAsync(query);
        }
    }
}

namespace EKOLidl.Services.Data.CosmosDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Common.Models;
    using EKOLidl.Data.Common.Repositories;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;

    public abstract class CosmosDbRepository<T> : EKOLidl.Data.Common.Repositories.IRepository<T>, IContainerContext<T>
        where T : BaseEntity
    {
        /// <summary>
        ///     Cosmos DB factory.
        /// </summary>
        private readonly ICosmosDbContainerFactory cosmosDbContainerFactory;

        /// <summary>
        ///     Cosmos DB container.
        /// </summary>
        private readonly Microsoft.Azure.Cosmos.Container container;

        public CosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory)
        {
            this.cosmosDbContainerFactory = cosmosDbContainerFactory ?? throw new ArgumentNullException(nameof(ICosmosDbContainerFactory));
            this.container = this.cosmosDbContainerFactory.GetContainer(this.ContainerName).Container;
        }

        /// <summary>
        ///     Gets name of the CosmosDB container.
        /// </summary>
        public abstract string ContainerName { get; }

        /// <summary>
        ///     Generate id.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract string GenerateId(T entity);

        /// <summary>
        ///     Resolve the partition key.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public abstract PartitionKey ResolvePartitionKey(string entityId);

        // Search data using SQL query string
        // This shows how to use SQL string to read data from Cosmos DB for demonstration purpose.
        // For production, try to use safer alternatives like Parameterized Query and LINQ if possible.
        // Using string can expose SQL Injection vulnerability, e.g. select * from c where c.id=1 OR 1=1. 
        // String can also be hard to work with due to special characters and spaces when advanced querying like search and pagination is required.
        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            FeedIterator<T> resultSetIterator = this.container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task AddItemAsync(T item)
        {
            item.Id = this.GenerateId(item);
            await this.container.CreateItemAsync<T>(item, this.ResolvePartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this.container.DeleteItemAsync<T>(id, this.ResolvePartitionKey(id));
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, this.ResolvePartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<int> GetItemsCountAsync()
        {
            IOrderedQueryable<T> linqQueryable = this.container.GetItemLinqQueryable<T>();
            int count = await linqQueryable.CountAsync();

            return count;
        }

        public async Task UpdateItemAsync(string id, T item)
        {
            // Update
            await this.container.UpsertItemAsync<T>(item, this.ResolvePartitionKey(id));
        }

        public async Task<IEnumerable<T>> GetItemsByPageAsync(int page, int itemsPerPage)
        {
            int offset = itemsPerPage * (page - 1);

            string query = @$"select * from {this.ContainerName} c
                            order by c.created_on
                            offset {offset} limit {itemsPerPage}";

            return await this.GetItemsAsync(query);
        }

        public async Task<IEnumerable<T>> GetNewestItems(int count = 4)
        {
            string query = $@"SELECT * FROM SIGNALS S
                              ORDER BY S.created_on
                              OFFSET 0 LIMIT {count}";

            return await this.GetItemsAsync(query);
        }
    }
}

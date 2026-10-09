namespace EKOLidl.Services.Data.CosmosDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EKOLidl.Data.Common.Models;
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
        ///     Gets the document property that lists sort by, newest first.
        /// </summary>
        protected virtual string SortProperty => "created_on";

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
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

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
            item.ModifiedOn = DateTime.UtcNow;
            await this.container.UpsertItemAsync<T>(item, this.ResolvePartitionKey(id));
        }

        public Task<IEnumerable<T>> GetItemsByPageAsync(int page, int itemsPerPage)
        {
            QueryDefinition query = new QueryDefinition(
                    $"SELECT * FROM c ORDER BY c.{this.SortProperty} DESC OFFSET @offset LIMIT @limit")
                .WithParameter("@offset", Offset(page, itemsPerPage))
                .WithParameter("@limit", itemsPerPage);

            return this.QueryAsync(query);
        }

        public Task<IEnumerable<T>> GetNewestItems(int count = 4)
        {
            QueryDefinition query = new QueryDefinition(
                    $"SELECT * FROM c ORDER BY c.{this.SortProperty} DESC OFFSET 0 LIMIT @limit")
                .WithParameter("@limit", count);

            return this.QueryAsync(query);
        }

        /// <summary>
        ///     Converts a 1-based page number to a query offset; pages below 1 are treated as the first page.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="itemsPerPage">Items per page.</param>
        /// <returns>The offset.</returns>
        protected static int Offset(int page, int itemsPerPage) => itemsPerPage * (Math.Max(page, 1) - 1);

        /// <summary>
        ///     Runs a parameterized query and reads every page of the result.
        /// </summary>
        /// <param name="query">The query, with values passed as parameters.</param>
        /// <returns>All matching items.</returns>
        protected async Task<IEnumerable<T>> QueryAsync(QueryDefinition query)
        {
            List<T> results = new List<T>();
            using FeedIterator<T> resultSetIterator = this.container.GetItemQueryIterator<T>(query);

            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        /// <summary>
        ///     Runs a parameterized <c>SELECT VALUE COUNT(1)</c> query.
        /// </summary>
        /// <param name="query">The count query.</param>
        /// <returns>The count.</returns>
        protected async Task<int> CountAsync(QueryDefinition query)
        {
            using FeedIterator<int> iterator = this.container.GetItemQueryIterator<int>(query);
            int count = 0;

            while (iterator.HasMoreResults)
            {
                FeedResponse<int> response = await iterator.ReadNextAsync();
                count += response.Sum();
            }

            return count;
        }
    }
}

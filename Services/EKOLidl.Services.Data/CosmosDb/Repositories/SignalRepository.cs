namespace EKOLidl.Services.Data.CosmosDb.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using Microsoft.Azure.Cosmos;

    public class SignalRepository : CosmosDbRepository<Signal>, ISignalRepository
    {
        public SignalRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        /// <summary>
        ///     Gets cosmosDB container name.
        /// </summary>
        public override string ContainerName { get; } = "Signals";

        /// <summary>
        ///     Generate Id.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string GenerateId(Signal entity)
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Returns the value of the partition key.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);

        public Task<IEnumerable<Signal>> GetSignalsByPageAsync(bool isApproved, int page, int itemsPerPage)
        {
            QueryDefinition query = new QueryDefinition(
                    "SELECT * FROM c WHERE (c.is_approved ?? false) = @approved ORDER BY c.created_on DESC OFFSET @offset LIMIT @limit")
                .WithParameter("@approved", isApproved)
                .WithParameter("@offset", Offset(page, itemsPerPage))
                .WithParameter("@limit", itemsPerPage);

            return this.QueryAsync(query);
        }

        public Task<int> GetSignalsCountAsync(bool isApproved)
        {
            QueryDefinition query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE (c.is_approved ?? false) = @approved")
                .WithParameter("@approved", isApproved);

            return this.CountAsync(query);
        }

        public Task<IEnumerable<Signal>> GetNewestApprovedSignalsAsync(int count)
        {
            QueryDefinition query = new QueryDefinition(
                    "SELECT * FROM c WHERE c.is_approved = true ORDER BY c.created_on DESC OFFSET 0 LIMIT @limit")
                .WithParameter("@limit", count);

            return this.QueryAsync(query);
        }
    }
}

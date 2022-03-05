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
        ///     Returns the value of the partition key
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);

        public async Task<IEnumerable<Signal>> GetApprovedSignalsByPageAsync(int page, int itemsPerPage)
        {
            int offset = itemsPerPage * (page - 1);

            string query = @$"select * from {this.ContainerName} c
                            where c.is_approved = true
                            order by c.created_on
                            offset {offset} limit {itemsPerPage}";

            return await this.GetItemsAsync(query);
        }
    }
}

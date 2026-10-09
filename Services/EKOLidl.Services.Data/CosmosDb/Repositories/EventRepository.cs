namespace EKOLidl.Services.Data.CosmosDb.Repositories
{
    using System;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using Microsoft.Azure.Cosmos;

    public class EventRepository : CosmosDbRepository<Event>, IEventRepository
    {
        public EventRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }

        /// <summary>
        ///     Gets cosmosDB container name.
        /// </summary>
        public override string ContainerName { get; } = "Events";

        /// <summary>
        ///     Gets the sort property: events are listed by when they happen, not when they were entered.
        /// </summary>
        protected override string SortProperty => "event_date";

        /// <summary>
        ///     Generate Id.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string GenerateId(Event entity)
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Returns the value of the partition key
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);
    }
}

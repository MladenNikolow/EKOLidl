namespace EKOLidl.Data.Common.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EKOLidl.Data.Common.Models;

    public interface IRepository<T>
        where T : BaseEntity
    {
        /// <summary>
        ///     Get items given a string SQL query directly.
        ///     Likely in production, you may want to use alternatives like Parameterized Query or LINQ to avoid SQL Injection and avoid having to work with strings directly.
        ///     This is kept here for demonstration purpose.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsAsync(string query);

        /// <summary>
        ///     Get the count on items that match the specification
        /// </summary>
        /// <returns></returns>
        Task<int> GetItemsCountAsync();

        /// <summary>
        ///     Get one item by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetItemAsync(string id);

        /// <summary>
        /// Add new item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddItemAsync(T item);

        /// <summary>
        /// Update item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task UpdateItemAsync(string id, T item);

        /// <summary>
        /// Delete item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteItemAsync(string id);

        /// <summary>
        /// Get list of items for the current page.
        /// </summary>
        /// <param name="page">Number of the current page.</param>
        /// <param name="itemsPerPage">How many items per page.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsByPageAsync(int page, int itemsPerPage);

        /// <summary>
        /// Get newest items by created_on.
        /// </summary>
        /// <param name="count">How many items to get.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetNewestItems(int count = 4);
    }
}

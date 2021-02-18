using System;
using System.Threading.Tasks;
using Data.Entities;

namespace API.Services
{
    public interface IEntityService<T> where T : EntityBase
    {
        /// <summary>
        /// Adds the specified entity
        /// </summary>
        /// <param name="entity">Instance of <typeparamref name="T"/></param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Deletes the specified entity
        /// </summary>
        /// <param name="entity">Instance of <typeparamref name="T"/></param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Deletes the entity with the specified id
        /// </summary>
        /// <param name="entityId">Entity id</param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task DeleteAsync(Guid entityId);

        /// <summary>
        /// Updates the entity
        /// </summary>
        /// <param name="entity">Instance of <typeparamref name="T"/></param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Retrieves the entity with the specified id. Returns null if no such entity exists
        /// </summary>
        /// <param name="entityId">Entity id</param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        Task<T> GetByIdAsync(Guid entityId);
    }
}
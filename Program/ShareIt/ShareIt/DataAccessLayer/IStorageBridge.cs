using System;
using System.Linq;

namespace DataAccessLayer
{
    public interface IStorageBridge : IDisposable
    {
        /// <summary>
        /// Fetches a single entity from the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to fetch</typeparam>
        /// <param name="id">The id of the entity you wish to fetch</param>
        /// <returns>The entity with the given ID. Throws an EntityNotFoundException if nothing is found</returns>
        /// <remarks>
        /// @pre id > 0
        /// @pre int.max > id
        /// </remarks>
        TEntity Get<TEntity>(int id) where TEntity : class, IEntityDto;

        /// <summary>
        /// Fetches entities from the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to fetch</typeparam>
        /// <returns>The entities as an IQueryable</returns>
        IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntityDto;

        /// <summary>
        /// Adds a new entity to the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to add</typeparam>
        /// <param name="entity">The entity to add to the storage</param>
        /// <remarks>
        /// @pre entity != null
        /// @pre entity.Id == 0
        /// @post entity.Id != 0
        /// </remarks>
        void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Puts the given entity to the database.
        /// This means that the currently DB stored entity will be overridden with the given entity. The match is made on ID's
        /// </summary>
        /// <typeparam name="TEntity">The entity type to update</typeparam>
        /// <param name="entity">The new version of the entity</param>
        /// <remarks>
        /// @pre entity != null
        /// @pre entity.Id > 0
        /// @pre int.MaxValue >= entity.Id
        /// </remarks>
        void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Deletes the given entity from the data
        /// </summary>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        /// <param name="entity">The entity to delete</param>
        /// <remarks>
        /// @pre entity != null
        /// @pre entity.Id > 0
        /// @pre int.MaxValue >= entity.Id
        /// </remarks>
        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Deletes the given entity from the data
        /// </summary>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        /// <param name="id">The id of the entity to delete</param>
        /// <remarks>
        /// @pre id >= 0
        /// @pre int.MaxValue > id
        /// </remarks>
        void Delete<TEntity>(int id) where TEntity : class, IEntityDto;
    }
}
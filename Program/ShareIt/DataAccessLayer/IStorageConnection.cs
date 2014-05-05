using System;
using System.Linq;

namespace DataAccessLayer
{
    /// <summary>
    /// Interface for defining storage connections
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public interface IStorageConnection : IDisposable
    {
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
        void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Puts the given entity to the database.
        /// This means that the currently DB stored entity will be overridden with the given entity. The match is made on ID's
        /// </summary>
        /// <typeparam name="TEntity">The entity type to update</typeparam>
        /// <param name="entity">The new version of the entity</param>
        void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Deletes the given entity from the data
        /// </summary>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        /// <param name="entity">The entity to delete</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto;

        /// <summary>
        /// Saves changes to the context
        /// </summary>
        /// <returns>The number of objects written to the underlying database</returns>
        int SaveChanges();
    }
}

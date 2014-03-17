using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace DataAccessLayer
{
    /// <summary>
    /// Represents an Entity framework connection
    /// </summary>
    /// <typeparam name="TContext">The context to connect to</typeparam>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public class EfStorageConnection<TContext> : IStorageConnection where TContext : IDbContext, new()
    {
        private readonly TContext _ef;

        /// <summary>
        /// Contructs an EFStorageConnection
        /// </summary>
        public EfStorageConnection()
        {
            _ef = new TContext();
        }

        /// <summary>
        /// Fetches entities from the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to fetch</typeparam>
        /// <returns>The entities as an IQueryable</returns>
        /// <remarks>
        /// </remarks>
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntityDto
        {
            return _ef.Set<TEntity>();
        }

        /// <summary>
        /// Adds a new entity to the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to add</typeparam>
        /// <param name="entity">The entity to add to the storage</param>
        /// <remarks>
        /// @pre entity.Id == 0
        /// </remarks>
        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
                if (entity.Id != 0) { throw new InternalDbException("The id was set"); }
                _ef.Entry(entity).State = EntityState.Added;
                _ef.Set<TEntity>().Add(entity);
            }

        /// <summary>
        /// Puts the given entity to the database.
        /// This means that the currently DB stored entity will be overridden with the given entity. The match is made on ID's
        /// </summary>
        /// <typeparam name="TEntity">The entity type to update</typeparam>
        /// <param name="entity">The new version of the entity</param>
        /// <remarks>
        /// </remarks>
        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            _ef.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes the given entity from the data
        /// </summary>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        /// <param name="entity">The entity to delete</param>
        /// <remarks>
        /// </remarks>
        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            _ef.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// Saves changes to the context
        /// </summary>
        /// <returns>true if entities was saved</returns>
        /// <remarks>
        /// </remarks>
        public bool SaveChanges()
        {
            try
            {
                return _ef.SaveChanges() > 0;
            }
            catch (DbEntityValidationException e)
            {
                Dispose();
                throw new InternalDbException("The entities you tried to save violated a db contraint", e);
            }
            catch (EntityException e)
            {
                throw new InternalDbException("The connection to the database failed or timed out", e);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new InternalDbException("The entity tried updated is not in the database", e);
            }
            catch (Exception e)
            {
                Dispose();
                throw new InternalDbException("The data was not saved due to an unexpected error", e);
            }
        }

        /// <summary>
        /// Disposes the current context
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void Dispose()
        {
            _ef.Dispose();
        }
    }
}

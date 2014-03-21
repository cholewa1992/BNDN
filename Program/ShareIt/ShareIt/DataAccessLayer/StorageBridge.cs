using System;
using System.Linq;

namespace DataAccessLayer
{
    /// <summary>
    /// Refined IStorageBridge implementation.
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public class StorageBridge : IStorageBridge
    {
        private readonly IStorageConnection _storageConnection;

        /// <summary>
        /// Constructs a new StorageBridgeFacade
        /// </summary>
        /// <param name="storageConnection">The storage to use</param>
        public StorageBridge(IStorageConnection storageConnection)
        {
            _storageConnection = storageConnection;
        }

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
        public TEntity Get<TEntity>(int id) where TEntity : class, IEntityDto
        {
            if (id <= 0) { throw new ArgumentException("Ids 0 or less"); } //Checks that he id is not 0 or less
            if (id > int.MaxValue) { throw new ArgumentException("Ids larger than int.MaxValue"); } //Checks the id is not more than int.MaxValue

            try { return _storageConnection.Get<TEntity>().Single(t => t.Id == id); } //Trying the fetch entity with given id
            catch (InvalidOperationException)
            { //If the entity could not be found, throws an exception to the client
                throw new InvalidOperationException("Either none or too many entities with given ID was found");
            }
        }

        /// <summary>
        /// Fetches entities from the storage
        /// </summary>
        /// <typeparam name="TEntity">The entity type to fetch</typeparam>
        /// <returns>The entities as an IQueryable</returns>
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntityDto
        {
            return _storageConnection.Get<TEntity>();
        }

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
        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            //Makes sure the entity is not null
            if (entity == null) throw new ArgumentNullException("entity");

            //Makes sure the entities id is not preset
            if (entity.Id != 0) throw new ArgumentException("Id can't be preset!");

            //Adds the entity to the context
            _storageConnection.Add(entity);

            //Saves the context
            SaveChanges();

            //Checks that the id has been set
            if (entity.Id == 0) throw new InternalDbException("Id was not set");

        }

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
        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            if (entity == null) { throw new ArgumentNullException("entity"); } //Checks that the entity is not null
            if (entity.Id <= 0) { throw new InternalDbException("Id was zero or below"); }
            if (entity.Id > int.MaxValue) { throw new InternalDbException("Id was larger than int.MaxValue"); }
            _storageConnection.Update(entity); //Updates the entity
            SaveChanges(); //Saves the changes to the context
        }

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
        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            if (entity == null) { throw new ArgumentNullException("entity"); } //Checks that the entity is not null
            if (entity.Id <= 0) throw new InternalDbException("Id was zero or below");
            if (entity.Id > int.MaxValue) throw new InternalDbException("Id was zero or below");
            _storageConnection.Delete(entity); //Deletes the entity
            SaveChanges(); //Saves the changes to the database
        }

        /// <summary>
        /// Deletes the given entity from the data
        /// </summary>
        /// <typeparam name="TEntity">The entity type to use</typeparam>
        /// <param name="id">The id of the entity to delete</param>
        /// <remarks>
        /// @pre id >= 0
        /// @pre int.MaxValue > id
        /// </remarks>
        public void Delete<TEntity>(int id) where TEntity : class, IEntityDto
        {
            if (id <= 0) { throw new ArgumentException("Ids 0 or less"); } //Checks that the id is not 0 or less
            if (id > int.MaxValue) { throw new ArgumentException("Ids larger than int.MaxValue"); } //Checks that the id is not more that int.maxValue
            Delete(Get<TEntity>(id)); //Calls Delete with entity fetched by id
        }

        /// <summary>
        /// Saves changes to the context
        /// </summary>
        protected void SaveChanges()
        {
            if (!_storageConnection.SaveChanges())
            {
                throw new ChangesWasNotSavedException("The changes in this context could not be saved!");
            }
        }

        /// <summary>
        /// Disposable methode to ensure that the bridge and its underlying storage is closed corretly 
        /// </summary>
        public void Dispose()
        {
            _storageConnection.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace DataAccessLayer
{
    public class StorageBridgeStub: IStorageBridge
    {

        private HashSet<IEntityDto> _storageStub;

        public StorageBridgeStub(HashSet<IEntityDto> stub)
        {
            _storageStub = stub;
        }

        public void Dispose()
        {
            
        }

        public TEntity Get<TEntity>(int id) where TEntity : class, IEntityDto
        {
            return Get<TEntity>().Single(t => t.Id == id);
        }

        public IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntityDto
        {
            return _storageStub.OfType<TEntity>().AsQueryable();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            _storageStub.Add(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
           
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            _storageStub.Remove(entity);
        }

        public void Delete<TEntity>(int id) where TEntity : class, IEntityDto
        {
            _storageStub.Remove(Get<TEntity>(id));
        }

        public void Delete<TEntity>(ICollection<TEntity> list) where TEntity : class, IEntityDto
        {
            
        }
    }
}
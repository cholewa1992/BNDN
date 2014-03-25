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
            return null;
        }

        public IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntityDto
        {
            return null;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            throw new System.NotImplementedException();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            throw new System.NotImplementedException();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntityDto
        {
            throw new System.NotImplementedException();
        }

        public void Delete<TEntity>(int id) where TEntity : class, IEntityDto
        {
            throw new System.NotImplementedException();
        }
    }
}
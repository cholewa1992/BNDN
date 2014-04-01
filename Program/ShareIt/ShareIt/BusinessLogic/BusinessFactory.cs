using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    class BusinessFactory : IBusinessLogicFactory 
    {
        public IUserLogic CreateUserLogic()
        {
            var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>());
            return new UserLogic(storage, new AuthLogic(storage));
        }

        public IAuthLogic CreateAuthLogic()
        {
            return new AuthLogic(new StorageBridge(new EfStorageConnection<RentIt08Entities>()));
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>());
            return new AccessRightLogic(new AuthLogic(storage), storage);
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>());
            return new DataTransferLogic(new FileStorage(), storage, new AuthLogic(storage));
        }

        public IMediaItemLogic CreateMediaItemLogic()
        {
            var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>());
            return new MediaItemLogic(storage, new AuthLogic(storage));
        }
    }
}

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
            return new UserLogic(new StorageBridge(new EfStorageConnection<RentIt08Entities>()));
        }

        public IAuthLogic CreateAuthLogic()
        {
            return new AuthLogic(new StorageBridge(new EfStorageConnection<RentIt08Entities>()));
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            throw new NotImplementedException();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>());
            return new DataTransferLogic(new FileStorage(), storage, new AuthLogic(storage));
        }

        public IMediaItemLogic CreateMediaItemLogic()
        {
            return new MediaItemLogic(new StorageBridge(new EfStorageConnection<RentIt08Entities>()));
        }
    }
}

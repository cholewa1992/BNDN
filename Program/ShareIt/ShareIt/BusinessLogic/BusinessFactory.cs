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
            return new UserLogic(new StorageBridge(new EfStorageConnection<BNDNEntities>()));
        }

        public IAuthLogic CreateAuthLogic()
        {
            return new AuthLogic(new StorageBridge(new EfStorageConnection<BNDNEntities>()));
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            throw new NotImplementedException();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            return new DataTransferLogic(new FileStorage(), new StorageBridge(new EfStorageConnection<BNDNEntities>()));
        }

        public IMediaItemLogic CreateMediaItemLogic()
        {
            return new MediaItemLogic(new StorageBridge(new EfStorageConnection<BNDNEntities>()));
        }
    }
}

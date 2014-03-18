using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    class BusinessFactory : IBusinessLogicFactory 
    {
        public IUserLogic CreateUserLogic()
        {
            throw new NotImplementedException();
        }

        public IAuthLogic CreateAuthLogic()
        {
            return new AuthLogic();
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            throw new NotImplementedException();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            throw new NotImplementedException();
        }

        public IMediaItemLogic CreateMediaItemLogic()
        {
            return new MediaItemLogic();
        }
    }
}

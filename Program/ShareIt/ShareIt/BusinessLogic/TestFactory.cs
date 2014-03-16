using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    class TestFactory : IBusinessLogicFactory
    {
        public IUserLogic CreateUserLogic()
        {
            throw new NotImplementedException();
        }

        public IAuthLogic CreateAuthLogic()
        {
            throw new NotImplementedException();
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            throw new NotImplementedException();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            throw new NotImplementedException();
        }
    }
}

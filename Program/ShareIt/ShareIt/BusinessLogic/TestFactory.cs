using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Stub;

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
            return new AuthLogicStub();
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            throw new NotImplementedException();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            return new DataTransferLogicStub();
        }
    }
}

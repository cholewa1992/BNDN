using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Stub;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class is a concrete implementation of an abstract factory (IBusinessLogicFactory).
    /// It creates instances of the different logic classes (AuthLogic, UserLogic, 
    /// AccessRightLogic, DataTransferLogic and MediaItemLogic) used for testing.
    /// </summary>
    internal class TestFactory : IBusinessLogicFactory
    {
        public IUserLogic CreateUserLogic()
        {
            return new UserLogicStub();
        }

        public IAuthLogic CreateAuthLogic()
        {
            return new AuthLogicStub();
        }

        public IAccessRightLogic CreateAccessRightLogic()
        {
            return new AccessRightLogicStub();
        }

        public IDataTransferLogic CreateDataTransferLogic()
        {
            return new DataTransferLogicStub();
        }

        public IMediaItemLogic CreateMediaItemLogic()
        {
            return new MediaItemLogicStub();
        }
    }
}

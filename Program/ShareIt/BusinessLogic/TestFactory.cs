﻿using System;
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
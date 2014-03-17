using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public interface IBusinessLogicFactory
    {
        IUserLogic CreateUserLogic();
        IAuthLogic CreateAuthLogic();
        IAccessRightLogic CreateAccessRightLogic();
        IDataTransferLogic CreateDataTransferLogic();
        IMediaItemLogic CreateMediaItemLogic();
    }
}

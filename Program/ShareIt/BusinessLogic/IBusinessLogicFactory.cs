using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This interface is an abstract factory interface.
    /// It specifies what types of logic the concrete factories must be able to create.
    /// </summary>
    public interface IBusinessLogicFactory
    {
        IUserLogic CreateUserLogic();
        IAuthLogic CreateAuthLogic();
        IAccessRightLogic CreateAccessRightLogic();
        IDataTransferLogic CreateDataTransferLogic();
        IMediaItemLogic CreateMediaItemLogic();
    }
}
